// /*======================================================================
// *
// *        Copyright (C)  1996-2012  lfz    
// *        All rights reserved
// *
// *        Filename :MqMonitorServiceBus.cs
// *        DESCRIPTION :
// *
// *        Created By 林芳崽 at 2015-11-10 10:41
// *        https://git.oschina.net/lfz
// *
// *======================================================================*/

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Lfz.Mq;
using Lfz.Mq.ActiveMQ;
using Lfz.MqListener.CacheShared;
using Lfz.MqListener.Mq;
using Lfz.MqListener.MqVistor;
using Lfz.MqListener.MqVistor.Heartbeat;
using Lfz.MqListener.MqVistor.PushMessage;
using Lfz.MqListener.MqVistor.SyncTable;
using Lfz.MqListener.Service;
using Lfz.Services;
using Lfz.Utitlies;

namespace Lfz.MqListener
{
    /// <summary>
    /// 
    /// </summary>
    public partial class TCSoftServiceBus : ServiceBase
    {
        private readonly ActiveMQPoolConnectionManager _poolConnectionManager;
        private readonly MqListenerFactoryThreadService _mqListenerFactoryThreadService;

        /// <summary>
        /// 消息生产者
        /// </summary>
        public IMqProducerService MQProducerService { get; private set; }


        private readonly IMqConfigService _clusterService;
        private readonly IConfigCenterMqListenerService _listenerService;

        #region 构造函数、析构函数

        /// <summary>
        /// 
        /// </summary>
        public TCSoftServiceBus()
        {
            _listenerService = new ConfigCenterMqListenerService();
            _clusterService = new MqConfigService();
            _poolConnectionManager = new ActiveMQPoolConnectionManager(_clusterService);
            this.MQProducerService = new MqProducerService(_clusterService, _poolConnectionManager);
            var topicVistorList = new List<IMqCommandTopicVistor>()
            {
                new AutoUpdateCommandVistor(_listenerService )
            };
            var queueVistorList = new List<IMqCommandQuqueVistor>()
                {
                    new ChargeInfoTableMqCommandQuqueVistor(),
                    new AppHeartbeatCommandVistor(_listenerService),
                    new DeviceHeatbeatCommandVistor( ),
                    new AppRegisterCommandVistor(_listenerService),
                    new DeviceRegisterCommandVistor()
                };
            _mqListenerFactoryThreadService = new MqListenerFactoryThreadService(
                _clusterService,
                _listenerService,
                topicVistorList,
                queueVistorList);
            this.Starting += OnStarting;
            this.Stoped += OnStoped;
        }

        ~TCSoftServiceBus()
        {
            if (_mqListenerFactoryThreadService != null)
                _mqListenerFactoryThreadService.Dispose();
            _poolConnectionManager.CleanPool();
            base.Dispose();
        }

        #endregion

        #region 单例服务构建

        /// <summary>
        /// Gets the singleton Nop engine used to access Nop services.
        /// </summary>
        public static TCSoftServiceBus Current
        {
            get
            {
                if (Singleton<TCSoftServiceBus>.Instance == null)
                {
                    Initialize();
                }
                return Singleton<TCSoftServiceBus>.Instance;
            }
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        private static void Initialize()
        {
            if (Singleton<TCSoftServiceBus>.Instance == null)
            {
                Singleton<TCSoftServiceBus>.Instance = new TCSoftServiceBus();
            }
        }


        #endregion

        #region 启动停止事件处理程序

        private void OnStoped()
        {
            _clusterService.Reset();
            _mqListenerFactoryThreadService.Stop();
            if (_poolConnectionManager != null)
            {
                _poolConnectionManager.CleanPool();
            }
        }

        private void OnStarting()
        {
            _clusterService.Reset();
            if (_poolConnectionManager != null)
                _poolConnectionManager.CleanPool();
            _mqListenerFactoryThreadService.Start();
        }

        #endregion
    }
}