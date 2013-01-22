using Carbon.V2Test.Contracts;
using Carbon.V2Test.Ninject;

using Ninject;

namespace Carbon.V2Test
{
    public static class Entry
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static void Main()
        {
            IKernel kernel = new StandardKernel(NinjectModuleManager.GetModules());
            kernel.Get<IV2Test>().Run();
        }
    }
}
