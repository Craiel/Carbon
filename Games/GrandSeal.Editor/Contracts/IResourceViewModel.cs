using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;

using Core.Engine.Contracts.Resource;
using Core.Engine.Resource.Content;

using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;

namespace GrandSeal.Editor.Contracts
{
    using System.Drawing;

    using Core.Editor.Processors;

    public interface IResourceRawViewModel : IResourceViewModel
    {
    }

    public interface IResourceFontViewModel : IResourceViewModel
    {
        FontStyle FontStyle { get; set; }

        int FontSize { get; set; }
        int FontCharactersPerRow { get; set; }
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

    public interface IResourceStageViewModel : IResourceViewModel
    {
    }

    public interface IResourceScriptViewModel : IResourceViewModel
    {
        ITextSource ScriptDocument { get; }

        void UpdateAutoCompletion(IList<ICompletionData> completionList, string context = null);
    }

    public interface IResourceViewModel : IEditorDocument
    {
        int? Id { get; }

        string Hash { get; }

        ResourceType Type { get; }

        long? SourceSize { get; }
        long? TargetSize { get; }

        bool IsValidSource { get; }
        
        bool ForceSave { get; set; }

        string SourcePath { get; }

        DateTime? LastChangeDate { get; }

        IFolderViewModel Parent { get; set; }

        ImageSource PreviewImage { get; }
        
        // Functions and Commands
        ICommand CommandSelectFile { get; }
        
        void Save(IContentManager target, IResourceManager resourceTarget, bool force);
        void Delete(IContentManager target, IResourceManager resourceTarget);

        void SelectFile(string path);
        void CheckSource();
    }
}
