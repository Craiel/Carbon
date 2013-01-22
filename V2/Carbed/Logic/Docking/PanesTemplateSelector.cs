using System.Windows.Controls;
using System.Windows;

using Carbed.Contracts;

namespace Carbed.Logic.Docking
{
    class PanesTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ModelTemplate { get; set; }
        public DataTemplate TextureFontTemplate { get; set; }
        public DataTemplate ProjectExplorerTemplate { get; set; }
        public DataTemplate PropertiesTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is IModelViewModel) return ModelTemplate;
            if (item is ITextureFontViewModel) return TextureFontTemplate;
            if (item is IProjectExplorerViewModel) return ProjectExplorerTemplate;
            if (item is IPropertyViewModel) return PropertiesTemplate;
            
            // Can add more templates here if needed
            return base.SelectTemplate(item, container);
        }
    }
}
