﻿namespace GrandSeal.Editor.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Input;

    using CarbonCore.ToolFramework.ViewModel;
    using CarbonCore.Utils.Compat.Contracts;
    using CarbonCore.Utils.Compat.IO;
    using CarbonCore.Utils.Contracts.IoC;
    using CarbonCore.UtilsWPF;

    using Core.Engine.Resource.Content;
    using GrandSeal.Editor.Contracts;
    using GrandSeal.Editor.Events;
    using GrandSeal.Editor.Logic;
    using GrandSeal.Editor.Logic.Docking;
    using GrandSeal.Editor.Views;
    using Microsoft.Win32;
    
    using Xceed.Wpf.AvalonDock.Themes;

    public class MainViewModel : BaseViewModel, IMainViewModel
    {
        private const string DefaultProjectFileExtension = ".crbn";
        private const string DefaultProjectLayoutFile = "CarbonLayout.cedl";

        private readonly IEditorLogic logic;
        private readonly IFactory factory;
        private readonly IEventRelay eventRelay;
        private readonly IUndoRedoManager undoRedoManager;
        private readonly IOperationProgress operationProgress;
        private readonly IPropertyViewModel propertyViewModel;
        private readonly IResourceExplorerViewModel resourceExplorerViewModel;
        private readonly IMaterialExplorerViewModel materialExplorerViewModel;
        private readonly IFontExplorerViewModel fontExplorerViewModel;

        private readonly List<IDocumentTemplate> documentTemplates;
        private readonly List<IDocumentTemplateCategory> documentTemplateCategories;

        private CarbonDirectory currentProject;
        
        private IProjectViewModel projectViewModel;
        private IEditorSettingsViewModel settingsViewModel;

        private IEditorDocument activeDocument;

        private IFolderViewModel currentCreationContext;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public MainViewModel(IEditorLogic logic, IFactory factory)
        {
            this.logic = logic;
            this.factory = factory;
            this.eventRelay = factory.Resolve<IEventRelay>();
            this.undoRedoManager = factory.Resolve<IUndoRedoManager>();
            this.undoRedoManager.PropertyChanged += this.OnUndoRedoManagerChanged;
            this.operationProgress = factory.Resolve<IOperationProgress>();
            this.propertyViewModel = factory.Resolve<IPropertyViewModel>();
            this.resourceExplorerViewModel = factory.Resolve<IResourceExplorerViewModel>();
            this.materialExplorerViewModel = factory.Resolve<IMaterialExplorerViewModel>();
            this.fontExplorerViewModel = factory.Resolve<IFontExplorerViewModel>();
            
            this.documentTemplates = new List<IDocumentTemplate>();
            this.documentTemplateCategories = new List<IDocumentTemplateCategory>();

            this.ToolWindows = new ObservableCollection<IEditorTool>
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
            this.CommandOpenProject = new RelayCommand<CarbonDirectory>(this.OnOpenProject);
            this.CommandUndo = new RelayCommand(this.OnUndo, this.CanUndo);
            this.CommandRedo = new RelayCommand(this.OnRedo, this.CanRedo);
            this.CommandCloseProject = new RelayCommand(this.OnCloseProject, this.CanCloseProject);
            this.CommandSaveProject = new RelayCommand(this.OnSaveProject, this.CanSaveProject);
            this.CommandSaveProjectAs = new RelayCommand(this.OnSaveProjectAs, this.CanSaveProject);
            this.CommandExit = new RelayCommand(this.OnExit);
            this.CommandOpenResourceExplorer = new RelayCommand(this.OnShowResourceExplorer);
            this.CommandOpenMaterialExplorer = new RelayCommand(this.OnShowMaterialExplorer);
            this.CommandOpenFontExplorer = new RelayCommand(this.OnShowFontExplorer);
            this.CommandOpenProperties = new RelayCommand(this.OnShowProperties);
            this.CommandOpenNewDialog = new RelayCommand<IFolderViewModel>(this.OnNewDialog);
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

        public ReadOnlyObservableCollection<CarbonDirectory> RecentProjects
        {
            get
            {
                return this.logic.RecentProjects;
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
                return this.projectViewModel ?? (this.projectViewModel = this.factory.Resolve<IProjectViewModel>());
            }
        }

        public IOperationProgress OperationProgress
        {
            get
            {
                return this.operationProgress;
            }
        }

        public Theme AvalonDockTheme
        {
            get
            {
                return new MetroTheme();
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
            PropertyChangedEventHandler handler = this.ProjectChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs("Project"));
            }
        }

        private void OnNewProject()
        {
            if (this.CanCloseProject())
            {
                this.OnCloseProject();
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
            this.currentProject = null;
            this.NotifyProjectChange();
        }

        private void OnNewResource()
        {
            if (this.currentCreationContext == null)
            {
                throw new InvalidOperationException("New resource can not be created without context");
            }

            // Todo...
            //IResourceViewModel vm = this.logic.AddResource();
            //this.currentCreationContext.AddContent(vm);
        }

        private void OnNewModel()
        {
            throw new NotImplementedException();
        }

        private void OnNewFont()
        {
            this.logic.AddFont();
        }

        private void OnNewStage()
        {
            throw new NotImplementedException();
        }

        private void OnNewMaterial()
        {
            this.logic.AddMaterial();
        }

        private void OnReload()
        {
            this.logic.Reload();
        }

        private bool CanReload()
        {
            return this.logic.IsProjectLoaded;
        }

        private void OnNewDialog(IFolderViewModel context)
        {
            this.currentCreationContext = context;
            var vm = this.factory.Resolve<INewDialogViewModel>();
            var view = new NewDocumentView { DataContext = vm };
            view.ShowDialog();
        }

        private bool CanCreateContent()
        {
            return this.logic.IsProjectLoaded;
        }

        private bool CanCreateResource()
        {
            return this.logic.IsProjectLoaded;
        }

        private void OnUndoRedoManagerChanged(object sender, PropertyChangedEventArgs args)
        {
            this.NotifyUndoRedoChanged();
        }

        private void NotifyProjectChange()
        {
            // ReSharper disable ExplicitCallerInfoArgument
            this.NotifyPropertyChanged("Project");
            this.NotifyPropertyChanged("ProjectTitle");
            // ReSharper restore ExplicitCallerInfoArgument
        }

        private void NotifyUndoRedoChanged()
        {
            // ReSharper disable ExplicitCallerInfoArgument
            this.NotifyPropertyChanged("UndoOperations");
            this.NotifyPropertyChanged("RedoOperations");
            this.NotifyPropertyChanged("CommandUndo");
            this.NotifyPropertyChanged("CommandRedo");
            // ReSharper restore ExplicitCallerInfoArgument
        }

        private void OnRedo()
        {
            this.undoRedoManager.ActiveGroup.Redo();
            this.NotifyUndoRedoChanged();
        }

        private bool CanRedo()
        {
            return this.undoRedoManager.ActiveGroup != null && this.undoRedoManager.ActiveGroup.CanRedo;
        }

        private void OnUndo()
        {
            this.undoRedoManager.ActiveGroup.Undo();
            this.NotifyUndoRedoChanged();
        }

        private bool CanUndo()
        {
            return this.undoRedoManager.ActiveGroup != null && this.undoRedoManager.ActiveGroup.CanUndo;
        }

        private bool CanCloseProject()
        {
            return this.logic.IsProjectLoaded;
        }

        private void OnCloseProject()
        {
            this.SaveProjectLayout();
            this.logic.CloseProject();
            this.currentProject = null;
            this.NotifyProjectChange();
        }

        private void OnOpenProject(CarbonDirectory source)
        {
            // See if we got a path to open
            if (source != null)
            {
                this.DoOpenProject(source);
                return;
            }

            var dialog = new OpenFileDialog
                {
                    CheckFileExists = true,
                    DefaultExt = DefaultProjectFileExtension
                };

            if (dialog.ShowDialog() == true)
            {
                this.DoOpenProject(new CarbonDirectory(dialog.FileName));
            }
        }

        private void DoOpenProject(CarbonDirectory path)
        {
            if (this.CanCloseProject())
                {
                    this.OnCloseProject();
                }

                this.currentProject = path;
                this.logic.OpenProject(this.currentProject);
                this.RestoreProjectLayout();
                this.NotifyProjectChange();
        }
        
        private bool CanSaveProject()
        {
            return this.logic.IsProjectLoaded;
        }

        private void OnSaveProject()
        {
            if (this.currentProject.IsNull)
            {
                this.OnSaveProjectAs();
                return;
            }

            this.logic.SaveProject(this.currentProject);
        }

        private void OnSaveProjectAs()
        {
            // Todo: replace with directory dialog
            var dialog = new OpenFileDialog
                {
                    CheckPathExists = true,
                    CheckFileExists = false,
                    DefaultExt = DefaultProjectFileExtension
                };

            if (dialog.ShowDialog() == true)
            {
                this.currentProject = new CarbonFile(dialog.FileName).GetDirectory();
                this.logic.SaveProject(this.currentProject);
            }
        }

        private void OnExit()
        {
            Application.Current.Shutdown(0);
        }
        
        private void OnShowResourceExplorer()
        {
            this.resourceExplorerViewModel.IsVisible = true;
            this.resourceExplorerViewModel.IsActive = true;
        }

        private void OnShowMaterialExplorer()
        {
            this.materialExplorerViewModel.IsVisible = true;
            this.materialExplorerViewModel.IsActive = true;
        }

        private void OnShowFontExplorer()
        {
            this.fontExplorerViewModel.IsVisible = true;
            this.fontExplorerViewModel.IsActive = true;
        }

        private void OnShowProperties()
        {
            this.propertyViewModel.IsVisible = true;
            this.propertyViewModel.IsActive = true;
        }

        private void OnShowSettings()
        {
            if (this.settingsViewModel == null)
            {
                this.settingsViewModel = this.factory.Resolve<IEditorSettingsViewModel>();
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
        
        private void RestoreProjectLayout()
        {
            if (this.currentProject.IsNull)
            {
                return;
            }

            CarbonFile file = this.currentProject.ToFile(DefaultProjectLayoutFile);
            if (!file.Exists)
            {
                return;
            }

            this.eventRelay.Relay(new EventLoadLayout(this, file));
        }

        private void SaveProjectLayout()
        {
            if (this.currentProject == null || this.currentProject.IsNull)
            {
                return;
            }

            CarbonFile file = this.currentProject.ToFile(DefaultProjectLayoutFile);
            this.eventRelay.Relay(new EventSaveLayout(file));
        }

        private void OnMainWindowClosing(EventWindowClosing obj)
        {
            this.SaveProjectLayout();
        }
    }
}
