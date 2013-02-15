using Carbed.Contracts;

using Carbon.Editor.Resource;
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
        public IFolderViewModel GetFolderViewModel()
        {
            return this.kernel.Get<IFolderViewModel>();
        }

        public IFontViewModel GetFontViewModel(FontEntry data)
        {
            return this.kernel.Get<IFontViewModel>(new ConstructorArgument("data", data));
        }

        public IStageViewModel GetPlayfieldViewModel(PlayfieldEntry data)
        {
            return this.kernel.Get<IStageViewModel>(new ConstructorArgument("data", data));
        }

        public IResourceViewModel GetResourceViewModel(ResourceEntry data)
        {
            return this.kernel.Get<IResourceViewModel>(new ConstructorArgument("data", data));
        }
    }
}
