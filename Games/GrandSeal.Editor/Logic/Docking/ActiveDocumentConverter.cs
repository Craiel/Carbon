using System;
using System.Windows.Data;

using GrandSeal.Editor.Contracts;

namespace GrandSeal.Editor.Logic.Docking
{
    public class ActiveDocumentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is IEditorDocument)
                return value;

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is IEditorDocument)
                return value;

            return Binding.DoNothing;
        }
    }
}
