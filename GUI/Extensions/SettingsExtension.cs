using Settings;
using Settings.Memento;
using Settings.Session;
using Settings.Stores;
using Unity;
using Unity.Extension;
using Unity.Lifetime;
using Util;

namespace Aura.Extensions
{
    public class SettingsExtension : UnityContainerExtension
    {
        private static IExtrusionCalculator _extrusionCalculator;

        //-------------------------------------

        private static ISettingsFactory _settingsFactory;

        private static ISettingsStore<IMaterialPMemento> _plasticStore;
        private static ISettingsStore<IMaterialFMemento> _fiberStore;
        private static ISettingsStore<IPrinterMemento> _printerStore;
        private static ISettingsStore<IProfileMemento> _profileStore;

        private static ISessionStore _sessionStore;
        public SettingsExtension()
        {
            var settingFileSerializer = new SettingsFileSerializer();
            _settingsFactory = new SettingsFactory();
            _plasticStore = new SettingsStore<IMaterialPMemento>(settingFileSerializer);
            _fiberStore = new SettingsStore<IMaterialFMemento>(settingFileSerializer);
            _printerStore = new SettingsStore<IPrinterMemento>(settingFileSerializer);
            _profileStore = new SettingsStore<IProfileMemento>(settingFileSerializer);
            _sessionStore = new SessionStore(settingFileSerializer, _plasticStore, _fiberStore, _printerStore, _profileStore, _settingsFactory, new SessionHelperWrapper());

            _extrusionCalculator = new ExtrusionCalculator();
        }

        protected override void Initialize()
        {
            var container = UnityCore.Container;

            container.RegisterInstance(_settingsFactory, new ContainerControlledLifetimeManager());
            container.RegisterInstance(_plasticStore, new ContainerControlledLifetimeManager());
            container.RegisterInstance(_fiberStore, new ContainerControlledLifetimeManager());
            container.RegisterInstance(_printerStore, new ContainerControlledLifetimeManager());
            container.RegisterInstance(_profileStore, new ContainerControlledLifetimeManager());
            container.RegisterInstance(_sessionStore, new ContainerControlledLifetimeManager());

            ////---------

            container.RegisterInstance(_extrusionCalculator, new ContainerControlledLifetimeManager());
        }
    }
}