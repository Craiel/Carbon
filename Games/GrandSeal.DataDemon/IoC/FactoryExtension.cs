namespace GrandSeal.DataDemon.IoC
{
    using Autofac;

    using Core.Engine.Contracts;

    using GrandSeal.DataDemon.Contracts;
    using GrandSeal.DataDemon.Logic;

    public static class FactoryExtension
    {
        public static IDemonBuild GetDemonBuild(this IEngineFactory factory, DemonBuildConfig config)
        {
            return factory.Kernel.Resolve<IDemonBuild>(new NamedParameter("config", config));
        }
    }
}
