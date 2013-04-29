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
                this.Bind<ICarbedLog>().To<CarbedLog>().InSingletonScope();
                this.Bind<ICarbedSettings>().To<CarbedSettings>().InSingletonScope();
                this.Bind<IViewModelFactory>().To<ViewModelFactory>().InSingletonScope();

                this.Bind<IMainViewModel>().To<MainViewModel>().InSingletonScope();

                this.Bind<ITextureSynchronizer>().To<TextureSynchronizer>();

                // Document views
                this.Bind<ICarbedSettingsViewModel>().To<CarbedSettingsViewModel>();
                this.Bind<IProjectViewModel>().To<ProjectViewModel>();
                this.Bind<IFolderViewModel>().To<FolderViewModel>();
                this.Bind<IStageViewModel>().To<StageViewModel>();
                this.Bind<IResourceTextureViewModel>().To<ResourceTextureViewModel>();
                this.Bind<IResourceModelViewModel>().To<ResourceModelViewModel>();
                this.Bind<IResourceScriptViewModel>().To<ResourceScriptViewModel>();
                this.Bind<IResourceFontViewModel>().To<ResourceFontViewModel>();
                this.Bind<IResourceRawViewModel>().To<ResourceRawViewModel>();
                this.Bind<IResourceStageViewModel>().To<ResourceStageViewModel>();
                this.Bind<IMaterialViewModel>().To<MaterialViewModel>();
                this.Bind<IFontViewModel>().To<FontViewModel>();

                // Tool views
                this.Bind<IResourceExplorerViewModel>().To<ResourceExplorerViewModel>().InSingletonScope();
                this.Bind<IMaterialExplorerViewModel>().To<MaterialExplorerViewModel>().InSingletonScope();
                this.Bind<IFontExplorerViewModel>().To<FontExplorerViewModel>().InSingletonScope();
                this.Bind<IPropertyViewModel>().To<PropertyViewModel>().InSingletonScope();
                this.Bind<INewDialogViewModel>().To<NewDialogViewModel>().InSingletonScope();
            }
        }
    }
}