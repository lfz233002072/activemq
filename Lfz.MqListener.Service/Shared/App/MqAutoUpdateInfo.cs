// /*======================================================================
// *
// *        Copyright (C)  1996-2012  lfz    
// *        All rights reserved
// *
// *        Filename :MqAutoUpdateInfo.cs
// *        DESCRIPTION :
// *
// *        Created By 林芳崽 at 2016-01-27 16:44
// *        https://git.oschina.net/lfz
// *
// *======================================================================*/

using System;
using Lfz.MqListener.Shared.Enums;

namespace Lfz.MqListener.Shared.App
{
    /// <summary>
    /// 
    /// </summary>
    public class MqAutoUpdateInfo
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
        public AppType AppType { get; set; }

        /// <summary>
        /// 强制更新为指定版本
        /// </summary>
        public bool ForceUpdate { get; set; }
           
    }
}