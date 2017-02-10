using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lfz.MqListener.Models.ConfigCenter
{
    /// <summary>
    /// 
    /// </summary>
    [Table("ConfigCenter_MqTable")]
    public partial class ConfigCenterMqTable
    {
        /// <summary>
        /// 门店ID
        /// </summary>
        [Key]
        public Guid StoreId { get; set; } 
         
        /// <summary>
        /// 客户默认消息队列配置ID
        /// </summary>
        public int? MqInstanceId { get; set; }

        /// <summary>
        /// MQ监听实例ID
        /// </summary>
        public int? MqListenerId { get; set; }
         

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }


        /// <summary>
        /// 过期时间，单位分钟
        /// </summary>
        public int Expired { get; set; }

    }
}
