using System;

using Carbed.Contracts;
using Carbed.Ninject;

using Core.Utils.Diagnostics;

using Ninject;

namespace Carbed
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
            kernel.Get<ICarbed>().Run();

            Profiler.TraceProfilerStatistics();
        }
    }
}
