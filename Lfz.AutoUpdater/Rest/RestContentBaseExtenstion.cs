using Newtonsoft.Json;

namespace Lfz.AutoUpdater.Rest
{
    /// <summary>
    /// 
    /// </summary>
    internal static class JsonContentExtenstion
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <param name="converters"></param>
        /// <returns></returns>
        public static string ToJsonString(this IJsonContent content, params JsonConverter[] converters)
        {
            return JsonUtils.SerializeObject(content, converters);
        }
    }
}