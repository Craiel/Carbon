using System;

using Carbed.Contracts;
using Carbed.Ninject;

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
            log4net.Config.XmlConfigurator.Configure();

            IKernel kernel = new StandardKernel(NinjectModuleManager.GetModules());
            kernel.Get<ICarbed>().Run();
        }
    }
}
