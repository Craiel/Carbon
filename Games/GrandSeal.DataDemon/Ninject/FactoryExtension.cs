using Ninject;
using Ninject.Parameters;

namespace GrandSeal.DataDemon.Ninject
{
    using Core.Engine.Contracts;

    using GrandSeal.DataDemon.Contracts;
    using GrandSeal.DataDemon.Logic;

    public static class FactoryExtension
    {
        public static IDemonConversion GetDemonConversion(this IEngineFactory factory, DemonConversionConfig config)
        {
            return factory.Kernel.Get<IDemonConversion>(new ConstructorArgument("config", config));
        }

        public static IDemonBuild GetDemonBuild(this IEngineFactory factory, DemonBuildConfig config)
        {
            return factory.Kernel.Get<IDemonBuild>(new ConstructorArgument("config", config));
        }
    }
}
