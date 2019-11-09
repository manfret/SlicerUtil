using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media.Media3D;
using Aura.CORE.AuraFile;
using devDept.Eyeshot.Entities;

namespace Aura.ViewModels
{
    public sealed class EntityDatasItemVM : INotifyPropertyChanged
    {
        #region ENTITY

        private Entity _entity;
        public Entity Entity
        {
            get => _entity;
            set
            {
                _entity = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region ENTITY NAME

        private string _entityName;
        public string EntityName
        {
            get => _entityName;
            set
            {
                _entityName = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region IS ENABLED

        private bool _isEnabled;

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                OnPropertyChanged();
                if (!_isEnabled) IsSelected = false;
            }
        }

        #endregion

        #region IS SELECTED

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region DATAS

        private bool _hasData;
        public bool HasData
        {
            get => _hasData;
            set
            {
                _hasData = value;
                OnPropertyChanged();
            }
        }

        private bool _isDataEnabled;
        public bool IsDataEnabled
        {
            get => _isDataEnabled;
            set
            {
                _isDataEnabled = value;
                OnPropertyChanged();
            }
        }

        private List<LayupDataLayerIndex> _datas;
        public List<LayupDataLayerIndex> Datas
        {
            get => _datas;
            set
            {
                _datas = value;
                HasData = _datas != null;
                OnPropertyChanged();
            }
        }

        #endregion

        #region ROTATION DATA

        public Point3D RotationData { get; private set; }

        public void UpdateRotationData(double x, double y, double z)
        {
            RotationData = new Point3D(x, y, z);
        }

        #endregion

        #region SCALE DATA

        public Point3D ScaleData { get; private set; }

        public void UpdateScaleData(double x, double y, double z)
        {
            ScaleData = new Point3D(
                Math.Round(x, 2), 
                Math.Round(y, 2), 
                Math.Round(z, 2));
        }

        #endregion

        #region CTOR

        public EntityDatasItemVM(Entity entity, List<LayupDataLayerIndex> datas, string entityName, bool isEnabled, bool isSelected, bool isDataEnabled)
        {
            Entity = entity;
            EntityName = entityName;
            IsEnabled = isEnabled;
            Datas = datas;
            IsSelected = isSelected;
            IsDataEnabled = isDataEnabled;
            RotationData = new Point3D(0, 0 ,0);
            ScaleData = new Point3D(100, 100, 100);
        }

        #endregion

        public List<LayupDataLayerIndex> UpdateLayupDatas(List<LayupDataLayerIndex> datas)
        {
            if (datas == null || datas.Count == 0) return null;

            var entityBoxMinZ = Entity.BoxMin.Z;
            var entityBoxMaxZ = Entity.BoxMax.Z;
            var res = new List<LayupDataLayerIndex>();

            foreach (var layupDataLayerIndex in datas)
            {
                var minRegion = layupDataLayerIndex.RegionData.StartHeightMM;
                var maxRegion = layupDataLayerIndex.RegionData.EndHeightMM;

                if (minRegion > entityBoxMaxZ || maxRegion < entityBoxMinZ) continue;

                var updatingData = layupDataLayerIndex.Clone();
                var startMM = updatingData.RegionData.StartHeightMM < entityBoxMinZ ? entityBoxMinZ : updatingData.RegionData.StartHeightMM;
                var endMM = updatingData.RegionData.EndHeightMM > entityBoxMaxZ ? entityBoxMaxZ : updatingData.RegionData.EndHeightMM;

                updatingData.RegionData.StartHeightMM = startMM;
                updatingData.RegionData.EndHeightMM = endMM;

                res.Add(updatingData);
            }
            return res;
        }

        public void UpdateSelfLayupDatas()
        {
            if (Datas == null || Datas.Count == 0) return;

            Datas = UpdateLayupDatas(Datas);

            var maxPriority = Datas.Max(a => a.RegionData.Priority);
            var defaultItem = Datas.Single(a => a.RegionData.Priority == maxPriority);
            defaultItem.RegionData.StartHeightMM = Entity.BoxMin.Z;
            defaultItem.RegionData.EndHeightMM = Entity.BoxMax.Z;
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