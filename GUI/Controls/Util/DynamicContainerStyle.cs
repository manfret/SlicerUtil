using System;
using System.Collections.Generic;
using System.Windows;

namespace Aura.Controls.Util
{
    public class DynamicContainerStyle
    {
        public static Style GetBaseStyle(DependencyObject obj)
        {
            return (Style)obj.GetValue(BaseStyleProperty);
        }

        public static void SetBaseStyle(DependencyObject obj, Style value)
        {
            obj.SetValue(BaseStyleProperty, value);
        }

        // Using a DependencyProperty as the backing store for BaseStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BaseStyleProperty =
            DependencyProperty.RegisterAttached("BaseStyle", typeof(Style), typeof(DynamicContainerStyle), new UIPropertyMetadata(DynamicContainerStyle.StylesChanged));

        public static Style GetDerivedStyle(DependencyObject obj)
        {
            return (Style)obj.GetValue(DerivedStyleProperty);
        }

        public static void SetDerivedStyle(DependencyObject obj, Style value)
        {
            obj.SetValue(DerivedStyleProperty, value);
        }

        // Using a DependencyProperty as the backing store for DerivedStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DerivedStyleProperty =
            DependencyProperty.RegisterAttached("DerivedStyle", typeof(Style), typeof(DynamicContainerStyle), new UIPropertyMetadata(DynamicContainerStyle.StylesChanged));

        private static void StylesChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            if (!typeof(System.Windows.Controls.ItemsControl).IsAssignableFrom(target.GetType()))
                throw new InvalidCastException("Target must be ItemsControl");

            var Element = (System.Windows.Controls.ItemsControl)target;

            var Styles = new List<Style>();

            var BaseStyle = GetBaseStyle(target);

            if (BaseStyle != null)
                Styles.Add(BaseStyle);

            var DerivedStyle = GetDerivedStyle(target);

            if (DerivedStyle != null)
                Styles.Add(DerivedStyle);

            Element.ItemContainerStyle = MergeStyles(Styles);
        }

        private static Style MergeStyles(ICollection<Style> Styles)
        {
            var NewStyle = new Style();

            foreach (var Style in Styles)
            {
                foreach (var Setter in Style.Setters)
                    NewStyle.Setters.Add(Setter);

                foreach (var Trigger in Style.Triggers)
                    NewStyle.Triggers.Add(Trigger);
            }

            return NewStyle;
        }
    }
}