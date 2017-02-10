// /*======================================================================
// *
// *        Copyright (C)  1996-2012  lfz    
// *        All rights reserved
// *
// *        Filename :ConfigCenterClusterComputerAccess.cs
// *        DESCRIPTION :
// *
// *        Created By 林芳崽 at 2016-01-26 15:44
// *        https://git.oschina.net/lfz
// *
// *======================================================================*/

using System.Collections.Generic;
using System.Data;
using Lfz.Data.RawSql;
using Lfz.MqListener.Models.ConfigCenter;
using Lfz.MqListener.Shared.Enums;
using Lfz.Utitlies;

namespace Lfz.MqListener.Access
{
    public class ConfigCenterClusterComputerAccess : BusinessService<ConfigCenterClusterComputer>
    {
        public ConfigCenterClusterComputerAccess(IDbProviderConfig providerConfig)
            : base(providerConfig)
        {
        }

        public override string TableName
        {
            get { return "ConfigCenter_ClusterComputer"; }
        }

        public override string PrimaryKey
        {
            get { return "Id"; }
        }

        public override IDictionary<string, object> GetDictionary(ConfigCenterClusterComputer model)
        {
            var dic = base.GetDictionary(model);
            if (dic.ContainsKey("ComputerType"))
                dic["ComputerType"] = (int)model.ComputerType;
            else dic.Add("ComputerType", (int)model.ComputerType);
            return dic;
        }

        public override ConfigCenterClusterComputer Format(DataRow row, DataColumnCollection columns = null)
        {
            var item = base.Format(row, columns);
            if (item != null)
                item.ComputerType = Utils.GetEnum<ClusterComputerType>(row["ComputerType"]);
            return item;
        }


        public int Count(ClusterComputerType mComputerType)
        {
            return SearchService.Count(TableName, string.Format("ComputerType={0}",
                (int)mComputerType));
        }

        public List<ConfigCenterClusterComputer> GetComputerList(ClusterComputerType mComputerType, int count)
        {

            return GetList(string.Format("ComputerType={0}",
                (int)mComputerType), "*", "IpAddress,Port", count);
        }
    }
}