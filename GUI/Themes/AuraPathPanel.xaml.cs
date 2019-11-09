using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace Aura.Themes
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class AuraPathPanel : UserControl
    {
        public static readonly DependencyProperty FilterProperty =
            DependencyProperty.Register("Filter", typeof(string), typeof(AuraPathPanel));

        public string Filter
        {
            get { return (string) GetValue(FilterProperty); }
            set { SetValue(FilterProperty, value); }
        }

        public static readonly DependencyProperty InitialDirectoryProperty =
            DependencyProperty.Register("InitialDirectory", typeof(string), typeof(AuraPathPanel));

        public string InitialDirectory
        {
            get { return (string) GetValue(InitialDirectoryProperty); }
            set { SetValue(InitialDirectoryProperty, value); }
        }

        public static readonly DependencyProperty PathProperty =
            DependencyProperty.Register("Path", typeof(string), typeof(AuraPathPanel));

        public string Path
        {
            get { return (string) GetValue(PathProperty); }
            set { SetValue(PathProperty, value); }
        }

        public static readonly RoutedEvent OnPathChoosenEvent =
            EventManager.RegisterRoutedEvent("OnPathChoosen", RoutingStrategy.Bubble, typeof(EventHandler), typeof(AuraPathPanel));

        public event EventHandler OnPathChoosen
        {
            add { AddHandler(OnPathChoosenEvent, value); }
            remove { RemoveHandler(OnPathChoosenEvent, value); }
        }

        public AuraPathPanel()
        {
            InitializeComponent();
        }

        private void ButtonPath_click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = Filter,
                InitialDirectory = InitialDirectory
            };

            if (openFileDialog.ShowDialog() == true)
            {
                Path = openFileDialog.FileName;
                TxtBoxFileName.Text = Path;
                InitialDirectory = System.IO.Path.GetDirectoryName(openFileDialog.FileName);
                RaiseEvent(new RoutedEventArgs(OnPathChoosenEvent));
            }
        }
    }
}
