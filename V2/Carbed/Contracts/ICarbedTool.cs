using System;

namespace Carbed.Contracts
{
    public interface ICarbedTool : ICarbedBase
    {
        string Title { get; }
        string ContentId { get; }

        Uri IconUri { get; }

        bool IsVisible { get; set; }
        bool IsSelected { get; set; }
        bool IsActive { get; set; }
    }
}
