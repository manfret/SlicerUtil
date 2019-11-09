using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;

namespace Aura.Controls.Primitive
{
    public class Size3DPanel : UserControl
    {
        #region VALUE

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(Point3D), typeof(Size3DPanel), new PropertyMetadata(new Point3D(), ValueChangedCallback));

        private static void ValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Size3DPanel)d).OnValueChangedCallback(d, e);
        }

        private void OnValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var value = (Point3D)e.NewValue;
            switch (BoxType)
            {
                case BoxType.INTEGERS:
                    SetValue(XProperty,Math.Round(value.X).ToString());
                    SetValue(YProperty,Math.Round(value.Y).ToString());
                    SetValue(ZProperty,Math.Round(value.Z).ToString());
                    break;
                case BoxType.DOUBLES:
                    SetValue(XProperty, Math.Round(value.X, 3).ToString());
                    SetValue(YProperty, Math.Round(value.Y, 3).ToString());
                    SetValue(ZProperty, Math.Round(value.Z, 3).ToString());
                    break;
            }
        }

        public Point3D Value
        {
            get { return (Point3D) GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        #endregion

        #region BOX TYPE

        public static readonly DependencyProperty BoxTypeProperty =
            DependencyProperty.Register("BoxType", typeof(BoxType), typeof(Size3DPanel),
                new PropertyMetadata(BoxType.DOUBLES));

        public BoxType BoxType
        {
            get { return (BoxType) GetValue(BoxTypeProperty); }
            set { SetValue(BoxTypeProperty, value); }
        }

        #endregion

        #region X

        public static readonly DependencyProperty XProperty =
            DependencyProperty.Register("X", typeof(string), typeof(Size3DPanel), new PropertyMetadata(string.Empty, XChangedCallback));

        private static void XChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Size3DPanel)d).OnXChangedCallback(d, e);
        }

        private void OnXChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            double.TryParse(e.NewValue.ToString(), out double newX);
            var newValue = new Point3D(newX, Value.Y, Value.Z);
            SetValue(ValueProperty, newValue);
        }

        public string X
        {
            get { return (string) GetValue(XProperty); }
            set { SetValue(XProperty, value); }
        }

        #endregion

        #region Y

        public static readonly DependencyProperty YProperty =
            DependencyProperty.Register("Y", typeof(string), typeof(Size3DPanel), new PropertyMetadata(string.Empty, YChangedCallback));

        private static void YChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Size3DPanel)d).OnYChangedCallback(d, e);
        }

        private void OnYChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            double.TryParse(e.NewValue.ToString(), out double newY);
            var newValue = new Point3D(Value.X, newY, Value.Z);
            SetValue(ValueProperty, newValue);
        }

        public string Y
        {
            get { return (string) GetValue(YProperty); }
            set { SetValue(YProperty, value); }
        }

        #endregion

        #region Z

        public static readonly DependencyProperty ZProperty =
            DependencyProperty.Register("Z", typeof(string), typeof(Size3DPanel), new PropertyMetadata(string.Empty, ZChangedCallback));

        private static void ZChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Size3DPanel)d).OnZChangedCallback(d, e);
        }

        private void OnZChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            double.TryParse(e.NewValue.ToString(), out double newZ);
            var newValue = new Point3D(Value.X, Value.Y, newZ);
            SetValue(ValueProperty, newValue);
        }

        public string Z
        {
            get { return (string) GetValue(ZProperty); }
            set { SetValue(ZProperty, value); }
        }

        #endregion

        #region IS READ ONLY

        public static DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(
                "IsReadOnly",
                typeof(bool),
                typeof(Size3DPanel),
                new PropertyMetadata(false)
            );

        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        #endregion
    }
}