using System;
using System.Globalization;
using System.Windows.Data;

namespace Aura.Controls.Util
{
    [ValueConversion(typeof(object), typeof(string))]
    public class ConcatObjectsAsStrings : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Concat(parameter.ToString(), value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}