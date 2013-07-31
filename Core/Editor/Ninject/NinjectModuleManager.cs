using Ninject.Modules;

namespace Core.Processing.Ninject
{
    using Core.Processing.Contracts;
    using Core.Processing.Logic;

    public static class NinjectModuleManager
    {
        public class CarbonEditorModule : NinjectModule
        {
            public override void Load()
            {
                this.Bind<IResourceProcessor>().To<ResourceProcessor>().InSingletonScope();
            }
        }
    }
}