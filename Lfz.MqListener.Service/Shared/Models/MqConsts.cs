// /*======================================================================
// *
// *        Copyright (C)  1996-2012  lfz    
// *        All rights reserved
// *
// *        Filename :MqConsumerService.cs
// *        DESCRIPTION :
// *
// *        Created By 林芳崽 at 2015-11-09 17:26
// *        https://git.oschina.net/lfz
// *
// *======================================================================*/

namespace Lfz.MqListener.Shared.Models
{
    /// <summary>
    /// 消息队列常量
    /// </summary>
    public class MqConsts
    {
        /// <summary>
        /// 过滤字段名称(门店ID)
        /// </summary>
        public const string ClientId = "clientid";

        /// <summary>
        /// 商家ID
        /// </summary>
        public const string MqInstanceId = "mqid";

        /// <summary>
        /// 商家All
        /// </summary>
        public const string MqInstanceAll = "mqall";

        /// <summary>
        /// App类型
        /// </summary>
        public const string MqAppType = "mat";

        /// <summary>
        /// 设备类型
        /// </summary>
        public const string MqDevice = "mdt";
        /// <summary>
        /// 消息传递是否压缩
        /// </summary>
        public const string IsZipCompress = "iszip";
         
    }
}