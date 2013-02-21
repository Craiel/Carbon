using System;
using System.Windows.Input;

namespace Carbed.Contracts
{
    public interface ICarbedDocument : ICarbedBase
    {
        string Name { get; set; }

        bool IsChanged { get; }
        
        Uri IconUri { get; }

        ICommand CommandOpen { get; }
        ICommand CommandSave { get; }
        ICommand CommandClose { get; }
        ICommand CommandDelete { get; }

        void Load();
    }
}
