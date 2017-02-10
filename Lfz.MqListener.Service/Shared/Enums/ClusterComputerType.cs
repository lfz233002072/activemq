// /*======================================================================
// *
// *        Copyright (C)  1996-2012  lfz    
// *        All rights reserved
// *
// *        Filename :ClusterComputerType.cs
// *        DESCRIPTION :
// *
// *        Created By 林芳崽 at 2015-12-23 11:01
// *        https://git.oschina.net/lfz
// *
// *======================================================================*/

namespace Lfz.MqListener.Shared.Enums
{
    /// <summary>
    /// 
    /// </summary>
    public enum ClusterComputerType
    {
        /// <summary>
        /// 消息队列
        /// </summary>
        [CustomDescription("消息主机")]
        MessageQueue = 0,
        /// <summary>
        /// Redis缓存
        /// </summary>
        [CustomDescription("Redis主机")]
        Redis = 1,
        /// <summary>
        /// Mysql数据库
        /// </summary>
        [CustomDescription("Mysql主机")]
        Mysql = 2,

        /// <summary>
        /// Sqlserver数据库
        /// </summary>
        [CustomDescription("Sqlserver主机")]
        Sqlserver=3

    }
}