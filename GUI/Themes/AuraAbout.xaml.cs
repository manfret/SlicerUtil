using System.Windows;
using System.Windows.Navigation;
using Aura.Themes.Localization;

namespace Aura.Themes
{
    /// <summary>
    /// Interaction logic for AuraAbout.xaml
    /// </summary>
    public partial class AuraAbout : Window
    {
        public AuraAbout()
        {
            InitializeComponent();

            Version.Text = $"{AuraNaming_en_EN.AnisoprintName} {AuraNaming_en_EN.AuraName} ({AuraNaming_en_EN.AuraVersion})";
        }

        private void ButtonOkClick(object sender, RoutedEventArgs e)
        {
            GetWindow(this).DialogResult = true;
            GetWindow(this).Close();
        }

        private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.ToString());
        }
    }
}
