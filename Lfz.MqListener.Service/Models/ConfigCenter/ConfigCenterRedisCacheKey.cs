using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lfz.MqListener.Models.ConfigCenter
{
    /// <summary>
    /// 
    /// </summary>
    [Table("ConfigCenter_RedisCacheKey")]
    public partial class ConfigCenterRedisCacheKey
    {
        /// <summary>
        /// 
        /// </summary>
        [Key]
        [StringLength(255)]
        public string CacheKey { get; set; }

        /// <summary>
        /// 缓存配置电脑
        /// </summary>
        public int RedisInstanceId { get; set; }

        /// <summary>
        /// 过期时间，单位分钟
        /// </summary>
        public int Expired { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}