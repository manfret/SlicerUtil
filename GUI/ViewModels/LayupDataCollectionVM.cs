using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;
using Aura.CORE.AuraFile;
using Aura.Managers;
using devDept.Eyeshot.Entities;
using MyToolkit.Messaging;
using Prism.Commands;
using Settings.LayupData;
using Settings.Memento;

namespace Aura.ViewModels
{
    public class LayupDataCollectionVM : ObservableCollection<LayupDataVM>
    {
        private EntityDatasItemVM _entityDatasItemVM;
        private List<EntityDatasItemVM> _allActivEntityDatasItemVms;
        private ILayupViewerManager _layupViewerManager;
        private ILayupRule _defaultLR;

        private readonly Dictionary<int, (Color mainColor, Color highlightColor)> _layerColors;

        public DelegateCommand AddNewLayupCommand { get; private set; }
        public DelegateCommand ApplyCommand { get; private set; }
        public DelegateCommand CancelCommand { get; private set; }
        public DelegateCommand RemoveCommand { get; private set; }
        public DelegateCommand PickCommand { get; private set; }


        public event EventHandler<List<LayupDataLayerIndex>> Applied;
        public event EventHandler Removed;
        public event EventHandler Cancelled;

        #region SELECTED ITEM

        private LayupDataVM _selectedItem;

        public LayupDataVM SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged(new PropertyChangedEventArgs("SelectedItem"));
            }
        }

        #endregion

        //ОБЯЗАТЕЛЬНО ДОЛЖЕН БЫТЬ ДЕФОЛТНЫЙ ИТЕМ
        public LayupDataCollectionVM()
        {
            AddNewLayupCommand = new DelegateCommand(AddNewLayupMethod, () => !IsPickingMode);
            ApplyCommand = new DelegateCommand(ApplyExecuteMethod, () => !IsPickingMode);
            CancelCommand = new DelegateCommand(CancelExecuteMethod, () => !IsPickingMode);
            RemoveCommand = new DelegateCommand(RemoveExecuteMethod, () => !IsPickingMode);
            PickCommand = new DelegateCommand(PickExecuteMethod, PickCommandCanExecuteMethod);

            #region STANDART COLORS

            //rgb(125, 206, 160)
            var green = (Color) ColorConverter.ConvertFromString("#FF7DCEA0");
            var lightGreen = new Color {A = green.A, R = 125 + 20, G = 206 + 20, B = 160 + 20};

            //rgb(247, 220, 111)
            var yellow = (Color) ColorConverter.ConvertFromString("#FFF7DC6F");
            var lightYellow = new Color {A = yellow.A, R = 255, G = 220 + 20, B = 111 + 20};

            //rgb(105, 93, 219)
            var violet = (Color) ColorConverter.ConvertFromString("#FF695ddb");
            var lightViolet = new Color {A = violet.A, R = 105 + 20, G = 93 + 20, B = 219 + 20};

            //rgb(229, 152, 102)
            var orange = (Color) ColorConverter.ConvertFromString("#FFE59866");
            var lightOrange = new Color {A = orange.A, R = 229 + 20, G = 152 + 20, B = 102 + 20};

            //rgb(118, 215, 196)
            var blueGreen = (Color) ColorConverter.ConvertFromString("#FF76D7C4");
            var lightBlueGreen = new Color {A = blueGreen.A, R = 118 + 20, G = 215 + 20, B = 196 + 20};

            //rgb(236, 112, 99)
            var red = (Color) ColorConverter.ConvertFromString("#FFEC7063");
            var lightRed = new Color {A = red.A, R = 255, G = 112 + 20, B = 99 + 20};

            //rgb(127, 179, 213)
            var blue = (Color) ColorConverter.ConvertFromString("#FF7FB3D5");
            var lightBlue = new Color {A = blue.A, R = 127 + 20, G = 179 + 20, B = 213 + 20};

            //rgb(240, 98, 146)
            var pink = (Color) ColorConverter.ConvertFromString("#FFF06292");
            var lightPink = new Color {A = pink.A, R = 255, G = 98 + 20, B = 146 + 20};

            //rgb(187, 143, 206)
            var purple = (Color) ColorConverter.ConvertFromString("#FFBB8FCE");
            var lightPurple = new Color {A = purple.A, R = 187 + 20, G = 143 + 20, B = 206 + 20};
            _layerColors = new Dictionary<int, (Color mainColor, Color highlightColor)>
            {
                {1, (mainColor: green, highlightColor: lightGreen)},
                {2, (mainColor: yellow, highlightColor: lightYellow)},
                {3, (mainColor: violet, highlightColor: lightViolet)},
                {4, (mainColor: orange, highlightColor: lightOrange)},
                {5, (mainColor: blueGreen, highlightColor: lightBlueGreen)},
                {6, (mainColor: red, highlightColor: lightRed)},
                {7, (mainColor: blue, highlightColor: lightBlue)},
                {8, (mainColor: pink, highlightColor: lightPink)},
                {9, (mainColor: purple, highlightColor: lightPurple)},
            };

            #endregion

            Messenger.Default.Register<MainWindow.MouseDownMessage>(this, MouseDownAction);
        }

        #region REMOVE EXECUTE

        private void RemoveExecuteMethod()
        {
            _isChanged = false;
            _layupViewerManager.ExitMode();
            Removed?.Invoke(this, null);
        }

        #endregion

        #region CANCEL EXECUTE

        private void CancelExecuteMethod()
        {
            _isChanged = false;
            _layupViewerManager.ExitMode();
            Cancelled?.Invoke(this, null);
        }

        #endregion

        #region APPLY EXECUTE

        private void ApplyExecuteMethod()
        {
            _isChanged = false;
            _layupViewerManager.ExitMode();
            Applied?.Invoke(this, Items.Select(a => a.LayupDataLayerIndex).ToList());
        }

        #endregion

        #region ADD NEW LAYUP METHOD

        private void AddNewLayupMethod()
        {
            AddNewLayup();
        }

        private class NewLayupData
        {
            public double StartMM { get; set; }
            public double EndMM { get; set; }
            public int LayerIndex { get; set; }
            public ILayupRule LayupRule { get; set; }
        }

        private void AddNewLayup(NewLayupData newLayupData = null)
        {
            var newLayupVM = new LayupDataVM();

            var layerIndex = newLayupData == null
                ? GetLayerIndex(Items.Max(a => a.LayupDataLayerIndex.LayerIndex) + 1)
                : GetLayerIndex(newLayupData.LayerIndex);

            var newLIData = new LayupDataLayerIndex(new RegionData
            {
                StartHeightMM = newLayupData?.StartMM ?? _entityDatasItemVM.Entity.BoxMin.Z,
                EndHeightMM = newLayupData?.EndMM ?? _entityDatasItemVM.Entity.BoxMax.Z,
                LayupRule = newLayupData?.LayupRule ?? new LayupRule(_defaultLR)
            }, layerIndex);
            newLayupVM.Initialize(this, _layupViewerManager, _layerColors[layerIndex].mainColor, newLIData, false,
                _entityDatasItemVM.Entity.BoxMin.Z, _entityDatasItemVM.Entity.BoxMax.Z,
                _sessionMemento.Profile.GlobalSettingsMemento.TrueAllowGenerateFiber);


            this.Insert(0, newLayupVM);
            SelectedItem = newLayupVM;
        }

        #endregion

        #region ON COLLECTION CHANGED

        private bool _supressRefresh;

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);

            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (var item in Items)
                {
                    if (item.LayupDataLayerIndex.RegionData.Priority != IndexOf(item) + 1)
                        item.LayupDataLayerIndex.RegionData.Priority = IndexOf(item) + 1;
                }

                if (!_supressRefresh) Refresh();
            }

            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (var item in Items)
                {
                    if (item.LayupDataLayerIndex.RegionData.Priority != IndexOf(item) + 1)
                        item.LayupDataLayerIndex.RegionData.Priority = IndexOf(item) + 1;
                }

                if (!_supressRefresh) Refresh();
            }
        }

        #endregion

        #region PRIORITY PLUS

        public void PriorityPlus(LayupDataVM vm)
        {
            var next = this.Single(a => a.LayupDataLayerIndex.RegionData.Priority == vm.LayupDataLayerIndex.RegionData.Priority + 1);
            var vmIndex = IndexOf(vm);
            _supressRefresh = true;
            this[vmIndex] = next;
            this[vmIndex + 1] = vm;
            _supressRefresh = false;
            Refresh();
        }

        #endregion

        #region PRIORITY MINUS

        public void PriorityMinus(LayupDataVM vm)
        {
            var previous = this.Single(a => a.LayupDataLayerIndex.RegionData.Priority == vm.LayupDataLayerIndex.RegionData.Priority - 1);
            var vmIndex = IndexOf(vm);
            _supressRefresh = true;
            this[vmIndex] = previous;
            this[vmIndex - 1] = vm;
            _supressRefresh = false;
            Refresh();
        }

        #endregion

        #region GET LAYER INDEX

        private int GetLayerIndex(int tryingLayerIndex)
        {
            var layerIndex = tryingLayerIndex;
            while (layerIndex > _layerColors.Count)
            {
                layerIndex -= _layerColors.Count;
            }

            return layerIndex;
        }

        #endregion

        #region INITIALIZE

        public void Initialize(ILayupViewerManager layupViewerManager)
        {
            _layupViewerManager = layupViewerManager;
            _layupViewerManager.Completed += (sender, args) => OnManagerCompleted();
        }

        private void OnManagerCompleted()
        {
            if (_entityDatasItemVM == null) return;

            var layupDataLayerIndexes = new List<LayupDataLayerIndex>();

            //если нет готовых данных
            if (_entityDatasItemVM.Datas == null || _entityDatasItemVM.Datas.Count == 0)
            {
                var layerIndex = _layerColors.Min(a => a.Key);
                var newLiData = new LayupDataLayerIndex(new RegionData
                {
                    StartHeightMM = _entityDatasItemVM.Entity.BoxMin.Z,
                    EndHeightMM = _entityDatasItemVM.Entity.BoxMax.Z,
                    LayupRule = new LayupRule(_defaultLR)
                }, layerIndex);
                layupDataLayerIndexes.Add(newLiData);
            }
            //если есть, то присваиваем c клонированием
            else
            {
                layupDataLayerIndexes = _entityDatasItemVM.Datas.Select(a => a.Clone()).ToList();
            }

            //сортируем
            layupDataLayerIndexes = layupDataLayerIndexes.OrderByDescending(a => a.RegionData.Priority).ToList();

            //сначала добавляем дефолтный
            var maxPriority = layupDataLayerIndexes.Max(a => a.RegionData.Priority);
            var defaultItem = layupDataLayerIndexes.Single(a => a.RegionData.Priority == maxPriority);
            defaultItem.RegionData.LayupRule = _defaultLR;
            var defaultDataVM = new LayupDataVM();
            defaultDataVM.Initialize(this, _layupViewerManager, _layerColors[defaultItem.LayerIndex].mainColor, defaultItem, true,
                _entityDatasItemVM.Entity.BoxMin.Z, _entityDatasItemVM.Entity.BoxMax.Z,
                _sessionMemento.Profile.GlobalSettingsMemento.TrueAllowGenerateFiber);
            Add(defaultDataVM);

            //потом добавляем все остальные
            foreach (var layupDataLayerIndex in layupDataLayerIndexes)
            {
                if (layupDataLayerIndex.Equals(defaultItem)) continue;

                var dDataVM = new LayupDataVM();
                dDataVM.Initialize(this, _layupViewerManager, _layerColors[layupDataLayerIndex.LayerIndex].mainColor,
                    layupDataLayerIndex, false, _entityDatasItemVM.Entity.BoxMin.Z, _entityDatasItemVM.Entity.BoxMax.Z,
                    _sessionMemento.Profile.GlobalSettingsMemento.TrueAllowGenerateFiber);
                Insert(0, dDataVM);
            }
        }

        private bool _isChanged;
        private ISessionMemento _sessionMemento;

        public void Initialize(EntityDatasItemVM entityDatasItemVM,
            List<EntityDatasItemVM> allActivEntityDatasItemVms,
            ILayupRule defaultLR,
            double macroLayerHeight,
            ISessionMemento sessionMemento)
        {
            Items.Clear();
            _sessionMemento = sessionMemento;

            _isChanged = true;
            _entityDatasItemVM = entityDatasItemVM;
            if (_entityDatasItemVM != null && _entityDatasItemVM.Datas != null)
            {
                foreach (var t in _entityDatasItemVM.Datas)
                {
                    t.LayerIndex = GetLayerIndex(t.LayerIndex);
                }
            }

            _allActivEntityDatasItemVms = allActivEntityDatasItemVms;
            _defaultLR = defaultLR;

            _layupViewerManager.EnterMode(_allActivEntityDatasItemVms.Select(a => a.Entity).ToList(), entityDatasItemVM.Entity,
                macroLayerHeight, _layerColors);

            PickCommand?.RaiseCanExecuteChanged();
            _layupViewerManager.Focus();
        }

        #endregion

        #region ON UNLOAD

        public void OnUnload()
        {
            if (_isChanged)
            {
                var applyCanExecute = ApplyCommand.CanExecute();
                if (applyCanExecute) ApplyCommand.Execute();
                else CancelCommand.Execute();
            }
        }

        #endregion

        #region REFRESH

        public void Refresh()
        {
            var datas = Items.Select(a => (a.LayupDataLayerIndex.LayerIndex, a.LayupDataLayerIndex.RegionData.TruePriority,
                a.LayupDataLayerIndex.RegionData.StartHeightMM, a.LayupDataLayerIndex.RegionData.EndHeightMM)).ToList();
            _layupViewerManager.Refresh(datas);
        }

        #endregion

        #region GET DATAS FROM ACTIVE

        private EntityDatasItemVM GetDatasFromActive(Entity entity)
        {
            return _allActivEntityDatasItemVms.SingleOrDefault(a => a.Entity.Equals(entity));
        }

        #endregion

        #region MOUSE DOWN ACTION

        private void MouseDownAction(MainWindow.MouseDownMessage obj)
        {
            if (_isPickingMode)
            {
                GetDatasFromAnotherModel(obj);
            }
        }

        private void GetDatasFromAnotherModel(MainWindow.MouseDownMessage obj)
        {
            var selectedEntity = _layupViewerManager.GetUnderMouse(obj.E);
            if (selectedEntity == null || selectedEntity.Equals(this._entityDatasItemVM.Entity))
            {
                ExitPickMode();
                return;
            }

            var datas = GetDatasFromActive(selectedEntity);
            if (datas == null)
            {
                ExitPickMode();
                return;
            }

            if (datas.Datas == null)
            {
                ExitPickMode();
                RemoveCommand.Execute();
                return;
            }

            UpdateDatas(datas.Datas);
            ExitPickMode();
        }

        private void UpdateDatas(List<LayupDataLayerIndex> datas)
        {
            //пропускаем дефолтный - он у нас уже есть
            var defaultItem = Items.Last();
            Clear();
            Add(defaultItem);
            var updatedData = _entityDatasItemVM.UpdateLayupDatas(datas);
            foreach (var data in updatedData.OrderByDescending(a => a.RegionData.Priority).Skip(1))
            {
                var vm = new LayupDataVM();
                vm.Initialize(this, _layupViewerManager, _layerColors[data.LayerIndex].mainColor, data, false,
                    _entityDatasItemVM.Entity.BoxMin.Z, _entityDatasItemVM.Entity.BoxMax.Z,
                    _sessionMemento.Profile.GlobalSettingsMemento.TrueAllowGenerateFiber);
                Insert(0, vm);
            }
        }

        #endregion

        #region PICK EXECUTE

        private bool _isPickingMode;

        private bool IsPickingMode
        {
            get => _isPickingMode;
            set
            {
                _isPickingMode = value;
                AddNewLayupCommand?.RaiseCanExecuteChanged();
                ApplyCommand?.RaiseCanExecuteChanged();
                CancelCommand?.RaiseCanExecuteChanged();
                RemoveCommand?.RaiseCanExecuteChanged();
                PickCommand?.RaiseCanExecuteChanged();
            }
        }

        private void PickExecuteMethod()
        {
            IsPickingMode = true;
            _layupViewerManager.SetCursorToEyeDropper();
        }

        private void ExitPickMode()
        {
            IsPickingMode = false;
            _layupViewerManager.SetCursorToDefault();
        }

        private bool PickCommandCanExecuteMethod()
        {
            return _allActivEntityDatasItemVms.Count > 1 && !_isPickingMode;
        }

        #endregion
    }
}