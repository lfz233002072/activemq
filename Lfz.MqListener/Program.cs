using System;
using System.Threading;
using Lfz.AutoUpdater;
using Lfz.AutoUpdater.Config;
using Lfz.MqListener.CacheShared;
using Lfz.Redis;
using Lfz.Services;
using Lfz.Utitlies;

namespace Lfz.MqListener
{
    class Program
    {
        static void Main(string[] args)
        { 
            var lockerid = ProcessLockHelper.GetProcessLockId();
            try
            {
                Console.Title = "消息调度程序 " + lockerid.Substring(0, 6);
                bool isCreated;
                string mutexName = lockerid;
                var appMutex = new Mutex(true, mutexName, out isCreated);
                //如果创建失败，则表示已经运行了。
                if (!isCreated)
                {
                    Console.WriteLine("已经运行,按任意键结束！");
                    Console.Read();
                }
                else
                {
                    Console.WriteLine("正在启动中");
                    CommonUnitity.Init(false);
                    RedisBase.Initialize(new RedisConfigService());
                    TCSoftServiceBus.Current.Start();

                    Console.WriteLine("服务总线启动成功");
                    while ((Console.ReadKey().Key != ConsoleKey.Q))
                    {
                        continue;
                    }
                    TCSoftServiceBus.Current.Stop();
                    Console.WriteLine("结束允许");
                    appMutex.ReleaseMutex();
                    appMutex.Dispose();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.Read();
            }
        }

        public static void TestAutoUpdate()
        {
            try
            {
                var service = TCSoftServiceBus.Current;
                service.Stop();
                int count = 0;
                //优先保障服务正常结束
                while (count < 10 && service.Status != ServiceStatus.Stoped)
                {
                    if (service.Status == ServiceStatus.Running)
                        service.Stop();
                    Thread.Sleep(1000);
                    count++;
                }
                AutoUpdaterInfo info = AutoUpdaterInfo.Current;
                AppUnitity.RunAutoUpdaterProcess(info.ExcuteFileName); 
                AppUnitity.KillOrStop(info.ExcuteFileName,info.IsWindowService,true); 
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
