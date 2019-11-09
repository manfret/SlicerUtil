using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Settings.ValidValues;

namespace Settings.Memento
{
    public interface IProfileMemento : ISettingsMemento
    {
        #region PART MEMENTO

        /// <summary>
        /// Настройки детали
        /// </summary>
        ILayupRule LayupRule { get; set; }

        #endregion

        #region GLOBAL MEMENTO

        /// <summary>
        /// Общие настройки
        /// </summary>
        IGlobalSettingsMemento GlobalSettingsMemento { get; set; }

        #endregion

        #region GLOBAL VARIABLE MEMENTO

        /// <summary>
        /// Общие изменяемые настройки
        /// </summary>
        IGlobalVariableSettingsMemento GlobalVariableSettingsMemento { get; set; }

        #endregion

        #region INSET 0 P MEMENTO

        /// <summary>
        /// Настройки самых тонких внешних инсетов
        /// </summary>
        IInset0PlasticSettingsMemento Inset0PlasticSettingsMemento { get; set; }

        #endregion

        #region INSET XP MEMENTO

        /// <summary>
        /// Настройки пластиковых инсетов
        /// </summary>
        IInsetXPlasticSettingsMemento InsetXPlasticSettingsMemento { get; set; }

        #endregion

        #region FIBER SETTINGS MEMENTO

        IFiberPrintingSettingsMemento FiberPrintingSettingsMemento { get; set; }

        #endregion

        #region INSET XF MEMENTO

        /// <summary>
        /// Настройки волоконных инсетов
        /// </summary>
        IInsetXFiberSettingsMemento InsetXFiberSettingsMemento { get; set; }

        #endregion

        #region INFILL MEMENTO

        /// <summary>
        /// Настройки инфилла
        /// </summary>
        IInfillSettingsMemento InfillSettingsMemento { get; set; }

        #endregion

        #region INFILL PLASTIC SOLID MEMENTO

        /// <summary>
        /// Настройки пластиковых инфиллов
        /// </summary>
        IInfillPlasticSolidSettingsMemento InfillPlasticSolidSettingsMemento { get; set; }

        #endregion

        #region INFILL PLASTIC CELLULAR MEMENTO

        /// <summary>
        /// Настройки несплошных пластиковых инфиллов
        /// </summary>
        IInfillPlasticCellularSettingsMemento InfillPlasticCellularSettingsMemento { get; set; }

        #endregion

        #region INFILL FIBER SOLID MEMENTO

        /// <summary>
        /// Настройки сплошных волоконных инфиллов
        /// </summary>
        IInfillXFiberSolidSettingsMemento InfillXFiberSolidSettingsMemento { get; set; }

        #endregion

        #region INFILL FIBER CELLULAR MEMENTO

        /// <summary>
        /// Настройки несплошных волоконных инфиллов
        /// </summary>
        IInfillXFiberCellularSettingsMemento InfillXFiberCellularSettingsMemento { get; set; }

        #endregion

        #region BRIM MEMENTO

        /// <summary>
        /// Настройки брима
        /// </summary>
        IBrimSettingsMemento BrimSettingsMemento { get; set; }

        #endregion

        #region SKIRT MEMENTO

        /// <summary>
        /// Настройки скирта
        /// </summary>
        ISkirtSettingsMemento SkirtSettingsMemento { get; set; }

        #endregion

        #region INSET MEMENTO

        /// <summary>
        /// Настройки инсетов
        /// </summary>
        IInsetSettingsMemento InsetSettingsMemento { get; set; }

        #endregion

        #region SUPPORT MEMENTO

        /// <summary>
        /// Настройки сапорта
        /// </summary>
        ISSMemento SSMemento { get; set; }

        #endregion

        #region VERSION

        /// <summary>
        /// Версия
        /// </summary>
        int? Version { get; set; }

        int TrueVersion { get; }

        #endregion

        #region DESCRIPTION

        /// <summary>
        /// Описание
        /// </summary>
        string Description { get; set; }

        #endregion

        #region SET DEFAULT

        /// <summary>
        /// Устанавливает значения по умолчанию
        /// </summary>
        void SetDefault();

        #endregion

        #region GUID

        Guid? GUID { get; }

        #endregion

        #region PARENT GUID

        Guid? ParentGUID { get; }

        #endregion
    }

    public class ProfileMemento : IProfileMemento
    {
        #region NAME

        private string _name;

        /// <summary>
        /// Название профиля
        /// </summary>
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
            if (newVal == null || newVal.Equals(string.Empty)) newVal = SProfile.Default.Name;

            return newVal.Equals(_name);
        }

        #endregion

        #region PART MEMENTO

        private ILayupRule _layupRule;

        /// <summary>
        /// Настройки детали
        /// </summary>
        [JsonProperty]
        public ILayupRule LayupRule
        {
            get => _layupRule;
            set
            {
                _layupRule = value;
                _layupRule.PropertyChanged += (sender, args) => OnPropertyChanged();
                OnPropertyChanged();
            }
        }

        #endregion

        #region GLOBAL MEMENTO

        private IGlobalSettingsMemento _globalSettingsMemento;

        /// <summary>
        /// Общие настройки
        /// </summary>
        [JsonProperty]
        public IGlobalSettingsMemento GlobalSettingsMemento
        {
            get => _globalSettingsMemento;
            set
            {
                _globalSettingsMemento = value;
                _globalSettingsMemento.PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == "AllowGenerateFiber" && !_globalSettingsMemento.TrueAllowGenerateFiber)
                    {
                        if (LayupRule != null) LayupRule.GenerateFiberPerimeters = false;
                        if (LayupRule != null) LayupRule.GenerateFiberInfill = false;
                    }
                    OnPropertyChanged();
                };
                OnPropertyChanged();
            }
        }

        #endregion

        #region GLOBAL VARIABLE MEMENTO

        private IGlobalVariableSettingsMemento _globalVariableSettingsMemento;

        /// <summary>
        /// Общие изменяемые настройки
        /// </summary>
        [JsonProperty]
        public IGlobalVariableSettingsMemento GlobalVariableSettingsMemento
        {
            get => _globalVariableSettingsMemento;
            set
            {
                _globalVariableSettingsMemento = value;
                _globalVariableSettingsMemento.PropertyChanged += (sender, args) => OnPropertyChanged();
                OnPropertyChanged();
            }
        }

        #endregion

        #region INSET 0 MEMENTO

        private IInset0PlasticSettingsMemento _inset0PlasticSettingsMemento;

        /// <summary>
        /// Настройки самых тонких внешних инсетов
        /// </summary>
        [JsonProperty]
        public IInset0PlasticSettingsMemento Inset0PlasticSettingsMemento
        {
            get => _inset0PlasticSettingsMemento;
            set
            {
                _inset0PlasticSettingsMemento = value;
                _inset0PlasticSettingsMemento.PropertyChanged += (sender, args) => OnPropertyChanged();
                OnPropertyChanged();
            }
        }

        #endregion

        #region INSET X P MEMENTO

        private IInsetXPlasticSettingsMemento _insetXPlasticSettingsMemento;

        /// <summary>
        /// Настройки пластиковых инсетов
        /// </summary>
        [JsonProperty]
        public IInsetXPlasticSettingsMemento InsetXPlasticSettingsMemento
        {
            get => _insetXPlasticSettingsMemento;
            set
            {
                _insetXPlasticSettingsMemento = value;
                _insetXPlasticSettingsMemento.PropertyChanged += (sender, args) => OnPropertyChanged();
                OnPropertyChanged();
            }
        }

        #endregion

        #region FIBER SETTINGS MEMENTO

        private IFiberPrintingSettingsMemento _fiberPrintingSettingsMemento;

        [JsonProperty]
        public IFiberPrintingSettingsMemento FiberPrintingSettingsMemento
        {
            get => _fiberPrintingSettingsMemento;
            set
            {
                _fiberPrintingSettingsMemento = value;
                _fiberPrintingSettingsMemento.PropertyChanged += (sender, args) => OnPropertyChanged();
                OnPropertyChanged();
            }
        }

        #endregion

        #region INSET X F MEMENTO

        private IInsetXFiberSettingsMemento _insetXFiberSettingsMemento;

        /// <summary>
        /// Настройки волоконных инсетов
        /// </summary>
        [JsonProperty]
        public IInsetXFiberSettingsMemento InsetXFiberSettingsMemento
        {
            get => _insetXFiberSettingsMemento;
            set
            {
                _insetXFiberSettingsMemento = value;
                _insetXFiberSettingsMemento.PropertyChanged += (sender, args) => OnPropertyChanged();
                OnPropertyChanged();
            }
        }

        #endregion

        #region INFILL MEMENTO

        private IInfillSettingsMemento _infillSettingsMemento;

        /// <summary>
        /// Настройки инфилла
        /// </summary>
        [JsonProperty]
        public IInfillSettingsMemento InfillSettingsMemento
        {
            get => _infillSettingsMemento;
            set
            {
                _infillSettingsMemento = value;
                _infillSettingsMemento.PropertyChanged += (sender, args) => OnPropertyChanged();
                OnPropertyChanged();
            }
        }

        #endregion

        #region INFILL PLASTIC SOLID MEMENTO

        private IInfillPlasticSolidSettingsMemento _infillPlasticSolidSettingsMemento;

        /// <summary>
        /// Настройки пластиковых инфиллов
        /// </summary>
        [JsonProperty]
        public IInfillPlasticSolidSettingsMemento InfillPlasticSolidSettingsMemento
        {
            get => _infillPlasticSolidSettingsMemento;
            set
            {
                _infillPlasticSolidSettingsMemento = value;
                _infillPlasticSolidSettingsMemento.PropertyChanged += (sender, args) => OnPropertyChanged();
                OnPropertyChanged();
            }
        }

        #endregion

        #region INFILL PLASTIC CELLULAR MEMENTO

        private IInfillPlasticCellularSettingsMemento _infillPlasticCellularSettingsMemento;

        /// <summary>
        /// Настройки несплошных пластиковых инфиллов
        /// </summary>
        [JsonProperty]
        public IInfillPlasticCellularSettingsMemento InfillPlasticCellularSettingsMemento
        {
            get => _infillPlasticCellularSettingsMemento;
            set
            {
                _infillPlasticCellularSettingsMemento = value;
                _infillPlasticCellularSettingsMemento.PropertyChanged += (sender, args) => OnPropertyChanged();
                OnPropertyChanged();
            }
        }

        #endregion

        #region INFILL FIBER SOLID MEMENTO

        private IInfillXFiberSolidSettingsMemento _infillXFiberSolidSettingsMemento;

        [JsonProperty]
        public IInfillXFiberSolidSettingsMemento InfillXFiberSolidSettingsMemento
        {
            get => _infillXFiberSolidSettingsMemento;
            set
            {
                _infillXFiberSolidSettingsMemento = value;
                _infillXFiberSolidSettingsMemento.PropertyChanged += (sender, args) => OnPropertyChanged();
                OnPropertyChanged();
            }
        }

        #endregion

        #region INFILL FIBER CELLULAR MEMENTO

        private IInfillXFiberCellularSettingsMemento _infillXFiberCellularSettingsMemento;

        /// <summary>
        /// Настройки несплошных волоконных инфиллов
        /// </summary>
        [JsonProperty]
        public IInfillXFiberCellularSettingsMemento InfillXFiberCellularSettingsMemento
        {
            get => _infillXFiberCellularSettingsMemento;
            set
            {
                _infillXFiberCellularSettingsMemento = value;
                _infillXFiberCellularSettingsMemento.PropertyChanged += (sender, args) => OnPropertyChanged();
                OnPropertyChanged();
            }
        }

        #endregion

        #region BRIM MEMENTO

        private IBrimSettingsMemento _brimSettingsMemento;

        /// <summary>
        /// Настройки брима
        /// </summary>
        [JsonProperty]
        public IBrimSettingsMemento BrimSettingsMemento
        {
            get => _brimSettingsMemento;
            set
            {
                _brimSettingsMemento = value;
                _brimSettingsMemento.PropertyChanged += (sender, args) => OnPropertyChanged();
                OnPropertyChanged();
            }
        }

        #endregion

        #region SKIRT MEMENTO

        private ISkirtSettingsMemento _skirtSettingsMemento;

        /// <summary>
        /// Настройки скирта
        /// </summary>
        [JsonProperty]
        public ISkirtSettingsMemento SkirtSettingsMemento
        {
            get => _skirtSettingsMemento;
            set
            {
                _skirtSettingsMemento = value;
                _skirtSettingsMemento.PropertyChanged += (sender, args) => OnPropertyChanged();
                OnPropertyChanged();
            }
        }

        #endregion

        #region INSET MEMENTO

        private IInsetSettingsMemento _insetSettingsMemento;

        /// <summary>
        /// Настройки инсетов
        /// </summary>
        [JsonProperty]
        public IInsetSettingsMemento InsetSettingsMemento
        {
            get => _insetSettingsMemento;
            set
            {
                _insetSettingsMemento = value;
                _insetSettingsMemento.PropertyChanged += (sender, args) => OnPropertyChanged();
                OnPropertyChanged();
            }
        }

        #endregion

        #region SS MEMENTO

        private ISSMemento _ssMemento;

        /// <summary>
        /// Настройки сапорта
        /// </summary>
        [JsonProperty]
        public ISSMemento SSMemento
        {
            get => _ssMemento;
            set
            {
                _ssMemento = value;
                _ssMemento.PropertyChanged += (sender, args) => OnPropertyChanged();
                OnPropertyChanged();
            }
        }

        #endregion

        #region VERSION

        private int _version;

        [JsonIgnore]
        public int TrueVersion => _version;

        /// <summary>
        /// Версия
        /// </summary>
        [JsonProperty]
        public int? Version
        {
            get => _version;
            set
            {
                if (_version == value) return;
                _version = value ?? SProfile.Default.Version;

                OnPropertyChanged();
            }
        }

        #endregion

        #region DESCRIPTION

        private string _description;

        [ExcludeFromCodeCoverage]
        [JsonProperty]
        public string Description
        {
            get { return _description; }
            set
            {
                if (_description == value)return;
                _description = value;

                OnPropertyChanged();
            }
        }

        #endregion

        #region IS ANISOPRINT APPROVED

        private bool _isAnisoprintApproved;

        [JsonIgnore]
        public bool TrueIsAnisoprintApproved => _isAnisoprintApproved;

        /// <summary>
        /// Указывает является ли данный принтер одобренным компанией Anisoprint
        /// </summary>
        [JsonProperty]
        public bool? IsAnisoprintApproved
        {
            get => _isAnisoprintApproved;
            set
            {
                if (_isAnisoprintApproved == value) return;
                _isAnisoprintApproved = value ?? SProfile.Default.IsAnisoprintApproved;

                OnPropertyChanged();
            }
        }

        #endregion

        #region CTOR

        public ProfileMemento() { }

        [ExcludeFromCodeCoverage]
        public ProfileMemento(IProfileMemento memento)
        {
            Name = memento.Name;
            Description = memento.Description;
            IsAnisoprintApproved = memento.IsAnisoprintApproved;

            LayupRule = new LayupRule(memento.LayupRule);
            BrimSettingsMemento = new BrimSettingsMemento(memento.BrimSettingsMemento);
            SkirtSettingsMemento = new SkirtSettingsMemento(memento.SkirtSettingsMemento);
            InfillSettingsMemento = new InfillSettingsMemento(memento.InfillSettingsMemento);
            InfillPlasticSolidSettingsMemento = new InfillPlasticSolidSettingsMemento(memento.InfillPlasticSolidSettingsMemento);
            InfillPlasticCellularSettingsMemento = new InfillPlasticCellularSettingsMemento(memento.InfillPlasticCellularSettingsMemento);
            InsetSettingsMemento = new InsetSettingsMemento(memento.InsetSettingsMemento);
            Inset0PlasticSettingsMemento = new Inset0PlasticSettingsMemento(memento.Inset0PlasticSettingsMemento);
            FiberPrintingSettingsMemento = new FiberPrintingSettingsMemento(memento.FiberPrintingSettingsMemento);
            InsetXFiberSettingsMemento = new InsetXFiberSettingsMemento(memento.InsetXFiberSettingsMemento);
            InsetXPlasticSettingsMemento = new InsetXPlasticSettingsMemento(memento.InsetXPlasticSettingsMemento);
            InfillXFiberSolidSettingsMemento = new InfillXFiberSolidSettingsMemento(memento.InfillXFiberSolidSettingsMemento);
            InfillXFiberCellularSettingsMemento = new InfillXFiberCellularSettingsMemento(memento.InfillXFiberCellularSettingsMemento);
            SSMemento = new SSMemento(memento.SSMemento);
            GlobalSettingsMemento = new GlobalSettingsMemento(memento.GlobalSettingsMemento);
            GlobalVariableSettingsMemento = new GlobalVariableSettingsMemento(memento.GlobalVariableSettingsMemento);

            Version = SProfile.Default.Version;
            CreateGUID();
            FillParentGUID(memento.GUID);
        }

        #endregion

        #region ADD FROM ANOTHER

        public void FillFromAnother(ISettingsMemento other)
        {
            if (other is IProfileMemento pm)
            {
                FillFromAnother(pm);
                return;
            }
            throw new InvalidCastException();
        }

        public void FillFromAnother(IProfileMemento other)
        {
            Name = other.Name;
            Version = other.Version;
            Description = other.Description;
            IsAnisoprintApproved = other.IsAnisoprintApproved;

            LayupRule = new LayupRule(other.LayupRule);
            BrimSettingsMemento = new BrimSettingsMemento(other.BrimSettingsMemento);
            SkirtSettingsMemento = new SkirtSettingsMemento(other.SkirtSettingsMemento);
            InfillSettingsMemento = new InfillSettingsMemento(other.InfillSettingsMemento);
            InfillPlasticSolidSettingsMemento = new InfillPlasticSolidSettingsMemento(other.InfillPlasticSolidSettingsMemento);
            InfillPlasticCellularSettingsMemento = new InfillPlasticCellularSettingsMemento(other.InfillPlasticCellularSettingsMemento);
            InsetSettingsMemento = new InsetSettingsMemento(other.InsetSettingsMemento);
            Inset0PlasticSettingsMemento = new Inset0PlasticSettingsMemento(other.Inset0PlasticSettingsMemento);
            FiberPrintingSettingsMemento = new FiberPrintingSettingsMemento(other.FiberPrintingSettingsMemento);
            InsetXFiberSettingsMemento = new InsetXFiberSettingsMemento(other.InsetXFiberSettingsMemento);
            InsetXPlasticSettingsMemento = new InsetXPlasticSettingsMemento(other.InsetXPlasticSettingsMemento);
            InfillXFiberSolidSettingsMemento = new InfillXFiberSolidSettingsMemento(other.InfillXFiberSolidSettingsMemento);
            InfillXFiberCellularSettingsMemento = new InfillXFiberCellularSettingsMemento(other.InfillXFiberCellularSettingsMemento);
            SSMemento = new SSMemento(other.SSMemento);
            GlobalSettingsMemento = new GlobalSettingsMemento(other.GlobalSettingsMemento);
            GlobalVariableSettingsMemento = new GlobalVariableSettingsMemento(other.GlobalVariableSettingsMemento);
            GUID = other.GUID;
        }

        #endregion

        #region JSON CTOR

        [JsonConstructor]
        [ExcludeFromCodeCoverage]
        public ProfileMemento(string name,
            LayupRule layupRule,
            BrimSettingsMemento brimSettingsMemento,
            SkirtSettingsMemento skirtSettingsMemento,
            InfillSettingsMemento infillSettingsMemento,
            InfillPlasticSolidSettingsMemento infillPlasticSolidSettingsMemento,
            InfillPlasticCellularSettingsMemento infillPlasticCellularSettingsMemento,
            InfillXFiberSolidSettingsMemento infillXFiberSolidSettingsMemento,
            InfillXFiberCellularSettingsMemento infillXFiberCellularSettingsMemento,
            InsetSettingsMemento insetSettingsMemento,
            Inset0PlasticSettingsMemento inset0PlasticSettingsMemento,
            FiberPrintingSettingsMemento fiberPrintingSettingsMemento,
            InsetXFiberSettingsMemento insetXFiberSettingsMemento,
            InsetXPlasticSettingsMemento insetXPlasticSettingsMemento,
            SSMemento ssMemento,
            GlobalSettingsMemento globalSettingsMemento,
            GlobalVariableSettingsMemento globalVariableSettingsMemento,
            int version,
            string description,
            bool? isAnisoprintApproved,
            Guid? guid,
            Guid? parentGuid)
        {
            Name = name;

            if (layupRule != null) LayupRule = layupRule;
            else
            {
                var lr = new LayupRule();
                lr.SetDefault();
                LayupRule = lr;
            }
            if (brimSettingsMemento != null) BrimSettingsMemento = brimSettingsMemento;
            else
            {
                var brimMemento = new BrimSettingsMemento();
                brimMemento.SetDefault();
                BrimSettingsMemento = brimMemento;
            }
            if (skirtSettingsMemento != null) SkirtSettingsMemento = skirtSettingsMemento;
            else
            {
                var skirtMemento = new SkirtSettingsMemento();
                skirtMemento.SetDefault();
                SkirtSettingsMemento = skirtMemento;
            }
            if (infillSettingsMemento != null) InfillSettingsMemento = infillSettingsMemento;
            else
            {
                var infillMemento = new InfillSettingsMemento();
                infillMemento.SetDefault();
                InfillSettingsMemento = infillMemento;
            }
            if (infillPlasticSolidSettingsMemento != null) InfillPlasticSolidSettingsMemento = infillPlasticSolidSettingsMemento;
            else
            {
                var infillPlasticSolidMemento = new InfillPlasticSolidSettingsMemento();
                infillPlasticSolidMemento.SetDefault();
                InfillPlasticSolidSettingsMemento = infillPlasticSolidMemento;
            }
            if (infillPlasticCellularSettingsMemento != null) InfillPlasticCellularSettingsMemento = infillPlasticCellularSettingsMemento;
            else
            {
                var infillPlasticCellularMemento = new InfillPlasticCellularSettingsMemento();
                infillPlasticCellularMemento.SetDefault();
                InfillPlasticCellularSettingsMemento = infillPlasticCellularMemento;
            }
            if (infillXFiberSolidSettingsMemento != null) InfillXFiberSolidSettingsMemento = infillXFiberSolidSettingsMemento;
            else
            {
                var infillFiberSolidMemento = new InfillXFiberSolidSettingsMemento();
                infillFiberSolidMemento.SetDefault();
                InfillXFiberSolidSettingsMemento = infillFiberSolidMemento;
            }
            if (infillXFiberCellularSettingsMemento != null) InfillXFiberCellularSettingsMemento = infillXFiberCellularSettingsMemento;
            else
            {
                var infillFiberCellularMemento = new InfillXFiberCellularSettingsMemento();
                infillFiberCellularMemento.SetDefault();
                InfillXFiberCellularSettingsMemento = infillFiberCellularMemento;
            }
            if (insetSettingsMemento != null) InsetSettingsMemento = insetSettingsMemento;
            else
            {
                var insetMemento = new InsetSettingsMemento();
                insetMemento.SetDefault();
                InsetSettingsMemento = insetMemento;
            }
            if (inset0PlasticSettingsMemento != null) Inset0PlasticSettingsMemento = inset0PlasticSettingsMemento;
            else
            {
                var inset0Memento = new Inset0PlasticSettingsMemento();
                inset0Memento.SetDefault();
                Inset0PlasticSettingsMemento = inset0Memento;
            }
            if (fiberPrintingSettingsMemento != null) FiberPrintingSettingsMemento = fiberPrintingSettingsMemento;
            else
            {
                var defaultFiberSettingsMemento = new FiberPrintingSettingsMemento();
                defaultFiberSettingsMemento.SetDefault();
                FiberPrintingSettingsMemento = defaultFiberSettingsMemento;
            }
            if (insetXFiberSettingsMemento != null) InsetXFiberSettingsMemento = insetXFiberSettingsMemento;
            else
            {
                var insetFiberMemento = new InsetXFiberSettingsMemento();
                insetFiberMemento.SetDefault();
                InsetXFiberSettingsMemento = insetFiberMemento;
            }
            if (insetXPlasticSettingsMemento != null) InsetXPlasticSettingsMemento = insetXPlasticSettingsMemento;
            else
            {
                var insetPlasticMemento = new InsetXPlasticSettingsMemento();
                insetPlasticMemento.SetDefault();
                InsetXPlasticSettingsMemento = insetPlasticMemento;
            }
            if (ssMemento != null) SSMemento = ssMemento;
            else
            {
                var sMemento = new SSMemento();
                sMemento.SetDefault();
                SSMemento = sMemento;
            }
            if (globalSettingsMemento != null) GlobalSettingsMemento = globalSettingsMemento;
            else
            {
                var globalMemento = new GlobalSettingsMemento();
                globalMemento.SetDefault();
                GlobalSettingsMemento = globalMemento;
            }
            if (globalVariableSettingsMemento != null) GlobalVariableSettingsMemento = globalVariableSettingsMemento;
            else
            {
                var globalVariableMemento = new GlobalVariableSettingsMemento();
                globalVariableMemento.SetDefault();
                GlobalVariableSettingsMemento = globalVariableMemento;
            }

            Version = version;
            Description = description;
            IsAnisoprintApproved = isAnisoprintApproved;

            GUID = guid;
            FillParentGUID(parentGuid);
        }

        #endregion

        #region SET DEFAULT

        public void SetDefault()
        {
            Name = SProfile.Default.Name;
            Version = SProfile.Default.Version;
            Description = string.Empty;
            IsAnisoprintApproved = SProfile.Default.IsAnisoprintApproved;

            var partMemento = new LayupRule();
            partMemento.SetDefault();
            LayupRule = partMemento;

            var brimMemento = new BrimSettingsMemento();
            brimMemento.SetDefault();
            BrimSettingsMemento = brimMemento;

            var skirtMemento = new SkirtSettingsMemento();
            skirtMemento.SetDefault();
            SkirtSettingsMemento = skirtMemento;

            var infillSettingsMemento = new InfillSettingsMemento();
            infillSettingsMemento.SetDefault();
            InfillSettingsMemento = infillSettingsMemento;

            var infillPSolidMemento = new InfillPlasticSolidSettingsMemento();
            infillPSolidMemento.SetDefault();
            InfillPlasticSolidSettingsMemento = infillPSolidMemento;

            var infillPCellularMemento = new InfillPlasticCellularSettingsMemento();
            infillPCellularMemento.SetDefault();
            InfillPlasticCellularSettingsMemento = infillPCellularMemento;

            var insetMemento = new InsetSettingsMemento();
            insetMemento.SetDefault();
            InsetSettingsMemento = insetMemento;

            var inset0Memento = new Inset0PlasticSettingsMemento();
            inset0Memento.SetDefault();
            Inset0PlasticSettingsMemento = inset0Memento;

            var fiberSettingsMemento = new FiberPrintingSettingsMemento();
            fiberSettingsMemento.SetDefault();
            FiberPrintingSettingsMemento = fiberSettingsMemento;

            var insetXFMemento = new InsetXFiberSettingsMemento();
            insetXFMemento.SetDefault();
            InsetXFiberSettingsMemento = insetXFMemento;

            var fiberInfillSolid = new InfillXFiberSolidSettingsMemento();
            fiberInfillSolid.SetDefault();
            InfillXFiberSolidSettingsMemento = fiberInfillSolid;

            var fiberInfillCellular = new InfillXFiberCellularSettingsMemento();
            fiberInfillCellular.SetDefault();
            InfillXFiberCellularSettingsMemento = fiberInfillCellular;

            var insetXPMemento = new InsetXPlasticSettingsMemento();
            insetXPMemento.SetDefault();
            InsetXPlasticSettingsMemento = insetXPMemento;

            var ssMemento = new SSMemento();
            ssMemento.SetDefault();
            SSMemento = ssMemento;

            var globalMemento = new GlobalSettingsMemento();
            globalMemento.SetDefault();
            GlobalSettingsMemento = globalMemento;

            var globalVariablememento = new GlobalVariableSettingsMemento();
            globalVariablememento.SetDefault();
            GlobalVariableSettingsMemento = globalVariablememento;

            Version = SProfile.Default.Version;

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

        #region ON PROPERTY CHANGING

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangingEventHandler PropertyChanging;

        [ExcludeFromCodeCoverage]
        private void OnPropertyChanging([CallerMemberName] string caller = null)
        {
            var handler = PropertyChanging;
            handler?.Invoke(this, new PropertyChangingEventArgs(caller));
        }

        #endregion

        #region EQUALS

        [ExcludeFromCodeCoverage]
        protected bool Equals(ProfileMemento other)
        {
            return string.Equals(_name, other._name) 
                   && Equals(_globalSettingsMemento, other._globalSettingsMemento) 
                   && Equals(_inset0PlasticSettingsMemento, other._inset0PlasticSettingsMemento) 
                   && Equals(_insetXPlasticSettingsMemento, other._insetXPlasticSettingsMemento) 
                   && Equals(_fiberPrintingSettingsMemento, other.FiberPrintingSettingsMemento)
                   && Equals(_insetXFiberSettingsMemento, other._insetXFiberSettingsMemento) 
                   && Equals(_infillSettingsMemento, other._infillSettingsMemento) 
                   && Equals(_infillPlasticSolidSettingsMemento, other._infillPlasticSolidSettingsMemento) 
                   && Equals(_infillPlasticCellularSettingsMemento, other._infillPlasticCellularSettingsMemento) 
                   && Equals(_brimSettingsMemento, other._brimSettingsMemento) 
                   && Equals(_skirtSettingsMemento, other._skirtSettingsMemento)
                   && Equals(_insetSettingsMemento, other._insetSettingsMemento) 
                   && Equals(_ssMemento, other._ssMemento) 
                   && _version == other._version 
                   && _isAnisoprintApproved == other._isAnisoprintApproved;
        }

        [ExcludeFromCodeCoverage]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ProfileMemento) obj);
        }

        public bool EqualsWithoutNameVersionApproved(IProfileMemento other)
        {
            return Equals(_globalSettingsMemento, other.GlobalSettingsMemento)
                   && Equals(_inset0PlasticSettingsMemento, other.Inset0PlasticSettingsMemento)
                   && Equals(_insetXPlasticSettingsMemento, other.InsetXPlasticSettingsMemento)
                   && Equals(_fiberPrintingSettingsMemento, other.FiberPrintingSettingsMemento)
                   && Equals(_insetXFiberSettingsMemento, other.InsetXFiberSettingsMemento)
                   && Equals(_infillSettingsMemento, other.InfillSettingsMemento)
                   && Equals(_infillPlasticSolidSettingsMemento, other.InfillPlasticSolidSettingsMemento)
                   && Equals(_infillPlasticCellularSettingsMemento, other.InfillPlasticCellularSettingsMemento)
                   && Equals(_brimSettingsMemento, other.BrimSettingsMemento)
                   && Equals(_skirtSettingsMemento, other.SkirtSettingsMemento)
                   && Equals(_insetSettingsMemento, other.InsetSettingsMemento)
                   && Equals(_ssMemento, other.SSMemento);
        }

        public bool EqualsWithoutNameVersionApproved(ISettingsMemento other)
        {
            if (other is IProfileMemento pm) return EqualsWithoutNameVersionApproved(pm);
            throw new InvalidCastException();
        }

        #endregion

        #region GUID

        private Guid _guid;

        [ExcludeFromCodeCoverage]
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

        [ExcludeFromCodeCoverage]
        private bool GetTrueGUID(Guid? value, out Guid newVal)
        {
            newVal = value ?? Guid.NewGuid();
            return newVal.Equals(_guid);
        }

        [ExcludeFromCodeCoverage]
        private void CreateGUID()
        {
            GUID = Guid.NewGuid();
        }

        #endregion

        #region PARENT GUID

        private Guid _parentGuid;

        [ExcludeFromCodeCoverage]
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

        [ExcludeFromCodeCoverage]
        private bool GetTrueParentGUID(Guid? value, out Guid newVal)
        {
            newVal = value ?? Guid.NewGuid();
            return newVal.Equals(_parentGuid);
        }

        [ExcludeFromCodeCoverage]
        private void FillParentGUID(Guid? guid)
        {
            ParentGUID = guid;
        }

        #endregion
    }
}