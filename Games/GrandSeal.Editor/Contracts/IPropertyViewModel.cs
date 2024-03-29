﻿using System.Windows.Controls;

namespace GrandSeal.Editor.Contracts
{
    using CarbonCore.ToolFramework.Contracts.ViewModels;

    public interface IPropertyViewModel : IEditorTool
    {
        IBaseViewModel ActiveContext { get; set; }

        Control PropertyControl { get; }

        void SetActivation(IBaseViewModel source, bool active);
    }
}
