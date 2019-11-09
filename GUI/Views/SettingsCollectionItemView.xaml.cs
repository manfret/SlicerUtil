using System.Windows;
using System.Windows.Controls;
using Aura.Controls;
using Aura.ViewModels;

namespace Aura.Views
{
    /// <summary>
    /// Interaction logic for SettingsCollectionItemView.xaml
    /// </summary>
    public partial class SettingsCollectionItemView : SelectableUserControl
    {
        public SettingsCollectionItemView()
        {
            InitializeComponent();
        }

        private void PART_Expander_OnExpanded(object sender, RoutedEventArgs e)
        {
            if (!e.OriginalSource.Equals(sender)) return;
            UIElement view = null;

            if (DataContext is SettingsItemVMPlastic) view = new SettingsItemPlasticView { DataContext = this.DataContext };
            if (DataContext is SettingsItemVMFiber) view = new SettingsItemFiberView { DataContext = this.DataContext };
            if (DataContext is SettingsItemVMPrinter) view = new SettingsItemPrinterView { DataContext = this.DataContext };
            if (DataContext is SettingsItemVMProfile)  view = new SettingsItemProfileView { DataContext = this.DataContext };

            if (view != null)
            {
                var expander = (Expander)sender;
                expander.Content = view;
            }
        }
    }
}
