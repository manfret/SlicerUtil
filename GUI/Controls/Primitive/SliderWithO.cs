using System;
using System.Windows;
using System.Windows.Controls;

namespace Aura.Controls.Primitive
{
    public class SliderWithO : Slider
    {

        #region TEXT STYLE PROPERTY

        public static DependencyProperty TextStyleProperty =
            DependencyProperty.Register(
                "TextStyle",
                typeof(Style),
                typeof(SliderWithO));

        public Style TextStyle
        {
            get { return (Style)GetValue(TextStyleProperty); }
            set { SetValue(TextStyleProperty, value); }
        }

        #endregion

        //NOT DP
        public Func<double, double> FieldCalculationWay { get; set; }

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);
/*
            UpdateValue();*/
        }


        public SliderWithO()
        {
            this.SelectionStart = Minimum;
        }

        protected TypedTextBox TypedTextBox;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            TypedTextBox = GetTemplateChild("PART_Input") as TypedTextBox;

            if (TypedTextBox != null)
            {
                if (FieldCalculationWay != null)
                {
                    TypedTextBox.Minimum = FieldCalculationWay(this.Minimum);
                    TypedTextBox.Maximum = FieldCalculationWay(this.Maximum);

                    var res = FieldCalculationWay(Value);
/*                    _typedTextBox.ValueString = res.ToString();*/
                    TypedTextBox.Value = res;
                    this.SelectionEnd = Value;
                }
                else
                {
                    TypedTextBox.Minimum = 0;
                    TypedTextBox.Maximum = 100;

                    TypedTextBox.Value = Value;
/*                    _typedTextBox.ValueString = Value.ToString();*/
                    this.SelectionEnd = Value;
                }

/*                //хак, нужен обход т.к. при установке значения Value в коде не вызывается OnValueChanged
                TypedTextBox.Loaded += (sender, args) => UpdateValue();*/
            }
        }

/*        private void UpdateValue()
        {
            if (TypedTextBox != null)
            {
                if (FieldCalculationWay != null)
                {
                    var res = FieldCalculationWay(Value);
                    TypedTextBox.Value = res;
                    TypedTextBox.ValueString = res.ToString();
                    this.SelectionEnd = Value;
                }
                else
                {
                    TypedTextBox.Value = Value;
                    TypedTextBox.ValueString = Value.ToString();
                    this.SelectionEnd = Value;
                }
            }
        }*/
    }
}