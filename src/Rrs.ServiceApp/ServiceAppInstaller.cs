using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text.RegularExpressions;

namespace Rrs.ServiceApp
{
    [RunInstaller(true)]
    public class ServiceAppInstaller : Installer
    {
        public static void Install(string serviceName, bool undo, string[] args, string description = null)
        {
            try
            {
                Console.WriteLine(undo ? "uninstalling" : "installing");
                using (var inst = new AssemblyInstaller(Assembly.GetEntryAssembly(), args))
                {
                    var processInstaller = new ServiceProcessInstaller
                    {
                        Account = ServiceAccount.LocalSystem
                    };

                    var serviceInstaller = new System.ServiceProcess.ServiceInstaller
                    {
                        StartType = ServiceStartMode.Automatic,
                        ServiceName = serviceName,
                        DisplayName = string.Join(" ", Regex.Matches(serviceName, @"([A-Z][a-z]+)").Cast<Match>().Select(m => m.Value)),
                        Description = description
                    };

                    inst.Installers.Add(processInstaller);
                    inst.Installers.Add(serviceInstaller);

                    IDictionary state = new Hashtable();
                    inst.UseNewContext = true;
                    try
                    {
                        if (undo)
                        {
                            var sc = new ServiceController(serviceName);
                            if (sc.Status == ServiceControllerStatus.Running) sc.Stop();
                            inst.Uninstall(state);
                        }
                        else
                        {
                            inst.BeforeInstall += (s, e) => { inst.Context.Parameters["assemblyPath"] = $@"""{inst.Context.Parameters["assemblyPath"]}"" --service"; };
                            inst.AfterInstall += (s, e) => { new ServiceController(serviceName).Start(); };
                            inst.Install(state);
                            inst.Commit(state);
                        }
                    }
                    catch
                    {
                        try
                        {
                            inst.Rollback(state);
                        }
                        catch { }
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }
    }
}
