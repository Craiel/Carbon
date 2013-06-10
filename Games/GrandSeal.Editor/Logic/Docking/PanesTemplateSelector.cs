using System.Windows;
using System.Windows.Controls;

using Core.Engine.Resource.Content;
using GrandSeal.Editor.Contracts;

namespace GrandSeal.Editor.Logic.Docking
{
    public class PanesTemplateSelector : DataTemplateSelector
    {
        // Resource templates
        public DataTemplate ResourceTemplate { get; set; }
        public DataTemplate FontTemplate { get; set; }
        public DataTemplate ScriptTemplate { get; set; }
        public DataTemplate UserInterfaceTemplate { get; set; }

        // Tool window templates
        public DataTemplate ResourceExplorerTemplate { get; set; }
        public DataTemplate MaterialExplorerTemplate { get; set; }
        public DataTemplate FontExplorerTemplate { get; set; }
        public DataTemplate PropertiesTemplate { get; set; }

        // Misc
        public DataTemplate EditorSettingsTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is IResourceExplorerViewModel)
            {
                return this.ResourceExplorerTemplate;
            }

            if (item is IMaterialExplorerViewModel)
            {
                return this.MaterialExplorerTemplate;
            }

            if (item is IFontExplorerViewModel)
            {
                return this.FontExplorerTemplate;
            }

            if (item is IPropertyViewModel)
            {
                return this.PropertiesTemplate;
            }

            if (item is IEditorSettingsViewModel)
            {
                return this.EditorSettingsTemplate;
            }

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

                    case ResourceType.UserInterface:
                        {
                            return this.UserInterfaceTemplate;
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
