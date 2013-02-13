namespace Carbed.ViewModels
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows.Input;

    using Carbon.Editor.Resource;
    using Carbon.Engine.Resource;
    using Carbon.Engine.Resource.Content;

    using global::Carbed.Contracts;
    using global::Carbed.Logic.MVVM;

    public enum ExplorableContent
    {
        Font
    }

    public class ContentExplorerViewModel : ToolViewModel, IContentExplorerViewModel
    {
        private readonly ICarbedLogic logic;
        private readonly IViewModelFactory viewModelFactory;

        private readonly List<ICarbedDocument> documents;

        private ICommand commandAdd;
        private ICommand commandReload;

        private ExplorableContent selectedContentType;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ContentExplorerViewModel(ICarbedLogic logic, IViewModelFactory viewModelFactory)
        {
            this.logic = logic;
            this.logic.ProjectChanged += this.OnProjectChanged;
            this.viewModelFactory = viewModelFactory;

            this.documents = new List<ICarbedDocument>();
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

        public ExplorableContent SelectedContentType
        {
            get
            {
                return this.selectedContentType;
            }

            set
            {
                this.selectedContentType = value;
                this.UpdateDocuments();
                this.NotifyPropertyChanged();
            }
        }

        public ReadOnlyCollection<ICarbedDocument> Documents
        {
            get
            {
                return this.documents.AsReadOnly();
            }
        }

        public ICommand CommandAdd
        {
            get
            {
                return this.commandAdd ?? (this.commandAdd = new RelayCommand(this.OnAdd, this.CanAdd));
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
        // Private
        // -------------------------------------------------------------------
        private void UpdateDocuments()
        {
            this.documents.Clear();

            if (this.logic.ProjectContent == null)
            {
                return;
            }

            // Todo: Support all content entries that we have a viewmodel for
            IList<FontEntry> results = this.logic.ProjectContent.TypedLoad(new ContentQuery<FontEntry>()).ToList<FontEntry>();
            for (int i = 0; i < results.Count; i++)
            {
                this.documents.Add(this.viewModelFactory.GetFontViewModel(results[i]));
            }

            this.NotifyPropertyChanged("Documents");
        }

        private void OnProjectChanged(SourceProject project)
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
            return this.logic.ProjectContent != null;
        }

        private void OnAdd(object obj)
        {
            // Todo
        }

        private bool CanAdd(object obj)
        {
            return this.logic.ProjectContent != null;
        }
    }
}
