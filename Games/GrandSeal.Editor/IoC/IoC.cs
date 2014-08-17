namespace GrandSeal.Editor.IoC
{
    using Autofac;
    using Autofac.Core;

    using CarbonCore.Utils.IoC;

    using Core.Engine.IoC;
    using Core.Processing.IoC;

    using GrandSeal.Editor.Contracts;
    using GrandSeal.Editor.Logic;
    using GrandSeal.Editor.ViewModels;
   
    [DependsOnModule(typeof(EngineModule))]
    [DependsOnModule(typeof(CarbonEditorModule))]
    [DependsOnModule(typeof(UtilsModule))]
    public class EditorModule : CarbonModule
    {
        public EditorModule()
        {
            this.For<IEditor>().Use<Editor>().Singleton();

            this.For<IApplicationLog>().Use<ApplicationLog>().Singleton();
            this.For<IOperationProgress>().Use<OperationProgress>().Singleton();
            this.For<IUndoRedoManager>().Use<UndoRedoManager>().Singleton();
            this.For<IEditorLogic>().Use<EditorLogic>().Singleton();
            this.For<IEditorLog>().Use<EditorLog>().Singleton();
            this.For<IEditorSettings>().Use<EditorSettings>().Singleton();

            this.For<IMainViewModel>().Use<MainViewModel>().Singleton();

            this.For<ITextureSynchronizer>().Use<TextureSynchronizer>();

            // Document views
            this.For<IEditorSettingsViewModel>().Use<EditorSettingsViewModel>();
            this.For<IProjectViewModel>().Use<ProjectViewModel>();
            this.For<IFolderViewModel>().Use<FolderViewModel>();
            this.For<IStageViewModel>().Use<StageViewModel>();
            this.For<IResourceTextureViewModel>().Use<ResourceTextureViewModel>();
            this.For<IResourceModelViewModel>().Use<ResourceModelViewModel>();
            this.For<IResourceScriptViewModel>().Use<ResourceScriptViewModel>();
            this.For<IResourceFontViewModel>().Use<ResourceFontViewModel>();
            this.For<IResourceRawViewModel>().Use<ResourceRawViewModel>();
            this.For<IResourceStageViewModel>().Use<ResourceStageViewModel>();
            this.For<IResourceUserInterfaceViewModel>().Use<ResourceUserInterfaceViewModel>();
            this.For<IMaterialViewModel>().Use<MaterialViewModel>();
            this.For<IFontViewModel>().Use<FontViewModel>();

            // Tool views
            this.For<IResourceExplorerViewModel>().Use<ResourceExplorerViewModel>().Singleton();
            this.For<IMaterialExplorerViewModel>().Use<MaterialExplorerViewModel>().Singleton();
            this.For<IFontExplorerViewModel>().Use<FontExplorerViewModel>().Singleton();
            this.For<IPropertyViewModel>().Use<PropertyViewModel>().Singleton();
            this.For<INewDialogViewModel>().Use<NewDialogViewModel>().Singleton();
        }
    }
}