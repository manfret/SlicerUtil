using System.Windows;
using System.Windows.Controls;
using Aura.Controls.Primitive;
using Prism.Commands;

namespace Aura.Controls
{
    public class ModelToolbox : UserControl
    {
        #region SHIFT EVENT

        public static readonly RoutedEvent ShiftEvent = EventManager.RegisterRoutedEvent("Shift",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ModelToolbox));

        public event RoutedEventHandler Shift
        {
            add { AddHandler(ShiftEvent, value); }
            remove { RemoveHandler(ShiftEvent, value); }
        }

        #endregion

        #region ROTATE EVENT

        public static readonly RoutedEvent RotateEvent = EventManager.RegisterRoutedEvent("Rotate",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ModelToolbox));

        public event RoutedEventHandler Rotate
        {
            add { AddHandler(RotateEvent, value); }
            remove { RemoveHandler(RotateEvent, value); }
        }

        #endregion

        #region RESIZE EVENT

        public static readonly RoutedEvent ResizeEvent = EventManager.RegisterRoutedEvent("Resize",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ModelToolbox));

        public event RoutedEventHandler Resize
        {
            add { AddHandler(ResizeEvent, value); }
            remove { RemoveHandler(ResizeEvent, value); }
        }

        #endregion

        #region CLONE EVENT

        public static readonly RoutedEvent CloneEvent = EventManager.RegisterRoutedEvent("Clone", 
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ModelToolbox));

        public event RoutedEventHandler Clone
        {
            add { AddHandler(CloneEvent, value); }
            remove { RemoveHandler(CloneEvent, value); }
        }

        #endregion

        #region SET LAYUP RULES

        public static readonly RoutedEvent SetLayupRulesEvent = EventManager.RegisterRoutedEvent("SetLayupRules",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ModelToolbox));

        public event RoutedEventHandler SetLayupRules
        {
            add { AddHandler(SetLayupRulesEvent, value);}
            remove { RemoveHandler(SetLayupRulesEvent, value);}
        }

        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (GetTemplateChild("ButtonShift") is ButtonWithVisual shiftButton)
            {
                shiftButton.Click += (sender, e) =>
                {
                    RaiseEvent(new RoutedEventArgs(ShiftEvent));
                };
            }

            if (GetTemplateChild("ButtonRotate") is ButtonWithVisual rotateButton)
            {
                rotateButton.Click += (sender, e) =>
                {
                    RaiseEvent(new RoutedEventArgs(RotateEvent));
                };
            }

            if (GetTemplateChild("ButtonResize") is ButtonWithVisual resizeButton)
            {
                resizeButton.Click += (sender, e) =>
                {
                    RaiseEvent(new RoutedEventArgs(ResizeEvent));
                };
            }

            if (GetTemplateChild("ButtonClone") is ButtonWithVisual cloneButton)
            {
                cloneButton.Click += (sender, e) =>
                {
                    RaiseEvent(new RoutedEventArgs(CloneEvent));
                };
            }

            if (GetTemplateChild("ButtonLayup") is ButtonWithVisual layupButton)
            {
                layupButton.Click += (sender, e) =>
                {
                    RaiseEvent(new RoutedEventArgs(SetLayupRulesEvent));
                };
            }
        }
    }
}
