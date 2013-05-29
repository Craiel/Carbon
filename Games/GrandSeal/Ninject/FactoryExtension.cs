using Core.Engine.Contracts;
using GrandSeal.Contracts;
using global::Ninject;
using global::Ninject.Parameters;

namespace GrandSeal.Ninject
{
    public static class FactoryExtension
    {
        public static IGrandSealScriptingProvider GetScriptingProvider(this IEngineFactory factory, IGrandSeal gameInstance)
        {
            return factory.Kernel.Get<IGrandSealScriptingProvider>(new ConstructorArgument("game", gameInstance));
        }
    }
}
