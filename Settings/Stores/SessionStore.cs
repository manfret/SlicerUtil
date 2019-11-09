using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Newtonsoft.Json;
using Settings.Memento;
using Settings.Session;

namespace Settings.Stores
{
    /// <summary>
    /// Интерфейс магазина сессий
    /// </summary>
    public interface ISessionStore
    {
        event EventHandler SessionChanged;

        #region SESSION

        /// <summary>
        /// Текущая сессия
        /// </summary>
        ISessionMemento Session { get; set; }

        #endregion

        #region EXPORT ALL

        /// <summary>
        /// Экспортирует настройки всех сессий в файл 
        /// </summary>
        /// <param name="path">Путь к файлу экспортирования</param>
        void ExportAll(string path);

        #endregion

        #region IMPORT

        /// <summary>
        /// Импортирует настройки сессий из файла (дубликаты не добавляются)
        /// </summary>
        /// <param name="path"></param>
        void Import(string path);

        #endregion

        bool CheckExtruder(IExtruderPMemento extruder);
        bool CheckExtruder(IExtruderPFMemento extruder);

        #region CHECK ITEM

        bool CheckItem<T>(T item) where T : ISettingsMemento;

        #endregion
    }

    /// <summary>
    /// Базовая реализация магазина сессий
    /// </summary>
    public sealed class SessionStore : ISessionStore
    {
        public event EventHandler SessionChanged;

        #region SESSION

        private ISessionMemento _session;

        /// <summary>
        /// Текущая сессия
        /// </summary>
        public ISessionMemento Session
        {
            get => _session;
            set
            {
                _session = value;

                SyncronizeSession(ref _session);

                if (_session != null)
                {
                    Session.PropertyChanged += SessionOnPropertyChanged;
                    Session.PropertyChanging += SessionOnPropertyChanging;
                }

                _settingFileSerializer.SaveMemento(Session);

                SessionChanged?.Invoke(this, null);
            }
        }

        #endregion

        private readonly ISettingFileSerializer _settingFileSerializer;
        private readonly ISettingsFactory _factory;

        private readonly ISettingsStore<IMaterialPMemento> _plasticStore;
        private readonly ISettingsStore<IMaterialFMemento> _fiberStore;
        private readonly ISettingsStore<IPrinterMemento> _printerStore;
        private readonly ISettingsStore<IProfileMemento> _profileStore;

        private readonly ISessionHelperWrapper _sessionHelperWrapper;

        #region CTOR

        public SessionStore(ISettingFileSerializer settingFileSerializer,
            ISettingsStore<IMaterialPMemento> plasticStore,
            ISettingsStore<IMaterialFMemento> fiberStore,
            ISettingsStore<IPrinterMemento> printerStore,
            ISettingsStore<IProfileMemento> profileSTore, 
            ISettingsFactory factory,
            ISessionHelperWrapper sessionHelperWrapper)
        {
            _settingFileSerializer = settingFileSerializer;
            _plasticStore = plasticStore;
            _fiberStore = fiberStore;
            _printerStore = printerStore;
            _profileStore = profileSTore;
            _factory = factory;
            _sessionHelperWrapper = sessionHelperWrapper;

            #region SESSION

            //загружаем дефолтную сессию
            Session = _settingFileSerializer.GetLastSession();

            #endregion
        }

        //конструктор для тестов
        public SessionStore(ISessionMemento sessionMemento)
        {
            _session = sessionMemento;
        }

        private void SyncronizeSession(ref ISessionMemento nonSyncronizedSession)
        {
            if (nonSyncronizedSession == null)
            {
                nonSyncronizedSession = _factory.CreateSessionMemento();
                nonSyncronizedSession.Name = "Default";
            }

            _sessionHelperWrapper.SyncronizePrinter(nonSyncronizedSession, _printerStore, _factory);
            _sessionHelperWrapper.SyncronizeMaterials(nonSyncronizedSession, _plasticStore, _fiberStore, _factory);
            _sessionHelperWrapper.SyncronizeProfile(nonSyncronizedSession, _profileStore, _factory);
        }


        #endregion

        #region SESSION ON PROPERTY CHANGED

        [ExcludeFromCodeCoverage]
        private void SessionOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var memento = (ISessionMemento)sender;
            if (e.PropertyName == "ExtrudersPMaterials" || e.PropertyName == "ExtrudersPFMaterials")
            {
                //обновляем информацию об экструдерах и их материалах
                SessionHelper.SyncronizeMaterials(Session, _plasticStore, _fiberStore, _factory);
            }
            //сохраняем мементо
            _settingFileSerializer.SaveMemento(memento);
        }

        #endregion

        #region SESSION ON PROPERTY CHANGING

        [ExcludeFromCodeCoverage]
        private void SessionOnPropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            var memento = (ISettingsMemento) sender;
            _settingFileSerializer.RemoveMemento(memento);
        }

        #endregion

        #region EXPORT ALL

        /// <summary>
        /// Экспортирует настройки всех сессий в файл 
        /// </summary>
        /// <param name="path">Путь к файлу экспортирования</param>
        [ExcludeFromCodeCoverage]
        public void ExportAll(string path)
        {
            _settingFileSerializer.ExportMementos(new List<ISessionMemento> {Session}, path);
        }

        #endregion

        #region IMPORT

        /// <summary>
        /// Импортирует настройки сессий из файла (дубликаты не добавляются)
        /// </summary>
        /// <param name="path"></param>
        public void Import(string path)
        {
            ISessionMemento memento = null;
            try
            {
                memento = _settingFileSerializer.ImportSession(path);
            }
            catch (JsonSerializationException e)
            {
                throw;
            }

            if (memento == null) return;

            Session = memento;
        }

        #endregion

        #region CHECK ITEM

        public bool CheckItem<T>(T item) where T : ISettingsMemento
        {
            if (item is IMaterialPMemento mpm) return CheckItem(mpm);
            if (item is IMaterialFMemento mfm) return CheckItem(mfm);
            if (item is IPrinterMemento pm) return CheckItem(pm);
            if (item is IProfileMemento pfm) return CheckItem(pfm);
            //для неизвестных типов неизвестен результат
            throw new NotImplementedException();
        }

        private bool CheckItem(IMaterialPMemento item)
        {
            if (Session?.ExtrudersPMaterials.Any() == true)
            {
                var isInP = Session.ExtrudersPMaterials.Where(a => Equals(a.Material, item)).ToList();
                if (isInP.Any()) return true;
            }
            if (Session?.ExtrudersPFMaterials.Any() == true)
            {
                var isInPF = Session.ExtrudersPFMaterials.Where(a => Equals(a.MaterialP, item)).ToList();
                if (isInPF.Any()) return true;
            }
            return false;
        }

        private bool CheckItem(IMaterialFMemento item)
        {
            if (Session?.ExtrudersPFMaterials.Any() == true)
            {
                var isInPF = Session.ExtrudersPFMaterials.Where(a => object.Equals(item, a.MaterialF)).ToList();
                if (isInPF.Any()) return true;
            }
            return false;
        }

        private bool CheckItem(IPrinterMemento item)
        {
            if (Session?.Printer == null) return false;
            return Session.Printer.Equals(item);
        }

        private bool CheckItem(IProfileMemento item)
        {
            if (Session?.Profile == null) return false;
            return Session.Profile.Equals(item);;
        }

        #endregion

        #region CHECK EXTRUDERS

        public bool CheckExtruder(IExtruderPMemento extruder)
        {
            if (Session.BrimExtruder == extruder.ExtruderIndex) return true;
            if (Session.SkirtExtruder == extruder.ExtruderIndex) return true;
            if (Session.Inset0Extruder == extruder.ExtruderIndex) return true;
            if (Session.InsetPlasticExtruder == extruder.ExtruderIndex) return true;
            if (Session.InfillSolidExtruder == extruder.ExtruderIndex) return true;
            if (Session.InfillPlasticCellularExtruder == extruder.ExtruderIndex) return true;
            if (Session.SupportExtruder == extruder.ExtruderIndex) return true;
            return false;
        }

        public bool CheckExtruder(IExtruderPFMemento extruder)
        {
            return Session.InsetFiberExtruder.HasValue && Session.InsetFiberExtruder.Value == extruder.ExtruderIndex;
        }

        #endregion
    }
}