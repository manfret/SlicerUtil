using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Prism.Commands;

namespace Aura.Views
{
    /// <summary>
    /// Interaction logic for LayupExpander.xaml
    /// </summary>
    public partial class LayupExpander : Expander
    {
        public static readonly DependencyProperty RemoveCommandProperty =
            DependencyProperty.Register("RemoveCommand", typeof(DelegateCommand), typeof(LayupExpander));

        public DelegateCommand RemoveCommand
        {
            get => (DelegateCommand)GetValue(RemoveCommandProperty);
            set => SetValue(RemoveCommandProperty, value);
        }

        public static readonly DependencyProperty UpCommandProperty =
            DependencyProperty.Register("UpCommand", typeof(DelegateCommand), typeof(LayupExpander));

        public DelegateCommand UpCommand
        {
            get => (DelegateCommand)GetValue(UpCommandProperty);
            set => SetValue(UpCommandProperty, value);
        }


        public static readonly DependencyProperty DownCommandProperty =
            DependencyProperty.Register("DownCommand", typeof(DelegateCommand), typeof(LayupExpander));

        public DelegateCommand DownCommand
        {
            get => (DelegateCommand)GetValue(DownCommandProperty);
            set => SetValue(DownCommandProperty, value);
        }

        public static readonly DependencyProperty AdditionalTextProperty =
            DependencyProperty.Register("AdditionalText", typeof(string), typeof(LayupExpander));

        public string AdditionalText
        {
            get => (string)GetValue(AdditionalTextProperty);
            set => SetValue(AdditionalTextProperty, value);
        }

        public static readonly DependencyProperty ExpandSiteThicknessProperty = DependencyProperty.Register("ExpandSiteThickness",
            typeof(Thickness), typeof(LayupExpander), new PropertyMetadata(new Thickness(1, 0, 1, 1)));
        public Thickness ExpandSiteThickness
        {
            get => (Thickness)GetValue(ExpandSiteThicknessProperty);
            set => SetValue(ExpandSiteThicknessProperty, value);
        }

        public static readonly DependencyProperty LeftColorProperty =
            DependencyProperty.Register("LeftColor", typeof(Color), typeof(LayupExpander));
        public Color LeftColor
        {
            get => (Color)GetValue(LeftColorProperty);
            set => SetValue(LeftColorProperty, value);
        }

        public static readonly DependencyProperty ShowColorProperty =
            DependencyProperty.Register("ShowColor", typeof(bool), typeof(LayupExpander));
        public bool ShowColor
        {
            get => (bool)GetValue(ShowColorProperty);
            set => SetValue(ShowColorProperty, value);
        }

        public LayupExpander()
        {
            InitializeComponent();
        }
    }
}
