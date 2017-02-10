using System.Threading;
using Lfz.AutoUpdater.Config;
using Lfz.AutoUpdater.Logging;
using Lfz.AutoUpdater.Utitlies;

namespace Lfz.AutoUpdater.Service
{
    /// <summary>
    /// 
    /// </summary>
    public class AutoUpdateBackgroupService
    {
        private readonly AutoUpdaterInfo _updaterInfo;
        private bool _success = false;
        private bool _hasCompleted = false;
        private object locakobj = new object();
        private readonly AutoUpdaterService _autoUpdaterService;
        private readonly ILogger _logger;
        public bool HasCompleted
        {
            get
            {
                bool temp;
                lock (locakobj)
                {
                    temp = _hasCompleted;
                }
                return temp;
            }
            set
            {
                lock (locakobj)
                {
                    _hasCompleted = value;
                }
            }
        }


        public AutoUpdateBackgroupService()
            : this(AutoUpdaterInfo.Current)
        {
        }

        public AutoUpdateBackgroupService(AutoUpdaterInfo updaterInfo)
        {
            _updaterInfo = updaterInfo;
            _autoUpdaterService = new AutoUpdaterService();
            _autoUpdaterService.CompletedCallBack += AutoUpdaterServiceOnCompletedCallBack;
            _autoUpdaterService.DownloadProgressChanged += AutoUpdaterServiceOnDownloadProgressChanged;
            _autoUpdaterService.DownloadCompleted += AutoUpdaterServiceOnDownloadCompleted;
            _logger = LoggerFactory.GetLog(RunConfig.Current.LoggerType);
        }

        public void Run()
        {
            _logger.Information("自动更新启动");
            ThreadPool.QueueUserWorkItem(new WaitCallback(this.ProcDownload));
        }
        private void ProcDownload(object o)
        {
            _autoUpdaterService.Excute(_updaterInfo, false);
        }

        #region 自动更新事件处理

        private void AutoUpdaterServiceOnDownloadCompleted(string filename)
        {
        }

        private void AutoUpdaterServiceOnDownloadProgressChanged(long bytesReceived, long totalBytesToReceive, int progressPercentage)
        {

        }

        private void AutoUpdaterServiceOnCompletedCallBack(string message, bool success)
        {
            this._success = success;
            if (!string.IsNullOrEmpty(message))
                _logger.Error(message);
            else
                _logger.Information("自动更新结束");
            HasCompleted = true;
        }

        #endregion

        public static void RunAutoUpdate()
        {
            var updateInfo = AutoUpdaterInfo.Current;
            if (!updateInfo.HasNewVersion() ||
                string.IsNullOrWhiteSpace(updateInfo.DownloadUrl))
            {

                LoggerFactory.GetLog().Error("下载文件无效或当前版本已经是最新版本！"); 
                return;
            }
            AppUnitity.KillOrStop(updateInfo.ExcuteFileName, updateInfo.IsWindowService);
            AutoUpdateBackgroupService service = new AutoUpdateBackgroupService(updateInfo);
            service.Run();
            while (!service.HasCompleted)
            {
                Thread.Sleep(1000);
            }
            LoggerFactory.GetLog().Error("重启应用程序！");
            AppUnitity.Restart(updateInfo.ExcuteFileName, updateInfo.IsWindowService);
        }
    }
}