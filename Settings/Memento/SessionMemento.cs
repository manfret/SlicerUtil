using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Media.Media3D;
using Newtonsoft.Json;
using Settings.ValidValues;

namespace Settings.Memento
{
    public interface ISessionMemento : INotifyPropertyChanged, INotifyPropertyChanging
    {
        #region NAME

        string Name { get; set; }

        #endregion

        #region DESCRIPTION

        /// <summary>
        /// Описание сессии
        /// </summary>
        string Descriprion { get; set; }

        #endregion

        #region LAST PRINTING DATE

        /// <summary>
        /// Дата последней печати
        /// </summary>
        DateTime LastPrintingDate { get; set; }

        #endregion

        #region PRINTER

        /// <summary>
        /// Принтер сессии
        /// </summary>
        IPrinterMemento Printer { get; set; }

        #endregion

        #region PROFILE

        /// <summary>
        /// Настройки
        /// </summary>
        IProfileMemento Profile { get; set; }

        #endregion

        #region BRIM EXTRUDER

        /// <summary>
        /// Экструдер брима
        /// </summary>
        int BrimExtruder { get; set; }

        #endregion

        #region SKIRT EXTRUDER

        /// <summary>
        /// Экструдер скирта
        /// </summary>
        int SkirtExtruder { get; set; }

        #endregion

        #region INSET 0 EXTRUDER

        /// <summary>
        /// Экструдер внешних пластиковых инсетов
        /// </summary>
        int Inset0Extruder { get; set; }

        #endregion

        #region INSET PLASTIC EXTRUDER

        /// <summary>
        /// Экструдер внутренних пластиковых инсетов
        /// </summary>
        int InsetPlasticExtruder { get; set; }

        #endregion

        #region INSET FIBER EXTRUDER

        /// <summary>
        /// Экструдер волоконных инсетов
        /// </summary>
        int? InsetFiberExtruder { get; set; }

        #endregion

        #region INFILL FIBER EXTRUDER

        /// <summary>
        /// Экструдер волоконных инфиллов
        /// </summary>
        int? InfillFiberExtruder { get; set; }

        #endregion

        #region INFILL SOLID EXTRUDER

        /// <summary>
        /// Экструдер слошного заполнения
        /// </summary>
        int InfillSolidExtruder { get; set; }

        #endregion

        #region INFILL PLASTIC CELLULAR EXTRUDER

        /// <summary>
        /// Экструдер несплошного пластикового заполнения
        /// </summary>
        int InfillPlasticCellularExtruder { get; set; }

        #endregion

        #region SUPPORT EXTRUDER

        /// <summary>
        /// Экструдер сапорта
        /// </summary>
        int SupportExtruder { get; set; }

        #endregion

        #region PLASTIC EXTRUDERS

        /// <summary>
        /// Словарь соответствия пластиков в экструдерах принтера
        /// </summary>
        IEnumerable<ExtruderPMaterial> ExtrudersPMaterials { get; }

        #endregion

        #region FIBER IN EXTRUDERS

        /// <summary>
        /// Словарь соответствия волоконных материалов в экструдерах принтера
        /// </summary>
        IEnumerable<ExtruderPFMaterial> ExtrudersPFMaterials { get; }

        #endregion

        #region SET DEFAULT

        /// <summary>
        /// Устанавливает значения по умолчанию
        /// </summary>
        void SetDefault();

        #endregion

        #region CHECK INPUTS

        bool CheckInputs(out string message);

        #endregion

        #region GET BRIM DISTANCE

        double GetBrimDistance();

        #endregion

        #region GET SKIRT DISTANCE

        double GetSkirtDistance();

        #endregion

        #region GET WT BOX

        (Point3D boxMin, Point3D boxMax) GetWTBox();

        #endregion
    }

    public sealed class SessionMemento : ISessionMemento
    {
        private readonly ISettingsFactory _settingsFactory;

        #region NAME

        private string _name;

        /// <summary>
        /// Название сессии
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
            if (newVal == null || newVal.Equals(string.Empty)) newVal = SSession.Default.Name;

            return newVal.Equals(_name);
        }

        #endregion

        #region DESCRIPTION

        private string _description;

        /// <summary>
        /// Описание сессии
        /// </summary>
        [JsonProperty]
        public string Descriprion
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

        #region LAST PRINTING DATE

        private DateTime _lastDateTime;

        /// <summary>
        /// Дата последней печати
        /// </summary>
        [JsonProperty]
        public DateTime LastPrintingDate
        {
            get => _lastDateTime;
            set
            {
                if (_lastDateTime == value) return;
                _lastDateTime = value;

                OnPropertyChanged();
            }
        }

        #endregion

        #region PRINTER

        private IPrinterMemento _printer;

        /// <summary>
        /// Принтер сессии
        /// </summary>
        [JsonProperty]
        public IPrinterMemento Printer
        {
            get => _printer;
            set
            {
                //отписываем старый принтер от реагирований на события
                if (_printer != null) _printer.PropertyChanged -= PrinterOnPropertyChanged;

                //если null, то ничего вызывать не нужно
                if (value == null) return;

                _printer = value;
                //инициализируем экструдеры и подписываем их на события изменения
                InitializeExtruderDictionaries();
                InitializeEntitiesExtruder();
                //подписываем принтер на событие изменения
                _printer.PropertyChanged += PrinterOnPropertyChanged;
                
                //вызываем события изменения принтера - для интерфейса
                OnPropertyChanged();
                //для интерфейса
                OnPropertyChanged($"ExtrudersPMaterials");
                //для интерфейса
                OnPropertyChanged($"ExtrudersPFMaterials");
            }
        }

        [ExcludeFromCodeCoverage]
        private void PrinterOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "ExtrudersP" || args.PropertyName == "ExtrudersPF")
            {
                InitializeExtruderDictionaries();
                //для интерфейса
                OnPropertyChanged($"ExtrudersPMaterials");
                OnPropertyChanged($"ExtrudersPFMaterials");
            }
            OnPropertyChanged($"Printer");
        }

        #endregion

        #region PROFILE

        private IProfileMemento _profile;

        /// <summary>
        /// Настройки
        /// </summary>
        [JsonProperty]
        public IProfileMemento Profile
        {
            get => _profile;
            set
            {
                if (_profile != null) _profile.PropertyChanged -= ProfileOnPropertyChanged;

                if (value == null) return;

                _profile = value;
                _profile.PropertyChanged += ProfileOnPropertyChanged;

                RefreshExtrudersPF();
                OnPropertyChanged();
            }
        }

        [ExcludeFromCodeCoverage]
        private void ProfileOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "LayupRule")
            {
                RefreshExtrudersPF();
            }

            //для стор - для сохранения
            OnPropertyChanged($"Profile");
        }

        [ExcludeFromCodeCoverage]
        private void RefreshExtrudersPF()
        {
            if (InsetFiberExtruder == null)
            {
                if (ExtrudersPFMaterials.Any()) InsetFiberExtruder = ExtrudersPFMaterials.First().Index;
                else InsetFiberExtruder = null;
            }
        }

        #endregion

        #region INSET 0 EXTRUDER

        private int _inset0Extruder;

        /// <summary>
        /// Экструдер внешних пластиковых инсетов
        /// </summary>
        [JsonProperty]
        public int Inset0Extruder
        {
            get => _inset0Extruder;
            set
            {
                if (_inset0Extruder == value) return;
                _inset0Extruder = value;

                OnPropertyChanged();
            }
        }

        #endregion

        #region INSET PLASTIC EXTRUDER

        private int _insetPlasticExtruder;

        /// <summary>
        /// Экструдер внутренних пластиковых инсетов
        /// </summary>
        [JsonProperty]
        public int InsetPlasticExtruder
        {
            get => _insetPlasticExtruder;
            set
            {
                if (_insetPlasticExtruder == value) return;
                _insetPlasticExtruder = value;

                OnPropertyChanged();
            }
        }

        #endregion

        #region INFILL SOLID EXTRUDER

        private int _infillSolidExtruder;

        /// <summary>
        /// Экструдер слошного заполнения
        /// </summary>
        [JsonProperty]
        public int InfillSolidExtruder
        {
            get => _infillSolidExtruder;
            set
            {
                if (_infillSolidExtruder == value) return;
                _infillSolidExtruder = value;

                OnPropertyChanged();
            }
        }

        #endregion

        #region INFILL PLASTIC CELLULAR EXTRUDER

        private int _infillPlasticCellularExtruder;

        /// <summary>
        /// Экструдер несплошного пластикового заполнения
        /// </summary>
        [JsonProperty]
        public int InfillPlasticCellularExtruder
        {
            get => _infillPlasticCellularExtruder;
            set
            {
                if (_infillPlasticCellularExtruder == value) return;
                _infillPlasticCellularExtruder = value;

                OnPropertyChanged();
            }
        }

        #endregion

        #region SUPPORT EXTRUDER

        private int _supportExtruder;

        /// <summary>
        /// Экструдер сапорта
        /// </summary>
        [JsonProperty]
        public int SupportExtruder
        {
            get => _supportExtruder;
            set
            {
                if (_supportExtruder == value) return;
                _supportExtruder = value;

                OnPropertyChanged();
            }
        }

        #endregion

        #region BRIM EXTRUDER

        private int _brimExtruder;

        /// <summary>
        /// Экструдер брима
        /// </summary>
        [JsonProperty]
        public int BrimExtruder
        {
            get => _brimExtruder;
            set
            {
                if (_brimExtruder == value) return;
                _brimExtruder = value;

                OnPropertyChanged();
            }
        }

        #endregion

        #region SKIRT EXTRUDER

        private int _skirtExtruder;

        /// <summary>
        /// Экструдер скирта
        /// </summary>
        [JsonProperty]
        public int SkirtExtruder
        {
            get => _skirtExtruder;
            set
            {
                if (_skirtExtruder == value) return;
                _skirtExtruder = value;

                OnPropertyChanged();
            }
        }

        #endregion

        #region INSET FIBER EXTRUDER

        private int? _insetFiberExtruder;

        /// <summary>
        /// Экструдер волоконных инсетов
        /// </summary>
        [JsonProperty]
        public int? InsetFiberExtruder
        {
            get => _insetFiberExtruder;
            set
            {
                if (_insetFiberExtruder == value) return;
                _insetFiberExtruder = value;

                OnPropertyChanged();
            }
        }

        #endregion

        #region INFILL FIBER EXTRUDER

        private int? _infillFiberExtruder;

        /// <summary>
        /// Экструдер волоконных инфиллов
        /// </summary>
        [JsonProperty]
        public int? InfillFiberExtruder
        {
            get => _infillFiberExtruder;
            set
            {
                if (_infillFiberExtruder == value) return;
                _infillFiberExtruder = value;

                OnPropertyChanged();
            }
        }

        #endregion

        #region PLASTIC IN EXTRUDERS

        /// <summary>
        /// Словарь соответствия пластиков в экструдерах принтера
        /// </summary>
        [ExcludeFromCodeCoverage]
        [JsonProperty]
        public IEnumerable<ExtruderPMaterial> ExtrudersPMaterials { get; private set; }

        public void SetExtrudersPMaterials(IEnumerable<ExtruderPMaterial> extruders)
        {
            ExtrudersPMaterials = extruders;
        }

        #endregion

        #region FIBER IN EXTRUDERS

        /// <summary>
        /// Словарь соответствия волоконных материалов в экструдерах принтера
        /// </summary>
        [ExcludeFromCodeCoverage]
        [JsonProperty]
        public IEnumerable<ExtruderPFMaterial> ExtrudersPFMaterials { get; private set; }

        public void SetExtrudersPFMaterials(IEnumerable<ExtruderPFMaterial> extruders)
        {
            ExtrudersPFMaterials = extruders;
        }

        #endregion

        #region IS ANISOPRINT APPROVED

        [ExcludeFromCodeCoverage]
        [JsonProperty]
        public bool? IsAnisoprintApproved
        {
            get => true;
            set { }
        }

        [ExcludeFromCodeCoverage]
        [JsonIgnore]
        public bool TrueIsAnisoprintApproved => true;

        #endregion

        #region CTOR

        public SessionMemento(ISettingsFactory factory)
        {
            _settingsFactory = factory;

            //важно. сначала установить экструдеры
            ExtrudersPMaterials = new List<ExtruderPMaterial>();
            ExtrudersPFMaterials = new List<ExtruderPFMaterial>();

            //важно. сначала установить профиль

            _lastDateTime = DateTime.Now;
        }

        public SessionMemento(IPrinterMemento printerMemento, IProfileMemento profileMemento, ISettingsFactory factory)
            :this(factory)
        {
            _profile = profileMemento;
            _printer = printerMemento;
        }

        #endregion

        #region JSON CONSTRUCTOR

        [JsonConstructor]
        [ExcludeFromCodeCoverage]
        public SessionMemento(
            string name,
            string description,
            PrinterMemento printer,
            ProfileMemento profile,
            int brimExtruder,
            int skirtExtruder,
            int inset0Extruder,
            int insetPlasticExtruder,
            int? insetFiberExtruder,
            int? infillFiberExtruder,
            int infillSolidExtruder,
            int infillPlasticCellularExtruder,
            int supportExtruder,
            List<ExtruderPMaterial> plasticInExtruders,
            List<ExtruderPFMaterial> fiberInExtruders)
        {
            _name = name;
            _description = description;
            _printer = printer;
            _profile = profile;
            _brimExtruder = brimExtruder;
            _skirtExtruder = skirtExtruder;
            _inset0Extruder = inset0Extruder;
            _insetPlasticExtruder = insetPlasticExtruder;
            _insetFiberExtruder = insetFiberExtruder;
            _infillFiberExtruder = infillFiberExtruder;
            _infillSolidExtruder = infillSolidExtruder;
            _infillPlasticCellularExtruder = infillPlasticCellularExtruder;
            _supportExtruder = supportExtruder;
            ExtrudersPMaterials = plasticInExtruders;
            ExtrudersPFMaterials = fiberInExtruders;
        }

        #endregion

        #region SET DEFAULT

        /// <summary>
        /// Устанавливает значения по умолчанию
        /// </summary>
        [ExcludeFromCodeCoverage]
        public void SetDefault()
        {
            Name = SSession.Default.Name;
            _lastDateTime = DateTime.Now;

            var printer = _settingsFactory.CreateNewPrinter();
            printer.SetDefault();
            _printer = printer;

            var profile = _settingsFactory.CreateNewProfile();
            profile.SetDefault();
            _profile = profile;

            var defaultPlastic = (MaterialPMemento)_settingsFactory.CreateNewMaterialPMemento();
            defaultPlastic.SetDefault();

            ExtrudersPMaterials = new List<ExtruderPMaterial>
            {
                new ExtruderPMaterial {Index = 0, Material = defaultPlastic}
            };

            Inset0Extruder = 0;
            InsetPlasticExtruder = 0;
            InfillSolidExtruder = 0;
            InfillPlasticCellularExtruder = 0;
            SupportExtruder = 0;
            BrimExtruder = 0;
            SkirtExtruder = 0;

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
        private bool Equals(SessionMemento other)
        {
            return string.Equals(_name, other._name) 
                   && Equals(_printer, other._printer) 
                   && Equals(_profile, other._profile) 
                   && _brimExtruder == other._brimExtruder 
                   && _skirtExtruder == other._skirtExtruder 
                   && _inset0Extruder == other._inset0Extruder 
                   && _insetPlasticExtruder == other._insetPlasticExtruder 
                   && _insetFiberExtruder == other._insetFiberExtruder 
                   && _infillFiberExtruder == other._infillFiberExtruder
                   && _infillSolidExtruder == other._infillSolidExtruder 
                   && _infillPlasticCellularExtruder == other._infillPlasticCellularExtruder 
                   && _supportExtruder == other._supportExtruder 
                   && ExtrudersPMaterials.SequenceEqual(other.ExtrudersPMaterials) 
                   && ExtrudersPFMaterials.SequenceEqual(other.ExtrudersPFMaterials);
        }

        [ExcludeFromCodeCoverage]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SessionMemento) obj);
        }

        #endregion

        #region REFRESH EXTRUDER DICTIONARIES

        private void InitializeExtruderDictionaries()
        {
            var newPlastics = new List<ExtruderPMaterial>();

            #region EXTRUDERS P

            Func<int, ExtruderPMaterial> createMaterialPNewByIndex = index =>
            {
                var item = new ExtruderPMaterial
                {
                    Index = index,
                    Material = null
                };
                return item;
            };
            foreach (var extrudersPMaterial in ExtrudersPMaterials)
            {
                extrudersPMaterial.PropertyChanged -= ExtrudersPMaterialOnPropertyChanged;
            }

            for (var i = 0; i < Printer.ExtrudersP.Count; i++)
            {
                var epIndex = Printer.ExtrudersP.ElementAt(i).TrueExtruderIndex;
                //если для такого номера экструдера уже был создан элемент с материалом, то его и оставляет
                //а если нет - то создаем заготовку
                var item = ExtrudersPMaterials.SingleOrDefault(a => a.Index == epIndex) ?? createMaterialPNewByIndex(epIndex);
                //подписываем этот итем заново на событие
                item.PropertyChanged += ExtrudersPMaterialOnPropertyChanged;
                newPlastics.Add(item);
            }


            ExtrudersPMaterials = newPlastics;

            #endregion

            #region EXTRUDERS PF

            var newFibers = new List<ExtruderPFMaterial>();

            ExtruderPFMaterial CreateMaterialPFNewByIndex(int index)
            {
                var item = new ExtruderPFMaterial
                {
                    Index = index,
                    MaterialP = null,
                    MaterialF = null
                };
                return item;
            }

            foreach (var extrudersPFMaterial in ExtrudersPFMaterials)
            {
                extrudersPFMaterial.PropertyChanged -= ExtrudersPFMaterialOnPropertyChanged;
            }

            for (var i = 0; i < Printer.ExtrudersPF.Count(); i++)
            {
                var efIndex = Printer.ExtrudersPF.ElementAt(i).TrueExtruderIndex;
                var item = ExtrudersPFMaterials.SingleOrDefault(a => a.Index == efIndex) ?? CreateMaterialPFNewByIndex(efIndex);
                item.PropertyChanged += ExtrudersPFMaterialOnPropertyChanged;
                newFibers.Add(item);
            }

            ExtrudersPFMaterials = newFibers;

            #endregion
        }

        private void ExtrudersPMaterialOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged($"Plastic");
        }

        private void ExtrudersPFMaterialOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged($"Fiber");
        }

        #endregion

        #region INITIALIZE ENTITIES EXTRUDER

        [ExcludeFromCodeCoverage]
        private void InitializeEntitiesExtruder()
        {
            bool HasIndex(int index)
            {
                if (Printer.ExtrudersP == null || !Printer.ExtrudersP.Any()) return false;
                var count = Printer.ExtrudersP.Count(a => a.ExtruderIndex == index);
                return count > 0;
            }

            int? defaultExtruderP = null;
            if (Printer.ExtrudersP != null && Printer.ExtrudersP.Any()) defaultExtruderP = Printer.ExtrudersP.Min(a => a.ExtruderIndex);
            int? defaultExtruderPF = null;
            if (Printer.ExtrudersPF != null && Printer.ExtrudersPF.Any()) defaultExtruderPF = Printer.ExtrudersPF.Min(a => a.ExtruderIndex);

            if (!HasIndex(Inset0Extruder) && defaultExtruderP.HasValue) Inset0Extruder = defaultExtruderP.Value;
            if (!HasIndex(InsetPlasticExtruder) && defaultExtruderP.HasValue) InsetPlasticExtruder = defaultExtruderP.Value;
            if (!HasIndex(InfillSolidExtruder) && defaultExtruderP.HasValue) InfillSolidExtruder = defaultExtruderP.Value;
            if (!HasIndex(InfillPlasticCellularExtruder) && defaultExtruderP.HasValue) InfillPlasticCellularExtruder = defaultExtruderP.Value;
            if (!HasIndex(SupportExtruder) && defaultExtruderP.HasValue) SupportExtruder = defaultExtruderP.Value;
            if (!HasIndex(BrimExtruder) && defaultExtruderP.HasValue) BrimExtruder = defaultExtruderP.Value;
            if (!HasIndex(SkirtExtruder) && defaultExtruderP.HasValue) SkirtExtruder = defaultExtruderP.Value;

            InsetFiberExtruder = defaultExtruderPF;
            InfillFiberExtruder = defaultExtruderPF;
        }

        #endregion

        #region CHECK INPUTS

        [ExcludeFromCodeCoverage]
        public bool CheckInputs(out string message)
        {
            if (this.Profile.GlobalSettingsMemento.TrueAllowGenerateFiber && 
                ((this.Profile.LayupRule.TrueGenerateFiberPerimeters && !this.InsetFiberExtruder.HasValue) || 
                 (this.Profile.LayupRule.TrueGenerateFiberInfill && !this.InfillFiberExtruder.HasValue)))
            {
                message = "Set extruder for fiber prints";
                return false;
            }
            message = string.Empty;
            return true;
        }

        #endregion

        #region GET BRIM DISTANCE

        public double GetBrimDistance()
        {
            if (!this.Profile.GlobalVariableSettingsMemento.TrueGenerateBrim) return 0.0;

            var brimNozzleDiameter = (int)(Printer.ExtrudersP.Single(a => a.TrueExtruderIndex == BrimExtruder).TrueNozzleDiameter * 1000);
            var brimSize = Math.Round((double)ExtrusionUtil.GetEWPlasticUM(Profile.BrimSettingsMemento.TrueEWCoeff, brimNozzleDiameter)/ 1000, 3);
            return Profile.BrimSettingsMemento.TrueLoops * brimSize;
        }

        #endregion

        #region GET SKIRT DISTANCE

        public double GetSkirtDistance()
        {
            if (!this.Profile.GlobalVariableSettingsMemento.TrueGenerateSkirt) return 0.0;

            var skirtNozzleDiameter = (int)(Printer.ExtrudersP.Single(a => a.TrueExtruderIndex == SkirtExtruder).TrueNozzleDiameter * 1000);
            var skirtSize = Math.Round((double)ExtrusionUtil.GetEWPlasticUM(Profile.BrimSettingsMemento.TrueEWCoeff, skirtNozzleDiameter) / 1000, 3);
            if (this.Profile.GlobalVariableSettingsMemento.TrueGenerateBrim)
            {
                var brimNozzleDiameter = (int)(Printer.ExtrudersP.Single(a => a.TrueExtruderIndex == BrimExtruder).TrueNozzleDiameter * 1000);
                var brimSize = Math.Round((double)ExtrusionUtil.GetEWPlasticUM(Profile.BrimSettingsMemento.TrueEWCoeff, brimNozzleDiameter) / 1000, 3);
                return 6 + brimSize * Profile.BrimSettingsMemento.TrueLoops;
            }
            return this.Profile.SkirtSettingsMemento.TrueDistance + Profile.SkirtSettingsMemento.TrueLoops * skirtSize;
        }

        #endregion

        #region GET WT BOX

        public (Point3D boxMin, Point3D boxMax) GetWTBox()
        {
            if (!this.Profile.GlobalVariableSettingsMemento.TrueGenerateWipeTower) return (new Point3D(0.0,0.0, 0.0), new Point3D(0.0,0.0, 0.0));
            var boxMin = new Point3D(this.Profile.GlobalVariableSettingsMemento.TrueWipeTowerPosX,
                this.Profile.GlobalVariableSettingsMemento.TrueWipeTowerPosY, 0.0);
            var boxMax = new Point3D(this.Profile.GlobalVariableSettingsMemento.TrueWipeTowerPosX + this.Profile.GlobalVariableSettingsMemento.TrueWipeTowerWidth, 
                this.Profile.GlobalVariableSettingsMemento.TrueWipeTowerPosY + this.Profile.GlobalVariableSettingsMemento.TrueWipeTowerLength, 0.0);

            var wtNozzleDiameter = (int)(Printer.ExtrudersP.Single(a => a.TrueExtruderIndex == Inset0Extruder).TrueNozzleDiameter * 1000);
            var brimSize = Math.Round((double)ExtrusionUtil.GetEWPlasticUM(Profile.Inset0PlasticSettingsMemento.TrueEWCoeff, wtNozzleDiameter) / 1000, 3);
            var brimWidth = this.Profile.GlobalVariableSettingsMemento.TrueBrimWTLoopsCount * brimSize;

            return (new Point3D(boxMin.X - brimWidth, boxMin.Y - brimWidth, 0.0), new Point3D(boxMax.X + brimWidth, boxMax.Y + brimWidth, 0.0));
        }

        #endregion
    }

    public sealed class ExtruderPMaterial : INotifyPropertyChanged
    {
        #region INDEX

        private int _index;
        [JsonProperty]
        [ExcludeFromCodeCoverage]
        public int Index
        {
            get => _index;
            set
            {
                if (_index == value) return;
                _index = value;

                OnPropertyChanged();
            }
        }

        #endregion

        #region MATERIAL

        private MaterialPMemento _material;

        [JsonProperty]
        public MaterialPMemento Material
        {
            get => _material;
            set
            {
                //отписываем старый материал от событий изменения
                if (value == null) return;
                if (_material != null) _material.PropertyChanged -= MaterialPOnPropertyChanged;

                _material = value;
                _material.PropertyChanged += MaterialPOnPropertyChanged;

                OnPropertyChanged();
            }
        }

        [ExcludeFromCodeCoverage]
        private void MaterialPOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            OnPropertyChanged($"Material");
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
    }

    public sealed class ExtruderPFMaterial : INotifyPropertyChanged
    {
        #region INDEX

        private int _index;

        [JsonProperty]
        [ExcludeFromCodeCoverage]
        public int Index
        {
            get => _index;
            set
            {
                if (_index == value) return;
                _index = value;

                OnPropertyChanged();
            }
        }

        #endregion

        #region MATERIAL P

        private MaterialPMemento _materialP;

        [JsonProperty]
        [ExcludeFromCodeCoverage]
        public MaterialPMemento MaterialP
        {
            get => _materialP;
            set
            {
                if (value == null) return;
                if (_materialP != null) _materialP.PropertyChanged -= MaterialPOnPropertyChanged;
                _materialP = value;
                _materialP.PropertyChanged += MaterialPOnPropertyChanged;

                OnPropertyChanged();
            }
        }

        [ExcludeFromCodeCoverage]
        private void MaterialPOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            OnPropertyChanged($"MaterialP");
        }

        #endregion

        #region MATERIAL F

        private MaterialFMemento _materialF;

        [JsonProperty]
        public MaterialFMemento MaterialF
        {
            get => _materialF;
            set
            {
                if (value == null) return;
                if (_materialF != null) _materialF.PropertyChanged -= MaterialFOnPropertyChanged;
                _materialF = value;
                _materialF.PropertyChanged += MaterialFOnPropertyChanged;

                OnPropertyChanged();
            }
        }

        [ExcludeFromCodeCoverage]
        private void MaterialFOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            OnPropertyChanged($"MaterialF");
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
    }

    [ExcludeFromCodeCoverage]
    public static class AddressHelper
    {
        private static object mutualObject;
        private static ObjectReinterpreter reinterpreter;

        static AddressHelper()
        {
            AddressHelper.mutualObject = new object();
            AddressHelper.reinterpreter = new ObjectReinterpreter();
            AddressHelper.reinterpreter.AsObject = new ObjectWrapper();
        }

        public static IntPtr GetAddress(object obj)
        {
            lock (AddressHelper.mutualObject)
            {
                AddressHelper.reinterpreter.AsObject.Object = obj;
                IntPtr address = AddressHelper.reinterpreter.AsIntPtr.Value;
                AddressHelper.reinterpreter.AsObject.Object = null;
                return address;
            }
        }

        public static T GetInstance<T>(IntPtr address)
        {
            lock (AddressHelper.mutualObject)
            {
                AddressHelper.reinterpreter.AsIntPtr.Value = address;
                return (T)AddressHelper.reinterpreter.AsObject.Object;
            }
        }

        // I bet you thought C# was type-safe.
        [StructLayout(LayoutKind.Explicit)]
        private struct ObjectReinterpreter
        {
            [FieldOffset(0)] public ObjectWrapper AsObject;
            [FieldOffset(0)] public IntPtrWrapper AsIntPtr;
        }

        private class ObjectWrapper
        {
            public object Object;
        }

        private class IntPtrWrapper
        {
            public IntPtr Value;
        }
    }
}