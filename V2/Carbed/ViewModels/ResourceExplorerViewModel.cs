using Carbed.Contracts;

namespace Carbed.ViewModels
{
    public class ResourceExplorerViewModel : ToolViewModel, IResourceExplorerViewModel
    {
        private readonly ICarbedLogic logic;
        private readonly IViewModelFactory viewModelFactory;

        private IFolderViewModel root;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ResourceExplorerViewModel(ICarbedLogic logic, IViewModelFactory viewModelFactory)
        {
            this.logic = logic;
            this.logic.ProjectChanged += this.OnProjectChanged;
            this.viewModelFactory = viewModelFactory;

            this.CreateViewModels();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override string Title
        {
            get
            {
                if (this.logic.ProjectResources != null)
                {
                    //return string.Format("Project '{0}'", this.logic.Project.Name);
                }

                return "Resource Explorer";
            }
        }

        public IFolderViewModel Root
        {
            get
            {
                return this.root;
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void ClearViewModels()
        {
            if (this.root != null)
            {
                this.root = null;
            }
        }

        private void CreateViewModels()
        {
            this.ClearViewModels();

            if (this.logic.ProjectResources == null)
            {
                return;
            }

            this.root = this.viewModelFactory.GetFolderViewModel();
            this.NotifyPropertyChanged("Root");
        }

        private void OnProjectChanged()
        {
            this.CreateViewModels();
            this.NotifyPropertyChanged(string.Empty);
        }
    }
}
