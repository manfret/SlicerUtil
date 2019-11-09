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
    public class CodeViewerGeometryVM : INotifyPropertyChanged
    {
        private ISettingsManager _settingsManager;
        private ICodeViewerManager _codeViewerManager;

        //entity colors

        #region INSET 0 COLOR

        private Color _inset0Color;
        public Color Inset0Color
        {
            get => _inset0Color;
            set
            {
                if (_inset0Color == value) return;
                _inset0Color = value;
                if (_settingsManager.Inset0Color != value) _settingsManager.Inset0Color = value;
                _codeViewerManager.UpdateLayerColor(EntityType.INSET0, value);
                OnPropertyChanged();
            }
        }

        #endregion

        #region MICRO INFILL COLOR

        private Color _microInfillColor;
        public Color MicroInfillColor
        {
            get => _microInfillColor;
            set
            {
                if (_microInfillColor == value) return;
                _microInfillColor = value;
                if (_settingsManager.MicroInfillColor != value) _settingsManager.MicroInfillColor = value;
                _codeViewerManager.UpdateLayerColor(EntityType.MICRO_INFILL_P, value);
                OnPropertyChanged();
            }
        }

        #endregion

        #region INSET XP COLOR

        private Color _insetXPColor;
        public Color InsetXPColor
        {
            get => _insetXPColor;
            set
            {
                if (_insetXPColor == value) return;
                _insetXPColor = value;
                if (_settingsManager.InsetXPColor != value) _settingsManager.InsetXPColor = value;
                _codeViewerManager.UpdateLayerColor(EntityType.INSET_XP, value);
                OnPropertyChanged();
            }
        }

        #endregion

        #region INFILL SOLID COLOR

        private Color _infillSolidColor;
        public Color InfillSolidColor
        {
            get => _infillSolidColor;
            set
            {
                if (_infillSolidColor == value) return;
                _infillSolidColor = value;
                if (_settingsManager.InfillSolidColor != value) _settingsManager.InfillSolidColor = value;
                _codeViewerManager.UpdateLayerColor(EntityType.INFILL_SOLID_P, value);
                OnPropertyChanged();
            }
        }

        #endregion

        #region INFILL CELLULAR COLOR

        private Color _infillCellularColor;
        public Color InfillCellularColor
        {
            get => _infillCellularColor;
            set
            {
                if (_infillCellularColor == value) return;
                _infillCellularColor = value;
                if (_settingsManager.InfillCellularColor != value) _settingsManager.InfillCellularColor = value;
                _codeViewerManager.UpdateLayerColor(EntityType.INFILL_CELLULAR_P, value);
                OnPropertyChanged();
            }
        }

        #endregion

        #region SUPPORT THIN COLOR

        private Color _supportThinColor;
        public Color SupportThinColor
        {
            get => _supportThinColor;
            set
            {
                if (_supportThinColor == value) return;
                _supportThinColor = value;
                if (_settingsManager.SupportThinColor != value) _settingsManager.SupportThinColor = value;
                _codeViewerManager.UpdateLayerColor(EntityType.SUPPORT_THIN, value);
                OnPropertyChanged();
            }
        }

        #endregion

        #region SUPPORT THICK COLOR

        private Color _supportThickColor;
        public Color SupportThickColor
        {
            get => _supportThickColor;
            set
            {
                if (_supportThickColor == value) return;
                _supportThickColor = value;
                if (_settingsManager.SupportThickColor != value) _settingsManager.SupportThickColor = value;
                _codeViewerManager.UpdateLayerColor(EntityType.SUPPORT_THICK, value);
                OnPropertyChanged();
            }
        }

        #endregion

        #region SKIRT COLOR

        private Color _skirtColor;
        public Color SkirtColor
        {
            get => _skirtColor;
            set
            {
                if (_skirtColor == value) return;
                _skirtColor = value;
                if (_settingsManager.SkirtColor != value) _settingsManager.SkirtColor = value;
                _codeViewerManager.UpdateLayerColor(EntityType.SKIRT, value);
                OnPropertyChanged();
            }
        }

        #endregion

        #region BRIM COLOR

        private Color _brimColor;
        public Color BrimColor
        {
            get => _brimColor;
            set
            {
                if (_brimColor == value) return;
                _brimColor = value;
                if (_settingsManager.BrimColor != value) _settingsManager.BrimColor = value;
                _codeViewerManager.UpdateLayerColor(EntityType.BRIM, value);
                OnPropertyChanged();
            }
        }

        #endregion

        #region WIPE TOWER COLOR

        private Color _wipeTowerColor;
        public Color WipeTowerColor
        {
            get => _wipeTowerColor;
            set
            {
                if (_wipeTowerColor == value) return;
                _wipeTowerColor = value;
                if (_settingsManager.WipeColor != value) _settingsManager.WipeColor = value;
                _codeViewerManager.UpdateLayerColor(EntityType.WIPE_TOWER, value);
                OnPropertyChanged();
            }
        }

        #endregion

        #region INSET XF COLOR

        private Color _insetXFColor;
        public Color InsetXFColor
        {
            get => _insetXFColor;
            set
            {
                if (_insetXFColor == value) return;
                _insetXFColor = value;
                if (_settingsManager.InsetXFColor != value) _settingsManager.InsetXFColor = value;
                _codeViewerManager.UpdateLayerColor(EntityType.INSET_XF, value);
                OnPropertyChanged();
            }
        }

        #endregion

        #region FIBER INFILL COLOR

        private Color _fiberInfillColor;
        public Color FiberInfillColor
        {
            get => _fiberInfillColor;
            set
            {
                if (_fiberInfillColor == value) return;
                _fiberInfillColor = value;
                if (_settingsManager.FiberInfillColor != value) _settingsManager.FiberInfillColor = value;
                _codeViewerManager.UpdateLayerColor(EntityType.INFILL_FIBER, value);
                OnPropertyChanged();
            }
        }

        #endregion

        //geometry visiblity

        #region INSET 0 SHOW

        private bool _inset0Show;
        public bool Inset0Show
        {
            get => _inset0Show;
            set
            {
                if (_inset0Show == value) return;
                _inset0Show = value;
                if (_settingsManager.Inset0Show != value) _settingsManager.Inset0Show = value;
                _codeViewerManager.UpdateLayerVisibility(EntityType.INSET0, value);
                OnPropertyChanged();
            }
        }

        #endregion

        #region MICRO INFILL SHOW

        private bool _microInfillShow;

        public bool MicroInfillShow
        {
            get => _microInfillShow;
            set
            {
                if (_microInfillShow == value) return;
                _microInfillShow = value;
                if (_settingsManager.MicroInfillShow != value) _settingsManager.MicroInfillShow = value;
                _codeViewerManager.UpdateLayerVisibility(EntityType.MICRO_INFILL_P, value);
                OnPropertyChanged();
            }
        }

        #endregion

        #region INSET XP SHOW

        private bool _insetXPShow;
        public bool InsetXPShow
        {
            get => _insetXPShow;
            set
            {
                if (_insetXPShow == value) return;
                _insetXPShow = value;
                if (_settingsManager.InsetXPShow != value) _settingsManager.InsetXPShow = value;
                _codeViewerManager.UpdateLayerVisibility(EntityType.INSET_XP, value);
                OnPropertyChanged();
            }
        }

        #endregion

        #region INFILL SOLID SHOW

        private bool _infillSolidShow;
        public bool InfillSolidShow
        {
            get => _infillSolidShow;
            set
            {
                if (_infillSolidShow == value) return;
                _infillSolidShow = value;
                if (_settingsManager.InfillSolidShow != value) _settingsManager.InfillSolidShow = value;
                _codeViewerManager.UpdateLayerVisibility(EntityType.INFILL_SOLID_P, value);
                OnPropertyChanged();
            }
        }

        #endregion

        #region INFILL CELLULAR SHOW

        private bool _infillCellularShow;

        public bool InfillCellularShow
        {
            get => _infillCellularShow;
            set
            {
                if (InfillCellularShow == value) return;
                _infillCellularShow = value;
                if (_settingsManager.InfillCellularShow != value) _settingsManager.InfillCellularShow = value;
                _codeViewerManager.UpdateLayerVisibility(EntityType.INFILL_CELLULAR_P, value);
                OnPropertyChanged();
            }
        }

        #endregion

        #region SUPPORT THIN SHOW

        private bool _supportThinShow;
        public bool SupportThinShow
        {
            get => _supportThinShow;
            set
            {
                if (_supportThinShow == value) return;
                _supportThinShow = value;
                if (_settingsManager.SupportThinShow != value) _settingsManager.SupportThinShow = value;
                _codeViewerManager.UpdateLayerVisibility(EntityType.SUPPORT_THIN, value);
                OnPropertyChanged();
            }
        }

        #endregion

        #region SUPPORT THICK SHOW

        private bool _supportThickShow;
        public bool SupportThickShow
        {
            get => _supportThickShow;
            set
            {
                if (_supportThickShow == value) return;
                _supportThickShow = value;
                if (_settingsManager.SupportThickShow != value) _settingsManager.SupportThickShow = value;
                _codeViewerManager.UpdateLayerVisibility(EntityType.SUPPORT_THICK, value);
                OnPropertyChanged();
            }
        }

        #endregion

        #region SKIRT SHOW

        private bool _skirtShow;

        public bool SkirtShow
        {
            get => _skirtShow;
            set
            {
                if (_skirtShow == value) return;
                _skirtShow = value;
                if (_settingsManager.SkirtShow != value) _settingsManager.SkirtShow = value;
                _codeViewerManager.UpdateLayerVisibility(EntityType.SKIRT, value);
                OnPropertyChanged();
            }
        }

        #endregion

        #region BRIM SHOW

        private bool _brimShow;
        public bool BrimShow
        {
            get => _brimShow;
            set
            {
                if (_brimShow == value) return;
                _brimShow = value;
                if (_settingsManager.BrimShow != value) _settingsManager.BrimShow = value;
                _codeViewerManager.UpdateLayerVisibility(EntityType.BRIM, value);
                OnPropertyChanged();
            }
        }

        #endregion

        #region WIPE TOWER SHOW

        private bool _wipeTowerShow;
        public bool WipeTowerShow
        {
            get => _wipeTowerShow;
            set
            {
                if (_wipeTowerShow == value) return;
                _wipeTowerShow = value;
                if (_settingsManager.WipeShow != value) _settingsManager.WipeShow = value;
                _codeViewerManager.UpdateLayerVisibility(EntityType.WIPE_TOWER, value);
                OnPropertyChanged();
            }
        }

        #endregion

        #region INSET XF SHOW

        private bool _insetXFShow;
        public bool InsetXFShow
        {
            get => _insetXFShow;
            set
            {
                if (_insetXFShow == value) return;
                _insetXFShow = value;
                if (_settingsManager.InsetXFShow != value) _settingsManager.InsetXFShow = value;
                _codeViewerManager.UpdateLayerVisibility(EntityType.INSET_XF, value);
                OnPropertyChanged();
            }
        }

        #endregion

        #region INFILL FIBER SHOW

        private bool _infillFiberShow;
        public bool InfillFiberShow
        {
            get => _infillFiberShow;
            set
            {
                if (_infillFiberShow == value) return;
                _infillFiberShow = value;
                if (_settingsManager.FiberInfillShow != value) _settingsManager.FiberInfillShow = value;
                _codeViewerManager.UpdateLayerVisibility(EntityType.INFILL_FIBER, value);
                OnPropertyChanged();
            }
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
                if (_insetXFEnable == value) return;
                _insetXFEnable = value;
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

            var entityColors = new Dictionary<EntityType, Color>
            {
                {EntityType.INSET0, Inset0Color},
                {EntityType.MICRO_INFILL_P, MicroInfillColor},
                {EntityType.INSET_XP, InsetXPColor},
                {EntityType.INFILL_SOLID_P, InfillSolidColor},
                {EntityType.INFILL_CELLULAR_P, InfillCellularColor},
                {EntityType.SUPPORT_THIN, SupportThinColor},
                {EntityType.SUPPORT_THICK, SupportThickColor},
                {EntityType.SKIRT, SkirtColor},
                {EntityType.BRIM, BrimColor},
                {EntityType.WIPE_TOWER, WipeTowerColor},
                {EntityType.INSET_XF, InsetXFColor},
                {EntityType.INFILL_FIBER, FiberInfillColor}
            };
            _codeViewerManager.DisposeGeometryLayers();
            _codeViewerManager.CreateLayers(entityColors);

            #endregion

            #region FILL ENTITY SHOWS

            Inset0Show = _settingsManager.Inset0Show;
            MicroInfillShow = _settingsManager.MicroInfillShow;
            InsetXPShow = _settingsManager.InsetXPShow;
            InfillSolidShow = _settingsManager.InfillSolidShow;
            InfillCellularShow = _settingsManager.InfillCellularShow;
            SupportThinShow = _settingsManager.SupportThinShow;
            SupportThickShow = _settingsManager.SupportThickShow;
            SkirtShow = _settingsManager.SkirtShow;
            BrimShow = _settingsManager.BrimShow;
            WipeTowerShow = _settingsManager.WipeShow;
            InsetXFShow = _settingsManager.InsetXFShow;
            InfillFiberShow = _settingsManager.FiberInfillShow;

            #endregion

            #region FILL ENTITY COLORS

            Inset0Color = _settingsManager.Inset0Color.Equals(new Color()) ? _allColors[CodeViewerVM.ColorNames.DARK_GRAY] : _settingsManager.Inset0Color;
            MicroInfillColor = _settingsManager.MicroInfillColor.Equals(new Color()) ? _allColors[CodeViewerVM.ColorNames.LIGHT_GRAY] : _settingsManager.MicroInfillColor;
            InsetXPColor = _settingsManager.InsetXPColor.Equals(new Color()) ? _allColors[CodeViewerVM.ColorNames.LIGHT_GRAY] : _settingsManager.InsetXPColor;
            InfillSolidColor = _settingsManager.InfillSolidColor.Equals(new Color()) ? _allColors[CodeViewerVM.ColorNames.LIGHT_GRAY] : _settingsManager.InfillSolidColor;
            InfillCellularColor = _settingsManager.InfillCellularColor.Equals(new Color()) ? _allColors[CodeViewerVM.ColorNames.LIGHT_GRAY] : _settingsManager.InfillCellularColor;
            SupportThinColor = _settingsManager.SupportThinColor.Equals(new Color()) ? _allColors[CodeViewerVM.ColorNames.LIGHT_GRAY] : _settingsManager.SupportThinColor;
            SupportThickColor = _settingsManager.SupportThickColor.Equals(new Color()) ? _allColors[CodeViewerVM.ColorNames.LIGHT_GRAY] : _settingsManager.SupportThickColor;
            SkirtColor = _settingsManager.SkirtColor.Equals(new Color()) ? _allColors[CodeViewerVM.ColorNames.LIGHT_GRAY] : _settingsManager.SkirtColor;
            BrimColor = _settingsManager.BrimColor.Equals(new Color()) ? _allColors[CodeViewerVM.ColorNames.LIGHT_GRAY] : _settingsManager.BrimColor;
            WipeTowerColor = _settingsManager.WipeColor.Equals(new Color()) ? _allColors[CodeViewerVM.ColorNames.LIGHT_GRAY] : _settingsManager.WipeColor;
            InsetXFColor = _settingsManager.InsetXFColor.Equals(new Color()) ? _allColors[CodeViewerVM.ColorNames.LIGHT_GRAY] : _settingsManager.InsetXFColor;
            FiberInfillColor = _settingsManager.FiberInfillColor.Equals(new Color()) ? _allColors[CodeViewerVM.ColorNames.LIGHT_GRAY] : _settingsManager.FiberInfillColor; ;

            #endregion
        }

        #region ON PROPERTY CHANGED

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string caller = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(caller));
        }

        #endregion
    }
}