// /*======================================================================
// *
// *        Copyright (C)  1996-2012  lfz    
// *        All rights reserved
// *
// *        Filename :DbConfigHelper.cs
// *        DESCRIPTION :
// *
// *        Created By 林芳崽 at 2016-01-26 14:20
// *        https://git.oschina.net/lfz
// *
// *======================================================================*/

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Lfz.Data.RawSql;

namespace Lfz.MqListener
{
    public static class DbConfigHelper
    {
        public static IDbProviderConfig GetConfig()
        {
            return new DbProviderConfig()
            {
                DbProvider = DbProvider.SqlServer,
                DbConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["CustomerServiceCenterConnectionStr"].ConnectionString
            };
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static string BuildCreateSql(string tableName, string primaryKey, IDictionary<string, object> dic)
        {
            StringBuilder fieldStr = new StringBuilder("");
            StringBuilder valueStr = new StringBuilder("");
            bool flag = false;
            foreach (var pair in dic)
            {
                if (string.Equals(pair.Key, primaryKey, StringComparison.OrdinalIgnoreCase)) continue;
                fieldStr.AppendFormat("{0}{1}", flag ? "," : "", pair.Key);
                valueStr.AppendFormat("{0}'{1}'", flag ? "," : "", pair.Value);
                flag = true;
            }
            var str = fieldStr.ToString();
            if (string.IsNullOrEmpty(str)) return string.Empty;
            return string.Format("INSERT {0}({1}) VALUES({2}); select @@IDENTITY;", tableName, str, valueStr.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static string BuildUpdateSql(string tableName, string primaryKey, IDictionary<string, object> dic, params string[] keys)
        {
            //如果没有自定义更新主键，那么使用表主键
            var keyList = new List<string>();
            if (keys != null && keys.Any())
                keyList.AddRange(keys);
            else keyList.Add(primaryKey);
            List<string> inValidList = new List<string>();
            foreach (var pair in dic)
            {
                if (pair.Value == null)
                    inValidList.Add(pair.Key);
            }
            foreach (var key in inValidList)
            {
                dic.Remove(key);
            }
            StringBuilder builder = new StringBuilder("update ");
            builder.AppendFormat("{0} SET ", tableName);
            bool hasSetClause = false;
            foreach (var pair in dic)
            {
                //除主键外，其它值都是可变的
                if (string.Equals(pair.Key, primaryKey, StringComparison.OrdinalIgnoreCase)
                     || pair.Value == null)
                    continue;
                builder.AppendFormat("{2}{0}='{1}'", pair.Key, pair.Value, hasSetClause ? "," : "");
                hasSetClause = true;
            }
            if (!hasSetClause) return string.Empty;
            hasSetClause = false;
            foreach (var key in keyList)
            {
                string tempkey = key;
                object value = null;
                if (dic.ContainsKey(tempkey))
                {
                    value = dic[tempkey];
                }
                else
                {
                    tempkey = dic.Keys.FirstOrDefault(x => string.Equals(x, tempkey, StringComparison.OrdinalIgnoreCase));
                    if (!string.IsNullOrEmpty(tempkey)) value = dic[tempkey];
                }
                if (value != null)
                {
                    builder.AppendFormat("{2}{0}='{1}'", tempkey, value, hasSetClause ? " and " : " where ");
                    hasSetClause = true;
                }
            }
            if (!hasSetClause) return string.Empty;
            return builder.ToString();
        }

        public static bool CreateOrUpdate<TElement>(
            this IRawSqlSearchService service, string tableName, string primaryKey,
            TElement model, params string[] keys) where TElement : class ,new()
        {
            var dic = ToSqlDictionary(service.GetDictionary(model));
            List<string> primaryKeyList = new List<string>();
            if (keys != null)
                primaryKeyList.AddRange(keys);
            if (keys == null || !keys.Any())
                primaryKeyList.Add(primaryKey);
            var whereClause = "";
            string sqlClause;
            foreach (var key in primaryKeyList)
            {
                var tempKey = key.ToLower();
                if (dic.ContainsKey(tempKey))
                    whereClause = string.Format("{0}{1}{2}='{3}'", whereClause, string.IsNullOrEmpty(whereClause) ? "" : " AND ", tempKey, dic[tempKey]);
            }
            if (string.IsNullOrEmpty(whereClause)) return false;
            if (service.Exists(tableName, whereClause))
            {
                sqlClause = BuildUpdateSql(tableName, primaryKey, dic, keys);
                if (string.IsNullOrEmpty(sqlClause)) return false;
                service.ExcuteSql(sqlClause);
            }
            else
            {
                sqlClause = BuildCreateSql(tableName, primaryKey, dic);
                if (string.IsNullOrEmpty(sqlClause)) return false;

                var propertyList = typeof(TElement).GetProperties().Where(x => x.CanWrite);
                var property =
                    propertyList.FirstOrDefault(
                        x => string.Equals(x.Name, primaryKey, StringComparison.OrdinalIgnoreCase));
                if (property != null && property.PropertyType == typeof(int))
                {
                    var value = service.GetSingle(sqlClause);
                    property.SetValue(model, value, new object[] { });
                }
                else
                {
                    service.ExcuteSql(sqlClause);
                }
            }
            return true;
        }

        private static IDictionary<string, object> ToSqlDictionary(IDictionary<string, object> dic)
        {
            IDictionary<string, object> result = new Dictionary<string, object>();
            foreach (var o in dic)
            {
                var key = o.Key.ToLower();
                if (result.ContainsKey(key) || string.IsNullOrEmpty(o.Key) || o.Value == null) continue;
                result.Add(key, o.Value);
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static bool Create<TElement>(this IRawSqlSearchService service, string tableName, string primaryKey,
            TElement model, params string[] keys) where TElement : class ,new()
        {
            var dic = ToSqlDictionary(service.GetDictionary(model));
            List<string> primaryKeyList = new List<string>();
            if (keys != null)
                primaryKeyList.AddRange(keys);
            if (keys == null || !keys.Any())
                primaryKeyList.Add(primaryKey);
            var whereClause = "";
            string sqlClause;
            foreach (var key in primaryKeyList)
            {
                var tempKey = key.ToLower();
                if (dic.ContainsKey(tempKey))
                    whereClause = string.Format("{0}{1}{2}='{3}'", whereClause, string.IsNullOrEmpty(whereClause) ? "" : " AND ", tempKey, dic[tempKey]);
            }
            if (string.IsNullOrEmpty(whereClause)) return false;

            sqlClause = BuildCreateSql(tableName, primaryKey, dic);
            if (string.IsNullOrEmpty(sqlClause)) return false;

            var propertyList = typeof(TElement).GetProperties().Where(x => x.CanWrite);
            var property =
                propertyList.FirstOrDefault(
                    x => string.Equals(x.Name, primaryKey, StringComparison.OrdinalIgnoreCase));
            if (property != null && property.PropertyType == typeof(int))
            {
                var value = service.GetSingle(sqlClause);
                property.SetValue(model, value, new object[] { });
            }
            else
            {
                service.ExcuteSql(sqlClause);
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static bool Update<TElement>(this IRawSqlSearchService service, string tableName, string primaryKey, TElement model, params string[] keys) where TElement : class ,new()
        {
            var dic = ToSqlDictionary(service.GetDictionary(model));
            List<string> primaryKeyList = new List<string>();
            if (keys != null)
                primaryKeyList.AddRange(keys);
            if (keys == null || !keys.Any())
                primaryKeyList.Add(primaryKey);
            var whereClause = "";
            string sqlClause;
            foreach (var key in primaryKeyList)
            {
                var tempKey = key.ToLower();
                if (dic.ContainsKey(tempKey))
                    whereClause = string.Format("{0}{1}{2}='{3}'", whereClause, string.IsNullOrEmpty(whereClause) ? "" : " AND ", tempKey, dic[tempKey]);
            }
            if (string.IsNullOrEmpty(whereClause)) return false;

            sqlClause = BuildUpdateSql(tableName, primaryKey, dic, keys);
            if (string.IsNullOrEmpty(sqlClause)) return false;
            service.ExcuteSql(sqlClause);

            return true;
        }

    }


    public class StringZipHelper
    {
        /// <summary>
        /// 解压
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static DataSet GetDatasetByString(string Value)
        {
            DataSet ds = new DataSet();
            string CC = GZipDecompressString(Value);
            System.IO.StringReader Sr = new StringReader(CC);
            ds.ReadXml(Sr);
            return ds;
        }
        /// <summary>
        /// 根据DATASET压缩字符串
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public static string GetStringByDataset(string ds)
        {
            return GZipCompressString(ds);
        }
        /// <summary>
        /// 将传入字符串以GZip算法压缩后，返回Base64编码字符
        /// </summary>
        /// <param name="rawString">需要压缩的字符串</param>
        /// <returns>压缩后的Base64编码的字符串</returns>
        public static string GZipCompressString(string rawString)
        {
            if (string.IsNullOrEmpty(rawString) || rawString.Length == 0)
            {
                return "";
            }
            else
            {
                byte[] rawData = System.Text.Encoding.UTF8.GetBytes(rawString.ToString());
                byte[] zippedData = Compress(rawData);
                return Convert.ToBase64String(zippedData);
            }
        }

        public static byte[] GZipCompress(string rawString)
        {
            if (string.IsNullOrEmpty(rawString) || rawString.Length == 0)
            {
                return new byte[] { };
            }
            else
            {
                byte[] rawData = System.Text.Encoding.UTF8.GetBytes(rawString);
                byte[] zippedData = Compress(rawData);
                return zippedData;
            }
        }


        /// <summary>
        /// GZip压缩
        /// </summary>
        /// <param name="rawData"></param>
        /// <returns></returns>
        private static byte[] Compress(byte[] rawData)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (GZipStream compressedzipStream = new GZipStream(ms, CompressionMode.Compress, true))
                {
                    compressedzipStream.Write(rawData, 0, rawData.Length);
                    compressedzipStream.Close();
                    var result = ms.ToArray();
                    return result;
                }
            }
        }
        /// <summary>
        /// 将传入的二进制字符串资料以GZip算法解压缩
        /// </summary>
        /// <param name="zippedString">经GZip压缩后的二进制字符串</param>
        /// <returns>原始未压缩字符串</returns>
        public static string GZipDecompressString(string zippedString)
        {
            if (string.IsNullOrEmpty(zippedString) || zippedString.Length == 0)
            {
                return "";
            }
            else
            {
                byte[] zippedData = Convert.FromBase64String(zippedString.ToString());
                return System.Text.Encoding.UTF8.GetString(Decompress(zippedData));
            }
        }

        /// <summary>
        /// ZIP解压
        /// </summary>
        /// <param name="zippedData"></param>
        /// <returns></returns>
        private static byte[] Decompress(byte[] zippedData)
        {
            using (MemoryStream ms = new MemoryStream(zippedData))
            {
                using (GZipStream compressedzipStream = new GZipStream(ms, CompressionMode.Decompress))
                {
                    using (MemoryStream outBuffer = new MemoryStream())
                    {
                        byte[] block = new byte[1024];
                        while (true)
                        {
                            int bytesRead = compressedzipStream.Read(block, 0, block.Length);
                            if (bytesRead <= 0)
                                break;
                            else
                                outBuffer.Write(block, 0, bytesRead);
                        }
                        compressedzipStream.Close();
                        var result = outBuffer.ToArray();
                        outBuffer.Close();
                        return result;
                    }
                }
            }
        }

        /// <summary>
        /// 解压缩
        /// </summary>
        /// <param name="zippedData"></param>
        /// <returns></returns>
        public static string GZipDecompress(byte[] zippedData)
        {
            if (zippedData == null || zippedData.Length == 0) return string.Empty;
            try
            {
                return System.Text.Encoding.UTF8.GetString(Decompress(zippedData));
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

    }
}