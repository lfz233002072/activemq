using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using Lfz.AutoUpdater.Config;
using Lfz.AutoUpdater.Logging;

namespace Lfz.AutoUpdater.Service
{
    /// <summary>
    /// 
    /// </summary>
    public class AutoUpdaterService
    {
        private readonly WebClient _webClient;
        private readonly ILogger _logger;
        /// <summary>
        /// 
        /// </summary>
        public event AutoUpdaterCompletedCallBack CompletedCallBack;
        /// <summary>
        /// 
        /// </summary>
        public event DownloadProgressChangedCallBack DownloadProgressChanged;
        /// <summary>
        /// 
        /// </summary>
        public event Action<string> DownloadCompleted;

        /// <summary>
        /// 
        /// </summary>
        public AutoUpdaterService()
        {
            _webClient = new WebClient();
            _logger = LoggerFactory.GetLog(RunConfig.Current.LoggerType);
        }

        ~AutoUpdaterService()
        {
            _webClient.Dispose();
        }

        #region 下载处理

        public void CancelAsync()
        {
            _webClient.CancelAsync();
        }

        public void Excute(AutoUpdaterInfo info, bool redownLoad = false)
        {
            try
            {

                DownLoad(info, redownLoad);
            }
            catch (Exception ex)
            {
                OnCompletedCallBack(ex.Message, false);
            }
        }

        private void DownLoad(AutoUpdaterInfo info, bool redownLoad = false)
        {
            if (string.IsNullOrEmpty(info.DownloadUrl))
            {
                OnCompletedCallBack("下载文件无效", false);
                return;
            }
            var filename = Path.GetFileName(info.DownloadUrl);
            var downloadFolder = CommonUnitity.GetDownLoadFolder();
            var fullname = Path.Combine(downloadFolder, filename);
            var subFolderList = Directory.EnumerateDirectories(downloadFolder);
            foreach (var tempFolder in subFolderList)
                Directory.Delete(tempFolder, true);
            if (File.Exists(fullname))
            {
                //如果已经存在该文件，且文件版本号和新文件版本号一致，那么直接跳过
                if (redownLoad)
                {
                    //要求重新下载
                    File.Delete(fullname);
                    CommonUnitity.ClearFolder(downloadFolder);
                }
                else
                {
                    //如果文件已经存在
                    if (DownloadProgressChanged != null)
                        DownloadProgressChanged(1, 1, 100);
                    Do(new DownloadUserToken() { Client = _webClient, FileName = fullname, ExcuteFileName = info.ExcuteFileName });
                    return;
                }
            }
            _webClient.DownloadFileCompleted += OnDownloadFileCompleted;
            _webClient.DownloadProgressChanged += OnDownloadProgressChanged;
            DownloadUserToken userToken = new DownloadUserToken() { Client = _webClient, FileName = fullname, ExcuteFileName = info.ExcuteFileName };
            _webClient.DownloadFileAsync(new Uri(info.DownloadUrl), fullname, userToken);
        }

        private void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (DownloadProgressChanged != null)
                DownloadProgressChanged(e.BytesReceived, e.TotalBytesToReceive, e.ProgressPercentage);
            if (e.UserState is DownloadUserToken)
            {
                var userToken = ((DownloadUserToken)e.UserState);
                userToken.BytesReceived = e.BytesReceived;
                userToken.TotalBytesToReceive = e.TotalBytesToReceive;
                userToken.ProgressPercentage = e.ProgressPercentage;
            }
        }


        private void OnDownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                OnCompletedCallBack("下载已经被取消", false);
                _logger.Error("下载已经被取消");
                return;
            }
            else if (e.Error != null)
            {
                OnCompletedCallBack(e.Error.Message, false);
                _logger.Error(e.Error, "下载错误");
                return;
            }
            if (e.UserState is DownloadUserToken)
            {
                var userToken = ((DownloadUserToken)e.UserState);
                if (DownloadProgressChanged != null)
                    DownloadProgressChanged(userToken.BytesReceived, userToken.TotalBytesToReceive, userToken.ProgressPercentage);
                if (DownloadCompleted != null)
                    DownloadCompleted(userToken.FileName);
                Do(userToken);
            }
            else
            {
                _logger.Error("无效的下载信息");
                OnCompletedCallBack("无效的下载信息", false);
            }
        }

        #endregion

        #region 升级处理

        private void OnCompletedCallBack(string message, bool success)
        {
            if (CompletedCallBack != null)
                CompletedCallBack(message, success);
        }

        private void Do(DownloadUserToken args)
        {
            bool hasBackup = false;
            var baseDirectory = CommonUnitity.GetSystemBinUrl();
            var backupdir = CommonUnitity.GetBackupFolder();
            try
            {
                if (!File.Exists(args.FileName))
                {
                    _logger.Error("升级错误" + args.FileName);
                    OnCompletedCallBack("下载文件失败", false);
                    return;
                }
                //解压
                var unzipFolder = CommonUnitity.GetUnzipFolder(args.FileName);
                var autoupdaterFiles = Unzip(unzipFolder, args.FileName);
                var downloadBinDirectory = GetDownloadBinDirectory(autoupdaterFiles);
                CommonUnitity.ClearFolder(backupdir);
                hasBackup = true;
                //备份待升级文档
                Backup(backupdir, baseDirectory, downloadBinDirectory, autoupdaterFiles);
                //更新指定文件
                MoveToBaseDirectory(baseDirectory, downloadBinDirectory, autoupdaterFiles);
                var info = AutoUpdaterInfo.Current;
                info.Version = info.NewVersion;
                info.Save();
                //升级成功
                OnCompletedCallBack(string.Empty, true);
                Directory.Delete(unzipFolder, true);
            }
            catch (Exception ex)
            {
                if (hasBackup)
                {
                    Restore(backupdir, baseDirectory);
                }
                _logger.Error(ex, "升级错误" + args.FileName);
                OnCompletedCallBack(ex.Message, false);
                return;
            }
        }

        private List<string> Unzip(string unzipFolder, string filename)
        {
            //如果目录存在，那么删除指定目录
            if (Directory.Exists(unzipFolder))
                Directory.Delete(unzipFolder, true);
            //解压文件到该目录中
            ZipHandler.UnZipFile(filename, unzipFolder);
            var files = CommonUnitity.GetAllFiles(unzipFolder);
            return files;
        }


        /// <summary>
        /// 如果当前执行目录有同名文件，那么先备份文件
        /// </summary>
        /// <param name="baseDirectory"></param>
        /// <param name="autoupdaterFiles"></param>
        private void Backup(string backupdir, string baseDirectory, string downloadBinDirectory, List<string> autoupdaterFiles)
        {
            foreach (var file in autoupdaterFiles)
            {
                if (!file.StartsWith(downloadBinDirectory, StringComparison.OrdinalIgnoreCase)
                    && file.Length > (downloadBinDirectory.Length + 1))
                    continue;
                //相对根目录文件路径
                var relativeFile = file.Substring(downloadBinDirectory.Length + 1);
                var currentFile = Path.Combine(baseDirectory, relativeFile);
                //判断文件是否需要备份
                if (string.Equals(currentFile, file, StringComparison.OrdinalIgnoreCase)) continue;
                var destFile = Path.Combine(backupdir, relativeFile);
                var destDir = Path.GetDirectoryName(destFile);
                if (!Directory.Exists(destDir))
                {
                    Directory.CreateDirectory(destDir);
                }
                if (File.Exists(currentFile))
                {
                    File.Move(currentFile, Path.Combine(backupdir, relativeFile));
                }
            }
        }

        private string GetDownloadBinDirectory(List<string> autoupdaterFiles)
        {
            var downLoadFolder = CommonUnitity.GetDownLoadFolder();
            //根据文件长度从小大大排序
            autoupdaterFiles = autoupdaterFiles.OrderBy(x => x.Length).ToList();
            foreach (var file in autoupdaterFiles)
            {
                var filename = Path.GetFileNameWithoutExtension(file);
                if (string.Equals(filename, "readme", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(filename, "说明", StringComparison.OrdinalIgnoreCase)
                    )
                {
                    return Path.GetDirectoryName(file);
                }
            }
            return downLoadFolder;
        }

        /// <summary>
        /// 更新文件移动到执行程序跟目录中
        /// </summary>
        /// <param name="baseDirectory"></param>
        /// <param name="downloadBinDirectory"></param>
        /// <param name="autoupdaterFiles"></param>
        private void MoveToBaseDirectory(string baseDirectory, string downloadBinDirectory, List<string> autoupdaterFiles)
        {
            foreach (var file in autoupdaterFiles)
            {
                if (!file.StartsWith(downloadBinDirectory, StringComparison.OrdinalIgnoreCase)
                    && file.Length > (downloadBinDirectory.Length + 1))
                    continue;
                var tempfile = file.Substring(downloadBinDirectory.Length + 1);
                var destFile = Path.Combine(baseDirectory, tempfile);
                var destDir = Path.GetDirectoryName(destFile);
                if (!Directory.Exists(destDir))
                {
                    Directory.CreateDirectory(destDir);
                }
                File.Move(file, destFile);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void Restore(string backupdir, string baseDirectory)
        {
            try
            {
                var backUpFiles = CommonUnitity.GetAllFiles(backupdir);
                foreach (var file in backUpFiles)
                {
                    if (!file.StartsWith(backupdir, StringComparison.OrdinalIgnoreCase)
                        && file.Length > (backupdir.Length + 1))
                        continue;
                    var tempfile = file.Substring(baseDirectory.Length + 1);
                    var destFile = Path.Combine(baseDirectory, tempfile);
                    var dir = Path.GetDirectoryName(destFile);
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    if (!File.Exists(destFile))
                    {
                        File.Move(file, destFile);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Restore");
            }
        }

        #endregion

        #region Helper Class

        /// <summary>
        /// 下载进度
        /// </summary>
        /// <param name="bytesReceived"></param>
        /// <param name="totalBytesToReceive"></param>
        /// <param name="progressPercentage"></param>
        public delegate void DownloadProgressChangedCallBack(long bytesReceived, long totalBytesToReceive, int progressPercentage);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="success">是否自动升级成功</param>
        public delegate void AutoUpdaterCompletedCallBack(string message, bool success);

        class DownloadUserToken
        {
            public WebClient Client { get; set; }

            public string FileName { get; set; }

            public string ExcuteFileName { get; set; }

            public long BytesReceived { get; set; }
            public long TotalBytesToReceive { get; set; }

            public int ProgressPercentage { get; set; }
        }

        #endregion
    }
}
