using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Carbed.Logic.MVVM
{
    [ValueConversion(typeof(object), typeof(Type))]
    public class ReferenceToTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                throw new ArgumentException();
            }

            return value.GetType();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(object), typeof(bool))]
    public class NullToBooleanConverter : IValueConverter
    {
        private bool invert;

        public bool Invert
        {
            get
            {
                return this.invert;
            }

            set
            {
                this.invert = value;
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value != null) ^ this.invert;
        }

        public object ConvertBack(
            object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

    [ValueConversion(typeof(Uri), typeof(BitmapImage))]
    public class UriToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var uri = value as Uri;
            if (uri == null)
            {
                throw new ArgumentException();
            }
            
            return new BitmapImage(uri);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(Uri), typeof(BitmapFrame))]
    public class UriToBitmapFrameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var uri = value as Uri;
            if (uri == null)
            {
                throw new ArgumentException();
            }

            return BitmapFrame.Create(uri);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(bool), typeof(Brush))]
    public class BooleanToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.GetType() != typeof(bool) || parameter == null || parameter.GetType() != typeof(Brush[]))
            {
                throw new ArgumentException();
            }

            var colors = (Brush[])parameter;
            return (bool)value ? colors[1] : colors[0];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
