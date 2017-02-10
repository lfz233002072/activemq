// /*======================================================================
// *
// *        Copyright (C)  1996-2012  lfz    
// *        All rights reserved
// *
// *        Filename :MqConfigService.cs
// *        DESCRIPTION :
// *
// *        Created By 林芳崽 at 2016-01-26 15:06
// *        https://git.oschina.net/lfz
// *
// *======================================================================*/

using System;
using System.Collections.Generic;
using System.Linq;
using Lfz.Caching;
using Lfz.Data.RawSql;
using Lfz.Logging;
using Lfz.Mq;
using Lfz.MqListener.Access;
using Lfz.MqListener.Models.ConfigCenter;
using Lfz.MqListener.Shared.Enums;

namespace Lfz.MqListener.CacheShared
{
    /// <summary>
    /// 
    /// </summary>
    public class MqConfigService : IMqConfigService
    {
        private readonly ILogger _logger;
        private readonly RedisCacheManager _cacheManager;
        private readonly IRawSqlSearchService _searchService;
        private readonly ConfigCenterClusterComputerAccess _computerAccess;

        /// <summary>
        /// 
        /// </summary>
        public MqConfigService()
        {
            _searchService = new RawSqlSearchService(DbConfigHelper.GetConfig());
            _computerAccess =new ConfigCenterClusterComputerAccess(DbConfigHelper.GetConfig());
            _cacheManager = new RedisCacheManager((int)CacheRegionName.MqConfig);
            _logger = LoggerFactory.GetLog();
        }

        MqClientConfigInfo IMqConfigService.GetMqConfig(Guid storeId, bool useLanIpAddress)
        {
            return _cacheManager.Get(storeId, x => GetMqConfig(x, useLanIpAddress));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="useLanIpAddress"></param>
        /// <returns></returns>
        public MqClientConfigInfo GetMqConfig(Guid storeId, bool useLanIpAddress)
        {
            int minutes = 1440;// 24*60分钟=1440分钟 

            ConfigCenterClusterComputer configComputer = null;
            var item = GetMqTable(storeId);
            if (item != null && item.MqInstanceId.HasValue)
            {
                configComputer = _computerAccess.Get(item.MqInstanceId.Value);
            }
            if (configComputer != null && configComputer.DelFlag == false)
            {
                //如果当前配置有效，那么刷新配置
                item.CreateTime = DateTime.Now;
                item.Expired = minutes;
                _searchService.ExcuteSql(string.Format("UPDATE ConfigCenter_MqTable SET CreateTime=GETDATE(),Expired={0} where StoreId='{1}'", minutes, storeId));
                return FillModel(item, configComputer, useLanIpAddress);
            }
            var count = _computerAccess.Count(ClusterComputerType.MessageQueue);
            if (count == 0) return null;
            int index = storeId.GetHashCode() % count;
            if (index >= count - 1) index = count - 1;
            if (index < 0) index = 0;
            configComputer = _computerAccess.GetComputerList(ClusterComputerType.MessageQueue, index).LastOrDefault();
            if (configComputer != null)
            {
                if (item != null)
                {
                    //如果当前配置有效，那么刷新配置
                    item.CreateTime = DateTime.Now;
                    item.Expired = minutes;
                    item.MqInstanceId = configComputer.Id;
                    _searchService.ExcuteSql(
                        string.Format("UPDATE ConfigCenter_MqTable SET CreateTime=GETDATE(),Expired={0},MqInstanceId={2} where StoreId='{1}'",
                        minutes, storeId, configComputer.Id));
                }
                else
                {
                    //如果当前配置有效，那么刷新配置
                    item = new ConfigCenterMqTable()
                    {
                        CreateTime = DateTime.Now,
                        StoreId = storeId,
                        MqInstanceId = configComputer.Id
                    };
                    item.CreateTime = DateTime.Now;
                    item.Expired = minutes;
                    _searchService.Create("ConfigCenter_MqTable", "StoreId", item);
                }
                return FillModel(item, configComputer, useLanIpAddress);
            }

            return null;
        }

        private MqInstanceInfo FillModel(ConfigCenterClusterComputer data)
        {
            bool flag = !string.IsNullOrEmpty(data.LanIpAddress);
            return new MqInstanceInfo()
            {
                AccessPassword = data.AccessPassword,
                AccessUsername = data.AccessUsername,
                IpAddress = flag ? data.LanIpAddress : data.IpAddress,
                MqttPort = data.MqttPort,
                Port = data.Port,
                MqInstanceId = data.Id,
                DelFalg = data.DelFlag,
                ExpiredTime = (data.LastUpdateTime ?? DateTime.Now).AddHours(28)
            };
        }

        private MqClientConfigInfo FillModel(ConfigCenterMqTable table, ConfigCenterClusterComputer data, bool useLanIpAddress)
        {
            bool flag = !string.IsNullOrEmpty(data.LanIpAddress) && useLanIpAddress;
            return new MqClientConfigInfo()
            {
                ClientId = table.StoreId,
                AccessPassword = data.AccessPassword,
                AccessUsername = data.AccessUsername,
                IpAddress = flag ? data.LanIpAddress : data.IpAddress,
                MqttPort = data.MqttPort,
                Port = data.Port,
                MqInstanceId = data.Id,
                MqListenerId = table.MqListenerId ?? 0,
                ExpiredTime = DateTime.Now.AddMinutes(table.Expired)
            };
        }

        public IEnumerable<MqInstanceInfo> GetAll()
        {
            return _cacheManager.Get("ConfigCenterService.GetAll", key =>_computerAccess. GetComputerList(ClusterComputerType.MessageQueue,  short.MaxValue).Select(FillModel));
        }

        /// <summary>
        /// 重置配置
        /// </summary>
        public void Reset()
        {
            try
            {

                _cacheManager.Remove("ConfigCenterService.GetAll");
                _cacheManager.RemoveRegion();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Reset");
            }
        }

        private ConfigCenterMqTable GetMqTable(Guid storeId)
        {
            var row = _searchService.GetModel("ConfigCenter_MqTable", string.Format("StoreId='{0}'", storeId));
            if (row != null)
                return _searchService.Format<ConfigCenterMqTable>(row);
            return null;
        }
 

        public bool Delete(IEnumerable<int> idlist)
        {
            foreach (var i in idlist)
            {
                _computerAccess.Delete(i); 
            }
            return true;
        }
    }
}