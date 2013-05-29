using System;

using GrandSeal.Editor.Contracts;
using GrandSeal.Editor.Ninject;

using Core.Utils.Diagnostics;

using Ninject;

namespace GrandSeal.Editor
{
    public static class Entry
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            //log4net.Config.XmlConfigurator.Configure();

            IKernel kernel = new StandardKernel(NinjectModuleManager.GetModules());
            kernel.Get<IEditor>().Run();

            Profiler.TraceProfilerStatistics();
        }
    }
}
