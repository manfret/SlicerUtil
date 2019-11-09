using System.Windows;
using System.Windows.Controls;
using Aura.Controls.Primitive;

namespace Aura.Controls
{
    public class LayupDataToolbox : UserControl
    {
        public static readonly DependencyProperty DeleteEnableProperty = DependencyProperty.Register("DeleteEnable",
            typeof(bool), typeof(LayupDataToolbox), new PropertyMetadata(true));

        public bool DeleteEnable
        {
            get { return (bool) GetValue(DeleteEnableProperty); }
            set { SetValue(DeleteEnableProperty, value);}
        }

        public static readonly RoutedEvent SplitEvent = EventManager.RegisterRoutedEvent("Split",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(LayupDataToolbox));

        public event RoutedEventHandler Split
        {
            add { AddHandler(SplitEvent, value); }
            remove { RemoveHandler(SplitEvent, value); }
        }

        public static readonly RoutedEvent DeleteEvent = EventManager.RegisterRoutedEvent("Delete",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(LayupDataToolbox));

        public event RoutedEventHandler Delete
        {
            add { AddHandler(DeleteEvent, value); }
            remove { RemoveHandler(DeleteEvent, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (GetTemplateChild("ButtonSplit") is ButtonWithVisual splitButton)
            {
                splitButton.Click += (sender, args) =>
                {
                    RaiseEvent(new RoutedEventArgs(SplitEvent));
                };
            }

            if (GetTemplateChild("ButtonDelete") is ButtonWithVisual deleteButton)
            {
                deleteButton.Click += (sender, args) =>
                {
                    RaiseEvent(new RoutedEventArgs(DeleteEvent));
                };
            }
        }
    }
}
