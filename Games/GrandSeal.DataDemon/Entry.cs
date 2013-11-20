using Ninject;

namespace GrandSeal.DataDemon
{
    using Core.Utils.Diagnostics;

    using GrandSeal.DataDemon.Contracts;
    using GrandSeal.DataDemon.Logic;
    using GrandSeal.DataDemon.Ninject;
    
    public class Entry
    {
        public static void Main(string[] args)
        {
            // Todo: Process Arguments
            var arguments = new DemonArguments { Config = @"C:\Dev\Carbon\Games\GrandSeal.Data\Demon.conf" };

            IKernel kernel = new StandardKernel(NinjectModuleManager.GetModules());
            kernel.Get<IDataDemon>().Run(arguments);

            Profiler.TraceProfilerStatistics();
        }
    }
}
