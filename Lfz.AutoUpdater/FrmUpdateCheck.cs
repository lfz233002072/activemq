using System;
using System.Windows.Forms;
using Lfz.AutoUpdater.Config;
using Lfz.AutoUpdater.Logging;
using Lfz.AutoUpdater.Utitlies;

namespace Lfz.AutoUpdater
{
    public partial class FrmUpdateCheck : Form
    {
        private AutoUpdaterInfo _autoUpdaterInfo;
        private readonly ILogger _logger;
        public FrmUpdateCheck()
        {
            InitializeComponent();
            _logger = LoggerFactory.GetLog(RunConfig.Current.LoggerType);
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_autoUpdaterInfo.DownloadUrl))
            {
                _logger.Error("FrmUpdateCheck.BtnUpdate_Click.待下载文件无效");
                MessageBox.Show("待下载文件无效");
                if (_autoUpdaterInfo.IsWindowService)
                    AppUnitity.Restart(_autoUpdaterInfo.ExcuteFileName,_autoUpdaterInfo.IsWindowService);
                return;
            }

            if (MessageBox.Show("升级系统要求先关闭程序，你是否确定升级系统？", "拓创自动升级程序", MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                DialogResult.No)
            {
                return;
            }
            AppUnitity.KillOrStop(_autoUpdaterInfo.ExcuteFileName, _autoUpdaterInfo.IsWindowService);
            FrmDownloadProgress progress = new FrmDownloadProgress(_autoUpdaterInfo);
            var result = progress.ShowDialog(this);
            if (result == DialogResult.OK || result == DialogResult.Yes)
            {
                AppUnitity.Restart(_autoUpdaterInfo.ExcuteFileName, _autoUpdaterInfo.IsWindowService);
            }
            else
            {
                if (_autoUpdaterInfo.IsWindowService)
                    AppUnitity.Restart(_autoUpdaterInfo.ExcuteFileName, _autoUpdaterInfo.IsWindowService);
                this.Close();
            }
        }

        private void BtnCheckVersion_Click(object sender, EventArgs e)
        {
            try
            {
                var url = string.Format("{0}/api/CheckUpdate", CommonUnitity.ApiDomainUrl);
                var result = ApiHttpClient.GetByApiResult<ApiResult<MqAutoUpdateInfo>>(url);
                var info = AutoUpdaterInfo.Current;
                if (result != null && result.Data != null && !string.IsNullOrEmpty(result.Data.Url))
                {
                    var data = result.Data;
                    info.NewVersion = data.Version;
                    info.LastUpdateTime = data.PublishTime;
                    if (!info.HasNewVersion())
                    {
                        //已经处理
                        this.BtnUpdate.Visible = false;
                        _logger.Information(string.Format("BtnCheckVersion_Click.暂无更新 result.Data.Url={0}", result.Data.Url));
                        MessageBox.Show("暂无更新");
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
                    info.AppType = data.AppType;
                    info.LastUpdateTime = data.PublishTime;
                    info.DownloadUrl = data.Url;
                    info.Save( );
                    this.BtnUpdate.Visible = true;
                    MessageBox.Show("有新版本更新");
                    _logger.Information("BtnCheckVersion_Click.有新版本更新");
                }
                else
                {
                    MessageBox.Show("暂无更新");
                    _logger.Information("BtnCheckVersion_Click.暂无更新 result.Data.Url=null");
                }
                this.lbAppName.Text = info.AppName;
                this.lbNewVersion.Text = info.NewVersion;
                this.lbVersion.Text = info.Version;
                _autoUpdaterInfo = info;
            }
            catch (Exception exception)
            {
                _logger.Error(exception, "BtnCheckVersion_Click");
                MessageBox.Show(exception.Message);
            }
        }

        private void FrmUpdateCheck_Load(object sender, EventArgs e)
        {
            try
            {
                var info = AutoUpdaterInfo.Current;
                this.lbAppName.Text = info.AppName;
                this.lbNewVersion.Text = info.NewVersion;
                this.lbVersion.Text = info.Version;
                this.BtnUpdate.Visible = info.HasNewVersion();
                _autoUpdaterInfo = info;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }


        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        class MqAutoUpdateInfo
        {
            /// <summary>
            /// 最新版本号
            /// </summary>
            public string Version { get; set; }

            /// <summary>
            /// 更新包下载地址
            /// </summary>
            public string Url { get; set; }


            /// <summary>
            /// 接收的AppId
            /// </summary>
            public string AppId { get; set; }

            /// <summary>
            /// 发布时间
            /// </summary>
            public DateTime PublishTime { get; set; }

            /// <summary>
            /// 作者
            /// </summary>
            public string Author { get; set; }

            /// <summary>
            /// 说明
            /// </summary>
            public string Remark { get; set; }

            /// <summary>
            /// app类型
            /// </summary>
            public int AppType { get; set; }

            /// <summary>
            /// 强制更新为指定版本
            /// </summary>
            public bool ForceUpdate { get; set; }

        }
    }
}
