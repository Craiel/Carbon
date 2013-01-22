using Ninject.Modules;

namespace Carbon.Editor.Ninject
{
    using Carbon.Editor.Contracts;
    using Carbon.Editor.Logic;

    public static class NinjectModuleManager
    {
        public class CarbonEditorModule : NinjectModule
        {
            public override void Load()
            {
                this.Bind<ICarbonBuilder>().To<CarbonBuilder>().InSingletonScope();
            }
        }
    }
}