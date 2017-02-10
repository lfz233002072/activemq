namespace Lfz.MqListener.Shared.Enums
{
    /// <summary>
    /// 
    /// </summary>
    public enum StoreStatus
    {


        /// <summary>
        ///失联 1
        /// </summary>
        [CustomDescription("失联")]
        Offline = 0,

        /// <summary>
        ///失联 1
        /// </summary>
        [CustomDescription("连接中")]
        InConnection = 1,

        /// <summary>
        ///在线 2
        /// </summary>
        [CustomDescription("在线")]
        Online = 2,


    }

    /// <summary>
    /// 门店审批状态
    /// </summary>
    public enum StoreAuditStatus
    {
        /// <summary>
        ///未提交 0
        /// </summary>
        [CustomDescription("未提交")]
        Normal = 0,

        /// <summary>
        ///审批中 0
        /// </summary>
        [CustomDescription("审批中")]
        Auditing = 1,

        /// <summary>
        /// 
        /// </summary>
        [CustomDescription("通过")]
        Passing = 2,

        /// <summary>
        /// 
        /// </summary>
        [CustomDescription("不通过")]
        Unpass = -1,

    }


}