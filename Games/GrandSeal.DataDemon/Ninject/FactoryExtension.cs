using Ninject;
using Ninject.Parameters;

namespace GrandSeal.DataDemon.Ninject
{
    using Core.Engine.Contracts;

    using GrandSeal.DataDemon.Contracts;
    using GrandSeal.DataDemon.Logic;

    public static class FactoryExtension
    {
        public static IDemonBuild GetDemonBuild(this IEngineFactory factory, DemonBuildConfig config)
        {
            return factory.Kernel.Get<IDemonBuild>(new ConstructorArgument("config", config));
        }
    }
}
