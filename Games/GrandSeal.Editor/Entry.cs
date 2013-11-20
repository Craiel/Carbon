using Ninject;

namespace GrandSeal.Editor
{
    using System;

    using Core.Utils.Diagnostics;

    using GrandSeal.Editor.Contracts;
    using GrandSeal.Editor.Ninject;

    public static class Entry
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            IKernel kernel = new StandardKernel(NinjectModuleManager.GetModules());
            kernel.Get<IEditor>().Run();

            Profiler.TraceProfilerStatistics();
        }
    }
}
