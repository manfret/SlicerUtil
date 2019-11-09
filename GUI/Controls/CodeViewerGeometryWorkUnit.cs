using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using Aura.Controls;
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
using Block = devDept.Eyeshot.Block;

namespace Aura.Obsolete
{
    public sealed partial class CodeViewer
    {
        private sealed class CodeViewerWorkUnit : WorkUnit
        {
            private readonly IFirstLayer _fl;
            private readonly List<IMicroLayer> _microLayers;
            private readonly List<IPPBlock> _blocks;

            //словарь хранения геометрии
            public Dictionary<(double zMM, EntityType entityType), Entity> Geometries { get; }
            //public Dictionary<(double zMM, EntityType entityType), (List<Entity> baseEntities, Block block, BlockReference blockReference)> Geometries { get; }
            //словарь хранения блоков кода, у которых есть общая исходная сущность
            public Dictionary<(int layerIndex, BlockType blockType), List<BlockReference>> BasedBlocks { get; }
            //словарь хранения блоков кода, у каждой из которых своя независимая сущность
            public Dictionary<(int layerIndex, BlockType blockType), Entity> NoBaseBlocks { get; }
            //public Dictionary<(int layerIndex, BlockType blockType), (BlockReference blockReference, Block block)> NoBaseBlocks { get; }

            public CodeViewerWorkUnit(
                IFirstLayer fl, 
                List<IMicroLayer> microLayers,
                List<IPPBlock> blocks)
            {
                _fl = fl;
                _microLayers = microLayers;
                _blocks = blocks;
                
                //Geometries = new Dictionary<(double zMM, EntityType entityType), (List<Entity> baseEntities, Block block, BlockReference blockReference)>();
                Geometries = new Dictionary<(double zMM, EntityType entityType), Entity>();
                BasedBlocks = new Dictionary<(int layerIndex, BlockType blockType), List<BlockReference>>();
                //NoBaseBlocks = new Dictionary<(int layerIndex, BlockType blockType), (BlockReference blockReference, Block block)>();
                NoBaseBlocks = new Dictionary<(int layerIndex, BlockType blockType), Entity>();
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
                        foreach (var polygon in island.Insets0Plastic.Polygons)
                        {
                            if (polygon.Any())
                            {
                                if (!geometries.ContainsKey((EntityType.INSET0, absoluteZUM))) geometries.Add((EntityType.INSET0, absoluteZUM), new List<List<Segment>>() { polygon });
                                else geometries[(EntityType.INSET0, absoluteZUM)].Add(polygon);
                            }
                        }
                    }

                    if (island.MicroInfill != null)
                    {
                        foreach (var microInfill in island.MicroInfill)
                        {
                            if (microInfill.Inners == null) continue;
                            foreach (var microInfillInner in microInfill.Inners)
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
                            foreach (var segment in solidInfill.Inners)
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
                            foreach (var (polygon, _) in insetXF.InnersPolygons)
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
                            if (!geometries.ContainsKey((EntityType.SUPPORT_THICK, absoluteZUM))) geometries.Add((EntityType.SUPPORT_THICK, absoluteZUM), new List<List<Segment>>() { new List<Segment>(){ segment } });
                            else geometries[(EntityType.SUPPORT_THICK, absoluteZUM)].Add(new List<Segment>(){ segment });
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

                            if (!geometries.ContainsKey((EntityType.SUPPORT_THIN, absoluteZUM))) geometries.Add((EntityType.SUPPORT_THIN, absoluteZUM), new List<List<Segment>>() { new List<Segment>(){ segment } });
                            else geometries[(EntityType.SUPPORT_THIN, absoluteZUM)].Add(new List<Segment>(){ segment });
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
                    var heightMM = Math.Round((double) geometry.Key.absoluteZUM / 1000, 3);
                    //находим цвет для сущности
                    var color = Color.FromArgb(TRANSPARENCY,
                        _entityColors[geometry.Key.entityType].R,
                        _entityColors[geometry.Key.entityType].G,
                        _entityColors[geometry.Key.entityType].B);

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
                    var multiLinearPath = new LinearPathMulti(paths, boxMin, boxMax)
                    {
                        Color = color,
                        LineWeight = 1.5f,
                        LineWeightMethod = colorMethodType.byEntity
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
                        lastPoint = travelBlock.MoveSegment.EndPoint;

                        if (!travelLayers.ContainsKey(currentMicroLayerIndex)) travelLayers.Add(currentMicroLayerIndex, new List<List<ITravel>>());

                        if (travelLayers[currentMicroLayerIndex].LastOrDefault() == null ||
                            travelLayers[currentMicroLayerIndex].Last().LastOrDefault() == null ||
                            !travelLayers[currentMicroLayerIndex].Last().Last().MoveSegment.EndPoint.Equals(travelBlock.MoveSegment.StartPoint))
                        {
                            travelLayers[currentMicroLayerIndex].Add(new List<ITravel>(){ travelBlock });
                            continue;
                        }

                        if (travelLayers[currentMicroLayerIndex].Last().Last().MoveSegment.EndPoint.Equals(travelBlock.MoveSegment.StartPoint))
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
                            point3ds[sIndex] = GetPoint3D(travelLayer.Value[index][sIndex].MoveSegment.StartPoint);
                            if (point3ds[sIndex].X < xMin) xMin = point3ds[sIndex].X;
                            if (point3ds[sIndex].X > xMax) xMax = point3ds[sIndex].X;
                            if (point3ds[sIndex].Y < yMin) yMin = point3ds[sIndex].Y;
                            if (point3ds[sIndex].Y > yMax) yMax = point3ds[sIndex].Y;
                            if (point3ds[sIndex].Z < zMin) zMin = point3ds[sIndex].Z;
                            if (point3ds[sIndex].Z > zMax) zMax = point3ds[sIndex].Z;
                        }
                        //преобразуем найденные точки в Point3D
                        point3ds[travelLayer.Value[index].Count] = GetPoint3D(travelLayer.Value[index][travelLayer.Value[index].Count - 1].MoveSegment.EndPoint);
                        paths[index] = point3ds;
                    }

                    var boxMin = new Point3D(xMin, yMin, zMin);
                    var boxMax = new Point3D(xMax, yMax, zMax);
                    //создаем путь из набора путей как единую сущность
                    var multiLinearPath = new LinearPathMulti(paths, boxMin, boxMax)
                    {
                        Color = Color.FromArgb(TRANSPARENCY,
                            _blockColors[BlockType.TRAVEL].R,
                            _blockColors[BlockType.TRAVEL].G,
                            _blockColors[BlockType.TRAVEL].B),
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

                        var blockReference = new BlockReference(centerPoint, _sourceBlocks[BlockType.EXTRUDE_P].name, 0);
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

                        var blockReference = new BlockReference(centerPoint, _sourceBlocks[BlockType.EXTRUDE_PF].name, 0);
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
                    var multiLinearPath = new LinearPathMulti(paths, boxMin, boxMax)
                    {
                        Color = Color.FromArgb(TRANSPARENCY,
                            _blockColors[BlockType.MOVE_P].R,
                            _blockColors[BlockType.MOVE_P].G,
                            _blockColors[BlockType.MOVE_P].B),
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
                    var multiLinearPath = new LinearPathMulti(paths, boxMin, boxMax)
                    {
                        Color = Color.FromArgb(TRANSPARENCY,
                            _blockColors[BlockType.MOVE_PF].R,
                            _blockColors[BlockType.MOVE_PF].G,
                            _blockColors[BlockType.MOVE_PF].B),
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

                        var blockReference = new BlockReference(centerPoint, _sourceBlocks[BlockType.RETRACT].name, 0);
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

                        var blockReference = new BlockReference(centerPoint, _sourceBlocks[BlockType.CUT].name, 0);
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