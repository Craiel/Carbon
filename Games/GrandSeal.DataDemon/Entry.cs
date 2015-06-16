namespace GrandSeal.DataDemon
{
    using Autofac;

    using CarbonCore.Utils.Compat.Diagnostics;
    using CarbonCore.Utils.Compat.IoC;
    using CarbonCore.Utils.IoC;

    using GrandSeal.DataDemon.Contracts;
    using GrandSeal.DataDemon.IoC;
    using GrandSeal.DataDemon.Logic;
    
    public class Entry
    {
        public static void Main(string[] args)
        {
            // Todo: Process Arguments
            var arguments = new DemonArguments { Config = @"C:\Dev\Carbon\Games\GrandSeal.Data\Demon.conf" };

            IContainer kernel = new CarbonContainerAutofacBuilder().Build<DataDemonModule>() as IContainer;
            kernel.Resolve<IDataDemon>().Run(arguments);

            Profiler.TraceProfilerStatistics();
        }
    }
}
