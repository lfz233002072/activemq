using System;

namespace Lfz.MqListener.Shared
{
    /// <summary>
    /// 树形节点
    /// </summary>
    public interface ITreeContent
    {
        System.Guid Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        Guid? ParentId { get; set; }

        /// <summary>
        /// 根路径，为0表示一级
        /// </summary>
        Guid? RootId { get; set; }

        /// <summary>
        /// 访问路径 树形节点路径就，使用逗号分割
        /// </summary> 
        string VisitPath { get; set; }

        /// <summary>
        /// 
        /// </summary>
        int? VisitLevel { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public interface IDepartmentContent
    {
        /// <summary>
        /// 客户ID
        /// </summary>
        Guid CustomerId { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public interface ICustomerContent
    {
        /// <summary>
        /// 客户ID
        /// </summary>
        Guid? CustomerId { get; set; }
    }

    /// <summary>
    /// 有记录孩子节点是否存在属性的树
    /// </summary>
    public interface IHasChildTreeContent : ITreeContent
    {
        /// <summary>
        /// 是否具有孩子节点
        /// </summary>
        bool? HasChild { get; set; }
    }
}