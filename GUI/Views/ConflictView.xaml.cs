using System.Windows;
using System.Windows.Controls;

namespace Aura.Views
{
    /// <summary>
    /// Interaction logic for ConflictView.xaml
    /// </summary>
    public partial class ConflictView : UserControl
    {
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(ConflictView));

        public string Header
        {
            get => (string) GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        public ConflictView()
        {
            InitializeComponent();
        }
    }
}
