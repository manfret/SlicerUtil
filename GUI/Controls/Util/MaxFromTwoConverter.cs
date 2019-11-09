using System;
using System.Globalization;
using System.Windows.Data;

namespace GUI.Controls.Util
{
    public class MaxFromTwoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null && parameter == null) return 0;
            double.TryParse(value.ToString(), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var first);
            if (parameter == null) return first;
            double.TryParse(parameter.ToString(), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var second);
            return Math.Max(first, second);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}