using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Aura.Controls.Util
{
    public class BooleanToVisibilityInvertConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                boolValue = !boolValue;
                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            }
            else return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var visibility = (Visibility) value;
            return visibility == Visibility.Visible;
        }
    }
}