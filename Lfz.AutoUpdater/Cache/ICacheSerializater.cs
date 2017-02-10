using System;
using System.IO;
using System.Text;
using Lfz.AutoUpdater.Config;
using Lfz.AutoUpdater.Logging;
using Lfz.AutoUpdater.Rest;
using Newtonsoft.Json;

namespace Lfz.AutoUpdater.Cache
{
    /// <summary>
    /// 缓存数据序列化
    /// </summary>
    internal interface ICacheSerializater
    {

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="filename">文件路径</param>
        bool Save(object obj, string filename);

        /// <summary>
        /// 反序列化,从XML文件中反序列化数据
        /// </summary>
        /// <param name="filename"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Load<T>(string filename) where T : class;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elementType"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        object Load(Type elementType, string filename);
    }

    /// <summary>
    /// 缓存数据序列化默认实现
    /// </summary>
    internal class DefaultCacheSerializater : ICacheSerializater
    {
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="filename">文件路径</param>
        public virtual bool Save(object obj, string filename)
        {
            return SerializationHelper.Save(obj, filename);
        }

        /// <summary>
        /// 反序列化,从XML文件中反序列化数据
        /// </summary>
        /// <param name="filename"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual T Load<T>(string filename) where T : class
        {
            return SerializationHelper.Load<T>(filename);
        }

        public object Load(Type elementType, string filename)
        {
            return SerializationHelper.Load(elementType,filename);
        }
    }


    /// <summary>
    /// 缓存数据序列化默认实现
    /// </summary>
    internal class JsonFileCacheSerializater : ICacheSerializater
    {
        private static readonly ILogger Logger;

        static JsonFileCacheSerializater()
        {
            Logger = LoggerFactory.GetLog();
        }
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="filename">文件路径</param>
        public virtual bool Save(object obj, string filename)
        {
            if (obj == null) return true;
            FileStream fs = null;
            try
            {
                var data = JsonUtils.SerializeObject(obj,Formatting.Indented);
                if (string.IsNullOrEmpty(data)) return true;
                var bytes = System.Text.Encoding.UTF8.GetBytes(data);
                fs = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                fs.Write(bytes, 0, bytes.Length);
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("JsonFileCacheSerializater.Save Msg:{0} StackTrace:{1} Source:{2} TargetSite:{3}", ex.Message, ex.StackTrace, ex.Source, ex.TargetSite));
                return false;
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
            return true;
        }

        /// <summary>
        /// 反序列化,从XML文件中反序列化数据
        /// </summary>
        /// <param name="filename"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual T Load<T>(string filename) where T : class
        {
            StreamReader reader = null;
            try
            {
                if (!File.Exists(filename)) return null;
                reader = new StreamReader(filename, Encoding.UTF8);
                var str = reader.ReadToEnd();
                return JsonUtils.Deserialize<T>(str);
            }
            catch (Exception ex)
            {
                Logger.Error("SerializationHelper.Load:" + ex.Message);
                return null;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }


        /// <summary>
        /// 反序列化,从XML文件中反序列化数据
        /// </summary>
        /// <param name="elementType"></param>
        /// <param name="filename"></param> 
        /// <returns></returns>
        public virtual object Load(Type elementType, string filename)  
        {
            StreamReader reader = null;
            try
            {
                if (!File.Exists(filename)) return null;
                reader = new StreamReader(filename, Encoding.UTF8);
                var str = reader.ReadToEnd();
                return JsonUtils.DeserializeObject(str, elementType);
            }
            catch (Exception ex)
            {
                Logger.Error("SerializationHelper.Load:" + ex.Message);
                return null;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }
    }
}