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

    public class EditorModule : Module
    {
        public static IModule[] GetModules()
        {
            return new IModule[]
                       {
                           new EngineModule(), new EditorModule(),
                           new CarbonEditorModule(),
                           new UtilsModule()
                       };
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<IEditor>().As<Editor>().SingleInstance();

            builder.RegisterType<IApplicationLog>().As<ApplicationLog>().SingleInstance();
            builder.RegisterType<IOperationProgress>().As<OperationProgress>().SingleInstance();
            builder.RegisterType<IUndoRedoManager>().As<UndoRedoManager>().SingleInstance();
            builder.RegisterType<IEditorLogic>().As<EditorLogic>().SingleInstance();
            builder.RegisterType<IEditorLog>().As<EditorLog>().SingleInstance();
            builder.RegisterType<IEditorSettings>().As<EditorSettings>().SingleInstance();
            builder.RegisterType<IViewModelFactory>().As<ViewModelFactory>().SingleInstance();

            builder.RegisterType<IMainViewModel>().As<MainViewModel>().SingleInstance();

            builder.RegisterType<ITextureSynchronizer>().As<TextureSynchronizer>();

            // Document views
            builder.RegisterType<IEditorSettingsViewModel>().As<EditorSettingsViewModel>();
            builder.RegisterType<IProjectViewModel>().As<ProjectViewModel>();
            builder.RegisterType<IFolderViewModel>().As<FolderViewModel>();
            builder.RegisterType<IStageViewModel>().As<StageViewModel>();
            builder.RegisterType<IResourceTextureViewModel>().As<ResourceTextureViewModel>();
            builder.RegisterType<IResourceModelViewModel>().As<ResourceModelViewModel>();
            builder.RegisterType<IResourceScriptViewModel>().As<ResourceScriptViewModel>();
            builder.RegisterType<IResourceFontViewModel>().As<ResourceFontViewModel>();
            builder.RegisterType<IResourceRawViewModel>().As<ResourceRawViewModel>();
            builder.RegisterType<IResourceStageViewModel>().As<ResourceStageViewModel>();
            builder.RegisterType<IResourceUserInterfaceViewModel>().As<ResourceUserInterfaceViewModel>();
            builder.RegisterType<IMaterialViewModel>().As<MaterialViewModel>();
            builder.RegisterType<IFontViewModel>().As<FontViewModel>();

            // Tool views
            builder.RegisterType<IResourceExplorerViewModel>().As<ResourceExplorerViewModel>().SingleInstance();
            builder.RegisterType<IMaterialExplorerViewModel>().As<MaterialExplorerViewModel>().SingleInstance();
            builder.RegisterType<IFontExplorerViewModel>().As<FontExplorerViewModel>().SingleInstance();
            builder.RegisterType<IPropertyViewModel>().As<PropertyViewModel>().SingleInstance();
            builder.RegisterType<INewDialogViewModel>().As<NewDialogViewModel>().SingleInstance();
        }
    }
}