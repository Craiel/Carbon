using Core.Utils.Contracts;

using Ninject.Modules;

namespace Core.Utils.Ninject
{
    public static class NinjectModuleManager
    {
        public class UtilsModule : NinjectModule
        {
            public override void Load()
            {
                this.Bind<IFormatter>().To<Formatter>();
                this.Bind<IEventRelay>().To<EventRelay>().InSingletonScope();
            }
        }
    }
}