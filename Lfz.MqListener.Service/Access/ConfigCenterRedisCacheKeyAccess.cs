// /*======================================================================
// *
// *        Copyright (C)  1996-2012  lfz    
// *        All rights reserved
// *
// *        Filename :ConfigCenterRedisCacheKeyAccess.cs
// *        DESCRIPTION :
// *
// *        Created By 林芳崽 at 2016-01-26 15:53
// *        https://git.oschina.net/lfz
// *
// *======================================================================*/

using Lfz.Data.RawSql;
using Lfz.MqListener.Models.ConfigCenter;

namespace Lfz.MqListener.Access
{
    public class ConfigCenterRedisCacheKeyAccess: BusinessService<ConfigCenterRedisCacheKey>
    {

        public ConfigCenterRedisCacheKeyAccess(IDbProviderConfig providerConfig)
            : base(providerConfig)
        {
        }

        public ConfigCenterRedisCacheKey GetByCacheKey(string key)
        {
            return Get(string.Format("CacheKey='{0}'", key));
        }

        public override bool IsIdentity()
        {
            return false;
        }

        public override string TableName
        {
            get { return "ConfigCenter_RedisCacheKey"; }
        }

        public override string PrimaryKey
        {
            get { return "CacheKey"; }
        } 
    }
}