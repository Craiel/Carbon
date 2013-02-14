using System;

using Carbed.Contracts;
using Carbed.Logic;

namespace Carbed
{
    public static class StaticResources
    {
        public static readonly Uri IconUri = new Uri("pack://application:,,,/Carbed;component/Resources/icon.png", UriKind.Absolute);
        public static readonly Uri NewIconUri = new Uri("pack://application:,,,/Carbed;component/Resources/icon_new.png", UriKind.Absolute);
        public static readonly Uri OpenIconUri = new Uri("pack://application:,,,/Carbed;component/Resources/icon_open.png", UriKind.Absolute);
        public static readonly Uri CloseIconUri = new Uri("pack://application:,,,/Carbed;component/Resources/icon_close.png", UriKind.Absolute);
        public static readonly Uri SaveIconUri = new Uri("pack://application:,,,/Carbed;component/Resources/icon_save.png", UriKind.Absolute);
        public static readonly Uri DeleteIconUri = new Uri("pack://application:,,,/Carbed;component/Resources/icon_delete.png", UriKind.Absolute);
        public static readonly Uri ExitIconUri = new Uri("pack://application:,,,/Carbed;component/Resources/icon_exit.png", UriKind.Absolute);
        public static readonly Uri ToolIconUri = new Uri("pack://application:,,,/Carbed;component/Resources/icon_tool.png", UriKind.Absolute);
        public static readonly Uri UndoIconUri = new Uri("pack://application:,,,/Carbed;component/Resources/icon_undo.png", UriKind.Absolute);
        public static readonly Uri RedoIconUri = new Uri("pack://application:,,,/Carbed;component/Resources/icon_redo.png", UriKind.Absolute);
        public static readonly Uri ProjectIconUri = new Uri("pack://application:,,,/Carbed;component/Resources/icon_project.png", UriKind.Absolute);
        public static readonly Uri FolderIconUri = new Uri("pack://application:,,,/Carbed;component/Resources/icon_folder.png", UriKind.Absolute);
        public static readonly Uri ReloadIconUri = new Uri("pack://application:,,,/Carbed;component/Resources/icon_placeholder.png", UriKind.Absolute);
        public static readonly Uri NewFolderIconUri = new Uri("pack://application:,,,/Carbed;component/Resources/icon_newFolder.png", UriKind.Absolute);
        public static readonly Uri NewResourceIconUri = new Uri("pack://application:,,,/Carbed;component/Resources/icon_newDocument.png", UriKind.Absolute);
        public static readonly Uri NewContentIconUri = new Uri("pack://application:,,,/Carbed;component/Resources/icon_placeholder.png", UriKind.Absolute);

        public static readonly Uri ResourceIconUri = new Uri("pack://application:,,,/Carbed;component/Resources/icon_placeholder.png", UriKind.Absolute);
        public static readonly Uri ResourceFontIconUri = new Uri("pack://application:,,,/Carbed;component/Resources/resourceicon_texturefont.png", UriKind.Absolute);
        public static readonly Uri ResourceModelIconUri = new Uri("pack://application:,,,/Carbed;component/Resources/resourceicon_model.png", UriKind.Absolute);
        public static readonly Uri ResourcePlayfieldIconUri = new Uri("pack://application:,,,/Carbed;component/Resources/resourceicon_playfield.png", UriKind.Absolute);

        public static IDocumentTemplate ProjectTemplate = new DocumentTemplate
            {
                TemplateName = "Project",
                Description = "Carbon Project",
                IconUri = ProjectIconUri
            };

        public static IDocumentTemplate ResourceTemplate = new DocumentTemplate
            {
                TemplateName = "Resource",
                Description = "Resource for use in the project",
                IconUri = ReloadIconUri
            };

        public static IDocumentTemplate ModelTemplate = new DocumentTemplate
        {
            TemplateName = "Model",
            Description = "Model for use in the project",
            IconUri = ResourceModelIconUri
        };

        public static IDocumentTemplate FontTemplate = new DocumentTemplate
            {
                TemplateName = "Font",
                Description = "Font for use in UI",
                IconUri = ResourceFontIconUri
            };

        public static IDocumentTemplate PlayfieldTemplate = new DocumentTemplate
        {
            TemplateName = "Playfield",
            Description = "Playfield",
            IconUri = ResourcePlayfieldIconUri
        };
    }
}
