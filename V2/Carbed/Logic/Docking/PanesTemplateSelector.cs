using System.Windows.Controls;
using System.Windows;

using Carbed.Contracts;

using Carbon.Engine.Resource.Content;

namespace Carbed.Logic.Docking
{
    class PanesTemplateSelector : DataTemplateSelector
    {
        // Resource templates
        public DataTemplate ResourceTemplate { get; set; }
        public DataTemplate FontTemplate { get; set; }
        public DataTemplate ScriptTemplate { get; set; }

        // Tool window templates
        public DataTemplate ResourceExplorerTemplate { get; set; }
        public DataTemplate MaterialExplorerTemplate { get; set; }
        public DataTemplate FontExplorerTemplate { get; set; }
        public DataTemplate PropertiesTemplate { get; set; }

        // Misc
        public DataTemplate CarbedSettingsTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is IResourceExplorerViewModel) return ResourceExplorerTemplate;
            if (item is IMaterialExplorerViewModel) return MaterialExplorerTemplate;
            if (item is IFontExplorerViewModel) return FontExplorerTemplate;
            if (item is IPropertyViewModel) return PropertiesTemplate;
            if (item is ICarbedSettingsViewModel) return CarbedSettingsTemplate;

            if (item is IResourceViewModel)
            {
                switch (((IResourceViewModel)item).Type)
                {
                    case ResourceType.Font:
                        {
                            return this.FontTemplate;
                        }

                    case ResourceType.Script:
                        {
                            return this.ScriptTemplate;
                        }

                    default:
                        {
                            return this.ResourceTemplate;
                        }
                }
            }
            
            // Can add more templates here if needed
            return base.SelectTemplate(item, container);
        }
    }
}
