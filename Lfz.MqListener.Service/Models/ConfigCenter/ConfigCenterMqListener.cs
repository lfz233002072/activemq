using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Lfz.Data;
using Lfz.MqListener.Shared.Enums;

namespace Lfz.MqListener.Models.ConfigCenter
{
    /// <summary>
    /// 
    /// </summary>
    [Table("ConfigCenter_MqListener")]
    public partial class ConfigCenterMqListener : EntityBase<int>
    {
        /// <summary>
        ///  
        /// </summary>
        [Key()]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]  //设置自增
        public int  Id { get; set; }

        /// <summary>
        /// 上线之后，处理消息数量
        /// </summary>
        public long? MsgCount { get; set; }

        /// <summary>
        /// 上线之后，处理消息内容大小
        /// </summary>
        public long? MsgSize { get; set; }

        /// <summary>
        /// 累计处理消息数量
        /// </summary>
        public long? TotalMsgCount { get; set; }

        /// <summary>
        /// 累计处理消息内容大小
        /// </summary>
        public long? TotalMsgSize { get; set; }

        /// <summary>
        ///  
        /// </summary>
        [StringLength(255)]
        public string ClientId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [StringLength(255)]
        public string MacAddress { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [StringLength(255)]
        public string ProcessDirectory { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [StringLength(1000)]
        public string IpAddress { get; set; }

     
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? UpTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public MqListenerStatus Status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? DownTime { get; set; }

        /// <summary>
        /// 最近心跳时间
        /// </summary>
        public DateTime? LastHeartTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [StringLength(255)]
        public string ComputerName { get; set; }


        public override int GetKeyValue()
        {
            return Id;
        }
    }
}