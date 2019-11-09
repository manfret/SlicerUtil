using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Aura.Controls.Primitive
{
    public class RBText : RadioButton
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(RBText));

        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty ActionColorBrushProperty =
            DependencyProperty.Register("ActionColorBrush", typeof(SolidColorBrush), typeof(RBText));

        public SolidColorBrush ActionColorBrush
        {
            get { return (SolidColorBrush) GetValue(ActionColorBrushProperty); }
            set { SetValue(ActionColorBrushProperty, value);}
        }

        public static readonly DependencyProperty ActLikeButtonProperty =
            DependencyProperty.Register("ActLikeButton", typeof(bool), typeof(RBText), new PropertyMetadata(false));

        public bool ActLikeButton
        {
            get => (bool) GetValue(ActLikeButtonProperty);
            set => SetValue(ActLikeButtonProperty, value);
        }

        protected override void OnChecked(RoutedEventArgs e)
        {
            base.OnChecked(e);
            if (ActLikeButton) SetValue(IsCheckedProperty, false);
        }
    }

    public class RBLeft : RBText
    {

    }

    public class RBRight : RBText
    {

    }
}
