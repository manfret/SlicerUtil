using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Aura.Controls.Resources;
using Aura.Controls.Util;
using Aura.Managers;
using Aura.Themes.Localization;
using Ionic.Zip;
using Tools;
using Unity;
using Util;

namespace Aura.Themes
{

    /// <summary>
    /// Interaction logic for ComposerFirmwarer.xaml
    /// </summary>
    public partial class AuraComposerFirmwarer : Window
    {
        private enum UploadMode
        {
            STANDART,
            DEV
        }

        private readonly ISettingsManager _settingsManager;
        private bool _isDevMode = false;
        private UploadMode _uploadMode = UploadMode.STANDART;

        private readonly Timer _timer;
        private int count = 0;

        private struct BWArguments
        {
            public string Port { get; set; }
            public string Path { get; set; }
            public string FileVersion { get; set; }
        }

        public AuraComposerFirmwarer()
        {
            InitializeComponent();
            _settingsManager = UnityCore.Container.Resolve<ISettingsManager>();
            PathPanel.InitialDirectory = _settingsManager.FirmwareDirectory;
            CMbPorts.ItemsSource = SerialPort.GetPortNames().Distinct();
            TextBlockFirmwareVersion.Visibility = Visibility.Hidden;
            CMbFileVersions.Visibility = Visibility.Hidden;
            BtnDev.Visibility = Visibility.Hidden;
            Pusheeen.Visibility = Visibility.Hidden;
            Pusheeen.Source = Imaging.CreateBitmapSourceFromHBitmap(SettingsTipsResorces.Pusheen.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            _timer = new Timer {Interval = 5000};
            _timer.Tick += (o, args) =>
            {
                count = 0;
                _timer.Stop();
            };

            Refresh();
        }

        private void Refresh()
        {
            TextBlockPrinterReady.Visibility = Visibility.Hidden;
            TextBlockFirmwareVerified.Visibility = Visibility.Hidden;
            TextBlockPrinterModelVerified.Visibility = Visibility.Hidden;
            TextBlockDisplayReady.Visibility = Visibility.Hidden;
            TextBlockSuccess.Visibility = Visibility.Hidden;
            TextBlockFirmwareStarted.Visibility = Visibility.Hidden;
            TextBlockDisplayFirmwareUploaded.Visibility = Visibility.Hidden;
            TextBlockEEPROMWritten.Visibility = Visibility.Hidden;
            TextBlockError.Text = string.Empty;
        }


        private void AddTimerForTextBlock(TextBlock txtb)
        {
            var timer = new Timer { Interval = 1000 };
            var i = 1;
            timer.Start();
            timer.Tick += (sender, eventArgs) =>
            {
                switch (i % 3)
                {
                    case 1:
                        txtb.Text = ".";
                        break;
                    case 2:
                        txtb.Text = "..";
                        break;
                    case 0:
                        txtb.Text = "...";
                        break;
                }
                i++;
            };

            txtb.Tag = timer;
        }

        private void StopTimerForTextBlock(TextBlock txtBlock)
        {
            var timer = txtBlock.Tag as Timer;

            timer?.Stop();
            timer?.Dispose();

            txtBlock.Text = string.Empty;
            txtBlock.Tag = null;
        }

        private void FirmIt(bool devMode, bool isObsolete)
        {
            TextBlockError.Text = GetErrorText();
            if (TextBlockError.Text.Any()) return;

            TextBlockPrinterReady.Visibility = Visibility.Visible;
            TextBlockFirmwareVerified.Visibility = Visibility.Visible;
            TextBlockPrinterModelVerified.Visibility = Visibility.Visible;
            TextBlockDisplayReady.Visibility = Visibility.Visible;
            TextBlockSuccess.Visibility = Visibility.Visible;
            TextBlockFirmwareStarted.Visibility = Visibility.Visible;
            TextBlockDisplayFirmwareUploaded.Visibility = Visibility.Visible;
            TextBlockEEPROMWritten.Visibility = Visibility.Visible; 

            TextBlockPrinterReady.Foreground = Brushes.DarkGray;
            TextBlockFirmwareVerified.Foreground = Brushes.DarkGray;
            TextBlockPrinterModelVerified.Foreground = Brushes.DarkGray;
            TextBlockDisplayReady.Foreground = Brushes.DarkGray;
            TextBlockSuccess.Foreground = Brushes.DarkGray;
            TextBlockFirmwareStarted.Foreground = Brushes.DarkGray;
            TextBlockDisplayFirmwareUploaded.Foreground = Brushes.DarkGray;
            TextBlockEEPROMWritten.Foreground = Brushes.DarkGray;

            TextBlockPrinterReady.Text = Common_en_EN.PrinterConnecting;
            TextBlockFirmwareVerified.Text = Common_en_EN.FirmwareUploading;
            TextBlockPrinterModelVerified.Text = Common_en_EN.PrinterModelVerifying;
            TextBlockDisplayReady.Text = Common_en_EN.DisplayConnecting;
            TextBlockSuccess.Text = Common_en_EN.Success;
            TextBlockFirmwareStarted.Text = Common_en_EN.FirmwaringStarted;
            TextBlockDisplayFirmwareUploaded.Text = Common_en_EN.FirmwareDisplayUpdating;
            TextBlockEEPROMWritten.Text = Common_en_EN.SystemConfigurationUpdating;

            TextBlockFirmwareStarted.Foreground = Brushes.Green;

            using (var bwWorker = new BackgroundWorker { WorkerReportsProgress = true })
            {
                (FirmwarerCode code, string error) resCode = (FirmwarerCode.NONE, "");
                bwWorker.DoWork += (o, args) =>
                {
                    //перепрошивка
                    if (isObsolete)
                    {
                        var cfObsolete = new ComposerFirmwarerObsolete();
                        var i = 0;
                        cfObsolete.Updated += (oo, status) => bwWorker.ReportProgress(++i, status);
                        var bwArguments = (BWArguments)args.Argument;
                        resCode = cfObsolete.FirmIt(bwArguments.Port, bwArguments.Path, devMode);
                    }
                    else
                    {
                        var cf = new ComposerFirmwarer();
                        var i = 0;
                        cf.Updated += (oo, status) => bwWorker.ReportProgress(++i, status);
                        var bwArguments = (BWArguments)args.Argument;
                        resCode = cf.FirmIt(bwArguments.Port, bwArguments.Path, devMode, bwArguments.FileVersion);
                    }
                };

                AddTimerForTextBlock(TextBlockPrinterReady_SPEC);

                bwWorker.ProgressChanged += (o, args) =>
                {
                    var status = (FirmwarerStatus)args.UserState;
                    switch (status)
                    {
                        case FirmwarerStatus.PRINTER_READY:
                            if (TextBlockPrinterModelVerified_SPEC.Tag != null) break;
                            StopTimerForTextBlock(TextBlockPrinterReady_SPEC);
                            AddTimerForTextBlock(TextBlockPrinterModelVerified_SPEC);
                            TextBlockPrinterReady.Text = Common_en_EN.PrinterReady;
                            TextBlockPrinterReady.Foreground = Brushes.Green;
                            break;
                        case FirmwarerStatus.PRINTER_MODEL_VERIFIED:
                            if (TextBlockFirmwareVerified_SPEC.Tag != null) break;
                            StopTimerForTextBlock(TextBlockPrinterModelVerified_SPEC);
                            AddTimerForTextBlock(TextBlockFirmwareVerified_SPEC);
                            TextBlockPrinterModelVerified.Foreground = Brushes.Green;
                            TextBlockPrinterModelVerified.Text = Common_en_EN.InformationReceived;
                            break;
                        case FirmwarerStatus.FIRMWARE_VERIFIED:
                            if (TextBlockDisplayReady_SPEC.Tag != null) break;
                            StopTimerForTextBlock(TextBlockFirmwareVerified_SPEC);
                            AddTimerForTextBlock(TextBlockDisplayReady_SPEC);
                            TextBlockFirmwareVerified.Foreground = Brushes.Green;
                            TextBlockFirmwareVerified.Text = Common_en_EN.FirmwareVerified;
                            break;
                        case FirmwarerStatus.DISPLAY_READY:
                            if (TextBlockDisplayFirmwareUploaded_SPEC.Tag != null) break;
                            StopTimerForTextBlock(TextBlockDisplayReady_SPEC);
                            AddTimerForTextBlock(TextBlockDisplayFirmwareUploaded_SPEC);
                            TextBlockDisplayReady.Foreground = Brushes.Green;
                            TextBlockDisplayReady.Text = Common_en_EN.FirmwareDisplayReady;
                            break;
                        case FirmwarerStatus.DISPLAY_UPDATED:
                            if (TextBlockEEPROMWritten_SPEC.Tag != null) break;
                            StopTimerForTextBlock(TextBlockDisplayFirmwareUploaded_SPEC);
                            AddTimerForTextBlock(TextBlockEEPROMWritten_SPEC);
                            TextBlockDisplayFirmwareUploaded.Foreground = Brushes.Green;
                            TextBlockDisplayFirmwareUploaded.Text = Common_en_EN.FirmwareDisplayUpdated;
                            break;
                        case FirmwarerStatus.EEPROM_UPDATED:
                            StopTimerForTextBlock(TextBlockEEPROMWritten_SPEC);
                            TextBlockEEPROMWritten.Foreground = Brushes.Green;
                            TextBlockEEPROMWritten.Text = Common_en_EN.SystemConfigurationUpdated;
                            break;
                    }
                };
                bwWorker.RunWorkerCompleted += (o, args) =>
                {
                    var resCodeMessage = resCode.error;
                    var errDialog = new ErrorDialog(new Exception(resCodeMessage), "Firmwaring failed");
                    switch (resCode.code)
                    {
                        case FirmwarerCode.SUCCESS:
                            TextBlockSuccess.Foreground = Brushes.Green;
                            break;
                        case FirmwarerCode.FIRMWARE_NOT_VERIFIED:
                            TextBlockFirmwareVerified.Foreground = Brushes.Red;
                            if (resCodeMessage != null && resCodeMessage.Any()) errDialog.ShowDialog();
                            break;
                        case FirmwarerCode.PRINTER_NOT_READY:
                            TextBlockPrinterReady.Foreground = Brushes.Red;
                            break;
                        case FirmwarerCode.PRINTER_MODEL_CANT_BE_RECEIVED:
                            TextBlockPrinterModelVerified.Foreground = Brushes.Red;
                            break;
                        case FirmwarerCode.UNKNOWN_ERROR:
                            TextBlockError.Text = resCode.error.Any() ? resCode.error : Common_en_EN.UnknownError;
                            TextBlockError.Foreground = Brushes.Red;
                            break;
                        case FirmwarerCode.UPLOADING_ERROR:
                            TextBlockFirmwareVerified.Foreground = Brushes.Red;
                            TextBlockError.Text = Common_en_EN.FirmwareUploadError;
                            TextBlockError.Foreground = Brushes.Red;
                            if (resCodeMessage != null && resCodeMessage.Any()) errDialog.ShowDialog();
                            break;
                        case FirmwarerCode.FIRMWARE_FILE_INCORRECT:
                            TextBlockError.Text = Common_en_EN.FirmwareFileIncorrect;
                            TextBlockError.Foreground = Brushes.Red;
                            TextBlockPrinterModelVerified.Foreground = Brushes.Red;
                            break;
                        case FirmwarerCode.PRINTER_MODEL_INCORRECT:
                            TextBlockError.Text = Common_en_EN.FirmwarePrinterModelIncorrect;
                            TextBlockError.Foreground = Brushes.Red;
                            TextBlockPrinterModelVerified.Foreground = Brushes.Red;
                            break;
                        case FirmwarerCode.DISPLAY_NOT_READY:
                            TextBlockDisplayReady.Foreground = Brushes.Red;
                            break; ;
                        case FirmwarerCode.DISPLAY_FIRMWARING_ERROR:
                        case FirmwarerCode.DISPLAY_FILE_INCORRECT:
                            TextBlockDisplayFirmwareUploaded.Foreground = Brushes.Red;
                            break;
                        case FirmwarerCode.CONFIG_WRITTING_ERROR:
                            TextBlockError.Text = Common_en_EN.SystemConfigurationWritingError;
                            TextBlockError.Foreground = Brushes.Red;
                            TextBlockEEPROMWritten.Foreground = Brushes.Red;
                            break;
                        case FirmwarerCode.EEPROM_WRITTING_ERROR:
                            TextBlockError.Text = Common_en_EN.SystemConfigurationSavingError;
                            TextBlockError.Foreground = Brushes.Red;
                            TextBlockEEPROMWritten.Foreground = Brushes.Red;
                            break;
                    }
                };

                bwWorker.RunWorkerAsync(new BWArguments
                {
                    Port = (string)(CMbPorts.SelectedValue),
                    Path = PathPanel.Path,
                    FileVersion = (_uploadMode == UploadMode.DEV && _isDevMode && CMbFileVersions.SelectedItem != null) ? ((FirmwarerVersion)CMbFileVersions.SelectedItem).ForShow : null
                });
            }
        }

        private void ButtonFirmawareIt_Click(object sender, RoutedEventArgs e)
        {
            _uploadMode = UploadMode.STANDART;
            var extension = Path.GetExtension(PathPanel.Path);
            FirmIt(false, extension == ".fw");
        }

        private void ButtonFirmawareItDev_Click(object sender, RoutedEventArgs e)
        {
            _uploadMode = UploadMode.DEV;
            var extension = Path.GetExtension(PathPanel.Path);
            FirmIt(true, extension == ".fw");
        }

        private void ButtonRefresh_OnClick(object sender, RoutedEventArgs e)
        {
            var allPorts = SerialPort.GetPortNames();
            var selectedItem = CMbPorts.SelectedItem;
            CMbPorts.ItemsSource = null;
            CMbPorts.ItemsSource = allPorts;
            if (selectedItem != null && allPorts.Any() && allPorts.Contains(selectedItem)) CMbPorts.SelectedItem = selectedItem;
        }

        private string GetErrorText()
        {
            if (CMbPorts.SelectedItem == null) return Common_en_EN.FirmwareNoCmbPort;
            if (PathPanel.Path == null || !PathPanel.Path.Any()) return Common_en_EN.FirmwareNoPath;
            var isFW2 = PathPanel.Path != null && Path.GetExtension(PathPanel.Path) == ".fw2";
            if (_uploadMode == UploadMode.DEV && isFW2 && CMbFileVersions.SelectedItem == null) return Common_en_EN.FirmwareNoVersionForDev;
            return string.Empty;
        }


        public class FirmwarerVersion
        {
            public string ForShow { get; set; }
            public string FileName { get; set; }
        }

        private void LoadCmbFileVersion()
        {
            if (PathPanel.Path == null || !PathPanel.Path.Any()) return; 
            var z = ZipFile.Read(PathPanel.Path);
            var fileNames = z.EntryFileNames;
            var realNames = new List<FirmwarerVersion>();
            foreach (var fileName in fileNames)
            {
                var extension = Path.GetExtension(fileName);
                if (extension != ".gc") continue;
                var realFileName = Path.GetFileNameWithoutExtension(fileName);
                realNames.Add(new FirmwarerVersion { ForShow = realFileName, FileName = fileName });
            }

            CMbFileVersions.DisplayMemberPath = "ForShow";
            CMbFileVersions.SelectedValuePath = "FileName";

            var selectedItem = CMbFileVersions.SelectedItem;
            CMbFileVersions.ItemsSource = null;
            CMbFileVersions.ItemsSource = realNames;
            if (selectedItem != null && realNames.Any() && realNames.Count(a=>a.ForShow.Equals(selectedItem as string)) > 0) CMbPorts.SelectedItem = selectedItem;
        }

        private void PathPanel_OnOnPathChoosen(object sender, EventArgs e)
        {
            _settingsManager.FirmwareDirectory = PathPanel.InitialDirectory;

            if (_isDevMode)
            {
                LoadCmbFileVersion();
            }

        }

        private void ButtonClose_OnClick(object sender, RoutedEventArgs e)
        {
            GetWindow(this).DialogResult = true;
            GetWindow(this).Close();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (count++ == 0) _timer.Start();
            if (count == 6)
            {
                TextBlockError.Text = "";

                _isDevMode = !_isDevMode;

                if (_isDevMode)
                {
                    TextBlockFirmwareVersion.Visibility = Visibility.Visible;
                    CMbFileVersions.Visibility = Visibility.Visible;
                    BtnDev.Visibility = Visibility.Visible;
                    Pusheeen.Visibility = Visibility.Visible;

                    LoadCmbFileVersion();
                }
                else
                {
                    TextBlockFirmwareVersion.Visibility = Visibility.Hidden;
                    CMbFileVersions.Visibility = Visibility.Hidden;
                    BtnDev.Visibility = Visibility.Hidden;
                    Pusheeen.Visibility = Visibility.Hidden;
                }
                _timer.Stop();
                count = 0;
            }
        }
    }
}
