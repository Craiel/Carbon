using System.Windows.Controls;
using System.Windows;

using GrandSeal.Editor.Contracts;

namespace GrandSeal.Editor.Logic.Docking
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
            if (item is IEditorTool)
                return ToolStyle;

            if (item is IEditorDocument)
                return DocumentStyle;

            return base.SelectStyle(item, container);
        }
    }
}
