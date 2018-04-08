using System.Windows;

namespace Rrs.ServiceApp
{
    public abstract class WpfApp : Application
    {
        public abstract void Application_Startup(object sender, StartupEventArgs args);
    }
}
