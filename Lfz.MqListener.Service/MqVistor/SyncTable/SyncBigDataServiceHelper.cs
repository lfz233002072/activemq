using System;
using System.Data;
using System.Data.SqlClient;
using Lfz.Logging;

namespace Lfz.MqListener.MqVistor.SyncTable
{
    /// <summary>
    /// 
    /// </summary>
    public static class SyncBigDataServiceHelper 
    {
        private static readonly ILogger _logger;
        static SyncBigDataServiceHelper(
            )
        {

            _logger = LoggerFactory.GetLog();
        }

        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="bigDataTable">数据集合</param>
        /// <param name="conStr"></param>
        /// <returns>运行记录ID</returns>
        public static bool BatchInsert(string conStr, string tableName, DataTable bigDataTable, int batchSize)
        {
            try
            {
                //数据连接字符串 
                using (SqlConnection conn = new SqlConnection(conStr))
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, transaction))
                        {
                            try
                            {
                                bulkCopy.BatchSize = batchSize;
                                bulkCopy.DestinationTableName = tableName;
                                Mapping(bulkCopy.ColumnMappings, bigDataTable);
                                bulkCopy.WriteToServer(bigDataTable);
                                transaction.Commit();
                                return true;
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                _logger.Log(LogLevel.Error, ex.Message + "\r\n ConnectionStr:" + conStr, ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex.Message + "\r\n ConnectionStr:" + conStr, ex);
            }
            return false;
        }

        /// <summary>
        /// 数据表映射
        /// </summary> 
        /// <param name="columnMapping"></param>
        /// <param name="bigDataTable"></param> 
        private static void Mapping(SqlBulkCopyColumnMappingCollection columnMapping, DataTable bigDataTable)
        {
            foreach (DataColumn propertyInfo in bigDataTable.Columns)
            {
                columnMapping.Add(propertyInfo.ColumnName, propertyInfo.ColumnName);
            }
        }
    }
}