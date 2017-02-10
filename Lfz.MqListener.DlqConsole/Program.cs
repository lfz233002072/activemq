using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using Lfz.MqListener.CacheShared;
using Lfz.MqListener.Mq.Dlq;
using Lfz.Redis;
using Lfz.Utitlies;

namespace Lfz.MqListener.DlqConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("正在启动中");
                //var list = Directory.EnumerateFiles(@"C:\Users\lfz\Desktop\Temp", "*", SearchOption.AllDirectories);
                //foreach (var f in list)
                //    Console.WriteLine(f);
                //Console.WriteLine("总文件数量:"+list.Count());
                RedisBase.Initialize(new RedisConfigService());
                ActiveMqdlqFactoryThreadService service=new ActiveMqdlqFactoryThreadService();
                service.Start();
                Console.WriteLine("按任何键结束进程");
                Console.Read();
                service.Stop();
                //Down(); 
                Console.Read();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                Console.WriteLine("按任何键结束进程");
                Console.Read();
            }
        }

        public static void Down()
        {
            var directory = Utils.MapPath("temp");
            if (!System.IO.Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            var file = directory + "/a.zip";
            WebClient webClient = new WebClient();
            webClient.DownloadFileCompleted += WebClientOnDownloadFileCompleted;
            webClient.DownloadProgressChanged += WebClientOnDownloadProgressChanged;
            webClient.DownloadFileAsync(new Uri("http://ziyouchi.com:3442/Uploads/文三西路菜单图.rar"), file);

        }

        private static void WebClientOnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs downloadProgressChangedEventArgs)
        {
            Console.WriteLine("接收大小：{0} 文件总大小{1}", downloadProgressChangedEventArgs.BytesReceived, downloadProgressChangedEventArgs.TotalBytesToReceive);
        }

        private static void WebClientOnDownloadFileCompleted(object sender, AsyncCompletedEventArgs asyncCompletedEventArgs)
        {
            if (asyncCompletedEventArgs.Error != null)
                Console.WriteLine(asyncCompletedEventArgs.Error);
            else if (asyncCompletedEventArgs.Cancelled)
            {
                Console.WriteLine("取消下载");
            }
            else
            {

                Console.WriteLine("下载成功");
            }

        }
    }
}
