using Ninject;
using Ninject.Parameters;

namespace GrandSeal.Ninject
{
    using Contracts;

    using Core.Engine.Contracts;

    public static class FactoryExtension
    {
        public static IGrandSealScriptingProvider GetScriptingProvider(this IEngineFactory factory, IGrandSeal gameInstance)
        {
            return factory.Kernel.Get<IGrandSealScriptingProvider>(new ConstructorArgument("game", gameInstance));
        }
    }
}
