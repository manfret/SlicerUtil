using System;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Aura.Managers;
using Aura.Themes;
using Aura.Themes.Localization;
using Prism.Commands;
using Util;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace Aura.ViewModels
{
    public class SendReportVM : INotifyPropertyChanged
    {
        private ISettingsManager _settingsManager;

        #region STATUS STRING

        private string _statusString;
        public string StatusString
        {
            get => _statusString;
            private set
            {
                _statusString = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region EMAIL

        public string _email;

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region DESCRIPTION

        private string _description;
        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }

        #endregion

        private string _attachedFilePath;
        public DelegateCommand AttachProjectCommand { get; private set; }
        public DelegateCommand SendReportCommand { get; private set; }

        public SendReportVM()
        {
            AttachProjectCommand = new DelegateCommand(AttachProjectExecuteMethod, () => CanAttach);
            SendReportCommand = new DelegateCommand(SendReportExecuteMethod, () => CanSend);

            _timer = new Timer {Interval = 7000};
            _timer.Tick += (sender, eventArgs) =>
            {
                StatusString = null;
                _timer.Stop();
            };
        }

        #region CAN ATTACH

        private bool _canAttach;
        private bool CanAttach
        {
            get => _canAttach;
            set
            {
                _canAttach = value;
                AttachProjectCommand?.RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region CAN SEND

        private bool _canSend;
        private bool CanSend
        {
            get => _canSend;
            set
            {
                _canSend = value;
                SendReportCommand?.RaiseCanExecuteChanged();
            }
        }

        #endregion

        private readonly Timer _timer;

        private bool IsValidEmail(string email)
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
        private void SendReportExecuteMethod()
        {
            if (Email == null || !Email.Any() || !IsValidEmail(Email))
            {
                StatusString = Common_en_EN.WrongEmail;
                _timer.Start();
                return;
            }

            CanAttach = false;
            CanSend = false;

            #region PING

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
                    StatusString = "Bad internet connection";
                    CanAttach = true;
                    CanSend = true;
                    return;
                }
            }
            catch
            {
                CanAttach = true;
                CanSend = true;
                return;
            }

            #endregion

            #region SENDING

            var to = AuraNaming_en_EN.AuraSupportEmail;
            var from = AuraNaming_en_EN.AuraBugsMailto;
            var replyTo = Email;

            try
            {
                StatusString = "Sending";
                var bwWorker = new BackgroundWorker { WorkerReportsProgress = false };
                bwWorker.DoWork += (o, args) =>
                {
                    var emailer = new Emailer();
                    emailer.MailTo((Emailer.EmailerStruct)args.Argument);
                };
                bwWorker.RunWorkerCompleted += (o, args) =>
                {
                    StatusString = "Success. Check your email for the confirmation";
                    CanAttach = true;
                    CanSend = true;
                    _timer.Start();
                };
                var emailerStruct = new Emailer.EmailerStruct
                {
                    To = to,
                    From = from,
                    ReplyTo = replyTo,
                    Version = AuraNaming_en_EN.AuraVersion,
                    Body = Description,
                    Attach = _attachedFilePath
                };
                bwWorker.RunWorkerAsync(emailerStruct);
            }
            catch (Exception exception)
            {
                var errDialog = new ErrorDialog(exception, "Send report failed");
                errDialog.ShowDialog();
                StatusString = "Failed";
                CanAttach = true;
                CanSend = true;
            }

            #endregion
        }

        private void AttachProjectExecuteMethod()
        {
            var openFileDialog = new OpenFileDialog
            {
                InitialDirectory = _settingsManager.ProjectDirectory,
                Filter = Common_en_EN.ProjectFilter
            };
            if (openFileDialog.ShowDialog() == true)
            {
                _attachedFilePath = openFileDialog.FileName;
            }
        }

        public void Initialize(ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
        }

        public void OnLoad()
        {
            _attachedFilePath = null;
            StatusString = null;
            CanAttach = true;
            CanSend = true;
        }

        #region ON PROPERTY CHANGED

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string caller = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(caller));
        }

        #endregion
    }
}
