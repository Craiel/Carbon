namespace GrandSeal.IoC
{
    using Autofac;

    using Contracts;

    using Core.Engine.Contracts;

    public static class FactoryExtension
    {
        public static IGrandSealScriptingProvider GetScriptingProvider(this IEngineFactory factory, IGrandSeal gameInstance)
        {
            return factory.Kernel.Resolve<IGrandSealScriptingProvider>(new NamedParameter("game", gameInstance));
        }
    }
}
