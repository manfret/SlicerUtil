using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using Aura.AuraFile;
using Aura.Managers;
using Aura.Themes.Localization;
using devDept.Eyeshot.Entities;
using devDept.Geometry;
using Microsoft.Win32;
using Prism.Commands;
using Unity.Interception.Utilities;

namespace Aura.ViewModels
{
    public class ModelWithDataCollectionVM : CollectionViewModel<EntityDatasItemVM>
    {
        private readonly IAuraViewportManager _auraViewportManager;
        public DelegateCommand OpenModelCommand { get; }
        public DelegateCommand RemoveModelsCommand { get; }

        private bool _isIntersected;
        public bool IsIntersected
        {
            get => _isIntersected;
            set
            {
                _isIntersected = value;
                OnPropertyChanged();
            }
        }

        private ModelModes _modelModes;
        public ModelModes ModelModes
        {
            get => _modelModes;
            set
            {
                _modelModes = value;
                OnPropertyChanged();
            }
        }

        #region OPEN PROJECT

        private void OpenProjectFileDialog()
        {
            if (_projectChanged && !Common_en_EN.DefaultProjectName.Equals(_currentProjectName))
            {
                var res = MessageBox.Show(Common_en_EN.OpenProjectHasChanges.Replace("{PROJECT_NAME}", _currentProjectName), Common_en_EN.OpenProjectTitle, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                switch (res)
                {
                    case MessageBoxResult.Cancel:
                        return;
                    case MessageBoxResult.Yes:
                        SaveProjectAsDialog();
                        break;
                }
            }

            var openFileDialog = new OpenFileDialog
            {
                InitialDirectory = _settingsManager.ProjectDirectory,
                Filter = Common_en_EN.ProjectFilter
            };
            if (openFileDialog.ShowDialog() == true)
            {
                _currentProjectFullPath = openFileDialog.FileName;
                OpenProject(_currentProjectFullPath);
            }
        }

        private void OpenProject(string path)
        {
            _settingsManager.ProjectDirectory = Path.GetDirectoryName(path);
            var auraMainPrinting = new AuraMainPrinting();
            var (res, entityLayupDatas, sessionMemento, projectName) = auraMainPrinting.LoadFile(path);
            if (res)
            {
                _sessionStore.Session.Printer.PropertyChanged -= PrinterOnPropertyChanged;
                _sessionStore.Session.PropertyChanged -= SessionOnPropertyChanged;
                _sessionStore.Session = sessionMemento;
                _sessionStore.Session.PropertyChanged += PrinterOnPropertyChanged;
                _sessionStore.Session.PropertyChanged += SessionOnPropertyChanged;

                LoadModels(entityLayupDatas);
                //SessionPanel.RefreshProfiles();

                _auraViewportManager.UpdateTable();
                UpdateTitle(projectName);
            }
        }

        #endregion

        #region HAS INTERACTIVE ITEMS

        private bool _hasInteractiveItems;

        public bool HasInteractiveItems
        {
            get => _hasInteractiveItems;
            private set
            {
                _hasInteractiveItems = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region INTERACTIVE ITEMS

        private List<EntityDatasItemVM> _interactiveItems;
        public List<EntityDatasItemVM> InteractiveItems
        {
            get => _interactiveItems;
            set
            {
                _interactiveItems = value;
                OnPropertyChanged();
                HasInteractiveItems = _interactiveItems != null && _interactiveItems.Count > 0;
            }
        }

        #endregion

        #region ADD MODEL DATA

        public void AddModelData(Entity entity, List<LayupDataLayerIndex> datas, bool enabled = true, bool isSelected = false)
        {
            var newItem = new EntityDatasItemVM(entity, datas, GetUniqueName(entity), enabled, isSelected);
            Items.Add(newItem);
        }

        #region GET UNIQUE NAME

        private string GetUniqueName(Entity entity)
        {
            var part1 = entity.EntityData == null ? "model" : entity.EntityData.ToString();
            var duplicates = Items.Where(a => a.EntityName.StartsWith(part1));
            if (!duplicates.Any()) return part1;

            var lastNum = 0;
            foreach (var duplicate in duplicates)
            {
                var afterPart2 = duplicate.EntityName.Substring(part1.Length);
                if (int.TryParse(afterPart2, out var num) && num > lastNum) lastNum = num + 1;
            }

            return part1 + lastNum;
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
            Items.Clear();
        }

        #endregion

        #region CONTAINS KEY

        public bool ContainsKey(Entity entity)
        {
            return Items.Count(a=>a.Entity.Equals(entity)) > 0;
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

        public EntityDatasItemVM this[Entity entity] => Items.Single(a=>a.Entity.Equals(entity));

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
            var allEntities = InteractiveItems.Select(a=>a.Entity).ToList();

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

        #region CTOR

        public ModelWithDataCollectionVM(IAuraViewportManager auraViewportManager)
        {
            _auraViewportManager = auraViewportManager;
            _auraViewportManager.ModelsAdded += (sender, list) =>
            {
                foreach (var entity in list) AddModelData(entity, null);
            };

            OpenModelCommand = new DelegateCommand(_auraViewportManager.OpenModel, () => true);

            RemoveModelsCommand = new DelegateCommand(() =>
            {
                if (!HasInteractiveItems) return;
                foreach (var interactiveItem in InteractiveItems)
                {
                    this.Items.Remove(interactiveItem);
                    _auraViewportManager.RemoveEntity(interactiveItem.Entity);
                }
            }, () => HasInteractiveItems);

            Items.CollectionChanged += (sender, args) =>
            {
                if (args.NewItems != null && args.NewItems.Count > 0)
                {
                    foreach (var newItem in args.NewItems)
                    {
                        var item = (EntityDatasItemVM)newItem;
                        item.EntityName = GetUniqueName(item.Entity);
                        item.PropertyChanged += ItemOnPropertyChanged;
                    }
                }

                if (args.OldItems != null && args.OldItems.Count > 0)
                {
                    foreach (var oldItem in args.OldItems)
                    {
                        var item = (EntityDatasItemVM)oldItem;
                        item.PropertyChanged -= ItemOnPropertyChanged;
                    }
                }
            };
        }

        private void ItemOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "IsSelected" && e.PropertyName != "IsEnabled") return;

            var item = sender as EntityDatasItemVM;

            switch (e.PropertyName)
            {
                case "IsSelected":
                    _auraViewportManager.SetSelection(item.Entity, item.IsSelected);
                    break;
                case "IsEnabled":
                    _auraViewportManager.SetVisibility(item.Entity, item.IsEnabled);
                    break;
            }

            InteractiveItems = GetInteractiveEntities();
            RemoveModelsCommand.RaiseCanExecuteChanged();
        }

        #endregion
    }

    #region MODEL MODES
    public enum ModelModes
    {
        NONE,
        SHIFT,
        ROTATE,
        RESIZE,
        REGIONS
    }

    #endregion
}