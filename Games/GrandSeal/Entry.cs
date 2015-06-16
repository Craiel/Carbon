namespace GrandSeal
{
    using CarbonCore.Utils.IoC;

    using Contracts;

    using global::GrandSeal.IoC;

    public static class Entry
    {
        public static void Main()
        {
            var container = CarbonContainerAutofacBuilder.Build<GameModule>();
            container.Resolve<IGrandSeal>().Run();
        }
    }
}
