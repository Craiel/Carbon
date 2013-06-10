using GrandSeal.Editor.Contracts;
using GrandSeal.Editor.Logic;
using GrandSeal.Editor.ViewModels;

using Core.Engine.Ninject;
using Ninject.Modules;

namespace GrandSeal.Editor.Ninject
{
    public static class NinjectModuleManager
    {
        public static NinjectModule[] GetModules()
        {
            return new NinjectModule[]
                       {
                           new EngineModule(), new EditorModule(),
                           new Core.Editor.Ninject.NinjectModuleManager.CarbonEditorModule(),
                           new Core.Utils.Ninject.NinjectModuleManager.UtilsModule(), 
                       };
        }

        public class EditorModule : NinjectModule
        {
            public override void Load()
            {
                this.Bind<IEditor>().To<Editor>().InSingletonScope();

                this.Bind<IApplicationLog>().To<ApplicationLog>().InSingletonScope();
                this.Bind<IOperationProgress>().To<OperationProgress>().InSingletonScope();
                this.Bind<IUndoRedoManager>().To<UndoRedoManager>().InSingletonScope();
                this.Bind<IEditorLogic>().To<EditorLogic>().InSingletonScope();
                this.Bind<IEditorLog>().To<EditorLog>().InSingletonScope();
                this.Bind<IEditorSettings>().To<EditorSettings>().InSingletonScope();
                this.Bind<IViewModelFactory>().To<ViewModelFactory>().InSingletonScope();

                this.Bind<IMainViewModel>().To<MainViewModel>().InSingletonScope();

                this.Bind<ITextureSynchronizer>().To<TextureSynchronizer>();

                // Document views
                this.Bind<IEditorSettingsViewModel>().To<EditorSettingsViewModel>();
                this.Bind<IProjectViewModel>().To<ProjectViewModel>();
                this.Bind<IFolderViewModel>().To<FolderViewModel>();
                this.Bind<IStageViewModel>().To<StageViewModel>();
                this.Bind<IResourceTextureViewModel>().To<ResourceTextureViewModel>();
                this.Bind<IResourceModelViewModel>().To<ResourceModelViewModel>();
                this.Bind<IResourceScriptViewModel>().To<ResourceScriptViewModel>();
                this.Bind<IResourceFontViewModel>().To<ResourceFontViewModel>();
                this.Bind<IResourceRawViewModel>().To<ResourceRawViewModel>();
                this.Bind<IResourceStageViewModel>().To<ResourceStageViewModel>();
                this.Bind<IResourceUserInterfaceViewModel>().To<ResourceUserInterfaceViewModel>();
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