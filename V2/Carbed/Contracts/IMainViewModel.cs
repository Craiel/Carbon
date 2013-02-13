using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

using Carbed.Logic;

namespace Carbed.Contracts
{
    public interface IMainViewModel : ICarbedBase
    {
        event PropertyChangedEventHandler ProjectChanged;

        string ProjectTitle { get; }

        DateTime DateTime { get; }

        ObservableCollection<ICarbedTool> ToolWindows { get; }
        ObservableCollection<ICarbedDocument> Documents { get; }
        ReadOnlyCollection<IDocumentTemplate> DocumentTemplates { get; }
        ReadOnlyCollection<IDocumentTemplateCategory> DocumentTemplateCategories { get; }

        ReadOnlyCollection<UndoRedoOperation> UndoOperations { get; }
        ReadOnlyCollection<UndoRedoOperation> RedoOperations { get; }
        
        ICarbedDocument ActiveDocument { get; set; }

        IProjectViewModel Project { get; }

        IOperationProgress OperationProgress { get; }

        ICommand CommandNewProject { get; }

        ICommand CommandOpenProject { get; }
        ICommand CommandCloseProject { get; }
        ICommand CommandSaveProject { get; }

        ICommand CommandBuild { get; }

        ICommand CommandUndo { get; }
        ICommand CommandRedo { get; }

        ICommand CommandExit { get; }

        ICommand CommandOpenResourceExplorer { get; }
        ICommand CommandOpenContentExplorer { get; }
        ICommand CommandOpenProperties { get; }
        ICommand CommandOpenNewDialog { get; }

        void OpenDocument(ICarbedDocument document);
        void CloseDocument(ICarbedDocument document);
    }
}
