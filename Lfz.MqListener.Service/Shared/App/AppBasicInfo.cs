using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Lfz.MqListener.Shared.Enums;

namespace Lfz.MqListener.Shared.App
{
    /// <summary>
    /// 
    /// </summary>
    [Table("App_BasicInfo")]
    public partial class AppBasicInfo
    {
        public int Id { get; set; }

        [StringLength(255)]
        public string RunVersion { get; set; }

        [StringLength(255)]
        public string AppName { get; set; }

        public AppType AppType { get; set; }

        public AppStatus Status { get; set; }

        [StringLength(255)]
        public string AppId { get; set; }

        public DateTime? CreateTime { get; set; }

        public DateTime? LastUpdateTime { get; set; }
        public DateTime? LastHeartTime { get; set; }

        public DateTime? OnlineTime { get; set; }

        [StringLength(255)]
        public string GitRepository { get; set; }

        [StringLength(255)]
        public string MacAddress { get; set; }

        [StringLength(255)]
        public string IpAddress { get; set; }

        [StringLength(255)]
        public string ProcessDirectory { get; set; }

        [StringLength(255)]
        public string ComputerName { get; set; }

        /// <summary>
        /// √≈µÍID
        /// </summary>
        public Guid? StoreId { get; set; }
    }
}
