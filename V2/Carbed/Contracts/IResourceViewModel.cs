using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;

using Carbon.Editor.Processors;
using Carbon.Engine.Contracts.Resource;
using Carbon.Engine.Resource.Content;

using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;

namespace Carbed.Contracts
{
    public interface IResourceRawViewModel : IResourceViewModel
    {
    }

    public interface IResourceTextureViewModel : IResourceViewModel
    {
        bool IsNormalMap { get; set; }
        bool ConvertToNormalMap { get; set; }
        bool CompressTexture { get; set; }

        TextureTargetFormat TextureTargetFormat { get; set; }
    }

    public interface IResourceModelViewModel : IResourceViewModel
    {
        bool IsHavingSourceElements { get; }

        ReadOnlyCollection<string> SourceElements { get; }
        string SelectedSourceElement { get; set; }

        ITextureSynchronizer TextureSynchronizer { get; }

        bool AutoUpdateTextures { get; set; }
        IFolderViewModel TextureFolder { get; set; }

        ICommand CommandSelectTextureFolder { get; }
    }

    public interface IResourceScriptViewModel : IResourceViewModel
    {
        ITextSource ScriptDocument { get; }

        void UpdateAutoCompletion(IList<ICompletionData> completionList, string context = null);
    }

    public interface IResourceViewModel : ICarbedDocument
    {
        int? Id { get; }

        string Hash { get; }

        ResourceType Type { get; }

        long? SourceSize { get; }
        long? TargetSize { get; }

        bool IsValidSource { get; }
        
        bool ForceExport { get; set; }

        string SourcePath { get; }

        DateTime? LastChangeDate { get; }

        IFolderViewModel Parent { get; set; }

        ImageSource PreviewImage { get; }
        
        // Functions and Commands
        ICommand CommandSelectFile { get; }
        
        void Save(IContentManager target, IResourceManager resourceTarget);
        void Delete(IContentManager target, IResourceManager resourceTarget);

        void SelectFile(string path);
        void CheckSource();
    }
}
