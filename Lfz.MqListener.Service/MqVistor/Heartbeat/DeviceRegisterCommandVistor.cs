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

using Lfz.MqListener.Access;
using Lfz.MqListener.Mq;
using Lfz.MqListener.Shared.App;
using Lfz.MqListener.Shared.Models;
using Lfz.Rest;
using Lfz.Utitlies;

namespace Lfz.MqListener.MqVistor.Heartbeat
{
    public class DeviceRegisterCommandVistor : IMqCommandQuqueVistor
    {
        private readonly DeviceAccess _deviceAccess;

        public DeviceRegisterCommandVistor()
        {
            _deviceAccess = new DeviceAccess(DbConfigHelper.GetConfig());
        }

        public void Vistor(QuqueName queueName, MqCommandInfo commandInfo)
        {
            if (queueName != QuqueName.BusinessProcessing
                && commandInfo.MessageType != NMSMessageType.None
                && !commandInfo.Properties.ContainsKey(MqConsts.MqDevice)) return;
            var deviceType = TypeParse.StrToInt(commandInfo.Properties[MqConsts.MqAppType]);
            if (deviceType == 0)
            {
                //不是有效的app类型
                return;
            }
            var data = JsonUtils.Deserialize<MqDeviceRegisterInfo>(commandInfo.Body);
            if (data == null) return;
            //App心跳
            //_deviceAccess.Exists(string.Format("StoreId='{0}' AND ", commandInfo.StoreId));
        }
    }
}