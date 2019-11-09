using EntityFiller;
using LayersStruct;
using LayersStruct.PPBlocksCollection;
using Unity;
using Unity.Extension;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Registration;
using Util;
using Util.GeometryBasics;
using Util.PolygonOptimizer;

namespace Aura.Extensions
{
    public class UtilExtension : UnityContainerExtension
    {
        private static IInfillHelperWrapper _infillHelperWrapper;
        private static IInsetFiberHelperWrapper _insetFiberHelperWrapper;
        private static IInsetPlasticHelperWrapper _insetPlasticHelper;
        private static IInfillFiberHelperWrapper _infillFiberHelperWrapper;
        private static ISupportHelperWrapper _supportHelperWrapper;
        private readonly IEntityFillerFactory _entityFillerFactory;
        private readonly IEntityFillerHelperWrapper _entityFillerHelper;
        private static ILayerStructFactory _layerStructFactory;
        private static IBlockFactory _blockFactory;
        private static IPolygonOptimizerWrapper _polygonOptimizerWrapper;
        private static IPPBlocksBuilderHelperWrapper _ppBlocksBuilderHelperWrapper;

        public UtilExtension()
        {
            _infillHelperWrapper = new InfillHelperWrapper();
            _insetFiberHelperWrapper = new InsetFiberHelperWrapper();
            _insetPlasticHelper = new InsetPlasticHelperWrapper();
            _infillFiberHelperWrapper = new InfillFiberHelperWrapper();
            _supportHelperWrapper = new SupportHelperWrapper(_insetPlasticHelper, _infillHelperWrapper);
            _layerStructFactory = new LayerStructFactory();
            _blockFactory = new BlockFactory();
            _polygonOptimizerWrapper = new PolygonOptimizerWrapper();
            _ppBlocksBuilderHelperWrapper = new PPBlocksBuilderHelperWrapper();
            _entityFillerFactory = new EntityFillerFactory();

            var helpers = new Helpers(_insetPlasticHelper, _infillHelperWrapper, _insetFiberHelperWrapper, _infillFiberHelperWrapper, _polygonOptimizerWrapper);
            _entityFillerHelper = new EntityFillerHelperWrapper(helpers, _supportHelperWrapper, _layerStructFactory);
        }

        protected override void Initialize()
        {
            var container = UnityCore.Container;

            container.RegisterInstance(_infillHelperWrapper, new ContainerControlledLifetimeManager());
            container.RegisterInstance(_insetFiberHelperWrapper, new ContainerControlledLifetimeManager());
            container.RegisterInstance(_insetPlasticHelper, new ContainerControlledLifetimeManager());
            container.RegisterInstance(_infillFiberHelperWrapper, new ContainerControlledLifetimeManager());
            container.RegisterInstance(_supportHelperWrapper, new ContainerControlledLifetimeManager());
            container.RegisterInstance(_entityFillerFactory, new ContainerControlledLifetimeManager());
            container.RegisterInstance(_entityFillerHelper, new ContainerControlledLifetimeManager());
            container.RegisterInstance(_layerStructFactory, new ContainerControlledLifetimeManager());
            container.RegisterInstance(_blockFactory, new ContainerControlledLifetimeManager());
            container.RegisterInstance(_polygonOptimizerWrapper, new ContainerControlledLifetimeManager());
            container.RegisterInstance(_ppBlocksBuilderHelperWrapper, new ContainerControlledLifetimeManager());
        }
    }
}
