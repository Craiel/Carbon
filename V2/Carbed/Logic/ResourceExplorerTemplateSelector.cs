using System.Windows;
using System.Windows.Controls;

using Carbed.Contracts;

namespace Carbed.Logic
{
    public class ResourceExplorerTemplateSelector : DataTemplateSelector
    {
        public DataTemplate FolderTemplate { get; set; }
        public DataTemplate GenericDocumentTemplate { get; set; }

        public DataTemplate ModelTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is IFolderViewModel) return FolderTemplate;
            if (item is IResourceViewModel) return ModelTemplate;

            return GenericDocumentTemplate;
        }
    }
}
