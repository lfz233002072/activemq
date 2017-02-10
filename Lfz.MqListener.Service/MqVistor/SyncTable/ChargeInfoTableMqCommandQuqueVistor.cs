// /*======================================================================
// *
// *        Copyright (C)  1996-2012  lfz    
// *        All rights reserved
// *
// *        Filename :SyncTableMqCommandQuqueVistor.cs
// *        DESCRIPTION :
// *
// *        Created By 林芳崽 at 2016-01-21 13:58
// *        https://git.oschina.net/lfz
// *
// *======================================================================*/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Lfz.Data.RawSql;
using Lfz.Logging;
using Lfz.MqListener.MqVistor.SyncTable.ComandHandler;
using Lfz.Utitlies;

namespace Lfz.MqListener.MqVistor.SyncTable
{
    /// <summary>
    /// 营业报表业务处理
    /// </summary>
    public class ChargeInfoTableMqCommandQuqueVistor : SyncTableMqCommandQuqueVistorBase
    {
        const string RawJsonDataColumnName = "RawJsonData";
        private readonly ILogger _logger; 
        private readonly SyncInsertOrUpdateCommandHandler _insertCommandHandler; 

        public ChargeInfoTableMqCommandQuqueVistor()
        {
            _logger = LoggerFactory.GetLog(); 
            _insertCommandHandler=new SyncInsertOrUpdateCommandHandler(); 
        }

        private static readonly ConcurrentDictionary<string, MapInfo> MapColumns = new ConcurrentDictionary<string, MapInfo>();

        public override void InsertOrUpdate(  SyncTableContent table)
        {
            string tablename = table.TableName.ToLower();

            //TODO 具体插入数据库操作
             var mapInfo = Get( tablename);
            if (mapInfo == null) return;
            var dt = Fill(mapInfo, table);
            List<string> primaryKeys = (table.PrimaryKeys ?? new List<string>()); 
            InsertOrUpdate(tablename,  dt, primaryKeys); 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tablename"></param> 
        /// <param name="table"></param>
        /// <param name="primaryKey"></param>
        private void InsertOrUpdate(string tablename,    DataTable table, List<string> primaryKey)
        { 
            switch (tablename)
            {
                case "sync_charge": 
                    //TODO InsertOrUpdate
                    break;
                default:
                    _insertCommandHandler.InsertOrUpdate(tablename,Guid.Empty,null,table,primaryKey);
                    break;
            }
            return;
        }

        #region Help Methods

        private MapInfo Get( string tablename)
        {
            MapInfo map = null;
            MapColumns.TryGetValue(tablename, out map);
            if (map != null && map.ExpiredTime > DateTime.Now && map.Columns != null && map.Columns.Count > 0) return map;
            IDatabaseInitialise databaseInitialise = new DatabaseInitialise(null/*service.DbProviderConfig*/);
            var columns = databaseInitialise.GetColumnList(tablename);
            if (columns == null || columns.Count == 0) return map;
            map = new MapInfo();
            foreach (DataColumn dataColumn in columns)
            {
                var temp = new ManpColumnInfo()
                {
                    Name = dataColumn.ColumnName,
                    DataType = dataColumn.DataType,
                    AllowDbNull = dataColumn.AllowDBNull,
                    Format = GetFormat(dataColumn)
                };
                map.Columns.Add(temp);
            }
            MapColumns.AddOrUpdate(tablename, map, (key, oldvalue) => map);
            return map;
        }

        private Func<object, object> GetFormat(DataColumn column)
        {
            var type = column.DataType;

            if (type == typeof(int) || type == typeof(int?))
            {
                return o =>
                {
                    if (o == null) return 0;
                    var tempType = o.GetType();
                    if (tempType == typeof(int) || tempType == typeof(int?))
                        return o;
                    return TypeParse.StrToInt(o);
                };
            }
            else if (type == typeof(decimal) || type == typeof(decimal?) ||
                type == typeof(short) || type == typeof(short?) ||
                type == typeof(long) || type == typeof(long?) ||
                type == typeof(float) || type == typeof(float?) ||
                type == typeof(double) || type == typeof(double?))
            {
                return o =>
                {
                    if (o == null) return 0;
                    var tempType = o.GetType();
                    if (tempType == typeof(decimal) || tempType == typeof(decimal?))
                        return o;
                    return TypeParse.StrToDecimal(o, 0);
                };
            }
            else if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                return o =>
                {
                    if (o == null) return column.AllowDBNull ? DateTime.Now : (DateTime?)null;
                    var tempType = o.GetType();
                    if (tempType == typeof(DateTime) || tempType == typeof(DateTime?))
                        return o;
                    return column.AllowDBNull ? TypeParse.ConvertToDateTime(o) : TypeParse.ConvertToNullableDateTime(o);
                };
            }
            else if (type == typeof(string))
            {
                return o =>
                {
                    if (o == null) return string.Empty;
                    var tempType = o.GetType();
                    if (tempType == typeof(string))
                        return o;
                    return o.ToString();
                };
            }
            return x => Convert.ChangeType(x, type);
        }

        private DataTable GetTable(MapInfo info)
        {
            DataTable dt = new DataTable();
            var type = typeof(int);
            var type2 = typeof(long);
            foreach (var colmuns in info.Columns)
            {
                //如果字段未Id字段，且类型为int的那么表示自增长字段
                if (string.Equals(colmuns.Name, "Id", StringComparison.OrdinalIgnoreCase)
                    && (colmuns.DataType == type || colmuns.DataType == type2))
                    continue;
                var columninfo = dt.Columns.Add(colmuns.Name, colmuns.DataType);
                columninfo.AllowDBNull = colmuns.AllowDbNull;
            }
            return dt;
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

        private DataTable Fill(MapInfo info, SyncTableContent content)
        {
            DataTable dt = GetTable(info);
            for (int i = 0; i < content.Rows.Count; i++)
            {
                var row = dt.NewRow();
                dt.Rows.Add(row);
            }
            for (int columnIndex = 0; columnIndex < content.Columns.Count; columnIndex++)
            {
                var columnName = content.Columns[columnIndex];
                if (!dt.Columns.Contains(columnName)) continue;
                var columnInfo = dt.Columns[columnName];
                try
                {
                    Func<object, object> formatFunc = null;
                    var mapColumnInfo =
                        info.Columns.FirstOrDefault(
                            x => string.Equals(x.Name, columnName, StringComparison.OrdinalIgnoreCase));
                    if (mapColumnInfo != null)
                        formatFunc = mapColumnInfo.Format;
                    if (formatFunc == null)
                        formatFunc = GetFormat(columnInfo);
                    for (int i = 0; i < content.Rows.Count; i++)
                    {
                        var dataArray = content.Rows[i];
                        //如果当前单元格不允许为空，那么设置默认值
                        if (columnIndex >= dataArray.Length && !columnInfo.AllowDBNull)
                        {
                            dt.Rows[i][columnName] = GetDefaultValue(columnInfo.DataType);
                            continue;
                        }
                        //设置第I行第n列的值
                        dt.Rows[i][columnName] = formatFunc(dataArray[columnIndex]);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, string.Format("列设置出错 ColumnName:{0} DataType:{1}", columnInfo.ColumnName, columnInfo.DataType));
                }
            }
            return dt;
        }

        #endregion

        #region HelpClass

        /// <summary>
        /// 
        /// </summary>
        class MapInfo
        {
            public List<ManpColumnInfo> Columns { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public DateTime ExpiredTime { get; set; }

        }

        /// <summary>
        /// 
        /// </summary>
        class ManpColumnInfo
        {
            public string Name { get; set; }

            public bool AllowDbNull { get; set; }
            public Type DataType { get; set; }

            public Func<object, object> Format { get; set; }
        }


        #endregion
    }
}