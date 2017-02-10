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

using Lfz.Logging;
using Lfz.MqListener.Mq;
using Lfz.MqListener.Shared.Models;

namespace Lfz.MqListener.MqVistor.Heartbeat
{
    public class DeviceHeatbeatCommandVistor : IMqCommandQuqueVistor
    { 

        public DeviceHeatbeatCommandVistor()
        { 
        }

        public void Vistor(QuqueName queueName, MqCommandInfo commandInfo)
        {
            if (queueName != QuqueName.ClientHeart
                && commandInfo.MessageType != NMSMessageType.Heart
                && !commandInfo.Properties.ContainsKey(MqConsts.MqDevice)) return;
            var deviceType = commandInfo.Properties[MqConsts.MqDevice];
            LoggerFactory.GetLog().Error(string.Format("{0}设备心跳包处理", deviceType));

        }
    }
}