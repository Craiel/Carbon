namespace GrandSeal.Editor.Contracts
{
    using System;
    using System.Windows.Input;

    using CarbonCore.ToolFramework.Contracts.ViewModels;

    public interface IEditorDocument : IBaseViewModel
    {
        string Title { get; }
        string Name { get; set; }
        string ContentId { get; }

        bool IsNamed { get; }
        bool IsChanged { get; }
        
        Uri IconUri { get; }

        ICommand CommandOpen { get; }
        ICommand CommandSave { get; }
        ICommand CommandClose { get; }
        ICommand CommandDelete { get; }
        ICommand CommandRefresh { get; }

        void Load();
    }
}
