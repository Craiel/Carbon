using System;
using System.Windows.Input;

namespace GrandSeal.Editor.Contracts
{
    public interface IEditorDocument : IEditorBase
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
