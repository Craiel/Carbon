using Carbed.Contracts;

using Carbon.Editor.Resource;

namespace Carbed.ViewModels
{
    public class ResourceExplorerViewModel : ToolViewModel, IResourceExplorerViewModel
    {
        private readonly ICarbedLogic logic;
        private readonly IViewModelFactory viewModelFactory;

        private IFolderViewModel content;
        
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
                if (this.logic.Project != null)
                {
                    return string.Format("Project '{0}'", this.logic.Project.Name);
                }

                return "Project <no project loaded>";
            }
        }

        public IFolderViewModel Root
        {
            get
            {
                return this.content;
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void ClearViewModels()
        {
            if (this.content != null)
            {
                this.content = null;
            }
        }

        private void CreateViewModels()
        {
            this.ClearViewModels();

            if (this.logic.Project == null)
            {
                return;
            }

            this.content = this.viewModelFactory.GetFolderViewModel(this.logic.Project.Root);
            this.NotifyPropertyChanged("Root");
        }

        private void OnProjectChanged(SourceProject project)
        {
            this.CreateViewModels();
            this.NotifyPropertyChanged(string.Empty);
        }
    }
}
