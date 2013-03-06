using System.Collections.Generic;
using System.Windows.Input;

using Carbed.Contracts;
using Carbed.Logic.MVVM;

namespace Carbed.ViewModels
{
    using System;
    using System.Threading.Tasks;

    using global::Carbed.Views;

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
            TaskProgress.Message = "Saving Resources...";
            new TaskProgress(new[] { new Task(() => this.DoSave(obj)) }, 1);
        }

        private void DoSave(object obj)
        {
            TaskProgress.CurrentProgress = 0;
            TaskProgress.CurrentMaxProgress = this.Folders.Count;
            foreach (IFolderViewModel folder in this.Folders)
            {
                TaskProgress.CurrentMessage = folder.FullPath;
                folder.CommandSave.Execute(obj);
                TaskProgress.CurrentProgress++;
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
