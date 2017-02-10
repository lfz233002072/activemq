// /*======================================================================
// *
// *        Copyright (C)  1996-2012  lfz    
// *        All rights reserved
// *
// *        Filename :AppStatus.cs
// *        DESCRIPTION :
// *
// *        Created By 林芳崽 at 2016-01-27 15:13
// *        https://git.oschina.net/lfz
// *
// *======================================================================*/

namespace Lfz.MqListener.Shared.Enums
{
    /// <summary>
    /// 
    /// </summary>
    public enum AppStatus
    { 
        /// <summary>
        /// 
        /// </summary>
        [CustomDescription("部署中")]
        Deploy = 0,


        /// <summary>
        /// 
        /// </summary>
        [CustomDescription("启动中")]
        Starting = 1,


        /// <summary>
        /// 
        /// </summary>
        [CustomDescription("正在运行")]
        Running = 2,

        /// <summary>
        /// 
        /// </summary>
        [CustomDescription("重新启动")]
        Restart = 3,

        /// <summary>
        /// 重新部署
        /// </summary>
        [CustomDescription("重新部署")]
        ReDeploy=4,


        /// <summary>
        /// 
        /// </summary>
        [CustomDescription("已停止")]
        Stoped = -1,
    }

    /// <summary>
    /// 
    /// </summary>
    public enum AppType
    {
        /// <summary>
        /// 非App类型 0
        /// </summary> 
        Empty=0,

        /// <summary>
        /// 1
        /// </summary>
        [CustomDescription("拓创云平台")]
        ZiyouchiWeb=1,

        /// <summary>
        /// 消息调度程序 2
        /// </summary>
        [CustomDescription("消息调度")]
        MqListener = 2,

        /// <summary>
        /// 门店助手
        /// </summary>
        [CustomDescription("门店助手")]
        StoreHelper = 3,


        /// <summary>
        /// 餐饮前台
        /// </summary>
        [CustomDescription("餐饮前台")]
        DinnerFront = 4,

        /// <summary>
        /// 餐饮后台
        /// </summary>
        [CustomDescription("餐饮后台")]
        DinnerAdmin = 5,

        /// <summary>
        /// 叫号系统
        /// </summary>
        [CustomDescription("叫号系统")]
        Callnumber = 6,

        /// <summary>
        /// 会员管理系统
        /// </summary>
        [CustomDescription("会员管理系统")]
        Member = 7,
         
         
    }
}