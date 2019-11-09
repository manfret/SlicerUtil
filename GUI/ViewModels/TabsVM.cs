using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Aura.ViewModels
{
    public class TabsVM : INotifyPropertyChanged
    {
        public ObservableCollection<TabItem> Tabs { get; set; }

        private TabItem _selectedItem;

        public TabItem SelectedTabItem
        {
            get => _selectedItem;
            set
            {
                if (_selectedItem?.VM is ModelsCollectionVM modelsCollectionVM)
                {
                    modelsCollectionVM.OnUnload();
                }
                _selectedItem = value;
                if (_selectedItem?.VM is SendReportVM sendReportVM)
                {
                    sendReportVM.OnLoad();
                }
                OnPropertyChanged();
            }
        }

        public TabsVM()
        {
            Tabs = new ObservableCollection<TabItem>();
        }

        #region ON PROPERTY CHANGED

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string caller = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(caller));
        }

        #endregion
    }

    public class TabItem : INotifyPropertyChanged
    {
        public INotifyPropertyChanged VM { get; set; }
        public string Name { get; set; }

        #region ON PROPERTY CHANGED

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string caller = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(caller));
        }

        #endregion
    }
}