namespace Lfz.MqListener.Shared.Models
{
    /// <summary>
    /// 
    /// </summary>
    public enum NMSMessageType
    {
        /// <summary>
        /// 无消息类别标记
        /// </summary>
        None = 0,

        /// <summary>
        /// 根据表同步
        /// </summary>
        SyncByTableName = 1,
        /// <summary>
        /// 拉取同步信息
        /// </summary>
        SyncByTableNamePullMessage = 2,

        /// <summary>
        /// 
        /// </summary>
        MenuSolutionVersion = 10,

        /// <summary>
        /// 
        /// </summary>
        MenuSolutionMdType = 11,

        /// <summary>
        /// 
        /// </summary>
        MenuSolutionMFType = 12,

        /// <summary>
        /// 
        /// </summary>
        MenuSolutionMenuConfig = 13,

        #region 消费数据


        #endregion

        /// <summary>
        /// 自动升级
        /// </summary>
        AutoUpdateInfo=998,
        /// <summary>
        /// 心跳包
        /// </summary>

        Heart = 999
    }
}