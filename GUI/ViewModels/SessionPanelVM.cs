using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Aura.Managers;
using Prism.Commands;
using Settings;
using Settings.Memento;
using Settings.Stores;

namespace Aura.ViewModels
{
    public class SessionPanelVM : INotifyPropertyChanged
    {
        private ISessionStore _sessionStore;
        private ISettingsStore<IPrinterMemento> _printerStore;
        private IAuraViewportManager _auraViewportManager;
        private ModelWithDataCollectionVM _modelWithDataCollectionVM;

        private GenerationParameters _generationParameters;

        #region SESSION

        private ISessionMemento _session;
        public ISessionMemento Session
        {
            get => _session;
            private set
            {
                _session = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region VMS

        private SettingsCollectionPlasticVM _settingsCollectionPlasticVM;
        public SettingsCollectionPlasticVM SettingsCollectionPlasticVM
        {
            get => _settingsCollectionPlasticVM;
            private set
            {
                _settingsCollectionPlasticVM = value;
                OnPropertyChanged();
            }
        }

        private SettingsCollectionFiberVM _settingsCollectionFiberVM;
        public SettingsCollectionFiberVM SettingsCollectionFiberVM
        {
            get => _settingsCollectionFiberVM;
            private set
            {
                _settingsCollectionFiberVM = value;
                OnPropertyChanged();
            }
        }

        private SettingsCollectionPrinterVM _settingsCollectionPrinterVM;
        public SettingsCollectionPrinterVM SettingsCollectionPrinterVM
        {
            get => _settingsCollectionPrinterVM;
            private set
            {
                _settingsCollectionPrinterVM = value;
                OnPropertyChanged();
            }
        }

        private SettingsCollectionProfileVM _settingsCollectionProfileVM;
        public SettingsCollectionProfileVM SettingsCollectionProfileVM
        {
            get => _settingsCollectionProfileVM;
            set
            {
                _settingsCollectionProfileVM = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public DelegateCommand GenerateCommand;
        public event EventHandler Generate;

        public SessionPanelVM()
        {
            GenerateCommand = new DelegateCommand(GenerateExecuteMethod, CanExecuteMethod);
        }

        private bool CanExecuteMethod()
        {
            if (_modelWithDataCollectionVM.HasInteractiveItems) return true;
            return false;
            //добавить что нет ошибок
        }

        private void GenerateExecuteMethod()
        {
            Generate?.Invoke(this, null);
        }

        public struct SessionInitization
        {
            public ISessionStore SessionStore { get; set; }
            public ISettingsStore<IPrinterMemento> PrinterStore { get; set; }
            public IAuraViewportManager AuraViewportManager { get; set; }
            public SettingsCollectionPlasticVM SettingsCollectionPlasticVM { get; set; }
            public SettingsCollectionFiberVM SettingsCollectionFiberVM { get; set; }
            public SettingsCollectionPrinterVM SettingsCollectionPrinterVM { get; set; }
            public SettingsCollectionProfileVM SettingsCollectionProfileVM { get; set; }
            public GenerationParameters GenerationParameters { get; set; }
        }

        public void Initialize(SessionInitization sessionInitizer)
        {
            _sessionStore = sessionInitizer.SessionStore;
            _sessionStore.SessionChanged += (sender, args) => Session = _sessionStore.Session;
            Session = _sessionStore.Session;
            _printerStore = sessionInitizer.PrinterStore;
            _generationParameters = sessionInitizer.GenerationParameters;
            _auraViewportManager = sessionInitizer.AuraViewportManager;
            this.SettingsCollectionPlasticVM = sessionInitizer.SettingsCollectionPlasticVM;
            this.SettingsCollectionFiberVM = sessionInitizer.SettingsCollectionFiberVM;
            this.SettingsCollectionPrinterVM = sessionInitizer.SettingsCollectionPrinterVM;
            this.SettingsCollectionProfileVM = sessionInitizer.SettingsCollectionProfileVM;
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