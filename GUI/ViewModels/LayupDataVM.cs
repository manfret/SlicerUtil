using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using Aura.Controls;
using Aura.CORE.AuraFile;
using Aura.Managers;
using Aura.Themes.Localization;
using MyToolkit.Messaging;
using Prism.Commands;
using Settings.Memento;

namespace Aura.ViewModels
{
    public class LayupDataVM : INotifyPropertyChanged
    {
        private LayupDataCollectionVM _collectionVM;
        private ILayupViewerManager _layupViewerManager;

        #region Z MIN

        private double _zMin;
        public double ZMin
        {
            get => _zMin;
            private set
            {
                _zMin = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Z MAX

        private double _zMax;
        public double ZMax
        {
            get => _zMax;
            private set
            {
                _zMax = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region COLOR

        private Color _color;
        public Color Color
        {
            get => _color;
            private set
            {
                _color = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region IS DEFAULT

        private bool _isDefault;
        public bool IsDefault
        {
            get => _isDefault;
            private set
            {
                _isDefault = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region IS ALLOW GENERATE FIBER

        private bool _isAllowGenerateFiber;
        public bool IsAllowGenerateFiber
        {
            get => _isAllowGenerateFiber;
            private set
            {
                _isAllowGenerateFiber = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region DATA

        private LayupDataLayerIndex _layupDataLayerIndex;
        public LayupDataLayerIndex LayupDataLayerIndex
        {
            get => _layupDataLayerIndex;
            private set
            {
                if (_layupDataLayerIndex?.RegionData != null) _layupDataLayerIndex.RegionData.PropertyChanged -= RegionDataOnPropertyChanged;
                _layupDataLayerIndex = value;
                if (_layupDataLayerIndex?.RegionData != null) _layupDataLayerIndex.RegionData.PropertyChanged += RegionDataOnPropertyChanged;
                OnPropertyChanged();
            }
        }

        private void RegionDataOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpCommand?.RaiseCanExecuteChanged();
            DownCommand?.RaiseCanExecuteChanged();
            if (e.PropertyName != "Priority") _collectionVM.Refresh();
        }

        #endregion

        public DelegateCommand RemoveCommand { get; private set; }
        public DelegateCommand UpCommand { get; private set; }
        public DelegateCommand DownCommand { get; private set; }
        public DelegateCommand SelectStartCommand { get; private set; }
        public DelegateCommand SelectEndCommand { get; private set; }

        public ObservableCollection<FiberInfillTypeData> FiberInfillTypeCollection { get; private set; }

        public LayupDataVM()
        {
            #region FIBER INFILL TYPE

            FiberInfillTypeCollection = new ObservableCollection<FiberInfillTypeData>();
            var fiSolid = new FiberInfillTypeData
            {
                InfillType = FiberInfillType.SOLID,
                InfillTypeCaption = InfillFiber_en_EN.TypeSolid
            };
            FiberInfillTypeCollection.Add(fiSolid);
            var fiRombic = new FiberInfillTypeData
            {
                InfillType = FiberInfillType.CELLULAR_RHOMBIC,
                InfillTypeCaption = InfillFiber_en_EN.TypeRombicGrid
            };
            FiberInfillTypeCollection.Add(fiRombic);
            var fiIsogrid = new FiberInfillTypeData
            {
                InfillType = FiberInfillType.CELLULAR_ISOGRID,
                InfillTypeCaption = InfillFiber_en_EN.TypeIsogrid
            };
            FiberInfillTypeCollection.Add(fiIsogrid);
            var fiAnisogrid = new FiberInfillTypeData
            {
                InfillType = FiberInfillType.CELLULAR_ANISOGRID,
                InfillTypeCaption = InfillFiber_en_EN.TypeAnisogrid
            };
            FiberInfillTypeCollection.Add(fiAnisogrid);

            #endregion

            RemoveCommand = new DelegateCommand(RemoveExecuteMethod, () => !IsDefault);
            UpCommand = new DelegateCommand(UpExecuteMethod, () => !IsDefault && LayupDataLayerIndex.RegionData.Priority > 1);
            DownCommand = new DelegateCommand(DownExecuteMethod, () => !IsDefault && LayupDataLayerIndex.RegionData.Priority < GetMaxPriority() - 1);
            SelectStartCommand = new DelegateCommand(SelectStartExecuteMethod, () => !_isSelectingStart && !_isSelectingEnd);
            SelectEndCommand = new DelegateCommand(SelectEndExecuteMethod, () => !_isSelectingStart && !_isSelectingEnd);
            Messenger.Default.Register<MainWindow.MouseMoveMessage>(this, MouseMoveAction);
            Messenger.Default.Register<MainWindow.MouseDownMessage>(this, MouseDownAction);
        }

        private int GetMaxPriority()
        {
            return _collectionVM.Count == 0 ? 1 : _collectionVM.Max(a => a.LayupDataLayerIndex.RegionData.TruePriority);
        }

        #region DOWN EXECUTE

        private void DownExecuteMethod()
        {
            _collectionVM.PriorityPlus(this);
            UpCommand?.RaiseCanExecuteChanged();
            DownCommand?.RaiseCanExecuteChanged();
        }

        #endregion

        #region UP EXECUTE

        private void UpExecuteMethod()
        {
            _collectionVM.PriorityMinus(this);
            UpCommand?.RaiseCanExecuteChanged();
            DownCommand?.RaiseCanExecuteChanged();
        }

        #endregion

        #region REMOVE EXECUTE

        private void RemoveExecuteMethod()
        {
            _collectionVM.Remove(this);
        }

        #endregion

        #region SELECT EXECUTE

        private bool _isSelectingStart = false;
        private bool _isSelectingEnd = false;
        private void SelectStartExecuteMethod()
        {
            _isSelectingStart = true;
            SelectStartCommand?.RaiseCanExecuteChanged();
            SelectEndCommand?.RaiseCanExecuteChanged();

        }

        private void SelectEndExecuteMethod()
        {
            _isSelectingEnd = true;
            SelectStartCommand?.RaiseCanExecuteChanged();
            SelectEndCommand?.RaiseCanExecuteChanged();
        }

        private void MouseMoveAction(MainWindow.MouseMoveMessage obj)
        {
            if (!_isSelectingStart && !_isSelectingEnd) return;
            _layupViewerManager.HighlightLayer(obj.E);
        }

        private void MouseDownAction(MainWindow.MouseDownMessage obj)
        {
            if (_isSelectingStart || _isSelectingEnd)
            {
                var (isOntoLayer, height) = _layupViewerManager.GetHeight(obj.E);
                _layupViewerManager.DehighlightAll();
                if (isOntoLayer)
                {
                    if (_isSelectingStart)
                    {
                        if (height <= this.LayupDataLayerIndex.RegionData.EndHeightMM) this.LayupDataLayerIndex.RegionData.StartHeightMM = height;
                        _isSelectingStart = false;
                    }

                    if (_isSelectingEnd)
                    {
                        if (height >= this.LayupDataLayerIndex.RegionData.StartHeightMM) this.LayupDataLayerIndex.RegionData.EndHeightMM = height;
                        _isSelectingEnd = false;
                    }
                }

                SelectStartCommand?.RaiseCanExecuteChanged();
                SelectEndCommand?.RaiseCanExecuteChanged();
            }
        }

        #endregion

        public void Initialize(LayupDataCollectionVM collectionVM,
            ILayupViewerManager layupViewerManager,
            Color color,
            LayupDataLayerIndex data,
            bool isDefault,
            double zMin,
            double zMax,
            bool isAllowGenerateFiber)
        {
            IsAllowGenerateFiber = isAllowGenerateFiber;
            _collectionVM = collectionVM;
            _collectionVM.CollectionChanged += (sender, args) =>
            {
                UpCommand?.RaiseCanExecuteChanged();
                DownCommand?.RaiseCanExecuteChanged();
            };
            _layupViewerManager = layupViewerManager;
            LayupDataLayerIndex = data;
            Color = color;
            _isDefault = isDefault;
            ZMin = zMin;
            ZMax = zMax;
        }

        #region ON PROPERTY CHANGED

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string caller = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(caller));
        }

        #endregion
    }
}