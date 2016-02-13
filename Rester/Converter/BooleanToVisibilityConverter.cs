using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Rester.Converter
{
    internal class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool invert = parameter as string == "invert";
            bool result = value is bool && (bool)value;
            if (invert)
                result = !result;
            return result ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    internal class DataRestMethodToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var typedValue = value as string;
            bool result = typedValue != null && IsDataMethod(typedValue);
            return result ? Visibility.Visible : Visibility.Collapsed;
        }

        private bool IsDataMethod(string value)
        {
            value = value.ToLower();
            return value == "post" || value == "put";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
