using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Aura.ViewModels
{
    public abstract class CollectionViewModel<TItem> : INotifyPropertyChanged
        where TItem : INotifyPropertyChanged
    {

        public CollectionViewModel()
        {
            Items = new ObservableCollection<TItem>();
        }

        public ObservableCollection<TItem> Items { get; }

        #region ON PROPERTY CHANGED

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string caller = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(caller));
        }

        #endregion
    }
}