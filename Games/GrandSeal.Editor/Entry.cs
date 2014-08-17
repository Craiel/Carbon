namespace GrandSeal.Editor
{
    using System;

    using Autofac;

    using CarbonCore.Utils.Diagnostics;
    using CarbonCore.Utils.IoC;

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
            IContainer container = CarbonContainerBuilder.Build<EditorModule>();
            container.Resolve<IEditor>().Run();

            Profiler.TraceProfilerStatistics();
        }
    }
}
