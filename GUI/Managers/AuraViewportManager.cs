using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Input;
using devDept.Eyeshot;
using devDept.Eyeshot.Entities;
using devDept.Eyeshot.Translators;
using devDept.Geometry;
using devDept.Graphics;
using Microsoft.Win32;
using MSClipperLib;
using Settings.Memento;
using Settings.Stores;
using Util.GeometryBasics;
using Xceed.Wpf.DataGrid.Converters;
using Environment = devDept.Eyeshot.Environment;
using Region = devDept.Eyeshot.Entities.Region;

namespace Aura.Managers
{
    public interface IAuraViewportManager
    {
        #region INITIALIZE

        void Initialize(AuraViewportLayout auraViewportLayout, ISessionStore sessionStore);

        #endregion

        //camera

        #region UPDATE LAYER COLOR

        void UpdateLayerColor(string layerName, Color color);

        #endregion

        #region UPDATE LAYER VISIBILITY

        void UpdateLayerVisibility(string layerName, bool visibility, bool needAdjust = true);

        #endregion

        #region CAMERA TO 2D

        void CameraTo2D(Point2D newCenter = null);

        #endregion

        #region CAMERA TO 3D

        void CameraTo3D(Point2D newCenter = null);

        #endregion

        #region ENTER LAYER RULE MODE

        void EnterLayerRuleMode(Entity highlight);

        #endregion

        #region EXIT LAYER RULES MODE

        void ExitLayerRulesMode();

        #endregion

        #region FOCUS

        void Focus();

        #endregion

        //entities

        #region ADD ENTITY

        void AddEntity(Entity entity, string layerName);

        void AddEntity(EntityList entity, string layerName);

        void AddTrueEntity(Entity entity);

        #endregion

        #region ADD BLOCK

        void AddBlock(Block block);

        #endregion

        #region ADD LAYER

        void AddLayer(Layer layer);

        #endregion

        #region REMOVE ENTITY

        void RemoveEntity(Entity entity);

        void RemoveEntities(List<Entity> entities);

        void RemoveEntities(List<BlockReference> blockReferences);

        #endregion

        #region REMOVE LAYER

        void RemoveLayer(Layer layer);

        #endregion

        #region CLEAR ENTITIES

        void ClearEntities();

        #endregion

        //materials

        #region SET ERROR MATERIAL FOR ENTITY

        void SetErrorMaterialForEntity(Entity entity);

        #endregion

        #region SET STANDART MATERIAL FOR ENTITY

        void SetStandartMaterialForEntity(Entity entity, colorMethodType colorMethodType = colorMethodType.byEntity);

        #endregion

        #region SET MATERIAL TO ENTITY

        void SetMaterialToEntity(Entity entity, MaterialTypes materialTypes, colorMethodType colorMethodType = colorMethodType.byEntity);

        #endregion

        //visibility

        #region SET VISIBILITY

        void SetVisibility(Entity entity, bool visibility);

        #endregion

        #region SET VISIBILITY FOR ALL

        void SetVisibilityForAll(bool visibility);

        #endregion

        //table

        #region NEED UPDATE TABLE

        bool NeedUpdateTable(double newWidth, double newLength);

        #endregion

        #region UPDATE TABLE

        void UpdateTable();

        #endregion

        #region PUT ENTITY ON TABLE

        void PutEntityOnTable(Entity entity);

        #endregion

        #region PUT ENTITIES TO TABLE CENTER

        void PutEntityToTableCenter(List<Entity> entities);
        void PutEntityToTableCenter(Entity entity);

        #endregion

        #region CHECK INTERSECTIONS

        void CheckIntersections();

        #endregion

        //select

        #region GET UNDER MOUSE

        Entity GetUnderMouse(MouseButtonEventArgs e);

        #endregion

        #region SELECT UNDER MOUSE

        Entity SelectUnderMouse(MouseButtonEventArgs e);

        #endregion

        #region DESELECT ALL

        void DeselectAll();

        #endregion

        #region SET SELECTION

        void SetSelection(Entity entity, bool isSelected);

        #endregion

        //object manipulator

        #region GET OBJECT MANIPULATOR

        ObjectManipulator GetObjectManipulator();

        #endregion

        #region CONFIGURE OBJECT MANIPULATOR FOR SHIFT

        void ConfigureObjectManipulatorForShift();

        #endregion

        #region CONFIGURE OBJECT MANIPULATOR FOR ROTATE

        void ConfigureObjectManipulatorForRotate();

        #endregion

        #region CONFIGURE OBJECT MANIPULATOR FOR RESIZE

        void ConfigureObjectManipulatorForResize();

        #endregion

        #region SET OBJECT MANIPULATOR TRANSFORMATION

        void SetObjectManipulatorTransformation(Transformation transformation);

        #endregion

        #region GET OBJECT MANIPULATOR TRANSFORMATION

        Transformation GetObjectManipulatorTransformation();

        #endregion

        #region GET OBJECT MANIPULATOR POSITION

        Point3D GetObjectManipulatorPosition();

        #endregion

        #region OBJECT MANIPULATOR APPLY

        void ObjectManipulatorApply();

        #endregion

        #region OBJECT MANIPULATOR CANCEL

        void ObjectManipulatorCancel();

        #endregion

        //misc

        #region ADD WORK UNIT

        void AddWorkUnit(WorkUnit workUnit);

        #endregion

        #region GO TO NON STANDART VIEW

        void GoToNonStandartView();

        #endregion

        #region GO TO STANDART VIEW

        void GoToStandartView();

        #endregion

        #region CODE VIEW

        void EnterViewCode();

        void ExitViewCode();

        #endregion

        #region IS STANDART VIEW

        bool IsStandartView { get; }

        #endregion


        event EventHandler<List<(Entity entity, string name)>> ModelsAdded;
        event EventHandler<List<CollisionType>> CollisionDetectionCalculated;

        
    }

    public enum CollisionType
    {
        NO,
        WITHIN_THEMSELVES,
        WITH_PRINT_AREA,
        BRIM_OR_SKIRT,
        WITH_WT
    }

    public class AuraViewportManager : IAuraViewportManager
    {
        private AuraViewportLayout _auraViewportLayout;
        private ISessionStore _sessionStore;

        private Grid _auraTable;

        public bool IsStandartView { get; private set; } = true;
        public event EventHandler<List<(Entity entity, string name)>> ModelsAdded;
        public event EventHandler<List<CollisionType>> CollisionDetectionCalculated;

        public void SaveToStl(Entity entity, string path)
        {
            var wpwm = new WriteParamsWithMaterials(_auraViewportLayout);
            var wo = new WriteSTL(wpwm, path);
            wo.DoWork();
        }

        #region INITIALIZE

        public void Initialize(AuraViewportLayout auraViewportLayout, ISessionStore sessionStore)
        {
            _auraViewportLayout = auraViewportLayout;
            _sessionStore = sessionStore;
            _auraViewportLayout.WorkCompleted += AuraViewportLayoutOnWorkCompleted;
            _auraViewportLayout.Focus();
        }

        #endregion

        //camera

        #region UPDATE LAYER COLOR

        public void UpdateLayerColor(string layerName, Color color)
        {
            _auraViewportLayout.Layers[layerName].Color = color;
            _auraViewportLayout.Invalidate();
        }

        #endregion

        #region UPDATE LAYER VISIBILITY

        public void UpdateLayerVisibility(string layerName, bool visibility, bool needAdjust = true)
        {
            _auraViewportLayout.Layers[layerName].Visible = visibility;
            if (needAdjust)
            {
                _auraViewportLayout.Entities.UpdateBoundingBox();
                _auraViewportLayout.AdjustNearAndFarPlanes();
            }
            _auraViewportLayout.Invalidate();
        }

        #endregion

        #region CAMERA TO 2D

        public void CameraTo2D(Point2D newCenter = null)
        {
            var target = newCenter == null ? GetEntitiesCenter() : newCenter;
            _auraViewportLayout.Rotate.Enabled = false;
            _auraViewportLayout.Camera.ProjectionMode = projectionType.Orthographic;
            _auraViewportLayout.SetView(viewType.Top);
            _auraViewportLayout.Camera.Target = new Point3D(target.X, target.Y, 0);
            _auraViewportLayout.AdjustNearAndFarPlanes();

            _auraViewportLayout.Invalidate();
        }

        #endregion

        #region CAMERA TO 3D

        public void CameraTo3D(Point2D newCenter = null)
        {
            var target = newCenter == null ? GetEntitiesCenter() : newCenter;
            _auraViewportLayout.Rotate.Enabled = true;
            _auraViewportLayout.Camera.ProjectionMode = projectionType.Perspective;
            _auraViewportLayout.SetView(viewType.Trimetric);
            _auraViewportLayout.Camera.Target = new Point3D(target.X, target.Y, 0);
            _auraViewportLayout.AdjustNearAndFarPlanes();

            _auraViewportLayout.Invalidate();
        }

        #endregion

        #region ENTER LAYER RULE MODE

        public void EnterLayerRuleMode(Entity highlight)
        {
            if (highlight == null) return;

            DeselectAll();
            foreach (var entity in _trueEntities)
            {
                SetMaterialToEntity(entity, !entity.Equals(highlight) ? MaterialTypes.ULTRA_TRANSPARENT : MaterialTypes.TRANSPARENT);
            }

            IsStandartView = false;
        }

        #endregion

        #region EXIT LAYER RULES MODE

        public void ExitLayerRulesMode()
        {
            foreach (var entity in _trueEntities)
            {
                SetMaterialToEntity(entity, MaterialTypes.NORMAL);
            }

            IsStandartView = true;
        }

        #endregion

        #region FOCUS

        public void Focus()
        {
            _auraViewportLayout.Focus();
        }

        #endregion

        //entities add/remove

        #region TRUE ENTITIES

        private readonly List<Entity> _trueEntities = new List<Entity>();

        #endregion

        #region GET ENTITIES CENTER

        public Point2D GetEntitiesCenter()
        {
            var allEntities = _trueEntities;

            if (allEntities.Count == 0)
            {
                var gridCenter = GetTableCenter();
                return new Point3D(gridCenter.X, gridCenter.Y, 0);
            }

            var minX = allEntities.Min(a => a.BoxMin.X);
            var maxX = allEntities.Max(a => a.BoxMax.X);
            var minY = allEntities.Min(a => a.BoxMin.Y);
            var maxY = allEntities.Max(a => a.BoxMax.Y);

            return new Point2D((minX + maxX) / 2, (minY + maxY) / 2);
        }

        #endregion

        #region ADD WORK UNIT

        public void AddWorkUnit(WorkUnit workUnit)
        {
            _auraViewportLayout.StartWork(workUnit);
        }

        #endregion

        #region AURA VIEWPORT LAYOUT ON WORK COMPLETED

        private void AuraViewportLayoutOnWorkCompleted(object sender, WorkCompletedEventArgs e)
        {
            if (e.WorkUnit is ReadFileAsync rfa)
            {
                if (rfa is MyReadSTL || rfa is MyReadSTEP || rfa is MyRead3DS || rfa is MyReadOBJ)
                {
                    foreach (var entity in rfa.Entities)
                    {
                        if (rfa is ReadSTEP rfaStep)
                        {
                            //приводим все к мм - возможно стоит подумать насчет того, чтобы унаследовать свой класс от ReadSTEP и устанавливать ему единицы измерения и переходы между ними.
                            switch (rfaStep.Units)
                            {
                                case linearUnitsType.Meters:
                                    entity.Scale(1000);
                                    break;
                                case linearUnitsType.Inches:
                                    entity.Scale(25.4);
                                    break;
                            }
                        }
                    }

                    if (rfa.Entities.Length > 0)
                    {
                        var entities = new List<(Entity entity, string name)>();
                        switch (rfa)
                        {
                            case MyReadSTEP myReadStep:
                                var allLeafs = GetLeafEntities(rfa.Entities.ToList(), myReadStep.MyBlocks);
                                if (allLeafs != null) entities.AddRange(allLeafs.Select(entity => (entity, myReadStep.FileName)));
                                break;
                            case MyReadSTL myReadStl:
                                entities.AddRange(myReadStl.Entities.Select(entity => (entity, myReadStl.FileName)));
                                break;
                            case MyRead3DS myRead3Ds:
                                entities.AddRange(myRead3Ds.Entities.Select(entity => (entity, myRead3Ds.FileName)));
                                break;
                            case MyReadOBJ myReadObj:
                                entities.AddRange(myReadObj.Entities.Select(entity => (entity, myReadObj.FileName)));
                                break;
                        }

                        if (rfa.Entities.Length == 1 || !(rfa is MyReadSTEP)) PutEntityToTableCenter(entities.Select(a => a.entity).ToList());

                        _auraViewportLayout.Invalidate();
                        ModelsAdded?.Invoke(this, entities);
                    }
                }

                _auraViewportLayout.Rotate.Enabled = true;
            }

            if (e.WorkUnit is MyCollisitionDetection cd)
            {
                var dict = new List<Entity>();
                foreach (var entityDatasItem in cd.Result)
                {
                    if (!dict.Contains(entityDatasItem.Item1.Entity)) dict.Add(entityDatasItem.Item1.Entity);
                    if (!dict.Contains(entityDatasItem.Item2.Entity)) dict.Add(entityDatasItem.Item2.Entity);
                }

                var collisionDict = new List<CollisionType>();

                void AddCollisionType(CollisionType ct)
                {
                    if (!collisionDict.Contains(ct)) collisionDict.Add(ct);
                }

                foreach (var trueEntity in _trueEntities)
                {
                    var inPrintArea = CheckEntityInPrintArea(trueEntity, cd.SessionMemento);
                    var brimSkirtInPrintArea = CheckBrimSkirtInPrintArea(trueEntity, cd.SessionMemento);
                    var withWT = CheckEntityIntersectionWithWT(trueEntity, cd.SessionMemento);

                    if (dict.Contains(trueEntity)) AddCollisionType(CollisionType.WITHIN_THEMSELVES);
                    if (!inPrintArea) AddCollisionType(CollisionType.WITH_PRINT_AREA);
                    if (!brimSkirtInPrintArea) AddCollisionType(CollisionType.BRIM_OR_SKIRT);
                    if (withWT) AddCollisionType(CollisionType.WITH_WT);
                    if (dict.Contains(trueEntity) || !inPrintArea || !brimSkirtInPrintArea || withWT)
                    {
                        SetErrorMaterialForEntity(trueEntity);
                    }
                    else SetStandartMaterialForEntity(trueEntity);
                }

                CollisionDetectionCalculated?.Invoke(this, collisionDict);
            }
        }

        #endregion



        #region OPEN MODEL

        public static List<Entity> GetLeafEntities(List<Entity> entities, List<Block> blocks)
        {
            List<Entity> GetLeafs(List<Entity> inEntities, Point3D translation)
            {
                var outEntites = new List<Entity>();
                foreach (var inEntity in inEntities)
                {
                    if (inEntity is BlockReference br)
                    {
                        var block = blocks.SingleOrDefault(a => a.Name == br.BlockName);
                        if (block?.Entities != null && block.Entities.Any())
                        {
                            var leafs = GetLeafs(block.Entities, translation + br.InsertionPoint - block.BasePoint);
                            if (leafs != null && leafs.Any()) outEntites.AddRange(leafs);
                        }
                    }
                    else
                    {
                        var clone = inEntity.Clone() as Entity;
                        clone.Translate(translation.X, translation.Y, translation.Z);
                        outEntites.Add(clone);
                    }
                }

                return outEntites;
            }

            return GetLeafs(entities.ToList(), new Point3D());
        }

        #endregion

        #region ADD ENTITY

        public void AddEntity(Entity entity, string layerName)
        {
            _auraViewportLayout.Entities.Add(entity, layerName);
            //entity.Regen(0.1f);
            _auraViewportLayout.Invalidate();
        }

        public void AddEntity(EntityList entity, string layerName)
        {
            _auraViewportLayout.Entities.AddRange(entity, layerName);
            //entity.Regen(0.1f);
            _auraViewportLayout.Invalidate();
        }

        public void AddTrueEntity(Entity entity)
        {
            entity.LayerName = "Default";
            _auraViewportLayout.Entities.Add(entity);
            SetStandartMaterialForEntity(entity);
            _trueEntities.Add(entity);
            _auraViewportLayout.Invalidate();
        }

        public void AddBlock(Block block)
        {
            _auraViewportLayout.Blocks.Add(block);
            _auraViewportLayout.Invalidate();
        }

        public void AddLayer(Layer layer)
        {
            _auraViewportLayout.Layers.Add(layer);
            _auraViewportLayout.Invalidate();
        }

        #endregion

        #region REMOVE ENTITY

        public void RemoveEntity(Entity entity)
        {
            _trueEntities.Remove(entity);
            _auraViewportLayout.Entities.Remove(entity);
            DeselectAll();
            _auraViewportLayout.Invalidate();
        }

        public void RemoveEntities(List<Entity> entities)
        {
            foreach (var entity in entities)
            {
                _auraViewportLayout.Entities.Remove(entity);
            }

            _auraViewportLayout.Invalidate();
        }

        public void RemoveEntities(List<BlockReference> blockReferences)
        {
            foreach (var entity in blockReferences)
            {
                _auraViewportLayout.Entities.Remove(entity);
            }

            _auraViewportLayout.Invalidate();
        }

        public void ClearEntities()
        {
            foreach (var trueEntity in _trueEntities)
            {
                RemoveEntity(trueEntity);
            }

            _auraViewportLayout.Invalidate();
        }

        #endregion

        #region REMOVE LAYER

        public void RemoveLayer(Layer layer)
        {
            _auraViewportLayout.Layers.Remove(layer);
        }

        #endregion

        //materials

        #region SET ERROR MATERIAL FOR ENTITY

        public void SetErrorMaterialForEntity(Entity entity)
        {
            SetMaterialToEntity(entity, MaterialTypes.ERROR);
        }

        #endregion

        #region SET STANDART MATERIAL FOR ENTITY

        public void SetStandartMaterialForEntity(Entity entity, colorMethodType colorMethodType = colorMethodType.byEntity)
        {
            SetMaterialToEntity(entity, MaterialTypes.NORMAL, colorMethodType);
        }

        #endregion

        #region SET MATERIAL TO ENTITY

        public void SetMaterialToEntity(Entity entity, MaterialTypes materialTypes, colorMethodType colorMethodType = colorMethodType.byEntity)
        {
            entity.MaterialName = _auraViewportLayout.AuraMaterials[materialTypes];
            entity.ColorMethod = colorMethodType;
            _auraViewportLayout.Invalidate();
        }

        #endregion

        //visibility

        #region SET VISIBILITY

        public void SetVisibility(Entity entity, bool visibility)
        {
            entity.SetVisibility(visibility, null);
            _auraViewportLayout.Invalidate();
        }

        #endregion

        #region SET VISIBILITY FOR ALL

        public void SetVisibilityForAll(bool visibility)
        {
            for (var i = 1; i < _auraViewportLayout.Entities.Count; i++)
            {
                _auraViewportLayout.Entities[i].Visible = visibility;
            }

            _auraViewportLayout.Invalidate();
        }

        #endregion

        //table

        #region SET TABLE

        private void SetTable(Point2D min, Point2D max, double height)
        {
            if (_auraTable == null) _auraTable = _auraViewportLayout.GetGrid();
            this._auraTable.Min = min;
            this._auraTable.Max = max;
            UpdateRotationSettings();
        }

        #endregion

        #region GET TABLE CENTER

        private Point2D GetTableCenter()
        {
            if (_auraTable == null) _auraTable = _auraViewportLayout.GetGrid();
            return new Point2D((_auraTable.Max.X + _auraTable.Min.X) / 2, (_auraTable.Max.Y + _auraTable.Min.Y) / 2);
        }

        #endregion

        #region NEED UPDATE TABLE

        public bool NeedUpdateTable(double newWidth, double newLength)
        {
            if (_auraTable == null) _auraTable = _auraViewportLayout.GetGrid();
            return !_auraTable.Max.X.Equals(newWidth) || !_auraTable.Max.Y.Equals(newLength);
        }

        #endregion

        #region UPDATE TABLE

        public void UpdateTable()
        {
            SetTable(new Point2D(0, 0), new Point2D(_sessionStore.Session.Printer.TrueWidth, _sessionStore.Session.Printer.TrueLength), _sessionStore.Session.Printer.TrueHeight);
            var tableCenter = GetTableCenter();
            _auraViewportLayout.Camera.Target = new Point3D(tableCenter.X, tableCenter.Y, 0);
            _auraViewportLayout.Camera.Distance = _sessionStore.Session.Printer.TrueWidth * 1.8;
            _auraViewportLayout.AdjustNearAndFarPlanes();
            CheckIntersections();
            _auraViewportLayout.Invalidate();
        }

        #endregion

        #region PUT ENTITY ON TABLE

        public void PutEntityOnTable(Entity entity)
        {
            var currentMinZ = entity.BoxMin.Z;

            var offset = 0 - currentMinZ;

            entity.Translate(0, 0, offset);
            _auraViewportLayout.Entities.Regen();
            _auraViewportLayout.Invalidate();
        }

        #endregion

        #region GET TRANSLATION TO TABLE CENTER

        public static Point2D GetEntityCenterXY(Entity entity)
        {
            var min = entity.BoxMin;
            var max = entity.BoxMax;

            return new Point2D((max.X + min.X) / 2, (max.Y + min.Y) / 2);
        }

        private Vector3D GetTranslationToTableCenter(List<Entity> entities)
        {
            var tableCenter = GetTableCenter();

            var allCenters = new List<Point2D>();
            foreach (var entity in entities)
            {
                var entCenter = GetEntityCenterXY(entity);
                allCenters.Add(entCenter);
            }

            var centerMassX = (int) ((allCenters.Max(a => a.X) + allCenters.Min(a => a.X)) / 2);
            var centerMassY = (int) ((allCenters.Max(a => a.Y) + allCenters.Min(a => a.Y)) / 2);

            var deltaX = tableCenter.X - centerMassX;
            var deltaY = tableCenter.Y - centerMassY;

            var minZ = entities.Min(a => a.BoxMin.Z);

            return new Vector3D(deltaX, deltaY, 0 - minZ);
        }

        private Vector3D GetTranslationToTableCenter(Entity entity)
        {
            var tableCenter = GetTableCenter();

            var entCenter = GetEntityCenterXY(entity);

            var deltaX = tableCenter.X - entCenter.X;
            var deltaY = tableCenter.Y - entCenter.Y;

            var minZ = entity.BoxMin.Z;

            return new Vector3D(deltaX, deltaY, 0 - minZ);
        }

        #endregion

        #region PUT ENTITIES AT TABLE CENTER

        public void PutEntityToTableCenter(List<Entity> entities)
        {
            if (entities == null || !entities.Any()) return;

            foreach (var entity in entities)
            {
                entity.Regen(0.1f);
            }

            var translation = GetTranslationToTableCenter(entities);
            foreach (var entity in entities)
            {
                entity.Translate(translation);
            }

            _auraViewportLayout.Invalidate();
        }

        public void PutEntityToTableCenter(Entity entity)
        {
            if (entity == null) return;
            var translation = GetTranslationToTableCenter(entity);
            entity.Translate(translation);
            _auraViewportLayout.Invalidate();
        }

        #endregion

        #region CHECK INTERSECTIONS

        public void CheckIntersections()
        {
            if (_trueEntities == null || !_trueEntities.Any()) return;

            var availableEntitites = _trueEntities.Where(a => a.Visible).ToList();
            var cd = new MyCollisitionDetection(availableEntitites, 
                _auraViewportLayout.Blocks, 
                _sessionStore.Session,
                false,
                CollisionDetection2D.collisionCheckType.OBWithSubdivisionTree, 
                5);
            _auraViewportLayout.StartWork(cd);
        }

        #endregion

        #region CHECK ENTITY IN PRINT AREA

        private bool CheckEntityInPrintArea(Entity entity, ISessionMemento sessionMemento)
        {
            var entityBoxMinX = entity.BoxMin.X;
            var entityBoxMinY = entity.BoxMin.Y;
            var entityBoxMaxX = entity.BoxMax.X;
            var entityBoxMaxY = entity.BoxMax.Y;

            if (entityBoxMinX < 0 - 1 
                || entityBoxMinY < 0 - 1 
                || entityBoxMaxX > sessionMemento.Printer.TrueWidth + 1 
                || entityBoxMaxY > sessionMemento.Printer.TrueLength + 1 
                || entity.BoxMin.Z < 0 - 1 || entity.BoxMax.Z > sessionMemento.Printer.TrueHeight + 1)
            {
                return false;
            }

            return true;
        }

        #endregion

        #region CHECK BRIM SKIRT IN PRINT AREA

        private bool CheckBrimSkirtInPrintArea(Entity entity, ISessionMemento sessionMemento)
        {
            var brimDistance = sessionMemento.GetBrimDistance();
            var skirtDistance = sessionMemento.GetSkirtDistance();
            var offsetBrimSkirtS = Math.Max(brimDistance, skirtDistance);
            var entityBoxMinXWithBrimSkirt = entity.BoxMin.X - offsetBrimSkirtS;
            var entityBoxMinYWithBrimSkirt = entity.BoxMin.Y - offsetBrimSkirtS;
            var entityBoxMaxXWithBrimSkirt = entity.BoxMax.X - offsetBrimSkirtS;
            var entityBoxMaxYWithBrimSkirt = entity.BoxMax.Y - offsetBrimSkirtS;

            if (entityBoxMinXWithBrimSkirt < 0 - 1
                || entityBoxMinYWithBrimSkirt < 0 - 1
                || entityBoxMaxXWithBrimSkirt > sessionMemento.Printer.TrueWidth + 1
                || entityBoxMaxYWithBrimSkirt > sessionMemento.Printer.TrueLength + 1
                || entity.BoxMin.Z < 0 - 1 || entity.BoxMax.Z > sessionMemento.Printer.TrueHeight + 1)
            {
                return false;
            }

            return true;
        }

        #endregion

        #region CHECK ENTITY INTERSECTION WITH WT

        private bool CheckEntityIntersectionWithWT(Entity entity, ISessionMemento sessionMemento)
        {
            var entityBoxMinX = (int)(entity.BoxMin.X * 1000);
            var entityBoxMinY = (int)(entity.BoxMin.Y * 1000);
            var entityBoxMaxX = (int)(entity.BoxMax.X * 1000);
            var entityBoxMaxY = (int)(entity.BoxMax.Y * 1000);

            var entityPolygon = new List<IntPoint>()
            {
                new IntPoint(entityBoxMinX, entityBoxMinY),
                new IntPoint(entityBoxMaxX, entityBoxMinY),
                new IntPoint(entityBoxMaxX, entityBoxMaxY),
                new IntPoint(entityBoxMinX, entityBoxMaxY),
            };
            
            var wtBox = sessionMemento.GetWTBox();

            var wtBoxMinX = (int)(wtBox.boxMin.X * 1000);
            var wtBoxMinY = (int)(wtBox.boxMin.Y * 1000);
            var wtBoxMaxX = (int)(wtBox.boxMax.X * 1000);
            var wtBoxMaxY = (int)(wtBox.boxMax.Y * 1000);

            var wtPolygon = new List<IntPoint>()
            {
                new IntPoint(wtBoxMinX, wtBoxMinY),
                new IntPoint(wtBoxMaxX, wtBoxMinY),
                new IntPoint(wtBoxMaxX, wtBoxMaxY),
                new IntPoint(wtBoxMinX, wtBoxMaxY),
            };
            
            return entityPolygon.HasIntersect(wtPolygon);
        }

        #endregion

        //select

        #region GET UNDER MOUSE

        public Entity GetUnderMouse(MouseButtonEventArgs e)
        {
            try
            {
                var currentSelection = _auraViewportLayout.GetItemUnderMouseCursor(RenderContextUtility.ConvertPoint(_auraViewportLayout.GetMousePosition(e)));
                if (currentSelection == null) return null;
                var selectedEntityIndex = _auraViewportLayout.GetEntityUnderMouseCursor(RenderContextUtility.ConvertPoint(_auraViewportLayout.GetMousePosition(e)));
                return _auraViewportLayout.Entities[selectedEntityIndex];
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

        #region SELECT

        public Entity SelectUnderMouse(MouseButtonEventArgs e)
        {
            var currentSelection = GetUnderMouse(e);
            if (currentSelection != null) SetSelection(currentSelection, true);

            return currentSelection;
        }

        public void SetSelection(Entity entity, bool isSelected)
        {
            if (entity.GetSelection() == isSelected) return;
            entity.SetSelection(isSelected);
            _auraViewportLayout.Invalidate();
        }

        #endregion

        #region DESELECT ALL

        public void DeselectAll()
        {
            foreach (var trueEntity in _trueEntities)
            {
                if (trueEntity.GetSelection()) trueEntity.SetSelection(false);
            }

            _auraViewportLayout.Invalidate();
        }

        #endregion

        //object manipulator

        #region GET OBJECT MANIPULATOR

        public ObjectManipulator GetObjectManipulator()
        {
            return _auraViewportLayout.ObjectManipulator;
        }

        #endregion

        #region CONFIGURE OBJECT MANIPULATOR FOR SHIFT

        public void ConfigureObjectManipulatorForShift()
        {
            _auraViewportLayout.ObjectManipulator.Enable(new Identity(), true);
            _auraViewportLayout.ObjectManipulator.Entities.Clear();
            var omOrig = Mesh.CreateBox(0.1, 0.1, 0.1);
            omOrig.Translate(-0.05, -0.05, -0.05);
            _auraViewportLayout.ObjectManipulator.Entities.Add(omOrig);

            var meshX = Mesh.CreateArrow(new Point3D(0.0, 0.0, 0.0), Vector3D.AxisX, 1, 20, 2, 4, 20, Mesh.natureType.ColorSmooth,
                Mesh.edgeStyleType.Sharp);
            meshX.Translate(-5, 0, 0);

            var meshY = (Mesh) meshX.Clone();
            meshY.Rotate(Math.PI / 2, Vector3D.AxisZ);

            _auraViewportLayout.ObjectManipulator.Entities.Add(meshX);
            _auraViewportLayout.ObjectManipulator.Entities.Add(meshY);

            _auraViewportLayout.ObjectManipulator.TranslateX.Color = Color.Green;
            _auraViewportLayout.ObjectManipulator.TranslateY.Color = Color.Blue;

            _auraViewportLayout.ObjectManipulator.ShowOriginalWhileEditing = false;

            _auraViewportLayout.CompileUserInterfaceElements();
            _auraViewportLayout.Invalidate();
        }

        #endregion

        #region CONFIGURE OBJECT MANIPULATOR FOR ROTATE

        public void ConfigureObjectManipulatorForRotate()
        {
            _auraViewportLayout.ObjectManipulator.Enable(new Identity(), true);
            _auraViewportLayout.ObjectManipulator.Entities.Clear();
            var omOrig = Mesh.CreateBox(0.1, 0.1, 0.1);
            omOrig.Translate(-0.05, -0.05, -0.05);
            _auraViewportLayout.ObjectManipulator.Entities.Add(omOrig);

            var meshX = Mesh.CreateBox(0.1, 0.1, 0.1);
            meshX.Translate(-0.05, -0.05, -0.05);

            var meshY = (Mesh) meshX.Clone();

            var meshZ = (Mesh) meshX.Clone();

            var regX = new Region(new Circle(Plane.XY, new Point2D(0, 8), 0.6));
            var arcX = regX.RevolveAsMesh(0, 2 * Math.PI, -1 * Vector3D.AxisX, Point3D.Origin, 100, 0.1, Mesh.natureType.Smooth);

            var regY = new Region(new Circle(Plane.XY, new Point2D(8, 0), 0.6));
            var arcY = regY.RevolveAsMesh(0, 2 * Math.PI, -1 * Vector3D.AxisY, Point3D.Origin, 100, 0.1, Mesh.natureType.Smooth);

            var regZ = new Region(new Circle(Plane.XZ, new Point2D(8, 0), 0.6));
            var arcZ = regZ.RevolveAsMesh(0, 2 * Math.PI, -1 * Vector3D.AxisZ, Point3D.Origin, 100, 0.1, Mesh.natureType.Smooth);

            _auraViewportLayout.ObjectManipulator.Entities.Add(meshX);
            _auraViewportLayout.ObjectManipulator.Entities.Add(meshY);
            _auraViewportLayout.ObjectManipulator.Entities.Add(meshZ);

            _auraViewportLayout.ObjectManipulator.Entities.Add(arcX);
            _auraViewportLayout.ObjectManipulator.Entities.Add(arcY);
            _auraViewportLayout.ObjectManipulator.Entities.Add(arcZ);

            _auraViewportLayout.ObjectManipulator.RotateX.Color = Color.Green;
            _auraViewportLayout.ObjectManipulator.RotateY.Color = Color.Blue;
            _auraViewportLayout.ObjectManipulator.RotateZ.Color = Color.Red;

            _auraViewportLayout.ObjectManipulator.ShowOriginalWhileEditing = false;

            _auraViewportLayout.CompileUserInterfaceElements();
            _auraViewportLayout.Invalidate();
        }

        #endregion

        #region CONFIGURE OBJECT MANIPULATOR FOR RESIZE

        public void ConfigureObjectManipulatorForResize()
        {
            _auraViewportLayout.ObjectManipulator.Enable(new Identity(), true);
            _auraViewportLayout.ObjectManipulator.Entities.Clear();
            _auraViewportLayout.ObjectManipulator.ShowOriginalWhileEditing = false;
            _auraViewportLayout.CompileUserInterfaceElements();
            _auraViewportLayout.Invalidate();
        }

        #endregion

        #region SET OBJECT MANIPULATOR TRANSFORMATION

        public void SetObjectManipulatorTransformation(Transformation transformation)
        {
            _auraViewportLayout.ObjectManipulator.Transformation = transformation;
            _auraViewportLayout.Invalidate();
        }

        #endregion

        #region GET OBJECT MANIPULATOR TRANSFORMATION

        public Transformation GetObjectManipulatorTransformation()
        {
            return _auraViewportLayout.ObjectManipulator.Transformation;
        }

        #endregion

        #region GET OBJECT MANIPULATOR POSITION

        public Point3D GetObjectManipulatorPosition()
        {
            return _auraViewportLayout.ObjectManipulator.Position;
        }

        #endregion

        #region OBJECT MANIPULATOR APPLY

        public void ObjectManipulatorApply()
        {
            _auraViewportLayout.ObjectManipulator.Apply();
            _auraViewportLayout.Invalidate();
        }

        #endregion

        #region OBJECT MANIPULATOR CANCEL

        public void ObjectManipulatorCancel()
        {
            _auraViewportLayout.ObjectManipulator.Cancel();
            _auraViewportLayout.Invalidate();
        }

        #endregion

        //misc

        #region GO TO STANDART VIEW

        public void GoToNonStandartView()
        {
            _auraViewportLayout.SaveView(out _cameraBeforeCodeView);
            IsStandartView = false;
        }

        #endregion

        #region GO TO STANDART VIEW

        public void GoToStandartView()
        {
            CameraTo3D();
            _auraViewportLayout.RestoreView(_cameraBeforeCodeView);
            IsStandartView = true;
        }

        #endregion

        #region CODE VIEW

        private Camera _cameraBeforeCodeView;

        public void EnterViewCode()
        {
            _auraViewportLayout.SaveView(out _cameraBeforeCodeView);
            _auraViewportLayout.Rendered.PlanarReflections = false;

            DeselectAll();
            SetVisibilityForAll(false);

            _auraViewportLayout.Light2.Active = false;
            IsStandartView = false;
        }

        public void ExitViewCode()
        {
            _auraViewportLayout.Light2.Active = true;
            _auraViewportLayout.Rendered.PlanarReflections = true;

            SetVisibilityForAll(true);

            var target = GetEntitiesCenter();
            CameraTo3D(new Point2D(target.X, target.Y));
            IsStandartView = true;

            _auraViewportLayout.RestoreView(_cameraBeforeCodeView);
        }

        #endregion

        #region GET MESH

        public static Mesh GetMesh(Entity entity)
        {
            switch (entity)
            {
                case Mesh rawMesh:
                    return rawMesh;
                case Brep rawBrep:
                    return rawBrep.ConvertToMesh();
                default:
                    throw new Exception("New type");
            }
        }

        #endregion

        #region UPDATE ROTATION SETTINGS

        private void UpdateRotationSettings()
        {
            if (_auraTable == null) _auraTable = _auraViewportLayout.GetGrid();
            var tableCenter = GetTableCenter();
            var rotationSettings = new RotateSettings
            {
                Center = new Point3D(tableCenter.X, tableCenter.Y, 0),
                RotationCenter = rotationCenterType.Point
            };
            _auraViewportLayout.Rotate = rotationSettings;
        }

        #endregion
    }

    public class MyReadSTEP : ReadSTEP
    {
        public List<Block> MyBlocks => base.Blocks.ToList();

        public string FileName { get; set; }

        public MyReadSTEP(Stream stream, bool includeWires = false) : base(stream, includeWires)
        {
        }

        public MyReadSTEP(string fileName, bool includeWires = false) : base(fileName, includeWires)
        {
        }
    }

    public class MyReadSTL : ReadSTL
    {
        public MyReadSTL(string fileName, bool lightWeight, bool splitDisjoint, Mesh.natureType meshNature) : base(fileName, lightWeight,
            splitDisjoint, meshNature)
        {
        }

        public MyReadSTL(string fileName, bool lightWeight, Mesh.natureType meshNature) : base(fileName, lightWeight, meshNature)
        {
        }

        public MyReadSTL(string fileName, Mesh.natureType meshNature) : base(fileName, meshNature)
        {
        }

        public MyReadSTL(string fileName, bool lightWeight) : base(fileName, lightWeight)
        {
        }

        public MyReadSTL(string fileName) : base(fileName)
        {
        }

        public MyReadSTL(Stream stream) : base(stream)
        {
        }

        public MyReadSTL(Stream stream, bool lightWeight) : base(stream, lightWeight)
        {
        }

        public MyReadSTL(Stream stream, Mesh.natureType meshNature) : base(stream, meshNature)
        {
        }

        public MyReadSTL(Stream stream, bool lightWeight, Mesh.natureType meshNature) : base(stream, lightWeight, meshNature)
        {
        }

        public MyReadSTL(Stream stream, bool lightWeight, bool splitDisjoint, Mesh.natureType meshNature) : base(stream, lightWeight, splitDisjoint,
            meshNature)
        {
        }

        public string FileName { get; set; }
    }

    public class MyRead3DS : Read3DS
    {
        public string FileName { get; set; }

        public MyRead3DS(string fileName) : base(fileName)
        {
        }

        public MyRead3DS(string fileName, double smoothingAngle) : base(fileName, smoothingAngle)
        {
        }
    }

    public class MyReadOBJ : ReadOBJ
    {
        public string FileName { get; set; }

        public MyReadOBJ(string fileName, string matName, Mesh.edgeStyleType edgeStyle) : base(fileName, matName, edgeStyle)
        {
        }

        public MyReadOBJ(string fileName, bool plain, Mesh.edgeStyleType edgeStyle) : base(fileName, plain, edgeStyle)
        {
        }

        public MyReadOBJ(Stream stream, Mesh.edgeStyleType edgeStyle) : base(stream, edgeStyle)
        {
        }

        public MyReadOBJ(Stream objStream, Stream materialStream, Dictionary<string, Stream> texturesStream, Mesh.edgeStyleType edgeStyle) : base(
            objStream, materialStream, texturesStream, edgeStyle)
        {
        }

        public MyReadOBJ(string fileName, bool plain) : base(fileName, plain)
        {
        }

        public MyReadOBJ(string fileName, Mesh.edgeStyleType edgeStyle) : base(fileName, edgeStyle)
        {
        }

        public MyReadOBJ(string fileName) : base(fileName)
        {
        }
    }

    public class MyCollisitionDetection : CollisionDetection
    {
        public double TableWidth => SessionMemento.Printer.TrueWidth;
        public double TableLength => SessionMemento.Printer.TrueLength;
        public double TableHeight => SessionMemento.Printer.TrueHeight;
        public ISessionMemento SessionMemento { get; private set; }

        public MyCollisitionDetection(IList<Entity> list1, 
            IList<Entity> list2, 
            BlockKeyedCollection blocksCollection, 
            ISessionMemento sessionMemento, 
            bool firstOnly = true,
            collisionCheckType checkMethod = collisionCheckType.OB, 
            int maxTrianglesNumForOctreeNode = 0)
            : base(list1, list2, blocksCollection, firstOnly, checkMethod, maxTrianglesNumForOctreeNode)
        {
            SessionMemento = sessionMemento;
        }

        public MyCollisitionDetection(IList<Entity> list, 
            BlockKeyedCollection blocksCollection, 
            ISessionMemento sessionMemento,
            bool firstOnly = true,
            collisionCheckType collisionCheckMethod = collisionCheckType.OB, 
            int maxTrianglesNumForOctreeNode = 0)
            : base(list, blocksCollection, firstOnly, collisionCheckMethod, maxTrianglesNumForOctreeNode)
        {
            SessionMemento = sessionMemento;
        }
    }
}