/*****************************************************************
 * Copyright (C) Knights Warrior Corporation. All rights reserved.
 * 
 * Author:   圣殿骑士（Knights Warrior） 
 * Email:    KnightsWarrior@msn.com
 * Website:  http://www.cnblogs.com/KnightsWarrior/       http://knightswarrior.blog.51cto.com/
 * Create Date:  5/8/2010 
 * Usage:
 *
 * RevisionHistory
 * Date         Author               Description
 * 
*****************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lfz.AutoUpdater.Config;
using Lfz.AutoUpdater.Utitlies;
using SharpCompress.Archive;
using SharpCompress.Common;

namespace Lfz.AutoUpdater
{ 

    /// <summary>
    /// 
    /// </summary>
    public class CommonUnitity
    {
        /// <summary>
        /// 初始化自动更新配置信息
        /// </summary>
        /// <param name="runningAsWindowServie"></param>
        /// <param name="name"></param>
        public static void Init(bool runningAsWindowServie, string name = "")
        {
            var data = AutoUpdaterInfo.Current;
            data.IsWindowService = runningAsWindowServie;
            if (!String.IsNullOrEmpty(name))
                data.ExcuteFileName = name;
            data.Save();
        }

        /// <summary>
        /// 系统跟目录
        /// </summary>
        /// <returns></returns>
        public static string GetSystemBinUrl()
        {
            return Utils.MapPath("~/").TrimEnd('\\');
        }

        /// <summary>
        /// 更新文件下载目录
        /// </summary>
        /// <returns></returns>
        public static string GetDownLoadFolder()
        {
            var directory = Utils.MapPath("~/DownLoad");
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            return directory;
        }

        /// <summary>
        /// 更新内容备份文件
        /// </summary>
        /// <returns></returns>
        public static string GetBackupFolder()
        {
            var directory = Utils.MapPath(string.Format("~/Backup/{0}", DateTime.Now.ToString("yyyyMMdd")));
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            return directory;
        }

        /// <summary>
        /// 更新文件解压路径
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string GetUnzipFolder(string filename)
        {
            if (string.IsNullOrEmpty(filename)) return string.Empty;
            return Path.Combine(Path.GetDirectoryName(filename), Path.GetFileNameWithoutExtension(filename));
        }

        /// <summary>
        /// 清空文件夹下所有文件
        /// </summary>
        /// <param name="folder"></param>
        public static void ClearFolder(string folder)
        {
            var files = Directory.EnumerateFiles(folder);
            foreach (var file in files)
                File.Delete(file);
            var folderList = Directory.EnumerateDirectories(folder);
            foreach (var tempFolder in folderList)
                Directory.Delete(tempFolder, true);
        }

        /// <summary>
        /// 获取指定目录下的所有文件
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public static List<string> GetAllFiles(string folder)
        {
            return Directory.EnumerateFiles(folder, "*", SearchOption.AllDirectories).ToList();
        }

        /// <summary>
        /// 对外公布Api接口
        /// </summary>
        public static string ApiDomainUrl
        {
            get
            {
                var url = RunConfig.Current.ApiDomainUrl ?? string.Empty;
                url = url.Trim().TrimEnd('/');
                if (string.IsNullOrEmpty(url)) return string.Empty;
                if (!url.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                    url = "http://" + url;
                return url;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class ZipHandler
    {
        /// <summary>
        /// 压缩
        /// </summary>
        /// <param name="zipedFileName"></param>
        /// <param name="targetDirectory"></param>
        public static void UnZipFile(string zipedFileName, string targetDirectory)
        {
            UnZipFile(zipedFileName, targetDirectory, string.Empty, string.Empty);
        }

        /// <summary>
        /// 解压缩文件
        /// </summary>
        /// <param name="zipedFileName">Zip的完整文件名（如D:\test.zip）</param>
        /// <param name="targetDirectory">解压到的目录</param>
        /// <param name="password">解压密码</param>
        /// <param name="fileFilter">文件过滤正则表达式</param>
        public static void UnZipFile(string zipedFileName, string targetDirectory, string password, string fileFilter)
        {
            using (Stream stream = File.OpenRead(zipedFileName))
            using (var archive = ArchiveFactory.Open(stream))
            {
                foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
                {
                    entry.WriteToDirectory(targetDirectory,
                                           ExtractOptions.ExtractFullPath | ExtractOptions.Overwrite);
                }
            }
        }

        public static void ZipFile(string zipedFileName, string targetDirectory, string password, string fileFilter)
        {
            using (Stream stream = File.OpenWrite(zipedFileName))
            using (var archive = ArchiveFactory.Open(stream))
            {
                foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
                {
                    entry.WriteToDirectory(targetDirectory,
                                           ExtractOptions.ExtractFullPath | ExtractOptions.Overwrite);
                }
            }
        }
    }
}
