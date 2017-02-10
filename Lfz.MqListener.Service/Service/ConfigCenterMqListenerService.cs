using System;
using Lfz.AutoUpdater.Config;
using Lfz.Logging;
using Lfz.MqListener.Access;
using Lfz.MqListener.Shared.App;
using Lfz.MqListener.Shared.Enums;

namespace Lfz.MqListener.Service
{
    /// <summary>
    /// 
    /// </summary>
    public class ConfigCenterMqListenerService : IConfigCenterMqListenerService
    {
        private readonly AppBasicInfoAccess _access;
        private readonly ILogger _logger;
        private AppBasicInfo _appinfo;

        /// <summary>
        /// 
        /// </summary>
        public ConfigCenterMqListenerService()
        {
            _access = new AppBasicInfoAccess(DbConfigHelper.GetConfig());
            _logger = LoggerFactory.GetLog();
        }

        public void MqListenerInitialize(string clientid)
        {
            try
            {
                _appinfo = _access.GetOrRegisterListener(clientid);
                var versionInfo = AutoUpdaterInfo.Current;
                if (!string.Equals(versionInfo.Version, _appinfo.RunVersion, StringComparison.OrdinalIgnoreCase))
                {
                    _access.UpdateVersion(_appinfo.Id, versionInfo.Version);
                }
                var newStatus = _access.Transform(_appinfo.Status);
                if (newStatus == AppStatus.Starting)
                {
                    _access.UpdateStatus(_appinfo.Id, newStatus);
                }
                _logger.Debug(string.Format("初始化成功，当前运行版本：{0}", _appinfo.RunVersion));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "CheckVersion");
            }
        }

        public void Initialize(Guid storeId, AppType appType, MqAppRegisterInfo info)
        {
            var data = _access.GetOrRegister(storeId, appType, info.AppId, info.ComputerName,
                info.IpAddress,
                info.MacAddress,
                info.ProcessDirectory,
                info.Version);
            if (!string.Equals(info.Version, data.RunVersion, StringComparison.OrdinalIgnoreCase))
            {
                _access.UpdateVersion(data.Id, info.Version);
                _appinfo.RunVersion = info.Version;
            }
            var newStatus = _access.Transform(data.Status);
            if (newStatus == AppStatus.Starting)
            {
                _access.UpdateStatus(data.Id, newStatus);
                _appinfo.Status = newStatus;
            }
            _logger.Information("ConfigCenterMqListenerService.初始化成功");
        }

        /// <summary>
        /// 消息调度程序心跳
        /// </summary>
        /// <param name="clientid"></param>
        /// <param name="totalCount"></param>
        /// <param name="totalMsgSize"></param>
        /// <returns></returns>
        public bool MqListenerHeartbeat(string clientid, int totalCount, long totalMsgSize)
        {
            if (_appinfo == null)
                _appinfo = _access.GetOrRegisterListener(clientid);
            if (_appinfo.Status == AppStatus.Running)
            {
                _access.UpdateLastHeartTime(_appinfo.Id);
                _appinfo.LastHeartTime =DateTime.Now;
                return true;
            }
            var newStatus = _access.Transform(_appinfo.Status);
            if (newStatus == AppStatus.Running || newStatus == AppStatus.Starting)
            {
                _access.Online(_appinfo.Id);
                _appinfo.Status=AppStatus.Running;
                return true;
            }
            _logger.Error("MqListenerHeartbeat未做任何处理 status:" + newStatus);
            return true;
        }

        /// <summary>
        /// 其它app心跳
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="appType"></param>
        /// <param name="beatinfo"></param>
        /// <returns></returns>
        public bool Heartbeat(Guid storeId, AppType appType, MqHeartbeatInfo beatinfo)
        {
            var data = _access.GetOrRegisterListener(beatinfo.AppId);
            if (data.Status == AppStatus.Running)
            {
                _access.UpdateLastHeartTime(data.Id);
                return true;
            }
            var newStatus = _access.Transform(data.Status);
            if (newStatus == AppStatus.Running || data.Status == AppStatus.Starting)
            {
                _access.Online(data.Id);
                return true;
            }
            _logger.Error("Heartbeat");
            return true;
        }

        public void VerifyListenStatus(AppType appType)
        {
            //30分钟内都没有收到心跳处理的数据，直接踢下线
            _access.VerifyListenStatus(appType);
        }

        public void DownLown(string clientid)
        {
            var tempId = _access.GetId(clientid);
            if (_appinfo != null && clientid == _appinfo.AppId) _appinfo = null;
            _access.UpdateStatus(tempId, AppStatus.Stoped);
            _logger.Trace(string.Format("设备下线处理 AppId;{0}", clientid));

        }

        public void Deploy(string clientid)
        {
            var temp = _access.GetOrRegisterListener(clientid);
            if (temp == null) return;
            if (_appinfo != null && clientid == _appinfo.AppId) _appinfo = null;
            _access.Deploy(temp.Id);
        }

        public void UpdateStatus(string clientid, AppStatus status)
        {
            var temp = _access.GetOrRegisterListener(clientid);
            if (temp == null) return;
            if (_appinfo != null && clientid == _appinfo.AppId) _appinfo = null;
            _access.UpdateStatus(temp.Id, status);
        }
    }
}