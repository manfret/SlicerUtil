using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using Aura.Controls.ValidationRules;

namespace Aura.Controls.Primitive
{
    public enum BoxType
    {
        INTEGERS,
        DOUBLES
    }

    public class TypedTextBox : RangeBase
    {
        #region BOX TYPE

        public static DependencyProperty BoxTypeProperty =
            DependencyProperty.Register(
                "BoxType",
                typeof(BoxType),
                typeof(TypedTextBox),
                new PropertyMetadata(BoxType.INTEGERS, PropertyChangedBoxType));

        private static void PropertyChangedBoxType(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TypedTextBox)d).OnPropertyChangedBoxType(d, e);
        }

        private void OnPropertyChangedBoxType(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue.Equals(BoxType.INTEGERS)) _typeValidationRule = new IsIntRule();
            if (e.NewValue.Equals(BoxType.DOUBLES)) _typeValidationRule = new IsDoubleRule();
        }

        public BoxType BoxType
        {
            get { return (BoxType)GetValue(BoxTypeProperty); }
            set { SetValue(BoxTypeProperty, value); }
        }

        private ValidationRule _typeValidationRule;
        private readonly MinMaxRule _minMaxRule;

        #endregion

        #region HAS ERROR

        public static readonly DependencyProperty HasErrorProperty =
            DependencyProperty.Register("HasError", typeof(bool), typeof(TypedTextBox));

        public bool HasError
        {
            get { return (bool) GetValue(HasErrorProperty); }
            set { SetValue(HasErrorProperty, value); } 
        }

        #endregion

        #region VALUE STRING

        public static DependencyProperty ValueStringProperty =
            DependencyProperty.Register(
                "ValueString",
                typeof(string),
                typeof(TypedTextBox),
                new PropertyMetadata(string.Empty));

        public string ValueString
        {
            get { return (string)GetValue(ValueStringProperty); }
            set { SetValue(ValueStringProperty, value); }
        }

        #endregion

        #region TEXT STYLE PROPERTY

        public static DependencyProperty TextStyleProperty =
            DependencyProperty.Register(
                "TextStyle",
                typeof(Style),
                typeof(TypedTextBox));

        public Style TextStyle
        {
            get { return (Style)GetValue(TextStyleProperty); }
            set { SetValue(TextStyleProperty, value); }
        }

        #endregion

        #region IS READ ONLY

        public static DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(
                "IsReadOnly",
                typeof(bool),
                typeof(TypedTextBox),
                new PropertyMetadata(false)
            );

        public bool IsReadOnly
        {
            get { return (bool) GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        #endregion

        public string Text => _textTextBox?.Text;

        static TypedTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TypedTextBox), new FrameworkPropertyMetadata(typeof(TypedTextBox)));

            FocusableProperty.OverrideMetadata(typeof(TypedTextBox), new FrameworkPropertyMetadata(false));
            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(TypedTextBox), new FrameworkPropertyMetadata(KeyboardNavigationMode.Local));
        }


        public TypedTextBox()
        {
            _typeValidationRule = new IsIntRule();
            _minMaxRule = new MinMaxRule
            {
                Minimum = this.Minimum,
                Maximum = this.Maximum
            };

            var bind = new Binding
            {
                RelativeSource = RelativeSource.Self,
                Mode = BindingMode.TwoWay,
                Path = new PropertyPath("Value")
            };
            //bind.ValidationRules.Add(new DotRule());
            this.SetBinding(ValueStringProperty, bind);
        }

        #region OVERRIDES MINIMUM & MAXIMUM

        protected override void OnMinimumChanged(double oldMinimum, double newMinimum)
        {
            base.OnMinimumChanged(oldMinimum, newMinimum);
            _minMaxRule.Minimum = (double)newMinimum;
        }

        protected override void OnMaximumChanged(double oldMaximum, double newMaximum)
        {
            base.OnMaximumChanged(oldMaximum, newMaximum);
            _minMaxRule.Maximum = (double)newMaximum;
        }

        #endregion

        private TextBox _textTextBox;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _textTextBox = GetTemplateChild("PART_TextBox") as TextBox;
            if (_textTextBox == null) return;

            var textBinding = new Binding
            {
                RelativeSource = RelativeSource.TemplatedParent,
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Path = new PropertyPath("ValueString"),
                Delay = 600
            };
            textBinding.ValidationRules.Add(_typeValidationRule);
            textBinding.ValidationRules.Add(_minMaxRule);

            var readOnlyBinding = new Binding
            {
                RelativeSource = RelativeSource.TemplatedParent,
                Path = new PropertyPath("IsReadOnly")
            };

            _textTextBox.SetBinding(TextBox.TextProperty, textBinding);
            _textTextBox.SetBinding(TextBoxBase.IsReadOnlyProperty, readOnlyBinding);

            HasError = false;

            _textTextBox.TextChanged += (sender, args) =>
            {
                var minMaxVal = _minMaxRule.Validate(_textTextBox.Text, CultureInfo.InvariantCulture).IsValid;
                var typeVal = _typeValidationRule.Validate(_textTextBox.Text, CultureInfo.InvariantCulture).IsValid;
                HasError = !minMaxVal || !typeVal;
            };

        }

        public void SetTextBoxContent(string content)
        {
            _textTextBox.SetValue(TextBox.TextProperty, content);
        }

        private int _doubleClickCount = 0;
        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            _doubleClickCount++;

            if (_doubleClickCount == 1)
            {
                base.OnMouseDoubleClick(e);
                return;
            }

            if (_doubleClickCount == 2)
            {
                _doubleClickCount = 0;
                _textTextBox.SelectAll();
            }
        }
    }
}
