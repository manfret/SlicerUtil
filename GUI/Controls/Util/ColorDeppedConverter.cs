using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace GUI.Controls.Util
{
    [ValueConversion(typeof(SolidColorBrush), typeof(SolidColorBrush))]
    public class ColorDeppedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var colorBrush = (SolidColorBrush)value;
            double factor = 1;
            if (parameter != null) factor = (double) parameter;

            if (colorBrush == null) return null;

            var newR = colorBrush.Color.R / factor;
            var newG = colorBrush.Color.G / factor;
            var newB = colorBrush.Color.B / factor;

            var copyColorBrush = new SolidColorBrush(new Color { R = (byte)newR, G = (byte)newG, B = (byte)newB, A = colorBrush.Color.A});

            return copyColorBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new Exception("Not implemented");
        }
    }


}