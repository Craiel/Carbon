namespace GrandSeal.IoC
{
    using CarbonCore.Utils.IoC;

    using Contracts;

    using Core.Engine.IoC;

    using Logic;
    using Scenes;

    [DependsOnModule(typeof(UtilsModule))]
    [DependsOnModule(typeof(EngineModule))]
    [DependsOnModule(typeof(GameModule))]
    public class GameModule : CarbonModule
    {
        public GameModule()
        {
            this.For<IGrandSeal>().Use<GrandSeal>().Singleton();

            this.For<IGrandSealScriptingProvider>().Use<GrandSealScriptingProvider>().Singleton();
            this.For<IGrandSealGameState>().Use<GrandSealGameState>().Singleton();
            this.For<IGrandSealSystemController>().Use<GrandSealSystemController>().Singleton();
            this.For<IGrandSealSettings>().Use<GrandSealSettings>().Singleton();

            this.For<ISceneEntry>().Use<SceneEntry>().Singleton();
            this.For<ISceneMainMenu>().Use<SceneMainMenu>().Singleton();
        }
    }
}
