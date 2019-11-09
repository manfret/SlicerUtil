using System;
using System.Globalization;
using System.Windows.Data;

namespace Aura.Controls.Util
{
    public class LayupDataConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var isDefault = (bool)values[2];
            var isDefaultText = (string)values[3];

            if (isDefault) return isDefaultText;
            else
            {
                var fromMM = (double)values[0];
                var toMM = (double)values[1];
                var mmText = (string)parameter;
                return $"{fromMM}{mmText} - {toMM}{mmText}";
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
