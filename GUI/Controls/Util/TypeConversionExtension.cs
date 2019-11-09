using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Aura.Controls.Util
{
/*    public static class TypeConversionExtension
    {
        public static Point3D GetPoint3DFromVertex(this VVertex vVertex)
        {
            if (vVertex == null) return new Point3D(0, 0, 0);
            return new Point3D(vVertex.X, vVertex.Y, vVertex.Z);
        }
    }*/

    [ValueConversion(typeof(SolidColorBrush), typeof(SolidColorBrush))]
    public class SolidColorBrushHigherConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var to = double.Parse($"{parameter}");

            var color = value as SolidColorBrush;
            var newColorBrush = new SolidColorBrush(Color.FromArgb(color.Color.A, (byte)(color.Color.R * to), (byte)(color.Color.G * to), (byte)(color.Color.B * to)));
            return newColorBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SolidColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = (Color)value;
            var newColorBrush = new SolidColorBrush(Color.FromArgb(color.A, (color.R), (color.G), (color.B)));
            return newColorBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(System.Drawing.Color), typeof(System.Windows.Media.Color))]
    public class DrawingColorToMediaColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var valueColor = (System.Drawing.Color) value;
            return Color.FromArgb(255, valueColor.R, valueColor.G, valueColor.B);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var valueColor = (System.Windows.Media.Color)value;
            return Color.FromArgb(255, valueColor.R, valueColor.G, valueColor.B);
        }
    }

    [ValueConversion(typeof(System.Windows.Media.Color), typeof(System.Drawing.Color))]
    public class MediaColorToDrawingColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var valueColor = (System.Windows.Media.Color)value;
            return System.Drawing.Color.FromArgb(255, valueColor.R, valueColor.G, valueColor.B);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var valueColor = (System.Drawing.Color)value;
            return System.Windows.Media.Color.FromArgb(255, valueColor.R, valueColor.G, valueColor.B);
        }
    }
}