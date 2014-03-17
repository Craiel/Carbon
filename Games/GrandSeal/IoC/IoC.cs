namespace GrandSeal.IoC
{
    using Autofac;
    using Autofac.Core;

    using CarbonCore.Utils.IoC;

    using Contracts;

    using Core.Engine.IoC;

    using Logic;
    using Scenes;

    public class GameModule : Module
    {
        public static IModule[] GetModules()
        {
            return new IModule[]
                       {
                           new EngineModule(), 
                           new GameModule(), 
                           new UtilsModule()
                       };
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<IGrandSeal>().As<GrandSeal>().SingleInstance();

            builder.RegisterType<IGrandSealLog>().As<GrandSealLog>();
            builder.RegisterType<IGrandSealScriptingProvider>().As<GrandSealScriptingProvider>().SingleInstance();
            builder.RegisterType<IGrandSealGameState>().As<GrandSealGameState>().SingleInstance();
            builder.RegisterType<IGrandSealSystemController>().As<GrandSealSystemController>().SingleInstance();
            builder.RegisterType<IGrandSealSettings>().As<GrandSealSettings>().SingleInstance();

            builder.RegisterType<ISceneEntry>().As<SceneEntry>().SingleInstance();
            builder.RegisterType<ISceneMainMenu>().As<SceneMainMenu>().SingleInstance();
        }
    }
}
