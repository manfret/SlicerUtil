using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using devDept.Eyeshot;
using devDept.Eyeshot.Entities;
using devDept.Geometry;
using devDept.Graphics;
using Unity.Interception.Utilities;

namespace Aura.Managers
{
    public interface ILayupViewerManager
    {
        #region INITIALIZE

        void Initialize(AuraViewportLayout auraViewportLayout);

        #endregion

        #region ENTER MODE

        void EnterMode(List<Entity> allActiveEntities,
            Entity interactiveEntity,
            double macroLayerHeight,
            Dictionary<int, (Color mainColor, Color highlightColor)> layerColors);

        #endregion

        #region EXIT MODE

        void ExitMode();

        #endregion

        #region REFRESH

        void Refresh(List<(int layerIndex, int priority, double from, double to)> layups);

        #endregion

        #region HIGHLIGHT LAYER

        void HighlightLayer(MouseEventArgs e);

        #endregion

        #region GET HEIGHT

        (bool isOntoLayer, double height) GetHeight(MouseEventArgs e);

        #endregion

        #region DEHIGHLIGHT ALL

        void DehighlightAll();

        #endregion

        #region SET CURSOR

        void SetCursorToEyeDropper();

        void SetCursorToDefault();

        #endregion

        #region SELECT UNDER MOUSE
        Entity GetUnderMouse(MouseButtonEventArgs e);

        #endregion

        #region FOCUS

        void Focus();

        #endregion

        event EventHandler Completed;
    }

    public class LayupViewerManager : ILayupViewerManager
    {
        private readonly IAuraViewportManager _auraViewportManager;
        private AuraViewportLayout _auraViewportLayout;
        private Dictionary<double, EntityList> _entititiesByHeights;
        private Dictionary<int, Layer> _layers;
        private List<Entity> _activeEntities;
        private Entity _interactiveEntitity;
        public event EventHandler Completed;

        public LayupViewerManager(IAuraViewportManager auraViewportManager)
        {
            _auraViewportManager = auraViewportManager;
        }

        #region INITIALIZE

        public void Initialize(AuraViewportLayout auraViewportLayout)
        {
            _auraViewportLayout = auraViewportLayout;
            _auraViewportLayout.WorkCompleted += AuraViewportLayoutOnWorkCompleted;
        }

        #endregion

        #region ENTER MODE

        public void EnterMode(List<Entity> allActiveEntities,
            Entity interactiveEntity,
            double macroLayerHeight,
            Dictionary<int, (Color mainColor, Color highlightColor)> layerColors)   
        {
            _activeEntities = allActiveEntities;
            _interactiveEntitity = interactiveEntity;                         

            //ИСКУССТВЕННО СНИМАЕМ ВЫДЕЛЕНИЕ ДЛЯ РЕЖИМА ОТОБРАЖЕНИЯ СЛОЕВ
            _auraViewportManager.SetSelection(interactiveEntity, false);

            #region SET MATERIALS

            foreach (var entity in allActiveEntities)
            {
                _auraViewportManager.SetMaterialToEntity(entity,
                    _interactiveEntitity.Equals(entity) ? MaterialTypes.TRANSPARENT : MaterialTypes.ULTRA_TRANSPARENT);
            }

            #endregion

            #region CALCULATE HEIGHTS

            var heights = new List<double>();
            var lastHeight = 0.0;

            while (lastHeight + macroLayerHeight <= _interactiveEntitity.BoxMax.Z)
            {
                lastHeight += macroLayerHeight;
                if (lastHeight < _interactiveEntitity.BoxMin.Z) continue;

                heights.Add(Math.Round(lastHeight, 3));
            }

            #endregion

            #region CREATE LAYERS

            _layers = new Dictionary<int, Layer>();
            foreach (var layerColor in layerColors)
            {
                var newColor = System.Drawing.Color.FromArgb(layerColor.Value.mainColor.A, layerColor.Value.mainColor.R, layerColor.Value.mainColor.G,
                    layerColor.Value.mainColor.B);
                var newLayer = new Layer(layerColor.Key.ToString(), newColor);
                _layers.Add(layerColor.Key, newLayer);
                _auraViewportManager.AddLayer(newLayer);
            }

            #endregion

            var workUnit = new GenerateModelLayersWorkUnit(heights, _interactiveEntitity);
            _auraViewportLayout.StartWork(workUnit);
        }

        private void AuraViewportLayoutOnWorkCompleted(object sender, WorkCompletedEventArgs e)
        {
            _entititiesByHeights = new Dictionary<double, EntityList>();
            if (e.WorkUnit is GenerateModelLayersWorkUnit gm)
            {
                foreach (var gmInnerModelLayer in gm.InnerModelLayers)
                {
                    _auraViewportManager.AddEntity(gmInnerModelLayer.Value, _layers.First().Value.Name);
                    _entititiesByHeights.Add(gmInnerModelLayer.Key, gmInnerModelLayer.Value);
                }
                _auraViewportLayout.Invalidate();
                Completed?.Invoke(this, null);
            }
        }

        #endregion

        #region EXIT MODE

        public void ExitMode()
        {
            //ИСКУССТВЕННО ОБРАТНО ВОЗВРЩАЕМ ВЫДЕЛЕНИЕ ДЛЯ РЕЖИМА ОТОБРАЖЕНИЯ СЛОЕВ
            _auraViewportManager.SetSelection(_interactiveEntitity, true);

            #region SET MATERIAL

            foreach (var activeEntity in _activeEntities)
            {
                _auraViewportManager.SetStandartMaterialForEntity(activeEntity);
            }

            #endregion

            #region REMOVE ENTITIES

            foreach (var entititiesByHeight in _entititiesByHeights)
            {
                _auraViewportManager.RemoveEntities(entititiesByHeight.Value.ToList());
            }
            _entititiesByHeights.Clear();

            #endregion

            #region REMOVE LAYERS

            foreach (var layer in _layers)
            {
                _auraViewportManager.RemoveLayer(layer.Value);
            }
            _layers.Clear();

            #endregion

            _auraViewportManager.CheckIntersections();
        }

        #endregion

        #region REFRESH

        public void Refresh(List<(int layerIndex, int priority, double from, double to)> layups)
        {
            foreach (var entititiesByHeight in _entititiesByHeights)
            {
                var layupsForHeight = layups.Where(a => a.from <= entititiesByHeight.Key && a.to >= entititiesByHeight.Key);
                var minPriority = layupsForHeight.Min(a => a.priority);
                var layerIndex = layupsForHeight.Single(a => a.priority == minPriority).layerIndex;
                entititiesByHeight.Value.ForEach(a=>a.LayerName = layerIndex.ToString());
            }
            _auraViewportLayout.Invalidate();
        }

        #endregion

        #region HIGHLIGHT LAYER

        public void HighlightLayer(MouseEventArgs e)
        {
            var selectedEntityIndexes = _auraViewportLayout.GetAllEntitiesUnderMouseCursor(RenderContextUtility.ConvertPoint(_auraViewportLayout.GetMousePosition(e)));
            if (selectedEntityIndexes == null || selectedEntityIndexes.Length == 0) return;

            var (isOntoLayer, height) = GetHeight(e);

            foreach (var entititiesByHeight in _entititiesByHeights)
            {
                foreach (var entity in entititiesByHeight.Value)
                {
                    if (isOntoLayer && Math.Abs(height - entititiesByHeight.Key) < 0.001 && !entity.GetSelection()) _auraViewportManager.SetSelection(entity, true);
                    else _auraViewportManager.SetSelection(entity, false);
                }
            }
        }

        #endregion

        #region GET HEIGHT

        public (bool isOntoLayer, double height) GetHeight(MouseEventArgs e)
        {
            var selectedEntityIndexes = _auraViewportLayout.GetAllEntitiesUnderMouseCursor(RenderContextUtility.ConvertPoint(_auraViewportLayout.GetMousePosition(e)));
            if (selectedEntityIndexes == null || selectedEntityIndexes.Length == 0) return (false, 0);

            foreach (var selectedEntityIndex in selectedEntityIndexes)
            {
                var entity = _auraViewportLayout.Entities[selectedEntityIndex];
                foreach (var entititiesByHeight in _entititiesByHeights)
                {
                    foreach (var entityByHeight in entititiesByHeight.Value)
                    {
                        if (entityByHeight.Equals(entity))
                        {
                            return (true, entititiesByHeight.Key);
                        }
                    }
                }
            }
            return (false, 0);
        }

        #endregion

        #region DEHIGHLIGHT ALL

        public void DehighlightAll()
        {
            foreach (var entititiesByHeight in _entititiesByHeights)
            {
                foreach (var entity in entititiesByHeight.Value)
                {
                    _auraViewportManager.SetSelection(entity, false);
                }
            }
        }

        #endregion

        #region WORK UNIT

        //класс приблизительного слайсинга модели в фоновом потоке
        private sealed class GenerateModelLayersWorkUnit : WorkUnit
        {
            private readonly List<double> _heights;
            private readonly Entity _entity;
            public Dictionary<double, EntityList> InnerModelLayers { get; }

            public GenerateModelLayersWorkUnit(List<double> heights, Entity entity)
            {
                _heights = heights;
                _entity = entity;
                InnerModelLayers = new Dictionary<double, EntityList>();
            }

            protected override void DoWork(BackgroundWorker worker, DoWorkEventArgs doWorkEventArgs)
            {
                var i = 0;
                foreach (var height in _heights)
                {
                    var plane = new Plane(new Point3D(0, 0, height), new Vector3D(0, 0, 1));
                    var mesh = AuraViewportManager.GetMesh(_entity);
                    var sliced = mesh.Section(plane, 0).ToList();

                    var curveMeshes = new EntityList();
                    foreach (var curve in sliced)
                    {
                        var ent = curve as Entity;
                        ent.ColorMethod = colorMethodType.byLayer;
                        curveMeshes.Add(ent);
                    }

                    InnerModelLayers.Add(height, curveMeshes);

                    UpdateProgress(++i, _heights.Count, string.Empty, worker);
                }
            }
        }

        #endregion

        #region SET CURSOR

        public void SetCursorToEyeDropper()
        {
            _auraViewportLayout.Cursor = AuraCursors.Eyedropper;
        }

        public void SetCursorToDefault()
        {
            _auraViewportLayout.Cursor = null;
        }

        #endregion

        #region GET UNDER MOUSE

        public Entity GetUnderMouse(MouseButtonEventArgs e)
        {
            return _auraViewportManager.GetUnderMouse(e);
        }

        #endregion

        #region FOCUS

        public void Focus()
        {
            _auraViewportManager.Focus();
        }

        #endregion
    }

    internal static class AuraCursors
    {
        public static Cursor Eyedropper { get; }

        static AuraCursors()
        {
            var sriCurs = Application.GetResourceStream(new Uri("/Controls/Resources/Images/eyedropper_cursor.cur", UriKind.Relative));
            Eyedropper = new Cursor(sriCurs.Stream);
        }
    }
}