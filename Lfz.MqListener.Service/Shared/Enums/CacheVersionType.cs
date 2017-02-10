namespace Lfz.MqListener.Shared.Enums
{
    /// <summary>
    /// 
    /// </summary>
    public enum CacheVersionType
    {
        /// <summary>
        /// 默认缓存版本（全局范围内只有一条记录）
        /// </summary>
        Default=0,

        /// <summary>
        /// 商家相关缓存版本（每个商家一条记录）
        /// </summary>
        Customer = 1,

        /// <summary>
        /// 门店相关缓存版本（每个门店一条记录）
        /// </summary>
        StoreInfo = 2,

        /// <summary>
        /// 用户相关缓存版本（每个用户一条记录）
        /// </summary>
        User=3
    }
}