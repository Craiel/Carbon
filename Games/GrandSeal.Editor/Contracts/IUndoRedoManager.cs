namespace GrandSeal.Editor.Contracts
{
    using System;

    using CarbonCore.ToolFramework.Contracts.ViewModels;

    public interface IUndoRedoManager : IBaseViewModel
    {
        IUndoRedoGroup ActiveGroup { get; }

        void RegisterGroup(object target);
        void ActivateGroup(object target);
        void ReleaseGroup(object target);

        void AddOperation(Action action, string name = "unknown");
    }
}
