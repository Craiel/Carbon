using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

using Carbed.Contracts;
using Carbed.Logic.MVVM;

using Carbon.Engine.Contracts;

namespace Carbed.ViewModels
{
    public abstract class ContentExplorerViewModel : ToolViewModel, IContentExplorerViewModel
    {
        private readonly ICarbedLogic logic;
        private readonly IEngineFactory factory;

        private readonly List<ICarbedDocument> documents;

        private ICommand commandReload;

        private IMainViewModel mainViewModel;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        protected ContentExplorerViewModel(IEngineFactory factory, ICarbedLogic logic)
        {
            this.factory = factory;
            this.logic = logic;
            this.logic.ProjectChanged += this.OnProjectChanged;

            this.documents = new List<ICarbedDocument>();
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

                return "Content Explorer";
            }
        }
        
        public ReadOnlyCollection<ICarbedDocument> Documents
        {
            get
            {
                return this.documents.AsReadOnly();
            }
        }

        public ICommand CommandOpenNewDialog
        {
            get
            {
                // Has to be acquired on demand otherwise we get circular IoC dependency
                if (this.mainViewModel == null)
                {
                    this.mainViewModel = this.factory.Get<IMainViewModel>();
                }

                return this.mainViewModel.CommandOpenNewDialog;
            }
        }

        public ICommand CommandReload
        {
            get
            {
                return this.commandReload ?? (this.commandReload = new RelayCommand(this.OnReload, this.CanReload));
            }
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected abstract void DoUpdate(List<ICarbedDocument> target);

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void UpdateDocuments()
        {
            this.documents.Clear();

            if (!this.logic.HasProjectLoaded)
            {
                return;
            }

            this.DoUpdate(this.documents);
            this.NotifyPropertyChanged("Documents");
        }

        private void OnProjectChanged()
        {
            this.UpdateDocuments();
            this.NotifyPropertyChanged(string.Empty);
        }

        private void OnReload(object obj)
        {
            this.UpdateDocuments();
        }

        private bool CanReload(object obj)
        {
            return this.logic.HasProjectLoaded;
        }
    }
}
