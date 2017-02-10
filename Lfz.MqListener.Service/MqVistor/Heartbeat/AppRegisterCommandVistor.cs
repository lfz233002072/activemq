// /*======================================================================
// *
// *        Copyright (C)  1996-2012  lfz    
// *        All rights reserved
// *
// *        Filename :HeartbeatCommandVistor.cs
// *        DESCRIPTION :
// *
// *        Created By 林芳崽 at 2016-01-27 16:01
// *        https://git.oschina.net/lfz
// *
// *======================================================================*/

using Lfz.MqListener.Mq;
using Lfz.MqListener.Service;
using Lfz.MqListener.Shared.App;
using Lfz.MqListener.Shared.Enums;
using Lfz.MqListener.Shared.Models;
using Lfz.Rest;
using Lfz.Utitlies;

namespace Lfz.MqListener.MqVistor.Heartbeat
{
    public class AppRegisterCommandVistor : IMqCommandQuqueVistor
    {
        private readonly IConfigCenterMqListenerService _listenerService;

        public AppRegisterCommandVistor(IConfigCenterMqListenerService listenerService)
        {
            _listenerService = listenerService;  
        }

        public void Vistor(QuqueName queueName, MqCommandInfo commandInfo)
        {
            if (queueName != QuqueName.BusinessProcessing
                && commandInfo.MessageType != NMSMessageType.None
                && !commandInfo.Properties.ContainsKey(MqConsts.MqAppType)) return;
            var appType = Utils.GetEnum<AppType>(commandInfo.Properties[MqConsts.MqAppType]);
            if (appType == AppType.Empty)
            {
                //不是有效的app类型
                return;
            }
            var data = JsonUtils.Deserialize<MqAppRegisterInfo>(commandInfo.Body);
            if (data == null) return;
            //App心跳
            _listenerService.Initialize(commandInfo.StoreId, appType, data);
        }
    }
}