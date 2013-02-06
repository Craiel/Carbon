using Carbon.Project.Contracts;
using Carbon.Project.Data;

using Ninject.Modules;

namespace Carbon.Project.Ninject
{
    public static class NinjectModuleManager
    {
        public static NinjectModule[] GetModules()
        {
            return new NinjectModule[] { new ProjectModule() };
        }

        public class ProjectModule : NinjectModule
        {
            public override void Load()
            {
                this.Bind<ICarbonProject>().To<CarbonProject>();
            }
        }
    }
}