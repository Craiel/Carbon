﻿using System;

using GrandSeal.Editor.Contracts;
using GrandSeal.Editor.Logic;

namespace GrandSeal.Editor
{
    public static class StaticResources
    {
        public static readonly Uri IconUri = new Uri("pack://application:,,,/GrandSeal.Editor;component/Resources/icon.png", UriKind.Absolute);
        public static readonly Uri NewIconUri = new Uri("pack://application:,,,/GrandSeal.Editor;component/Resources/icon_new.png", UriKind.Absolute);
        public static readonly Uri OpenIconUri = new Uri("pack://application:,,,/GrandSeal.Editor;component/Resources/icon_open.png", UriKind.Absolute);
        public static readonly Uri CloseIconUri = new Uri("pack://application:,,,/GrandSeal.Editor;component/Resources/icon_close.png", UriKind.Absolute);
        public static readonly Uri ClearIconUri = new Uri("pack://application:,,,/GrandSeal.Editor;component/Resources/icon_clear.png", UriKind.Absolute);
        public static readonly Uri SaveIconUri = new Uri("pack://application:,,,/GrandSeal.Editor;component/Resources/icon_save.png", UriKind.Absolute);
        public static readonly Uri DeleteIconUri = new Uri("pack://application:,,,/GrandSeal.Editor;component/Resources/icon_delete.png", UriKind.Absolute);
        public static readonly Uri RefreshIconUri = new Uri("pack://application:,,,/GrandSeal.Editor;component/Resources/icon_placeholder.png", UriKind.Absolute);
        public static readonly Uri ExitIconUri = new Uri("pack://application:,,,/GrandSeal.Editor;component/Resources/icon_exit.png", UriKind.Absolute);
        public static readonly Uri ToolIconUri = new Uri("pack://application:,,,/GrandSeal.Editor;component/Resources/icon_tool.png", UriKind.Absolute);
        public static readonly Uri UndoIconUri = new Uri("pack://application:,,,/GrandSeal.Editor;component/Resources/icon_undo.png", UriKind.Absolute);
        public static readonly Uri RedoIconUri = new Uri("pack://application:,,,/GrandSeal.Editor;component/Resources/icon_redo.png", UriKind.Absolute);
        public static readonly Uri CopyIconUri = new Uri("pack://application:,,,/GrandSeal.Editor;component/Resources/icon_copy.png", UriKind.Absolute);
        public static readonly Uri PasteIconUri = new Uri("pack://application:,,,/GrandSeal.Editor;component/Resources/icon_paste.png", UriKind.Absolute);
        public static readonly Uri ProjectIconUri = new Uri("pack://application:,,,/GrandSeal.Editor;component/Resources/icon_project.png", UriKind.Absolute);
        public static readonly Uri FolderIconUri = new Uri("pack://application:,,,/GrandSeal.Editor;component/Resources/icon_folder.png", UriKind.Absolute);
        public static readonly Uri ReloadIconUri = new Uri("pack://application:,,,/GrandSeal.Editor;component/Resources/icon_placeholder.png", UriKind.Absolute);
        public static readonly Uri NewFolderIconUri = new Uri("pack://application:,,,/GrandSeal.Editor;component/Resources/icon_newFolder.png", UriKind.Absolute);
        public static readonly Uri NewResourceIconUri = new Uri("pack://application:,,,/GrandSeal.Editor;component/Resources/icon_newDocument.png", UriKind.Absolute);
        public static readonly Uri NewContentIconUri = new Uri("pack://application:,,,/GrandSeal.Editor;component/Resources/icon_placeholder.png", UriKind.Absolute);

        public static readonly Uri NoPreviewImageUri = new Uri("pack://application:,,,/GrandSeal.Editor;component/Resources/no_preview.png", UriKind.Absolute);
        public static readonly Uri PlaceholderIconUri = new Uri("pack://application:,,,/GrandSeal.Editor;component/Resources/icon_placeholder.png", UriKind.Absolute);

        public static readonly Uri ResourceTextureIconUri = new Uri("pack://application:,,,/GrandSeal.Editor;component/Resources/icon_placeholder.png", UriKind.Absolute);
        public static readonly Uri ResourceMeshIconUri = new Uri("pack://application:,,,/GrandSeal.Editor;component/Resources/icon_placeholder.png", UriKind.Absolute);

        public static readonly Uri ContentMaterialIconUri = new Uri("pack://application:,,,/GrandSeal.Editor;component/Resources/icon_placeholder.png", UriKind.Absolute);
        public static readonly Uri ContentFontIconUri = new Uri("pack://application:,,,/GrandSeal.Editor;component/Resources/resourceicon_texturefont.png", UriKind.Absolute);
        public static readonly Uri ContentModelIconUri = new Uri("pack://application:,,,/GrandSeal.Editor;component/Resources/resourceicon_model.png", UriKind.Absolute);
        public static readonly Uri ContentStageIconUri = new Uri("pack://application:,,,/GrandSeal.Editor;component/Resources/resourceicon_playfield.png", UriKind.Absolute);

        public static IDocumentTemplate ProjectTemplate = new DocumentTemplate
            {
                TemplateName = "Project",
                IconUri = ProjectIconUri
            };

        public static IDocumentTemplate TextureTemplate = new DocumentTemplate
            {
                TemplateName = "Texture",
                IconUri = ResourceTextureIconUri
            };

        public static IDocumentTemplate MeshTemplate = new DocumentTemplate
        {
            TemplateName = "Mesh",
            IconUri = ResourceMeshIconUri
        };

        public static IDocumentTemplate FontTemplate = new DocumentTemplate
            {
                TemplateName = "Font",
                IconUri = ContentFontIconUri
            };

        public static IDocumentTemplate ModelTemplate = new DocumentTemplate
        {
            TemplateName = "Model",
            IconUri = ContentModelIconUri
        };

        public static IDocumentTemplate MaterialTemplate = new DocumentTemplate
        {
            TemplateName = "Material",
            IconUri = ContentMaterialIconUri
        };

        public static IDocumentTemplate StageTemplate = new DocumentTemplate
        {
            TemplateName = "Stage",
            IconUri = ContentStageIconUri
        };
    }
}