using System.Windows;
using System.Windows.Controls;

using Carbed.Contracts;

namespace Carbed.Logic
{
    public class ProjectExplorerTemplateSelector : DataTemplateSelector
    {
        public DataTemplate FolderTemplate { get; set; }
        public DataTemplate GenericDocumentTemplate { get; set; }
        public DataTemplate TextureFontTemplate { get; set; }
        public DataTemplate ModelTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is IProjectFolderViewModel) return FolderTemplate;
            if (item is IFontViewModel) return TextureFontTemplate;
            if (item is IModelViewModel) return ModelTemplate;

            return GenericDocumentTemplate;
        }
    }
}
