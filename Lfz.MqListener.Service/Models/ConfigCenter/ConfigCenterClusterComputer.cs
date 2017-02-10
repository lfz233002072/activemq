using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Lfz.MqListener.Shared.Enums;

namespace Lfz.MqListener.Models.ConfigCenter
{
    /// <summary>
    /// 
    /// </summary> 
    [Table("ConfigCenter_ClusterComputer")]
    public partial class ConfigCenterClusterComputer
    {
        /// <summary>
        /// 
        /// </summary>
        [Key()]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]  //设置自增
        public int Id { get; set; } 

        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "ip地址不能为空")] 
        [StringLength(50)]
        public string IpAddress { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? MqttPort { get; set; }

        /// <summary>
        /// 局域网IP
        /// </summary>
        public string LanIpAddress { get; set; }

        ///// <summary>
        ///// TODO 暂时要求内网、外网的端口一直
        ///// </summary>
        //public int LanPort { get; set; }
        ///// <summary>
        ///// 
        ///// </summary>
        //public int? LanMqttPort { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "主机名称不能为空")]
        [StringLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsMaster { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [StringLength(255)]
        public string Description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [StringLength(255)]
        public string AdminUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [StringLength(50)]
        public string AccessUsername { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [StringLength(50)]
        public string AccessPassword { get; set; }

        /// <summary>
        /// 权重
        /// </summary>
        public int Weight { get; set; }

        /// <summary>
        /// 集群电脑类型
        /// </summary>
        public ClusterComputerType ComputerType { get; set; }

        /// <summary>
        /// 删除标记
        /// </summary>
        public bool DelFlag { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 最近删除时间，如果数据标记未删除，那么为删除时间
        /// </summary>
        public DateTime? LastUpdateTime { get; set; }
    }
}
