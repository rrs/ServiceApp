using System;
using System.Linq;
using System.Security.Principal;
using System.ServiceProcess;
using System.Windows;

namespace Rrs.ServiceApp
{
    public static class ProgramHelper
    {
        public static int Run(ServiceBase service, Func<IWpfApp> appFunc, string[] args, string serviceDescription = null)
        {
            try
            {
                if (args.Length > 0)
                {
                    if (args[0] == "--service")
                    {
                        ServiceBase.Run(new ServiceBase[]
                        {
                            service
                        });
                    }
                    else
                    {
                        ConsoleApp.Run(service.ServiceName, args);
                    }
                }
                else
                {
                    if (!new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
                    {
                        MessageBox.Show("Run again as an Administrator!");
                        return 0;
                    }

                    var sc = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == service.ServiceName);
                    var wasRunnnig = false;
                    if (sc?.Status == ServiceControllerStatus.Running)
                    {
                        sc.Stop();
                        wasRunnnig = true;
                    }

                    var app = appFunc();
                    app.InitializeComponent();
                    app.Run();
                    if (wasRunnnig)
                    {
                        sc.Start();
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return -1;
            }
        }

        public static int Run<T>(ServiceBase service, string[] args) where T : IWpfApp, new()
        {
            return Run(service, () => new T(), args);
        }
    }
}
