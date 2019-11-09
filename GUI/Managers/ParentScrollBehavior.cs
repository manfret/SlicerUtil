using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Aura.Managers
{
    public class ParentScrollBehavior : Behavior<UIElement>
    {
        protected override void OnAttached()
        {
            AssociatedObject.PreviewMouseWheel += OnPreviewMouseWheel;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.PreviewMouseWheel -= OnPreviewMouseWheel;
        }

        private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = true;
                var scrollPos = ScrollViewer.VerticalOffset;
                var newPos = scrollPos - e.Delta;
                if (newPos <= ScrollViewer.ScrollableHeight || newPos >= 0)
                {
                    e.Handled = true;
                    ScrollViewer?.UpdateLayout();
                    ScrollViewer?.ScrollToVerticalOffset(newPos);
                }
            }
        }

        public static readonly DependencyProperty ScrollViewerProperty =
            DependencyProperty.Register("ScrollViewer", typeof(ScrollViewer), typeof(ParentScrollBehavior));

        public ScrollViewer ScrollViewer
        {
            get => (ScrollViewer) GetValue(ScrollViewerProperty);
            set => SetValue(ScrollViewerProperty, value);
        }
    }
}