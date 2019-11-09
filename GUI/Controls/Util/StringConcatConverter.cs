using System;
using System.Globalization;
using System.Windows.Data;

namespace GUI.Controls.Util
{
    public class StringConcatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;
            if (parameter != null)
            {
                return value.ToString() + parameter.ToString();
            }
            else
            {
                return value.ToString();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}