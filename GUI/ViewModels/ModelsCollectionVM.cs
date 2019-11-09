using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Aura.CORE.AuraFile;
using Aura.Managers;
using Aura.Themes.Localization;
using devDept.Eyeshot.Entities;
using devDept.Geometry;
using Microsoft.Win32;
using MyToolkit.Messaging;
using Prism.Commands;
using Settings.Stores;
using Unity;
using Unity.Interception.Utilities;
using Util;

namespace Aura.ViewModels
{
    public class ModelsCollectionVM : CollectionViewModel<EntityDatasItemVM>
    {
        private IAuraViewportManager _auraViewportManager;
        private ILayupViewerManager _layupViewerManager;
        private ISettingsManager _settingsManager;

        public event EventHandler ModelsChanged;

        public DelegateCommand OpenModelCommand { get; private set; }
        public DelegateCommand RemoveModelCommand { get; private set; }
        public DelegateCommand ShiftModelCommand { get; private set; }
        public DelegateCommand RotateModelCommand { get; private set; }
        public DelegateCommand ResizeModelCommand { get; private set; }
        public DelegateCommand CloneModelCommand { get; private set; }
        public DelegateCommand LayupModelCommand { get; private set; }

        #region SLICING IN PROCESS

        private bool _slicingInProcess;
        public bool SlicingInProcess
        {
            get => _slicingInProcess;
            set
            {
                _slicingInProcess = value;
                OpenModelCommand?.RaiseCanExecuteChanged();
                RemoveModelCommand?.RaiseCanExecuteChanged();
                ShiftModelCommand?.RaiseCanExecuteChanged();
                RotateModelCommand?.RaiseCanExecuteChanged();
                ResizeModelCommand?.RaiseCanExecuteChanged();
                CloneModelCommand?.RaiseCanExecuteChanged();
                LayupModelCommand?.RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region IN PROCESS

        private bool _inProcess;

        public bool InProcess
        {
            get => _inProcess;
            set
            {
                _inProcess = value;
                OnPropertyChanged();
                RemoveModelCommand?.RaiseCanExecuteChanged();
                ShiftModelCommand?.RaiseCanExecuteChanged();
                RotateModelCommand?.RaiseCanExecuteChanged();
                ResizeModelCommand?.RaiseCanExecuteChanged();
                CloneModelCommand?.RaiseCanExecuteChanged();
                LayupModelCommand?.RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region CURRENT ACTION VM

        private ShiftModelVM _shiftModelVM;
        private RotateModelVM _rotateModelVM;
        private ResizeModelVM _resizeModelVM;
        private LayupDataCollectionVM _layupDataCollectionVM;

        private INotifyPropertyChanged _currentActionVM;

        public INotifyPropertyChanged CurrentActionVM
        {
            get => _currentActionVM;
            private set
            {
                _currentActionVM = value;
                OnPropertyChanged();
                InProcess = CurrentActionVM != null;
            }
        }

        private void FinishAction()
        {
            if (CurrentActionVM != null && CurrentActionVM is ShiftModelVM shiftVM)
            {
                shiftVM.OnUnload();
            }

            if (CurrentActionVM != null && CurrentActionVM is RotateModelVM rotateVM)
            {
                rotateVM.OnUnload();
            }

            if (CurrentActionVM != null && CurrentActionVM is ResizeModelVM resizeVM)
            {
                resizeVM.OnUnload();
            }

            if (CurrentActionVM != null && CurrentActionVM is LayupDataCollectionVM layupDataCollectionVM)
            {
                layupDataCollectionVM.OnUnload();
            }
        }

        #endregion

        #region HAS ACTIVE ITEMS

        private bool _hasActiveItems;

        public bool HasActiveItems
        {
            get => _hasActiveItems;
            private set
            {
                if (_hasActiveItems == value) return;
                _hasActiveItems = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region ACTIVE ITEMS

        private ObservableCollection<EntityDatasItemVM> _activeItems;

        public ObservableCollection<EntityDatasItemVM> ActiveItems
        {
            get => _activeItems;
            private set
            {
                _activeItems = value;
                OnPropertyChanged();
                HasActiveItems = _activeItems != null && _activeItems.Count > 0;
            }
        }

        #endregion

        #region INTERACTIVE ITEMS

        private bool _hasInteractiveItems;

        public bool HasInteractiveItems
        {
            get => _hasInteractiveItems;
            private set
            {
                _hasInteractiveItems = value;
                OnPropertyChanged();
                RemoveModelCommand?.RaiseCanExecuteChanged();
                ShiftModelCommand?.RaiseCanExecuteChanged();
                RotateModelCommand?.RaiseCanExecuteChanged();
                ResizeModelCommand?.RaiseCanExecuteChanged();
                CloneModelCommand?.RaiseCanExecuteChanged();
                LayupModelCommand?.RaiseCanExecuteChanged();
            }
        }

        private ObservableCollection<EntityDatasItemVM> _interactiveItems;

        public ObservableCollection<EntityDatasItemVM> InteractiveItems
        {
            get => _interactiveItems;
            private set
            {
                _interactiveItems = value;
                OnPropertyChanged();
                HasInteractiveItems = _interactiveItems != null && _interactiveItems.Count > 0;
            }
        }

        #endregion

        #region CURRENT COLLISION TYPES

        private List<CollisionType> _collisionTypes;
        public List<CollisionType> CollisionTypes
        {
            get => _collisionTypes;
            private set
            {
                if (Equals(_collisionTypes, value) && Enumerable.SequenceEqual(_collisionTypes, value)) return;
                _collisionTypes = value;
                OnPropertyChanged();
            }
        }

        #endregion

        //methods

        #region ADD MODEL DATA

        public void AddModelData((Entity entity, string name) entityName, List<LayupDataLayerIndex> datas, bool enabled, bool isSelected,
            bool isDataEnabled)
        {
            var newItem = new EntityDatasItemVM(entityName.entity, datas, GetUniqueName(entityName.name), enabled, isSelected, isDataEnabled);
            Items.Add(newItem);
        }

        #region GET UNIQUE NAME

        private string GetUniqueName(string name)
        {
            var duplicates = Items.Where(a => a.EntityName.StartsWith(name)).ToList();
            if (duplicates.Count == 0) return name;

            var lastNum = 0;
            foreach (var duplicate in duplicates)
            {
                var afterPart2 = duplicate.EntityName.Substring(name.Length);
                if (int.TryParse(afterPart2, out var num) && num > lastNum) lastNum = num + 1;
            }

            return name + lastNum;
        }

        #endregion

        #endregion

        #region GET BY ENTITY

        public (List<LayupDataLayerIndex> datas, string name, bool enabled) GetByEntity(Entity entity)
        {
            var item = this[entity];
            return (item.Datas, item.EntityName, item.IsEnabled);
        }

        #endregion

        #region SET REGION DATAS

        public void SetRegionDatas(Entity entity, List<LayupDataLayerIndex> datas)
        {
            this[entity].Datas = datas;
        }

        #endregion

        #region RESET REGION DATAS

        public void ResetRegiondatas(Entity entity)
        {
            this[entity].Datas = null;
        }

        #endregion

        #region GET MODELS COUNT

        public int GetModelsCount()
        {
            return Items.Count;
        }

        #endregion

        #region CLEAR

        public void Clear()
        {
            for (var i = Items.Count - 1; i >= 0; i--)
            {
                Items.Remove(Items[i]);
            }
        }

        #endregion

        #region CONTAINS KEY

        public bool ContainsKey(Entity entity)
        {
            return Items.Count(a => a.Entity.Equals(entity)) > 0;
        }

        #endregion

        #region WHERE

        public IEnumerable<EntityDatasItemVM> Where(Func<EntityDatasItemVM, bool> func)
        {
            return Items.Where(func);
        }

        #endregion

        #region MIN

        public TResult Min<TResult>(Func<EntityDatasItemVM, TResult> func)
        {
            return Items.Min(func);
        }

        #endregion

        #region MAX

        public TResult Max<TResult>(Func<EntityDatasItemVM, TResult> func)
        {
            return Items.Max(func);
        }

        #endregion

        #region FOREACH

        public void Foreach(Action<EntityDatasItemVM> action)
        {
            Items.ForEach(action);
        }

        #endregion

        #region SELECT

        public IEnumerable<TResult> Select<TResult>(Func<EntityDatasItemVM, TResult> func)
        {
            return Items.Select(func);
        }

        #endregion

        #region []

        public EntityDatasItemVM this[Entity entity] => Items.Single(a => a.Entity.Equals(entity));

        #endregion

        #region GET NAME

        public string GetName()
        {
            var projNameFromModels = string.Empty;
            var allUniqueNames = Items.Select(a => a.EntityName).Distinct();
            foreach (var uniqueName in allUniqueNames)
            {
                projNameFromModels += uniqueName + "_";
            }

            return projNameFromModels.TrimEnd('_');
        }

        #endregion

        #region GET ITEMS

        public List<EntityDatasItemVM> GetItems()
        {
            return Items.ToList();
        }

        #endregion

        #region GET INTERACTIVE ENTITIES

        private List<EntityDatasItemVM> GetInteractiveEntities()
        {
            return Items.Where(a => a.IsEnabled && a.IsSelected).ToList();
        }

        #endregion

        #region GET ENTITIES CENTER

        private Point2D GeEntitiesCenter()
        {
            var allEntities = InteractiveItems.Select(a => a.Entity).ToList();

            if (allEntities.Count == 0)
            {
                return new Point3D();
            }

            var minX = allEntities.Min(a => a.BoxMin.X);
            var maxX = allEntities.Max(a => a.BoxMax.X);
            var minY = allEntities.Min(a => a.BoxMin.Y);
            var maxY = allEntities.Max(a => a.BoxMax.Y);

            return new Point2D((minX + maxX) / 2, (minY + maxY) / 2);
        }

        #endregion

        public ModelsCollectionVM()
        {
            InteractiveItems = new ObservableCollection<EntityDatasItemVM>();
            InteractiveItems.CollectionChanged += (sender, args) =>
            {
                HasInteractiveItems = InteractiveItems.Count > 0;
                OnPropertyChanged("InteractiveItems");
            };
            ActiveItems = new ObservableCollection<EntityDatasItemVM>();
            ActiveItems.CollectionChanged += (sender, args) =>
            {
                HasActiveItems = ActiveItems.Count > 0;
                OnPropertyChanged("ActiveItems");
            };
        }

        #region INITIALIZE

        private void ItemOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "IsSelected" && e.PropertyName != "IsEnabled" && e.PropertyName != "IsDataEnabled") return;

            if (!(sender is EntityDatasItemVM item)) throw new Exception();

            switch (e.PropertyName)
            {
                case "IsSelected":
                    _auraViewportManager.SetSelection(item.Entity, item.IsSelected);
                    if (item.IsSelected)
                    {
                        if (!InteractiveItems.Contains(item)) InteractiveItems.Add(item);
                    }
                    else
                    {
                        if (InteractiveItems.Contains(item)) InteractiveItems.Remove(item);
                    }

                    break;
                case "IsEnabled":
                    _auraViewportManager.SetVisibility(item.Entity, item.IsEnabled);
                    if (!item.IsEnabled)
                    {
                        if (InteractiveItems.Contains(item)) InteractiveItems.Remove(item);
                        if (ActiveItems.Contains(item)) ActiveItems.Remove(item);
                    }
                    else
                    {
                        if (!ActiveItems.Contains(item)) ActiveItems.Add(item);
                    }
                    _auraViewportManager.CheckIntersections();
                    break;
                case "IsDataEnabled":
                    ModelsChanged?.Invoke(this, null);
                    break;
            }
        }

        #endregion

        private bool _blockCheckIntersections;
        public void Initialize(IAuraViewportManager auraViewportManager, ILayupViewerManager layupViewerManager, ISettingsManager settingsManager)
        {
            _auraViewportManager = auraViewportManager;
            _layupViewerManager = layupViewerManager;
            _settingsManager = settingsManager;

            OpenModelCommand = new DelegateCommand(OpenModel, () =>  !SlicingInProcess);
            RemoveModelCommand = new DelegateCommand(RemoveModelExecuteMethod, () => HasInteractiveItems && !InProcess && !SlicingInProcess);
            ShiftModelCommand = new DelegateCommand(ShiftExecuteMethod, () => HasInteractiveItems && !InProcess && !SlicingInProcess);
            RotateModelCommand = new DelegateCommand(RotateExecuteMethod, () => HasInteractiveItems && !InProcess && !SlicingInProcess);
            ResizeModelCommand = new DelegateCommand(ResizeExecuteMethod, () => HasInteractiveItems && !InProcess && !SlicingInProcess);
            CloneModelCommand = new DelegateCommand(CloneExecuteMethod, () => HasInteractiveItems && !InProcess && !SlicingInProcess);
            LayupModelCommand = new DelegateCommand(LayupModelExecuteMethod, () => HasInteractiveItems && InteractiveItems.Count == 1 && !InProcess && !SlicingInProcess);

            Items.CollectionChanged += (sender, args) =>
            {
                if (args.NewItems != null && args.NewItems.Count > 0)
                {
                    foreach (var newItem in args.NewItems)
                    {
                        var item = (EntityDatasItemVM) newItem;
                        item.PropertyChanged += ItemOnPropertyChanged;
                        if (!ActiveItems.Contains(item)) ActiveItems.Add(item);
                        _auraViewportManager.AddTrueEntity(item.Entity);
                        _auraViewportManager.PutEntityOnTable(item.Entity);
                        _auraViewportManager.SetVisibility(item.Entity, item.IsEnabled);
                    }
                    if (!_blockCheckIntersections) _auraViewportManager.CheckIntersections();
                }

                if (args.OldItems != null && args.OldItems.Count > 0)
                {
                    foreach (var oldItem in args.OldItems)
                    {
                        var item = (EntityDatasItemVM) oldItem;
                        item.PropertyChanged -= ItemOnPropertyChanged;
                        if (ActiveItems.Contains(item)) ActiveItems.Remove(item);
                        _auraViewportManager.RemoveEntity(item.Entity);
                    }
                    if (!_blockCheckIntersections) _auraViewportManager.CheckIntersections();
                }
            };
            _auraViewportManager.ModelsAdded += (sender, list) =>
            {
                foreach (var tuple in list)
                {
                    AddModelData(tuple, null, true, false, true);
                }
            };
            _auraViewportManager.CollisionDetectionCalculated += (sender, list) =>
            {
                CollisionTypes = list;
            };

            Messenger.Default.Register<MainWindow.MouseDownMessage>(this, SelectDeselect);
        }

        #region OPEN MODEL EXECUTE

        private void OpenModel()
        {
            var openFileDialog = new OpenFileDialog
            {
                InitialDirectory = _settingsManager.ModelDirectory,
                Filter = Common_en_EN.ModelFilter
            };
            if (openFileDialog.ShowDialog() == true)
            {
                _settingsManager.ModelDirectory = Path.GetDirectoryName(openFileDialog.FileName);
                OpenModel(openFileDialog.FileName);
                ModelsChanged?.Invoke(this, null);
            }
        }

        public void OpenModel(string path)
        {
            var extension = Path.GetExtension(path).ToLower();
            switch (extension)
            {
                case ".stl":
                    var rstl = new MyReadSTL(path);
                    rstl.FileName = Path.GetFileNameWithoutExtension(path);
                    _auraViewportManager.AddWorkUnit(rstl);
                    break;
                case ".step":
                case ".stp":
                    var rstep = new MyReadSTEP(path);
                    rstep.FileName = Path.GetFileNameWithoutExtension(path);
                    _auraViewportManager.AddWorkUnit(rstep);
                    break;
                case ".3ds":
                    var r3Ds = new MyRead3DS(path);
                    r3Ds.FileName = Path.GetFileNameWithoutExtension(path);
                    _auraViewportManager.AddWorkUnit(r3Ds);
                    break;
                case ".obj":
                    var rObj = new MyReadOBJ(path);
                    rObj.FileName = Path.GetFileNameWithoutExtension(path);
                    _auraViewportManager.AddWorkUnit(rObj);
                    break;
            }
        }

        #endregion

        #region LOAD MODELS

        public void LoadModels(List<EntityLayupData> entityLayupDatas)
        {
            Clear();
            _blockCheckIntersections = true;
            foreach (var modelData in entityLayupDatas)
            {
                var mesh = new Mesh(modelData.Points.Count, modelData.Triangles.Count, Mesh.natureType.Smooth);
                for (var i = 0; i < mesh.Vertices.Length; i++)
                {
                    mesh.Vertices[i] = new Point3D(modelData.Points[i].X, modelData.Points[i].Y, modelData.Points[i].Z);
                }

                for (var i = 0; i < mesh.Triangles.Length; i++)
                {
                    var tri = modelData.Triangles[i];
                    mesh.Triangles[i] = new SmoothTriangle(tri.V1, tri.V2, tri.V3, tri.N1, tri.N2, tri.N3);
                }

                AddModelData((mesh, modelData.ModelName), modelData.LayupDatas, modelData.TrueEnabled, false, modelData.TrueEnableData);
            }
            _blockCheckIntersections = false;
            _auraViewportManager.CheckIntersections();
            ModelsChanged?.Invoke(this, null);
        }

        #endregion

        #region REMOVE EXECUTE

        private void RemoveModelExecuteMethod()
        {
            foreach (var interactiveItem in InteractiveItems)
            {
                this.Items.Remove(interactiveItem);
            }
            InteractiveItems.Clear();
            ModelsChanged?.Invoke(this, null);
        }

        #endregion

        #region SHIFT EXECUTE

        private void ShiftExecuteMethod()
        {
            if (_shiftModelVM == null)
            {
                _shiftModelVM = new ShiftModelVM();
                _shiftModelVM.Initialize(_auraViewportManager, _interactiveItems.ToList());
                _shiftModelVM.Applied += (sender, args) =>
                {
                    _shiftModelVM.OnUnload();
                    ModelsChanged?.Invoke(this, null);
                    CurrentActionVM = null;
                };
                _shiftModelVM.Canceled += (sender, args) =>
                {
                    _shiftModelVM.OnUnload();
                    CurrentActionVM = null;
                };
            }

            _shiftModelVM.OnLoad();
            CurrentActionVM = _shiftModelVM;
        }

        #endregion

        #region ROTATE EXECUTE

        private void RotateExecuteMethod()
        {
            if (_rotateModelVM == null)
            {
                _rotateModelVM = new RotateModelVM();
                _rotateModelVM.Initialize(_auraViewportManager, _interactiveItems);
                _rotateModelVM.Applied += (sender, args) =>
                {
                    foreach (var interactiveItem in InteractiveItems)
                    {
                        interactiveItem.UpdateSelfLayupDatas();
                    }

                    _rotateModelVM.OnUnload();
                    ModelsChanged?.Invoke(this, null);
                    CurrentActionVM = null;
                };
                _rotateModelVM.Canceled += (sender, args) =>
                {
                    _rotateModelVM.OnUnload();
                    CurrentActionVM = null;
                };
            }

            _rotateModelVM.OnLoad();
            CurrentActionVM = _rotateModelVM;
        }

        #endregion

        #region RESIZE EXECUTE

        private void ResizeExecuteMethod()
        {
            if (_resizeModelVM == null)
            {
                _resizeModelVM = new ResizeModelVM();
                _resizeModelVM.Initialize(_auraViewportManager, _interactiveItems);
                _resizeModelVM.Applied += (sender, args) =>
                {
                    foreach (var interactiveItem in InteractiveItems)
                    {
                        interactiveItem.UpdateSelfLayupDatas();
                    }

                    _resizeModelVM.OnUnload();
                    ModelsChanged?.Invoke(this, null);
                    CurrentActionVM = null;
                };
                _resizeModelVM.Canceled += (sender, args) =>
                {
                    _resizeModelVM.OnUnload();
                    CurrentActionVM = null;
                };
            }

            _resizeModelVM.OnLoad();
            CurrentActionVM = _resizeModelVM;
        }

        #endregion

        #region CLONE EXECUTE

        private void CloneExecuteMethod()
        {
            foreach (var entityDatasItemVm in InteractiveItems)
            {
                var cloneEntity = entityDatasItemVm.Entity.Clone() as Entity;
                AddModelData((cloneEntity, entityDatasItemVm.EntityName + "(Clone)"),
                    entityDatasItemVm.Datas?.Select(a => a.Clone()).ToList(), true, false, entityDatasItemVm.IsDataEnabled);
            }

            _auraViewportManager.CheckIntersections();
            ModelsChanged?.Invoke(this, null);
        }

        #endregion

        #region LAYUP EXECUTE

        private void LayupModelExecuteMethod()
        {
            var sessionMemento = UnityCore.Container.Resolve<ISessionStore>().Session;

            if (_layupDataCollectionVM == null)
            {
                _layupDataCollectionVM = new LayupDataCollectionVM();
                _layupDataCollectionVM.Initialize(_layupViewerManager);
                _layupDataCollectionVM.Applied += (sender, list) =>
                {
                    _layupDataCollectionVM.OnUnload();
                    SetRegionDatas(InteractiveItems.Single().Entity, list);
                    ModelsChanged?.Invoke(this, null);
                    CurrentActionVM = null;
                };
                _layupDataCollectionVM.Removed += (sender, args) =>
                {
                    _layupDataCollectionVM.OnUnload();
                    SetRegionDatas(InteractiveItems.Single().Entity, null);
                    ModelsChanged?.Invoke(this, null);
                    CurrentActionVM = null;
                };
                _layupDataCollectionVM.Cancelled += (sender, args) =>
                {
                    _layupDataCollectionVM.OnUnload();
                    CurrentActionVM = null;
                };
            }

            _layupDataCollectionVM.Initialize(InteractiveItems.Single(), GetItems(), sessionMemento.Profile.LayupRule,
                sessionMemento.Profile.GlobalSettingsMemento.TrueMacroLayerHeight, sessionMemento);
            CurrentActionVM = _layupDataCollectionVM;
        }

        #endregion

        #region SELECT DESELECT

        private void SelectDeselect(MainWindow.MouseDownMessage obj)
        {
            if (InProcess) return;
            var e = obj.E;

            var selectedEntity = _auraViewportManager.SelectUnderMouse(e);
            if (selectedEntity != null)
            {
                var entity = Items.Single(a => a.Entity.Equals(selectedEntity));
                entity.IsSelected = !entity.IsSelected;
            }
            else
            {
                _auraViewportManager.DeselectAll();
                Items.ForEach(a => a.IsSelected = false);
            }
        }

        public void DeselectAll()
        {
            Items.ForEach(a => a.IsSelected = false);
            _auraViewportManager.DeselectAll();
        }

        #endregion

        public void OnUnload()
        {
            FinishAction();
        }
    }
}