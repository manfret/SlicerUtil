using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using Aura.Themes.Localization;
using Prism.Commands;
using Settings.Memento;
using Settings.Stores;

namespace Aura.ViewModels
{
    public class ConflictCollectionVM : INotifyPropertyChanged, IDialogResultVMHelper
    {
        public ObservableCollection<ConflictsVM> VMs { get; private set; }
        public bool Empty => VMs == null || !VMs.Any();

        private readonly Dictionary<Type, ConflictsVM> _dictionaryVMs;

        public DelegateCommand SetFillOldFromNewToAllCommand { get; private set; }
        public DelegateCommand SetSaveNewToAllCommand { get; private set; }
        public DelegateCommand SubmitCommand { get; private set; }
        public DelegateCommand CancelCommand { get; private set; }

        public ConflictCollectionVM()
        {
            SetFillOldFromNewToAllCommand = new DelegateCommand(SetConflictModeToFillOldFromNewExecuteMethod, () => true);
            SetSaveNewToAllCommand = new DelegateCommand(SetConflictModeToSaveNewExecuteMethod, () => true);
            SubmitCommand = new DelegateCommand(CloseDialogOk);
            CancelCommand = new DelegateCommand(CloseDialogCancel);
            VMs = new ObservableCollection<ConflictsVM>();
            _dictionaryVMs = new Dictionary<Type, ConflictsVM>();
        }

        #region SET CONFLICT MODE

        private void SetConflictModeToFillOldFromNewExecuteMethod()
        {
            SetConflictModeExecuteMethod(ConflictMode.FILL_OLD_FROM_NEW);
        }

        private void SetConflictModeToSaveNewExecuteMethod()
        {
            SetConflictModeExecuteMethod(ConflictMode.SAVE_NEW);
        }

        private void SetConflictModeExecuteMethod(ConflictMode conflictMode)
        {
            foreach (var conflictsVM in VMs)
            {
                conflictsVM.SetConflictModeExecuteMethod(conflictMode);
            }
        }

        #endregion

        #region INITIALIZE

        public void Initialize(IEnumerable<ISettingsMemento> items)
        {
            var lst = items.ToList();
            if (!lst.Any()) return;

            var plastics = lst.OfType<IMaterialPMemento>();
            Initialize(plastics);
            var fibers = lst.OfType<IMaterialFMemento>();
            Initialize(fibers);
            var printers = lst.OfType<IPrinterMemento>();
            Initialize(printers);
            var profiles = lst.OfType<IProfileMemento>();
            Initialize(profiles);
        }

        public void Initialize(IEnumerable<IMaterialPMemento> items)
        {
            var list = items.ToList();
            if (!list.Any()) return;
            var header = ConflictDialog_en_EN.Plastics;
            var vm = new ConflictsVM();
            vm.Initialize(list, header);
            VMs.Add(vm);
            _dictionaryVMs.Add(typeof(IMaterialPMemento), vm);
        }

        public void Initialize(IEnumerable<IMaterialFMemento> items)
        {
            var list = items.ToList();
            if (!list.Any()) return;
            var header = ConflictDialog_en_EN.Fibers;
            var vm = new ConflictsVM();
            vm.Initialize(list, header);
            VMs.Add(vm);
            _dictionaryVMs.Add(typeof(IMaterialFMemento), vm);
        }

        public void Initialize(IEnumerable<IPrinterMemento> items)
        {
            var list = items.ToList();
            if (!list.Any()) return;
            var header = ConflictDialog_en_EN.Printers;
            var vm = new ConflictsVM();
            vm.Initialize(list, header);
            VMs.Add(vm);
            _dictionaryVMs.Add(typeof(IPrinterMemento), vm);
        }

        public void Initialize(IEnumerable<IProfileMemento> items)
        {
            var list = items.ToList();
            if (!list.Any()) return;
            var header = ConflictDialog_en_EN.Profiles;
            var vm = new ConflictsVM();
            vm.Initialize(list, header);
            VMs.Add(vm);
            _dictionaryVMs.Add(typeof(IProfileMemento), vm);
        }

        #endregion

        #region GET CONFLICTS

        public Dictionary<T, ConflictMode> GetConflicts<T>() where T : ISettingsMemento
        {
            var res = new Dictionary<T, ConflictMode>();
            foreach (var item in _dictionaryVMs[typeof(T)].Conflicts)
            {
                res.Add((T)item.Essence, item.ConflictMode);
            }
            return res;
        }

        #endregion

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

        #region CLOSE DIALOG

        public event EventHandler<RequestCloseDialogEventArgs> RequestCloseDialog;

        public void CloseDialogOk()
        {
            CloseDialog(true);
        }

        public void CloseDialogCancel()
        {
            CloseDialog(false);
        }

        public void CloseDialog(bool isOk)
        {
            var args = new RequestCloseDialogEventArgs(isOk);
            RequestCloseDialog?.Invoke(this, args);
        }

        #endregion
    }

    #region CONFLICT MODE

    [ValueConversion(typeof(bool), typeof(ConflictMode))]
    public class ConflictModeToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (ConflictMode)value;
            switch (val)
            {
                case ConflictMode.FILL_OLD_FROM_NEW:
                    return true;
                case ConflictMode.SAVE_NEW:
                    return false;
            }

            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (bool)value;
            return val ? ConflictMode.FILL_OLD_FROM_NEW : ConflictMode.SAVE_NEW;
        }
    }

    [ValueConversion(typeof(bool), typeof(ConflictMode))]
    public class ConflictModeEqualityToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (ConflictMode)value;
            var par = (ConflictMode)parameter;

            return val == par;

            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (bool)value;
            var par = (ConflictMode)parameter;
            var notPar = par == ConflictMode.FILL_OLD_FROM_NEW ? ConflictMode.SAVE_NEW : ConflictMode.FILL_OLD_FROM_NEW;
            return val ? par : notPar;
        }
    }

    #endregion

    #region CONFLICT VM

    public class ConflictsVM : INotifyPropertyChanged
    {
        #region CONFLICTS

        private ObservableCollection<EssenseConflict> _conflicts;

        public ObservableCollection<EssenseConflict> Conflicts
        {
            get => _conflicts;
            private set
            {
                _conflicts = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region HEADER

        private string _header;
        public string Header
        {
            get => _header;
            set
            {
                _header = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region SET CONFLICT MODE EXECUTE METHOD

        public void SetConflictModeExecuteMethod(ConflictMode conflictMode)
        {
            foreach (var conflict in Conflicts)
            {
                conflict.ConflictMode = conflictMode;
            }
        }

        #endregion

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

        #region INITIALIZE

        public void Initialize(IEnumerable<ISettingsMemento> items, string header)
        {
            var lst = new ObservableCollection<EssenseConflict>();
            foreach (var item in items)
            {
                var pc = new EssenseConflict { ConflictMode = ConflictMode.SAVE_NEW, Essence = item };
                lst.Add(pc);
            }
            Conflicts = lst;
            Header = header;
        }

        #endregion
    }


    #endregion

    public class EssenseConflict : INotifyPropertyChanged
    {
        #region ESSENCE

        private ISettingsMemento _essence;

        public ISettingsMemento Essence
        {
            get => _essence;
            set
            {
                _essence = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region CONFLICT MODE

        private ConflictMode _conflictMode;

        public ConflictMode ConflictMode
        {
            get => _conflictMode;
            set
            {
                _conflictMode = value;
                OnPropertyChanged();
            }
        }

        #endregion

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