// /*======================================================================
// *
// *        Copyright (C)  1996-2012  lfz    
// *        All rights reserved
// *
// *        Filename :CustomerInfoStatus.cs
// *        DESCRIPTION :
// *
// *        Created By 林芳崽 at 2015-12-16 17:40
// *        https://git.oschina.net/lfz
// *
// *======================================================================*/

namespace Lfz.MqListener.Shared.Enums
{
    /// <summary>
    /// 
    /// </summary>
    public enum DataAuditingStatus
    {

        /// <summary>
        /// 
        /// </summary>
        [CustomDescription("正常")]
        Normal = 0,

        /// <summary>
        /// 待审批
        /// </summary>
        [CustomDescription("待审批")]
        Auditing = 1,

        /// <summary>
        /// 通过
        /// </summary>
        [CustomDescription("通过")]
        Pass = 2,


        /// <summary>
        /// 不通过
        /// </summary>
        [CustomDescription("不通过")]
        Unpass = -1

    } 
    
    /// <summary>
    /// 
    /// </summary>
    public enum TradeStatus
    { 
        /// <summary>
        /// 
        /// </summary>
        [CustomDescription("正常")]
        Normal = 0,
          
        /// <summary>
        /// 不通过
        /// </summary>
        [CustomDescription("暂停")]
        Stop = -1

    }
}