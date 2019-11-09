using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Aura.Controls.ValidationRules;

namespace Aura.Controls.Primitive
{
    public class SliderWithIO : Slider
    {
        #region ACCENT COLOR

        public static DependencyProperty AccentColorProperty =
            DependencyProperty.Register(
                "AccentColor",
                typeof(Color),
                typeof(SliderWithIO),
                new PropertyMetadata(Colors.Green)
            );

        public Color AccentColor
        {
            get { return (Color)GetValue(AccentColorProperty); }
            set { SetValue(AccentColorProperty, value); }
        }

        #endregion

        #region TEXT STYLE PROPERTY

        public static DependencyProperty TextStyleProperty =
            DependencyProperty.Register(
                "TextStyle",
                typeof(Style),
                typeof(SliderWithIO));

        public Style TextStyle
        {
            get { return (Style)GetValue(TextStyleProperty); }
            set { SetValue(TextStyleProperty, value); }
        }

        #endregion

        #region BOX TYPE

        public static DependencyProperty BoxTypeProperty =
            DependencyProperty.Register(
                "BoxType",
                typeof(BoxType),
                typeof(SliderWithIO),
                new PropertyMetadata(BoxType.INTEGERS, PropertyChangedBoxType));

        private static void PropertyChangedBoxType(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SliderWithIO)d).OnPropertyChangedBoxType(d, e);
        }

        private void OnPropertyChangedBoxType(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue.Equals(BoxType.INTEGERS)) TypeValidationRule = new IsIntRule();
            if (e.NewValue.Equals(BoxType.DOUBLES)) TypeValidationRule = new IsDoubleRule();
        }

        public BoxType BoxType
        {
            get { return (BoxType)GetValue(BoxTypeProperty); }
            set { SetValue(BoxTypeProperty, value); }
        }

        public ValidationRule TypeValidationRule { get; private set; }

        #endregion

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);
            this.SelectionEnd = Value;
        }

        static SliderWithIO()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SliderWithIO), new FrameworkPropertyMetadata(typeof(SliderWithIO)));
        }

        public SliderWithIO()
        {
            this.SelectionStart = Minimum;
        }


        private TypedTextBox _typedTextBox;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _typedTextBox = GetTemplateChild("PART_Input") as TypedTextBox;

            if (_typedTextBox != null)
            {
                var bind = new Binding
                {
                    Source = this,
                    Path = new PropertyPath("Value"),
                };
                _typedTextBox.SetBinding(ValueProperty, bind);

                _typedTextBox.ValueChanged += (sender, args) =>
                {
                    this.Value = args.NewValue;
                };
            }
            
        }
    }
}
