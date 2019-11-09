using System.Windows;
using System.Windows.Controls;
using Prism.Commands;

namespace Aura.Views
{
    /// <summary>
    /// Interaction logic for ExpanderExtruderP.xaml
    /// </summary>
    public partial class ExtruderExpander : Expander
    {
        public static readonly DependencyProperty RemoveCommandProperty =
            DependencyProperty.Register("RemoveCommand", typeof(DelegateCommand), typeof(ExtruderExpander));

        public DelegateCommand RemoveCommand
        {
            get => (DelegateCommand)GetValue(RemoveCommandProperty);
            set => SetValue(RemoveCommandProperty, value);
        }

        public static readonly DependencyProperty AdditionalTextProperty =
            DependencyProperty.Register("AdditionalText", typeof(string), typeof(ExtruderExpander));

        public string AdditionalText
        {
            get => (string) GetValue(AdditionalTextProperty);
            set => SetValue(AdditionalTextProperty, value);
        }

        public static readonly DependencyProperty ExpandSiteThicknessProperty = DependencyProperty.Register("ExpandSiteThickness",
            typeof(Thickness), typeof(ExtruderExpander), new PropertyMetadata(new Thickness(1, 0, 1, 1)));
        public Thickness ExpandSiteThickness
        {
            get => (Thickness) GetValue(ExpandSiteThicknessProperty);
            set => SetValue(ExpandSiteThicknessProperty, value);
        }

        public ExtruderExpander()
        {
            InitializeComponent();
        }

    }
}
