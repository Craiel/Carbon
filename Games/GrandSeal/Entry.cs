using Ninject;

namespace GrandSeal
{
    using Contracts;

    using Ninject;

    public static class Entry
    {
        public static void Main()
        {
            IKernel kernel = new StandardKernel(NinjectModuleManager.GetModules());
            kernel.Get<IGrandSeal>().Run();
        }
    }
}
