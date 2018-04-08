using System;
using System.Collections;
using System.Configuration.Install;
using System.Reflection;
using System.ServiceProcess;

namespace Rrs.ServiceApp
{
    public static class ServiceInstaller
    {
        public static void Install(string serviceName, bool undo, string[] args)
        {
            try
            {
                Console.WriteLine(undo ? "uninstalling" : "installing");
                using (var inst = new AssemblyInstaller(Assembly.GetEntryAssembly(), args))
                {
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
