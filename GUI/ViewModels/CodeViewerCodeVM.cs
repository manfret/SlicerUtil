using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using Aura.Managers;
using PostProcessor.Properties;
using Xceed.Wpf.Toolkit;

namespace Aura.ViewModels
{
    public class CodeViewerCodeVM : INotifyPropertyChanged
    {
        private ISettingsManager _settingsManager;
        private ICodeViewerManager _codeViewerManager;

        //code colors

        #region TRAVEL COLOR

        private Color _travelColor;
        public Color TravelColor
        {
            get => _travelColor;
            set
            {
                if (_travelColor == value) return;
                _travelColor = value;
                if (_settingsManager.TravelColor != value) _settingsManager.TravelColor = value;
                _codeViewerManager.UpdateLayerColor(BlockType.TRAVEL, value);
                OnPropertyChanged();
            }
        }

        #endregion

        #region MOVE COLOR

        private Color _moveColor;
        public Color MovePColor
        {
            get => _moveColor;
            set
            {
                if (_moveColor == value) return;
                _moveColor = value;
                if (_settingsManager.MovePColor != value) _settingsManager.MovePColor = value;
                _codeViewerManager.UpdateLayerColor(BlockType.MOVE_P, value);
                OnPropertyChanged();
            }
        }

        #endregion

        #region MOVE PF COLOR

        private Color _movePFColor;
        public Color MovePFColor
        {
            get => _movePFColor;
            set
            {
                if (_movePFColor == value) return;
                _movePFColor = value;
                if (_settingsManager.MovePFColor != value) _settingsManager.MovePFColor = value;
                _codeViewerManager.UpdateLayerColor(BlockType.MOVE_PF, value);
                OnPropertyChanged();
            }
        }

        #endregion

        #region EXTRUDE P COLOR

        private Color _extrudePColor;
        public Color ExtrudePColor
        {
            get => _extrudePColor;
            set
            {
                if (_extrudePColor == value) return;
                _extrudePColor = value;
                if (_settingsManager.ExtrudePColor != value) _settingsManager.ExtrudePColor = value;
                _codeViewerManager.UpdateLayerColor(BlockType.EXTRUDE_P, value);
                OnPropertyChanged();
            }
        }

        #endregion

        #region EXTRUDE PF COLOR

        private Color _extrudePFColor;
        public Color ExtrudePFColor
        {
            get => _extrudePFColor;
            set
            {
                if (_extrudePFColor == value) return;
                _extrudePFColor = value;
                if (_settingsManager.ExtrudePFColor != value) _settingsManager.ExtrudePFColor = value;
                _codeViewerManager.UpdateLayerColor(BlockType.EXTRUDE_PF, value);
                OnPropertyChanged();
            }
        }

        #endregion

        #region RETRACT COLOR

        private Color _retractColor;
        public Color RetractColor
        {
            get => _retractColor;
            set
            {
                if (_retractColor == value) return;
                _retractColor = value;
                if (_settingsManager.RetractColor != value) _settingsManager.RetractColor = value;
                _codeViewerManager.UpdateLayerColor(BlockType.RETRACT, value);
                OnPropertyChanged();
            }
        }

        #endregion

        #region CUT COLOR

        private Color _cutColor;
        public Color CutColor
        {
            get => _cutColor;
            set
            {
                if (_cutColor == value) return;
                _cutColor = value;
                if (_settingsManager.CutColor != value) _settingsManager.CutColor = value;
                _codeViewerManager.UpdateLayerColor(BlockType.CUT, value);
                OnPropertyChanged();
            }
        }

        #endregion

        //blocks visibility

        #region TRAVEL SHOW

        private bool _travelShow;
        public bool TravelShow
        {
            get => _travelShow;
            set
            {
                if (_travelShow == value) return;
                _travelShow = value;
                if (_settingsManager.TravelShow != value) _settingsManager.TravelShow = value;
                _codeViewerManager.UpdateLayerVisibility(BlockType.TRAVEL, value);
                OnPropertyChanged();
            }
        }

        #endregion

        #region MOVE P SHOW

        private bool _movePShow;
        public bool MovePShow
        {
            get => _movePShow;
            set
            {
                if (_movePShow == value) return;
                _movePShow = value;
                if (_settingsManager.MovePShow != value) _settingsManager.MovePShow = value;
                _codeViewerManager.UpdateLayerVisibility(BlockType.MOVE_P, value);
                OnPropertyChanged();
            }
        }

        #endregion

        #region MOVE PF SHOW

        private bool _movePFShow;
        public bool MovePFShow
        {
            get => _movePFShow;
            set
            {
                if (_movePFShow == value) return;
                _movePFShow = value;
                if (_settingsManager.MovePFShow != value) _settingsManager.MovePFShow = value;
                _codeViewerManager.UpdateLayerVisibility(BlockType.MOVE_PF, value);
                OnPropertyChanged();
            }
        }

        #endregion

        #region EXTRUDE P SHOW

        private bool _extrudePShow;
        public bool ExtrudePShow
        {
            get => _extrudePShow;
            set
            {
                if (_extrudePShow == value) return;
                _extrudePShow = value;
                if (_settingsManager.ExtrudePShow != value) _settingsManager.ExtrudePShow = value;
                _codeViewerManager.UpdateLayerVisibility(BlockType.EXTRUDE_P, value);
                OnPropertyChanged();
            }
        }

        #endregion

        #region EXTRUDE PF SHOW

        private bool _extrudePFShow;
        public bool ExtrudePFShow
        {
            get => _extrudePFShow;
            set
            {
                if (_extrudePFShow == value) return;
                _extrudePFShow = value;
                if (_settingsManager.ExtrudePFShow != value) _settingsManager.ExtrudePFShow = value;
                _codeViewerManager.UpdateLayerVisibility(BlockType.EXTRUDE_P, value);
                OnPropertyChanged();
            }
        }

        #endregion

        #region RETRACT SHOW

        private bool _retractShow;
        public bool RetractShow
        {
            get => _retractShow;
            set
            {
                if (_retractShow == value) return;
                _retractShow = value;
                if (_settingsManager.RetractShow != value) _settingsManager.RetractShow = value;
                _codeViewerManager.UpdateLayerVisibility(BlockType.RETRACT, value);
                OnPropertyChanged();
            }
        }

        #endregion

        #region CUT SHOW

        private bool _cutShow;
        public bool CutShow
        {
            get => _cutShow;
            set
            {
                if (_cutShow == value) return;
                _cutShow = value;
                if (_settingsManager.CutShow != value) _settingsManager.CutShow = value;
                _codeViewerManager.UpdateLayerVisibility(BlockType.CUT, value);
                OnPropertyChanged();
            }
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

        //enbles

        #region INSET XF ENABLE

        private bool _insetXFEnable;
        public bool InsetXFEnable
        {
            get => _insetXFEnable;
            set
            {
                _insetXFEnable = value;
                FiberEnabled = InfillXFEnable || value;
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
                FiberEnabled = InsetXFEnable || value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region FIBER ENABLED

        private bool _fiberEnabled;
        public bool FiberEnabled
        {
            get => _fiberEnabled;
            set
            {
                _fiberEnabled = value;
                OnPropertyChanged();
            }
        }

        #endregion

        //misc

        #region COLORS

        private ObservableCollection<ColorItem> _colors;
        public ObservableCollection<ColorItem> Colors
        {
            get => _colors;
            private set
            {
                _colors = value;
                OnPropertyChanged();
            }
        }

        #endregion

        private Dictionary<CodeViewerVM.ColorNames, Color> _allColors;

        public void Initialize(ISettingsManager settingsManager,
            ICodeViewerManager codeViewerManager,
            Dictionary<CodeViewerVM.ColorNames, Color> allColors)
        {
            _settingsManager = settingsManager;
            _codeViewerManager = codeViewerManager;
            _allColors = allColors;

            #region CREATE COLORS

            Colors = new ObservableCollection<ColorItem>();
            foreach (var allColor in _allColors)
            {
                Colors.Add(new ColorItem(allColor.Value, allColor.Key.ToString()));
            }

            #endregion

            #region CREATE LAYERS

            var blockColors = new Dictionary<BlockType, Color>
            {
                {BlockType.TRAVEL, TravelColor},
                {BlockType.MOVE_P, MovePColor},
                {BlockType.MOVE_PF, MovePFColor},
                {BlockType.EXTRUDE_P, ExtrudePColor},
                {BlockType.EXTRUDE_PF, ExtrudePFColor},
                {BlockType.RETRACT, RetractColor},
                {BlockType.CUT, CutColor}
            };
            _codeViewerManager.DisposeBlockLayers();
            _codeViewerManager.CreateLayers(blockColors);

            #endregion

            #region FILL BLOCK COLORS

            TravelColor = _settingsManager.TravelColor.Equals(new Color()) ? _allColors[CodeViewerVM.ColorNames.LIGHT_GRAY] : _settingsManager.TravelColor;
            MovePColor = _settingsManager.MovePColor.Equals(new Color()) ? _allColors[CodeViewerVM.ColorNames.BLUE] : _settingsManager.MovePColor;
            MovePFColor = _settingsManager.MovePFColor.Equals(new Color()) ? _allColors[CodeViewerVM.ColorNames.YELLOW] : _settingsManager.MovePFColor;
            ExtrudePColor = _settingsManager.ExtrudePColor.Equals(new Color()) ? _allColors[CodeViewerVM.ColorNames.BLUE] : _settingsManager.ExtrudePColor;
            ExtrudePFColor = _settingsManager.ExtrudePFColor.Equals(new Color()) ? _allColors[CodeViewerVM.ColorNames.YELLOW] : _settingsManager.ExtrudePFColor;
            RetractColor = _settingsManager.RetractColor.Equals(new Color()) ? _allColors[CodeViewerVM.ColorNames.BLUE] : _settingsManager.RetractColor;
            CutColor = _settingsManager.CutColor.Equals(new Color()) ? _allColors[CodeViewerVM.ColorNames.YELLOW] : _settingsManager.CutColor;

            #endregion

            #region FILL BLOCK SHOWS

            TravelShow = _settingsManager.TravelShow;
            MovePShow = _settingsManager.MovePShow;
            MovePFShow = _settingsManager.MovePFShow;
            ExtrudePShow = _settingsManager.ExtrudePShow;
            ExtrudePFShow = _settingsManager.ExtrudePFShow;
            RetractShow = _settingsManager.RetractShow;
            CutShow = _settingsManager.CutShow;

            #endregion
        }
    }
}