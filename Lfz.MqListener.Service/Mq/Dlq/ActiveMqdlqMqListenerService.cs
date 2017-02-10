using System;
using System.Runtime.CompilerServices;
using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Apache.NMS.ActiveMQ.Commands;
using Lfz.Logging;
using Lfz.Mq;
using Lfz.MqListener.Shared.Models;
using Lfz.Services;
using Lfz.Utitlies;

namespace Lfz.MqListener.Mq.Dlq
{
    /// <summary>
    /// 
    /// </summary>
    public class ActiveMqdlqMqListenerService : ServiceBase
    {
        private readonly MqInstanceInfo _configInfo;
        private readonly ILogger _logger;
        private IConnection _connection;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="configInfo"></param> 
        public ActiveMqdlqMqListenerService(MqInstanceInfo configInfo)
        {
            _logger = LoggerFactory.GetLog();
            ClientId = ProcessLockHelper.GetProcessLockId() + "_MqId:" + configInfo.MqInstanceId;
            ServiceName = "Mq监听服务" + ClientId;
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
                CreateConsumer(_connection);
                _logger.Debug(string.Format("创建DLQ消息连接{0}:{1}", _configInfo.IpAddress, _configInfo.Port));
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


        #region 消费者监听处理事件

        private void CreateConsumer(IConnection connection)
        {
            //通过连接创建一个会话
            ISession session = connection.CreateSession();
            //通过会话创建一个消费者，这里就是Queue这种会话类型的监听参数设置 
            IMessageConsumer consumer = session.CreateConsumer(new ActiveMQQueue("ActiveMQ.DLQ"));

            //注册监听事件
            consumer.Listener += message => { ConsumerOnListener( message); };
            _logger.Debug(string.Format("消费者创建：{0} IP:{1}:{2} ", "ActiveMQ.DLQ", _configInfo.IpAddress, _configInfo.Port));
        }

        private void ConsumerOnListener( IMessage message)
        {
            try
            {
                ActiveMQTextMessage msg = (ActiveMQTextMessage)message;
                var client = GetClientId(msg);
                if (client == null || client == Guid.Empty)
                {
                    _logger.Error("消息来源无效，直接抛弃");
                    return;
                }
                using (ISession session = msg.Connection.CreateSession())
                {
                    IMessageProducer producer = session.CreateProducer(new ActiveMQQueue(GetQueueName(QuqueName.StoreReportData)));
                    //创建一个发送的消息对象
                    ITextMessage newmessage = producer.CreateTextMessage(msg.Text);
                    newmessage.NMSType = message.NMSType;
                    if (msg.Properties != null)
                        foreach (var pair in msg.Properties.Keys)
                        {
                            var key = pair.ToString();
                            if (key != "dlqDeliveryFailureCause" && key != "originalExpiration")
                            {
                                newmessage.Properties.SetString(key, msg.Properties.GetString(key));
                            }
                        }
                    producer.Send(newmessage, MsgDeliveryMode.Persistent, MsgPriority.Low, TimeSpan.FromHours(1));
                    _logger.Debug("重新发生消息");
                } 
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "消费者监听事件处理错误.ConsumerOnListener");
            }
        }

        private Guid? GetClientId(ActiveMQTextMessage message)
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

      
        #endregion
         

    }
}
