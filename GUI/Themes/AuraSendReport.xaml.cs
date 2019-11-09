using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using Aura.Managers;
using Aura.Themes.Localization;
using Unity;
using Util;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using Path = System.IO.Path;

namespace Aura.Themes
{
    /// <summary>
    /// Interaction logic for AuraSendReport.xaml
    /// </summary>
    public partial class AuraSendReport : IDisposable
    {
        private MainWindow _mainWindow;

        private string _projectFilePath;
        private readonly string _tempFolder;
        private readonly ISettingsManager _settingsManager;

        public AuraSendReport()
        {
            InitializeComponent();

            _tempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempFolder);
            _settingsManager = UnityCore.Container.Resolve<ISettingsManager>();
        }

        private void SendReport_OnClick(object sender, RoutedEventArgs e)
        {
            TextBlockSuccess.Visibility = Visibility.Hidden;

            _mainWindow = FindParent<MainWindow>(this);

            if (EMail.Text == string.Empty)
            {
                ErrorText.Text = Common_en_EN.YourEMail;
                return;
            }

            if (!IsValidEmail(EMail.Text))
            {
                ErrorText.Text = Common_en_EN.WrongEmail;
                return;
            }

            try
            {
                var myPing = new Ping();
                var host = "google.com";
                var buffer = new byte[32];
                var timeout = 1000;
                var pingOptions = new PingOptions();
                var reply = myPing.Send(host, timeout, buffer, pingOptions);
                if (reply.Status != IPStatus.Success)
                {
                    ErrorText.Text = "Bad internet connection";
                    return;
                }
            }
            catch
            {
                return;
            }

            try
            {
                if (CheckBoxAddProject.IsChecked.HasValue)
                {
                    if (CheckBoxAddProject.IsChecked.Value)
                    {
                        if (RBAttachCurrent.IsChecked.Value)
                        {
                            var projectName = $"BugReport_{EMail.Text}_{DateTime.Now.ToShortDateString()}";
                            _projectFilePath = Path.Combine(_tempFolder, $"{projectName}.auproj");
                            //_mainWindow.SaveProject(_projectFilePath);
                        }
                    }
                    else
                    {
                        _projectFilePath = null;
                    }
                }
            }
            catch (Exception ex)
            {
                if (_tempFolder != null && _tempFolder.Any() && Directory.Exists(_tempFolder)) Directory.Delete(_tempFolder, true);
                var errDialog = new ErrorDialog(ex, "Send report failed. File saving failed");
                errDialog.ShowDialog();
                return;
            }


            if (ReportText.Text == string.Empty && _projectFilePath == string.Empty)
            {
                ErrorText.Text = Common_en_EN.ReportIsEmptyFillFields;
                return;
            }

            if (_projectFilePath == string.Empty)
            {
                ErrorText.Text = Common_en_EN.ReportIsEmptyFiles;
            }

            ErrorText.Text = string.Empty;

            var to = AuraNaming_en_EN.AuraBugsMailto;
            var from = EMail.Text;

            try
            {
                var bwWorker = new BackgroundWorker { WorkerReportsProgress = false };
                bwWorker.DoWork += (o, args) =>
                {
                    var emailer = new Emailer();
                    emailer.MailTo((Emailer.EmailerStruct)args.Argument);
                };
                bwWorker.RunWorkerCompleted += (o, args) =>
                {
                    var timer = new Timer {Interval = 5000};
                    timer.Start();
                    timer.Tick += (obj, a) =>
                    {
                        TextBlockSuccess.Visibility = Visibility.Collapsed;
                        timer.Stop();
                    };
                    TextBlockSuccess.Visibility = Visibility.Visible;
                };
                var emailerStruct = new Emailer.EmailerStruct
                {
                    To = to,
                    From = from,
                    Version = AuraNaming_en_EN.AuraVersion,
                    Body = ReportText.Text,
                    Attach = _projectFilePath
                };
                bwWorker.RunWorkerAsync(emailerStruct);
            }
            catch (Exception exception)
            {
               var errDialog = new ErrorDialog(exception, "Send report failed");
                errDialog.ShowDialog();
            }
        }

        public void Dispose()
        {
            if (_tempFolder != null && _tempFolder.Any() && Directory.Exists(_tempFolder)) Directory.Delete(_tempFolder, true);
        }

        private static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            //get parent item
            var parentObject = VisualTreeHelper.GetParent(child);

            switch (parentObject)
            {
                //we've reached the end of the tree
                case null:
                    return null;
                //check if the parent matches the type we're looking for
                case T parent:
                    return parent;
                default:
                    return FindParent<T>(parentObject);
            }
        }

        bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void RBAttachAnothert_OnChecked(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                InitialDirectory = _settingsManager.ProjectDirectory,
                Filter = Common_en_EN.ProjectFilter
            };
            if (openFileDialog.ShowDialog() == true)
            {
                _projectFilePath = openFileDialog.FileName;
            }
        }

        private void CheckBoxAddProject_OnChecked(object sender, RoutedEventArgs e)
        {
            if (StackPanelProject != null) StackPanelProject.Visibility = Visibility.Visible;
        }

        private void CheckBoxAddProject_OnUnchecked(object sender, RoutedEventArgs e)
        {
            if (StackPanelProject != null) StackPanelProject.Visibility = Visibility.Hidden;
        }
    }
}
