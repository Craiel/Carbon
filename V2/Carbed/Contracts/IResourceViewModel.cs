using System;
using System.Windows.Input;

using Carbon.Engine.Contracts.Resource;
using Carbon.Engine.Resource.Content;

namespace Carbed.Contracts
{
    public interface IResourceViewModel : ICarbedDocument
    {
        ResourceType Type { get; set; }

        bool IsValidSource { get; }
        string SourcePath { get; }
        DateTime? LastChangeDate { get; }

        IFolderViewModel Parent { get; set; }

        ICommand CommandSelectFile { get; }

        void Save(IContentManager target, IResourceManager resourceTarget);
        void Delete(IContentManager target, IResourceManager resourceTarget);
    }
}
