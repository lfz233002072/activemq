// /*======================================================================
// *
// *        Copyright (C)  1996-2012  lfz    
// *        All rights reserved
// *
// *        Filename :MqCommandInfo.cs
// *        DESCRIPTION :
// *
// *        Created By 林芳崽 at 2016-01-20 16:54
// *        https://git.oschina.net/lfz
// *
// *======================================================================*/

using System;
using System.Collections.Generic;
using Lfz.MqListener.Shared.Models;

namespace Lfz.MqListener.Mq
{
    /// <summary>
    /// 
    /// </summary>
    public class MqCommandInfo
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        public NMSMessageType MessageType { get; set; }

        /// <summary>
        /// 消息类型原始字符串
        /// </summary>
        public string RawNMSType { get; set; }


        /// <summary>
        /// 消息来源门店ID（或客户端ID）
        /// </summary>
        public Guid StoreId { get; set; }

        /// <summary>
        /// 消息内容体
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// 执行次数
        /// </summary>
        public int ExcuteCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string NMSMessageId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int MqInstancId { get; set; }

        /// <summary>
        /// 属性列表
        /// </summary>
        public IDictionary<string, string> Properties { get; set; }
    }
}