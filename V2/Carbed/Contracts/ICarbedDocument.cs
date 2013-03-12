using System;
using System.Windows.Input;

namespace Carbed.Contracts
{
    public interface ICarbedDocument : ICarbedBase
    {
        string Title { get; }
        string Name { get; set; }

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
