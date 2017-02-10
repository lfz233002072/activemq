// /*======================================================================
// *
// *        Copyright (C)  1996-2012  lfz    
// *        All rights reserved
// *
// *        Filename :IConfigCenterMqListenerService.cs
// *        DESCRIPTION :
// *
// *        Created By 林芳崽 at 2016-01-26 13:54
// *        https://git.oschina.net/lfz
// *
// *======================================================================*/

using System;
using Lfz.MqListener.Shared.App;
using Lfz.MqListener.Shared.Enums;

namespace Lfz.MqListener.Service
{
    /// <summary>
    /// 
    /// </summary>
    public interface IConfigCenterMqListenerService : IPerHttpRequestDependency
    {
        /// <summary>
        /// 开始部署
        /// </summary>
        /// <param name="clientid"></param>
        void Deploy(string clientid);

        void UpdateStatus(string clientid, AppStatus status);

        /// <summary>
        /// 初始化监听程序(MqListener)
        /// </summary>
        /// <param name="clientid"></param>
        void MqListenerInitialize(string clientid);

        /// <summary>
        /// 初始化App程序（远程客户端）
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="appType"></param>
        /// <param name="info"></param>
        void Initialize(Guid storeId, AppType appType, MqAppRegisterInfo info);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool MqListenerHeartbeat(string clientid, int totalCount, long totalMsgSize);

        /// <summary>
        /// 检测一次服务状态(做不做线清理)
        /// </summary>
        void VerifyListenStatus(AppType appType);

        void DownLown(string clientid);

        bool Heartbeat(Guid storeId, AppType appType, MqHeartbeatInfo beatinfo);
         
    }
}