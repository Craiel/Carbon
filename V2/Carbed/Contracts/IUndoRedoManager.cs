using System;

namespace Carbed.Contracts
{
    public interface IUndoRedoManager : ICarbedBase
    {
        IUndoRedoGroup ActiveGroup { get; }

        void RegisterGroup(object target);
        void ActivateGroup(object target);
        void ReleaseGroup(object target);

        void AddOperation(Action action, string name = "unknown");
    }
}
