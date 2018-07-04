using System.ComponentModel;
using System.ServiceProcess;

namespace $rootnamespace$
{
    [RunInstaller(true)]
    public sealed class MyServiceInstaller : ServiceInstaller
    {
        public MyServiceInstaller()
        {
            Description = "My Service Description";
            DisplayName = "My Service Display Name";
            ServiceName = "MyServiceName";
            StartType = ServiceStartMode.Automatic;
        }
    }
}
