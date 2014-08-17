using System.Collections.Generic;
using System.Windows.Input;

using GrandSeal.Editor.Contracts;
using GrandSeal.Editor.Logic.MVVM;

namespace GrandSeal.Editor.ViewModels
{
    using System;
    using System.Threading.Tasks;

    using CarbonCore.UtilsWPF;

    using global::GrandSeal.Editor.Views;

    public class ResourceExplorerViewModel : ToolViewModel, IResourceExplorerViewModel
    {
        private readonly IEditorLogic logic;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ResourceExplorerViewModel(IEditorLogic logic)
        {
            this.logic = logic;
            this.logic.ProjectChanged += this.OnProjectChanged;

            this.CommandSave = new RelayCommand(this.OnSave, this.CanSave);
            this.CommandRefresh = new RelayCommand(this.OnRefresh);
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
                return "Resources";
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
        public ICommand CommandRefresh { get; private set; }
        public ICommand CommandAddFolder { get; private set; }
        public ICommand CommandReload { get; private set; }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void OnProjectChanged()
        {
            this.NotifyPropertyChanged("Folders");
        }

        private void OnSave()
        {
            TaskProgress.Message = "Saving Resources...";
            new TaskProgress(new[] { new Task(this.DoSave) }, 1);
        }

        private void DoSave()
        {
            TaskProgress.CurrentProgress = 0;
            TaskProgress.CurrentMaxProgress = this.Folders.Count;
            foreach (IFolderViewModel folder in this.Folders)
            {
                TaskProgress.CurrentMessage = folder.FullPath.ToString();
                folder.CommandRefresh.Execute(null);
                folder.CommandSave.Execute(null);
                TaskProgress.CurrentProgress++;
            }
        }

        private void OnRefresh()
        {
            TaskProgress.Message = "Refreshing Resource Status...";
            new TaskProgress(new[] { new Task(this.DoRefresh) }, 1);
        }

        private void DoRefresh()
        {
            TaskProgress.CurrentProgress = 0;
            TaskProgress.CurrentMaxProgress = this.Folders.Count;
            foreach (IFolderViewModel folder in this.Folders)
            {
                TaskProgress.CurrentMessage = folder.FullPath.ToString();
                folder.CommandRefresh.Execute(null);
                TaskProgress.CurrentProgress++;
            }
        }

        private bool CanSave()
        {
            return true;
        }

        private void OnAddFolder()
        {
            this.logic.AddFolder();
            this.NotifyPropertyChanged("Folders");
        }

        private bool CanAddFolder()
        {
            return this.logic.IsProjectLoaded;
        }

        private void OnReload()
        {
            // Todo:
            this.NotifyPropertyChanged("Folders");
        }

        private bool CanReload()
        {
            return this.logic.IsProjectLoaded;
        }
    }
}
