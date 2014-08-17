using System.Collections.ObjectModel;

using GrandSeal.Editor.Logic;

namespace GrandSeal.Editor.Contracts
{
    using CarbonCore.ToolFramework.Contracts;

    public interface IUndoRedoGroup : IBaseViewModel
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
