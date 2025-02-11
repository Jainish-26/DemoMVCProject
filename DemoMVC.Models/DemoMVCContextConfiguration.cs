using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoMVC.Models
{
    public class DemoMVCContextConfiguration : DbConfiguration
    {
        private static bool useCachedDbModelStore;

        public static void Configure(bool useCachedDbModelStore)
        {
            DemoMVCContextConfiguration.useCachedDbModelStore = useCachedDbModelStore;
        }

        public DemoMVCContextConfiguration()
        {
            //string cachePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\TTS\EFCache\";
            string cachePath = AppDomain.CurrentDomain.BaseDirectory + @"EFCache\";
            if (!Directory.Exists(cachePath))
            {
                Directory.CreateDirectory(cachePath);
            }
            MyDbModelStore cachedDbModelStore = new MyDbModelStore(cachePath);
            IDbDependencyResolver dependencyResolver = new SingletonDependencyResolver<DbModelStore>(cachedDbModelStore);
            AddDependencyResolver(dependencyResolver);
        }

        private class MyDbModelStore : DefaultDbModelStore
        {
            public MyDbModelStore(string location)
                : base(location)
            { }

            public override DbCompiledModel TryLoad(Type contextType)
            {
                string path = GetFilePath(contextType);

                if (File.Exists(path))
                {
                    DateTime lastWriteTime = File.GetLastWriteTimeUtc(path);
                    DateTime lastWriteTimeDomainAssembly = File.GetLastWriteTimeUtc(typeof(DemoMVC.Models.DemoMVCContextConfiguration).Assembly.Location);
                    if (lastWriteTimeDomainAssembly > lastWriteTime)
                    {
                        File.Delete(path);
                    }
                }
                else
                {

                }

                return base.TryLoad(contextType);
            }
        }
    }
}
