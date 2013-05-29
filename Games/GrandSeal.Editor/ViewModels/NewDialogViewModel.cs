using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using GrandSeal.Editor.Contracts;
using GrandSeal.Editor.Logic;

namespace GrandSeal.Editor.ViewModels
{
    public class NewDialogViewModel : EditorBase, INewDialogViewModel
    {
        private readonly IMainViewModel mainViewModel;
        private readonly IEditorLogic logic;
        private readonly List<IDocumentTemplate> filteredTemplates;

        private string name;

        private IDocumentTemplate selectedTemplate;
        private IDocumentTemplateCategory selectedCategory;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public NewDialogViewModel(IMainViewModel mainViewModel, IEditorLogic logic)
        {
            this.mainViewModel = mainViewModel;
            this.logic = logic;

            this.filteredTemplates = new List<IDocumentTemplate>();

            this.SelectedCategory = this.Categories.FirstOrDefault();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                if (this.name != value)
                {
                    this.name = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        public ReadOnlyCollection<IDocumentTemplate> Templates
        {
            get
            {
                return this.filteredTemplates.AsReadOnly();
            }
        }

        public IDocumentTemplate SelectedTemplate
        {
            get
            {
                return this.selectedTemplate;
            }

            set
            {
                if (this.selectedTemplate != value)
                {
                    this.selectedTemplate = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        public ReadOnlyCollection<IDocumentTemplateCategory> Categories
        {
            get
            {
                return this.mainViewModel.DocumentTemplateCategories;
            }
        }

        public IDocumentTemplateCategory SelectedCategory
        {
            get
            {
                return this.selectedCategory;
            }

            set
            {
                if (this.selectedCategory != value)
                {
                    this.selectedCategory = value;
                    this.FilterTemplateList();
                    if (this.SelectedTemplate != null && !this.filteredTemplates.Contains(this.SelectedTemplate))
                    {
                        this.SelectedTemplate = null;
                    }
                    this.NotifyPropertyChanged();
                }
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void FilterTemplateList()
        {
            this.filteredTemplates.Clear();
            if (this.selectedCategory != null)
            {
                foreach (IDocumentTemplate documentTemplate in this.mainViewModel.DocumentTemplates)
                {
                    foreach (IDocumentTemplateCategory category in documentTemplate.Categories)
                    {
                        if (this.selectedCategory == category || this.selectedCategory.Contains(category))
                        {
                            this.filteredTemplates.Add(documentTemplate);
                            break;
                        }
                        ;
                    }
                }
            }

            this.NotifyPropertyChanged("Templates");
        }
    }
}
