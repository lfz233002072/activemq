// /*======================================================================
// *
// *        Copyright (C)  1996-2012  lfz    
// *        All rights reserved
// *
// *        Filename :AutoUpdateCommandVistor.cs
// *        DESCRIPTION :
// *
// *        Created By 林芳崽 at 2016-01-27 16:42
// *        https://git.oschina.net/lfz
// *
// *======================================================================*/

using System;
using Lfz.AutoUpdater.Config;
using Lfz.Logging;
using Lfz.MqListener.Mq;
using Lfz.MqListener.Service;
using Lfz.MqListener.Shared.App;
using Lfz.MqListener.Shared.Enums;
using Lfz.MqListener.Shared.Models;
using Lfz.Rest;
using Lfz.Utitlies;

namespace Lfz.MqListener.MqVistor.PushMessage
{
    public class AutoUpdateCommandVistor : IMqCommandTopicVistor
    {
        private readonly ILogger _logger;
        private readonly IConfigCenterMqListenerService _listenerService;
        public AutoUpdateCommandVistor(IConfigCenterMqListenerService listenerService)
        {
            _listenerService = listenerService;
            _logger = LoggerFactory.GetLog();
        }

        public void Vistor(TopicName topicName, MqCommandInfo commandInfo)
        {
            if (topicName != TopicName.PushMessage && commandInfo.MessageType != NMSMessageType.AutoUpdateInfo)
            {
                return;
            }
            var data = JsonUtils.Deserialize<MqAutoUpdateInfo>(commandInfo.Body);
            if (data == null)
            {
                _logger.Error("自动更新数据包无效 Body:" + commandInfo.Body);
                return;
            }
            if (data.AppType != AppType.MqListener)
            {

                _logger.Error("自动更新AppType无效:" + data.AppType.ToString() + " Body:" + commandInfo.Body);
                return;
            }
            var clientId = ProcessLockHelper.GetProcessLockId();
            if (!string.Equals(clientId, data.AppId, StringComparison.OrdinalIgnoreCase))
            {
                _logger.Error("自动更新数据包客户ID不匹配" + data.AppId + " Body:" + commandInfo.Body);
                return;
            }
            AutoUpdaterInfo info = AutoUpdaterInfo.Current;
            UpdateListener(info, data);
        }

        private void UpdateListener(AutoUpdaterInfo info, MqAutoUpdateInfo data)
        {
            if (data.ForceUpdate)
                info.Version = string.Empty;
            info.NewVersion = data.Version;
            if (!info.HasNewVersion())
            {
                _listenerService.UpdateStatus(data.AppId, AppStatus.Running);
                _logger.Error("本地已经是最新版本包");
                //已经处理
                return;
            }
            if (string.IsNullOrEmpty(info.ExcuteFileName))
            {
                info.ExcuteFileName = "TCSoft.MqListener.exe";
            }
            if (string.IsNullOrEmpty(info.AppName))
            {
                info.AppName = "拓创消息调度程序";
            }
            info.AppType = (int)AppType.MqListener;
            info.LastUpdateTime = data.PublishTime;
            info.DownloadUrl = data.Url;
            info.Save();
            _listenerService.Deploy(data.AppId); 
            AppUnitity.RunAutoUpdaterProcess(info.ExcuteFileName); 
        }
 
    }
}