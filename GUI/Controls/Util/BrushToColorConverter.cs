using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace GUI.Controls.Util
{
    [ValueConversion(typeof(SolidColorBrush), typeof(Color))]
    public class BrushToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var colorBrush = value as SolidColorBrush;

            if (colorBrush == null) return new Color();

            var color = new Color
            {
                A = colorBrush.Color.A,
                R = colorBrush.Color.R,
                G = colorBrush.Color.G,
                B = colorBrush.Color.B
            };

            return color;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = (Color)value;
            var solidColoBrush = new SolidColorBrush(color);
            return solidColoBrush;
        }
    }
}