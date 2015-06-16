namespace GrandSeal.Editor
{
    using System;

    using CarbonCore.Utils.Compat.Diagnostics;
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
            var container = CarbonContainerAutofacBuilder.Build<EditorModule>();
            container.Resolve<IEditor>().Run();

            Profiler.TraceProfilerStatistics();
        }
    }
}
