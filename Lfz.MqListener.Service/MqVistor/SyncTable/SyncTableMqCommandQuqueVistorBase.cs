// /*======================================================================
// *
// *        Copyright (C)  1996-2012  lfz    
// *        All rights reserved
// *
// *        Filename :SyncTableMqCommandQuqueVistor.cs
// *        DESCRIPTION :
// *
// *        Created By 林芳崽 at 2016-01-20 17:28
// *        https://git.oschina.net/lfz
// *
// *======================================================================*/

using System;
using System.Collections.Generic;
using System.Data;
using Lfz.Caching;
using Lfz.Logging;
using Lfz.MqListener.Mq;
using Lfz.MqListener.Shared.Enums;
using Lfz.MqListener.Shared.Models;
using Lfz.Rest;
using Newtonsoft.Json;

namespace Lfz.MqListener.MqVistor.SyncTable
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class SyncTableMqCommandQuqueVistorBase : IMqCommandQuqueVistor
    {  
        private readonly ILogger _logger;
        private readonly FileCacheManager<MqCommandInfo> _cacheManager;  

        /// <summary>
        /// 
        /// </summary> 
        protected SyncTableMqCommandQuqueVistorBase()
        { 
            _logger = LoggerFactory.GetLog();
            _cacheManager = new FileCacheManager<MqCommandInfo>((int)CacheRegionName.FileMqQuqueError); 
        }
         

        public void Vistor(QuqueName topicName, MqCommandInfo commandInfo)
        { 
            try
            {
                if (commandInfo.MessageType != NMSMessageType.SyncByTableName
                    || string.IsNullOrEmpty(commandInfo.Body))
                    return;
                var content = JsonUtils.Deserialize<SyncTableContent>(commandInfo.Body);
                if (content == null
                    || !content.HasData)
                {
                    _logger.Error(string.Format("内容解析无效 StoreId: {0} Body:{1}", commandInfo.StoreId, commandInfo.Body));
                    return;
                }  
                //TODO 上报数据插入数据库
                InsertOrUpdate(   content);
            }
            catch (Exception ex)
            { 
                //错误处理，缓存到本地文件中
                _cacheManager.AddOnlyFileCache(Guid.NewGuid(), commandInfo);
                _logger.Error(ex, "Vistor");
            } 
        }

        /// <summary>
        /// 更新数据实现
        /// </summary> 
        /// <param name="table"></param>
        public abstract void InsertOrUpdate(  SyncTableContent table);
         

        #region HelperClass
         
        /// <summary>
        /// 
        /// </summary>
        public class SyncTableContent
        {
            /// <summary>
            /// 
            /// </summary>
            public SyncTableContent()
            {

            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="dt"></param>
            public SyncTableContent(DataTable dt)
            {
                Columns = new List<string>();
                foreach (DataColumn column in dt.Columns)
                {
                    Columns.Add(column.ColumnName);
                }
                Rows = new List<object[]>();
                foreach (DataRow row in dt.Rows)
                {
                    Rows.Add(row.ItemArray);
                }
            }

            /// <summary>
            /// 
            /// </summary>
            [JsonProperty("PK")]
            public List<string> PrimaryKeys { get; set; }

            /// <summary>
            /// 同步表名
            /// </summary>
            [JsonProperty("T")]
            public string TableName { get; set; }

            /// <summary>
            /// 列名列表
            /// </summary>
            [JsonProperty("C")]
            public List<string> Columns { get; set; }

            /// <summary>
            /// 行数
            /// </summary>
            [JsonProperty("R")]
            public List<object[]> Rows { get; set; }

            /// <summary>
            /// 是否有效
            /// </summary> 
            [JsonProperty]
            public bool HasData
            {
                get
                {
                    return
                        !string.IsNullOrEmpty(TableName)
                    && this.Columns != null
                    && this.Columns.Count > 0
                    && this.Rows != null
                    && this.Rows.Count > 0;
                }
            }

        }

        #endregion
    }
}