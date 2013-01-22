using Carbed.Contracts;
using Carbed.Logic;
using Carbed.ViewModels;

using Carbon.Engine.Ninject;
using Ninject.Modules;

namespace Carbed.Ninject
{
    public static class NinjectModuleManager
    {
        public static NinjectModule[] GetModules()
        {
            return new NinjectModule[]
                       {
                           new EngineModule(), new CarbedModule(),
                           new Carbon.Project.Ninject.NinjectModuleManager.ProjectModule(),
                           new Carbon.Editor.Ninject.NinjectModuleManager.CarbonEditorModule(),
                           new Core.Utils.Ninject.NinjectModuleManager.UtilsModule(), 
                       };
        }

        public class CarbedModule : NinjectModule
        {
            public override void Load()
            {
                this.Bind<ICarbed>().To<Carbed>().InSingletonScope();

                this.Bind<IApplicationLog>().To<ApplicationLog>().InSingletonScope();
                this.Bind<IOperationProgress>().To<OperationProgress>().InSingletonScope();
                this.Bind<IUndoRedoManager>().To<UndoRedoManager>().InSingletonScope();
                this.Bind<ICarbedLogic>().To<CarbedLogic>().InSingletonScope();
                this.Bind<IViewModelFactory>().To<ViewModelFactory>().InSingletonScope();

                this.Bind<IMainViewModel>().To<MainViewModel>().InSingletonScope();

                // Document views
                this.Bind<IProjectViewModel>().To<ProjectViewModel>();
                this.Bind<IProjectFolderViewModel>().To<ProjectFolderViewModel>();
                this.Bind<ITextureFontViewModel>().To<TextureFontViewModel>();
                this.Bind<IModelViewModel>().To<ModelViewModel>();

                // Tool views
                this.Bind<IProjectExplorerViewModel>().To<ProjectExplorerViewModel>().InSingletonScope();
                this.Bind<IPropertyViewModel>().To<PropertyViewModel>().InSingletonScope();
                this.Bind<INewDialogViewModel>().To<NewDialogViewModel>().InSingletonScope();
            }
        }
    }
}