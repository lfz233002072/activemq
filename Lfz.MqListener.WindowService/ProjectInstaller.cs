using System.ComponentModel;
using System.Configuration.Install;

namespace Lfz.MqListener.WindowService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        private void TCMqListener_AfterInstall(object sender, InstallEventArgs e)
        {

        }
    }
}
