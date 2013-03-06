using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

using Carbon.Engine.Contracts.Resource;
using Carbon.Engine.Resource.Content;

namespace Carbed.Contracts
{
    public interface IResourceViewModel : ICarbedDocument
    {
        int? Id { get; }

        ResourceType Type { get; set; }

        long? SourceSize { get; }
        long? TargetSize { get; }

        bool IsValidSource { get; }
        bool IsHavingSourceElements { get; }

        bool ForceExport { get; set; }
        bool AutoUpdateTextures { get; set; }

        string SourcePath { get; }
        
        ReadOnlyCollection<string> SourceElements { get; }
        string SelectedSourceElement { get; set; }

        DateTime? LastChangeDate { get; }

        IFolderViewModel Parent { get; set; }
        IFolderViewModel TextureFolder { get; set; }

        ICommand CommandSelectFile { get; }
        ICommand CommandSelectTextureFolder { get; }

        void Save(IContentManager target, IResourceManager resourceTarget);
        void Delete(IContentManager target, IResourceManager resourceTarget);

        void SelectFile(string path);
        void CheckSource();
    }
}
