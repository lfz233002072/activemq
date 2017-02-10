namespace Lfz.MqListener.Shared.Enums
{
    /// <summary>
    /// 
    /// </summary>
    public enum MqListenerStatus
    {
        /// <summary>
        /// 在线
        /// </summary>
        [CustomDescription("在线")]
        Online = 1,

        /// <summary>
        /// 离线
        /// </summary>
        [CustomDescription("离线")]
        Offline=0,
    }

}