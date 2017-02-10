using System;
using System.Collections.Generic;
using System.Data;
using Lfz.Data.RawSql;

namespace Lfz.MqListener.MqVistor.SyncTable
{
    public interface ISyncTableComand:IPerHttpRequestDependency
    {
        void InsertOrUpdate
            (string tablename, Guid storeId, IRawSqlSearchService service, DataTable table, List<string> primaryKey);
    }
}