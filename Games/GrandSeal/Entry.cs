using GrandSeal.Contracts;
using GrandSeal.Ninject;

using Ninject;

namespace GrandSeal
{
    public static class Entry
    {
        public static void Main()
        {
            IKernel kernel = new StandardKernel(NinjectModuleManager.GetModules());
            kernel.Get<IGrandSeal>().Run();
        }
    }
}
