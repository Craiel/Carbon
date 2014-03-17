namespace GrandSeal.Editor.IoC
{
    using Autofac;

    using Core.Engine.Resource.Content;

    using GrandSeal.Editor.Contracts;

    public class ViewModelFactory : IViewModelFactory
    {
        private readonly IContainer kernel;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ViewModelFactory(IContainer kernel)
        {
            this.kernel = kernel;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public IFolderViewModel GetFolderViewModel(ResourceTree data)
        {
            return this.kernel.Resolve<IFolderViewModel>(new NamedParameter("data", data));
        }

        public IStageViewModel GetStageViewModel(StageEntry data)
        {
            return this.kernel.Resolve<IStageViewModel>(new NamedParameter("data", data));
        }

        public IMaterialViewModel GetMaterialViewModel(MaterialEntry data)
        {
            return this.kernel.Resolve<IMaterialViewModel>(new NamedParameter("data", data));
        }

        public IFontViewModel GetFontViewModel(FontEntry data)
        {
            return this.kernel.Resolve<IFontViewModel>(new NamedParameter("data", data));
        }

        public IResourceTextureViewModel GetResourceTextureViewModel(ResourceEntry data)
        {
            return this.kernel.Resolve<IResourceTextureViewModel>(new NamedParameter("data", data));
        }

        public IResourceModelViewModel GetResourceModelViewModel(ResourceEntry data)
        {
            return this.kernel.Resolve<IResourceModelViewModel>(new NamedParameter("data", data));
        }

        public IResourceScriptViewModel GetResourceScriptViewModel(ResourceEntry data)
        {
            return this.kernel.Resolve<IResourceScriptViewModel>(new NamedParameter("data", data));
        }

        public IResourceRawViewModel GetResourceRawViewModel(ResourceEntry data)
        {
            return this.kernel.Resolve<IResourceRawViewModel>(new NamedParameter("data", data));
        }

        public IResourceFontViewModel GetResourceFontViewModel(ResourceEntry data)
        {
            return this.kernel.Resolve<IResourceFontViewModel>(new NamedParameter("data", data));
        }

        public IResourceStageViewModel GetResourceStageViewModel(ResourceEntry data)
        {
            return this.kernel.Resolve<IResourceStageViewModel>(new NamedParameter("data", data));
        }

        public IResourceUserInterfaceViewModel GetResourceUserInterfaceViewModel(ResourceEntry data)
        {
            return this.kernel.Resolve<IResourceUserInterfaceViewModel>(new NamedParameter("data", data));
        }
    }
}
