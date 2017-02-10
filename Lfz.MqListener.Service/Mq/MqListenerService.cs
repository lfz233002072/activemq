using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Apache.NMS.ActiveMQ.Commands;
using Lfz.Logging;
using Lfz.Mq;
using Lfz.MqListener.Shared.Models;
using Lfz.Services;
using Lfz.Utitlies;

namespace Lfz.MqListener.Mq
{
    /// <summary>
    /// 
    /// </summary>
    public class MqListenerService : ServiceBase
    {
        private readonly string _appId;
        private readonly MqInstanceInfo _configInfo;
        private readonly ILogger _logger;
        private IConnection _connection;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="configInfo"></param> 
        public MqListenerService(string appId, MqInstanceInfo configInfo)
        {
            _logger = LoggerFactory.GetLog();
            ClientId = ProcessLockHelper.GetProcessLockId() + "_MqId:" + configInfo.MqInstanceId;
            ServiceName = "Mq监听服务" + ClientId;
            _appId = appId;
            _configInfo = configInfo;
            Started += OnStarted;
            Stoping += OnStoping;
        }

        /// <summary>
        /// 更新配置信息
        /// </summary>
        /// <param name="configInfo"></param>
        public void UpdateConfig(MqInstanceInfo configInfo)
        {
            bool isReflesh = _configInfo.Port != configInfo.Port
                               || _configInfo.MqttPort != configInfo.MqttPort
                               || !string.Equals(_configInfo.IpAddress, configInfo.IpAddress, StringComparison.OrdinalIgnoreCase)
                               || !string.Equals(_configInfo.AccessPassword, configInfo.AccessPassword)
                               || !string.Equals(_configInfo.AccessUsername, configInfo.AccessUsername);
            if (isReflesh)
            {
                _configInfo.IpAddress = configInfo.IpAddress;
                _configInfo.Port = configInfo.Port;
                _configInfo.AccessPassword = configInfo.AccessPassword;
                _configInfo.AccessUsername = configInfo.AccessUsername;
                _configInfo.MqttPort = configInfo.MqttPort;
            }
            _configInfo.ExpiredTime = configInfo.ExpiredTime;
            _configInfo.DelFalg = configInfo.DelFalg;

            if (isReflesh) Restart();
        }

        private void OnStoping()
        {
            TryClose();
        }

        private void OnStarted()
        {
            if (string.IsNullOrEmpty(_configInfo.IpAddress))
            {
                Status = ServiceStatus.UnStarted;
                return;
            }
            try
            {
                var connectionFactory = new ConnectionFactory(string.Format("tcp://{0}:{1}", _configInfo.IpAddress, _configInfo.Port))
                   {
                       UserName = _configInfo.AccessUsername,
                       Password = _configInfo.AccessPassword
                   };
                //通过工厂构建连接
                _connection = connectionFactory.CreateConnection();
                //这个是连接的客户端名称标识
                _connection.ClientId = ClientId;
                //启动连接，监听的话要主动启动连接
                _connection.ConnectionInterruptedListener += ConnectionOnConnectionInterruptedListener;
                _connection.ConnectionResumedListener += ConnectionOnConnectionResumedListener;
                _connection.ExceptionListener += ConnectionOnExceptionListener;
                _connection.Start();
                CreateConsumer(_connection, QuqueName.BusinessProcessing);
                CreateConsumer(_connection, QuqueName.ClientHeart);
                CreateConsumer(_connection, QuqueName.StoreReportData);
                CreateTopicConsumer(_connection, TopicName.PushMessage);
                CreateAppTopicConsumer(_connection, TopicName.PushMessage);
                _logger.Debug(string.Format("创建消息连接{0}:{1}", _configInfo.IpAddress, _configInfo.Port));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, this.ServiceName + ".OnStarted");
                Status = ServiceStatus.UnStarted;
                TryClose();
            }
        }

        /// <summary>
        /// 消费者客ID
        /// </summary>
        public string ClientId { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool ConnectionCheck()
        {
            if (_configInfo.DelFalg && _configInfo.ExpiredTime < DateTime.Now)
            {
                //如果该配置已经过期，那么直接删除
                return false;
            }
            try
            {
                if (_connection == null) return false;
                if (!_connection.IsStarted)
                {
                    _connection.Start();
                }
                if (_connection is Connection)
                {
                    Connection connection = _connection as Connection;
                    if (connection.FirstFailureError != null || connection.TransportFailed)
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, this.GetType().Name + ".ConnectionCheck");
            }
            return false;
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        private void TryClose()
        {
            if (_connection != null)
            {
                _connection.Stop();
                _connection.Close();
                _connection.Dispose();
                _connection = null;
            }
        }

        /// <summary>
        /// 消息分派程序
        /// </summary>
        public event Action<QuqueName, MqCommandInfo> QuqueDispatch;

        /// <summary>
        /// 
        /// </summary>
        public event Action<TopicName, MqCommandInfo> TopicDispatch;

        #region 消费者监听处理事件

        private void CreateConsumer(IConnection connection, QuqueName ququeName)
        {
            //通过连接创建一个会话
            ISession session = connection.CreateSession();
            //通过会话创建一个消费者，这里就是Queue这种会话类型的监听参数设置
            string whereClause = string.Format("{0}='{1}' OR {2}='0'", MqConsts.MqInstanceId, _configInfo.MqInstanceId, MqConsts.MqInstanceAll);
            IMessageConsumer consumer = session.CreateConsumer(new ActiveMQQueue(GetQueueName(ququeName)), whereClause); 
            //注册监听事件
            consumer.Listener += message => { ConsumerOnListener(ququeName, message); };
            _logger.Debug(string.Format("消费者创建：{0} IP:{1}:{2} where:{3}", ququeName, _configInfo.IpAddress, _configInfo.Port, whereClause));
        }

        private void CreateTopicConsumer(IConnection connection, TopicName topicName)
        {
            //通过连接创建一个会话
            ISession session = connection.CreateSession();
            //通过会话创建一个消费者，这里就是Queue这种会话类型的监听参数设置
            string whereClause = string.Format("{0}='{1}'OR {2}='0'", MqConsts.MqInstanceId, _configInfo.MqInstanceId, MqConsts.MqInstanceAll);
            IMessageConsumer consumer = session.CreateConsumer(new ActiveMQTopic(GetTopicName(topicName)), whereClause);
            //注册监听事件
            consumer.Listener += message => { ConsumerOnListener(topicName, message); };
            _logger.Debug(string.Format("主题消费者创建：{0} IP:{1}:{2}  where:{3}", topicName, _configInfo.IpAddress, _configInfo.Port, whereClause));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="topicName"></param>
        private void CreateAppTopicConsumer(IConnection connection, TopicName topicName)
        {
            //通过连接创建一个会话
            ISession session = connection.CreateSession();
            //通过会话创建一个消费者，这里就是Queue这种会话类型的监听参数设置
            string whereClause = string.Format("{0}='{1}'", MqConsts.ClientId, TypeParse.StrToGuid(_appId).ToString().ToLower());
            IMessageConsumer consumer = session.CreateConsumer(new ActiveMQTopic(GetAppTopicName(topicName)), whereClause);
            //注册监听事件
            consumer.Listener += message => { ConsumerOnListener(topicName, message); };
            _logger.Debug(string.Format("Listener主题消费者创建：{0} IP:{1}:{2}  where:{3}", topicName, _configInfo.IpAddress, _configInfo.Port, whereClause));
        }

        private void ConsumerOnListener(QuqueName ququeName, IMessage message)
        {
            try
            {
                string msgContext = "";
                if (message is ActiveMQBytesMessage)
                {
                    bool isZipCompress = false;
                    if (message.Properties.Contains(MqConsts.IsZipCompress))
                        isZipCompress = message.Properties.GetBool(MqConsts.IsZipCompress);
                    ActiveMQBytesMessage msg = (ActiveMQBytesMessage)message;
                    if (isZipCompress)
                        msgContext = StringZipHelper.GZipDecompress(msg.Content);
                    else
                        msgContext = System.Text.Encoding.UTF8.GetString(msg.Content);
                }
                else if (message is ActiveMQTextMessage)
                {
                    ActiveMQTextMessage msg = (ActiveMQTextMessage)message;
                    msgContext = msg.Text;
                }
                else
                {
                    _logger.Error("消费者监听事件处理错误.暂时只接受ActiveMQBytesMessage、ActiveMQTextMessage消息体");
                }
                var clientId = GetClientId(message)  ;
                if (clientId == Guid.Empty || clientId == null)
                { 
                    _logger.Error(string.Format("消息无效，没有匹配的门店与命令号 "));
                    return;
                }
                var msgType = message.NMSType;
                NMSMessageType messageType = NMSMessageType.None;
                if (!string.IsNullOrEmpty(msgType))
                {
                    messageType = Utils.GetEnum<NMSMessageType>(msgType);
                }
                MqCommandInfo commandInfo = new MqCommandInfo()
                {
                    StoreId = clientId.Value,
                    Body = msgContext,
                    Length = msgContext.Length,
                    MessageType = messageType,
                    NMSMessageId = message.NMSMessageId,
                    MqInstancId = _configInfo.MqInstanceId,
                    ClientId = this.ClientId,
                    Properties = new Dictionary<string, string>()
                };
                if (message.Properties != null)
                    foreach (var key in message.Properties.Keys)
                    {
                        if (key == null) continue;
                        var keystr = key.ToString();
                        if (!commandInfo.Properties.ContainsKey(keystr))
                            commandInfo.Properties.Add(keystr, message.Properties.GetString(keystr));
                    }
                OnQuqueDispatch(ququeName, commandInfo);
                if (commandInfo.ExcuteCount == 0)
                    _logger.Error(string.Format("消息未找到处理程序 类型： {0}大小： {1}", commandInfo.MessageType, commandInfo.Length));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "消费者监听事件处理错误.ConsumerOnListener");
            }
        }

        private void ConsumerOnListener(TopicName topicName, IMessage message)
        {
            try
            {
                ActiveMQTextMessage msg = (ActiveMQTextMessage)message;
                var clientId = GetClientId(msg);
                if (clientId == null || clientId == Guid.Empty)
                {
                    _logger.Error(string.Format("topicName:{0} ConsumerOnListener无效客户端，数据直接抛弃 IP:{1}:{2}", topicName, _configInfo.IpAddress, _configInfo.Port));
                    return;
                }
                var msgType = message.NMSType;
                NMSMessageType messageType = NMSMessageType.None;
                if (!string.IsNullOrEmpty(msgType))
                {
                    messageType = Utils.GetEnum<NMSMessageType>(msgType);
                }
                MqCommandInfo commandInfo = new MqCommandInfo()
                {
                    StoreId = clientId.Value,
                    Body = msg.Text,
                    Length = msg.Size(),
                    MessageType = messageType,
                    RawNMSType = msgType,
                    NMSMessageId = msg.NMSMessageId,
                    MqInstancId = _configInfo.MqInstanceId,
                    ClientId = this.ClientId,
                    Properties = new Dictionary<string, string>(),
                };
                if (msg.Properties != null)
                    foreach (var key in msg.Properties.Keys)
                    {
                        if (key == null) continue;
                        var keystr = key.ToString();
                        if (!commandInfo.Properties.ContainsKey(keystr))
                            commandInfo.Properties.Add(keystr, msg.Properties.GetString(keystr));
                    }
                OnTopicDispatch(topicName, commandInfo);
                if (commandInfo.ExcuteCount == 0)
                    _logger.Error(string.Format("消息未找到处理程序 类型： {0}大小： {1}", commandInfo.MessageType, commandInfo.Length));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "主题消费者监听事件处理.ConsumerOnListener");
            }
        }

        private Guid? GetClientId(IMessage message)
        {
            if (message.Properties == null || !message.Properties.Contains(MqConsts.ClientId))
            {
                return null;
            }
            return TypeParse.StrToGuid(message.Properties.GetString(MqConsts.ClientId));
        }

        #endregion

        #region 连接监听事件

        private void ConnectionOnExceptionListener(Exception exception)
        {
            TryClose();
        }

        private void ConnectionOnConnectionInterruptedListener()
        {
            _logger.Trace("ConnectionOnConnectionInterruptedListener");

        }

        private void ConnectionOnConnectionResumedListener()
        {
            _logger.Trace("ConnectionOnConnectionResumedListener");
        }

        #endregion

        #region MyRegion

        /// <summary>
        /// 客户端为消息生成者
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetQueueName(QuqueName name)
        {
            return "tcsoft.client." + name.ToString().ToLower();
        }

        /// <summary>
        /// 客户端为消息生成者
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetTopicName(TopicName name)
        {
            return "tcsoft.client." + name.ToString().ToLower();
        }

        /// <summary>
        /// 客户端为消息生成者
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetAppTopicName(TopicName name)
        {
            return "tcsoft.app." + name.ToString().ToLower();
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        protected virtual void OnQuqueDispatch(QuqueName arg1, MqCommandInfo arg2)
        {
            var handler = QuqueDispatch;
            if (handler != null) handler(arg1, arg2);
            else
            {
                _logger.Error("未找到队列消息分配处理程序");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        protected virtual void OnTopicDispatch(TopicName arg1, MqCommandInfo arg2)
        {
            var handler = TopicDispatch;
            if (handler != null) handler(arg1, arg2);
            else
            {
                _logger.Error("未找到主题消息分配处理程序");
            }
        }

    }
}
