using System;
using System.Collections.Generic;
using System.Linq;
using Lfz.MqListener.Access;
using Lfz.MqListener.Models.ConfigCenter;
using Lfz.MqListener.Shared.Enums;
using Lfz.Redis;

namespace Lfz.MqListener.CacheShared
{
    /// <summary>
    /// 
    /// </summary>
    public class RedisConfigService : IRedisConfigService
    {
        #region IRedisConfigService接口实现 
        private readonly ConfigCenterClusterComputerAccess _computerAccess;
        private readonly ConfigCenterRedisCacheKeyAccess _cacheKeyAccess;

        public RedisConfigService()
        { 
            _computerAccess = new ConfigCenterClusterComputerAccess(DbConfigHelper.GetConfig());
            _cacheKeyAccess = new ConfigCenterRedisCacheKeyAccess(DbConfigHelper.GetConfig());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public RedisConfig GetRedisConfig(string key)
        {

            ConfigCenterClusterComputer configComputer = null;
            var item = _cacheKeyAccess.GetByCacheKey(key);
            if (item != null && item.RedisInstanceId != 0)
                configComputer = _computerAccess.Get(item.RedisInstanceId);
            string ipAddress;
            if (configComputer != null)
            {
                ipAddress = configComputer.IpAddress;
                if (!string.IsNullOrEmpty(configComputer.AccessPassword))
                    ipAddress = configComputer.AccessPassword + "@" + ipAddress;
                if (configComputer.Port != 0)
                {
                    ipAddress += ":" + configComputer.Port;
                }
                return new RedisConfig()
                {
                    ReadOnlyHosts = ipAddress,
                    ReadWriteHosts = ipAddress,
                    ConfigId = configComputer.Id
                };
            }
            var list = _computerAccess.GetComputerList(ClusterComputerType.Redis, short.MaxValue)
                .ToList();
            if (list.Count == 0) return null;
            List<ConfigCenterClusterComputer> renewList = new List<ConfigCenterClusterComputer>();
            for (int i = 0; i < list.Count(); i++)
            {
                var computer = list[i];
                if (computer.Weight <= 1) renewList.Add(computer);
                else
                {
                    for (int j = 0; j < computer.Weight; j++)
                    {
                        renewList.Add(computer);
                    }
                }
            }
            //保证相同的机器编码计算出来的结果一致
            var hashcode = GetMyHashCode(key);
            int index = hashcode % renewList.Count;
            if (index < renewList.Count) configComputer = renewList[index];
            else configComputer = renewList[0];
            ipAddress = configComputer.IpAddress;
            if (!string.IsNullOrEmpty(configComputer.AccessPassword))
                ipAddress = configComputer.AccessPassword + "@" + ipAddress;
            if (configComputer.Port != 0)
            {
                ipAddress += ":" + configComputer.Port;
            }
            if (item != null)
            {
                item.RedisInstanceId = configComputer.Id;
                item.CreateTime = DateTime.Now;
                item.Expired = 1440;
                _cacheKeyAccess.Update(item);
            }
            else
            {
                item = new ConfigCenterRedisCacheKey()
                {
                    CacheKey = key,
                    RedisInstanceId = configComputer.Id,
                    CreateTime = DateTime.Now,
                    Expired = 1440
                };
                _cacheKeyAccess.Create(item);
            }
            return new RedisConfig()
            {
                ReadOnlyHosts = ipAddress,
                ReadWriteHosts = ipAddress,
                ConfigId = configComputer.Id,
                ExpiredTime = DateTime.Now.AddMinutes(item.Expired == 0 ? 1440 : item.Expired)
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RedisConfig> GetAllRedisConfig()
        {
            var list = _computerAccess.GetComputerList(ClusterComputerType.Redis, short.MaxValue);
            foreach (var configComputer in list)
            {
                var ipAddress = configComputer.IpAddress;
                if (!string.IsNullOrEmpty(configComputer.AccessPassword))
                    ipAddress = configComputer.AccessPassword + "@" + ipAddress;
                if (configComputer.Port != 0)
                {
                    ipAddress += ":" + configComputer.Port;
                }
                yield return new RedisConfig()
                {
                    ReadOnlyHosts = ipAddress,
                    ReadWriteHosts = ipAddress,
                    ConfigId = configComputer.Id
                };
            }
        }

        RedisConfig IRedisConfigService.Get(string key)
        {
            return GetRedisConfig(key);
        }
        IEnumerable<RedisConfig> IRedisConfigService.GetAll()
        {
            return GetAllRedisConfig();
        }


        private int GetMyHashCode(string key)
        {
            int value = 0;
            if (string.IsNullOrEmpty(key)) return 0;
            int offset = 22;
            for (int i = 0; i < key.Length; i++)
            {
                value = key[i].GetHashCode() << offset;
                offset--;
                if (offset < 1) offset = 32;
            }
            return value;
        }
        #endregion
         
    }
}