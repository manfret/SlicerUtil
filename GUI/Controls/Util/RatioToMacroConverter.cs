using System;
using System.Globalization;
using System.Windows.Data;

namespace Aura.Controls.Util
{
    public class RatioToMacroConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double.TryParse(values[0].ToString(), out double height);
            int.TryParse(values[1].ToString(), out int ratio);

            int.TryParse(parameter.ToString(), out int digits);
            if (digits > 0) return Math.Round(height / ratio, digits);
            else return Math.Round(height / ratio, 5);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}