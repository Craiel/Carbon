namespace GrandSeal.DataDemon.IoC
{
    using Autofac;
    using Autofac.Core;

    using CarbonCore.Utils.IoC;

    using Core.Engine.IoC;
    using Core.Processing.IoC;

    using GrandSeal.DataDemon.Contracts;
    using GrandSeal.DataDemon.Logic;

    public class DataDemonModule : Module
    {
        public static IModule[] GetModules()
        {
            return new IModule[]
                    {
                        new EngineModule(), new DataDemonModule(),
                        new CarbonEditorModule(),
                        new UtilsModule()
                    };
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<IDataDemon>().As<DataDemon>().SingleInstance();

            builder.RegisterType<IDemonLogic>().As<DemonLogic>().SingleInstance();
            builder.RegisterType<IDemonLog>().As<DemonLog>().SingleInstance();

            builder.RegisterType<IDemonFileInfo>().As<DemonFileInfo>().SingleInstance();
            builder.RegisterType<IDemonBuild>().As<DemonBuild>();
        }
    }
}