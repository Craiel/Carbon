using System;

namespace GrandSeal.Editor.Contracts
{
    public interface IEditorTool : IEditorBase
    {
        string Title { get; }
        string ContentId { get; }

        Uri IconUri { get; }

        bool IsVisible { get; set; }
        bool IsSelected { get; set; }
        bool IsActive { get; set; }
    }
}
