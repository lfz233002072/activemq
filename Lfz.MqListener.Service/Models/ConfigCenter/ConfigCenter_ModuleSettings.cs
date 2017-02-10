using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Lfz.Data;

namespace Lfz.MqListener.Models.ConfigCenter
{
    /// <summary>
    /// 
    /// </summary>
    [Table("ConfigCenter_ModuleSettings")]
    [Serializable]
    public partial class ConfigCenter_ModuleSettings : EntityBase<Guid>
    { 
        
        /// <summary>
        /// 
        /// </summary>
        public Guid CustomerId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Key]
        public Guid ModuleId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column(TypeName = "ntext")]
        public string JsonContent { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateTime { get; set; }
         
        public override Guid GetKeyValue()
        {
            return ModuleId;
        }
    }
}
