using Carbed.Contracts;

using Carbon.Engine.Resource.Content;

using Ninject;
using Ninject.Parameters;

namespace Carbed.Ninject
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

        public IFontViewModel GetFontViewModel(FontEntry data)
        {
            return this.kernel.Get<IFontViewModel>(new ConstructorArgument("data", data));
        }

        public IStageViewModel GetStageViewModel(StageEntry data)
        {
            return this.kernel.Get<IStageViewModel>(new ConstructorArgument("data", data));
        }

        public IMaterialViewModel GetMaterialViewModel(MaterialEntry data)
        {
            return this.kernel.Get<IMaterialViewModel>(new ConstructorArgument("data", data));
        }

        public IResourceViewModel GetResourceViewModel(ResourceEntry data)
        {
            return this.kernel.Get<IResourceViewModel>(new ConstructorArgument("data", data));
        }
    }
}
