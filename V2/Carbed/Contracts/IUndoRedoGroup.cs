using System.Collections.ObjectModel;

using Carbed.Logic;

namespace Carbed.Contracts
{
    public interface IUndoRedoGroup : ICarbedBase
    {
        bool CanUndo { get; }
        bool CanRedo { get; }

        ReadOnlyCollection<UndoRedoOperation> UndoOperations { get; }
        ReadOnlyCollection<UndoRedoOperation> RedoOperations { get; }

        void Undo();
        void Redo();

        void AddOperation(UndoRedoOperation operation);
    }
}
