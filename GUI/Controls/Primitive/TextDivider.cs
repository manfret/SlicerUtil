using System.Windows;
using System.Windows.Controls;

namespace Aura.Controls.Primitive
{
    public class TextDivider : Control
    {
        public static DependencyProperty TextProperty =
            DependencyProperty.Register(
                "Text",
                typeof(string),
                typeof(TextDivider),
                new PropertyMetadata(string.Empty)
                );

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        static TextDivider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TextDivider), new FrameworkPropertyMetadata(typeof(TextDivider)));
        }

        private TextBlock _dividerTextBlock;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _dividerTextBlock = GetTemplateChild("PART_DividerTextBlock") as TextBlock;
            if (_dividerTextBlock != null)
            {
                _dividerTextBlock.Height = string.IsNullOrEmpty(Text) ? 0 : double.NaN;
            }
        }
    }
}