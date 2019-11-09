using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Prism.Commands;
using Settings.Memento;
using Settings.Stores;

namespace Aura.ViewModels
{
    public class ExtrudersVM : INotifyPropertyChanged
    {
        private ISessionStore _sessionStore;
        private IPrinterMemento _printerMemento;

        public DelegateCommand AddPlasticCommand { get; private set; }
        public DelegateCommand AddCompositeCommand { get; private set; }

        public ObservableCollection<ExtruderVM> ExtrudersVMs { get; private set; }

        #region SELECTED ITEM

        private object _selectedItem;

        public object SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public ExtrudersVM()
        {
            ExtrudersVMs = new ObservableCollection<ExtruderVM>();
            AddPlasticCommand = new DelegateCommand(() => _printerMemento.AddExtruderP(), () => true);
            AddCompositeCommand = new DelegateCommand(() => _printerMemento.AddExtruderPF(), () => true);
        }

        public void Initialize(ISessionStore sessionStore,
            IPrinterMemento printerMemento)
        {
            _sessionStore = sessionStore;
            _printerMemento = printerMemento;

            //для ускорения сначала просто все заполним, а потом подпишемся на события
            FillExtruders();
            _printerMemento.ExtrudersP.CollectionChanged += ExtrudersOnCollectionChanged;
            _printerMemento.ExtrudersPF.CollectionChanged += ExtrudersOnCollectionChanged;
        }

        private ExtruderPlasticVM GetExtruderPByIndex(int index)
        {
            var vms = ExtrudersVMs.OfType<ExtruderPlasticVM>();
            if (vms == null || !vms.Any()) return null;
            var item = vms.SingleOrDefault(a => a.Extruder.TrueExtruderIndex == index);
            return item;
        }

        private ExtruderFiberVM GetExtruderFByIndex(int index)
        {
            var vms = ExtrudersVMs.OfType<ExtruderFiberVM>();
            if (vms == null || !vms.Any()) return null;
            var item = vms.SingleOrDefault(a => a.Extruder.TrueExtruderIndex == index);
            return item;
        }

        private void ExtrudersOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var siP = (SelectedItem as ExtruderPlasticVM)?.Extruder.ExtruderIndex;
            var siF = (SelectedItem as ExtruderFiberVM)?.Extruder.ExtruderIndex;
            this.ExtrudersVMs.Clear();
            FillExtruders();
            if (siP != null) SelectedItem = GetExtruderPByIndex(siP.Value);
            if (siF != null) SelectedItem = GetExtruderFByIndex(siF.Value);
        }

        #region FILL EXTRUDERS

        private void FillExtruders()
        {
            var extrudersP = new List<ExtruderPlasticVM>();
            var extrudersPF = new List<ExtruderFiberVM>();

            if (_printerMemento.ExtrudersP != null)
            {
                foreach (var extruderPMemento in _printerMemento.ExtrudersP)
                {
                    var vm = new ExtruderPlasticVM();
                    vm.Initialize(_sessionStore, _printerMemento, extruderPMemento);
                    extrudersP.Add(vm);
                }
            }

            if (_printerMemento.ExtrudersPF != null)
            {
                foreach (var extruderPfMemento in _printerMemento.ExtrudersPF)
                {
                    var vm = new ExtruderFiberVM();
                    vm.Initialize(_sessionStore, _printerMemento, extruderPfMemento);
                    extrudersPF.Add(vm);
                }
            }

            extrudersP = extrudersP.OrderBy(a => a.Extruder.TrueExtruderIndex).ToList();
            extrudersPF = extrudersPF.OrderBy(a => a.Extruder.TrueExtruderIndex).ToList();

            ExtrudersVMs.AddRange(extrudersP);
            ExtrudersVMs.AddRange(extrudersPF);
        }

        #endregion

        #region ON PROPERTY CHANGED

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string caller = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(caller));
        }

        #endregion
    }

    public abstract class ExtruderVM : INotifyPropertyChanged
    {
        protected ISessionStore SessionStore;

        #region PRINTER

        private IPrinterMemento _printer;

        public IPrinterMemento Printer
        {
            get => _printer;
            set
            {
                _printer = value;
                _printer.ExtrudersP.CollectionChanged += (sender, args) => RemoveExtruderCommand.RaiseCanExecuteChanged();
                _printer.ExtrudersPF.CollectionChanged += (sender, args) => RemoveExtruderCommand.RaiseCanExecuteChanged();
                OnPropertyChanged();
            }
        }

        #endregion

        public DelegateCommand RemoveExtruderCommand { get; private set; }

        protected ExtruderVM()
        {
            RemoveExtruderCommand = new DelegateCommand(RemoveExtruderExecuteMethod, RemoveExtruderCanExecuteMethod);
        }

        #region REMOVE COMMAND

        private bool RemoveExtruderCanExecuteMethod()
        {
            var plasticExtrudersCount = _printer.ExtrudersP?.Count() ?? 0;
            var fiberExtrudersCount = _printer.ExtrudersPF?.Count() ?? 0;
            if ((plasticExtrudersCount + fiberExtrudersCount) < 2) return false;
            return !CheckExtruder();
        }

        protected abstract bool CheckExtruder();
        protected abstract void RemoveExtruderExecuteMethod();

        #endregion

        #region ON PROPERTY CHANGED

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string caller = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(caller));
        }

        #endregion

        #region INITIALIZE

        protected void Initialize(ISessionStore sessionStore, IPrinterMemento printer)
        {
            SessionStore = sessionStore;
            SessionStore.Session.PropertyChanged += (sender, args) => RemoveExtruderCommand.RaiseCanExecuteChanged();
            Printer = printer;
        }

        #endregion
    }

    public class ExtruderPlasticVM : ExtruderVM
    {
        #region EXTRUDER

        private IExtruderPMemento _extruder;

        public IExtruderPMemento Extruder
        {
            get => _extruder;
            set
            {
                _extruder = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region CHECK EXTRUDERS

        protected override bool CheckExtruder()
        {
            if (!SessionStore.Session.Printer.Equals(Printer)) return false;
            return SessionStore.CheckExtruder(Extruder);
        }

        #endregion

        #region REMOVE EXTRUDER

        protected override void RemoveExtruderExecuteMethod()
        {
            Printer.RemoveExtruder(Extruder);
        }

        #endregion

        #region INITIALIZE

        public void Initialize(ISessionStore sessionStore, IPrinterMemento printer, IExtruderPMemento extruderPMemento)
        {
            base.Initialize(sessionStore, printer);
            Extruder = extruderPMemento;
        }

        #endregion
    }

    public class ExtruderFiberVM : ExtruderVM
    {
        #region EXTRUDER

        private IExtruderPFMemento _extruder;

        public IExtruderPFMemento Extruder
        {
            get => _extruder;
            set
            {
                _extruder = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region CHECK EXTRUDERS

        protected override bool CheckExtruder()
        {
            if (!SessionStore.Session.Printer.Equals(Printer)) return false;
            return SessionStore.CheckExtruder(Extruder);
        }

        #endregion

        #region REMOVE EXTRUDER

        protected override void RemoveExtruderExecuteMethod()
        {
            Printer.RemoveExtruder(Extruder);
        }

        #endregion

        #region INITIALIZE

        public void Initialize(ISessionStore sessionStore, IPrinterMemento printer, IExtruderPFMemento extruderPMemento)
        {
            base.Initialize(sessionStore, printer);
            Extruder = extruderPMemento;
        }

        #endregion
    }
}