using Ninject.Modules;

namespace Core.Editor.Ninject
{
    using Core.Editor.Contracts;
    using Core.Editor.Logic;

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