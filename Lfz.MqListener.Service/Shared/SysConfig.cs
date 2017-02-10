using System;
using System.ComponentModel.DataAnnotations.Schema;
using Lfz.Config;
using Lfz.MqListener.Shared.Enums;
using Newtonsoft.Json;

namespace Lfz.MqListener.Shared
{
    /// <summary>
    /// 系统配置
    /// </summary>
    [Table("Sys_Config")]
    public class SysConfig : ISysConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 缓存版本号
        /// </summary>
        [JsonProperty("V")]
        public int CacheVersion { get; set; } 

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("T")]
        public CacheVersionType? CacheVersionType { get; set; }

        /// <summary>
        /// 缓存关联实体ID
        /// </summary>
        [JsonProperty("eid")]
        public Guid? CacheEntityId { get; set; }

        /// <summary>
        /// 关联商家ID
        /// </summary>
        [JsonProperty("cid")]
        public Guid? CustomerId { get; set; }
        
    }
}