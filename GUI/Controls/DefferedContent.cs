using System.Windows;

namespace Aura.Controls
{
    public class Deferred
    {
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.RegisterAttached(
                "Content",
                typeof(object),
                typeof(Deferred),
                new PropertyMetadata());

        public static object GetContent(DependencyObject obj)
        {
            return obj.GetValue(ContentProperty);
        }

        public static void SetContent(DependencyObject obj, object value)
        {
            obj.SetValue(ContentProperty, value);
        }
    }
}