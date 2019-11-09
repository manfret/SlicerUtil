using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Aura.Managers;
using LayersStruct.FirstLayer;
using LayersStruct.MacroLayer.MicroLayer;
using MyToolkit.Messaging;
using PostProcessor.Blocks;
using Prism.Commands;
using Util;

namespace Aura.ViewModels
{
    public class CodeViewerVM : INotifyPropertyChanged
    {
        public enum ColorNames
        {
            GREEN,
            YELLOW,
            VIOLET,
            ORANGE,
            BLUE_GREEN,
            RED,
            BLUE,
            PINK,
            PURPLE,
            DARK_GRAY,
            LIGHT_GRAY,
            WHITE
        }

        private Dictionary<ColorNames, Color> _allColors;
        private ISettingsManager _settingsManager;
        private ICodeViewerManager _codeViewerManager;
        private IAuraViewportManager _auraViewportManager;

        private Dictionary<int, double> _heights;

        public sealed class LayerZ : INotifyPropertyChanged
        {
            private int _layerIndex;

            public int LayerIndex
            {
                get => _layerIndex;
                set
                {
                    if (_layerIndex == value) return;

                    _layerIndex = value;
                    OnPropertyChanged();
                }
            }

            private double _z;

            public double Z
            {
                get => _z;
                set
                {
                    if (Math.Abs(_z - value) < 0.001) return;
                    _z = value;

                    OnPropertyChanged();
                }
            }

            #region ON PROPERTY CHANGED

            /// <summary>
            /// Occurs when a property value changes.
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;

            private void OnPropertyChanged([CallerMemberName] string caller = null)
            {
                var handler = PropertyChanged;
                handler?.Invoke(this, new PropertyChangedEventArgs(caller));
            }

            #endregion
        }

        #region CODE BLOCKS

        private List<RunToBlock> _codeBlocks;

        public List<RunToBlock> CodeTexts
        {
            get => _codeBlocks;
            set
            {
                _codeBlocks = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region LAYERS COUNT

        private int _layersCount;

        public int LayersCount
        {
            get => _layersCount;
            set
            {
                _layersCount = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region RENDER MODE

        private RenderModes _renderMode;

        public RenderModes RenderMode
        {
            get => _renderMode;
            set
            {
                _renderMode = value;

                if (_renderMode == RenderModes.MODE2D)
                {
                    _auraViewportManager.CameraTo2D();
                    LayersMode = LayersMode.ONE_LAYER;
                }
                else _auraViewportManager.CameraTo3D();
                OnPropertyChanged();
            }
        }

        #endregion

        #region BLOCK MODE

        private BlockModes _blockMode;

        public BlockModes BlockMode
        {
            get => _blockMode;
            set
            {
                _blockMode = value;
                if (_blockMode == BlockModes.GEOMETRY)
                {
                    _codeViewerManager.SwitchToGeometry();
                    CurrentGeometryBlockVM = _codeViewerGeometryVM;
                }
                else
                {
                    _codeViewerManager.SwitchToBlocks();
                    CurrentGeometryBlockVM = _codeViewerCodeVM;
                }
                UpdateLayersModeView();
                OnPropertyChanged();
            }
        }

        #endregion

        #region LAYERS MODE

        private void UpdateLayersModeView()
        {
            switch (LayersMode)
            {
                case LayersMode.ALL_CODE:
                    _codeViewerManager.UpdateAllZMode(BlockMode);
                    break;
                case LayersMode.RANGE:
                    _codeViewerManager.UpdateRangeMode(BlockMode, LayerStart, LayerEnd);
                    break;
                case LayersMode.ONE_LAYER:
                    _codeViewerManager.UpdateOneLayer(BlockMode, LayerSelected);
                    break;
            }
        }

        private LayersMode _layersMode;

        public LayersMode LayersMode
        {
            get => _layersMode;
            set
            {
                _layersMode = value;
                UpdateLayersModeView();
                LayerSelectedDownCommand?.RaiseCanExecuteChanged();
                LayerSelectedUpCommand?.RaiseCanExecuteChanged();
                OnPropertyChanged();
            }
        }

        #endregion

        #region HAS INSET XF

        private bool _hasInsetXF;
        public bool HasInsetXF
        {
            get => _hasInsetXF;
            set
            {
                _hasInsetXF = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region HAS INFILL FIBER

        private bool _hasInfillFiber;
        public bool HasInfillFiber
        {
            get => _hasInfillFiber;
            set
            {
                _hasInfillFiber = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region LAYER START

        private void UpdateLayerStart(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "LayerIndex") return;
            var senderLayerZ = ((LayerZ)sender);
            senderLayerZ.Z = _heights[senderLayerZ.LayerIndex];
            _codeViewerManager.UpdateRangeMode(BlockMode, LayerStart, LayerEnd);
            UpdateLayersRangeCanExecute();
            OnPropertyChanged("LayerStart");
        }

        private LayerZ _layerStart;
        public LayerZ LayerStart
        {
            get => _layerStart;
            set
            {
                if (_layerStart != null && _layerStart.Equals(value)) return;
                _layerStart = value;
                if (LayerStart != null) LayerStart.PropertyChanged += UpdateLayerStart;
                UpdateLayersRangeCanExecute();
                OnPropertyChanged();
            }
        }

        #region COMMAND

        private void UpdateLayersRangeCanExecute()
        {
            LayerStartUpCommand.RaiseCanExecuteChanged();
            LayerStartDownCommand.RaiseCanExecuteChanged();
            LayerEndUpCommand.RaiseCanExecuteChanged();
            LayerEndDownCommand.RaiseCanExecuteChanged();
        }

        private bool LayerStartDownCanExecuteMethod()
        {
            return LayerStart != null && 
                   LayerStart.LayerIndex > 1;
        }

        private void LayerStartDown()
        {
            LayerStart.LayerIndex--;
            UpdateLayersRangeCanExecute();
        }

        private bool LayerStartUpCanExecuteMethod()
        {
            return LayerStart != null &&
                   LayerEnd != null &&
                   LayerStart.LayerIndex < LayersCount && 
                   LayerStart.LayerIndex + 1 < LayerEnd.LayerIndex;
        }

        private void LayerStartUp()
        {
            LayerStart.LayerIndex++;
            UpdateLayersRangeCanExecute();
        }

        #endregion

        #endregion

        #region LAYER END

        private void UpdateLayerEnd(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "LayerIndex") return;
            var senderLayerZ = ((LayerZ)sender);
            senderLayerZ.Z = _heights[senderLayerZ.LayerIndex];
            _codeViewerManager.UpdateRangeMode(BlockMode, LayerStart, LayerEnd);
            UpdateLayersRangeCanExecute();
            OnPropertyChanged("LayerEnd");
        }

        private LayerZ _layerEnd;

        public LayerZ LayerEnd
        {
            get => _layerEnd;
            set
            {
                if (_layerEnd != null && _layerEnd.Equals(value)) return;
                _layerEnd = value;
                if (LayerEnd != null) LayerEnd.PropertyChanged += UpdateLayerEnd;
                UpdateLayersRangeCanExecute();
                OnPropertyChanged();
            }
        }

        #region COMMAND

        private bool LayerEndDownCanExecuteMethod()
        {
            return LayerEnd != null &&
                   LayerStart != null &&
                   LayerEnd.LayerIndex > 1 && 
                   LayerEnd.LayerIndex - 1 > LayerStart.LayerIndex;
        }

        private void LayerEndDown()
        {
            LayerEnd.LayerIndex--;
            UpdateLayersRangeCanExecute();
        }

        private bool LayerEndUpCanExecuteMethod()
        {
            return LayerEnd != null && 
                   LayerEnd.LayerIndex < LayersCount;
        }

        private void LayerEndUp()
        {
            LayerEnd.LayerIndex++;
            UpdateLayersRangeCanExecute();
        }

        #endregion

        #endregion

        #region LAYER SELECTED

        private void UpdateOneLayer(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "LayerIndex") return;
            var senderLayerZ = ((LayerZ)sender);
            senderLayerZ.Z = _heights[senderLayerZ.LayerIndex];
            _codeViewerManager.UpdateOneLayer(BlockMode, LayerSelected);
            UpdateLayersOneCanExecute();
            OnPropertyChanged("LayerSelected");
        }

        private LayerZ _layerSelected;
        public LayerZ LayerSelected
        {
            get => _layerSelected;
            set
            {
                if (_layerSelected != null && _layerSelected.Equals(value)) return;
                _layerSelected = value;
                if (LayerSelected != null) LayerSelected.PropertyChanged += UpdateOneLayer;
                UpdateLayersOneCanExecute();
                OnPropertyChanged();
            }
        }

        #region COMMAND

        private bool LayerSelectedDownCanExecuteMethod()
        {
            return LayerSelected != null && 
                   LayerSelected.LayerIndex > 1 && LayersMode == LayersMode.ONE_LAYER;
        }

        private void LayerSelectedDown()
        {
            LayerSelected.LayerIndex--;
            UpdateLayersOneCanExecute();
        }

        private bool LayerSelectedUpCanExecuteMethod()
        {
            return LayerSelected != null && 
                   LayerSelected.LayerIndex < LayersCount && LayersMode == LayersMode.ONE_LAYER;
        }

        private void LayerSelectedUp()
        {
            LayerSelected.LayerIndex++;
            UpdateLayersOneCanExecute();
        }

        private void UpdateLayersOneCanExecute()
        {
            LayerSelectedDownCommand.RaiseCanExecuteChanged();
            LayerSelectedUpCommand.RaiseCanExecuteChanged();
        }

        #endregion

        #endregion

        //enables 

        #region INSET XF ENABLE

        private bool _insetXFEnable;
        public bool InsetXFEnable
        {
            get => _insetXFEnable;
            set
            {
                _insetXFEnable = value;
                _codeViewerGeometryVM.InsetXFEnable = value;
                _codeViewerCodeVM.InsetXFEnable = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region INFILL XF ENABLE

        private bool _infillXFEnable;
        public bool InfillXFEnable
        {
            get => _infillXFEnable;
            set
            {
                _infillXFEnable = value;
                _codeViewerGeometryVM.InfillXFEnable = value;
                _codeViewerCodeVM.InfillXFEnable = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region IN PROGRESS

        private bool _inProgress;
        public bool InProgress
        {
            get => _inProgress;
            private set
            {
                _inProgress = value;
                OnPropertyChanged();
            }
        }

        #endregion

        //commands
        public DelegateCommand ExitCodeView { get; private set; }
        public DelegateCommand SaveGCodeToFileCommand { get; set; }
        public DelegateCommand LayerSelectedDownCommand { get; private set; }
        public DelegateCommand LayerSelectedUpCommand { get; private set; }
        public DelegateCommand LayerStartDownCommand { get; private set; }
        public DelegateCommand LayerStartUpCommand { get; private set; }
        public DelegateCommand LayerEndDownCommand { get; private set; }
        public DelegateCommand LayerEndUpCommand { get; private set; }

        //VMs

        private CodeViewerGeometryVM _codeViewerGeometryVM;
        private CodeViewerCodeVM _codeViewerCodeVM;

        #region CURRENT GEOMETRY CODE VM

        private object _currentGeometryBlocksVM;
        public object CurrentGeometryBlockVM
        {
            get => _currentGeometryBlocksVM;
            set
            {
                _currentGeometryBlocksVM = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public CodeViewerVM()
        {
            ExitCodeView = new DelegateCommand(Exit);
            LayerSelectedDownCommand = new DelegateCommand(LayerSelectedDown, LayerSelectedDownCanExecuteMethod);
            LayerSelectedUpCommand = new DelegateCommand(LayerSelectedUp, LayerSelectedUpCanExecuteMethod);
            LayerStartDownCommand = new DelegateCommand(LayerStartDown, LayerStartDownCanExecuteMethod);
            LayerStartUpCommand = new DelegateCommand(LayerStartUp, LayerStartUpCanExecuteMethod);
            LayerEndDownCommand = new DelegateCommand(LayerEndDown, LayerEndDownCanExecuteMethod);
            LayerEndUpCommand = new DelegateCommand(LayerEndUp, LayerEndUpCanExecuteMethod);
        }

        #region SWITCH TO BLOCK MODE COMMAND

        private void SwitchToGeometryAction()
        {
            BlockMode = BlockModes.GEOMETRY;
        }

        private bool SwitchToGeometryCanExecute()
        {
            return BlockMode == BlockModes.GEOMETRY;
        }

        private void SwitchToBlocksAction()
        {
            BlockMode = BlockModes.BLOCKS;
        }

        private bool SwitchToBlocksCanExecute()
        {
            return BlockMode == BlockModes.BLOCKS;
        }

        #endregion

        #region SWITCH TO RENDER MODE

        private void SwitchTo3DRenderModeAction()
        {
            RenderMode = RenderModes.MODE3D;
        }

        private bool SwitchTo3DRenderModeCanExecute()
        {
            return RenderModes.MODE3D != RenderMode;
        }

        private void SwitchTo2DRenderModeAction()
        {
            RenderMode = RenderModes.MODE2D;
        }

        private bool SwitchTo2DRenderModeCanExecute()
        {
            return RenderModes.MODE2D != RenderMode;
        }

        #endregion

        #region EXIT

        public event EventHandler ExitCodeViewer;

        private void Exit()
        {
            _codeViewerManager.DisposeAll();
            LayerSelected.PropertyChanged -= UpdateOneLayer;
            LayerStart.PropertyChanged -= UpdateLayerStart;
            LayerEnd.PropertyChanged -= UpdateLayerEnd;
            _codeViewerManager.ExitViewCode();
            ExitCodeViewer?.Invoke(this, null);
            InProgress = false;
        }

        #endregion

        #region ENTER

        private void Enter()
        {
            _codeViewerManager.EnterViewCode();
            LayerSelectedDownCommand?.RaiseCanExecuteChanged();
            LayerSelectedUpCommand?.RaiseCanExecuteChanged();
            InProgress = true;
        }

        #endregion

        #region GET INFILL XF ENABLED

        private bool GetInfillXFEnabled(IFirstLayer fl, List<IMicroLayer> microLayers)
        {
            var enabled = false;

            if (fl != null)
            {
                foreach (var microLayer in microLayers)
                {
                    if (enabled) break;
                    foreach (var microLayerModel in microLayer.Models)
                    {
                        if (enabled) break;
                        foreach (var printIsland in microLayerModel.Islands)
                        {
                            if (printIsland.FiberInfill != null && printIsland.FiberInfill.Any())
                            {
                                enabled = true;
                                break;
                            }
                        }
                    }
                }
            }
            foreach (var microLayerModel in fl.Models)
            {
                if (enabled) break;
                foreach (var printIsland in microLayerModel.Islands)
                {
                    if (printIsland.FiberInfill != null && printIsland.FiberInfill.Any())
                    {
                        enabled = true;
                        break;
                    }
                }
            }

            return enabled;
        }

        #endregion

        #region GET INSET XF ENABLED

        private bool GetInsetXFEnabled(IFirstLayer fl, List<IMicroLayer> microLayers)
        {
            var enabled = false;

            if (fl != null)
            {
                foreach (var microLayerModel in fl.Models)
                {
                    if (enabled) break;
                    foreach (var printIsland in microLayerModel.Islands)
                    {
                        if (printIsland.InsetsXFiber != null && printIsland.InsetsXFiber.Any())
                        {
                            enabled = true;
                            break;
                        }
                    }
                }
            }

            foreach (var microLayer in microLayers)
            {
                if (enabled) break;
                foreach (var microLayerModel in microLayer.Models)
                {
                    if (enabled) break;
                    foreach (var printIsland in microLayerModel.Islands)
                    {
                        if (printIsland.InsetsXFiber != null && printIsland.InsetsXFiber.Any())
                        {
                            enabled = true;
                            break;
                        }
                    }
                }
            }
            return enabled;
        }

        #endregion

        #region ON PROPERTY CHANGED

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string caller = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(caller));
        }

        #endregion

        #region INITIALIZE

        public void Initialize(ISettingsManager settingsManager,
            ICodeViewerManager codeViewerManager,
            IAuraViewportManager auraViewportManager)
        {
            _settingsManager = settingsManager;
            _codeViewerManager = codeViewerManager;
            _codeViewerManager.ProcessFinished += OnProcessFinished;
            _auraViewportManager = auraViewportManager;
            _codeViewerGeometryVM = new CodeViewerGeometryVM();
            _codeViewerCodeVM = new CodeViewerCodeVM();

            #region CREATE COLORS

            var green = (Color)ColorConverter.ConvertFromString("#FF7DCEA0");
            var yellow = (Color)ColorConverter.ConvertFromString("#FFF7DC6F");
            var violet = (Color)ColorConverter.ConvertFromString("#FF695ddb");
            var orange = (Color)ColorConverter.ConvertFromString("#FFE59866");
            var blueGreen = (Color)ColorConverter.ConvertFromString("#FF76D7C4");
            var red = (Color)ColorConverter.ConvertFromString("#FFEC7063");
            var blue = (Color)ColorConverter.ConvertFromString("#FF4286F4");
            var pink = (Color)ColorConverter.ConvertFromString("#FFF06292");
            var purple = (Color)ColorConverter.ConvertFromString("#FFBB8FCE");
            var darkGray = (Color)ColorConverter.ConvertFromString("#FF474747");
            var lightGray = (Color)ColorConverter.ConvertFromString("#FFA3A3A3");
            var white = (Color)ColorConverter.ConvertFromString("#FFFFFFFF");
            _allColors = new Dictionary<ColorNames, Color>
            {
                {ColorNames.WHITE, white},
                {ColorNames.LIGHT_GRAY, lightGray},
                {ColorNames.DARK_GRAY, darkGray},
                {ColorNames.BLUE, blue},
                {ColorNames.BLUE_GREEN, blueGreen},
                {ColorNames.GREEN, green},
                {ColorNames.YELLOW, yellow},
                {ColorNames.ORANGE, orange},
                {ColorNames.RED, red},
                {ColorNames.PINK, pink},
                {ColorNames.PURPLE, purple},
                {ColorNames.VIOLET, violet},
            };

            #endregion

            _codeViewerGeometryVM.Initialize(_settingsManager, _codeViewerManager, _allColors);
            _codeViewerCodeVM.Initialize(_settingsManager, _codeViewerManager, _allColors);
        }

        public void Initialize(List<IPPBlock> blocks,
            IFirstLayer fl,
            List<IMicroLayer> microLayers)
        {
            Enter();
            FillFields(blocks, fl, microLayers);
        }

        #endregion

        #region SET TO DEFAULT MODES

        private void SetToDefaultModes()
        {
            BlockMode = BlockModes.GEOMETRY;
            RenderMode = RenderModes.MODE3D;
            LayersMode = LayersMode.ALL_CODE;
        }

        #endregion

        #region PROCESS FINISHED

        private void OnProcessFinished(object sender, EventArgs args)
        {
            SetToDefaultModes();
        }

        #endregion

        #region FILL FIELDS

        private string _gCode;

        private void FillFields(List<IPPBlock> blocks,
            IFirstLayer fl,
            List<IMicroLayer> microLayers)
        {
            if ((fl == null && !microLayers.TrueAny()) || blocks == null) return;

            LayersCount = fl == null ? 0 : 1 + microLayers.Count;

            #region FILL CODE TEXTS

            var codeTexts = new List<RunToBlock>();
            var i = 0;
            //добавляем индекс блока
            foreach (var ppBlock in blocks)
            {
                var code = ppBlock.GetCode();
                var groups = code.Split(Environment.NewLine.ToCharArray());
                foreach (var @group in groups)
                {
                    if (!@group.Any()) continue;
                    var codeInGroup = @group.Replace("\\r", "").Replace(Environment.NewLine, "");
                    if (!codeInGroup.Any()) continue;
                    codeTexts.Add(new RunToBlock { BlockCode = new BlockCode { Block = ppBlock, PartCode = codeInGroup }, Index = ++i });
                }
            }

            CodeTexts = codeTexts;

            #endregion

            #region GENERATE G-CODE

            //генерируем код по блокам
            var stringBuilder = new StringBuilder();
            foreach (var block in blocks)
            {
                stringBuilder.Append(block.GetCode());
            }

            _gCode = stringBuilder.ToString();

            #endregion

            #region ENABLES

            InsetXFEnable = GetInsetXFEnabled(fl, microLayers);
            InfillXFEnable = GetInfillXFEnabled(fl, microLayers);

            #endregion

            double GetMM(int um) => Math.Round((double) um / 1000, 3);

            LayerStart = new LayerZ
            {
                LayerIndex = 1,
                Z = fl != null ? GetMM(fl.AbsoluteZUM) : GetMM(microLayers[0].AbsoluteZUM)
            };
            LayerEnd = new LayerZ
            {
                LayerIndex = microLayers.TrueAny() ? LayersCount : 1,
                Z = microLayers.TrueAny() ? GetMM(microLayers.Last().AbsoluteZUM) : GetMM(fl.AbsoluteZUM)
            };
            LayerSelected = new LayerZ()
            {
                LayerIndex = 1,
                Z = fl != null ? GetMM(fl.AbsoluteZUM) : GetMM(microLayers[0].AbsoluteZUM)
            };

            _heights = new Dictionary<int, double>();
            var zFirst = fl != null ? GetMM(fl.AbsoluteZUM) : GetMM(microLayers[0].AbsoluteZUM);
            _heights.Add(1, zFirst);
            for (var j = 1; j < LayersCount; j++)
            {
                var z =  GetMM(microLayers[j-1].AbsoluteZUM);
                _heights.Add(j + 1, z);
            }

            _codeViewerManager.FillCodeEntities(fl, microLayers, blocks);
        }

        #endregion
    }

    public struct BlockCode
    {
        public IPPBlock Block { get; set; }
        public string PartCode { get; set; }
    }

    public struct RunToBlock
    {
        public BlockCode BlockCode { get; set; }
        public int Index { get; set; }
    }

    public enum BlockModes
    {
        GEOMETRY,
        BLOCKS
    }

    public enum RenderModes
    {
        MODE3D,
        MODE2D
    }

    public enum LayersMode
    {
        ALL_CODE,
        RANGE,
        ONE_LAYER
    }

    public class BlockModesToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var pt = (BlockModes)parameter;
            var vt = (BlockModes)value;

            return pt == vt ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class RenderModesToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var pt = (RenderModes)parameter;
            var vt = (RenderModes)value;

            return pt == vt ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class LayersModeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var pt = (LayersMode)parameter;
            var vt = (LayersMode)value;

            return pt == vt ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class EqualityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null && parameter == null) return true;
            if (value == null || parameter == null) return false;
            return value.Equals(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return parameter;
        }
    }
}