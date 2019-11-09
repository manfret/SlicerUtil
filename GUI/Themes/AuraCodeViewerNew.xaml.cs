using System.Windows.Controls;
using Aura.Controls.Util;
using Aura.Managers;
using Aura.ViewModels;
using Unity;
using Util;

namespace Aura.Views
{
    /// <summary>
    /// Interaction logic for AuraCodeViewerNew.xaml
    /// </summary>
    public partial class AuraCodeViewerNew : UserControl
    {
        protected CodeViewerVM Model
        {
            get { return (CodeViewerVM)Resources["ViewModel"]; }
        }

        public AuraCodeViewerNew()
        {
            InitializeComponent();
            var settingsManager = UnityCore.Container.Resolve<ISettingsManager>();
            var codeViewerManager = UnityCore.Container.Resolve<ICodeViewerManager>();
            var auraViewportManager = UnityCore.Container.Resolve<IAuraViewportManager>();
            Model.Initialize(settingsManager, codeViewerManager, auraViewportManager);
            this.DataContext = Model;
            this.Loaded += (sender, args) => Model.OnLoaded();
            this.Unloaded += (sender, args) => Model.OnUnload();
        }
    }
}
