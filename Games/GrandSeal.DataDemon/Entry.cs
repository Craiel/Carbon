namespace GrandSeal.DataDemon
{
    using Autofac;

    using CarbonCore.Utils.Diagnostics;

    using GrandSeal.DataDemon.Contracts;
    using GrandSeal.DataDemon.IoC;
    using GrandSeal.DataDemon.Logic;
    
    public class Entry
    {
        public static void Main(string[] args)
        {
            // Todo: Process Arguments
            var arguments = new DemonArguments { Config = @"C:\Dev\Carbon\Games\GrandSeal.Data\Demon.conf" };

            var builder = new ContainerBuilder();
            builder.RegisterModule<DataDemonModule>();
            IContainer kernel = builder.Build();
            kernel.Resolve<IDataDemon>().Run(arguments);

            Profiler.TraceProfilerStatistics();
        }
    }
}
