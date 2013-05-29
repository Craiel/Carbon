using System;

namespace GrandSeal.Editor.Contracts
{
    public interface IUndoRedoManager : IEditorBase
    {
        IUndoRedoGroup ActiveGroup { get; }

        void RegisterGroup(object target);
        void ActivateGroup(object target);
        void ReleaseGroup(object target);

        void AddOperation(Action action, string name = "unknown");
    }
}
