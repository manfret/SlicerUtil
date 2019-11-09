using Aura.Managers;
using Unity;
using Unity.Extension;
using Unity.Lifetime;
using Util;

namespace Aura.Extensions
{
    public class GUIExtension : UnityContainerExtension
    {
        private static ISettingsManager _settingsManager;
        private static IAuraViewportManager _auraViewportManager;
        private static ICodeViewerManager _codeViewerManager;
        private static ILayupViewerManager _layupViewerManager;
        private static ISlicingManager _slicingManager;

        public GUIExtension()
        {
            _settingsManager = new SettingsManager();
            _auraViewportManager = new AuraViewportManager();
            _codeViewerManager = new CodeViewerManager();
            _layupViewerManager = new LayupViewerManager(_auraViewportManager);
            _slicingManager = new SlicingManager();
        }

        protected override void Initialize()
        {
            var container = UnityCore.Container;

            container.RegisterInstance(_settingsManager, new ContainerControlledLifetimeManager());
            container.RegisterInstance(_auraViewportManager, new ContainerControlledLifetimeManager());
            container.RegisterInstance(_codeViewerManager, new ContainerControlledLifetimeManager());
            container.RegisterInstance(_layupViewerManager, new ContainerControlledLifetimeManager());
            container.RegisterInstance(_slicingManager, new ContainerControlledLifetimeManager());
        }
    }
}