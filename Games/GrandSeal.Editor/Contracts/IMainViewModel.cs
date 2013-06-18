﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

using GrandSeal.Editor.Logic;

namespace GrandSeal.Editor.Contracts
{
    public interface IMainViewModel : IEditorBase
    {
        event PropertyChangedEventHandler ProjectChanged;

        string ProjectTitle { get; }

        DateTime DateTime { get; }

        ObservableCollection<IEditorTool> ToolWindows { get; }
        ObservableCollection<IEditorDocument> Documents { get; }
        ReadOnlyCollection<IDocumentTemplate> DocumentTemplates { get; }
        ReadOnlyCollection<IDocumentTemplateCategory> DocumentTemplateCategories { get; }

        ReadOnlyCollection<UndoRedoOperation> UndoOperations { get; }
        ReadOnlyCollection<UndoRedoOperation> RedoOperations { get; }
        
        IEditorDocument ActiveDocument { get; set; }

        IProjectViewModel Project { get; }

        IOperationProgress OperationProgress { get; }

        ICommand CommandNewProject { get; }
        ICommand CommandNewMaterial { get; }
        ICommand CommandNewStage { get; }
        ICommand CommandNewFont { get; }
        ICommand CommandNewModel { get; }
        ICommand CommandNewResource { get; }

        ICommand CommandOpenProject { get; }
        ICommand CommandCloseProject { get; }
        ICommand CommandSaveProject { get; }

        ICommand CommandBuild { get; }

        ICommand CommandUndo { get; }
        ICommand CommandRedo { get; }

        ICommand CommandExit { get; }

        ICommand CommandOpenResourceExplorer { get; }
        ICommand CommandOpenMaterialExplorer { get; }
        ICommand CommandOpenFontExplorer { get; }
        ICommand CommandOpenProperties { get; }
        ICommand CommandOpenNewDialog { get; }
        ICommand CommandOpenSettings { get; }

        void OpenDocument(IEditorDocument document);
        void CloseDocument(IEditorDocument document);
    }
}