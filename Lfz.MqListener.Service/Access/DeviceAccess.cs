// /*======================================================================
// *
// *        Copyright (C)  1996-2012  lfz    
// *        All rights reserved
// *
// *        Filename :DeviceAccess.cs
// *        DESCRIPTION :
// *
// *        Created By 林芳崽 at 2016-01-29 9:54
// *        https://git.oschina.net/lfz
// *
// *======================================================================*/

using System.Collections.Generic;
using System.Data;
using Lfz.Data.RawSql;
using Lfz.MqListener.Models.Device;

namespace Lfz.MqListener.Access
{
    public class DeviceAccess:BusinessService<DeviceBasicInfo>
    {
        public DeviceAccess(IDbProviderConfig providerConfig)
            : base(providerConfig)
        {
        }

        public override bool IsIdentity()
        {
            return false;
        }
        public override string TableName
        {
            get { return "Device_BasicInfo"; }
        }

        public override string PrimaryKey
        {
            get { return "Id"; }
        }

        public override IDictionary<string, object> GetDictionary(DeviceBasicInfo model)
        {
            var dic = base.GetDictionary(model); 
            return dic;
        }

        public override DeviceBasicInfo Format(DataRow row, DataColumnCollection columns = null)
        {
            var item = base.Format(row, columns); 
            return item;
        }
         
    }
}