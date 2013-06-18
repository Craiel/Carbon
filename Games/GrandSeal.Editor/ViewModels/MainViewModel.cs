﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

using GrandSeal.Editor.Contracts;
using GrandSeal.Editor.Events;
using GrandSeal.Editor.Logic;
using GrandSeal.Editor.Logic.MVVM;
using GrandSeal.Editor.Views;

using Core.Engine.Contracts;
using Core.Engine.Resource.Content;

using Core.Utils.Contracts;

using Microsoft.Win32;

namespace GrandSeal.Editor.ViewModels
{
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    public class MainViewModel : EditorBase, IMainViewModel
    {
        private const string DefaultProjectFileExtension = ".crbn";
        private const string DefaultProjectLayoutExtension = ".cedl";

        private readonly IEditorLogic logic;
        private readonly IEngineFactory factory;
        private readonly IViewModelFactory viewModelFactory;
        private readonly IEventRelay eventRelay;
        private readonly IUndoRedoManager undoRedoManager;
        private readonly IOperationProgress operationProgress;
        private readonly IPropertyViewModel propertyViewModel;
        private readonly IResourceExplorerViewModel resourceExplorerViewModel;
        private readonly IMaterialExplorerViewModel materialExplorerViewModel;
        private readonly IFontExplorerViewModel fontExplorerViewModel;

        private readonly List<IDocumentTemplate> documentTemplates;
        private readonly List<IDocumentTemplateCategory> documentTemplateCategories;

        private readonly List<IEditorTool> availableTools;

        private string currentProjectFile;

        private bool isClosing;

        private IProjectViewModel projectViewModel;
        private IEditorSettingsViewModel settingsViewModel;

        private IEditorDocument activeDocument;

        private IFolderViewModel currentCreationContext;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public MainViewModel(IEditorLogic logic, IEngineFactory factory)
        {
            this.logic = logic;
            this.factory = factory;
            this.viewModelFactory = factory.Get<IViewModelFactory>();
            this.eventRelay = factory.Get<IEventRelay>();
            this.undoRedoManager = factory.Get<IUndoRedoManager>();
            this.undoRedoManager.PropertyChanged += this.OnUndoRedoManagerChanged;
            this.operationProgress = factory.Get<IOperationProgress>();
            this.propertyViewModel = factory.Get<IPropertyViewModel>();
            this.resourceExplorerViewModel = factory.Get<IResourceExplorerViewModel>();
            this.materialExplorerViewModel = factory.Get<IMaterialExplorerViewModel>();
            this.fontExplorerViewModel = factory.Get<IFontExplorerViewModel>();
            
            this.documentTemplates = new List<IDocumentTemplate>();
            this.documentTemplateCategories = new List<IDocumentTemplateCategory>();

            this.ToolWindows = new ObservableCollection<IEditorTool>()
                                   {
                                       this.propertyViewModel,
                                       this.resourceExplorerViewModel,
                                       this.materialExplorerViewModel,
                                       this.fontExplorerViewModel,
                                   };

            this.Documents = new ObservableCollection<IEditorDocument>();

            this.CommandNewProject = new RelayCommand(this.OnNewProject);
            this.CommandNewMaterial = new RelayCommand(this.OnNewMaterial, this.CanCreateContent);
            this.CommandNewStage = new RelayCommand(this.OnNewStage, this.CanCreateContent);
            this.CommandNewFont = new RelayCommand(this.OnNewFont, this.CanCreateContent);
            this.CommandNewModel = new RelayCommand(this.OnNewModel, this.CanCreateContent);
            this.CommandNewResource = new RelayCommand(this.OnNewResource, this.CanCreateResource);
            this.CommandOpenProject = new RelayCommand(this.OnOpenProject);
            this.CommandUndo = new RelayCommand(this.OnUndo, this.CanUndo);
            this.CommandRedo = new RelayCommand(this.OnRedo, this.CanRedo);
            this.CommandCloseProject = new RelayCommand(this.OnCloseProject, this.CanCloseProject);
            this.CommandSaveProject = new RelayCommand(this.OnSaveProject, this.CanSaveProject);
            this.CommandSaveProjectAs = new RelayCommand(this.OnSaveProjectAs, this.CanSaveProject);
            this.CommandBuild = new RelayCommand(this.OnBuild, this.CanBuild);
            this.CommandExit = new RelayCommand(this.OnExit);
            this.CommandOpenResourceExplorer = new RelayCommand(this.OnShowResourceExplorer);
            this.CommandOpenMaterialExplorer = new RelayCommand(this.OnShowMaterialExplorer);
            this.CommandOpenFontExplorer = new RelayCommand(this.OnShowFontExplorer);
            this.CommandOpenProperties = new RelayCommand(this.OnShowProperties);
            this.CommandOpenNewDialog = new RelayCommand(this.OnNewDialog);
            this.CommandReload = new RelayCommand(this.OnReload, this.CanReload);
            this.CommandOpenSettings = new RelayCommand(this.OnShowSettings);
            
            this.eventRelay.Subscribe<EventWindowClosing>(this.OnMainWindowClosing);
            
            this.LoadDocumentTemplates();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public event PropertyChangedEventHandler ProjectChanged;

        public ObservableCollection<IEditorTool> ToolWindows { get; private set; }
        public ObservableCollection<IEditorDocument> Documents { get; private set; }

        public ReadOnlyCollection<IDocumentTemplate> DocumentTemplates
        {
            get
            {
                return this.documentTemplates.AsReadOnly();
            }
        }

        public ReadOnlyCollection<IDocumentTemplateCategory> DocumentTemplateCategories
        {
            get
            {
                return this.documentTemplateCategories.AsReadOnly();
            }
        }

        public ReadOnlyCollection<UndoRedoOperation> UndoOperations
        {
            get
            {
                if (this.undoRedoManager.ActiveGroup == null)
                {
                    return null;
                }

                return this.undoRedoManager.ActiveGroup.UndoOperations;
            }
        }

        public ReadOnlyCollection<UndoRedoOperation> RedoOperations
        {
            get
            {
                if (this.undoRedoManager.ActiveGroup == null)
                {
                    return null;
                }

                return this.undoRedoManager.ActiveGroup.RedoOperations;
            }
        }

        public IEditorDocument ActiveDocument
        {
            get
            {
                return this.activeDocument;
            }
            set
            {
                if (this.activeDocument != value)
                {
                    this.activeDocument = value;
                    this.propertyViewModel.ActiveContext = this.activeDocument;
                    this.NotifyPropertyChanged();
                    this.NotifyProjectChanged();
                }
            }
        }

        public string ProjectTitle
        {
            get
            {
                if (this.Project == null)
                {
                    return "<no project loaded>";
                }

                return this.Project.Name;
            }
        }

        public DateTime DateTime
        {
            get
            {
                return DateTime.Now;
            }
        }

        public IProjectViewModel Project
        {
            get
            {
                if (this.projectViewModel == null)
                {
                    this.projectViewModel = this.factory.Get<IProjectViewModel>();
                }

                return this.projectViewModel;
            }
        }

        public IOperationProgress OperationProgress
        {
            get
            {
                return this.operationProgress;
            }
        }

        public ICommand CommandNewProject { get; private set; }
        public ICommand CommandNewMaterial { get; private set; }
        public ICommand CommandNewStage { get; private set; }
        public ICommand CommandNewFont { get; private set; }
        public ICommand CommandNewModel { get; private set; }
        public ICommand CommandNewResource { get; private set; }

        public ICommand CommandOpenProject { get; private set; }
        public ICommand CommandCloseProject { get; private set; }
        public ICommand CommandSaveProject { get; private set; }
        public ICommand CommandSaveProjectAs { get; private set; }

        public ICommand CommandBuild { get; private set; }

        public ICommand CommandUndo { get; private set; }
        public ICommand CommandRedo { get; private set; }

        public ICommand CommandExit { get; private set; }

        public ICommand CommandReload { get; private set; }

        public ICommand CommandOpenResourceExplorer { get; private set; }
        public ICommand CommandOpenMaterialExplorer { get; private set; }
        public ICommand CommandOpenFontExplorer { get; private set; }
        public ICommand CommandOpenProperties { get; private set; }
        public ICommand CommandOpenNewDialog { get; private set; }
        public ICommand CommandOpenSettings { get; private set; }

        public void OpenDocument(IEditorDocument document)
        {
            if (this.Documents.Contains(document))
            {
                return;
            }

            this.Documents.Add(document);
        }

        public void CloseDocument(IEditorDocument document)
        {
            if (this.Documents.Contains(document))
            {
                this.Documents.Remove(document);
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void NotifyProjectChanged()
        {
            PropertyChangedEventHandler handler = ProjectChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs("Project"));
            }
        }

        private void OnNewProject(object obj)
        {
            if (this.CanCloseProject(null))
            {
                this.OnCloseProject(null);
            }

            if (this.logic.IsProjectLoaded)
            {
                if (
                    MessageBox.Show(
                        "Creating a new Project will close the existing one, continue?",
                        "Confirm",
                        MessageBoxButton.OKCancel,
                        MessageBoxImage.Question,
                        MessageBoxResult.Cancel) == MessageBoxResult.Cancel)
                {
                    return;
                }

                this.logic.CloseProject();
            }

            this.logic.NewProject();
            this.currentProjectFile = null;
            this.NotifyProjectChange();
        }

        private void OnNewResource(object obj)
        {
            if (this.currentCreationContext == null)
            {
                throw new InvalidOperationException("New resource can not be created without context");
            }

            // Todo...
            //IResourceViewModel vm = this.logic.AddResource();
            //this.currentCreationContext.AddContent(vm);
        }

        private void OnNewModel(object obj)
        {
            throw new NotImplementedException();
        }

        private void OnNewFont(object obj)
        {
            this.logic.AddFont();
        }

        private void OnNewStage(object obj)
        {
            throw new NotImplementedException();
        }

        private void OnNewMaterial(object obj)
        {
            this.logic.AddMaterial();
        }

        private void OnReload(object obj)
        {
            this.logic.Reload();
        }

        private bool CanReload(object obj)
        {
            return this.logic.IsProjectLoaded;
        }

        private void OnNewDialog(object obj)
        {
            this.currentCreationContext = obj as IFolderViewModel;
            var vm = this.factory.Get<INewDialogViewModel>();
            var view = new NewDocumentView { DataContext = vm };
            view.ShowDialog();
        }

        private bool CanCreateContent(object obj)
        {
            return this.logic.IsProjectLoaded;
        }

        private bool CanCreateResource(object obj)
        {
            return this.logic.IsProjectLoaded;
        }

        private void OnUndoRedoManagerChanged(object sender, PropertyChangedEventArgs args)
        {
            this.NotifyUndoRedoChanged();
        }

        private void NotifyProjectChange()
        {
            this.NotifyPropertyChanged("Project");
            this.NotifyPropertyChanged("ProjectTitle");
        }

        private void NotifyUndoRedoChanged()
        {
            this.NotifyPropertyChanged("UndoOperations");
            this.NotifyPropertyChanged("RedoOperations");
            this.NotifyPropertyChanged("CommandUndo");
            this.NotifyPropertyChanged("CommandRedo");
        }

        private void OnRedo(object obj)
        {
            this.undoRedoManager.ActiveGroup.Redo();
            this.NotifyUndoRedoChanged();
        }

        private bool CanRedo(object obj)
        {
            return this.undoRedoManager.ActiveGroup != null && this.undoRedoManager.ActiveGroup.CanRedo;
        }

        private void OnUndo(object obj)
        {
            this.undoRedoManager.ActiveGroup.Undo();
            this.NotifyUndoRedoChanged();
        }

        private bool CanUndo(object obj)
        {
            return this.undoRedoManager.ActiveGroup != null && this.undoRedoManager.ActiveGroup.CanUndo;
        }

        private bool CanCloseProject(object obj)
        {
            return this.logic.IsProjectLoaded;
        }

        private void OnCloseProject(object obj)
        {
            this.SaveProjectLayout();
            this.logic.CloseProject();
            this.currentProjectFile = null;
            this.NotifyProjectChange();
        }

        private void OnOpenProject(object obj)
        {
            var dialog = new OpenFileDialog
                {
                    CheckFileExists = true,
                    DefaultExt = DefaultProjectFileExtension
                };

            if (dialog.ShowDialog() == true)
            {
                if (this.CanCloseProject(null))
                {
                    this.OnCloseProject(null);
                }

                this.logic.OpenProject(dialog.FileName);
                this.currentProjectFile = dialog.FileName;
                this.RestoreProjectLayout();
                this.NotifyProjectChange();
            }
        }

        private bool CanSaveProject(object obj)
        {
            return this.logic.IsProjectLoaded;
        }

        private void OnSaveProject(object obj)
        {
            if(string.IsNullOrEmpty(this.currentProjectFile))
            {
                this.OnSaveProjectAs(obj);
                return;
            }

            this.logic.SaveProject(this.currentProjectFile);
        }

        private void OnSaveProjectAs(object obj)
        {
            var dialog = new OpenFileDialog
                {
                    CheckPathExists = true,
                    CheckFileExists = false,
                    DefaultExt = DefaultProjectFileExtension
                };

            if (dialog.ShowDialog() == true)
            {
                this.logic.SaveProject(dialog.FileName);
                this.currentProjectFile = dialog.FileName;
            }
        }

        private void OnExit(object obj)
        {
            Application.Current.Shutdown(0);
        }
        
        private void OnShowResourceExplorer(object obj)
        {
            this.resourceExplorerViewModel.IsVisible = true;
            this.resourceExplorerViewModel.IsActive = true;
        }

        private void OnShowMaterialExplorer(object obj)
        {
            this.materialExplorerViewModel.IsVisible = true;
            this.materialExplorerViewModel.IsActive = true;
        }

        private void OnShowFontExplorer(object obj)
        {
            this.fontExplorerViewModel.IsVisible = true;
            this.fontExplorerViewModel.IsActive = true;
        }

        private void OnShowProperties(object obj)
        {
            this.propertyViewModel.IsVisible = true;
            this.propertyViewModel.IsActive = true;
        }

        private void OnShowSettings(object obj)
        {
            if (this.settingsViewModel == null)
            {
                this.settingsViewModel = this.factory.Get<IEditorSettingsViewModel>();
            }

            if (this.Documents.Contains(this.settingsViewModel))
            {
                return;
            }

            this.Documents.Add(this.settingsViewModel);
        }

        private void LoadDocumentTemplates()
        {
            var globalMain = new DocumentTemplateCategory { Name = "Project" };
            var contentMain = new DocumentTemplateCategory { Name = "Content" };
            var resourceMain = new DocumentTemplateCategory { Name = "Resource" };

            this.documentTemplateCategories.Clear();
            this.documentTemplateCategories.Add(globalMain);
            this.documentTemplateCategories.Add(contentMain);
            this.documentTemplateCategories.Add(resourceMain);

            StaticResources.ProjectTemplate.CommandCreate = this.CommandNewProject;
            StaticResources.ProjectTemplate.Categories.Add(globalMain);
            this.documentTemplates.Add(StaticResources.ProjectTemplate);

            StaticResources.MaterialTemplate.CommandCreate = this.CommandNewMaterial;
            StaticResources.MaterialTemplate.Categories.Add(contentMain);
            this.documentTemplates.Add(StaticResources.MaterialTemplate);

            StaticResources.StageTemplate.CommandCreate = this.CommandNewStage;
            StaticResources.StageTemplate.Categories.Add(contentMain);
            this.documentTemplates.Add(StaticResources.StageTemplate);

            StaticResources.FontTemplate.CommandCreate = this.CommandNewFont;
            StaticResources.FontTemplate.Categories.Add(contentMain);
            this.documentTemplates.Add(StaticResources.FontTemplate);

            StaticResources.ModelTemplate.CommandCreate = this.CommandNewModel;
            StaticResources.ModelTemplate.Categories.Add(contentMain);
            this.documentTemplates.Add(StaticResources.ModelTemplate);

            StaticResources.TextureTemplate.CommandCreate = this.CommandNewResource;
            StaticResources.TextureTemplate.CreateParameter = ResourceType.Texture;
            StaticResources.TextureTemplate.Categories.Add(resourceMain);
            this.documentTemplates.Add(StaticResources.TextureTemplate);

            StaticResources.MeshTemplate.CommandCreate = this.CommandNewResource;
            StaticResources.MeshTemplate.CreateParameter = ResourceType.Model;
            StaticResources.MeshTemplate.Categories.Add(resourceMain);
            this.documentTemplates.Add(StaticResources.MeshTemplate);
        }

        private void OnBuild(object obj)
        {
            var dialog = new OpenFileDialog { CheckPathExists = true, CheckFileExists = false };
            if (dialog.ShowDialog() != true)
            {
                return;
            }

            string folder = Path.GetDirectoryName(dialog.FileName);
            if (Directory.GetFiles(folder, "*", SearchOption.TopDirectoryOnly).Length > 0)
            {
                if (MessageBox.Show(
                    "Folder is not empty, continue?",
                    "Confirmation",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Question,
                    MessageBoxResult.Cancel) == MessageBoxResult.Cancel)
                {
                    return;
                }
            }

            throw new NotImplementedException();
            //this.logic.Build(folder);
        }

        private bool CanBuild(object obj)
        {
            return this.logic.IsProjectLoaded;
        }

        private void RestoreProjectLayout()
        {
            if (string.IsNullOrEmpty(this.currentProjectFile))
            {
                return;
            }

            string file = Path.Combine(
                Path.GetDirectoryName(this.currentProjectFile),
                Path.GetFileNameWithoutExtension(this.currentProjectFile) + DefaultProjectLayoutExtension);
            if (!File.Exists(file))
            {
                return;
            }

            this.eventRelay.Relay(new EventLoadLayout(this, file));
        }

        private void SaveProjectLayout()
        {
            if (string.IsNullOrEmpty(this.currentProjectFile))
            {
                return;
            }

            string file = Path.Combine(
                Path.GetDirectoryName(this.currentProjectFile),
                Path.GetFileNameWithoutExtension(this.currentProjectFile) + DefaultProjectLayoutExtension);

            this.eventRelay.Relay(new EventSaveLayout(file));
        }

        private void OnMainWindowClosing(EventWindowClosing obj)
        {
            this.isClosing = true;

            this.SaveProjectLayout();
        }
    }
}