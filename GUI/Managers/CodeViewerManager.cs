using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;
using Aura.Controls;
using Aura.ViewModels;
using devDept.Eyeshot;
using devDept.Eyeshot.Entities;
using devDept.Geometry;
using LayersStruct.FirstLayer;
using LayersStruct.MacroLayer.MicroLayer;
using LayersStruct.MacroLayer.MicroLayer.ModelMicroLayer.ModelIsland;
using LayersStruct.MacroLayer.MicroLayer.ModelMicroLayer.SupportIsland;
using MSClipperLib;
using PostProcessor.Blocks;
using PostProcessor.Blocks.Cut;
using PostProcessor.Blocks.Extrude;
using PostProcessor.Blocks.Move;
using PostProcessor.Blocks.Retract;
using PostProcessor.Blocks.Travel;
using PostProcessor.Properties;
using Util.GeometryBasics;
using BlockModes = Aura.ViewModels.BlockModes;

namespace Aura.Managers
{
    public interface ICodeViewerManager
    {
        event EventHandler ProcessFinished;

        #region INITIALIZE

        void Initialize(IAuraViewportManager auraViewportManager, AuraViewportLayout auraViewportLayout);

        #endregion

        #region UPDATE LAYER COLOR

        void UpdateLayerColor(EntityType type, Color color);

        void UpdateLayerColor(BlockType type, Color color);

        #endregion

        #region UPDATE LAYER VISIBILITY

        void UpdateLayerVisibility(EntityType type, bool visibility);
        void UpdateLayerVisibility(BlockType type, bool visibility);

        #endregion

        #region CREATE LAYERS

        void CreateLayers(Dictionary<BlockType, Color> blockColors);
        void CreateLayers(Dictionary<EntityType, Color> entityColors);

        #endregion

        #region FILL CODE ENTITIES

        void FillCodeEntities(IFirstLayer fl, List<IMicroLayer> microLayers, List<IPPBlock> blocks);

        #endregion

        #region SWITCH TO BLOCKS

        void SwitchToBlocks();

        #endregion

        #region SWITCH TO GEOMETRY

        void SwitchToGeometry();

        #endregion

        #region SWITCH TO ALL Z MODE GEOMETRY

        void UpdateAllZMode(BlockModes blockModes);

        #endregion

        #region SWITCH TO RANGE MODE

        void UpdateRangeMode(BlockModes blockModes, CodeViewerVM.LayerZ start, CodeViewerVM.LayerZ end);

        #endregion

        #region SWITCH TO ONE LAYER

        void UpdateOneLayer(BlockModes blockModes, CodeViewerVM.LayerZ layer);

        #endregion

        #region SWITCH TO 3D

        void SwitchTo3D();

        #endregion

        #region SWITCH TO 2D

        void SwitchTo2D();

        #endregion

        #region DISPOSE ALL

        void DisposeAll();

        #endregion

        #region DISPOSE GEOMETRY LAYERS

        void DisposeGeometryLayers();

        #endregion

        #region DISPOSE BLOCK LAYERS

        void DisposeBlockLayers();

        #endregion

        #region ENTER VIEW

        void EnterViewCode();

        #endregion

        #region EXIT VIEW CODE

        void ExitViewCode();

        #endregion
    }

    public class CodeViewerManager : ICodeViewerManager
    {
        private IAuraViewportManager _auraViewportManager;
        private AuraViewportLayout _auraViewportLayout;
        private const int TRANSPARENCY = 255;
        private Dictionary<BlockType, (string name, Block block, List<Entity> sourceMeshes)> _sourceBlocks;

        private Dictionary<EntityType, Layer> _geometryLayers;
        private Dictionary<BlockType, Layer> _blockLayers;

        private class Data
        {
            public Dictionary<(double zMM, EntityType entityType), Entity> Geometries { get; set; }
            public Dictionary<(int layerIndex, BlockType blockType), List<BlockReference>> BasedBlocks { get; set; }
            public Dictionary<(int layerIndex, BlockType blockType), Entity> NoBasedBlocks { get; set; }
        }

        private Data _data;

        public event EventHandler ProcessFinished;

        public void Initialize(IAuraViewportManager auraViewportManager, AuraViewportLayout auraViewportLayout)
        {
            _auraViewportManager = auraViewportManager;
            _auraViewportLayout = auraViewportLayout;
            _auraViewportLayout.WorkCompleted += AuraViewportLayoutOnWorkCompleted;
            CreateBasedBlocks();
        }

        #region CREATE BASED BLOCKS

        public void CreateBasedBlocks()
        {
            _sourceBlocks = new Dictionary<BlockType, (string name, Block block, List<Entity> sourceMeshes)>();

            #region EXTRUDE P

            var extrudePMesh = Mesh.CreateCone(0, 0.2, new Point3D(0, 0, 0), new Point3D(0, 0, 0.5), 3);
            extrudePMesh.ColorMethod = colorMethodType.byParent;
            //extrudePMesh.Color = System.Drawing.Color.FromArgb(TRANSPARENCY, extrudePColor.R, extrudePColor.G, extrudePColor.B);

            var extrudePBlock = new Block("ExtrudeP");
            extrudePBlock.Entities.Add(extrudePMesh);
            _sourceBlocks.Add(BlockType.EXTRUDE_P, ("ExtrudeP", extrudePBlock, new List<Entity>() { extrudePMesh }));
            _auraViewportManager.AddBlock(extrudePBlock);

            #endregion

            #region EXTRUDE PF

            var extrudePFMesh = Mesh.CreateCone(0, 0.2, new Point3D(0, 0, 0), new Point3D(0, 0, 0.5), 3);
            extrudePFMesh.ColorMethod = colorMethodType.byParent;
            //extrudePFMesh.Color = System.Drawing.Color.FromArgb(TRANSPARENCY, extrudePFColor.R, extrudePFColor.G, extrudePFColor.B);

            var extrudePFBlock = new Block("ExtrudePF");
            extrudePFBlock.Entities.Add(extrudePMesh);
            _sourceBlocks.Add(BlockType.EXTRUDE_PF, ("ExtrudePF", extrudePFBlock, new List<Entity>() { extrudePFMesh }));
            _auraViewportManager.AddBlock(extrudePFBlock);

            #endregion

            #region RETRACT

            var retractMesh = Mesh.CreateCone(0.2, 0, new Point3D(0, 0, 0), new Point3D(0, 0, 0.5), 3);
            retractMesh.ColorMethod = colorMethodType.byParent;
            //retractMesh.Color = System.Drawing.Color.FromArgb(TRANSPARENCY, retractColor.R, retractColor.G, retractColor.B);

            var retractBlock = new Block("Retract");
            retractBlock.Entities.Add(retractMesh);
            _sourceBlocks.Add(BlockType.RETRACT, ("Retract", retractBlock, new List<Entity>() { retractMesh }));
            _auraViewportManager.AddBlock(retractBlock);

            #endregion

            #region CUT

            var cutMesh = Mesh.CreateTorus(0.2, 0.05, 10, 30);
            cutMesh.ColorMethod = colorMethodType.byParent;
            //cutMesh.Color = System.Drawing.Color.FromArgb(TRANSPARENCY, cutColor.R, cutColor.G, cutColor.B);
            cutMesh.TransformBy(new Translation(0, 0, 0 - cutMesh.BoxMin.Z));

            var cutBlockZCenter = (cutMesh.BoxMax.Z - cutMesh.BoxMin.Z) / 2;

            var firstCrossLine = Mesh.CreateBox(0.4, 0.04, 0.04);
            firstCrossLine.ColorMethod = colorMethodType.byParent;
            //firstCrossLine.Color = System.Drawing.Color.FromArgb(TRANSPARENCY, cutColor.R, cutColor.G, cutColor.B);
            var firstCrossLineZCenter
                = new Point3D(
                    (firstCrossLine.BoxMax.X - firstCrossLine.BoxMin.X) / 2,
                    (firstCrossLine.BoxMax.Y - firstCrossLine.BoxMin.Y) / 2,
                    (firstCrossLine.BoxMax.Z - firstCrossLine.BoxMin.Z) / 2);
            firstCrossLine.TransformBy(new Translation(0 - firstCrossLineZCenter.X, 0 - firstCrossLineZCenter.Y,
                cutBlockZCenter - firstCrossLineZCenter.Z));
            var secondCrossLine = firstCrossLine.Clone() as Mesh;

            firstCrossLine.TransformBy(new Rotation(45 * Math.PI / 180, Vector3D.AxisZ));
            secondCrossLine.TransformBy(new Rotation(-45 * Math.PI / 180, Vector3D.AxisZ));

            var cutBlock = new Block("Cut");
            cutBlock.Entities.Add(cutMesh);
            cutBlock.Entities.Add(firstCrossLine);
            cutBlock.Entities.Add(secondCrossLine);
            _sourceBlocks.Add(BlockType.CUT, ("Cut", cutBlock, new List<Entity>() { cutMesh, firstCrossLine, secondCrossLine }));
            _auraViewportManager.AddBlock(cutBlock);

            #endregion
        }

        #endregion

        #region CREATE GEOMETRY LAYER

        private void CreateGeometryLayer(EntityType type, Color color)
        {
            var newColor = System.Drawing.Color.FromArgb(TRANSPARENCY, color.R, color.G, color.B);
            var layer = new Layer(type.ToString(), newColor)
            {
                LineWeight = 1.5f,
                Visible = false
            };
            _geometryLayers.Add(type, layer);
            _auraViewportManager.AddLayer(layer);
        }

        #endregion

        #region CREATE BLOCK LAYER

        private void CreateBlockLayer(BlockType type, Color color)
        {
            var newColor = System.Drawing.Color.FromArgb(TRANSPARENCY, color.R, color.G, color.B);
            var layer = new Layer(type.ToString(), newColor)
            {
                LineWeight = 1.5f,
                Visible = false
            };
            _blockLayers.Add(type, layer);
            _auraViewportManager.AddLayer(layer);
        }

        #endregion

        #region UPDATE LAYER COLOR

        public void UpdateLayerColor(EntityType type, Color color)
        {
            var newColor = System.Drawing.Color.FromArgb(TRANSPARENCY, color.R, color.G, color.B);
            if (_geometryLayers[type].Color == newColor) return;
            _auraViewportManager.UpdateLayerColor(type.ToString(), newColor);
        }

        public void UpdateLayerColor(BlockType type, Color color)
        {
            var newColor = System.Drawing.Color.FromArgb(TRANSPARENCY, color.R, color.G, color.B);
            if (_blockLayers[type].Color == newColor) return;
            _auraViewportManager.UpdateLayerColor(type.ToString(), newColor);
        }

        #endregion

        #region UPDATE LAYER VISIBILITY

        public void UpdateLayerVisibility(EntityType type, bool visibility)
        {
            if (_geometryLayers[type].Visible == visibility) return;
            _auraViewportManager.UpdateLayerVisibility(type.ToString(), visibility, _isInCodeView);
        }

        public void UpdateLayerVisibility(BlockType type, bool visibility)
        {
            if (_blockLayers[type].Visible == visibility) return;
            _auraViewportManager.UpdateLayerVisibility(type.ToString(), visibility, _isInCodeView);
        }

        #endregion

        #region CREATE LAYERS

        public void CreateLayers(Dictionary<BlockType, Color> blockColors)
        {
            _blockLayers = new Dictionary<BlockType, Layer>();
            foreach (var blockColor in blockColors)
            {
                CreateBlockLayer(blockColor.Key, blockColor.Value);
            }
        }

        public void CreateLayers(Dictionary<EntityType, Color> entityColors)
        {
            _geometryLayers = new Dictionary<EntityType, Layer>();
            foreach (var entityColor in entityColors)
            {
                CreateGeometryLayer(entityColor.Key, entityColor.Value);
            }
        }

        #endregion

        #region CREATE CODE ENTITIES

        public void FillCodeEntities(IFirstLayer fl, List<IMicroLayer> microLayers, List<IPPBlock> blocks)
        {
            var layers = new List<Layer>(_geometryLayers.Select(a => a.Value)){};
            layers.AddRange(_blockLayers.Select(a => a.Value));
            var geometryWU = new CodeViewerWorkUnit(fl, microLayers, blocks, _sourceBlocks, layers);
            _auraViewportLayout.StartWork(geometryWU);
        }

        #endregion

        #region AURA VIEWPORT ON WORK COMPLETED

        private void AuraViewportLayoutOnWorkCompleted(object sender, WorkCompletedEventArgs e)
        {
            if (e.WorkUnit is CodeViewerWorkUnit geometryWU)
            {
                _data = new Data
                {
                    Geometries = geometryWU.Geometries,
                    BasedBlocks = geometryWU.BasedBlocks,
                    NoBasedBlocks = geometryWU.NoBaseBlocks
                };
                ProcessFinished?.Invoke(this, null);
            }
        }

        #endregion

        #region SWITCH TO BLOCKS

        public void SwitchToBlocks()
        {
            var i = 0;

            _auraViewportLayout.AuraProgressBar.Maximum = 3;
            _auraViewportLayout.AuraProgressBar.Visible = true;

            ClearGeometries();
            _auraViewportLayout.AuraProgressBar.NewValue = ++i;
            AddEntitiesNoBasedBlocks();
            _auraViewportLayout.AuraProgressBar.NewValue = ++i;
            AddEntitiesBasedBlocks();
            _auraViewportLayout.AuraProgressBar.NewValue = ++i;

            _auraViewportLayout.AuraProgressBar.Visible = false;

            _auraViewportLayout.AdjustNearAndFarPlanes();

            _auraViewportLayout.Invalidate();
        }

        #endregion

        #region SWITCH TO GEOMETRY

        public void SwitchToGeometry()
        {
            var i = 0;

            _auraViewportLayout.AuraProgressBar.Maximum = 3;
            _auraViewportLayout.AuraProgressBar.Visible = true;

            ClearBasedBlocks();
            _auraViewportLayout.AuraProgressBar.NewValue = ++i;
            ClearNoBasedBlocks();
            _auraViewportLayout.AuraProgressBar.NewValue = ++i;
            AddEntiesGeometries();
            _auraViewportLayout.AuraProgressBar.NewValue = ++i;

            _auraViewportLayout.AuraProgressBar.Visible = false;

            _auraViewportLayout.AdjustNearAndFarPlanes();

            _auraViewportLayout.Invalidate();
        }

        #endregion

        #region SHOW / HIDE ENTITY

        private void ShowEntity(IEnumerable<Entity> entities)
        {
            foreach (var entity in entities)
            {
                entity.SetVisibility(true, null);
            }
            _auraViewportLayout.Invalidate();
        }

        private void HideEntity(IEnumerable<Entity> entities)
        {
            foreach (var entity in entities)
            {
                entity.SetVisibility(false, null);
            }
            _auraViewportLayout.Invalidate();
        }

        #endregion

        #region UPDATE ALL Z MODE

        public void UpdateAllZMode(BlockModes blockModes)
        {
            switch (blockModes)
            {
                case BlockModes.GEOMETRY:
                    if (_data == null) break;
                    var forVisibleGs = _data.Geometries.Where(a => !a.Value.Visible).Select(a => a.Value);
                    ShowEntity(forVisibleGs);
                    break;
                case BlockModes.BLOCKS:
                    if (_data == null) break;
                    var forVisibleBBs = _data.BasedBlocks.Where(a => !a.Value.First().Visible).Select(a => a.Value).ToList().MakeAHugeMess();
                    ShowEntity(forVisibleBBs);
                    var forVisibleNBBs = _data.NoBasedBlocks.Where(a => !a.Value.Visible).Select(a => a.Value);
                    ShowEntity(forVisibleNBBs);
                    break;
            }

            _auraViewportLayout.Entities.UpdateBoundingBox();
            _auraViewportLayout.AdjustNearAndFarPlanes();
            _auraViewportLayout.Invalidate();
        }

        #endregion

        #region UPDATE RANGE MODE

        public void UpdateRangeMode(BlockModes blockModes, CodeViewerVM.LayerZ start, CodeViewerVM.LayerZ end)
        {
            switch (blockModes)
            {
                case BlockModes.GEOMETRY:
                    if (_data == null) break;
                    var forVisibleGs = _data.Geometries.Where(a => a.Key.zMM >= start.Z && a.Key.zMM <= end.Z).Select(a => a.Value);
                    var forHideGs = _data.Geometries.Where(a => a.Key.zMM < start.Z || a.Key.zMM > end.Z).Select(a => a.Value);
                    ShowEntity(forVisibleGs);
                    HideEntity(forHideGs);
                    break;
                case BlockModes.BLOCKS:
                    if (_data == null) break;
                    var trueLayerIndexStart = start.LayerIndex - 1;
                    var trueLayerIndexEnd = end.LayerIndex - 1;
                    var forVisibleBs = _data.BasedBlocks.Where(a => a.Key.layerIndex >= trueLayerIndexStart && a.Key.layerIndex <= trueLayerIndexEnd).Select(a => a.Value).ToList().MakeAHugeMess();
                    var forHideBs = _data.BasedBlocks.Where(a => a.Key.layerIndex < trueLayerIndexStart || a.Key.layerIndex > trueLayerIndexEnd).Select(a => a.Value).ToList().MakeAHugeMess();
                    ShowEntity(forVisibleBs);
                    HideEntity(forHideBs);
                    var forVisibleNBs = _data.NoBasedBlocks.Where(a => a.Key.layerIndex >= trueLayerIndexStart && a.Key.layerIndex <= trueLayerIndexEnd).Select(a => a.Value);
                    var forHideNBs = _data.NoBasedBlocks.Where(a => a.Key.layerIndex < trueLayerIndexStart || a.Key.layerIndex > trueLayerIndexEnd).Select(a => a.Value);
                    ShowEntity(forVisibleNBs);
                    HideEntity(forHideNBs);
                    break;
            }
            _auraViewportLayout.Entities.UpdateBoundingBox();
            _auraViewportLayout.AdjustNearAndFarPlanes();
        }

        #endregion

        #region UPDATE ONE LAYER

        public void UpdateOneLayer(BlockModes blockModes, CodeViewerVM.LayerZ layer)
        {
            switch (blockModes)
            {
                case BlockModes.GEOMETRY:
                    if (_data == null) break;
                    var forVisibleGs = _data.Geometries.Where(a => Math.Abs(a.Key.zMM - layer.Z) < 0.0001).Select(a => a.Value);
                    var forHideGs = _data.Geometries.Where(a => Math.Abs(a.Key.zMM - layer.Z) > 0.0001).Select(a => a.Value);
                    ShowEntity(forVisibleGs);
                    HideEntity(forHideGs);
                    break;
                case BlockModes.BLOCKS:
                    if (_data == null) break;
                    var trueLayerIndex = layer.LayerIndex - 1;
                    var forVisibleBs = _data.BasedBlocks.Where(a => a.Key.layerIndex == trueLayerIndex).Select(a => a.Value).ToList().MakeAHugeMess();
                    var forVisibleNBs = _data.NoBasedBlocks.Where(a => a.Key.layerIndex == trueLayerIndex).Select(a => a.Value);
                    var forHideBs = _data.BasedBlocks.Where(a => a.Key.layerIndex != trueLayerIndex).Select(a => a.Value).ToList().MakeAHugeMess();
                    var forHideNBs = _data.NoBasedBlocks.Where(a => a.Key.layerIndex != trueLayerIndex).Select(a => a.Value);
                    ShowEntity(forVisibleBs);
                    ShowEntity(forVisibleNBs);
                    HideEntity(forHideBs);
                    HideEntity(forHideNBs);
                    break;
            }
            _auraViewportLayout.Entities.UpdateBoundingBox();
            _auraViewportLayout.AdjustNearAndFarPlanes();
        }

        #endregion

        #region SWITCH TO 3D

        public void SwitchTo3D()
        {
            _auraViewportManager.CameraTo3D();
        }

        #endregion

        #region SWITCH TO 2D

        public void SwitchTo2D()
        {
            _auraViewportManager.CameraTo2D();
        }

        #endregion

        #region CODE VIEW

        private bool _isInCodeView = false;

        private List<Entity> _selected;
        public void EnterViewCode()
        {
            _isInCodeView = true;
            _auraViewportManager.GoToNonStandartView();
            _auraViewportLayout.Rendered.PlanarReflections = false;

            _selected = _auraViewportLayout.Entities.Where(a => a.GetSelection()).ToList();
            _auraViewportManager.DeselectAll();
            _auraViewportManager.SetVisibilityForAll(false);

            _auraViewportLayout.Light2.Active = false;
        }

        public void ExitViewCode()
        {
            _isInCodeView = false;
            _auraViewportLayout.Light2.Active = true;
            _auraViewportLayout.Rendered.PlanarReflections = true;

            _auraViewportManager.SetVisibilityForAll(true);
            if (_selected != null && _selected.Any())
            {
                foreach (var entity in _selected)
                {
                    _auraViewportManager.SetSelection(entity, true);
                }
            }

            _auraViewportManager.GoToStandartView();
        }

        #endregion

        #region ADD ENTITY

        private void AddEntiesGeometries()
        {
            if (_data == null) return;
            AddEntiies(_data.Geometries.Select(a => (a.Value, a.Key.entityType.ToString())));
        }

        private void AddEntitiesBasedBlocks()
        {
            if (_data == null) return;
            AddEntiies(_data.BasedBlocks.Select(a => (a.Value, a.Key.blockType.ToString())));
        }

        private void AddEntitiesNoBasedBlocks()
        {
            if (_data == null) return;
            AddEntiies(_data.NoBasedBlocks.Select(a => (a.Value, a.Key.blockType.ToString())));
        }

        private void AddEntiies(IEnumerable<(Entity entity, string layerName)> entities)
        {
            if (entities == null) return;

            foreach (var (entity, layerName) in entities)
            {
                entity.Selectable = false;
                _auraViewportManager.AddEntity(entity, layerName);
            }
            _auraViewportLayout.Invalidate();
        }

        private void AddEntiies(IEnumerable<(List<BlockReference> entities, string layerName)> entities)
        {
            if (entities == null) return;

            foreach (var (entitiesInItem, layerName) in entities)
            {
                foreach (var entity in entitiesInItem)
                {
                    _auraViewportManager.AddEntity(entity, layerName);
                }
            }
            _auraViewportLayout.Invalidate();
        }

        #endregion

        #region CLEAR

        private void ClearGeometries()
        {
            if (_data == null) return;
            _auraViewportManager.RemoveEntities(_data.Geometries.Select(a => a.Value).ToList());
        }

        private void ClearBasedBlocks()
        {
            if (_data == null) return;
            foreach (var dataBasedBlock in _data.BasedBlocks) 
            {
                _auraViewportManager.RemoveEntities(dataBasedBlock.Value.ToList());
            }
        }

        private void ClearNoBasedBlocks()
        {
            if (_data == null) return;
            _auraViewportManager.RemoveEntities(_data.NoBasedBlocks.Select(a=>a.Value).ToList());
        }

        #endregion

        #region DISPOSE

        private void DisposeGeometries()
        {
            if (_data == null) return;
            DisposeEntities(_data.Geometries.Select(a => a.Value));
            _data.Geometries.Clear();
        }

        private void DisposeBasedBlocks()
        {
            if (_data == null) return;
             DisposeEntities(_data.BasedBlocks.Select(a => a.Value).ToList().MakeAHugeMess());
             _data.BasedBlocks.Clear();
        }

        private void DisposeNoBasedBlocks()
        {
            if (_data == null) return;
            DisposeEntities(_data.NoBasedBlocks.Select(a => a.Value));
            _data.NoBasedBlocks.Clear();

        }

        private void DisposeEntities(IEnumerable<Entity> entities)
        {
            if (entities == null) return;

            _auraViewportManager.RemoveEntities(entities.ToList());
            _auraViewportLayout.Invalidate();
        }

        public void DisposeGeometryLayers()
        {
            if (_geometryLayers == null) return;

            foreach (var geometryLayer in _geometryLayers)
            {
                _auraViewportManager.RemoveLayer(geometryLayer.Value);
            }
            _geometryLayers.Clear();
            _auraViewportLayout.Invalidate();
        }

        public void DisposeBlockLayers()
        {
            if (_blockLayers == null) return;

            foreach (var blockValue in _blockLayers)
            {
                _auraViewportManager.RemoveLayer(blockValue.Value);
            }
            _blockLayers.Clear();
            _auraViewportLayout.Invalidate();
        }

        public void DisposeAll()
        {
            DisposeGeometries();
            DisposeBasedBlocks();
            DisposeNoBasedBlocks();
            _data = null;
        }

        #endregion

        private class CodeViewerWorkUnit : WorkUnit
        {
            private readonly IFirstLayer _fl;
            private readonly List<IMicroLayer> _microLayers;
            private readonly List<IPPBlock> _blocks;
            private readonly Dictionary<BlockType, (string name, Block block, List<Entity> sourceMeshes)> _sourceBlocks;
            private readonly List<Layer> _layers;

            //словарь хранения геометрии
            public Dictionary<(double zMM, EntityType entityType), Entity> Geometries { get; }
            //словарь хранения блоков кода, у которых есть общая исходная сущность
            public Dictionary<(int layerIndex, BlockType blockType), List<BlockReference>> BasedBlocks { get; }
            //словарь хранения блоков кода, у каждой из которых своя независимая сущность
            public Dictionary<(int layerIndex, BlockType blockType), Entity> NoBaseBlocks { get; }

            public CodeViewerWorkUnit(
                IFirstLayer fl,
                List<IMicroLayer> microLayers,
                List<IPPBlock> blocks,
                Dictionary<BlockType, (string name, Block block, List<Entity> sourceMeshes)> sourceBlocks,
                List<Layer> layers)
            {
                _fl = fl;
                _microLayers = microLayers;
                _blocks = blocks;
                _layers = layers;

                //Geometries = new Dictionary<(double zMM, EntityType entityType), (List<Entity> baseEntities, Block block, BlockReference blockReference)>();
                Geometries = new Dictionary<(double zMM, EntityType entityType), Entity>();
                BasedBlocks = new Dictionary<(int layerIndex, BlockType blockType), List<BlockReference>>();
                //NoBaseBlocks = new Dictionary<(int layerIndex, BlockType blockType), (BlockReference blockReference, Block block)>();
                NoBaseBlocks = new Dictionary<(int layerIndex, BlockType blockType), Entity>();
                _sourceBlocks = sourceBlocks;
            }

            protected override void DoWork(BackgroundWorker worker, DoWorkEventArgs doWorkEventArgs)
            {
                #region GEOMETRY

                var i = 0.0;

                var geometries = new Dictionary<(EntityType entityType, int absoluteZUM), List<List<Segment>>>();

                //заполняем словарь соответсвия тип+высота = полигон
                void FillFromPrintIsland(IPrintIsland island, int absoluteZUM)
                {
                    if (island.Insets0Plastic != null)
                    {
                        foreach (var insetX0Outer in island.Insets0Plastic.Outers)
                        {
                            foreach (var polygon in insetX0Outer)
                            {
                                if (polygon.Any())
                                {
                                    if (!geometries.ContainsKey((EntityType.INSET0, absoluteZUM))) geometries.Add((EntityType.INSET0, absoluteZUM), new List<List<Segment>>() { polygon });
                                    else geometries[(EntityType.INSET0, absoluteZUM)].Add(polygon);
                                }
                            }
                        }

                        if (island.Insets0Plastic.Inners != null && island.Insets0Plastic.Inners.Any())
                        {
                            foreach (var insetX0Inner in island.Insets0Plastic.Inners)
                            {
                                foreach (var polygon in insetX0Inner)
                                {
                                    if (polygon.Any())
                                    {
                                        if (!geometries.ContainsKey((EntityType.INSET0, absoluteZUM))) geometries.Add((EntityType.INSET0, absoluteZUM), new List<List<Segment>>() { polygon });
                                        else geometries[(EntityType.INSET0, absoluteZUM)].Add(polygon);
                                    }
                                }
                            }
                        }
                    }

                    if (island.MicroInfill != null)
                    {
                        foreach (var microInfill in island.MicroInfill)
                        {
                            if (microInfill.Segments == null) continue;
                            foreach (var microInfillInner in microInfill.Segments)
                            {
                                if (!geometries.ContainsKey((EntityType.MICRO_INFILL_P, absoluteZUM))) geometries.Add((EntityType.MICRO_INFILL_P, absoluteZUM), new List<List<Segment>>() { new List<Segment>() { microInfillInner } });
                                else geometries[(EntityType.MICRO_INFILL_P, absoluteZUM)].Add(new List<Segment>() { microInfillInner });
                            }
                        }
                    }

                    if (island.SolidMicroInfills != null)
                    {
                        foreach (var solidMicroInfill in island.SolidMicroInfills)
                        {
                            if (solidMicroInfill.Segments == null) continue;
                            foreach (var microInfillInner in solidMicroInfill.Segments)
                            {
                                if (!geometries.ContainsKey((EntityType.MICRO_INFILL_P, absoluteZUM))) geometries.Add((EntityType.MICRO_INFILL_P, absoluteZUM), new List<List<Segment>>() { new List<Segment>() { microInfillInner } });
                                else geometries[(EntityType.MICRO_INFILL_P, absoluteZUM)].Add(new List<Segment>() { microInfillInner });
                            }
                        }
                    }

                    if (island.InsetsXPlastic != null)
                    {
                        foreach (var insetXP in island.InsetsXPlastic)
                        {
                            foreach (var insetXPOuter in insetXP.Outers)
                            {
                                foreach (var polygon in insetXPOuter)
                                {
                                    if (polygon.Any())
                                    {
                                        if (!geometries.ContainsKey((EntityType.INSET_XP, absoluteZUM))) geometries.Add((EntityType.INSET_XP, absoluteZUM), new List<List<Segment>>() { polygon });
                                        else geometries[(EntityType.INSET_XP, absoluteZUM)].Add(polygon);
                                    }
                                }
                            }

                            if (insetXP.Inners != null && insetXP.Inners.Any())
                            {
                                foreach (var insetXPInner in insetXP.Inners)
                                {
                                    foreach (var polygon in insetXPInner)
                                    {
                                        if (polygon.Any())
                                        {
                                            if (!geometries.ContainsKey((EntityType.INSET_XP, absoluteZUM))) geometries.Add((EntityType.INSET_XP, absoluteZUM), new List<List<Segment>>() { polygon });
                                            else geometries[(EntityType.INSET_XP, absoluteZUM)].Add(polygon);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (island.SolidInfills != null)
                    {
                        foreach (var solidInfill in island.SolidInfills)
                        {
                            foreach (var segment in solidInfill.Segments)
                            {
                                if (!geometries.ContainsKey((EntityType.INFILL_SOLID_P, absoluteZUM))) geometries.Add((EntityType.INFILL_SOLID_P, absoluteZUM), new List<List<Segment>>() { new List<Segment>() { segment } });
                                else geometries[(EntityType.INFILL_SOLID_P, absoluteZUM)].Add(new List<Segment>() { segment });
                            }
                        }
                    }

                    if (island.CellularInfills != null)
                    {
                        foreach (var cellularInfill in island.CellularInfills)
                        {
                            foreach (var segment in cellularInfill.Segments)
                            {
                                if (!geometries.ContainsKey((EntityType.INFILL_CELLULAR_P, absoluteZUM))) geometries.Add((EntityType.INFILL_CELLULAR_P, absoluteZUM), new List<List<Segment>>() { new List<Segment>() { segment } });
                                else geometries[(EntityType.INFILL_CELLULAR_P, absoluteZUM)].Add(new List<Segment>() { segment });
                            }
                        }
                    }

                    if (island.InsetsXFiber != null)
                    {
                        foreach (var insetXF in island.InsetsXFiber)
                        {
                            foreach (var (polygon, _) in insetXF.Polygons)
                            {
                                if (polygon.Any())
                                {
                                    if (!geometries.ContainsKey((EntityType.INSET_XF, absoluteZUM))) geometries.Add((EntityType.INSET_XF, absoluteZUM), new List<List<Segment>>() { polygon });
                                    else geometries[(EntityType.INSET_XF, absoluteZUM)].Add(polygon);
                                }
                            }
                        }
                    }

                    if (island.FiberInfill != null)
                    {
                        foreach (var fiberIfInfill in island.FiberInfill)
                        {
                            foreach (var polygon in fiberIfInfill.Polygons)
                            {
                                if (polygon.polygon.Any())
                                {
                                    if (!geometries.ContainsKey((EntityType.INFILL_FIBER, absoluteZUM))) geometries.Add((EntityType.INFILL_FIBER, absoluteZUM), new List<List<Segment>>() { polygon.polygon });
                                    else geometries[(EntityType.INFILL_FIBER, absoluteZUM)].Add(polygon.polygon);
                                }
                            }
                        }
                    }
                }

                void FillFromSupportIslandThick(ISupportIsland island, int absoluteZUM)
                {
                    if (island.Perimeters != null && island.Perimeters.Any())
                    {
                        foreach (var supportThickPerimeter in island.Perimeters)
                        {
                            if (supportThickPerimeter.Any())
                            {
                                if (!geometries.ContainsKey((EntityType.SUPPORT_THICK, absoluteZUM))) geometries.Add((EntityType.SUPPORT_THICK, absoluteZUM), new List<List<Segment>>() { supportThickPerimeter });
                                else geometries[(EntityType.SUPPORT_THICK, absoluteZUM)].Add(supportThickPerimeter);
                            }
                        }
                    }

                    if (island.Infill != null && island.Infill.Any())
                    {
                        foreach (var segment in island.Infill)
                        {
                            if (!geometries.ContainsKey((EntityType.SUPPORT_THICK, absoluteZUM))) geometries.Add((EntityType.SUPPORT_THICK, absoluteZUM), new List<List<Segment>>() { new List<Segment>() { segment } });
                            else geometries[(EntityType.SUPPORT_THICK, absoluteZUM)].Add(new List<Segment>() { segment });
                        }
                    }
                }

                void FillFromSupportIslandThin(ISupportIsland island, int absoluteZUM, bool isFL)
                {
                    if (island.Perimeters != null && island.Perimeters.Any())
                    {
                        foreach (var supportThinPerimeter in island.Perimeters)
                        {
                            if (supportThinPerimeter.Any())
                            {
                                if (!geometries.ContainsKey((EntityType.SUPPORT_THIN, absoluteZUM))) geometries.Add((EntityType.SUPPORT_THIN, absoluteZUM), new List<List<Segment>>() { supportThinPerimeter });
                                else geometries[(EntityType.SUPPORT_THIN, absoluteZUM)].Add(supportThinPerimeter);
                            }
                        }
                    }

                    if (island.Infill != null && island.Infill.Any())
                    {
                        foreach (var segment in island.Infill)
                        {

                            if (!geometries.ContainsKey((EntityType.SUPPORT_THIN, absoluteZUM))) geometries.Add((EntityType.SUPPORT_THIN, absoluteZUM), new List<List<Segment>>() { new List<Segment>() { segment } });
                            else geometries[(EntityType.SUPPORT_THIN, absoluteZUM)].Add(new List<Segment>() { segment });
                        }
                    }
                }

                if (_fl.ModelBrim != null && _fl.ModelBrim.BrimLoops.Any())
                {
                    foreach (var brimLoop in _fl.ModelBrim.BrimLoops)
                    {
                        foreach (var brimPolygon in brimLoop)
                        {
                            if (!geometries.ContainsKey((EntityType.BRIM, _fl.AbsoluteZUM))) geometries.Add((EntityType.BRIM, _fl.AbsoluteZUM), new List<List<Segment>>() { brimPolygon });
                            else geometries[(EntityType.BRIM, _fl.AbsoluteZUM)].Add(brimPolygon);
                        }
                    }
                }

                if (_fl.ModelSkirt != null && _fl.ModelSkirt.SkirtLoops.Any())
                {
                    foreach (var skirtLoop in _fl.ModelSkirt.SkirtLoops)
                    {
                        foreach (var skirtPolygon in skirtLoop)
                        {
                            if (!geometries.ContainsKey((EntityType.SKIRT, _fl.AbsoluteZUM))) geometries.Add((EntityType.SKIRT, _fl.AbsoluteZUM), new List<List<Segment>>() { skirtPolygon });
                            else geometries[(EntityType.SKIRT, _fl.AbsoluteZUM)].Add(skirtPolygon);
                        }
                    }
                }

                if (_fl.Models != null && _fl.Models.Any())
                {
                    foreach (var flModel in _fl.Models)
                    {
                        if (flModel.SupportsThick != null && flModel.SupportsThick.Any())
                        {
                            foreach (var supportIslandThick in flModel.SupportsThick)
                            {
                                FillFromSupportIslandThick(supportIslandThick, _fl.AbsoluteZUM);
                            }
                        }

                        if (flModel.SupportThin != null && flModel.SupportThin.Any())
                        {
                            foreach (var supportIslandThin in flModel.SupportThin)
                            {
                                FillFromSupportIslandThin(supportIslandThin, _fl.AbsoluteZUM, true);
                            }
                        }

                        if (flModel.Islands != null && flModel.Islands.Any())
                        {
                            foreach (var flModelIsland in flModel.Islands)
                            {
                                FillFromPrintIsland(flModelIsland, _fl.AbsoluteZUM);
                            }
                        }
                    }

                    foreach (var wtPrintPolygon in _fl.WTPrintPolygons)
                    {
                        if (!geometries.ContainsKey((EntityType.WIPE_TOWER, _fl.AbsoluteZUM))) geometries.Add((EntityType.WIPE_TOWER, _fl.AbsoluteZUM), new List<List<Segment>>() { wtPrintPolygon });
                        else geometries[(EntityType.WIPE_TOWER, _fl.AbsoluteZUM)].Add(wtPrintPolygon);
                    }

                    foreach (var wtPrintPolygonBrim in _fl.WTPrintPolygonsBrim)
                    {
                        if (!geometries.ContainsKey((EntityType.WIPE_TOWER, _fl.AbsoluteZUM))) geometries.Add((EntityType.WIPE_TOWER, _fl.AbsoluteZUM), new List<List<Segment>>() { wtPrintPolygonBrim });
                        else geometries[(EntityType.WIPE_TOWER, _fl.AbsoluteZUM)].Add(wtPrintPolygonBrim);
                    }
                }

                UpdateProgress(++i, _blocks.Count * 2, string.Empty, worker);

                if (_microLayers != null && _microLayers.Any())
                {
                    var del = _blocks.Count / _microLayers.Count;
                    foreach (var microLayer in _microLayers)
                    {
                        foreach (var microLayerModel in microLayer.Models)
                        {
                            if (microLayerModel.Islands != null && microLayerModel.Islands.Any())
                            {
                                foreach (var printIsland in microLayerModel.Islands)
                                {
                                    FillFromPrintIsland(printIsland, microLayer.AbsoluteZUM);
                                }
                            }

                            if (microLayerModel.SupportsThick != null && microLayerModel.SupportsThick.Any())
                            {
                                foreach (var supportIslandThick in microLayerModel.SupportsThick)
                                {
                                    FillFromSupportIslandThick(supportIslandThick, microLayer.AbsoluteZUM);
                                }
                            }

                            if (microLayerModel.SupportThin != null && microLayerModel.SupportThin.Any())
                            {
                                foreach (var supportIslandThin in microLayerModel.SupportThin)
                                {
                                    FillFromSupportIslandThin(supportIslandThin, microLayer.AbsoluteZUM, false);
                                }
                            }
                        }
                        foreach (var wtPrintPolygon in microLayer.WTPrintPolygons)
                        {
                            if (!geometries.ContainsKey((EntityType.WIPE_TOWER, _fl.AbsoluteZUM))) geometries.Add((EntityType.WIPE_TOWER, _fl.AbsoluteZUM), new List<List<Segment>>() { wtPrintPolygon });
                            else geometries[(EntityType.WIPE_TOWER, _fl.AbsoluteZUM)].Add(wtPrintPolygon);
                        }

                        i += del;
                        UpdateProgress(i, _blocks.Count * 2, string.Empty, worker);
                    }
                }


                //для каждого уникального сочетания тип геометрии + высота
                foreach (var geometry in geometries)
                {
                    var heightMM = Math.Round((double)geometry.Key.absoluteZUM / 1000, 3);
/*                    //находим цвет для сущности
                    var color = System.Drawing.Color.FromArgb(TRANSPARENCY,
                        _entityColors[geometry.Key.entityType].R,
                        _entityColors[geometry.Key.entityType].G,
                        _entityColors[geometry.Key.entityType].B);*/

                    var paths = new Point3D[geometry.Value.Count][];
                    double xMin = double.MaxValue, xMax = double.MinValue, yMin = double.MaxValue, yMax = double.MinValue, zMin = double.MaxValue, zMax = double.MinValue;
                    for (var index = 0; index < geometry.Value.Count; index++)
                    {
                        var point3ds = new Point3D[geometry.Value[index].Count + 1];
                        for (var sIndex = 0; sIndex < geometry.Value[index].Count; sIndex++)
                        {
                            point3ds[sIndex] = GetPoint3D(geometry.Value[index][sIndex].StartPoint);
                            point3ds[sIndex].Z = heightMM;
                            if (point3ds[sIndex].X < xMin) xMin = point3ds[sIndex].X;
                            if (point3ds[sIndex].X > xMax) xMax = point3ds[sIndex].X;
                            if (point3ds[sIndex].Y < yMin) yMin = point3ds[sIndex].Y;
                            if (point3ds[sIndex].Y > yMax) yMax = point3ds[sIndex].Y;
                            if (point3ds[sIndex].Z < zMin) zMin = point3ds[sIndex].Z;
                            if (point3ds[sIndex].Z > zMax) zMax = point3ds[sIndex].Z;
                        }
                        //преобразуем найденные точки в Point3D
                        point3ds[geometry.Value[index].Count] = GetPoint3D(geometry.Value[index][geometry.Value[index].Count - 1].EndPoint);
                        point3ds[geometry.Value[index].Count].Z = heightMM;
                        paths[index] = point3ds;
                    }

                    var boxMin = new Point3D(xMin, yMin, zMin);
                    var boxMax = new Point3D(xMax, yMax, zMax);

                    //создаем путь из набора путей как единую сущность
                    var multiLinearPath = new LinearPathMulti(paths, boxMin, boxMax, _layers.Single(a=>a.Name == geometry.Key.entityType.ToString()))
                    {
                        LineWeight = 1.5f,
                        LineWeightMethod = colorMethodType.byLayer
                    };

                    //в итоговый лист для каждого сочетания высота + тип добавляем все сущности, их блок и ссылку на блок
                    Geometries.Add((heightMM, geometry.Key.entityType), multiLinearPath);
                }

                #endregion

                #region BLOCKS

                var lastPoint = new IntPoint();

                var blockCopy = new IPPBlock[_blocks.Count];
                _blocks.CopyTo(blockCopy);

                var travelLayers = new Dictionary<int, List<List<ITravel>>>();
                var extrudePLayers = new Dictionary<int, List<IntPoint>>();
                var extrudePFLayers = new Dictionary<int, List<IntPoint>>();
                var movePLayers = new Dictionary<int, List<List<IMoveP>>>();
                var movePFLayers = new Dictionary<int, List<List<IMovePF>>>();
                var retractLayers = new Dictionary<int, List<IntPoint>>();
                var cutLayers = new Dictionary<int, List<IntPoint>>();

                //проходя по всем блокам, генерируем словарь для соответстия номера слоя к листу листов блоков, где каждый лист представляет собой замкнутый полигон
                for (var j = 0; j < _blocks.Count; j++)
                {
                    var currentMicroLayerIndex = _blocks[j].OwnMicroLayerIndex;

                    if (_blocks[j] is ITravel travelBlock)
                    {
                        lastPoint = travelBlock.Segment.EndPoint;

                        if (!travelLayers.ContainsKey(currentMicroLayerIndex)) travelLayers.Add(currentMicroLayerIndex, new List<List<ITravel>>());

                        if (travelLayers[currentMicroLayerIndex].LastOrDefault() == null ||
                            travelLayers[currentMicroLayerIndex].Last().LastOrDefault() == null ||
                            !travelLayers[currentMicroLayerIndex].Last().Last().Segment.EndPoint.Equals(travelBlock.Segment.StartPoint))
                        {
                            travelLayers[currentMicroLayerIndex].Add(new List<ITravel>() { travelBlock });
                            continue;
                        }

                        if (travelLayers[currentMicroLayerIndex].Last().Last().Segment.EndPoint.Equals(travelBlock.Segment.StartPoint))
                        {
                            travelLayers[currentMicroLayerIndex].Last().Add(travelBlock);
                            continue;
                        }
                    }

                    if (_blocks[j] is IExtrudeP)
                    {
                        if (!extrudePLayers.ContainsKey(currentMicroLayerIndex)) extrudePLayers.Add(currentMicroLayerIndex, new List<IntPoint>());
                        extrudePLayers[currentMicroLayerIndex].Add(lastPoint);
                    }

                    if (_blocks[j] is IExtrudePF)
                    {
                        if (!extrudePFLayers.ContainsKey(currentMicroLayerIndex)) extrudePFLayers.Add(currentMicroLayerIndex, new List<IntPoint>());
                        extrudePFLayers[currentMicroLayerIndex].Add(lastPoint);
                    }

                    if (_blocks[j] is IMoveP moveP)
                    {
                        lastPoint = moveP.Segment.EndPoint;

                        if (!movePLayers.ContainsKey(currentMicroLayerIndex)) movePLayers.Add(currentMicroLayerIndex, new List<List<IMoveP>>());

                        if (movePLayers[currentMicroLayerIndex].LastOrDefault() == null ||
                            movePLayers[currentMicroLayerIndex].Last().LastOrDefault() == null ||
                            !movePLayers[currentMicroLayerIndex].Last().Last().Segment.EndPoint.Equals(moveP.Segment.StartPoint))
                        {
                            movePLayers[currentMicroLayerIndex].Add(new List<IMoveP>() { moveP });
                            continue;
                        }

                        if (movePLayers[currentMicroLayerIndex].Last().Last().Segment.EndPoint.Equals(moveP.Segment.StartPoint))
                        {
                            movePLayers[currentMicroLayerIndex].Last().Add(moveP);
                            continue;
                        }
                    }

                    if (_blocks[j] is IMovePF movePF)
                    {
                        lastPoint = movePF.Segment.EndPoint;

                        if (!movePFLayers.ContainsKey(currentMicroLayerIndex)) movePFLayers.Add(currentMicroLayerIndex, new List<List<IMovePF>>());

                        if (movePFLayers[currentMicroLayerIndex].LastOrDefault() == null ||
                            movePFLayers[currentMicroLayerIndex].Last().LastOrDefault() == null ||
                            !movePFLayers[currentMicroLayerIndex].Last().Last().Segment.EndPoint.Equals(movePF.Segment.StartPoint))
                        {
                            movePFLayers[currentMicroLayerIndex].Add(new List<IMovePF>() { movePF });
                            continue;
                        }

                        if (movePFLayers[currentMicroLayerIndex].Last().Last().Segment.EndPoint.Equals(movePF.Segment.StartPoint))
                        {
                            movePFLayers[currentMicroLayerIndex].Last().Add(movePF);
                            continue;
                        }
                    }

                    if (_blocks[j] is IRetract)
                    {
                        if (!retractLayers.ContainsKey(currentMicroLayerIndex)) retractLayers.Add(currentMicroLayerIndex, new List<IntPoint>());
                        retractLayers[currentMicroLayerIndex].Add(lastPoint);
                    }

                    if (_blocks[j] is ICut)
                    {
                        if (!cutLayers.ContainsKey(currentMicroLayerIndex)) cutLayers.Add(currentMicroLayerIndex, new List<IntPoint>());
                        cutLayers[currentMicroLayerIndex].Add(lastPoint);
                    }

                    UpdateProgress(++i, _blocks.Count * 2, string.Empty, worker);
                }

                #region TRAVELS

                foreach (var travelLayer in travelLayers)
                {
                    var paths = new Point3D[travelLayer.Value.Count][];
                    double xMin = double.MaxValue, xMax = double.MinValue, yMin = double.MaxValue, yMax = double.MinValue, zMin = double.MaxValue, zMax = double.MinValue;
                    for (var index = 0; index < travelLayer.Value.Count; index++)
                    {
                        var point3ds = new Point3D[travelLayer.Value[index].Count + 1];
                        for (var sIndex = 0; sIndex < travelLayer.Value[index].Count; sIndex++)
                        {
                            point3ds[sIndex] = GetPoint3D(travelLayer.Value[index][sIndex].Segment.StartPoint);
                            if (point3ds[sIndex].X < xMin) xMin = point3ds[sIndex].X;
                            if (point3ds[sIndex].X > xMax) xMax = point3ds[sIndex].X;
                            if (point3ds[sIndex].Y < yMin) yMin = point3ds[sIndex].Y;
                            if (point3ds[sIndex].Y > yMax) yMax = point3ds[sIndex].Y;
                            if (point3ds[sIndex].Z < zMin) zMin = point3ds[sIndex].Z;
                            if (point3ds[sIndex].Z > zMax) zMax = point3ds[sIndex].Z;
                        }
                        //преобразуем найденные точки в Point3D
                        point3ds[travelLayer.Value[index].Count] = GetPoint3D(travelLayer.Value[index][travelLayer.Value[index].Count - 1].Segment.EndPoint);
                        paths[index] = point3ds;
                    }

                    var boxMin = new Point3D(xMin, yMin, zMin);
                    var boxMax = new Point3D(xMax, yMax, zMax);
                    //создаем путь из набора путей как единую сущность
                    var multiLinearPath = new LinearPathMulti(paths, boxMin, boxMax, _layers.Single(a=>a.Name == BlockType.TRAVEL.ToString()))
                    {
                        ColorMethod = colorMethodType.byLayer,
                        LineWeight = 1.5f,
                    };
                    NoBaseBlocks.Add((travelLayer.Key, BlockType.TRAVEL), multiLinearPath);
                }

                #endregion

                #region EXTRUDE P

                foreach (var extrudePLayer in extrudePLayers)
                {
                    BasedBlocks.Add((extrudePLayer.Key, BlockType.EXTRUDE_P), new List<BlockReference>());
                    foreach (var point in extrudePLayer.Value)
                    {
                        var centerPoint = GetPoint3D(point);

                        var blockReference = new BlockReference(centerPoint, _sourceBlocks[BlockType.EXTRUDE_P].name, 0)
                        {
                            ColorMethod = colorMethodType.byLayer,
                            Selectable = false
                        };
                        BasedBlocks[(extrudePLayer.Key, BlockType.EXTRUDE_P)].Add(blockReference);
                    }
                }

                #endregion

                #region EXTRUDE PF

                foreach (var extrudePFLayer in extrudePFLayers)
                {
                    BasedBlocks.Add((extrudePFLayer.Key, BlockType.EXTRUDE_PF), new List<BlockReference>());
                    foreach (var point in extrudePFLayer.Value)
                    {
                        var centerPoint = GetPoint3D(point);

                        var blockReference = new BlockReference(centerPoint, _sourceBlocks[BlockType.EXTRUDE_PF].name, 0)
                        {
                            ColorMethod = colorMethodType.byLayer,
                            Selectable = false
                        };
                        BasedBlocks[(extrudePFLayer.Key, BlockType.EXTRUDE_PF)].Add(blockReference);
                    }
                }


                #endregion

                #region MOVE P

                foreach (var movePLayer in movePLayers)
                {
                    var paths = new Point3D[movePLayer.Value.Count][];
                    double xMin = double.MaxValue, xMax = double.MinValue, yMin = double.MaxValue, yMax = double.MinValue, zMin = double.MaxValue, zMax = double.MinValue;
                    for (var index = 0; index < movePLayer.Value.Count; index++)
                    {
                        var point3ds = new Point3D[movePLayer.Value[index].Count + 1];
                        for (var sIndex = 0; sIndex < movePLayer.Value[index].Count; sIndex++)
                        {
                            point3ds[sIndex] = GetPoint3D(movePLayer.Value[index][sIndex].Segment.StartPoint);
                            if (point3ds[sIndex].X < xMin) xMin = point3ds[sIndex].X;
                            if (point3ds[sIndex].X > xMax) xMax = point3ds[sIndex].X;
                            if (point3ds[sIndex].Y < yMin) yMin = point3ds[sIndex].Y;
                            if (point3ds[sIndex].Y > yMax) yMax = point3ds[sIndex].Y;
                            if (point3ds[sIndex].Z < zMin) zMin = point3ds[sIndex].Z;
                            if (point3ds[sIndex].Z > zMax) zMax = point3ds[sIndex].Z;
                        }

                        point3ds[movePLayer.Value[index].Count] = GetPoint3D(movePLayer.Value[index][movePLayer.Value[index].Count - 1].Segment.EndPoint);
                        paths[index] = point3ds;
                    }

                    var boxMin = new Point3D(xMin, yMin, zMin);
                    var boxMax = new Point3D(xMax, yMax, zMax);
                    var multiLinearPath = new LinearPathMulti(paths, boxMin, boxMax, _layers.Single(a => a.Name == BlockType.MOVE_P.ToString()))
                    {
                        ColorMethod = colorMethodType.byLayer,
                        LineWeight = 1.5f,
                    };
                    NoBaseBlocks.Add((movePLayer.Key, BlockType.MOVE_P), multiLinearPath);
                }

                #endregion

                #region MOVE PF

                foreach (var movePFLayer in movePFLayers)
                {
                    var paths = new Point3D[movePFLayer.Value.Count][];
                    double xMin = double.MaxValue, xMax = double.MinValue, yMin = double.MaxValue, yMax = double.MinValue, zMin = double.MaxValue, zMax = double.MinValue;
                    for (var index = 0; index < movePFLayer.Value.Count; index++)
                    {
                        var point3ds = new Point3D[movePFLayer.Value[index].Count + 1];
                        for (var sIndex = 0; sIndex < movePFLayer.Value[index].Count; sIndex++)
                        {
                            point3ds[sIndex] = GetPoint3D(movePFLayer.Value[index][sIndex].Segment.StartPoint);
                            if (point3ds[sIndex].X < xMin) xMin = point3ds[sIndex].X;
                            if (point3ds[sIndex].X > xMax) xMax = point3ds[sIndex].X;
                            if (point3ds[sIndex].Y < yMin) yMin = point3ds[sIndex].Y;
                            if (point3ds[sIndex].Y > yMax) yMax = point3ds[sIndex].Y;
                            if (point3ds[sIndex].Z < zMin) zMin = point3ds[sIndex].Z;
                            if (point3ds[sIndex].Z > zMax) zMax = point3ds[sIndex].Z;
                        }

                        point3ds[movePFLayer.Value[index].Count] = GetPoint3D(movePFLayer.Value[index][movePFLayer.Value[index].Count - 1].Segment.EndPoint);
                        paths[index] = point3ds;
                    }
                    var boxMin = new Point3D(xMin, yMin, zMin);
                    var boxMax = new Point3D(xMax, yMax, zMax);
                    var multiLinearPath = new LinearPathMulti(paths, boxMin, boxMax, _layers.Single(a => a.Name == BlockType.MOVE_PF.ToString()))
                    {
                        ColorMethod = colorMethodType.byLayer,
                        LineWeight = 1.5f,
                    };
                    NoBaseBlocks.Add((movePFLayer.Key, BlockType.MOVE_PF), multiLinearPath);
                }

                #endregion

                #region RETRACT

                foreach (var retractLayer in retractLayers)
                {
                    BasedBlocks.Add((retractLayer.Key, BlockType.RETRACT), new List<BlockReference>());
                    foreach (var point in retractLayer.Value)
                    {
                        var centerPoint = GetPoint3D(point);

                        var blockReference = new BlockReference(centerPoint, _sourceBlocks[BlockType.RETRACT].name, 0)
                        {
                            ColorMethod = colorMethodType.byLayer,
                            Selectable = false
                        };
                        BasedBlocks[(retractLayer.Key, BlockType.RETRACT)].Add(blockReference);
                    }
                }


                #endregion

                #region CUT

                foreach (var cutLayer in cutLayers)
                {
                    BasedBlocks.Add((cutLayer.Key, BlockType.CUT), new List<BlockReference>());
                    foreach (var point in cutLayer.Value)
                    {
                        var centerPoint = GetPoint3D(point);

                        var blockReference = new BlockReference(centerPoint, _sourceBlocks[BlockType.CUT].name, 0)
                        {
                            ColorMethod = colorMethodType.byLayer,
                            Selectable = false
                        };
                        BasedBlocks[(cutLayer.Key, BlockType.CUT)].Add(blockReference);
                    }
                }

                #endregion

                #endregion
            }

            private static Point3D GetPoint3D(IntPoint point)
            {
                return new Point3D(Math.Round((double)point.X / 1000, 3),
                                   Math.Round((double)point.Y / 1000, 3),
                                   Math.Round((double)point.Z / 1000, 3));
            }
        }
    }
}