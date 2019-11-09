using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Settings.ValidValues;

namespace Settings.Memento
{
    public interface IMaterialFMemento : ISettingsMemento
    {
        #region FIBER TYPE

        /// <summary>
        /// Тип волокна
        /// </summary>
        string FiberType { get; set; }

        #endregion

        #region FIBER DIAMETER

        /// <summary>
        /// Диаметр композитного волокна (мм)
        /// </summary>
        double? FiberDiameter { get; set; }
        double TrueFiberDiameter { get; }

        #endregion

        #region EXTRA SPEED F

        /// <summary>
        /// Скорость экструзии (для случая без движения) (мм/сек)
        /// </summary>
        int? ExtraSpeedF { get; set; }
        int TrueExtraSpeedF { get; }

        #endregion

        #region Z HOP F

        /// <summary>
        /// Величина поднятия экструдера в случае печати пластиком с волокном
        /// </summary>
        double? ZHopF { get; set; }
        double TrueZHopF { get; }

        #endregion

        #region Z HOP F PAUSE ADHESION

        /// <summary>
        /// Пауза после подъема-опускания экструдера для лучшего приклеивания волокна
        /// </summary>
        double? ZHopFPauseAdhesion { get; set; }
        double TrueZHopFPauseAdhesion { get; }

        #endregion

        #region END POLYGON EMPTY DISTANCE

        /// <summary>
        /// Дистанция пустого перемещения в конце полигона
        /// </summary>
        double? EndPolygonEmptyDistanceMM { get; set; }
        double TrueEndPolygonEmptyDistanceMM { get; }

        #endregion

        #region DO PLASTIC RETRACT

        /// <summary>
        /// True, если нужно ретрактить пластик в композитном материале
        /// </summary>
        bool? DoPlasticRetract { get; set; }
        bool TrueDoPlasticRetract { get; }

        #endregion

        #region FAN SPEED

        int? FanSpeed { get; set; }
        int TrueFanSpeed { get; }

        #endregion

        #region LINEAR DENSITY

        /// <summary>
        /// Линейная плотность (текс = грамм/километр)
        /// </summary>
        double? LinearDensity { get; set; }
        double TrueLinearDensity { get; }

        #endregion

        #region COST PER SPOOL

        /// <summary>
        /// Цена за катушку
        /// </summary>
        double? CostPerSpool { get; set; }
        double TrueCostPerSpool { get; }

        #endregion

        #region LEGTH PER SPOOL

        /// <summary>
        /// Длина волокна в катушке (км)
        /// </summary>
        double? LengthPerSpool { get; set; }
        double TrueLengthPerSpool { get; }

        #endregion

        #region DESCRIPTION

        /// <summary>
        /// Описание материала
        /// </summary>
        string Description { get; set; }

        #endregion

        #region VERSION

        int? Version { get; set; }
        int TrueVersion { get; }

        #endregion

        #region SET DEFAULT

        void SetDefault();

        #endregion

        #region FILL FROM ANOTHER

        void FillFromAnother(IMaterialFMemento other);

        #endregion

        #region GUID

        Guid? GUID { get; }

        #endregion

        #region PARENT GUID

        Guid? ParentGUID { get; }

        #endregion
    }

    /// <summary>
    /// Хранитель настроек о волоконном материале
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class MaterialFMemento : IMaterialFMemento
    {
        #region NAME

        private string _name;

        [JsonProperty]
        public string Name
        {
            get => _name;
            set
            {
                if (GetTrueName(value, out var newVal)) return;
                OnPropertyChanging();
                _name = newVal;

                OnPropertyChanged();
            }
        }

        private bool GetTrueName(string value, out string newVal)
        {
            newVal = value;
            if (newVal == null || newVal.Equals(string.Empty)) newVal = SMaterialF.Default.Name;

            return newVal.Equals(_name);
        }

        #endregion

        #region FIBER TYPE

        private string _fiberType;

        /// <summary>
        /// Тип волокна
        /// </summary>
        [JsonProperty]
        public string FiberType
        {
            get => _fiberType;
            set
            {
                if (GetTrueFiberType(value, out var newVal)) return;
                _fiberType = newVal;

                OnPropertyChanged();
            }
        }

        private bool GetTrueFiberType(string value, out string newVal)
        {
            newVal = value;
            if (newVal == null || newVal.Equals(string.Empty)) newVal = SMaterialF.Default.FiberType;

            return newVal.Equals(_fiberType);
        }

        #endregion

        #region FIBER DIAMETER

        public double TrueFiberDiameter { get; private set; }

        /// <summary>
        /// Диаметр композитного волокна (мм)
        /// </summary>
        [JsonProperty]
        public double? FiberDiameter
        {
            get => TrueFiberDiameter;
            set
            {
                if (GetTrueFiberDiameter(value, out var newVal)) return;
                TrueFiberDiameter = newVal;

                OnPropertyChanged();
            }
        }

        private bool GetTrueFiberDiameter(double? value, out double newVal)
        {
            newVal = value ?? SMaterialF.Default.FiberDiameter;

            if (newVal < SMaterialF.Default.FiberDiameterMin || newVal > SMaterialF.Default.FiberDiameterMax)
                newVal = SMaterialF.Default.FiberDiameter;

            return Math.Abs(TrueFiberDiameter - newVal) < 0.0001;
        }

        #endregion

        #region EXTRA SPEED F

        public int TrueExtraSpeedF { get; private set; }

        /// <summary>
        /// Скорость экструзии (для случая без движения) (мм/сек)
        /// </summary>
        [JsonProperty]
        public int? ExtraSpeedF
        {
            get => TrueExtraSpeedF;
            set
            {
                if (GetTrueExtraSpeedF(value, out var newVal)) return;
                TrueExtraSpeedF = newVal;

                OnPropertyChanged();
            }
        }

        private bool GetTrueExtraSpeedF(int? value, out int newVal)
        {
            newVal = value ?? SMaterialF.Default.ExtraSpeedF;

            if (newVal < SMaterialF.Default.ExtraSpeedFMin || newVal > SMaterialF.Default.ExtraSpeedFMax)
                newVal = SMaterialF.Default.ExtraSpeedF;

            return TrueExtraSpeedF == newVal;
        }

        #endregion

        #region Z HOP F
        public double TrueZHopF { get; private set; }

        /// <summary>
        /// Величина поднятия экструдера в случае печати пластиком с волокном
        /// </summary>
        [JsonProperty]
        public double? ZHopF
        {
            get => TrueZHopF;
            set
            {
                if (GetTrueZHopF(value, out var newValue)) return;
                TrueZHopF = newValue;

                OnPropertyChanged();
            }
        }

        private bool GetTrueZHopF(double? value, out double newVal)
        {
            newVal = value ?? SMaterialF.Default.ZHopF;

            if (newVal < SMaterialF.Default.ZHopFMin || newVal > SMaterialF.Default.ZHopFMax)
                newVal = SMaterialF.Default.ZHopF;

            return Math.Abs(TrueZHopF - newVal) < 0.0001;
        }

        #endregion

        #region Z HOP F PAUSE ADHESION

        public double TrueZHopFPauseAdhesion { get; private set; }

        /// <summary>
        /// Пауза после подъема-опускания экструдера для лучшего приклеивания волокна
        /// </summary>
        [JsonProperty]
        public double? ZHopFPauseAdhesion
        {
            get => TrueZHopFPauseAdhesion;
            set
            {
                if (GetTrueZHopFPauseAdhesion(value, out var newVal)) return;
                TrueZHopFPauseAdhesion = newVal;

                OnPropertyChanged();
            }
        }

        private bool GetTrueZHopFPauseAdhesion(double? value, out double newVal)
        {
            newVal = value ?? SMaterialF.Default.ZHopFPauseAdhesion;

            if (newVal < SMaterialF.Default.ZHopFPauseAdhesionMin || newVal > SMaterialF.Default.ZHopFPauseAdhesionMax)
                newVal = SMaterialF.Default.ZHopFPauseAdhesion;

            return Math.Abs(TrueZHopFPauseAdhesion - newVal) < 0.0001;
        }

        #endregion

        #region END POLYGON EMPTY DISTANCE

        public double TrueEndPolygonEmptyDistanceMM { get; private set; }

        /// <summary>
        /// Дистанция пустого перемещения в конце полигона
        /// </summary>
        [JsonProperty]
        public double? EndPolygonEmptyDistanceMM
        {
            get => TrueEndPolygonEmptyDistanceMM;
            set
            {
                if (GetTrueEndPolygonEmptyDistanceMM(value, out var newVal)) return;
                TrueEndPolygonEmptyDistanceMM = newVal;

                OnPropertyChanged();
            }
        }

        private bool GetTrueEndPolygonEmptyDistanceMM(double? value, out double newVal)
        {
            newVal = value ?? SMaterialF.Default.EndPolygonEmptyDistanceMM;

            if (newVal < SMaterialF.Default.EndPolygonEmptyDistanceMMMin || newVal > SMaterialF.Default.EndPolygonEmptyDistanceMMMax)
                newVal = SMaterialF.Default.EndPolygonEmptyDistanceMM;

            return Math.Abs(TrueEndPolygonEmptyDistanceMM - newVal) < 0.0001;
        }

        #endregion

        #region DO PLASTIC RETRACT
        public bool TrueDoPlasticRetract { get; private set; }

        [JsonProperty]
        public bool? DoPlasticRetract
        {
            get => TrueDoPlasticRetract;
            set
            {
                if (GetTruePlasticRetract(value, out var newVal)) return;
                TrueDoPlasticRetract = newVal;

                OnPropertyChanged();
            }
        }

        private bool GetTruePlasticRetract(bool? value, out bool newVal)
        {
            newVal = value ?? SMaterialF.Default.DoPlasticRetract;

            return TrueDoPlasticRetract == newVal;
        }

        #endregion

        #region FAN SPEED
        public int TrueFanSpeed { get; private set; }

        [JsonProperty]
        public int? FanSpeed
        {
            get => TrueFanSpeed;
            set
            {
                if (GetTrueFanSpeed(value, out var newVal)) return;
                TrueFanSpeed = newVal;

                OnPropertyChanged();
            }
        }

        private bool GetTrueFanSpeed(int? value, out int newVal)
        {
            newVal = value ?? SMaterialF.Default.FanSpeed;

            if (newVal < SMaterialF.Default.FanSpeedMin || newVal > SMaterialF.Default.FanSpeedMax)
                newVal = SMaterialF.Default.FanSpeed;

            return TrueFanSpeed == newVal;
        }

        #endregion

        #region LINEAR DENSITY
        public double TrueLinearDensity { get; private set; }

        /// <summary>
        /// Линейная плотность (текс = грамм/километр)
        /// </summary>
        [JsonProperty]
        public double? LinearDensity
        {
            get => TrueLinearDensity;
            set
            {
                if (GetTrueLinearDensity(value, out var newVal)) return;
                TrueLinearDensity = newVal;

                OnPropertyChanged();
            }
        }

        private bool GetTrueLinearDensity(double? value, out double newVal)
        {
            newVal = value ?? SMaterialF.Default.LinearDensity;

            if (newVal < SMaterialF.Default.LinearDensityMin || newVal > SMaterialF.Default.LinearDensityMax)
                newVal = SMaterialF.Default.LinearDensity;

            return Math.Abs(TrueLinearDensity - newVal) < 0.0001;
        }

        #endregion

        #region COST PER SPOOL

        public double TrueCostPerSpool { get; private set; }

        /// <summary>
        /// Цена за катушку
        /// </summary>
        [JsonProperty]
        public double? CostPerSpool
        {
            get => TrueCostPerSpool;
            set
            {
                if (GetTrueCostPerSpool(value, out var newVal)) return;
                TrueCostPerSpool = newVal;

                OnPropertyChanged();
            }
        }

        private bool GetTrueCostPerSpool(double? value, out double newVal)
        {
            newVal = value ?? SMaterialF.Default.CostPerSpool;

            if (newVal < SMaterialF.Default.CostPerSpoolMin || newVal > SMaterialF.Default.CostPerSpoolMax)
                newVal = SMaterialF.Default.CostPerSpool;

            return Math.Abs(TrueCostPerSpool - newVal) < 0.0001;
        }

        #endregion

        #region LENGTH PER SPOOL

        public double TrueLengthPerSpool { get; private set; }

        /// <summary>
        /// Длина волокна в катушке
        /// </summary>
        [JsonProperty]
        public double? LengthPerSpool
        {
            get => TrueLengthPerSpool;
            set
            {
                if (GetTrueLengthPerSpool(value, out var newVal)) return;
                TrueLengthPerSpool = newVal;

                OnPropertyChanged();
            }
        }

        private bool GetTrueLengthPerSpool(double? value, out double newVal)
        {
            newVal = value ?? SMaterialF.Default.LengthPerSpool;

            if (newVal < SMaterialF.Default.LengthPerSpoolMin || newVal > SMaterialF.Default.LengthPerSpoolMax)
                newVal = SMaterialF.Default.LengthPerSpool;

            return Math.Abs(TrueLengthPerSpool - newVal) < 0.0001;
        }

        #endregion

        #region DESCRIPTION

        private string _description;

        /// <summary>
        /// Описание материала
        /// </summary>
        [JsonProperty]
        public string Description
        {
            get => _description;
            set
            {
                if (_description == value) return;
                _description = value;

                OnPropertyChanged();
            }
        }

        #endregion

        #region VERSION

        public int TrueVersion { get; private set; }

        [JsonProperty]
        public int? Version
        {
            get => TrueVersion;
            set
            {
                if (TrueVersion == value) return;
                TrueVersion = value ?? SMaterialF.Default.Version;

                OnPropertyChanged();
            }
        }

        #endregion

        #region IS ANISOPRINT APPROVED

        private bool _isAnisoprintApproved;
        public bool TrueIsAnisoprintApproved => _isAnisoprintApproved;

        /// <summary>
        /// Поле, указывающее одобрен ли материал компанией Anisoprint
        /// </summary>
        [JsonProperty]
        public bool? IsAnisoprintApproved
        {
            get => _isAnisoprintApproved;
            set
            {
                if (value == _isAnisoprintApproved) return;
                _isAnisoprintApproved = value ?? SMaterialF.Default.IsAnisoprintApproved;
                OnPropertyChanged();
            }
        }
        #endregion

        #region CTOR

        public MaterialFMemento() { }

        [ExcludeFromCodeCoverage]
        public MaterialFMemento(IMaterialFMemento memento)
        {
            Name = memento.Name;
            FiberType = memento.FiberType;
            FiberDiameter = memento.FiberDiameter;
            ExtraSpeedF = memento.ExtraSpeedF;
            ZHopF = memento.ZHopF;
            ZHopFPauseAdhesion = memento.ZHopFPauseAdhesion;
            EndPolygonEmptyDistanceMM = memento.EndPolygonEmptyDistanceMM;
            DoPlasticRetract = memento.DoPlasticRetract;
            FanSpeed = memento.FanSpeed;
            LinearDensity = memento.LinearDensity;
            CostPerSpool = memento.CostPerSpool;
            LengthPerSpool = memento.LengthPerSpool;
            Description = memento.Description;
            IsAnisoprintApproved = memento.IsAnisoprintApproved;
            Version = SMaterialF.Default.Version;
            CreateGUID();
            FillParentGUID(memento.GUID);
        }

        [ExcludeFromCodeCoverage]
        [JsonConstructor]
        public MaterialFMemento(
            string name, 
            string fiberType, 
            double? fiberDiameter,
            
            int? extraSpeedF,
            double? zHopF,
            double? zHopFPauseAdhesion,
            double? endPolygonEmptyDistanceMM,
            bool? doPlasticRetract,
            int? fanSpeed,
            double? linearDensity, 
            double? costPerSpool, 
            double? lengthPerSpool, 
            string description, 
            int? version, 
            bool? isAnisoprintApproved,
            Guid? guid,
            Guid? parentGuid)
        {
            Name = name;
            FiberType = fiberType;
            FiberDiameter = fiberDiameter;
            ExtraSpeedF = extraSpeedF;
            ZHopF = zHopF;
            ZHopFPauseAdhesion = zHopFPauseAdhesion;
            EndPolygonEmptyDistanceMM = endPolygonEmptyDistanceMM;
            DoPlasticRetract = doPlasticRetract;
            FanSpeed = fanSpeed;
            LinearDensity = linearDensity;
            CostPerSpool = costPerSpool;
            LengthPerSpool = lengthPerSpool;
            Description = description;
            Version = version;
            IsAnisoprintApproved = isAnisoprintApproved;
            GUID = guid;
            FillParentGUID(parentGuid);
        }

        #endregion

        #region FILL FROM ANOTHER

        public void FillFromAnother(ISettingsMemento other)
        {
            if (other is IMaterialFMemento mfm)
            {
                FillFromAnother(mfm);
                return;
            }
            throw new InvalidCastException();
        }

        public void FillFromAnother(IMaterialFMemento other)
        {
            Name = other.Name;
            FiberType = other.FiberType;
            FiberDiameter = other.FiberDiameter;
            ExtraSpeedF = other.ExtraSpeedF;
            ZHopF = other.ZHopF;
            ZHopFPauseAdhesion = other.ZHopFPauseAdhesion;
            EndPolygonEmptyDistanceMM = other.EndPolygonEmptyDistanceMM;
            DoPlasticRetract = other.DoPlasticRetract;
            FanSpeed = other.FanSpeed;
            LinearDensity = other.LinearDensity;
            CostPerSpool = other.CostPerSpool;
            LengthPerSpool = other.LengthPerSpool;
            Description = other.Description;
            Version = other.Version;
            IsAnisoprintApproved = other.IsAnisoprintApproved;
            GUID = other.GUID;
            FillParentGUID(other.ParentGUID);
        }

        #endregion

        #region SET DEFAULT

        [ExcludeFromCodeCoverage]
        public void SetDefault()
        {
            Name = SMaterialF.Default.Name;
            FiberType = SMaterialF.Default.FiberType;
            FiberDiameter = SMaterialF.Default.FiberDiameter;
            ExtraSpeedF = SMaterialF.Default.ExtraSpeedF;
            ZHopF = SMaterialF.Default.ZHopF;
            ZHopFPauseAdhesion = SMaterialF.Default.ZHopFPauseAdhesion;
            EndPolygonEmptyDistanceMM = SMaterialF.Default.EndPolygonEmptyDistanceMM;
            DoPlasticRetract = SMaterialF.Default.DoPlasticRetract;
            FanSpeed = SMaterialF.Default.FanSpeed;
            LinearDensity = SMaterialF.Default.LinearDensity;
            CostPerSpool = SMaterialF.Default.CostPerSpool;
            LengthPerSpool = SMaterialF.Default.LengthPerSpool;
            Version = SMaterialF.Default.Version;

            CreateGUID();
        }

        #endregion

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

        #region ON PROPERTY CHANGED

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangingEventHandler PropertyChanging;

        private void OnPropertyChanging([CallerMemberName] string caller = null)
        {
            var handler = PropertyChanging;
            handler?.Invoke(this, new PropertyChangingEventArgs(caller));
        }

        #endregion

        #region EQUALS

        [ExcludeFromCodeCoverage]
        protected bool Equals(MaterialFMemento other)
        {
            return string.Equals(_name, other._name) && 
                   string.Equals(_fiberType, other._fiberType) && 
                   TrueFiberDiameter.Equals(other.TrueFiberDiameter) && 
                   TrueExtraSpeedF == other.TrueExtraSpeedF &&
                   TrueZHopF.Equals(other.TrueZHopF) &&
                   TrueZHopFPauseAdhesion.Equals(other.TrueZHopFPauseAdhesion) &&
                   TrueEndPolygonEmptyDistanceMM.Equals(other.TrueEndPolygonEmptyDistanceMM) &&
                   TrueDoPlasticRetract == other.TrueDoPlasticRetract &&
                   TrueFanSpeed == other.TrueFanSpeed &&
                   TrueLinearDensity.Equals(other.TrueLinearDensity) && 
                   TrueCostPerSpool.Equals(other.TrueCostPerSpool) && 
                   TrueLengthPerSpool.Equals(other.TrueLengthPerSpool) &&
                   TrueVersion == other.TrueVersion &&
                   _isAnisoprintApproved == other._isAnisoprintApproved;
        }

        [ExcludeFromCodeCoverage]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MaterialFMemento) obj);
        }

        public bool EqualsWithoutNameVersionApproved(ISettingsMemento other)
        {
            if (other is IMaterialFMemento mfm) return EqualsWithoutNameVersionApproved(mfm);
            throw new InvalidCastException();
        }

        public bool EqualsWithoutNameVersionApproved(IMaterialFMemento other)
        {
            return string.Equals(_fiberType, other.FiberType) &&
                   TrueFiberDiameter.Equals(other.TrueFiberDiameter) &&
                   TrueExtraSpeedF == other.TrueExtraSpeedF &&
                   TrueZHopF.Equals(other.TrueZHopF) &&
                   TrueZHopFPauseAdhesion.Equals(other.TrueZHopFPauseAdhesion) &&
                   TrueEndPolygonEmptyDistanceMM.Equals(other.TrueEndPolygonEmptyDistanceMM) &&
                   TrueDoPlasticRetract == other.TrueDoPlasticRetract &&
                   TrueFanSpeed == other.TrueFanSpeed &&
                   TrueLinearDensity.Equals(other.TrueLinearDensity) &&
                   TrueCostPerSpool.Equals(other.TrueCostPerSpool) &&
                   TrueLengthPerSpool.Equals(other.TrueLengthPerSpool);
        }

        [ExcludeFromCodeCoverage]
        private sealed class MaterialFMementoEqualityComparer : IEqualityComparer<MaterialFMemento>
        {
            public bool Equals(MaterialFMemento x, MaterialFMemento y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return string.Equals(x.FiberType, y.FiberType) 
                    && x.FiberDiameter.Equals(y.FiberDiameter) 
                    && x.Name.Equals(y.Name)
                    && x.ExtraSpeedF == y.ExtraSpeedF
                    && Math.Abs(x.TrueZHopF - y.TrueZHopF) < 0.0001
                    && Math.Abs(x.TrueZHopFPauseAdhesion - y.TrueZHopFPauseAdhesion) < 0.0001   
                    && x.LinearDensity.Equals(y.LinearDensity) 
                    && x.CostPerSpool.Equals(y.CostPerSpool) 
                    && x.LengthPerSpool.Equals(y.LengthPerSpool) 
                    && x.Version == y.Version
                    && x.IsAnisoprintApproved == y.IsAnisoprintApproved;
            }

            public int GetHashCode(MaterialFMemento obj)
            {
                unchecked
                {
                    var hashCode = (obj.FiberType != null ? obj.FiberType.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ obj.Name.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.FiberDiameter.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.ExtraSpeedF.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.ZHopF.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.ZHopFPauseAdhesion.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.LinearDensity.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.CostPerSpool.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.LengthPerSpool.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.Version.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.IsAnisoprintApproved.GetHashCode();
                    return hashCode;
                }
            }
        }

        public static IEqualityComparer<MaterialFMemento> MaterialFMementoComparer { get; } = new MaterialFMementoEqualityComparer();

        #endregion

        #region GUID

        private Guid _guid;

        [JsonProperty]
        public Guid? GUID
        {
            get => _guid;
            set
            {
                if (GetTrueGUID(value, out var newVal)) return;
                _guid = newVal;
            }
        }

        private bool GetTrueGUID(Guid? value, out Guid newVal)
        {
            newVal = value ?? Guid.NewGuid();
            return newVal.Equals(_guid);
        }

        public void CreateGUID()
        {
            GUID = Guid.NewGuid();
        }

        #endregion

        #region PARENT GUID

        private Guid _parentGuid;

        [JsonProperty]
        public Guid? ParentGUID
        {
            get => _parentGuid;
            set
            {
                if (GetTrueParentGUID(value, out var newVal)) return;
                _parentGuid = newVal;
            }
        }

        private bool GetTrueParentGUID(Guid? value, out Guid newVal)
        {
            newVal = value ?? Guid.NewGuid();
            return newVal.Equals(_parentGuid);
        }

        [ExcludeFromCodeCoverage]
        public void FillParentGUID(Guid? guid)
        {
            ParentGUID = guid;
        }

        #endregion
    }
}