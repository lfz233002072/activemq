// /*======================================================================
// *
// *        Copyright (C)  1996-2012  lfz    
// *        All rights reserved
// *
// *        Filename :MqListenerFactoryThreadService.cs
// *        DESCRIPTION :
// *
// *        Created By 林芳崽 at 2016-01-20 14:22
// *        https://git.oschina.net/lfz
// *
// *======================================================================*/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Lfz.Caching;
using Lfz.Logging;
using Lfz.Mq;
using Lfz.MqListener.MqVistor;
using Lfz.MqListener.Service;
using Lfz.MqListener.Shared.Enums;
using Lfz.MqListener.Shared.Models;
using Lfz.Services;

namespace Lfz.MqListener.Mq
{
    /// <summary>
    /// 监听服务器
    /// </summary>
    public class MqListenerFactoryThreadService : ThreadServiceBase
    {
        private readonly IMqConfigService _configCenterService;
        private readonly IEnumerable<IMqCommandTopicVistor> _topicVistors;
        private readonly IEnumerable<IMqCommandQuqueVistor> _queueVistors;
        private readonly FileCacheManager<ConcurrentDictionary<int, MqInstanceInfo>> _cacheManager;
        private static readonly Guid CacheKey = Guid.Parse("1F793467-7C85-40D5-8D80-2562EE793031");
        private readonly ConcurrentDictionary<string, ExcuteInfo> _excuteInfoDictionary;
        private readonly IConfigCenterMqListenerService _configCenterMqListenerService;
        private object LockExcuteInfoObj = new object();

        /// <summary>
        /// 
        /// </summary>
        readonly ConcurrentDictionary<int, MqListenerService> _serviceList;

        /// <summary>
        /// 已经删除的服务
        /// </summary>
        private readonly ConcurrentDictionary<int, MqInstanceInfo> _delInstanceInfos;

        /// <summary>
        /// 当前程序APPID
        /// </summary>
        private readonly string _listenerAppId;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configCenterService"></param>
        /// <param name="topicVistors"></param>
        /// <param name="queueVistors"></param>
        public MqListenerFactoryThreadService(
            IMqConfigService configCenterService,
            IConfigCenterMqListenerService listenerService,
            IEnumerable<IMqCommandTopicVistor> topicVistors,
              IEnumerable<IMqCommandQuqueVistor> queueVistors)
        {
            _listenerAppId = ProcessLockHelper.GetProcessLockId();
            _configCenterMqListenerService = listenerService;
            _cacheManager = new FileCacheManager<ConcurrentDictionary<int, MqInstanceInfo>>((int)CacheRegionName.FileMqDeleteInstace);
            this._configCenterService = configCenterService;
            _topicVistors = topicVistors;
            _queueVistors = queueVistors;
            _serviceList = new ConcurrentDictionary<int, MqListenerService>();
            _delInstanceInfos = new ConcurrentDictionary<int, MqInstanceInfo>();
            _excuteInfoDictionary = new ConcurrentDictionary<string, ExcuteInfo>();
            Stoped += OnStoped;
            Stoping += OnStoping;
            Starting += OnStarting;
        }

        private void OnStarting()
        {
            try
            {
                //更新当前程序允许的版本信息
                _configCenterMqListenerService.MqListenerInitialize(_listenerAppId);
                _delInstanceInfos.Clear();
                var list = _cacheManager.Get(CacheKey);
                if (list != null && !list.IsEmpty)
                {
                    foreach (var item in list)
                    {
                        _delInstanceInfos.TryAdd(item.Key, item.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, this.ServiceName + ".OnStarting");
            }
        }

        private void OnStoping()
        {
            try
            {
                foreach (var mqListenerService in _serviceList)
                {
                    mqListenerService.Value.Stop();
                    mqListenerService.Value.Dispose();
                }
                _serviceList.Clear();
                _configCenterMqListenerService.DownLown(_listenerAppId);
            }
            catch (Exception ex)
            {

                Logger.Error(ex, this.ServiceName + ".OnStoping");
            }
        }

        private void OnStoped()
        {
            try
            {
                if (!_delInstanceInfos.IsEmpty)
                {
                    _cacheManager.AddOnlyFileCache(CacheKey, _delInstanceInfos);
                    _delInstanceInfos.Clear();
                }
                int totalCount = 0;
                long totalMsg = 0;
                foreach (var info in _excuteInfoDictionary)
                {
                    totalCount += info.Value.TotalCount;
                    totalMsg += info.Value.TotalSize;
                }
                Logger.Information(string.Format("{0}.OnStoped 总接收消息数量:{1}条 接收消息大小：{2}Kb", this.ServiceName, totalCount, totalMsg / 1024));
                _excuteInfoDictionary.Clear();
                _configCenterMqListenerService.DownLown(_listenerAppId);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, this.ServiceName + ".OnStoped");
            }
        }


        private void ServiceOnTopicDispatch(TopicName topicName, MqCommandInfo commandInfo)
        {
            foreach (var mqCommandTopicVistor in _topicVistors)
            {
                commandInfo.ExcuteCount++;
                mqCommandTopicVistor.Vistor(topicName, commandInfo);
            }
            ExcuteInfo info;
            _excuteInfoDictionary.TryGetValue(commandInfo.ClientId, out info);
            if (info != null)
            {
                lock (LockExcuteInfoObj)
                {
                    info.TotalCount++;
                    info.TotalSize += commandInfo.Length;
                }
            }
        }

        private void ServiceOnQuqueDispatch(QuqueName ququeName, MqCommandInfo commandInfo)
        {
            foreach (var mqCommandTopicVistor in _queueVistors)
            {
                commandInfo.ExcuteCount++;
                mqCommandTopicVistor.Vistor(ququeName, commandInfo);
            }
            ExcuteInfo info;
            _excuteInfoDictionary.TryGetValue(commandInfo.ClientId, out info);
            if (info != null)
            {
                lock (LockExcuteInfoObj)
                {
                    info.TotalCount++;
                    info.TotalSize += commandInfo.Length;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        protected override void Excute(object obj)
        {
            List<MqInstanceInfo> instanceList = new List<MqInstanceInfo>();
            long count = 0;
            //将从数据库中直接删除的数据
            var expiredInstanceIdList = new List<int>();
            while (IsRunning)
            {
                count++;
                try
                {
                    //1分钟一次心跳矫正
                    Heart(count);

                    if (count % 10 != 0 && count > 1)
                    {
                        //10*10秒 
                        Thread.Sleep(10000);
                        continue;
                    }

                    // 1、构造消息配置列表(30分钟重新构造一次)
                    if (instanceList.Count == 0 || count % 180 == 0)
                    {
                        if (instanceList.Count > 0) instanceList.Clear();
                        if (expiredInstanceIdList.Count > 0) expiredInstanceIdList.Clear();
                        InitMqInstance(instanceList, _delInstanceInfos, expiredInstanceIdList);
                    }

                    // 2、 查找出所有带监听服务,清理掉过期服务  
                    VerifyListenService(_serviceList);

                    // 3、过期消息队列删除 (根据数据库中检查出的数据进行对比)
                    RemoveExpiredService(_serviceList, expiredInstanceIdList);

                    // 4、已经删除的实例配置构造  （合并当前运行服务的情况）
                    RemoveExpiredMqInstance(_delInstanceInfos, expiredInstanceIdList);

                    // 5、更新服务
                    AddOrUpdateService(_serviceList, instanceList, _delInstanceInfos);

                    // 6、从数据库中删除配置信息
                    if (expiredInstanceIdList.Count > 0)
                    {
                        _configCenterService.Delete(expiredInstanceIdList);
                        expiredInstanceIdList.Clear();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, this.ServiceName + ".Excute");
                }
                //每分钟检测一次通信有效性
                Thread.Sleep(10000);
            }
        }

        #region Excute 内部方法

        /// <summary>
        /// 1、构造消息配置列表
        /// </summary>
        /// <param name="list"></param>
        /// <param name="delInstanceList"></param>
        /// <param name="expiredInstanceIdList"></param>
        private void InitMqInstance(List<MqInstanceInfo> list, ConcurrentDictionary<int, MqInstanceInfo> delInstanceList, List<int> expiredInstanceIdList)
        {
            //30分钟进行一次数据清理
            var templist = _configCenterService.GetAll().ToList();
            var templist2 = templist.Where(x => x.DelFalg);
            foreach (var mqInstanceInfo in templist2)
            {
                //已经到过期时间的，直接清理数据
                if (mqInstanceInfo.ExpiredTime < DateTime.Now)
                {
                    expiredInstanceIdList.Add(mqInstanceInfo.MqInstanceId);
                    continue;
                }
                //添加到待删除列表中
                delInstanceList.TryAdd(mqInstanceInfo.MqInstanceId, mqInstanceInfo);
            }
            //重构消息队列配置列表 
            list.AddRange(templist.Where(x => !x.DelFalg));
        }

        /// <summary>
        /// 2、查找出所有带监听服务,清理掉过期服务(返回过期服务的实例ID)
        /// </summary>
        private void VerifyListenService(ConcurrentDictionary<int, MqListenerService> services)
        {
            var temList = new List<int>();
            //清理已经过期项
            foreach (var mqListenerService in services)
            {
                MqListenerService service = mqListenerService.Value;
                if (service.IsRunning && service.ConnectionCheck()) continue;
                service.Stop();

                temList.Add(mqListenerService.Key);
                ExcuteInfo info;
                _excuteInfoDictionary.TryRemove(service.ClientId, out info);
                if (info == null) continue;
                Logger.Information(string.Format("移除消息监听服务 ClientId：{0} Count:{1} size:{2}", service.ClientId, info.TotalCount, info.TotalSize));
            }
            //移除已经停止的服务
            foreach (var i in temList)
            {
                MqListenerService service;
                services.TryRemove(i, out service);
            }
        }

        /// <summary>
        /// 3\过期消息队列删除(数据库中检测出属于已经过期的)
        /// </summary>
        /// <param name="services"></param>
        /// <param name="expiredInstanceIdList"></param>
        private void RemoveExpiredService(ConcurrentDictionary<int, MqListenerService> services, List<int> expiredInstanceIdList)
        {
            foreach (var instanceid in expiredInstanceIdList)
            {
                //过期消息队列删除
                MqListenerService service;
                services.TryGetValue(instanceid, out service);
                if (service == null) continue;
                service.Stop();

                ExcuteInfo info;
                _excuteInfoDictionary.TryRemove(service.ClientId, out info);
                if (info == null) continue;
                Logger.Information(string.Format("移除消息监听服务 ClientId：{0} Count:{1} size:{2}", service.ClientId, info.TotalCount, info.TotalSize));
            }
        }

        /// <summary>
        /// 4移除过期实例(服务实例中检测出已经过期的)
        /// </summary>
        /// <param name="delInstanceList"></param>
        /// <param name="expiredInstanceIdList"></param>
        private void RemoveExpiredMqInstance(ConcurrentDictionary<int, MqInstanceInfo> delInstanceList, List<int> expiredInstanceIdList)
        {
            var tempList = (from delInstanceInfo in delInstanceList
                            where delInstanceInfo.Value.ExpiredTime < DateTime.Now
                            select delInstanceInfo.Key).ToList();
            foreach (var i in tempList)
            {
                MqInstanceInfo instance;
                delInstanceList.TryRemove(i, out instance);
                if (!expiredInstanceIdList.Contains(i)) expiredInstanceIdList.Add(i);
            }
        }

        /// <summary>
        /// 5\更新服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="instanceList"></param>
        /// <param name="delInstanceList"></param>
        private void AddOrUpdateService(ConcurrentDictionary<int, MqListenerService> services,
            List<MqInstanceInfo> instanceList, ConcurrentDictionary<int, MqInstanceInfo> delInstanceList)
        {
            foreach (var mqInstanceInfo in instanceList)
            {
                MqListenerService service;
                if (services.TryGetValue(mqInstanceInfo.MqInstanceId, out service))
                {
                    service.UpdateConfig(mqInstanceInfo);
                    continue;
                }
                AddService(mqInstanceInfo);
            }
            foreach (var delInstanceInfo in delInstanceList)
            {
                MqListenerService service;
                if (services.TryGetValue(delInstanceInfo.Key, out service))
                {
                    service.UpdateConfig(delInstanceInfo.Value);
                    continue;
                }
                AddService(delInstanceInfo.Value);
            }
        }

        private void AddService(MqInstanceInfo instanceInfo)
        {
            if (!IsRunning) return;
            var service = new MqListenerService(_listenerAppId, instanceInfo);
            if (_serviceList.TryAdd(instanceInfo.MqInstanceId, service))
            {
                service.QuqueDispatch += ServiceOnQuqueDispatch;
                service.TopicDispatch += ServiceOnTopicDispatch;
                service.Start();
                _excuteInfoDictionary.TryAdd(service.ClientId, new ExcuteInfo());
            }
            else service.Dispose();
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        protected void Heart(long excuteCount)
        {
            try
            {
                int totalCount = 0;
                long totalMsgSize = 0;
                foreach (var info in _excuteInfoDictionary)
                {
                    totalCount += info.Value.TotalCount;
                    totalMsgSize += info.Value.TotalSize;
                }
                _configCenterMqListenerService.MqListenerHeartbeat(_listenerAppId, totalCount, totalMsgSize);
                //(修正无效的设备,1800秒=60*30S=3分钟)
                if (excuteCount % 18 == 0)
                    _configCenterMqListenerService.VerifyListenStatus(AppType.MqListener);

                Logger.Trace(string.Format("{0}Heart 总接收消息数量:{1}条 接收消息大小：{2}Kb", this.ServiceName, totalCount, totalMsgSize / 1024));
            }
            catch (Exception ex)
            {
                Logger.Error(ex, this.ServiceName + "Heart");
            }
        }

        #region HelperClass

        class ExcuteInfo
        {
            /// <summary>
            /// 接收数量
            /// </summary>
            public int TotalCount { get; set; }

            /// <summary>
            /// 接收文件大小
            /// </summary>
            public long TotalSize { get; set; }
        }

        #endregion
    }
}