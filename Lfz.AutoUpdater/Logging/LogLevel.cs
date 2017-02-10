namespace Lfz.AutoUpdater.Logging
{
    /// <summary>
    /// 日志等级 Fatal > Error > Warning> Information > Debug > Trace
    /// </summary>
    internal enum LogLevel
    {
        /// <summary>
        /// 跟踪信息
        /// </summary> 
        Trace,

        /// <summary>
        /// 调试
        /// </summary> 
        Debug,
        /// <summary>
        /// 信息
        /// </summary> 
        Information,
        /// <summary>
        /// 警报
        /// </summary> 
        Warning,
        /// <summary>
        /// 错误
        /// </summary> 
        Error,
        /// <summary>
        /// 致命错误
        /// </summary> 
        Fatal,
    }
}