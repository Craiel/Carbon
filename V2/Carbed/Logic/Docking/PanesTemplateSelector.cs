using System.Windows.Controls;
using System.Windows;

using Carbed.Contracts;

namespace Carbed.Logic.Docking
{
    class PanesTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ModelTemplate { get; set; }
        public DataTemplate FontTemplate { get; set; }
        public DataTemplate ResourceExplorerTemplate { get; set; }
        public DataTemplate ContentExplorerTemplate { get; set; }
        public DataTemplate PropertiesTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is IModelViewModel) return ModelTemplate;
            if (item is IFontViewModel) return FontTemplate;
            if (item is IResourceExplorerViewModel) return ResourceExplorerTemplate;
            if (item is IContentExplorerViewModel) return ContentExplorerTemplate;
            if (item is IPropertyViewModel) return PropertiesTemplate;
            
            // Can add more templates here if needed
            return base.SelectTemplate(item, container);
        }
    }
}
