using System.Windows;
using System.Windows.Controls;

using GrandSeal.Editor.Contracts;

namespace GrandSeal.Editor.Logic
{
    class ResourceExplorerTemplateSelector : DataTemplateSelector
    {
        public DataTemplate FolderTemplate { get; set; }
        public DataTemplate ResourceTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is IFolderViewModel) return FolderTemplate;
            if (item is IResourceViewModel) return ResourceTemplate;

            // Can add more templates here if needed
            return base.SelectTemplate(item, container);
        }
    }
}
