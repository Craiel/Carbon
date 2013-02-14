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

        public IPlayfieldViewModel GetPlayfieldViewModel(PlayfieldEntry data)
        {
            return this.kernel.Get<IPlayfieldViewModel>(new ConstructorArgument("data", data));
        }

        public IModelViewModel GetModelViewModel(SourceModel data)
        {
            return this.kernel.Get<IModelViewModel>(new ConstructorArgument("data", data));
        }
    }
}
