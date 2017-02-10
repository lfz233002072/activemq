using System.ServiceProcess;
using Lfz.AutoUpdater;
using Lfz.MqListener.CacheShared;
using Lfz.Redis;

namespace Lfz.MqListener.WindowService
{
    public partial class TCMqListener : ServiceBase
    {
        public TCMqListener()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        { 
            CommonUnitity.Init(true,this.ServiceName);
            RedisBase.Initialize(new RedisConfigService()); 
            TCSoftServiceBus.Current.Start();
        }

        protected override void OnStop()
        {
            TCSoftServiceBus.Current.Stop();
        }
    }
}
