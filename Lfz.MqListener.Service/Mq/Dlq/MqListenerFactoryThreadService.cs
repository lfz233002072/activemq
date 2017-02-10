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
using System.Collections.Generic;
using System.Linq;
using Lfz.Logging;
using Lfz.Mq;
using Lfz.MqListener.CacheShared;
using Lfz.Services;

namespace Lfz.MqListener.Mq.Dlq
{
    /// <summary>
    /// 监听服务器
    /// </summary>
    public class ActiveMqdlqFactoryThreadService : ServiceBase
    {
        private readonly IMqConfigService _configCenterService;
        private readonly List<ActiveMqdlqMqListenerService> _services;

        /// <summary>
        /// 
        /// </summary> 
        public ActiveMqdlqFactoryThreadService(
           )
        {
            this._configCenterService = new MqConfigService();
            _services = new List<ActiveMqdlqMqListenerService>();
            Stoping+=OnStoping;
            Started+=OnStarted;
        }

        private void OnStarted()
        {
            Excute(null);
        }

        private void OnStoping()
        { 
            foreach (var mqListenerService in _services)
            {
                mqListenerService.Stop();
            }
            _services.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        private void Excute(object obj)
        {
            try
            {
                foreach (var mqListenerService in _services)
                {
                    mqListenerService.Stop();
                }
                _services.Clear();
                var templist = _configCenterService.GetAll().ToList();
                foreach (var mqInstanceInfo in templist)
                {
                    var service = new ActiveMqdlqMqListenerService(mqInstanceInfo);
                    service.Start();
                    _services.Add(service);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, this.ServiceName + "Excute");
            }
        }
         
    }
}