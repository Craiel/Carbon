using System.Windows.Controls;
using System.Windows;

using Carbed.Contracts;

namespace Carbed.Logic.Docking
{
    class PanesTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ResourceTemplate { get; set; }
        public DataTemplate FontTemplate { get; set; }
        public DataTemplate ResourceExplorerTemplate { get; set; }
        public DataTemplate MaterialExplorerTemplate { get; set; }
        public DataTemplate PropertiesTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is IFontViewModel) return FontTemplate;
            if (item is IResourceExplorerViewModel) return ResourceExplorerTemplate;
            if (item is IMaterialExplorerViewModel) return MaterialExplorerTemplate;
            if (item is IPropertyViewModel) return PropertiesTemplate;

            if (item is IResourceViewModel) return ResourceTemplate;
            
            // Can add more templates here if needed
            return base.SelectTemplate(item, container);
        }
    }
}
