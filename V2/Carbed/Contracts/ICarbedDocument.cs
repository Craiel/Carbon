using System;
using System.Windows.Input;

using Carbed.Logic;

namespace Carbed.Contracts
{
    public interface ICarbedDocument : ICarbedBase
    {
        string Name { get; set; }
        string Title { get; }
        
        Uri IconUri { get; }

        ICommand CommandOpen { get; }
        ICommand CommandClose { get; }
        ICommand CommandDelete { get; }
    }
}
