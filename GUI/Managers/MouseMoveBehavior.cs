using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Aura.Managers
{
    public class MouseMoveBehavior : Behavior<UIElement>
    {
        public static readonly DependencyProperty EProperty = DependencyProperty.Register(
            "E", typeof(MouseEventArgs), typeof(MouseMoveBehavior), new PropertyMetadata(default(MouseEventArgs)));

        public MouseEventArgs E
        {
            get { return (MouseEventArgs)GetValue(EProperty); }
            set { SetValue(EProperty, value); }
        }

        protected override void OnAttached()
        {
            AssociatedObject.MouseMove += AssociatedObjectOnMouseMove;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.MouseMove -= AssociatedObjectOnMouseMove;
        }

        private void AssociatedObjectOnMouseMove(object sender, MouseEventArgs e)
        {
            E = e;
        }
    }
}
