using CliFx;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Linq;

namespace CLITool
{
    class Program
    {
        private static IServiceProvider GetServiceProvider()
        {
            var services = new ServiceCollection();
            services.AddTransient<HelloCommand>();

            return services.BuildServiceProvider();
        }

        public static async Task<int> Main()
        {

            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            baseDirectory = baseDirectory.Substring(0, baseDirectory.IndexOf("CLITool"));
            var coredll = Assembly.LoadFile(Directory.GetFiles(baseDirectory, "CoreLib.dll", searchOption: SearchOption.AllDirectories).First());
            var coreClass = coredll.GetType("CoreLib.HelloCore");
            var core = coreClass.GetConstructor(Type.EmptyTypes).Invoke(new object[] { });
            var coreMethod = coreClass.GetMethod("SayHello");
            var coreValue = coreMethod.Invoke(core, new object[] { }).ToString();

            return await new CliApplicationBuilder()
                .AddCommandsFromThisAssembly()
                .UseTypeActivator(GetServiceProvider().GetRequiredService)
#if DEBUG
                .AllowDebugMode(true)
                .AllowPreviewMode(true)
#else
                .AllowDebugMode(false)
                .AllowPreviewMode(false)
#endif
                .Build()
                .RunAsync();
        }
    }
}
