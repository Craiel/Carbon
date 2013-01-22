using System.Windows.Controls;
using System.Windows;

using Carbed.Contracts;

namespace Carbed.Logic.Docking
{
    class PanesStyleSelector : StyleSelector
    {
        public Style ToolStyle
        {
            get;
            set;
        }

        public Style DocumentStyle
        {
            get;
            set;
        }
        
        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is ICarbedTool)
                return ToolStyle;

            if (item is ICarbedDocument)
                return DocumentStyle;

            return base.SelectStyle(item, container);
        }
    }
}
