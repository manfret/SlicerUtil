using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Aura.Controls.Resources;

namespace Aura.Controls.Primitive
{
    public class ButtonWithVisual : ButtonBase
    {
        private static readonly TileBrush img = new ImageBrush(Imaging.CreateBitmapSourceFromHBitmap(Images.transparent.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()));


        public static readonly DependencyProperty NormalBrushProperty =
            DependencyProperty.Register("NormalBrush", typeof(TileBrush), typeof(ButtonWithVisual), new PropertyMetadata(img));


        public TileBrush NormalBrush
        {
            get { return (TileBrush) GetValue(NormalBrushProperty); }
            set { SetValue(NormalBrushProperty, value);}
        }

        public static readonly DependencyProperty MouseOverBrushProperty =
            DependencyProperty.Register("MouseOverBrush", typeof(TileBrush), typeof(ButtonWithVisual), new PropertyMetadata(img));

        public TileBrush MouseOverBrush
        {
            get { return (TileBrush)GetValue(MouseOverBrushProperty); }
            set { SetValue(MouseOverBrushProperty, value); }
        }

        public static readonly DependencyProperty DisabledBrushProperty = DependencyProperty.Register("DisabledBrush", typeof(TileBrush) , typeof(ButtonWithVisual));

        public TileBrush DisabledBrush
        {
            get { return (TileBrush) GetValue(DisabledBrushProperty); }
            set { SetValue(DisabledBrushProperty, value); }
        }

        public static readonly DependencyProperty VisualWidthProperty =
            DependencyProperty.Register("VisualWidth", typeof(double), typeof(ButtonWithVisual), new PropertyMetadata(double.NaN));

        public double VisualWidth
        {
            get { return (double)GetValue(VisualWidthProperty); }
            set { SetValue(VisualWidthProperty, value); }
        }

        public static readonly DependencyProperty VisualHeightProperty =
            DependencyProperty.Register("VisualHeight", typeof(double), typeof(ButtonWithVisual), new PropertyMetadata(double.NaN));

        public double VisualHeight
        { 
            get { return (double)GetValue(VisualHeightProperty); }
            set { SetValue(VisualHeightProperty, value); }
        }

        public static readonly DependencyProperty ImgBorderMarginProperty =
            DependencyProperty.Register("ImgBorderMargin", typeof(Thickness), typeof(ButtonWithVisual));

        public Thickness ImgBorderMargin
        {
            get { return (Thickness) GetValue(ImgBorderMarginProperty); }
            set { SetValue(ImgBorderMarginProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (DisabledBrush == null) DisabledBrush = NormalBrush.Clone();
        }
    }
}