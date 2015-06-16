namespace GrandSeal
{
    using Autofac;

    using CarbonCore.Utils.Compat.IoC;
    using CarbonCore.Utils.IoC;

    using Contracts;

    using global::GrandSeal.IoC;

    public static class Entry
    {
        public static void Main()
        {
            IContainer kernel = new CarbonContainerAutofacBuilder().Build<GameModule>() as IContainer;
            kernel.Resolve<IGrandSeal>().Run();
        }
    }
}
