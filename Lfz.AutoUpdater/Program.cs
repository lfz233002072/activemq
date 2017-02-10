using System;
using System.Windows.Forms;
using Lfz.AutoUpdater.Config;
using Lfz.AutoUpdater.Logging;
using Lfz.AutoUpdater.Service;
using Lfz.AutoUpdater.Utitlies;

namespace Lfz.AutoUpdater
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(params string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            bool flag = false;
            if (args != null && args.Length > 0)
            {
                flag = TypeParse.StrToBool(args[0]);
            }
            var logger = LoggerFactory.GetLog(RunConfig.Current.LoggerType);
            if (flag)
            {
                logger.Information("启动自动更新程序（后台程序，不可见）");
                AutoUpdateBackgroupService.RunAutoUpdate();
            }
            else
            {
                logger.Information("启动更新程序(需要手动操作)");
                Application.Run(new FrmUpdateCheck());
            }
        }

    }
}
