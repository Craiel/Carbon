using Carbon.Engine.Ninject;
using Carbon.V2Test.Contracts;
using Carbon.V2Test.Logic;

using Ninject.Modules;
using Carbon.V2Test.Scenes;

namespace Carbon.V2Test.Ninject
{
    public static class NinjectModuleManager
    {
        public static NinjectModule[] GetModules()
        {
            return new NinjectModule[] { new EngineModule(), new GameModule() };
        }

        public class GameModule : NinjectModule
        {
            public override void Load()
            {
                this.Bind<IV2Test>().To<Logic.V2Test>().InSingletonScope();

                this.Bind<IApplicationLog>().To<ApplicationLog>();
                this.Bind<ITestScene>().To<TestScene2>();
                this.Bind<ITestSceneSponza>().To<TestSceneSponza>();
            }
        }
    }
}