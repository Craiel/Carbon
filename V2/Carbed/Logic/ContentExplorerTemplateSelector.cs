using System.Windows;
using System.Windows.Controls;

namespace Carbed.Logic
{
    public class ContentExplorerTemplateSelector : DataTemplateSelector
    {
        public DataTemplate GenericDocumentTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            // Todo: Fill in here

            return GenericDocumentTemplate;
        }
    }
}
