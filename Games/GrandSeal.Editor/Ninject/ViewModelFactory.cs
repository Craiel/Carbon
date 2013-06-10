﻿using GrandSeal.Editor.Contracts;

using Core.Engine.Resource.Content;

using Ninject;
using Ninject.Parameters;

namespace GrandSeal.Editor.Ninject
{
    public class ViewModelFactory : IViewModelFactory
    {
        private readonly IKernel kernel;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ViewModelFactory(IKernel kernel)
        {
            this.kernel = kernel;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public IFolderViewModel GetFolderViewModel(ResourceTree data)
        {
            return this.kernel.Get<IFolderViewModel>(new ConstructorArgument("data", data));
        }

        public IStageViewModel GetStageViewModel(StageEntry data)
        {
            return this.kernel.Get<IStageViewModel>(new ConstructorArgument("data", data));
        }

        public IMaterialViewModel GetMaterialViewModel(MaterialEntry data)
        {
            return this.kernel.Get<IMaterialViewModel>(new ConstructorArgument("data", data));
        }

        public IFontViewModel GetFontViewModel(FontEntry data)
        {
            return this.kernel.Get<IFontViewModel>(new ConstructorArgument("data", data));
        }

        public IResourceTextureViewModel GetResourceTextureViewModel(ResourceEntry data)
        {
            return this.kernel.Get<IResourceTextureViewModel>(new ConstructorArgument("data", data));
        }

        public IResourceModelViewModel GetResourceModelViewModel(ResourceEntry data)
        {
            return this.kernel.Get<IResourceModelViewModel>(new ConstructorArgument("data", data));
        }

        public IResourceScriptViewModel GetResourceScriptViewModel(ResourceEntry data)
        {
            return this.kernel.Get<IResourceScriptViewModel>(new ConstructorArgument("data", data));
        }

        public IResourceRawViewModel GetResourceRawViewModel(ResourceEntry data)
        {
            return this.kernel.Get<IResourceRawViewModel>(new ConstructorArgument("data", data));
        }

        public IResourceFontViewModel GetResourceFontViewModel(ResourceEntry data)
        {
            return this.kernel.Get<IResourceFontViewModel>(new ConstructorArgument("data", data));
        }

        public IResourceStageViewModel GetResourceStageViewModel(ResourceEntry data)
        {
            return this.kernel.Get<IResourceStageViewModel>(new ConstructorArgument("data", data));
        }

        public IResourceUserInterfaceViewModel GetResourceUserInterfaceViewModel(ResourceEntry data)
        {
            return this.kernel.Get<IResourceUserInterfaceViewModel>(new ConstructorArgument("data", data));
        }
    }
}
