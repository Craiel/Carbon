﻿using System.Windows.Input;

namespace Carbed.Contracts
{
    public interface IResourceViewModel : ICarbedDocument
    {
        bool IsExpanded { get; set; }

        string FileName { get; }

        IFolderViewModel Parent { get; set; }

        ICommand CommandSelectFile { get; }
    }
}
