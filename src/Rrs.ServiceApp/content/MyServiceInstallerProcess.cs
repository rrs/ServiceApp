using System.ComponentModel;
using System.ServiceProcess;

namespace $rootnamespace$
{
    [RunInstaller(true)]
    public sealed class MyServiceInstallerProcess : ServiceProcessInstaller
    {
        public MyServiceInstallerProcess()
        {
            Account = ServiceAccount.LocalSystem;
        }
    }
}
