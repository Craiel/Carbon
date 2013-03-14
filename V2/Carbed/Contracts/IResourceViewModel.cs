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
    public interface IResourceViewModel : ICarbedDocument
    {
        int? Id { get; }

        string Hash { get; }

        ResourceType Type { get; set; }

        long? SourceSize { get; }
        long? TargetSize { get; }

        bool CanChangeType { get; }

        bool IsValidSource { get; }
        bool IsHavingSourceElements { get; }

        bool ForceExport { get; set; }

        string SourcePath { get; }

        DateTime? LastChangeDate { get; }

        IFolderViewModel Parent { get; set; }

        ImageSource PreviewImage { get; }

        // Texture Options
        bool IsNormalMap { get; set; }
        bool ConvertToNormalMap { get; set; }
        bool CompressTexture { get; set; }
        TextureTargetFormat TextureTargetFormat { get; set; }

        // Model Options
        ReadOnlyCollection<string> SourceElements { get; }
        string SelectedSourceElement { get; set; }

        ITextureSynchronizer TextureSynchronizer { get; }

        bool AutoUpdateTextures { get; set; }
        IFolderViewModel TextureFolder { get; set; }

        // Script Options
        ITextSource ScriptDocument { get; }

        // Functions and Commands
        ICommand CommandSelectFile { get; }
        ICommand CommandSelectTextureFolder { get; }

        void Save(IContentManager target, IResourceManager resourceTarget);
        void Delete(IContentManager target, IResourceManager resourceTarget);

        void SelectFile(string path);
        void CheckSource();

        void UpdateAutoCompletion(IList<ICompletionData> completionList, string context = null);
    }
}
