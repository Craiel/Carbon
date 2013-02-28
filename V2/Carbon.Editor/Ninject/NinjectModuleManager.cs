using Carbon.Editor.Contracts;
using Carbon.Editor.Logic;

using Ninject.Modules;

namespace Carbon.Editor.Ninject
{
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