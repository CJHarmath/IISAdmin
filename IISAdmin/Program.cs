using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Web.Administration;

namespace IISAdmin
{
    class Program
    {
        
        static void Main(string[] args)
        {

            while (true)
            {
                hammerWas();
            }
        
        }

        private static void hammerWas()
        {
            for (int i = 0; i < 1000; i++)
            {
                Console.WriteLine(i);
                createPool(i);
                createSite(i);
                createApp(i);
            }
        }

        private static void createApp(int i)
        {
            using (ServerManager serverManager = new ServerManager())
            {
                string name = String.Format("testapp-{0}", i);
                string path = @"c:\inetpub\wwwroot\";
                string poolName = String.Format("testpool-{0}", i);
                string siteName = String.Format("testsite-{0}", i);
                deleteApp(siteName, name);
                var site = serverManager.Sites[siteName];
                Application app = site.Applications.Add("/", path);
                app.ApplicationPoolName = poolName;
                serverManager.CommitChanges();
            }
        }

        private static Site createSite(int i)
        {
            using (ServerManager serverManager = new ServerManager())
            {
                string name = String.Format("testsite-{0}", i);
                deleteSite(name);
                string poolName = String.Format("testpool-{0}", i);
                int port = 80 + i;
                Site site = serverManager.Sites.Add(name, @"c:\inetpub\wwwroot", port);
                site.Id = port;
                site.ServerAutoStart = true;
                site.ApplicationDefaults.ApplicationPoolName = poolName;
                serverManager.CommitChanges();
                return site;
            }
            
        }

        static void createPool(int i)
        {
            using (ServerManager serverManager = new ServerManager())
            {
                string name = String.Format("testpool-{0}", i);
                deletePool(name);
                ApplicationPool pool = serverManager.ApplicationPools.Add(name);
                pool.AutoStart = true;
                pool.StartMode = StartMode.AlwaysRunning;
                serverManager.CommitChanges();
                //pool.Recycling.LogEventOnRecycle = RecyclingLogEventOnRecycle.ConfigChange | RecyclingLogEventOnRecycle.IsapiUnhealthy;
                pool = serverManager.ApplicationPools[name];
                pool.ManagedPipelineMode = ManagedPipelineMode.Integrated;
                serverManager.CommitChanges();
                pool = serverManager.ApplicationPools[name];
                pool.ManagedRuntimeVersion = "v4.0";
                serverManager.CommitChanges();
                pool = serverManager.ApplicationPools[name];
                pool.Enable32BitAppOnWin64 = false;
                serverManager.CommitChanges();
                pool = serverManager.ApplicationPools[name];
                pool.Cpu.Action = ProcessorAction.NoAction;
                serverManager.CommitChanges();
            }
        }

        static void deletePool(string poolName)
        {
            using (ServerManager serverManager = new ServerManager())
            {
                var pool = serverManager.ApplicationPools.Where(p => p.Name == poolName).SingleOrDefault();
                if (pool != null)
                {
                    serverManager.ApplicationPools.Remove(pool);
                    serverManager.CommitChanges();
                }
            }
        }

        static void deleteSite(string siteName)
        {
            using (ServerManager serverManager = new ServerManager())
            {
                var site = serverManager.Sites.Where(s => s.Name == siteName).SingleOrDefault();
                if (site != null)
                {
                    serverManager.Sites.Remove(site);
                    serverManager.CommitChanges();
                }
            }
        }

        static void deleteApp(string siteName, string appName)
        {
            using (ServerManager serverManager = new ServerManager())
            {
                var site = serverManager.Sites.Where(s => s.Name == siteName).SingleOrDefault();
                if (site != null)
                {
                    var app = site.Applications.Where(a => a.Path == "/").SingleOrDefault();
                    if (app != null)
                    {
                        site.Applications.Remove(app);
                        serverManager.CommitChanges();
                    }
                }
            }
        }
    }
}
