//======================================================================
//
//        Copyright (C)  1996-2012  lfz    
//        All rights reserved
//
//        Filename :CacheRegionName
//        DESCRIPTION :  
//
//        Created By 林芳崽 at  2013-01-08 09:11:01
//        https://git.oschina.net/lfz
//
//======================================================================   

namespace Lfz.MqListener.Shared.Enums
{
    /// <summary>
    /// 缓存区域名称枚举
    /// </summary>
    public enum CacheRegionName
    {
        /// <summary>
        /// 
        /// </summary>
        Default = 0,

        #region 核心数据存储100-199

        /// <summary>
        /// 
        /// </summary>
        StoreExtendInfo = 101,

        /// <summary>
        /// 
        /// </summary>
        MqListener = 102,
         
        /// <summary>
        /// 
        /// </summary>
        WxComponent = 103,
        /// <summary>
        /// 
        /// </summary>
        WxAuthorizerInfo = 104,

        /// <summary>
        /// 
        /// </summary>
        PayConfig = 105,

        /// <summary>
        /// 
        /// </summary>
        AlibabaStoreSettings = 106,

        /// <summary>
        /// 
        /// </summary>
        SysMenuGroup=107,

        /// <summary>
        /// 
        /// </summary>
        Department = 108, 
        /// <summary>
        /// 
        /// </summary>
        CustomerInfo = 109, 

        #endregion

        #region 权限模块 200-220

        /// <summary>
        /// 权限模块缓存
        /// </summary>
        UserPermissions = 200,

        /// <summary>
        /// 
        /// </summary>
        AppUserinfo = 201,

        #endregion

        #region 800-899 文件缓存

        /// <summary>
        /// 
        /// </summary>
        FileMqTopicError = 993,

        /// <summary>
        /// 
        /// </summary>
        FileMqQuqueError = 994,

        /// <summary>
        /// mq数据清理时的删除实例
        /// </summary>
        FileMqDeleteInstace = 995,

        /// <summary>
        /// NHibernate配置项缓存
        /// </summary>
        NHibernateCfg=996,

        /// <summary>
        /// 文件缓存
        /// </summary>
        FileUserPermissions=999,


        #endregion

        /// <summary>
        /// 消息队列获取实现(消息队列使用共享所有broker，redis使用的是键值分摊方式,)
        /// </summary>
        MqConfig=999,

        /// <summary>
        /// 使用服务基类缓存的数据
        /// </summary>
        CacheServiceBase = 1000,

        /// <summary>
        ///系统配置
        /// </summary>
        SysConfig=1001
    }
}