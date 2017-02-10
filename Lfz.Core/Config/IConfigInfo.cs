namespace Lfz.Config
{
    /// <summary>
    /// 配置信息基类
    /// </summary>
    public interface IConfigInfo
    {
    }

    /// <summary>
    /// 
    /// </summary>
    public interface ISysConfig
    {
        /// <summary>
        /// 缓存版本号
        /// </summary>
        int CacheVersion { get; }

    }

}
