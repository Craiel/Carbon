using Ninject.Modules;

namespace GrandSeal.DataDemon.Ninject
{
    using Core.Engine.Ninject;

    using GrandSeal.DataDemon.Contracts;
    using GrandSeal.DataDemon.Logic;

    public static class NinjectModuleManager
    {
        public static INinjectModule[] GetModules()
        {
            return new NinjectModule[]
                       {
                           new EngineModule(), new DataDemonModule(),
                           new Core.Processing.Ninject.NinjectModuleManager.CarbonEditorModule(),
                           new Core.Utils.Ninject.NinjectModuleManager.UtilsModule()
                       };
        }

        public class DataDemonModule : NinjectModule
        {
            public override void Load()
            {
                this.Bind<IDataDemon>().To<DataDemon>().InSingletonScope();

                this.Bind<IDemonLogic>().To<DemonLogic>().InSingletonScope();
                this.Bind<IDemonLog>().To<DemonLog>().InSingletonScope();

                this.Bind<IDemonFileInfo>().To<DemonFileInfo>().InSingletonScope();
                this.Bind<IDemonBuild>().To<DemonBuild>();
            }
        }
    }
}