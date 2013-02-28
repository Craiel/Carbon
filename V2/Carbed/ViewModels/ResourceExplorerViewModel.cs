using System.Collections.Generic;
using System.Windows.Input;

using Carbed.Contracts;
using Carbed.Logic.MVVM;

using Microsoft.Win32;

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
            this.CommandReload = new RelayCommand(this.OnReload, this.CanReload);
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override string Title
        {
            get
            {
                if (this.logic.IsProjectLoaded)
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
        public ICommand CommandReload { get; private set; }

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
            return true;
        }

        private void OnAddFolder(object obj)
        {
            this.logic.AddFolder();
            this.NotifyPropertyChanged("Folders");
        }

        private bool CanAddFolder(object obj)
        {
            return this.logic.IsProjectLoaded;
        }

        private void OnReload(object obj)
        {
            // Todo:
            this.NotifyPropertyChanged("Folders");
        }

        private bool CanReload(object obj)
        {
            return this.logic.IsProjectLoaded;
        }
    }
}
