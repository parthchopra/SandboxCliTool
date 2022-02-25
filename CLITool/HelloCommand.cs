using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CLITool
{
    [Command("sayhello")]
    public class HelloCommand : ICommand
    {
        public ValueTask ExecuteAsync(IConsole console)
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            baseDirectory = baseDirectory.Substring(0, baseDirectory.IndexOf("CLITool"));
            var frameworkdll = Assembly.LoadFile(Directory.GetFiles(baseDirectory, "FullFrameworkLib.dll", searchOption: SearchOption.AllDirectories).First());            
            var frameworkClass = frameworkdll.GetType("FullFrameworkLib.HelloFramework");
            var framework = frameworkClass.GetConstructor(Type.EmptyTypes).Invoke(new object[] { });
            var helloFrameworkMethod = frameworkClass.GetMethod("SayHello");
            var frameworkValue = helloFrameworkMethod.Invoke(framework, new object[] { }).ToString();

            console.Output.WriteLine("reflected output from FullFramework project");
            console.Output.WriteLine(frameworkValue);
            console.Output.WriteLine();

            var coredll = Assembly.LoadFile(Directory.GetFiles(baseDirectory, "CoreLib.dll", searchOption: SearchOption.AllDirectories).First());
            var coreClass = coredll.GetType("CoreLib.HelloCore");
            var core = coreClass.GetConstructor(Type.EmptyTypes).Invoke(new object[] { });
            var coreMethod = coreClass.GetMethod("SayHello");
            var coreValue = coreMethod.Invoke(core, new object[] { }).ToString();

            console.Output.WriteLine("reflected output from net core project");
            console.Output.WriteLine(coreValue);
            console.Output.WriteLine();
            console.Output.WriteLine("command works");

            return default;
        }
    }
}
