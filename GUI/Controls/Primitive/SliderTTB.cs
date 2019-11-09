using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Aura.Controls.Util;

namespace Aura.Controls.Primitive
{
    public class SliderTTB : Slider
    {
        #region TEXT STYLE PROPERTY

        public static DependencyProperty TextStyleProperty =
            DependencyProperty.Register(
                "TextStyle",
                typeof(Style),
                typeof(SliderTTB));

        public Style TextStyle
        {
            get { return (Style)GetValue(TextStyleProperty); }
            set { SetValue(TextStyleProperty, value); }
        }

        #endregion

        public SliderTTB()
        {
            this.SelectionStart = Minimum;
        }

        private TypedTextBox _ttb;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _ttb = GetTemplateChild("PART_Input") as TypedTextBox;
            if (_ttb != null)
            {
                var bindToMin = new Binding
                {
                    Path = new PropertyPath("Minimum"),
                    RelativeSource = RelativeSource.TemplatedParent
                };
                _ttb.SetBinding(MinimumProperty, bindToMin);


                var bindToMax = new Binding
                {
                    Path = new PropertyPath("Maximum"),
                    RelativeSource = RelativeSource.TemplatedParent
                };
                _ttb.SetBinding(MaximumProperty, bindToMax);


                var bindToValueSelf = new Binding
                {
                    Path = new PropertyPath("Value"),
                    RelativeSource = RelativeSource.Self
                };
                this.SetBinding(SelectionEndProperty, bindToValueSelf);

                var bindToValue = new Binding();
                bindToValue.Path = new PropertyPath("Value");
                bindToValue.RelativeSource = RelativeSource.TemplatedParent;
                bindToValue.Mode = BindingMode.TwoWay;
                _ttb.SetBinding(ValueProperty, bindToValue);

                var bindReadonly = new Binding
                {
                    Path = new PropertyPath("IsEnabled"),
                    RelativeSource = RelativeSource.TemplatedParent,
                    Converter = new InvertBooleanConverter()
                };
                _ttb.SetBinding(TypedTextBox.IsReadOnlyProperty, bindReadonly);
            }
        }
    }
}