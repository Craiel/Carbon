using Core.Engine.Ninject;
using Ninject.Modules;

using GrandSeal.Contracts;
using GrandSeal.Logic;

namespace GrandSeal.Ninject
{
    using Scenes;

    public static class NinjectModuleManager
    {
        public static NinjectModule[] GetModules()
        {
            return new NinjectModule[]
                       {
                           new EngineModule(), 
                           new GameModule(), 
                           new Core.Utils.Ninject.NinjectModuleManager.UtilsModule()
                       };
        }

        public class GameModule : NinjectModule
        {
            public override void Load()
            {
                this.Bind<IGrandSeal>().To<GrandSeal>().InSingletonScope();

                this.Bind<IApplicationLog>().To<ApplicationLog>();
                this.Bind<IGrandSealScriptingProvider>().To<GrandSealScriptingProvider>().InSingletonScope();
                this.Bind<IGrandSealGameState>().To<GrandSealGameState>().InSingletonScope();

                this.Bind<ISceneEntry>().To<SceneEntry>().InSingletonScope();
                this.Bind<ISceneMainMenu>().To<SceneMainMenu>().InSingletonScope();
            }
        }
    }
}
