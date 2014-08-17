namespace GrandSeal
{
    using Autofac;

    using CarbonCore.Utils.IoC;

    using Contracts;

    using global::GrandSeal.IoC;

    public static class Entry
    {
        public static void Main()
        {
            IContainer kernel = CarbonContainerBuilder.Build<GameModule>();
            kernel.Resolve<IGrandSeal>().Run();
        }
    }
}
