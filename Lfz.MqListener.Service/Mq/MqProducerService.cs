using System;
using System.Collections.Generic;
using Apache.NMS;
using Lfz.Logging;
using Lfz.Mq;
using Lfz.Mq.ActiveMQ;
using Lfz.MqListener.Shared.Models;
using Lfz.Rest;

namespace Lfz.MqListener.Mq
{
    /// <summary>
    /// 
    /// </summary>
    public class MqProducerService : IMqProducerService
    {
        private readonly IMqConfigService _clusterService;
        private readonly ActiveMQPoolConnectionManager _poolConnectionManager;
        private readonly ILogger _logger;

        /// <summary>
        /// 
        /// </summary> 
        public MqProducerService(IMqConfigService clusterService, ActiveMQPoolConnectionManager poolConnectionManager)
        {
            _logger = LoggerFactory.GetLog();
            _clusterService = clusterService;
            _poolConnectionManager = poolConnectionManager;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isTopic"></param>
        /// <param name="clientId"></param>
        /// <param name="command"></param>
        /// <param name="content"></param>
        /// <param name="timeSpan"></param>
        /// <param name="propertydic"></param>
        /// <returns></returns>
        private bool SendMessage(bool isTopic, Guid clientId, QuqueName command, TopicName topicName, object content,
            TimeSpan timeSpan, NMSMessageType? messageType = null, Dictionary<string, string> propertydic = null)
        {
            var mqClientId = clientId.ToString().ToLower();
            if (propertydic == null) propertydic = new Dictionary<string, string>();
            //设置消息对象的属性，这个很重要哦，是Queue的过滤条件，也是P2P消息的唯一指定属性 
            if (propertydic.ContainsKey(MqConsts.ClientId))
                propertydic[MqConsts.ClientId] = mqClientId;
            else
            {
                propertydic.Add(MqConsts.ClientId, mqClientId);
            }
            var json = "";
            try
            {
                if (content != null)
                {
                    if (!(content is string))
                        json = JsonUtils.SerializeObject(content);
                    else
                        json = content as string;
                }
                int mqInstanceId = 0;
                var connection = _poolConnectionManager.GetPoolConnection(clientId, out mqInstanceId);
                //通过连接创建Session会话
                using (ISession session = connection.CreateSession())
                {
                    //通过会话创建生产者，方法里面new出来的是MQ中的Queue
                    IMessageProducer prod = isTopic ? session.CreateProducer(new Apache.NMS.ActiveMQ.Commands.ActiveMQTopic(GetTopicName(topicName))) :
                        session.CreateProducer(new Apache.NMS.ActiveMQ.Commands.ActiveMQQueue(GetQueueName(command)));
                    //创建一个发送的消息对象
                    ITextMessage message = prod.CreateTextMessage(json);
                    if (messageType.HasValue && messageType.Value != NMSMessageType.None)
                        message.NMSType = messageType.ToString();
                    foreach (var pair in propertydic)
                    {
                        if (string.IsNullOrEmpty(pair.Key) || string.IsNullOrEmpty(pair.Value)
                            )
                            continue;
                        message.Properties.SetString(pair.Key, pair.Value);
                    }
                    //生产者把消息发送出去，几个枚举参数MsgDeliveryMode是否长链，MsgPriority消息优先级别，发送最小单位，当然还有其他重载
                    if (isTopic)
                        prod.Send(message, MsgDeliveryMode.Persistent, MsgPriority.Normal, timeSpan);
                    else
                        prod.Send(message, GetMsgDeliveryMode(command), GetMsgPriority(command), timeSpan);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, string.Format("{0}SendMessage isTopic:{1} storeId:{2} command:{3} json:{4}", this.GetType(), isTopic, clientId, command, json));
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientId"></param> 
        /// <param name="command"></param>
        /// <param name="content"></param>
        /// <param name="timeSpan"></param>
        /// <param name="propertydic"></param>
        public bool SendQueueMessage(Guid clientId, QuqueName command, object content, TimeSpan timeSpan, NMSMessageType? messageType = null, Dictionary<string, string> propertydic = null)
        {
            return SendMessage(false, clientId, command, TopicName.PushMessage, content, timeSpan, messageType, propertydic);
        }

        /// <summary>
        /// 某人消息队列有效期1天
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="command"></param>
        /// <param name="content"></param> 
        public bool SendQueueMessage(Guid clientId, QuqueName command, object content, NMSMessageType? messageType = null)
        {
            return SendQueueMessage(clientId, command, content, TimeSpan.FromDays(1), messageType);
        }

        /// <summary>
        /// 某人消息主题有效期7天
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="topic"></param>
        /// <param name="content"></param> 
        public bool SendTopicMessage(Guid clientId, TopicName topic, object content, NMSMessageType? messageType = null)
        {
            return SendTopicMessage(clientId, topic, content, TimeSpan.FromDays(7), messageType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="topic"></param>
        /// <param name="content"></param>
        /// <param name="timeSpan"></param>
        /// <param name="propertydic"></param>
        public bool SendTopicMessage(Guid clientId, TopicName topic, object content, TimeSpan timeSpan, NMSMessageType? messageType = null,
            Dictionary<string, string> propertydic = null)
        {
            return SendMessage(true, clientId, QuqueName.BusinessProcessing, TopicName.PushMessage, content, timeSpan, messageType, propertydic);
        }

        #region 获取属性


        /// <summary>
        /// app为消息生产者
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetQueueName(QuqueName name)
        {
            return "tcsoft.app." + name.ToString().ToLower();
        }

        /// <summary>
        /// app为消息生产者
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetTopicName(TopicName name)
        {
            return "tcsoft.app." + name.ToString().ToLower();
        }
        private MsgPriority GetMsgPriority(QuqueName command)
        {
            MsgPriority priority = MsgPriority.Normal;
            switch (command)
            {
                case QuqueName.BusinessProcessing:
                    priority = MsgPriority.High;
                    break;
                case QuqueName.ClientHeart:
                case QuqueName.StoreReportData:
                    priority = MsgPriority.Normal;
                    break;
            }
            return priority;
        }
        private MsgDeliveryMode GetMsgDeliveryMode(QuqueName command)
        {
            MsgDeliveryMode mode = MsgDeliveryMode.NonPersistent;
            switch (command)
            {
                case QuqueName.BusinessProcessing:
                    mode = MsgDeliveryMode.Persistent;
                    break;
            }
            return mode;
        }
        #endregion
    }
}
