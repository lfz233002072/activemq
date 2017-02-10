using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Lfz.Data.RawSql;

namespace Lfz.MqListener.MqVistor.SyncTable.ComandHandler
{
    public class SyncChargeCjCommandHandler : ISyncTableComand
    {

        private string GetChargeNoList(DataTable table)
        {
            string result = "";
            foreach (DataRow dataRow in table.Rows)
            {
                result += "," + dataRow["ChargeNo"];
            }
            return result.Trim(',');
        }

        public void InsertOrUpdate(string tablename, Guid storeId, IRawSqlSearchService service, DataTable table, List<string> primaryKey)
        {
            string chargeIdList = GetChargeNoList(table);
            var whereClause = string.Format(" WHERE ChargeNo IN({0})  and StoreId='{1}'", chargeIdList, storeId);
            var existsData = service.GetListWithSingleValue(tablename, whereClause, "ChargeNo", string.Empty, short.MaxValue)
                .Select(x => Convert.ToString(x).ToLower()).ToList();
            if (!table.Columns.Contains("ChargeNo")) return;
            List<DataRow> willRemoveIndexs = new List<DataRow>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                var item = table.Rows[i];
                var chargeNo = item["ChargeNo"].ToString().ToLower();
                if (existsData.Contains(chargeNo))
                {
                    willRemoveIndexs.Add(item);
                    continue;
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
            //账单重交时需要清理原来账单数据
            service.ExcuteSql("delete from Sync_charge " + whereClause);
            service.ExcuteSql("delete from Sync_consumeperm  " + whereClause);
            service.ExcuteSql("delete from Sync_consumeperm2  " + whereClause);
        }
    }
}