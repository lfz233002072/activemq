using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lfz.MqListener.Models.Device
{
    [Table("Device_BasicInfo")]
    public partial class DeviceBasicInfo
    {
        public Guid Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid? StoreId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid CustomerId { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        public int DeviceType { get; set; }

        /// <summary>
        ///  设备IP地址
        /// </summary>
        [StringLength(255)]
        public string IpAddress { get; set; }

        /// <summary>
        /// 设备MAC地址
        /// </summary>
        [StringLength(255)]
        public string MacAddress { get; set; }

        [StringLength(2000)]
        public string Description { get; set; }

        /// <summary>
        ///  设备状态
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 运行系统版本
        /// </summary>
        [StringLength(255)]
        public string OSVersion { get; set; }

        /// <summary>
        /// 驱动版本
        /// </summary>
        [StringLength(255)]
        public string DriverVersion { get; set; }

        /// <summary>
        /// 远程访问信息
        /// </summary>
        [StringLength(255)]
        public string RemoteAccessInfo { get; set; }

        /// <summary>
        /// 工作时间段
        /// </summary>
        [StringLength(2000)]
        public string WorkTimePeriod { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public Guid? CreateUserId { get; set; }

        /// <summary>
        /// 最近更新时间
        /// </summary>
        public DateTime? LastUpdateTime { get; set; }

        public Guid? LastUpdateUserId { get; set; }
    }
}
