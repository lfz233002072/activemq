using System;
using Lfz.AutoUpdater.Utitlies;

namespace Lfz.AutoUpdater.Config
{

    /// <summary>
    /// 
    /// </summary>
    public class AutoUpdaterInfo
    {
        /// <summary>
        /// 当前运行版本号
        /// </summary>
        public string Version { get; set; }

        public int AppType { get; set; }

        /// <summary>
        /// 服务器更新版本号
        /// </summary>
        public string NewVersion { get; set; }

        /// <summary>
        /// 当前运行程序
        /// </summary>
        public string AppName { get; set; }


        /// <summary>
        /// 可执行文件名称
        /// </summary>
        public string ExcuteFileName { get; set; }

        /// <summary>
        /// 是否Window服务中
        /// </summary>
        public bool IsWindowService { get; set; }

        /// <summary>
        /// 最新版本下载地址(要求升级包跟目录包含readme文件)
        /// </summary>
        public string DownloadUrl { get; set; }

        /// <summary>
        /// 最近更新时间
        /// </summary>
        public DateTime LastUpdateTime { get; set; }

        #region The public method
        /// <summary>
        /// Gets the singleton Nop engine used to access Nop services.
        /// </summary>
        public static AutoUpdaterInfo Current
        {
            get
            {
                var data = JsonConfigHelper.GetCurrent(typeof(AutoUpdaterInfo), () => "~/Config/autoupdater.json");
                return data as AutoUpdaterInfo;
            }
        }

        #endregion

        public bool HasNewVersion()
        {
            if (string.IsNullOrEmpty(NewVersion)) return false;
            if (string.IsNullOrEmpty(Version)) return true;
            var tempList = Version.Split('.');
            var newTemplist = NewVersion.Split('.');
            for (int i = 0; i < tempList.Length; i++)
            {
                var old = TypeParse.StrToInt(tempList[i]);
                var newValue = TypeParse.StrToInt(newTemplist[i]);
                if (newValue > old) return true;
                if (newTemplist.Length - 1 <= i) return false;
            }
            if (newTemplist.Length > tempList.Length) return true;
            return false;
        }


        public void Save()
        {
            JsonConfigHelper.Save(this, this.GetConfigFile);
        }


        public string GetConfigFile()
        {
            return "~/Config/autoupdater.json";
        }
    }

}
