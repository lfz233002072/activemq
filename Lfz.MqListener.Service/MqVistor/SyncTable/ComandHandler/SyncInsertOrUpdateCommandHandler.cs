using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Lfz.Data.RawSql;

namespace Lfz.MqListener.MqVistor.SyncTable.ComandHandler
{
    /// <summary>
    /// 
    /// </summary>
    public class SyncInsertOrUpdateCommandHandler : ISyncTableComand
    {
        private readonly bool _willUpdate;

        public SyncInsertOrUpdateCommandHandler(bool willUpdate = false)
        {
            _willUpdate = willUpdate;
        }

        private string BuildWhereCluase(DataRow row, Guid storeId, List<string> primaryKey)
        {
            StringBuilder builder = new StringBuilder(string.Format("storeId='{0}'", storeId));
            bool flag = false;
            foreach (var key in primaryKey)
            {
                var vaule = row[key];
                if (vaule != null && !string.IsNullOrEmpty(vaule.ToString()))
                {
                    builder.AppendFormat(" AND {0}='{1}'", key, row[key]);
                    flag = true;
                }
            }
            var result = builder.ToString();
            return flag ? result : string.Empty;
        }

        private string BuildWhereCluase(DataTable table, Guid storeId, string primaryKey)
        {
            StringBuilder builder = new StringBuilder();
            bool flag = false;
            foreach (DataRow row in table.Rows)
            {
                var vaule = row[primaryKey];
                if (vaule != null && !string.IsNullOrEmpty(vaule.ToString()))
                {
                    builder.AppendFormat(" {0}'{1}'", flag ? "," : "", vaule);
                    flag = true;
                }
            }
            var temp = builder.ToString();
            if (string.IsNullOrEmpty(temp)) return string.Empty;
            return string.Format("storeId='{0}' AND {1} IN({2})", storeId, primaryKey, temp);
        }


        public void InsertOrUpdate(string tablename, Guid storeId, IRawSqlSearchService service, DataTable table, List<string> primaryKey)
        {
            primaryKey = primaryKey.Where(x => table.Columns.Contains(x)).ToList();
            if (primaryKey.Count == 0) return;

            List<DataRow> willRemoveIndexs = new List<DataRow>();
            if (primaryKey.Count == 1)
            {
                string currentPrimaryKey = primaryKey[0];
                if (string.IsNullOrEmpty(currentPrimaryKey)) return;
                var whereClause = BuildWhereCluase(table, storeId, currentPrimaryKey);
                if (string.IsNullOrEmpty(whereClause)) return;
                var existsData = service.GetListWithSingleValue(tablename, whereClause, currentPrimaryKey, string.Empty, short.MaxValue)
                    .Select(x => Convert.ToString(x).ToLower()).ToList();
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    var item = table.Rows[i];
                    var currentPrimaryKeyValue = Convert.ToString(item[currentPrimaryKey]) ?? string.Empty;
                    if (existsData.Contains(currentPrimaryKeyValue.ToLower()))
                    {
                        willRemoveIndexs.Add(item);
                        whereClause = BuildWhereCluase(item, storeId, primaryKey);
                        if (string.IsNullOrEmpty(whereClause)) continue;
                        Update(service, tablename, item, primaryKey, whereClause);
                    }
                    continue;
                }
            }
            else
            {
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    var item = table.Rows[i];
                    var whereClause = BuildWhereCluase(item, storeId, primaryKey);
                    if (service.Exists(tablename, whereClause))
                    {
                        willRemoveIndexs.Add(item);
                        if (string.IsNullOrEmpty(whereClause)) continue;
                        Update(service, tablename, item, primaryKey, whereClause);
                    }
                }
            }
            foreach (var row in willRemoveIndexs)
            {
                table.Rows.Remove(row);
            }
            if (table.Rows.Count > 0)
            {
                SyncBigDataServiceHelper.BatchInsert(service.DbProviderConfig.DbConnectionString, tablename, table, table.Rows.Count);
            }
        }

        public void Update(IRawSqlSearchService service, string tablename, DataRow newRow, List<string> primaryKey, string whereClaue)
        {
            if (!_willUpdate) return;
            var colmunsCount = newRow.Table.Columns.Count;
            if (colmunsCount <= primaryKey.Count) return;
            StringBuilder builder = new StringBuilder(string.Format("update {0} set ", tablename));
            bool flag = false;
            foreach (DataColumn column in newRow.Table.Columns)
            {
                if (primaryKey.Any(x => string.Equals(x, column.ColumnName, StringComparison.OrdinalIgnoreCase)))
                    continue;
                var value = newRow[column.ColumnName];
                if (value == null)
                    value = GetDefaultValue(column.DataType);
                if (value != null)
                {
                    builder.AppendFormat("{0}{1}='{2}'", flag ? "," : "", column.ColumnName, value);
                    flag = true;
                }
            }
            if (!flag) return;
            builder.AppendFormat(" WHERE {0}", whereClaue);
            service.ExcuteSql(builder.ToString());
        }

        static bool IsNullableType(Type theType)
        {
            return (theType.IsGenericType && theType.
              GetGenericTypeDefinition().Equals
              (typeof(Nullable<>)));
        }
        private object GetDefaultValue(Type type)
        {
            if (IsNullableType(type)) return null;

            if (type == typeof(int) ||
                type == typeof(decimal) ||
                type == typeof(short) ||
                type == typeof(long) ||
                type == typeof(float) ||
                type == typeof(double))
            {
                return 0;
            }
            else if (type == typeof(DateTime))
            {
                return DateTime.Now;
            }
            else if (type == typeof(DateTime?))
            {
                return null;
            }
            else if (type == typeof(string))
            {
                return string.Empty;
            }
            else if (type == typeof(Guid))
            {
                return Guid.Empty;
            }
            return null;
        }
    }
}