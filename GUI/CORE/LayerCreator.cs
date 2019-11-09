using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFiller;
using LayersStruct;
using LayersStruct.FirstLayer;
using LayersStruct.MacroLayer.MicroLayer;
using LayersStruct.PPBlocksCollection;
using Settings;
using Settings.Memento;
using Settings.Session;
using Slicer;
using Util.GeometryBasics;
using Island = System.Collections.Generic.List<System.Collections.Generic.List<MSClipperLib.IntPoint>>;

namespace Aura.CORE
{
    public class LayerCreator
    {
        private readonly IEntityFillerFactory _entityFillerFactory;
        private readonly ISession _session;
        private readonly Helpers _helpers;
        private readonly ILayerStructFactory _layerStructFactory;
        private readonly ISupportHelperWrapper _supportHelper;
        private readonly IEntityFillerHelperWrapper _entityFillerHelper;
        private readonly IPPBlocksBuilderHelperWrapper _blocksBuilderHelper;

        private readonly IWipeTower _wipeTower;

        public LayerCreator(
            IEntityFillerFactory entityFillerFactory, 
            ISession session, 
            Helpers helpers, 
            ILayerStructFactory layerStructFactory,
            ISupportHelperWrapper supportHelper,
            IEntityFillerHelperWrapper entityFillerHelper,
            IPPBlocksBuilderHelperWrapper blocksBuilderHelper)
        {
            _entityFillerFactory = entityFillerFactory;
            _layerStructFactory = layerStructFactory;
            _session = session;
            _helpers = helpers;
            _supportHelper = supportHelper;
            _entityFillerHelper = entityFillerHelper;
            _blocksBuilderHelper = blocksBuilderHelper;

            if (_session.GlobalSettings.GenerateWipeTower)
            {
                _wipeTower = _layerStructFactory.CreateWipeTower(_session);
            }
        }

        /// <summary>
        /// Главный метод, который на основе геометрической модели создает заполненные сущностями слои
        /// </summary>
        /// <param name="models">Геометрические модели</param>
        /// <param name="fl">Первый слой</param>
        /// <param name="slicedMicroLayers">Оставшиеся слои</param>
        /// <param name="microLayerContours"></param>
        /// <param name="generationData"></param>
        /// <param name="session"></param>
        /// <param name="token"></param>
        public async Task<(IFirstLayer fl, List<IMicroLayer> slicedMicroLayers, List<MicroLayerContours> microLayerContours)> CreateMicroLayersAsync(
            List<ITriangleModel> models,
            GenerationData generationData,
            ISession session,
            CancellationToken token = default(CancellationToken))
        {
            //проверяем на отмену
            token.ThrowIfCancellationRequested();

            var collectionMinZUM = (int)models.Min(a => a.BoxMin.Z);
            var collectionMaxZUM = (int)models.Max(a => a.BoxMax.Z);
            var dictModelAndEntities = new Dictionary<ITriangleModel, List<LayerData>>();
            foreach (var model in models)
            {
                var layerDatas = SlicerHelper.GetLayerDatas(model, _session, collectionMinZUM, collectionMaxZUM);
                dictModelAndEntities.Add(model, layerDatas);
            }

            var uniqueZs = dictModelAndEntities.Select(a => a.Value.Select(b => b.AbsoluteZUM).ToList()).ToList().MakeAHugeMess().Distinct().OrderBy(a => a).ToList();
            var layerZs = new List<(int layerZUM, int layerIndex)>();
            for (var i = 0; i < uniqueZs.Count; i++)
            {
                layerZs.Add((uniqueZs[i], i));
            }

            var smodels = new List<ISlicedModel>();
            var microLayerContours = new List<MicroLayerContours>();
            for (var layerIndex = 0; layerIndex < layerZs.Count; layerIndex++)
            {
                var contoursLayer = new MicroLayerContours
                {
                    Index = layerZs[layerIndex].layerIndex,
                    Islands = new List<Island>(),
                    IslandsShrinked = new List<Island>(),
                    IslandsExpanded = new List<Island>(),
                    AbsoluteZUM = layerZs[layerIndex].layerZUM
                };
                microLayerContours.Add(contoursLayer);
            }

            #region GET MAX TICKS

            int GetMaxValueForGenerations()
            {
                var layersForSlicingAndFilling = 0;
                var globalMinZ = double.MaxValue;
                var globalMaxZ = double.MinValue;

                foreach (var triangleModel in models)
                {
                    var modelMin = triangleModel.BoxMin.Z;
                    if (modelMin < globalMinZ) globalMinZ = modelMin;
                    var modelMax = triangleModel.BoxMax.Z;
                    if (modelMax > globalMaxZ) globalMaxZ = modelMax;

                    //slicing
                    layersForSlicingAndFilling += 10;
                    //support
                    if (session.SupportGlobalSettings.Generate)
                    {
                        layersForSlicingAndFilling++;
                    }
                    //inset0 + Micro
                    layersForSlicingAndFilling++;
                    //insetXPFirstPart
                    layersForSlicingAndFilling++;
                    //SIA
                    layersForSlicingAndFilling++;
                    //insetXF
                    if (session.InsetFiberSettings.GenerateFiberPerimeters)
                    {
                        //InsetXF
                        layersForSlicingAndFilling++;
                        //InsetXPSecondPart
                        layersForSlicingAndFilling++;
                        //SolidInfillUpAndBottom
                        layersForSlicingAndFilling++;
                    }
                    //merge
                    layersForSlicingAndFilling++;
                    //solidInfill
                    layersForSlicingAndFilling++;
                    //cellular
                    layersForSlicingAndFilling++;
                }

                //prepare
                layersForSlicingAndFilling += 10;

                //generate 
                layersForSlicingAndFilling += 10;

                return layersForSlicingAndFilling;
            }
            generationData.MaxTicks = GetMaxValueForGenerations();

            #endregion

            var ewumShrinked = _session.Inset0PlasticSettings.EWUM + (int)(_session.InsetXPlasticSettings.EWUM * 0.45);
            var ewum25 = (int)(2.5 * _session.Inset0PlasticSettings.EWUM);
            //для всех моделей-сущностей
            foreach (var model in models)
            {
                var smls = new List<ISlicedMicroLayer>();
                //по всем высотам
                foreach (var layerZ in layerZs)
                {
                    if (layerZ.layerZUM < model.BoxMin.Z || layerZ.layerZUM > model.BoxMax.Z)
                    {
                        var smlByond = _entityFillerFactory.CreateMicroLayer(layerZ.layerZUM, layerZ.layerIndex, LayerEntity.NONE, _layerStructFactory, _session, _helpers, _entityFillerHelper);
                        smls.Add(smlByond);
                    }
                }

                var modelEntities = dictModelAndEntities[model];
                var modelZUms = modelEntities.Select(a => a.AbsoluteZUM).Where(a => a >= model.BoxMin.Z && a <= model.BoxMax.Z).OrderBy(a => a).ToList();

                var meshLayers = SlicerHelper.SliceModel(model, modelZUms, generationData);
                foreach (var meshLayer in meshLayers)
                {
                    var le = modelEntities.Single(a => a.AbsoluteZUM == meshLayer.Z);
                    var layedIndex = layerZs.Single(a => a.layerZUM == meshLayer.Z).layerIndex;
                    var sml = _entityFillerFactory.CreateMicroLayer((int)meshLayer.Z, layedIndex, le.LayerEntity, _layerStructFactory, session, _helpers, _entityFillerHelper);
                    sml.CreateRawIslands(meshLayer.PolygonList.ProcessIntoSeparateIsland(), _entityFillerFactory);
                    smls.Add(sml);
                }

                //ожидаем окончания выполнения всех тасков - полученный результат отсортировываем по индексу
                //накапливаем в контуры все контуры модели для соответсвующего глобального слоя
                smls = smls.OrderBy(a => a.LayerIndex).ToList();
                foreach (var sml in smls)
                {
                    if (sml.AllArea == null || !sml.AllArea.Any()) continue;
                    var mlContours = microLayerContours.SingleOrDefault(a => a.AbsoluteZUM == sml.AbsoluteZUM);
                    mlContours.Islands.AddRange(sml.AllArea);
                    foreach (var area in sml.AllArea)
                    {
                        var expanded = area.Offset(ewum25).ProcessIntoSeparateIsland();
                        if (expanded.Any()) mlContours.IslandsExpanded.AddRange(expanded);
                        var shrinked = area.Offset(-ewumShrinked).ProcessIntoSeparateIsland();
                        if (shrinked.Any()) mlContours.IslandsShrinked.AddRange(shrinked);
                    }
                }

                if (smls.Any())
                {
                    //создаем модель, передавая ей созданные ранее микросло
                    var smodel = new SlicedModel(smls, _session, _entityFillerFactory, _helpers, _layerStructFactory, _entityFillerHelper, _supportHelper, model.RegionDatas);
                    smodels.Add(smodel);
                }
            }

            //создаем коллекцию моделей

            //var series = MainWindow.MyMarkerWriter.CreateMarkerSeries("Ser1");
            //series.WriteFlag("Model STARTED");
            var modelCollection = new SlicedModelsCollection(_session, _layerStructFactory, _blocksBuilderHelper, _entityFillerHelper) { Models = smodels };

            var layersResult = await Task.Run(()=>modelCollection.FillToDataAsync(generationData, token, _wipeTower), token);
            //series.WriteFlag("Model FINISH");

            return (layersResult.FL, layersResult.MicroLayers, microLayerContours);
        }

        public void CreateMicroLayers(
            List<ITriangleModel> models,
            out IFirstLayer fl,
            out List<IMicroLayer> slicedMicroLayers,
            out List<MicroLayerContours> microLayerContours,
            GenerationData generationData,
            ISession session,
            CancellationToken token = default(CancellationToken))
        {
            //проверяем на отмену
            token.ThrowIfCancellationRequested();

            var collectionMinZUM = (int)models.Min(a => a.BoxMin.Z);
            var collectionMaxZUM = (int)models.Max(a => a.BoxMax.Z);
            var dictModelAndEntities = new Dictionary<ITriangleModel, List<LayerData>>();
            foreach (var model in models)
            {
                var layerDatas = SlicerHelper.GetLayerDatas(model, _session, collectionMinZUM, collectionMaxZUM);
                dictModelAndEntities.Add(model, layerDatas);
            }

            var uniqueZs = dictModelAndEntities.Select(a => a.Value.Select(b => b.AbsoluteZUM).ToList()).ToList().MakeAHugeMess().Distinct().OrderBy(a => a).ToList();
            var layerZs = new List<(int layerZUM, int layerIndex)>();
            for (var i = 0; i < uniqueZs.Count; i++)
            {
                layerZs.Add((uniqueZs[i], i));
            }

            var smodels = new List<ISlicedModel>();
            microLayerContours = new List<MicroLayerContours>();
            for (var layerIndex = 0; layerIndex < layerZs.Count; layerIndex++)
            {
                var contoursLayer = new MicroLayerContours
                {
                    Index = layerZs[layerIndex].layerIndex,
                    Islands = new List<Island>(),
                    IslandsExpanded = new List<Island>(),
                    IslandsShrinked = new List<Island>(),
                    AbsoluteZUM = layerZs[layerIndex].layerZUM
                };
                microLayerContours.Add(contoursLayer);
            }

            #region GET MAX TICKS

            int GetMaxValueForGenerations()
            {
                var layersForSlicingAndFilling = 0;
                var globalMinZ = double.MaxValue;
                var globalMaxZ = double.MinValue;
                foreach (var triangleModel in models)
                {
                    var modelEntities = dictModelAndEntities[triangleModel];

                    var modelMin = triangleModel.BoxMin.Z;
                    if (modelMin < globalMinZ) globalMinZ = modelMin;
                    var modelMax = triangleModel.BoxMax.Z;
                    if (modelMax > globalMaxZ) globalMaxZ = modelMax;

                    //slicing
                    layersForSlicingAndFilling += 10;
                    //inset0 + Micro
                    layersForSlicingAndFilling += modelEntities.Count(a => a.LayerEntity.HasFlag(LayerEntity.MACRO));
                    //insetXPFirstPart
                    layersForSlicingAndFilling += modelEntities.Count(a => a.LayerEntity.HasFlag(LayerEntity.MACRO) && a.LayerEntity.HasFlag(LayerEntity.INSET_XP));
                    //SIAFirstPart
                    layersForSlicingAndFilling += modelEntities.Count(a => a.LayerEntity.HasFlag(LayerEntity.MACRO));
                    //SIASecondPart
                    layersForSlicingAndFilling += modelEntities.Count(a => a.LayerEntity.HasFlag(LayerEntity.MACRO));
                    //SIAPThirdPart
                    layersForSlicingAndFilling += modelEntities.Count(a => a.LayerEntity.HasFlag(LayerEntity.MACRO));
                    //insetXF
                    if (session.InsetFiberSettings.GenerateFiberPerimeters)
                    {
                        //InsetXF
                        layersForSlicingAndFilling += modelEntities.Count(a => a.LayerEntity.HasFlag(LayerEntity.MACRO) && a.LayerEntity.HasFlag(LayerEntity.FIBER));
                        //InsetXPSecondPart
                        layersForSlicingAndFilling += modelEntities.Count(a => a.LayerEntity.HasFlag(LayerEntity.MACRO) && a.LayerEntity.HasFlag(LayerEntity.INSET_XP) && a.LayerEntity.HasFlag(LayerEntity.FIBER));
                        //SolidInfillUpAndBottom
                        layersForSlicingAndFilling += modelEntities.Count(a => a.LayerEntity.HasFlag(LayerEntity.MACRO));
                    }
                    //merge
                    layersForSlicingAndFilling += modelEntities.Count(a => a.LayerEntity.HasFlag(LayerEntity.MACRO) && a.LayerEntity.HasFlag(LayerEntity.INSET_XP));
                    //solidInfill
                    layersForSlicingAndFilling += modelEntities.Count(a => a.LayerEntity.HasFlag(LayerEntity.MACRO) && a.LayerEntity.HasFlag(LayerEntity.INFILL_P));
                    //cellular
                    layersForSlicingAndFilling += modelEntities.Count(a => a.LayerEntity.HasFlag(LayerEntity.MACRO) && a.LayerEntity.HasFlag(LayerEntity.INFILL_P));
                }
                //generate GCode
                var globalLayerCounts = uniqueZs.Count;
                return layersForSlicingAndFilling + globalLayerCounts;
            }
            generationData.MaxTicks = GetMaxValueForGenerations();

            #endregion

            var ewum5 = (int)(5 * _session.Inset0PlasticSettings.EWUM);
            var ewumShrinked = _session.Inset0PlasticSettings.EWUM + (int)(_session.InsetXPlasticSettings.EWUM * 0.45);
            //для всех моделей-сущностей
            foreach (var model in models)
            {
                var smls = new List<ISlicedMicroLayer>();
                //по всем высотам
                foreach (var layerZ in layerZs)
                {
                    if (layerZ.layerZUM < model.BoxMin.Z || layerZ.layerZUM > model.BoxMax.Z)
                    {
                        var smlByond = _entityFillerFactory.CreateMicroLayer(layerZ.layerZUM, layerZ.layerIndex, LayerEntity.NONE, _layerStructFactory, _session, _helpers, _entityFillerHelper);
                        smls.Add(smlByond);
                    }
                }

                var modelEntities = dictModelAndEntities[model];
                var modelZUms = modelEntities.Select(a => a.AbsoluteZUM).Where(a=>a >= model.BoxMin.Z && a <= model.BoxMax.Z).OrderBy(a=>a).ToList();

                var meshLayers = SlicerHelper.SliceModel(model, modelZUms, generationData);
                foreach (var meshLayer in meshLayers)
                {
                    var le = modelEntities.Single(a => a.AbsoluteZUM == meshLayer.Z);
                    var layedIndex = layerZs.Single(a => a.layerZUM == meshLayer.Z).layerIndex;
                    var sml = _entityFillerFactory.CreateMicroLayer((int)meshLayer.Z, layedIndex, le.LayerEntity, _layerStructFactory, session, _helpers, _entityFillerHelper);
                    sml.CreateRawIslands(meshLayer.PolygonList.ProcessIntoSeparateIsland(), _entityFillerFactory);
                    smls.Add(sml);
                }

                //ожидаем окончания выполнения всех тасков - полученный результат отсортировываем по индексу
                //накапливаем в контуры все контуры модели для соответсвующего глобального слоя
                smls = smls.OrderBy(a => a.LayerIndex).ToList();
                foreach (var sml in smls)
                {
                    if (sml.AllArea == null || !sml.AllArea.Any()) continue;
                    var mlContours = microLayerContours.SingleOrDefault(a => a.AbsoluteZUM == sml.AbsoluteZUM);
                    mlContours.Islands.AddRange(sml.AllArea);
                    foreach (var area in sml.AllArea)
                    {
                        var expanded = area.Offset(ewum5).ProcessIntoSeparateIsland();
                        if (expanded.Any()) mlContours.IslandsExpanded.AddRange(expanded);
                        var shrinked = area.Offset(-ewumShrinked).ProcessIntoSeparateIsland();
                        if (shrinked.Any()) mlContours.IslandsShrinked.AddRange(shrinked);
                    }
                }

                //создаем модель, передавая ей созданные ранее микросло
                var smodel = new SlicedModel(smls, _session, _entityFillerFactory, _helpers, _layerStructFactory, _entityFillerHelper, _supportHelper, model.RegionDatas);
                smodels.Add(smodel);
            }

            //создаем коллекцию моделей

            //var series = MainWindow.MyMarkerWriter.CreateMarkerSeries("Ser1");
            //series.WriteFlag("Model STARTED");
            var modelCollection = new SlicedModelsCollection(_session, _layerStructFactory, _blocksBuilderHelper, _entityFillerHelper) { Models = smodels };

            var layersResult = modelCollection.FillToData(generationData, token, _wipeTower);
            //series.WriteFlag("Model FINISH");
            fl = layersResult.FL;
            slicedMicroLayers = layersResult.MicroLayers;

        }
    }
}
