using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Rester.Converters
{
    internal class BooleanToGreenOrRedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool result = value is bool && (bool)value;
            return new SolidColorBrush(result ? Colors.Green : Colors.Red);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}