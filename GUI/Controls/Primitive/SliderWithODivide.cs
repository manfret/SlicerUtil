using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Aura.Controls.Util;

namespace Aura.Controls.Primitive
{
    public class SliderWithODivide : Slider
    {
        #region TEXT STYLE PROPERTY

        public static DependencyProperty TextStyleProperty =
            DependencyProperty.Register(
                "TextStyle",
                typeof(Style),
                typeof(SliderWithODivide));

        public Style TextStyle
        {
            get { return (Style)GetValue(TextStyleProperty); }
            set { SetValue(TextStyleProperty, value); }
        }

        #endregion

        #region DIVIVDEND

        public static readonly DependencyProperty DiviProperty =
            DependencyProperty.Register("Divi", typeof(double), typeof(SliderWithODivide), new PropertyMetadata(0.0));

        public double Divi
        {
            get { return (double) GetValue(DiviProperty); }
            set { SetValue(DiviProperty, value); }
        }

        #endregion

        public SliderWithODivide()
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
                var multiBind = new MultiBinding();
                var bindToDivi = new Binding();
                bindToDivi.Path = new PropertyPath("Divi");
                bindToDivi.RelativeSource = RelativeSource.TemplatedParent;

                var bindToValue = new Binding();
                bindToValue.Path = new PropertyPath("Value");
                bindToValue.RelativeSource = RelativeSource.TemplatedParent;
                bindToValue.Mode = BindingMode.TwoWay;
                multiBind.Bindings.Add(bindToDivi);
                multiBind.Bindings.Add(bindToValue);
                multiBind.Converter = new RatioToMacroConverter();
                multiBind.ConverterParameter = 4;
                _ttb.SetBinding(ValueProperty, multiBind);


                var bindToMin = new Binding();
                bindToMin.Path = new PropertyPath("Maximum");
                bindToMin.RelativeSource = RelativeSource.TemplatedParent;
                var multiMinBind = new MultiBinding();
                multiMinBind.Bindings.Add(bindToDivi);
                multiMinBind.Bindings.Add(bindToMin);
                multiMinBind.Converter = new RatioToMacroConverter();
                multiMinBind.ConverterParameter = 4;
                _ttb.SetBinding(MinimumProperty, multiMinBind);


                var bindToMax = new Binding();
                bindToMax.Path = new PropertyPath("Minimum");
                bindToMax.RelativeSource = RelativeSource.TemplatedParent;
                var multiMaxBind = new MultiBinding();
                multiMaxBind.Bindings.Add(bindToDivi);
                multiMaxBind.Bindings.Add(bindToMax);
                multiMaxBind.Converter = new RatioToMacroConverter();
                multiMaxBind.ConverterParameter = 4;
                _ttb.SetBinding(MaximumProperty, multiMaxBind);


                var bindToValueSelf = new Binding();
                bindToValueSelf.Path = new PropertyPath("Value");
                bindToValueSelf.RelativeSource = RelativeSource.Self;
                this.SetBinding(Slider.SelectionEndProperty, bindToValueSelf);
            }
        }

        protected override void OnMinimumChanged(double oldMinimum, double newMinimum)
        {
            base.OnMinimumChanged(oldMinimum, newMinimum);

            if (Value <= Minimum) SetValue(ValueProperty, Minimum);
        }

        protected override void OnMaximumChanged(double oldMaximum, double newMaximum)
        {
            base.OnMaximumChanged(oldMaximum, newMaximum);

            if (Value >= Maximum) SetValue(ValueProperty, Maximum);
        }
    }
}