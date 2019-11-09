using System.Windows;
using System.Windows.Media;

namespace Aura.Views
{
    /// <summary>
    /// Interaction logic for ToggleButtonWithBrush.xaml
    /// </summary>
    public partial class ToggleButtonWithBrush 
    {
        public static readonly DependencyProperty NormalBrushProperty =
            DependencyProperty.Register("NormalBrush", typeof(Brush), typeof(ToggleButtonWithBrush));

        public Brush NormalBrush
        {
            get => (Brush) GetValue(NormalBrushProperty);
            set => SetValue(NormalBrushProperty, value);
        }

        public static readonly DependencyProperty HighlightBrushProperty =
            DependencyProperty.Register("HighlightBrush", typeof(Brush), typeof(ToggleButtonWithBrush));

        public Brush HighlightBrush
        {
            get => (Brush)GetValue(HighlightBrushProperty);
            set => SetValue(HighlightBrushProperty, value);
        }

        public static readonly DependencyProperty DisabledBrushProperty =
            DependencyProperty.Register("DisabledBrush", typeof(Brush), typeof(ToggleButtonWithBrush));

        public Brush DisabledBrush
        {
            get => (Brush)GetValue(DisabledBrushProperty);
            set => SetValue(DisabledBrushProperty, value);
        }

        public ToggleButtonWithBrush()
        {
            InitializeComponent();
        }
    }
}
