namespace GrandSeal.Editor.Contracts
{
    using System;

    using CarbonCore.ToolFramework.Contracts;

    public interface IEditorTool : IBaseViewModel
    {
        string Title { get; }
        string ContentId { get; }

        Uri IconUri { get; }

        bool IsVisible { get; set; }
        bool IsSelected { get; set; }
        bool IsActive { get; set; }
    }
}
