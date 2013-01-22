using Carbed.Contracts;

using Carbon.Editor.Resource;

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
        public IProjectFolderViewModel GetFolderViewModel(SourceProjectFolder data)
        {
            return this.kernel.Get<IProjectFolderViewModel>(new ConstructorArgument("data", data));
        }

        public ITextureFontViewModel GetTextureFontViewModel(SourceTextureFont data)
        {
            return this.kernel.Get<ITextureFontViewModel>(new ConstructorArgument("data", data));
        }

        public IModelViewModel GetModelViewModel(SourceModel data)
        {
            return this.kernel.Get<IModelViewModel>(new ConstructorArgument("data", data));
        }
    }
}
