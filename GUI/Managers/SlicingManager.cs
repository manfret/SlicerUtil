using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Aura.CORE;
using Aura.Themes.Localization;
using Aura.ViewModels;
using devDept.Eyeshot.Entities;
using devDept.Geometry;
using EntityFiller;
using LayersStruct;
using LayersStruct.PPBlocksCollection;
using MSClipperLib;
using Settings;
using Settings.LayupData;
using Settings.Memento;
using Settings.Session;
using Slicer;
using Util.GeometryBasics;
using Triangle = Slicer.Triangle;

namespace Aura.Managers
{
    public interface ISlicingManager
    {
        (SessionVM.Generated generated, double time) GenerateSync(List<EntityDatasItemVM> entityWithDataItems,
            ISessionMemento sessionMemento, GenerationData generationData, CancellationTokenSource cts);

        Task<(SessionVM.Generated? generated, double? time, Exception e)> GenerateAsync(
            List<EntityDatasItemVM> entityWithDataItems,
            ISessionMemento sessionMemento,
            GenerationData generationData,
            CancellationTokenSource cts);

        void Initialize(IEntityFillerFactory entityFillerFactory,
            IExtrusionCalculator extrusionCalculator,
            ISettingsFactory settingsFactory,
            Helpers helpers,
            ILayerStructFactory layerStructFactory,
            ISupportHelperWrapper supportHelper,
            IBlockFactory blockFactory, IPPBlocksBuilderHelperWrapper ppBlocksBuilderHelperWrapper,
            IEntityFillerHelperWrapper entityFillerHelper);
    }

    public class SlicingManager : ISlicingManager
    {
        private IEntityFillerFactory _entityFillerFactory;
        private IExtrusionCalculator _extrusionCalculator;
        private ISettingsFactory _settingsFactory;
        private Helpers _helpers;
        private ILayerStructFactory _layerStructFactory;
        private ISupportHelperWrapper _supportHelper;
        private IPPBlocksBuilderHelperWrapper _ppBlocksBuilderHelperWrapper;
        private IBlockFactory _blockFactory;
        private IEntityFillerHelperWrapper _entityFillerHelper;

        public void Initialize(IEntityFillerFactory entityFillerFactory,
            IExtrusionCalculator extrusionCalculator,
            ISettingsFactory settingsFactory,
            Helpers helpers,
            ILayerStructFactory layerStructFactory,
            ISupportHelperWrapper supportHelper,
            IBlockFactory blockFactory, 
            IPPBlocksBuilderHelperWrapper ppBlocksBuilderHelperWrapper,
            IEntityFillerHelperWrapper entityFillerHelper)
        {
            _entityFillerFactory = entityFillerFactory;
            _extrusionCalculator = extrusionCalculator;
            _settingsFactory = settingsFactory;
            _helpers = helpers;
            _layerStructFactory = layerStructFactory;
            _supportHelper = supportHelper;
            _blockFactory = blockFactory;
            _ppBlocksBuilderHelperWrapper = ppBlocksBuilderHelperWrapper;
            _entityFillerHelper = entityFillerHelper;
        }


        #region GENERATE SYNC

        public (SessionVM.Generated generated, double time) GenerateSync(List<EntityDatasItemVM> entityWithDataItems, ISessionMemento sessionMemento, GenerationData generationData, CancellationTokenSource cts)
        {
            List<ITriangleModel> triangleModels = null;

            //метод создания модели из сущности EyeShot

            #region CREATE MODELS

            List<ITriangleModel> CreateTriangleModels()
            {
                ITriangleModel CreateTriangleModel(Entity rawEntity, List<IRegionData> regionDatas)
                {
                    var mesh = AuraViewportManager.GetMesh(rawEntity);
                    var modelTriangles = new List<ITriangle>();
                    var newVertices = new List<Point3D>();
                    foreach (var vertex in mesh.Vertices)
                    {
                        var newVertex = new Point3D(Math.Round(vertex.X, 3), Math.Round(vertex.Y, 3),
                            Math.Round(vertex.Z, 3));
                        newVertices.Add(newVertex);
                    }

                    foreach (var triangle in mesh.Triangles)
                    {
                        var triangleVertices = new List<Point3D>
                        {
                            newVertices[triangle.V1],
                            newVertices[triangle.V2],
                            newVertices[triangle.V3]
                        };
                        var slicerTriangle = new Triangle();
                        slicerTriangle.SetVertices(triangleVertices);
                        modelTriangles.Add(slicerTriangle);
                    }

                    var rawtriangleModel = new TriangleModel("ModelName") { RegionDatas = regionDatas };
                    rawtriangleModel.SetTriangles(modelTriangles, rawEntity.BoxMin, rawEntity.BoxMax);
                    return rawtriangleModel;
                }

                var models = new List<ITriangleModel>();
                entityWithDataItems.ForEach(pair =>
                {
                    var regionDatas = pair.Datas?.Select(a => a.RegionData).ToList();
                    models.Add(CreateTriangleModel(pair.Entity, regionDatas));
                });

                return models;
            }

            triangleModels = CreateTriangleModels();

            #endregion

            var session = new Session(sessionMemento, _extrusionCalculator, _settingsFactory);
            var layerCreator = new LayerCreator(_entityFillerFactory, session, _helpers, _layerStructFactory, _supportHelper, _entityFillerHelper, _ppBlocksBuilderHelperWrapper);

            #region GENERATE CODE

            var layersContours = new List<MicroLayerContours>();
            //основной метод слайсинга модели по слоям и заполнения сущностями
            layerCreator.CreateMicroLayers(
                triangleModels,
                out var fl,
                out var slicedMicroLayers,
                out layersContours,
                generationData,
                session,
                cts.Token);

            IPPBlocksCollectionBuilder collectionBuilder = new PPBlocksCollectionBuilder(_blockFactory, _ppBlocksBuilderHelperWrapper, session, layersContours);

            var microLayerCollection = new MicroLayersCollection(session, AuraNaming_en_EN.AuraVersion)
            {
                FirstLayer = fl,
                MicroLayers = slicedMicroLayers
            };
            var lastPoint = new IntPoint(0, 0);
            microLayerCollection.Prepare(collectionBuilder, generationData, cts.Token);
            microLayerCollection.GenerateBlocksToCollection(ref collectionBuilder, generationData, cts.Token);
            var blocks = collectionBuilder.GetBlocks();
            var stringBuilder = new StringBuilder();
            var time = 0.0;
            foreach (var block in blocks)
            {
                stringBuilder.Append(block.GetCode());
                time += block.GetSeconds();
            }
            time = Math.Round(time, 2);

            var generated = new SessionVM.Generated
            {
                FirstLayer = fl,
                MicroLayers = slicedMicroLayers,
                GeneratedBlocks = blocks,
                GCode = stringBuilder.ToString()
            };

            #endregion

            return (generated, time);
        }

        #endregion

        #region GENERATE ASYNC

        public async Task<(SessionVM.Generated? generated, double? time, Exception e)> GenerateAsync(
            List<EntityDatasItemVM> entityWithDataItems, 
            ISessionMemento sessionMemento, 
            GenerationData generationData, 
            CancellationTokenSource cts)
        {
            try
            {
                //var series = MyMarkerWriter.CreateMarkerSeries("Ser1");

                var session = new Session(sessionMemento, _extrusionCalculator, _settingsFactory);
                var layerCreator = new LayerCreator(_entityFillerFactory, session, _helpers, _layerStructFactory, _supportHelper, _entityFillerHelper, _ppBlocksBuilderHelperWrapper);

                #region CREATE MODELS

                async Task<List<ITriangleModel>> CreateTriangleModels(ISession inSession)
                {
                    ITriangleModel CreateTriangleModel(Entity rawEntity, List<IRegionData> regionDatas)
                    {
                        var mesh = AuraViewportManager.GetMesh(rawEntity);

                        var modelTriangles = new List<ITriangle>();
                        var newVertices = new List<Point3D>();
                        foreach (var vertex in mesh.Vertices)
                        {
                            var newVertex = new Point3D(Math.Round(vertex.X, 3), Math.Round(vertex.Y, 3),
                                Math.Round(vertex.Z, 3));
                            newVertices.Add(newVertex);
                        }

                        foreach (var triangle in mesh.Triangles)
                        {
                            var triangleVertices = new List<Point3D>
                            {
                                newVertices[triangle.V1],
                                newVertices[triangle.V2],
                                newVertices[triangle.V3]
                            };
                            var slicerTriangle = new Triangle();
                            slicerTriangle.SetVertices(triangleVertices);
                            modelTriangles.Add(slicerTriangle);
                        }

                        var rawtriangleModel = new TriangleModel("ModelName") { RegionDatas = regionDatas };
                        rawtriangleModel.SetTriangles(modelTriangles, rawEntity.BoxMin, rawEntity.BoxMax);
                        return rawtriangleModel;
                    }

                    var modelTasks = new List<Task<ITriangleModel>>();
                    entityWithDataItems.ForEach(pair =>
                    {
                        var regionDatas = pair.Datas?.Select(a => a.RegionData).ToList();
                        modelTasks.Add(Task.Run(() => CreateTriangleModel(pair.Entity, regionDatas)));
                    });

                    await Task.WhenAll(modelTasks);
                    var models = new List<ITriangleModel>();
                    foreach (var modelTask in modelTasks)
                    {
                        var model = modelTask.Result;
                        var modelHeight = model.BoxMax.Z - model.BoxMin.Z;
                        if ((model.BoxMin.Z == 0 && modelHeight > inSession.GlobalSettings.FLThicknessUM) ||
                            (model.BoxMin.Z > 0 && modelHeight > inSession.GlobalSettings.MacroLayerHeightUM /
                             inSession.SessionMemento.Profile.GlobalSettingsMemento.Inset0RatioToMacro))
                        {
                            models.Add(model);
                        }
                    }

                    return models;
                }

                //series.WriteFlag("CreateTriangleModels");
                var triangleModels = await Task.Run(() => CreateTriangleModels(session));

                #endregion

                #region GENERATE CODE

                // series.WriteFlag("CreateMicroLayersAsync");
                //основной метод слайсинга модели по слоям и заполнения сущностями
                var (fl, slicedMicroLayers, microLayerContours) = await Task.Run(() => layerCreator.CreateMicroLayersAsync(
                    triangleModels,
                    generationData,
                    session,
                    cts.Token));
                IPPBlocksCollectionBuilder collectionBuilder =
                    new PPBlocksCollectionBuilder(_blockFactory, _ppBlocksBuilderHelperWrapper, session, microLayerContours);

                var microLayerCollection = new MicroLayersCollection(session, AuraNaming_en_EN.AuraVersion)
                {
                    FirstLayer = fl,
                    MicroLayers = slicedMicroLayers
                };
                await Task.Run(() => microLayerCollection.PrepareAsync(collectionBuilder, generationData, cts.Token));
                collectionBuilder.CurrentMicroLayer = 0;
                await Task.Run(() => microLayerCollection.GenerateBlocksToCollection(ref collectionBuilder, generationData, cts.Token));
                var blocks = collectionBuilder.GetBlocks();
                var stringBuilder = new StringBuilder();
                var time = 0.0;
                foreach (var block in blocks)
                {
                    stringBuilder.Append(block.GetCode());
                    time += block.GetSeconds();
                }

                var generated = new SessionVM.Generated
                {
                    FirstLayer = fl,
                    MicroLayers = slicedMicroLayers,
                    GeneratedBlocks = blocks,
                    GCode = stringBuilder.ToString()
                };

                #endregion

                time = Math.Round(time, 2);
                collectionBuilder.InsertComment($"EST_TIME:{time}", 2, 0);

                return (generated, time, null);

            }
            catch (Exception e)
            {
                return (null, null, e);
            }
        }

        #endregion
    }
}
