using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Settings.ValidValues;

namespace Settings.Memento
{
    public interface IPrinterMemento : ISettingsMemento
    {
        #region TRAVEL SPEED XY

        /// <summary>
        /// Скорость пустого перемещения (без подачи пластика или волокна) для перемещения по осям X и/или Y
        /// </summary>
        int? TravelSpeedXY { get; set; }
        int TrueTravelSpeedXY { get; }

        #endregion

        #region TRAVEL SPEED Z

        /// <summary>
        /// Скорость пустого перемещения (без подачи пластика или волокна) для перемещения по оси Z
        /// </summary>
        int? TravelSpeedZ { get; set; }
        int TrueTravelSpeedZ { get; }

        #endregion

        #region BUILD AREA

        /// <summary>
        /// Размер рабочей области
        /// </summary>

        double? Width { get; set; }
        double TrueWidth { get; }

        double? Length { get; set; }
        double TrueLength { get; }

        double? Height { get; set; }
        double TrueHeight { get; }

        #endregion

        #region HOME X POSITION

        double? HomeXPosition { get; set; }
        double TrueHomeXPosition { get; }

        #endregion

        #region HOME Y POSITION

        double? HomeYPosition { get; set; }
        double TrueHomeYPosition { get; }

        #endregion

        #region HOME Z POSITION

        double? HomeZPosition { get; set; }
        double TrueHomeZPosition { get; }

        #endregion

        #region HAS HEATED TABLE

        /// <summary>
        /// Указывает имеется ли подогреваемый стол у принтер
        /// </summary>
        bool? HasHeatedTable { get; set; }
        bool TrueHasHeatedTable { get; }

        #endregion

        #region ADDITIONAL RETRACT

        /// <summary>
        /// Дополнительный к материалу ретракт
        /// </summary>
        double? AdditionalRetractMM { get; set; }
        double TrueAdditionalRetractMM { get; }

        #endregion

        #region ON CHANGE EXTRUDER UP

        /// <summary>
        /// Величина подъема при смене экструдера
        /// </summary>
        double? OnChangeExtruderUpMM { get; set; }
        double TrueOnChangeExtruderUpMM { get; }

        #endregion

        #region USE ACCELERATIONS

        /// <summary>
        /// True, если использовать кастомные ускорения
        /// </summary>
        bool? UseAccelerations { get; set; }
        bool TrueUseAccelerations { get; }

        #endregion

        #region INSET0 ACCELERATIONS

        /// <summary>
        /// Ускорения для внешних периметров
        /// </summary>
        int? Inset0Acceleration { get; set; }
        int TrueInset0Acceleration { get; }

        #endregion

        #region OTHERS ACCELERATIONS

        /// <summary>
        /// Остальные ускорения
        /// </summary>
        int? OthersAcceleration { get; set; }
        int TrueOthersAcceleration { get; }

        #endregion

        #region USE JERK

        /// <summary>
        /// True, если использовать кастомные рывки
        /// </summary>
        bool? UseJerks { get; set; }
        bool TrueUseJerks { get; }

        #endregion

        #region INSET 0 JERK

        /// <summary>
        /// Рывок для внешних периметров
        /// </summary>
        int? Inset0Jerk { get; set; }
        int TrueInset0Jerk { get; }

        #endregion

        #region OTHERS JERK

        /// <summary>
        /// Рывок для остальных сущностей
        /// </summary>
        int? OthersJerk { get; set; }
        int TrueOthersJerk { get; }

        #endregion

        #region START G CODE

        /// <summary>
        /// Стартовый G-code
        /// </summary>
        string StartGCode { get; set; }

        #endregion

        #region END G CODE

        /// <summary>
        /// Конечный G-code
        /// </summary>
        string EndGCode { get; set; }

        #endregion

        #region DESCRIPTION

        /// <summary>
        /// Описание притера
        /// </summary>
        string Description { get; set; }

        #endregion

        #region VERSION

        /// <summary>
        /// Версия принтера
        /// </summary>
        int? Version { get; set; }
        int TrueVersion { get; }

        #endregion

        #region EXTRUDERS P

        /// <summary>
        /// Пластиковые экструдеры
        /// </summary>
        ObservableCollection<IExtruderPMemento> ExtrudersP { get; }

        #endregion

        #region EXTRUDERS PF

        /// <summary>
        /// Композитные экструдеры
        /// </summary>
        ObservableCollection<IExtruderPFMemento> ExtrudersPF { get; }

        #endregion

        #region ADD EXTRUDER P

        /// <summary>
        /// Добавляет экструдер к списку платиковых экструдеров
        /// </summary>
        IExtruderPMemento AddExtruderP();

        #endregion

        #region REMOVE EXTRUDER P

        /// <summary>
        /// Удаляет пластиковый экструдер из списка
        /// </summary>
        /// <param name="extruder">Экструдер для удаления</param>
        void RemoveExtruder(IExtruderPMemento extruder);

        #endregion

        #region ADD EXTRUDER PF

        /// <summary>
        /// Добавляет композитный экструдер в список
        /// </summary>
        /// <param name="extruder">Экструдер для добавления</param>
        IExtruderPFMemento AddExtruderPF();

        #endregion

        #region REMOVE EXTRUDER PF

        /// <summary>
        /// Удаляет композитный экструдер из списка
        /// </summary>
        /// <param name="extruder">Экструдер для удаления</param>
        void RemoveExtruder(IExtruderPFMemento extruder);

        #endregion

        #region GUID

        Guid? GUID { get; }

        #endregion

        #region PARENT GUID

        Guid? ParentGUID { get; }

        #endregion

        #region SET DEFAULT

        void SetDefault();

        #endregion
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class PrinterMemento : IPrinterMemento
    {
        private ISettingsFactory _settingsFactory;

        #region NAME

        private string _name;

        /// <summary>
        /// Название принтера
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
            if (newVal == null || newVal.Equals(string.Empty)) newVal = SPrinter.Default.Name;

            return newVal.Equals(_name);
        }

        #endregion

        #region TRAVEL SPEED

        public int TrueTravelSpeedXY { get; private set; }

        /// <summary>
        /// Скорость пустого перемещения (без подачи пластика или волокна) для перемещения по осям X и/или Y
        /// </summary>
        [JsonProperty]
        public int? TravelSpeedXY
        {
            get => TrueTravelSpeedXY;
            set
            {
                if (GetTrueTravelSpeedXY(value, out var newVal)) return;
                TrueTravelSpeedXY = newVal;
                OnPropertyChanged();
            }
        }

        private bool GetTrueTravelSpeedXY(int? value, out int newVal)
        {
            newVal = value ?? SPrinter.Default.TravelSpeedXY;

            if (newVal < SPrinter.Default.TravelSpeedXYMin || newVal > SPrinter.Default.TravelSpeedXYMax)
                newVal = SPrinter.Default.TravelSpeedXY;

            return Math.Abs(TrueTravelSpeedXY - newVal) < 0.0001;
        }

        #endregion

        #region TRAVEL SPEED Z

        public int TrueTravelSpeedZ { get; private set; }

        /// <summary>
        /// Скорость пустого перемещения (без подачи пластика или волокна) для перемещения по оси Z
        /// </summary>
        [JsonProperty]
        public int? TravelSpeedZ
        {
            get => TrueTravelSpeedZ;
            set
            {
                if (GetTrueTravelSpeedZ(value, out var newVal)) return;
                TrueTravelSpeedZ = newVal;
                OnPropertyChanged();
            }
        }

        private bool GetTrueTravelSpeedZ(int? value, out int newVal)
        {
            newVal = value ?? SPrinter.Default.TravelSpeedZ;

            if (newVal < SPrinter.Default.TravelSpeedZMin || newVal > SPrinter.Default.TravelSpeedZMax)
                newVal = SPrinter.Default.TravelSpeedZ;

            return Math.Abs(TrueTravelSpeedZ - newVal) < 0.0001;
        }

        #endregion

        #region BUILD AREA

        public double TrueWidth { get; private set; }

        [JsonProperty]
        public double? Width
        {
            get => TrueWidth;
            set
            {
                if (GetTrueWidth(value, out var newVal)) return;
                TrueWidth = newVal;

                OnPropertyChanged();
            }
        }

        private bool GetTrueWidth(double? value, out double newVal)
        {
            newVal = value ?? SPrinter.Default.Width;

            if (newVal < SPrinter.Default.WidthMin || newVal > SPrinter.Default.WidthMax)
                newVal = SPrinter.Default.Width;

            return Math.Abs(TrueWidth - newVal) < 0.0001;
        }

        public double TrueLength { get; private set; }

        [JsonProperty]
        public double? Length
        {
            get => TrueLength;
            set
            {
                if (GetTrueLength(value, out var newVal)) return;
                TrueLength = newVal;

                OnPropertyChanged();
            }
        }

        private bool GetTrueLength(double? value, out double newVal)
        {
            newVal = value ?? SPrinter.Default.Length;

            if (newVal < SPrinter.Default.LengthMin || newVal > SPrinter.Default.LengthMax)
                newVal = SPrinter.Default.Length;

            return Math.Abs(TrueLength - newVal) < 0.0001;
        }

        public double TrueHeight { get; private set; }

        [JsonProperty]
        public double? Height
        {
            get => TrueHeight;
            set
            {
                if (GetTrueHeight(value, out var newVal)) return;
                TrueHeight = newVal;

                OnPropertyChanged();
            }
        }

        private bool GetTrueHeight(double? value, out double newVal)
        {
            newVal = value ?? SPrinter.Default.Height;

            if (newVal < SPrinter.Default.HeightMin || newVal > SPrinter.Default.HeightMax)
                newVal = SPrinter.Default.Height;

            return Math.Abs(TrueHeight - newVal) < 0.0001;
        }

        #endregion

        #region HOME X POSITION

        public double TrueHomeXPosition { get; private set; }

        [JsonProperty]
        public double? HomeXPosition
        {
            get => TrueHomeXPosition;
            set
            {
                if (GetTrueHomeXPosition(value, out var newVal)) return;

                TrueHomeXPosition = newVal;
                OnPropertyChanged();
            }
        }

        private bool GetTrueHomeXPosition(double? value, out double newVal)
        {
            newVal = value ?? SPrinter.Default.HomeXPosition;

            if (newVal < SPrinter.Default.HomeXPositionMin || newVal > SPrinter.Default.HomeXPositionMax)
                newVal = SPrinter.Default.HomeXPosition;

            return Math.Abs(TrueHomeXPosition - newVal) < 0.0001;
        }

        #endregion

        #region HOME Y POSITION

        public double TrueHomeYPosition { get; private set; }

        [JsonProperty]
        public double? HomeYPosition
        {
            get => TrueHomeYPosition;
            set
            {
                if (GetTrueHomeYPosition(value, out var newVal)) return;

                TrueHomeYPosition = newVal;
                OnPropertyChanged();
            }
        }

        private bool GetTrueHomeYPosition(double? value, out double newVal)
        {
            newVal = value ?? SPrinter.Default.HomeYPosition;

            if (newVal < SPrinter.Default.HomeYPositionMin || newVal > SPrinter.Default.HomeYPositionMax)
                newVal = SPrinter.Default.HomeYPosition;

            return Math.Abs(TrueHomeYPosition - newVal) < 0.0001;
        }

        #endregion

        #region HOME Z POSITION

        public double TrueHomeZPosition { get; private set; }

        [JsonProperty]
        public double? HomeZPosition
        {
            get => TrueHomeZPosition;
            set
            {
                if (GetTrueHomeZPosition(value, out var newVal)) return;

                TrueHomeZPosition = newVal;
                OnPropertyChanged();
            }
        }

        private bool GetTrueHomeZPosition(double? value, out double newVal)
        {
            newVal = value ?? SPrinter.Default.HomeZPosition;

            if (newVal < SPrinter.Default.HomeZPositionMin || newVal > SPrinter.Default.HomeZPositionMax)
                newVal = SPrinter.Default.HomeZPosition;

            return Math.Abs(TrueHomeZPosition - newVal) < 0.0001;
        }

        #endregion

        #region HAS HEATED TABLE

        public bool TrueHasHeatedTable { get; private set; }

        /// <summary>
        /// Указывает имеется ли подогреваемый стол у принтер
        /// </summary>
        [JsonProperty]
        public bool? HasHeatedTable
        {
            get => TrueHasHeatedTable;
            set
            {
                if (TrueHasHeatedTable == value) return;
                TrueHasHeatedTable = value ?? SPrinter.Default.HasHeatedTable;

                OnPropertyChanged();
            }
        }

        #endregion

        #region ADDITIONAL RETRACT

        public double TrueAdditionalRetractMM { get; private set; }

        /// <summary>
        /// Дополнительный к материалу ретракт
        /// </summary>
        [JsonProperty]
        public double? AdditionalRetractMM
        {
            get => TrueAdditionalRetractMM;
            set
            {
                if (GetTrueAdditionalRetractMM(value, out var newVal)) return;
                TrueAdditionalRetractMM = newVal;

                OnPropertyChanged();
            }
        }

        private bool GetTrueAdditionalRetractMM(double? value, out double newVal)
        {
            newVal = value ?? SPrinter.Default.AdditionalRetractMM;

            if (newVal < SPrinter.Default.AdditionalRetractMMMin || newVal > SPrinter.Default.AdditionalRetractMMMax)
                newVal = SPrinter.Default.AdditionalRetractMM;

            return Math.Abs(TrueAdditionalRetractMM - newVal) < 0.0001;
        }

        #endregion

        #region ON CHANGE EXTRUDER UP

        public double TrueOnChangeExtruderUpMM { get; private set; }

        /// <summary>
        /// Величина подъема при смене экструдера
        /// </summary>
        [JsonProperty]
        public double? OnChangeExtruderUpMM
        {
            get => TrueOnChangeExtruderUpMM;
            set
            {
                if (GetTrueOnChangeExtruderUpMM(value, out var newVal)) return;
                TrueOnChangeExtruderUpMM = newVal;

                OnPropertyChanged();
            }
        }

        private bool GetTrueOnChangeExtruderUpMM(double? value, out double newVal)
        {
            newVal = value ?? SPrinter.Default.OnChangeExtruderUpMM;

            if (newVal < SPrinter.Default.OnChangeExtruderUpMMMin || newVal > SPrinter.Default.OnChangeExtruderUpMMMax)
                newVal = SPrinter.Default.OnChangeExtruderUpMM;

            return Math.Abs(TrueOnChangeExtruderUpMM - newVal) < 0.0001;
        }

        #endregion

        #region USE ACCELERATIONS

        public bool TrueUseAccelerations { get; private set; }

        /// <summary>
        /// True, если использовать кастомные ускорения
        /// </summary>
        [JsonProperty]
        public bool? UseAccelerations
        {
            get => TrueUseAccelerations;
            set
            {
                if (TrueUseAccelerations == value) return;
                TrueUseAccelerations = value ?? SPrinter.Default.UseAcceleration;
                OnPropertyChanged();
            }
        }

        #endregion

        #region INSET 0 ACCELERATION

        public int TrueInset0Acceleration { get; private set; }

        /// <summary>
        /// Ускорения для внешних периметров
        /// </summary>
        [JsonProperty]
        public int? Inset0Acceleration
        {
            get => TrueInset0Acceleration;
            set
            {
                if (GetTrueInset0Acceleration(value, out var newVal)) return;
                TrueInset0Acceleration = newVal;

                OnPropertyChanged();
            }
        }

        private bool GetTrueInset0Acceleration(int? value, out int newValue)
        {
            newValue = value ?? SPrinter.Default.Inset0Acceleration;

            if (newValue < SPrinter.Default.Inset0AccelerationMin || newValue > SPrinter.Default.Inset0AccelerationMax)
                newValue = SPrinter.Default.Inset0Acceleration;

            return TrueInset0Acceleration == newValue;
        }

        #endregion

        #region OTHERS ACCELERATION

        public int TrueOthersAcceleration { get; private set; }

        /// <summary>
        /// Остальные ускорения
        /// </summary>
        [JsonProperty]
        public int? OthersAcceleration
        {
            get => TrueOthersAcceleration;
            set
            {
                if (GetTrueOthersAcceleration(value, out var newVal)) return;
                TrueOthersAcceleration = newVal;

                OnPropertyChanged();
            }
        }

        private bool GetTrueOthersAcceleration(int? value, out int newValue)
        {
            newValue = value ?? SPrinter.Default.OtherAcceleration;

            if (newValue < SPrinter.Default.OtherAccelerationMin || newValue > SPrinter.Default.OtherAccelerationMax)
                newValue = SPrinter.Default.OtherAcceleration;

            return TrueOthersAcceleration == newValue;
        }

        #endregion

        #region USE JERK

        public bool TrueUseJerks { get; private set; }

        /// <summary>
        /// True, если использовать кастомные рывки
        /// </summary>
        [JsonProperty]
        public bool? UseJerks
        {
            get => TrueUseJerks;
            set
            {
                if (TrueUseJerks == value) return;
                TrueUseJerks = value ?? SPrinter.Default.UseJerk;
                OnPropertyChanged();
            }
        }

        #endregion

        #region INSET 0 JERK
        public int TrueInset0Jerk { get; private set; }

        /// <summary>
        /// Рывок для внешних периметров
        /// </summary>
        [JsonProperty]
        public int? Inset0Jerk
        {
            get => TrueInset0Jerk;
            set
            {
                if (GetTrueInset0Jerk(value, out var newValue)) return;
                TrueInset0Jerk = newValue;

                OnPropertyChanged();
            }
        }

        private bool GetTrueInset0Jerk(int? value, out int newValue)
        {
            newValue = value ?? SPrinter.Default.Inset0Jerk;

            if (newValue < SPrinter.Default.Inset0JerkMin || newValue > SPrinter.Default.Inset0JerkMax)
                newValue = SPrinter.Default.Inset0Jerk;

            return TrueInset0Jerk == newValue;
        }

        #endregion

        #region OTHERS JERK

        public int TrueOthersJerk { get; private set; }

        /// <summary>
        /// Рывок для остальных сущностей
        /// </summary>
        [JsonProperty]
        public int? OthersJerk
        {
            get => TrueOthersJerk;
            set
            {
                if (GetTrueOthersJerk(value, out var newValue)) return;
                TrueOthersJerk = newValue;

                OnPropertyChanged();
            }
        }

        private bool GetTrueOthersJerk(int? value, out int newValue)
        {
            newValue = value ?? SPrinter.Default.OtherJerk;

            if (newValue < SPrinter.Default.OtherJerkMin || newValue > SPrinter.Default.OtherJerkMax)
                newValue = SPrinter.Default.OtherJerk;

            return TrueOthersJerk == newValue;
        }

        #endregion

        #region START G CODE

        private string _startGCode;

        /// <summary>
        /// Стартовый G-code
        /// </summary>
        [JsonProperty]
        public string StartGCode
        {
            get => _startGCode;
            set
            {
                if (_startGCode == value) return;
                _startGCode = value;

                _startGCode = _startGCode.Replace("\\n", Environment.NewLine);

                OnPropertyChanged();
            }
        }

        #endregion

        #region END G CODE

        private string _endGCode;

        /// <summary>
        /// Конечный G-code
        /// </summary>
        [JsonProperty]
        public string EndGCode
        {
            get => _endGCode;
            set
            {
                if (_endGCode == value) return;
                _endGCode = value;

                _endGCode = _endGCode.Replace("\\n", Environment.NewLine);

                OnPropertyChanged();
            }
        }

        #endregion

        #region DESCRIPTION

        private string _description;

        /// <summary>
        /// Описание притера
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

        #region IS ANISOPRINT APPROVED

        private bool _isAnisoprintApproved;
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
                _isAnisoprintApproved = value ?? SPrinter.Default.IsAnisoprintApproved;

                OnPropertyChanged();
            }
        }

        #endregion

        #region VERSION

        private int _version;
        public int TrueVersion => _version;

        /// <summary>
        /// Версия принтера
        /// </summary>
        [JsonProperty]
        public int? Version
        {
            get => _version;
            set
            {
                if (_version == value) return;
                _version = value ?? SPrinter.Default.Version;

                OnPropertyChanged();
            }
        }

        #endregion

        #region EXTRUDERS P

        /// <summary>
        /// Пластиковые экструдеры
        /// </summary>
        [JsonProperty]
        [ExcludeFromCodeCoverage]
        public ObservableCollection<IExtruderPMemento> ExtrudersP { get; private set; }

        private void ExtrudersPOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged($"ExtrudersP");
        }

        /// <summary>
        /// Добавляет экструдер к списку платиковых экструдеров
        /// </summary>
        public IExtruderPMemento AddExtruderP()
        {
            var extruder = _settingsFactory.CreateNewExtruderP();
            var count = ExtrudersP.Count;
            //новый индекс - это количество экструдеров
            var newIndex = count;
            extruder.ExtruderIndex = newIndex;

            //т.к. IEnumerable не поддерживает методов листа - то пользуемся временной переменной
            ExtrudersP.Add(extruder);

            //увеличиваем  индексы композитных экструдеров
            foreach (var extruderPfMemento in ExtrudersPF)
            {
                extruderPfMemento.ExtruderIndex++;
            }

            extruder.PropertyChanged += ExtruderPMementoOnPropertyChanged;
            return extruder;
        }

        /// <summary>
        /// Удаляет пластиковый экструдер из списка
        /// </summary>
        /// <param name="extruder">Экструдер для удаления</param>
        public void RemoveExtruder(IExtruderPMemento extruder)
        {
            //находим все пластиковые экструдеры, оторый после удаляемого
            //уменьшаем их индексы
            foreach (var item in ExtrudersP)
            {
                if (item.ExtruderIndex > extruder.ExtruderIndex) item.ExtruderIndex--;
            }

            foreach (var extruderPfMemento in ExtrudersPF)
            {
                extruderPfMemento.ExtruderIndex--;
            }

            ExtrudersP.Remove(extruder);
        }

        #endregion

        #region EXTRUDER PF


        /// <summary>
        /// Композитные экструдеры
        /// </summary>
        [JsonProperty]
        [ExcludeFromCodeCoverage]
        public ObservableCollection<IExtruderPFMemento> ExtrudersPF { get; private set; }

        private void ExtrudersPFOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged($"ExtrudersPF");
        }

        private IExtruderPFMemento AddExtruderSilentPF()
        {
            var extruder = new ExtruderPFMemento();
            extruder.SetDefault();    
            var count = ExtrudersP.Count + ExtrudersPF.Count;
            var newIndex = count;
            extruder.ExtruderIndex = newIndex;

            ExtrudersPF.Add(extruder);

            extruder.PropertyChanged += ExtruderPFmementoOnPropertyChanged;
            return extruder;
        }

        /// <summary>
        /// Добавляет композитный экструдер в список
        /// </summary>
        public IExtruderPFMemento AddExtruderPF()
        {
            var extruder = AddExtruderSilentPF();

            OnPropertyChanged($"ExtrudersPF");

            return extruder;
        }

        /// <summary>
        /// Удаляет композитный экструдер из списка
        /// </summary>
        /// <param name="extruder">Экструдер для удаления</param>
        public void RemoveExtruder(IExtruderPFMemento extruder)
        {
            foreach (var item in ExtrudersPF.Where(a => a.ExtruderIndex > extruder.ExtruderIndex))
            {
                item.ExtruderIndex--;
            }
            ExtrudersPF.Remove(extruder);

            OnPropertyChanged($"ExtrudersPF");
        }

        #endregion

        #region CTOR

        [ExcludeFromCodeCoverage]
        public PrinterMemento(ISettingsFactory factory)
        {
            _settingsFactory = factory;
            ExtrudersP = new ObservableCollection<IExtruderPMemento>();
            ExtrudersP.CollectionChanged += ExtrudersPOnCollectionChanged;
            ExtrudersPF = new ObservableCollection<IExtruderPFMemento>();
            ExtrudersPF.CollectionChanged += ExtrudersPFOnCollectionChanged;
        }

        [ExcludeFromCodeCoverage]
        public PrinterMemento(IPrinterMemento memento)
        {
            Name = memento.Name;
            TravelSpeedXY = memento.TravelSpeedXY;
            TravelSpeedZ = memento.TravelSpeedZ;
            Width = memento.Width;
            Length = memento.Length;
            Height = memento.Height;
            HomeXPosition = memento.HomeXPosition;
            HomeYPosition = memento.HomeYPosition;
            HomeZPosition = memento.HomeZPosition;
            HasHeatedTable = memento.HasHeatedTable;
            AdditionalRetractMM = memento.AdditionalRetractMM;
            OnChangeExtruderUpMM = memento.OnChangeExtruderUpMM;
            UseAccelerations = memento.UseAccelerations;
            Inset0Acceleration = memento.Inset0Acceleration;
            OthersAcceleration = memento.OthersAcceleration;
            Inset0Jerk = memento.Inset0Jerk;
            OthersJerk = memento.OthersJerk;
            StartGCode = memento.StartGCode;
            EndGCode = memento.EndGCode;
            Description = memento.Description;
            IsAnisoprintApproved = memento.IsAnisoprintApproved;

            Version = SPrinter.Default.Version;

            ExtrudersP =
                new ObservableCollection<IExtruderPMemento>(memento.ExtrudersP.Select(extruderP => new ExtruderPMemento(extruderP)));
            ExtrudersPF =
                new ObservableCollection<IExtruderPFMemento>(memento.ExtrudersPF.Select(extruderPF => new ExtruderPFMemento(extruderPF)));

            CreateGUID();
            FillParentGuid(memento.GUID);
        }

        #endregion

        #region FILL FROM ANOTHER
        
        public void FillFromAnother(ISettingsMemento other)
        {
            if (other is IPrinterMemento pm)
            {
                FillFromAnother(pm);
                return;
            }
            throw new InvalidCastException();
        }

        [ExcludeFromCodeCoverage]
        public void FillFromAnother(IPrinterMemento other)
        {
            Name = other.Name;
            TravelSpeedXY = other.TravelSpeedXY;
            TravelSpeedZ = other.TravelSpeedZ;
            Width = other.Width;
            Length = other.Length;
            Height = other.Height;
            HomeXPosition = other.HomeXPosition;
            HomeYPosition = other.HomeYPosition;
            HomeZPosition = other.HomeZPosition;
            HasHeatedTable = other.HasHeatedTable;
            AdditionalRetractMM = other.AdditionalRetractMM;
            OnChangeExtruderUpMM = other.OnChangeExtruderUpMM;
            StartGCode = other.StartGCode;
            EndGCode = other.EndGCode;
            Description = other.Description;
            IsAnisoprintApproved = other.IsAnisoprintApproved;
            Version = other.Version;

            ExtrudersP =
                new ObservableCollection<IExtruderPMemento>(other.ExtrudersP.Select(extruderP => new ExtruderPMemento(extruderP)));
            ExtrudersPF =
                new ObservableCollection<IExtruderPFMemento>(other.ExtrudersPF.Select(extruderPF => new ExtruderPFMemento(extruderPF)));

            GUID = other.GUID;
            UseAccelerations = other.UseAccelerations;
            Inset0Acceleration = other.Inset0Acceleration;
            OthersAcceleration = other.OthersAcceleration;
            UseJerks = other.TrueUseJerks;
            Inset0Jerk = other.Inset0Jerk;
            OthersJerk = other.OthersJerk;
        }

        #endregion

        #region JSON CTOR

        [JsonConstructor]
        [ExcludeFromCodeCoverage]
        public PrinterMemento(string name,
            int? travelSpeedXY,
            int? travelSpeedZ,
            double? width,
            double? length,
            double? height,
            double? homeXPosition,
            double? homeYPosition,
            double? homeZPosition,
            bool? hasHeatedTable,
            double? additionalRetractMM,
            double? onChangeExtruderUpMM,
            bool? useAccelerations,
            int? inset0Acceleration,
            int? othersAcceleration,
            bool? useJerk,
            int? inset0Jerk,
            int? othersJerk,
            string startGCode,
            string endGCode,
            string description,
            bool? isAnisoprintApproved,
            int? version,
            ObservableCollection<ExtruderPMemento> extrudersP,
            ObservableCollection<ExtruderPFMemento> extrudersPf,
            Guid? guid,
            Guid? parentGuid)
        {
            Name = name;
            TravelSpeedXY = travelSpeedXY;
            TravelSpeedZ = travelSpeedZ;
            Width = width;
            Length = length;
            Height = height;
            HomeXPosition = homeXPosition;
            HomeYPosition = homeYPosition;
            HomeZPosition = homeZPosition;
            HasHeatedTable = hasHeatedTable;
            AdditionalRetractMM = additionalRetractMM;
            OnChangeExtruderUpMM = onChangeExtruderUpMM;
            UseAccelerations = useAccelerations;
            Inset0Acceleration = inset0Acceleration;
            OthersAcceleration = othersAcceleration;
            UseJerks = useJerk;
            Inset0Jerk = inset0Jerk;
            OthersJerk = othersJerk;
            StartGCode = startGCode;
            EndGCode = endGCode;
            Description = description;
            IsAnisoprintApproved = isAnisoprintApproved;
            Version = version;
            ExtrudersP = new ObservableCollection<IExtruderPMemento>(extrudersP);
            ExtrudersP.CollectionChanged += ExtrudersPOnCollectionChanged;
            ExtrudersPF = new ObservableCollection<IExtruderPFMemento>(extrudersPf);
            ExtrudersPF.CollectionChanged += ExtrudersPFOnCollectionChanged;
            GUID = guid;
            FillParentGuid(parentGuid);
        }

        #endregion

        #region EXTRUDERS PROPERTY CHANGED

        [ExcludeFromCodeCoverage]
        private void ExtruderPMementoOnPropertyChanged(object d, PropertyChangedEventArgs e)
        {
            OnPropertyChanged($"ExtrudersP");
        }

        [ExcludeFromCodeCoverage]
        private void ExtruderPFmementoOnPropertyChanged(object d, PropertyChangedEventArgs e)
        {
            OnPropertyChanged($"ExtrudersPF");
        }

        #endregion

        #region SET DEFAULT

        [ExcludeFromCodeCoverage]
        public void SetDefault()
        {
            Name = SPrinter.Default.Name;
            TravelSpeedXY = SPrinter.Default.TravelSpeedXY;
            TravelSpeedZ = SPrinter.Default.TravelSpeedZ;
            Width = SPrinter.Default.Width;
            Length = SPrinter.Default.Length;
            Height = SPrinter.Default.Height;
            HomeXPosition = SPrinter.Default.HomeXPosition;
            HomeYPosition = SPrinter.Default.HomeYPosition;
            HomeZPosition = SPrinter.Default.HomeZPosition;
            HasHeatedTable = SPrinter.Default.HasHeatedTable;
            AdditionalRetractMM = SPrinter.Default.AdditionalRetractMM;
            OnChangeExtruderUpMM = SPrinter.Default.OnChangeExtruderUpMM;
            UseAccelerations = SPrinter.Default.UseAcceleration;
            Inset0Acceleration = SPrinter.Default.Inset0Acceleration;
            OthersAcceleration = SPrinter.Default.OtherAcceleration;
            Inset0Jerk = SPrinter.Default.Inset0Jerk;
            OthersJerk = SPrinter.Default.OtherJerk;
            StartGCode = SPrinter.Default.StartGCode;
            EndGCode = SPrinter.Default.EndGCode;
            IsAnisoprintApproved = SPrinter.Default.IsAnisoprintApproved;
            Version = SPrinter.Default.Version;

            ExtrudersP.Clear();
            ExtrudersPF.Clear();
            AddExtruderP();

            CreateGUID();
        }

        #endregion

        #region EQUALS

        [ExcludeFromCodeCoverage]
        protected bool Equals(PrinterMemento other)
        {
            return string.Equals(_name, other._name) && 
                   TrueTravelSpeedXY == other.TrueTravelSpeedXY && 
                   TrueTravelSpeedZ == other.TrueTravelSpeedZ && 
                   TrueWidth.Equals(other.TrueWidth) && 
                   TrueLength.Equals(other.TrueLength) && 
                   TrueHeight.Equals(other.TrueHeight) && 
                   TrueHomeXPosition.Equals(other.TrueHomeXPosition) &&
                   TrueHomeYPosition.Equals(other.TrueHomeYPosition) &&
                   TrueHomeZPosition.Equals(other.TrueHomeZPosition) &&
                   TrueHasHeatedTable == other.TrueHasHeatedTable && 
                   TrueAdditionalRetractMM.Equals(other.TrueAdditionalRetractMM) && 
                   TrueOnChangeExtruderUpMM.Equals(other.TrueOnChangeExtruderUpMM) && 
                   string.Equals(_startGCode, other._startGCode) && 
                   string.Equals(_endGCode, other._endGCode) && 
                   _isAnisoprintApproved == other._isAnisoprintApproved && 
                   _version == other._version &&
                   Enumerable.SequenceEqual(ExtrudersP, other.ExtrudersP) &&
                   Enumerable.SequenceEqual(ExtrudersPF, other.ExtrudersPF) &&
                   TrueUseAccelerations == other.TrueUseAccelerations && 
                   TrueInset0Acceleration == other.TrueInset0Acceleration && 
                   TrueOthersAcceleration == other.TrueOthersAcceleration && 
                   TrueUseJerks == other.TrueUseJerks &&
                   TrueInset0Jerk == other.TrueInset0Jerk &&
                   TrueOthersJerk == other.TrueOthersJerk;
        }

        [ExcludeFromCodeCoverage]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PrinterMemento) obj);
        }

        public bool EqualsWithoutNameVersionApproved(ISettingsMemento other)
        {
            if (other is IPrinterMemento pm) return EqualsWithoutNameVersionApproved(pm);
            throw new InvalidCastException();
        }

        [ExcludeFromCodeCoverage]
        public bool EqualsWithoutNameVersionApproved(IPrinterMemento other)
        {
            return TravelSpeedXY == other.TravelSpeedXY &&
                   TravelSpeedZ == other.TravelSpeedZ &&
                   Width.Equals(other.Width) &&
                   Length.Equals(other.Length) &&
                   Height.Equals(other.Height) &&
                   HasHeatedTable == other.HasHeatedTable &&
                   AdditionalRetractMM.Equals(other.AdditionalRetractMM) &&
                   OnChangeExtruderUpMM.Equals(other.OnChangeExtruderUpMM) &&
                   string.Equals(_startGCode, other.StartGCode) &&
                   string.Equals(_endGCode, other.EndGCode) &&
                   Enumerable.SequenceEqual(ExtrudersP, other.ExtrudersP) &&
                   Enumerable.SequenceEqual(ExtrudersPF, other.ExtrudersPF) &&
                   UseAccelerations == other.UseAccelerations &&
                   Inset0Acceleration == other.Inset0Acceleration &&
                   OthersAcceleration == other.OthersAcceleration &&
                   Inset0Jerk == other.Inset0Jerk &&
                   OthersJerk == other.OthersJerk;
        }

        #region COMPARER

        [ExcludeFromCodeCoverage]
        private sealed class PrinterMementoEqualityComparer : IEqualityComparer<PrinterMemento>
        {
            public bool Equals(PrinterMemento x, PrinterMemento y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.TrueTravelSpeedXY == y.TrueTravelSpeedXY
                       && x.TrueTravelSpeedZ == y.TrueTravelSpeedZ
                       && x.TrueWidth.Equals(y.TrueWidth)
                       && x.TrueLength.Equals(y.TrueLength)
                       && x.TrueHeight.Equals(y.TrueHeight)
                       && string.Equals(x._startGCode, y._startGCode)
                       && string.Equals(x._endGCode, y._endGCode)
                       && x.TrueHasHeatedTable == y.TrueHasHeatedTable
                       && x._isAnisoprintApproved == y._isAnisoprintApproved
                       && x._version == y._version
                       && Enumerable.SequenceEqual(x.ExtrudersP, y.ExtrudersP)
                       && Enumerable.SequenceEqual(x.ExtrudersPF, y.ExtrudersPF);
            }

            public int GetHashCode(PrinterMemento obj)
            {
                unchecked
                {
                    var hashCode = obj.TrueTravelSpeedXY;
                    hashCode = (hashCode * 397) ^ obj.TrueTravelSpeedZ;
                    hashCode = (hashCode * 397) ^ obj.TrueWidth.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.TrueLength.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.TrueHeight.GetHashCode();
                    hashCode = (hashCode * 397) ^ (obj._startGCode != null ? obj._startGCode.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj._endGCode != null ? obj._endGCode.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ obj.TrueHasHeatedTable.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj._isAnisoprintApproved.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj._version;
                    hashCode = (hashCode * 397) ^ (obj.ExtrudersP != null ? obj.ExtrudersP.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.ExtrudersPF != null ? obj.ExtrudersPF.GetHashCode() : 0);
                    return hashCode;
                }
            }
        }

        public static IEqualityComparer<PrinterMemento> PrinterMementoComparer { get; } =
            new PrinterMementoEqualityComparer();

        #endregion

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

        private void CreateGUID()
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
        private void FillParentGuid(Guid? guid)
        {
            ParentGUID = guid;
        }

        #endregion
    }
}