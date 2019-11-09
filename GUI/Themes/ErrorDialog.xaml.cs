using System;
using System.Windows;
using Aura.Themes.Localization;

namespace Aura.Themes
{
    /// <summary>
    /// Interaction logic for ErrorDialog.xaml
    /// </summary>
    public partial class ErrorDialog : Window
    {
        private readonly string _shortMessage;
        private readonly string _details;

        public ErrorDialog(Exception ex, string newHeader = null)
        {
            InitializeComponent();

            if (newHeader != null) ErrorSlicingUnsuccessMessage.Text = newHeader;

            if (ex is AggregateException ae)
            {
                if (ae.InnerExceptions.Count > 0)
                {
                    _shortMessage = ae.InnerExceptions[0].InnerException.Message;
                }

                _details += $"Base error {ae.Message}\n\n";
                var i = 0;
                foreach (var innerException in ae.InnerExceptions)
                {
                    _details += $"Error {i++} \n";
                    if (!string.IsNullOrEmpty(innerException.InnerException.Message)) _details += $"Message {innerException.InnerException.Message} \n";
                    if (!string.IsNullOrEmpty(innerException.InnerException.StackTrace)) _details += $"Stack trace {innerException.InnerException.StackTrace} \n";
                }
            }
            else
            {
                _shortMessage = ex.Message;
                if (!string.IsNullOrEmpty(ex.StackTrace)) _details = $"Stack trace {ex.StackTrace} \n";
            }

            ErrorMessage.Text = _shortMessage += "\n" + _details;
        }

        private void ButtonOkClick(object sender, RoutedEventArgs e)
        {
            GetWindow(this).DialogResult = true;
            GetWindow(this).Close();
        }

        private void ButtonCopyDetails(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(_details);
        }
    }
}
