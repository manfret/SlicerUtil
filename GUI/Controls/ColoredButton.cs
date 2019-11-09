using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Aura.Controls
{
    public class ColoredButton : Button
    {
        #region MAIN COLOR BRUSH

        public static readonly DependencyProperty MainColorBrushProperty = DependencyProperty.Register(
            "MainColorBrush", typeof(Brush), typeof(ColoredButton),
            new PropertyMetadata(new SolidColorBrush(new Color { A = 255, R = 125, G = 125, B = 125 })));

        public Brush MainColorBrush
        {
            get { return (Brush)GetValue(MainColorBrushProperty); }
            set { SetValue(MainColorBrushProperty, value); }
        }

        #endregion

        #region DISABLED COLOR BRUSH

        public static readonly DependencyProperty DisabledColorBrushProperty = DependencyProperty.Register(
            "DisabledColorBrush", typeof(Brush), typeof(ColoredButton),
            new PropertyMetadata(new SolidColorBrush(new Color { A = 255, R = 200, G = 200, B = 200 })));

        public Brush DisabledColorBrush
        {
            get { return (Brush)GetValue(DisabledColorBrushProperty); }
            set { SetValue(DisabledColorBrushProperty, value); }
        }

        #endregion

        #region HIGHLIGHT BRUSH

        public static readonly DependencyProperty HighlightBrushProperty = DependencyProperty.Register(
            "HighlightBrush", typeof(Brush), typeof(ColoredButton),
            new PropertyMetadata(new SolidColorBrush(new Color { A = 255, R = 125, G = 125, B = 125 })));

        public Brush HighlightBrush
        {
            get { return (Brush)GetValue(HighlightBrushProperty); }
            set { SetValue(HighlightBrushProperty, value); }
        }

        #endregion

        #region FOREGROUND BRUSH

        public static readonly DependencyProperty ForegroundBrushProperty = DependencyProperty.Register(
            "ForegroundBrush", typeof(Brush), typeof(ColoredButton),
            new PropertyMetadata(new SolidColorBrush(new Color { A = 255, R = 0, G = 0, B = 0 })));

        public Brush ForegroundBrush
        {
            get { return (Brush)GetValue(ForegroundBrushProperty); }
            set { SetValue(ForegroundBrushProperty, value); }
        }

        #endregion

        #region MAIN TEXT

        public static readonly DependencyProperty MainTextProperty = DependencyProperty.Register(
            "MainText", typeof(string), typeof(ColoredButton));

        public string MainText
        {
            get { return (string)GetValue(MainTextProperty); }
            set { SetValue(MainTextProperty, value); }
        }

        #endregion
    }
}