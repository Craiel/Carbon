namespace GrandSeal
{
    using Autofac;

    using Contracts;

    using global::GrandSeal.IoC;

    public static class Entry
    {
        public static void Main()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<GameModule>();
            IContainer kernel = builder.Build();
            kernel.Resolve<IGrandSeal>().Run();
        }
    }
}
