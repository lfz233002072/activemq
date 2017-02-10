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

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Lfz.AutoUpdater.Config;
using Lfz.Data.RawSql;
using Lfz.Logging;
using Lfz.MqListener.Shared.App;
using Lfz.MqListener.Shared.Enums;
using Lfz.Utitlies;

namespace Lfz.MqListener.Access
{
    public class AppBasicInfoAccess : BusinessService<AppBasicInfo>
    {
        private readonly ILogger _logger;
        public AppBasicInfoAccess(IDbProviderConfig providerConfig)
            : base(providerConfig)
        {
            _logger = LoggerFactory.GetLog();
        }

        public override string TableName
        {
            get { return "App_BasicInfo"; }
        }

        public override string PrimaryKey
        {
            get { return "Id"; }
        }

        public override IDictionary<string, object> GetDictionary(AppBasicInfo model)
        {
            var dic = base.GetDictionary(model);
            if (dic.ContainsKey("AppType"))
                dic["AppType"] = (int)(model.AppType);
            else dic.Add("AppType", (int)(model.AppType));
            if (dic.ContainsKey("Status"))
                dic["Status"] = (int)(model.Status);
            else dic.Add("Status", (int)(model.Status));
            return dic;
        }

        public override AppBasicInfo Format(DataRow row, DataColumnCollection columns = null)
        {
            var item = base.Format(row, columns);
            if (item != null && row["Status"] != null)
                item.Status = Utils.GetEnum<AppStatus>(row["Status"]);
            if (item != null && row["AppType"] != null)
                item.AppType = Utils.GetEnum<AppType>(row["AppType"]);
            return item;
        }

        public AppBasicInfo GetByAppId(string appId)
        {
            return Get(string.Format("AppId='{0}'", appId));
        }

        public AppBasicInfo GetOrRegister(Guid? storeId, AppType appType, string appId,
            string computername, string ipAddress, string mac, string direc, string version)
        {
            var data = GetByAppId(appId);
            if (data != null)
            {
                return data;
            }
            data = new AppBasicInfo()
            {
                AppId = appId,
                AppType = appType,
                CreateTime = DateTime.Now,
                ComputerName = computername,
                IpAddress = ipAddress,
                MacAddress = mac,
                ProcessDirectory = direc,
                OnlineTime = DateTime.Now,
                Status = AppStatus.Starting,
                LastUpdateTime = DateTime.Now,
                LastHeartTime = DateTime.Now,
                AppName = Utils.GetEnumDesc<AppType>(appType) + computername,
                RunVersion = version,
                GitRepository = string.Empty,
                StoreId = storeId,
            };
            Create(data);
            _logger.Trace(string.Format("注册新客户端 Id;{0}", data.AppId));
            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientid"></param>
        /// <returns></returns>
        public AppBasicInfo GetOrRegisterListener(string clientid)
        {
            var computername = ProcessLockHelper.GetRunningComputerName();
            var data = GetOrRegister(null,
                     AppType.MqListener,
                     clientid,
                     computername,
                     ProcessLockHelper.GetIpAddresses(),
                     ProcessLockHelper.GetRunningMacAddress(),
                     ProcessLockHelper.GetProcessDirectoryName(),
                     AutoUpdaterInfo.Current.Version);
            return data;
        }

        public void UpdateVersion(int id, string version)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("RunVersion", version);
            dic.Add(PrimaryKey, id);
            var sqlClause = BuildUpdateSql(dic);
            ExcuteSql(sqlClause);
        }

        public int GetId(string appId)
        {
            return SearchService.GetSingle(
                   string.Format("SELECT Id FROM App_BasicInfo where AppId='{0}'", appId));
        }

        public void UpdateStatus(int id, AppStatus status)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("Status", (int)status);
            dic.Add(PrimaryKey, id);
            var sqlClause = BuildUpdateSql(dic);
            ExcuteSql(sqlClause);

            _logger.Information("修改状态,ID:" + id + " status:" + status.ToString());
        }
        public void UpdateLastHeartTime(int id)
        {
            StringBuilder builder = new StringBuilder("update [App_BasicInfo] set ");
            builder.Append("LastHeartTime=getdate()");
            builder.AppendFormat(" where {1} ={0}", id, PrimaryKey);
            ExcuteSql(builder.ToString());
        }

        public void Deploy(int id)
        { 
            StringBuilder builder = new StringBuilder("update [App_BasicInfo] set ");
            builder.AppendFormat("LastUpdateTime=getdate(),Status={0}", (int)AppStatus.Restart);
            builder.AppendFormat(" where {1} ={0}", id, PrimaryKey);
            ExcuteSql(builder.ToString());
            _logger.Information("重新部署,ID:" + id);
        }
        public void Online(int id)
        {
            StringBuilder builder = new StringBuilder("update [App_BasicInfo] set ");
            builder.AppendFormat("OnlineTime=getdate(),LastHeartTime=getdate(),Status={0}", (int)AppStatus.Running);
            builder.AppendFormat(" where {1} ={0}", id, PrimaryKey);
            ExcuteSql(builder.ToString()); 
            _logger.Information("App上线,ID:" + id);
        }

        public AppStatus Transform(AppStatus status)
        {
            switch (status)
            {
                case AppStatus.Deploy:
                case AppStatus.ReDeploy:
                    return status;
                case AppStatus.Restart:
                case AppStatus.Stoped:
                    return AppStatus.Starting;
                case AppStatus.Starting:
                case AppStatus.Running:
                    return AppStatus.Running;
                default:
                    return status;
            }
        }

        public void VerifyListenStatus(AppType appType)
        {
            _logger.Trace("30分钟内都没有收到心跳处理的数据，直接踢下线 appType " + appType.ToString());
            //30分钟内都没有收到心跳处理的数据，直接踢下线  
            StringBuilder builder = new StringBuilder("update [App_BasicInfo] set ");
            builder.AppendFormat("Status={0}", (int)AppStatus.Stoped);
            builder.Append(" where LastHeartTime< DATEADD(MI,-3,getdate()) ");
            builder.AppendFormat(" AND Status={0}", (int)AppStatus.Running);
            builder.AppendFormat(" AND AppType={0}", (int)appType);
            ExcuteSql(builder.ToString());
        }
    }
}