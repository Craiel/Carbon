using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

using Carbed.Contracts;
using Carbed.Logic.MVVM;

namespace Carbed.ViewModels
{
    public class ResourceExplorerViewModel : ToolViewModel, IResourceExplorerViewModel
    {
        private readonly ICarbedLogic logic;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ResourceExplorerViewModel(ICarbedLogic logic)
        {
            this.logic = logic;
            this.logic.ProjectChanged += this.OnProjectChanged;

            this.CommandSave = new RelayCommand(this.OnSave, this.CanSave);
            this.CommandAddFolder = new RelayCommand(this.OnAddFolder, this.CanAddFolder);
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override string Title
        {
            get
            {
                if (this.logic.HasProjectLoaded)
                {
                    //return string.Format("Project '{0}'", this.logic.Project.Name);
                }

                return "Resource Explorer";
            }
        }

        public IReadOnlyCollection<IFolderViewModel> Folders
        {
            get
            {
                return this.logic.Folders;
            }
        }

        public ICommand CommandSave { get; private set; }
        public ICommand CommandAddFolder { get; private set; }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void OnProjectChanged()
        {
            this.NotifyPropertyChanged("Folders");
        }

        private void OnSave(object obj)
        {
            foreach (IFolderViewModel folder in Folders)
            {
                folder.CommandSave.Execute(obj);
            }
        }

        private bool CanSave(object obj)
        {
            return this.Folders.Any(x => x.CommandSave.CanExecute(obj));
        }

        private void OnAddFolder(object obj)
        {
            this.logic.AddFolder();
        }

        private bool CanAddFolder(object obj)
        {
            return this.logic.HasProjectLoaded;
        }
    }
}
