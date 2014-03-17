namespace GrandSeal.Editor
{
    using System;

    using Autofac;

    using CarbonCore.Utils.Diagnostics;
    
    using GrandSeal.Editor.Contracts;
    using GrandSeal.Editor.IoC;

    public static class Entry
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<EditorModule>();
            IContainer kernel = builder.Build();
            kernel.Resolve<IEditor>().Run();

            Profiler.TraceProfilerStatistics();
        }
    }
}
