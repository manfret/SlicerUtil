using System.Windows;
using System.Windows.Controls;
using Prism.Commands;

namespace Aura.Views
{
    /// <summary>
    /// Interaction logic for SettingsExpander.xaml
    /// </summary>
    public partial class SettingsExpander : Expander
    {
        public static readonly DependencyProperty AdditionalTextProperty =
            DependencyProperty.Register("AdditionalText", typeof(string), typeof(SettingsExpander));

        public string AdditionalText
        {
            get { return (string)GetValue(AdditionalTextProperty); }
            set { SetValue(AdditionalTextProperty, value); }
        }

        public DelegateCommand RemoveCommand { get; set; }
        public DelegateCommand DuplicateCommand { get; set; }
        public DelegateCommand ExportCommand { get; set; }
        

        public SettingsExpander()
        {
            InitializeComponent();
        }
    }
}
