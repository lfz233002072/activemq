using System;
using System.Threading;
using System.Windows.Forms;
using Lfz.AutoUpdater.Config;
using Lfz.AutoUpdater.Logging;
using Lfz.AutoUpdater.Service;
using Lfz.AutoUpdater.Utitlies;

namespace Lfz.AutoUpdater
{
    public partial class FrmDownloadProgress : Form
    {
        private readonly AutoUpdaterInfo _updaterInfo;
        private bool _isFinished = false;
        private bool _success = false;
        private readonly ILogger _logger;
        private readonly AutoUpdaterService _autoUpdaterService;

        public FrmDownloadProgress()
            : this(AutoUpdaterInfo.Current)
        {
        }

        public FrmDownloadProgress(AutoUpdaterInfo updaterInfo)
        {
            InitializeComponent();
            _updaterInfo = updaterInfo;
            _autoUpdaterService = new AutoUpdaterService();
            _autoUpdaterService.CompletedCallBack += AutoUpdaterServiceOnCompletedCallBack;
            _autoUpdaterService.DownloadProgressChanged += AutoUpdaterServiceOnDownloadProgressChanged;
            _autoUpdaterService.DownloadCompleted += AutoUpdaterServiceOnDownloadCompleted;
            _logger = LoggerFactory.GetLog(RunConfig.Current.LoggerType);
        }


        #region 窗体事件
         

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        { 
            if (!_isFinished && (DialogResult.No ==
                     MessageBox.Show("自动升级中，你是否取消升级？", "拓创自动升级程序", MessageBoxButtons.YesNo, MessageBoxIcon.Question)))
            {
                e.Cancel = true;
                return;
            }
            else
            { 
                _autoUpdaterService.CancelAsync();
            }
            this.DialogResult = this._success ? DialogResult.OK : DialogResult.Cancel;
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            this.panel2.Visible = false;
            this.lbFileName.Text = "名称：" + _updaterInfo.ExcuteFileName;
            this.lbFileName2.Text = this.lbFileName.Text;
            ThreadPool.QueueUserWorkItem(new WaitCallback(this.ProcDownload));
        }

        private void OnCancel(object sender, EventArgs e)
        {
            ShowErrorAndRestartApplication("取消升级");
        }

        #endregion

        #region 自动更新事件处理

        private void AutoUpdaterServiceOnDownloadCompleted(string filename)
        {
            if (this.InvokeRequired)
            {
                Action<string> cb = AutoUpdaterServiceOnDownloadCompleted;
                this.Invoke(cb, new object[] { filename });
            }
            else
            {
                EnableTimer();
            }
        }

        private void AutoUpdaterServiceOnDownloadProgressChanged(long bytesReceived, long totalBytesToReceive, int progressPercentage)
        {
            if (this.progressBarCurrent.InvokeRequired)
            {
                AutoUpdaterService.DownloadProgressChangedCallBack cb = AutoUpdaterServiceOnDownloadProgressChanged;
                this.Invoke(cb, new object[] { (int)bytesReceived, (int)totalBytesToReceive, progressPercentage });
            }
            else
            {
                string unit = "Kb";
                double count = (totalBytesToReceive / 1024);
                double rece = (bytesReceived / 1024);
                if (count > 2048)
                {
                    unit = "M";
                    count = TypeParse.Round2(count / 1024);
                    rece = rece / 1024;
                }
                if (progressPercentage == 100)
                {
                    EnableTimer();
                }
                this.lbTotalKb.Text = TypeParse.RoundString2(count) + unit;
                this.lbRece.Text = TypeParse.RoundString2(rece) + unit;
                this.lbTotal2.Text = lbTotalKb.Text;
                this.progressBarCurrent.Value = (int)progressPercentage;
            }
        }

        private void AutoUpdaterServiceOnCompletedCallBack(string message, bool success)
        {
            if (this.InvokeRequired)
            {
                AutoUpdaterService.AutoUpdaterCompletedCallBack cb = AutoUpdaterServiceOnCompletedCallBack;
                this.Invoke(cb, new object[] { message, success });
            }
            else
            {
                if (!string.IsNullOrEmpty(message))
                {
                    ShowErrorAndRestartApplication(message);
                }
                else if (success)
                {
                    _logger.Information("更新成功");
                    MessageBox.Show("更新成功");
                }
                _success = success;
                this._isFinished = true;
                this.DialogResult = success ? DialogResult.OK : DialogResult.Cancel;
                this.Close();
            }
        }

        #endregion

        #region The method and event

        private void ProcDownload(object o)
        {
            _autoUpdaterService.Excute(_updaterInfo, false);
        }

        private void ShowErrorAndRestartApplication(string message)
        {
            MessageBox.Show(message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            _logger.Error(message + ".ShowErrorAndRestartApplication");
            AppUnitity.Restart(_updaterInfo.ExcuteFileName, _updaterInfo.IsWindowService);
        }

        #endregion

        private void EnableTimer()
        {
            this.timer1.Enabled = true;
            this.panel2.Visible = true;
        }

        private int i = 1;
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (i >= 100)
                i = 1;
            this.progressBar2.Value = i;
        }
    }
}
