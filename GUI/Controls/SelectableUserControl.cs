using System.Windows;
using System.Windows.Controls;

namespace Aura.Controls
{
    public class SelectableUserControl : UserControl
    {
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(SelectableUserControl));

        public bool IsSelected
        {
            get => (bool) GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }
    }
}