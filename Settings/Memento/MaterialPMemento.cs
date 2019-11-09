using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using Newtonsoft.Json;
using Settings.ValidValues;

namespace Settings.Memento
{
    public interface IMaterialPMemento : ISettingsMemento
    {
        #region PLASTIC TYPE

        /// <summary>
        /// Тип пластика
        /// </summary>
        string PlasticType { get; set; }

        #endregion

        #region COLOR

        Color Color { get; set; }

        #endregion

        #region FILAMENT DIAMETER

        /// <summary>
        /// Диаметр пластиковой нити (мм)
        /// </summary>
        double? FilamentDiameter { get; set; }
        double TrueFilamentDiameter { get; }

        #endregion

        #region EXTRUSION MULTIPLIER P

        /// <summary>
        /// Множитель экструзии
        /// </summary>
        double? ExtrusionMultiplierP { get; set; }
        double TrueExtrusionMultiplierP { get; }

        #endregion

        #region TEMPERATURE PRINT

        /// <summary>
        /// Температура при печати
        /// </summary>
        int? TemperaturePrint { get; set; }
        int TrueTemperaturePrint { get; }

        #endregion

        #region TEMPERATURE ON FLS

        /// <summary>
        /// Температура печати на первых слоях
        /// </summary>
        int? TemperatureOnFLs { get; set; }
        int TrueTemperatureOnFLs { get; }

        #endregion

        #region TEMPERATURE WAIT

        /// <summary>
        /// Температура при ожидании
        /// </summary>
        int? TemperatureWait { get; set; }
        int TrueTemperatureWait { get; }

        #endregion

        #region BED HEATUP TEMPERATURE

        /// <summary>
        /// Температура разорева стола
        /// </summary>
        int? BedHeatupTemperature { get; set; }
        int TrueBedHeatupTemperature { get; }

        #endregion

        #region BED HEATUP ON FLS

        /// <summary>
        /// Температура стола на первых слоях
        /// </summary>
        int? BedHeatupOnFLs { get; set; }
        int TrueBedHeatupOnFLs { get; }

        #endregion

        #region FIRST LAYER HEIGHT

        /// <summary>
        /// Высота первых слоев, для которых действуют особые правила по температуре
        /// </summary>
        double? FirstLayersHeightMM { get; set; }
        double TrueFirstLayersHeightMM { get; }

        #endregion

        #region RETRACTION SPEED

        /// <summary>
        /// Скорость ретракта (для случая без движения) (мм/сек)
        /// </summary>
        int? RetractionSpeed { get; set; }
        int TrueRetractionSpeed { get; }

        #endregion

        #region RETRACTION LENGTH ON TRAVEL

        /// <summary>
        /// Длина ретракта на тревел (мм)
        /// </summary>
        double? RetractionLengthOnTravel { get; set; }
        double TrueRetractionLengthOnTravel { get; }

        #endregion

        #region RETRACTION LENGTH ON CHANGING EXTRUDER

        /// <summary>
        /// Длина ретракта при смене экструдеров (мм)
        /// </summary>
        double? RetractionLengthOnChangingExtruder { get; set; }
        double TrueRetractionLengthOnChangingExtruder { get; }

        #endregion

        #region RETRACT MINIMUM TRAVEL NO CROSSING

        /// <summary>
        /// Достаточное расстояние тревела для ретракта (мм)
        /// </summary>
        double? RetractMinimumTravelNoCrossing { get; set; }
        double TrueRetractMinimumTravelNoCrossing { get; }

        #endregion

        #region RETRACT MINIMUM TRAVEL ON CROSSING

        /// <summary>
        /// Достаточное расстояние тревела для ретракта (мм)
        /// </summary>
        double? RetractMinimumTravelOnCrossing { get; set; }
        double TrueRetractMinimumTravelOnCrossing { get; }

        #endregion

        #region COAST AT END LENGTH

        /// <summary>
        /// Длина просушки сопла в конце полигона
        /// </summary>
        double? CoastAtEndLength { get; set; }
        double TrueCoastAtEndLength { get; }

        #endregion

        #region WIPE NOZZLE LENGTH

        /// <summary>
        /// Длина прохода для вытирания сопла после окончания печати полигона
        /// </summary>
        double? WipeNozzleLength { get; set; }
        double TrueWipeNozzleLength { get; }

        #endregion

        #region EXTRA SPEED P

        /// <summary>
        /// Скорость экструзии (для случая без движения) (мм/сек)
        /// </summary>
        int? ExtraSpeedP { get; set; }
        int TrueExtraSpeedP { get; }

        #endregion

        #region EXTRA LENGTH P

        /// <summary>
        /// Длина экстра-экструзии (мм)
        /// </summary>
        double? ExtraLengthP { get; set; }
        double TrueExtraLengthP { get; }

        #endregion

        #region Z HOP P

        /// <summary>
        /// Величина поднятия экструдера в случае печати чистым пластиком
        /// </summary>
        double? ZhopP { get; set; }
        double TrueZhopP { get; }

        #endregion

        #region ENABLE PRINT COOLING

        /// <summary>
        /// True, если нужно охлаждать деталь при печати
        /// </summary>
        bool? EnablePrintCooling { get; set; }
        bool TrueEnablePrintCooling { get; }

        #endregion

        #region FIRST LAYERS FAN HEIGHT

        /// <summary>
        /// Высота первых слоев, на которых вентилятор работает в особом режиме
        /// </summary>
        double? FirstLayersFanHeight { get; set; }
        double TrueFirstLayersFanHeight { get; }

        #endregion

        #region FIRST LAYERS FAN SPEED

        /// <summary>
        /// Скоррость работы вентилятора на первом слое
        /// </summary>
        int? FirstLayersFanSpeed { get; set; }
        int TrueFirstLayersFanSpeed { get; }

        #endregion

        #region REGULAR FAN SPEED

        /// <summary>
        /// Обычная скорость обдува
        /// </summary>
        int? RegularFanSpeed { get; set; }
        int TrueRegularFanSpeed { get; }

        #endregion

        #region MAXIMUM FAN SPEED

        /// <summary>
        /// Максимальная скорость обдува
        /// </summary>
        int? MaximumFanSpeed { get; set; }
        int TrueMaximumFanSpeed { get; }

        #endregion

        #region MIN LAYER TIME FOR COOLING

        /// <summary>
        /// Минимальное время печати слоя, при котором вентиллятор будет охлаждать сильнее
        /// </summary>
        int? MinLayerTimeForCooling { get; set; }
        int TrueMinLayerTimeForCooling { get; }

        #endregion

        #region DENSITY

        /// <summary>
        /// Плостность материала (г/см3)
        /// </summary>
        double? Density { get; set; }
        double TrueDensity { get; }

        #endregion

        #region COST PER SPOOL

        /// <summary>
        /// Цена за катушку материала
        /// </summary>
        double? CostPerSpool { get; set; }
        double TrueCostPerSpool { get; }

        #endregion

        #region SPOOL WEIGHT

        /// <summary>
        /// Масса материала на катушке
        /// </summary>
        double? SpoolWeight { get; set; }
        double TrueSpoolWeight { get; }

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

    /// <summary>
    /// Хранитель настроек о пластиковом материале
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class MaterialPMemento : IMaterialPMemento
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
            if (newVal == null || newVal.Equals(string.Empty)) newVal = SMaterialP.Default.Name;

            return newVal.Equals(_name);
        }

        #endregion

        #region PLASTIC TYPE

        private string _plasticType;

        [JsonProperty]
        public string PlasticType
        {
            get => _plasticType;
            set
            {
                if (GetTruePlasticType(value, out var newVal)) return;
                _plasticType = newVal;

                OnPropertyChanged();
            }
        }

        private bool GetTruePlasticType(string value, out string newVal)
        {
            newVal = value;
            if (newVal == null || newVal.Equals(string.Empty)) newVal = SMaterialP.Default.PlasticType;

            return newVal.Equals(_plasticType);
        }

        #endregion

        #region COLOR

        private Color _color;

        [JsonProperty]
        public Color Color
        {
            get => _color;
            set
            {
                var newVal = value;

                if (newVal.Equals(_color)) return;
                _color = newVal;

                OnPropertyChanged();
            }
        }

        #endregion

        #region FILAMENT DIAMETER

        public double TrueFilamentDiameter { get; private set; }

        /// <summary>
        /// Диаметр пластиковой нити (мм)
        /// </summary>
        [JsonProperty]
        public double? FilamentDiameter
        {
            get => TrueFilamentDiameter;
            set
            {
                if (GetTrueFilamentDiameter(value, out var newVal)) return;
                TrueFilamentDiameter = newVal;

                OnPropertyChanged();
            }
        }

        private bool GetTrueFilamentDiameter(double? value, out double newVal)
        {
            newVal = value ?? SMaterialP.Default.FilamentDiameter;

            if (newVal < SMaterialP.Default.FilamentDiameterMin || newVal > SMaterialP.Default.FilamentDiameterMax)
                newVal = SMaterialP.Default.FilamentDiameter;

            return Math.Abs(TrueFilamentDiameter - newVal) < 0.0001;
        }

        #endregion

        #region EXTRUSION MULTIPLIER P

        public double TrueExtrusionMultiplierP { get; private set; }

        /// <summary>
        /// Множитель экструзии
        /// </summary>
        [JsonProperty]
        public double? ExtrusionMultiplierP
        {
            get => TrueExtrusionMultiplierP;
            set
            {
                if (GetTrueExtrusionMultiplierP(value, out var newVal)) return;
                TrueExtrusionMultiplierP = newVal;

                OnPropertyChanged();
            }
        }

        private bool GetTrueExtrusionMultiplierP(double? value, out double newVal)
        {
            newVal = value ?? SMaterialP.Default.ExtrusionMultiplierP;

            if (newVal < SMaterialP.Default.ExtrusionMultiplierPMin || newVal > SMaterialP.Default.ExtrusionMultiplierPMax)
                newVal = SMaterialP.Default.ExtrusionMultiplierP;

            return Math.Abs(TrueExtrusionMultiplierP - newVal) < 0.0001;
        }

        #endregion

        #region TEMPERATURE PRINT

        public int TrueTemperaturePrint { get; private set; }

        /// <summary>
        /// Температура при печати
        /// </summary>
        [JsonProperty]
        public int? TemperaturePrint
        {
            get => TrueTemperaturePrint;
            set
            {
                if (GetTrueTemperaturePrint(value, out var newVal)) return;
                TrueTemperaturePrint = newVal;

                OnPropertyChanged();
            }
        }

        private bool GetTrueTemperaturePrint(int? value, out int newVal)
        {
            newVal = value ?? SMaterialP.Default.TemperaturePrint;
            if (newVal < SMaterialP.Default.TemperaturePrintMin || newVal > SMaterialP.Default.TemperaturePrintMax)
                newVal = SMaterialP.Default.TemperaturePrint;

            return newVal == TrueTemperaturePrint;
        }

        #endregion

        #region TEMPERATURE ON FLS

        public int TrueTemperatureOnFLs { get; private set; }

        /// <summary>
        /// Температура печати на первых слоях
        /// </summary>
        [JsonProperty]
        public int? TemperatureOnFLs
        {
            get => TrueTemperatureOnFLs;
            set
            {
                if (GetTrueTemperatureOnFLs(value, out var newVal)) return;
                TrueTemperatureOnFLs = newVal;

                OnPropertyChanged();
            }
        }

        private bool GetTrueTemperatureOnFLs(int? value, out int newVal)
        {
            newVal = value ?? SMaterialP.Default.TemperatureOnFLs;
            if (newVal < SMaterialP.Default.TemperatureOnFLsMin || newVal > SMaterialP.Default.TemperatureOnFLsMax)
                newVal = SMaterialP.Default.TemperatureOnFLs;

            return newVal == TrueTemperatureOnFLs;
        }

        #endregion

        #region TEMPERATURE WAIT

        public int TrueTemperatureWait { get; private set; }

        /// <summary>
        /// Температура при ожидании
        /// </summary>
        [JsonProperty]
        public int? TemperatureWait
        {
            get => TrueTemperatureWait;
            set
            {
                if (GetTrueTemperatureWait(value, out var newVal)) return;
                TrueTemperatureWait = newVal;

                OnPropertyChanged();
            }
        }

        private bool GetTrueTemperatureWait(int? value, out int newVal)
        {
            newVal = value ?? SMaterialP.Default.TemperatureWait;
            if (newVal < SMaterialP.Default.TemperatureWaitMin || newVal > SMaterialP.Default.TemperatureWaitMax) newVal = SMaterialP.Default.TemperatureWait;

            return newVal == TrueTemperatureWait;
        }

        #endregion

        #region BED HEATUP TEMPERATURE
        public int TrueBedHeatupTemperature { get; private set; }

        /// <summary>
        /// Температура разорева стола
        /// </summary>
        [JsonProperty]
        public int? BedHeatupTemperature
        {
            get => TrueBedHeatupTemperature;
            set
            {
                if (GetTrueBedHeatupTemperature(value, out var newVal)) return;
                TrueBedHeatupTemperature = newVal;

                OnPropertyChanged();
            }
        }

        private bool GetTrueBedHeatupTemperature(int? value, out int newVal)
        {
            newVal = value ?? SMaterialP.Default.BedHeatupTemperature;
            if (newVal < SMaterialP.Default.BedHeatupTemperatureMin ||
                newVal > SMaterialP.Default.BedHeatupTemperatureMax)
                newVal = SMaterialP.Default.BedHeatupTemperature;

            return newVal == TrueBedHeatupTemperature;
        }

        #endregion

        #region BED HEATUP ON FLS

        public int TrueBedHeatupOnFLs { get; private set; }

        /// <summary>
        /// Температура стола на первых слоях
        /// </summary>
        [JsonProperty]
        public int? BedHeatupOnFLs
        {
            get => TrueBedHeatupOnFLs;
            set
            {
                if (GetTrueBedHeatupOnFLs(value, out var newVal)) return;
                TrueBedHeatupOnFLs = newVal;

                OnPropertyChanged();
            }
        }

        private bool GetTrueBedHeatupOnFLs(int? value, out int newVal)
        {
            newVal = value ?? SMaterialP.Default.BedHeatupOnFLs;
            if (newVal < SMaterialP.Default.BedHeatupOnFLsMin || newVal > SMaterialP.Default.BedHeatupOnFLsMax) newVal = SMaterialP.Default.BedHeatupOnFLs;

            return newVal == TrueBedHeatupOnFLs;
        }

        #endregion

        #region FIRST LAYERS HEIGHT

        public double TrueFirstLayersHeightMM { get; private set; }

        /// <summary>
        /// Высота первых слоев, для которых действуют особые правила по температуре
        /// </summary>
        [JsonProperty]
        public double? FirstLayersHeightMM
        {
            get => TrueFirstLayersHeightMM;
            set
            {
                if (GetTRueFirstLayersHeightMM(value, out var newVal)) return;
                TrueFirstLayersHeightMM = newVal;

                OnPropertyChanged();
            }
        }

        private bool GetTRueFirstLayersHeightMM(double? value, out double newVal)
        {
            newVal = value ?? SMaterialP.Default.FirstLayersHeightMM;

            if (newVal < SMaterialP.Default.FirstLayersHeightMMMin ||
                newVal > SMaterialP.Default.FirstLayersHeightMMMax) newVal = SMaterialP.Default.FirstLayersHeightMM;

            return Math.Abs(TrueFirstLayersHeightMM - newVal) < 0.0001;
        }

        #endregion

        #region RETRACTION SPEED

        public int TrueRetractionSpeed { get; private set; }

        /// <summary>
        /// Скорость ретракта (для случая без движения) (мм/сек)
        /// </summary>
        [JsonProperty]
        public int? RetractionSpeed
        {
            get => TrueRetractionSpeed;
            set
            {
                if (GetTrueRetractionSpeed(value, out var newVal)) return;
                TrueRetractionSpeed = newVal;

                OnPropertyChanged();
            }
        }

        private bool GetTrueRetractionSpeed(int? value, out int newVal)
        {
            newVal = value ?? SMaterialP.Default.RetractionSpeed;

            if (newVal < SMaterialP.Default.RetractionSpeedMin || newVal > SMaterialP.Default.RetractionSpeedMax)
                newVal = SMaterialP.Default.RetractionSpeed;

            return TrueRetractionSpeed == newVal;
        }

        #endregion

        #region RETRACTION LENGTH ON TRAVEL

        public double TrueRetractionLengthOnTravel { get; private set; }

        /// <summary>
        /// Длина ретракта на тревел (мм)
        /// </summary>
        [JsonProperty]
        public double? RetractionLengthOnTravel
        {
            get => TrueRetractionLengthOnTravel;
            set
            {
                if (GetTrueRetractionLengthOnTravel(value, out var newVal)) return;
                TrueRetractionLengthOnTravel = newVal;

                OnPropertyChanged();
            }
        }

        private bool GetTrueRetractionLengthOnTravel(double? value, out double newVal)
        {
            newVal = value ?? SMaterialP.Default.RetractionLengthOnTravel;

            if (newVal < SMaterialP.Default.RetractionLengthOnTravelMin ||
                newVal > SMaterialP.Default.RetractionLengthOnTravelMax)
                newVal = SMaterialP.Default.RetractionLengthOnTravel;

            return Math.Abs(TrueRetractionLengthOnTravel - newVal) < 0.0001;
        }

        #endregion

        #region RETRACTION LENGTH ON CHANGING EXTRUDER

        public double TrueRetractionLengthOnChangingExtruder { get; private set; }

        /// <summary>
        /// Длина ретракта при смене экструдеров (мм)
        /// </summary>
        [JsonProperty]
        public double? RetractionLengthOnChangingExtruder
        {
            get => TrueRetractionLengthOnChangingExtruder;
            set
            {
                if (GetTrueRetractionLengthOnChangingExtruder(value, out var newVal)) return;
                TrueRetractionLengthOnChangingExtruder = newVal;

                OnPropertyChanged();
            }
        }

        private bool GetTrueRetractionLengthOnChangingExtruder(double? value, out double newVal)
        {
            newVal = value ?? SMaterialP.Default.RetractionLengthOnChangingExtruder;

            if (newVal < SMaterialP.Default.RetractionLengthOnChangingExtruderMin ||
                newVal > SMaterialP.Default.RetractionLengthOnChangingExtruderMax)
                newVal = SMaterialP.Default.RetractionLengthOnChangingExtruder;

            return Math.Abs(TrueRetractionLengthOnChangingExtruder - newVal) < 0.0001;
        }

        #endregion

        #region RETRACT MINIMUM TRAVEL NO CROSSING

        public double TrueRetractMinimumTravelNoCrossing { get; private set; }

        /// <summary>
        /// Достаточное расстояние тревела для ретракта (мм)
        /// </summary>
        [JsonProperty]
        public double? RetractMinimumTravelNoCrossing
        {
            get => TrueRetractMinimumTravelNoCrossing;
            set
            {
                if (GetTrueRetractMinimumTravelNoCrossing(value, out var newVal)) return;
                TrueRetractMinimumTravelNoCrossing = newVal;

                OnPropertyChanged();
            }
        }

        private bool GetTrueRetractMinimumTravelNoCrossing(double? value, out double newVal)
        {
            newVal = value ?? SMaterialP.Default.RetractMinimumTravelNoCrossing;

            if (newVal < SMaterialP.Default.RetractMinimumTravelNoCrossingMin ||
                newVal > SMaterialP.Default.RetractMinimumTravelNoCrossingMax)
                newVal = SMaterialP.Default.RetractMinimumTravelNoCrossing;

            return Math.Abs(TrueRetractMinimumTravelNoCrossing - newVal) < 0.0001;
        }

        #endregion

        #region RETRACT MINIMUM TRAVEL ON CROSSING

        public double TrueRetractMinimumTravelOnCrossing { get; private set; }

        /// <summary>
        /// Достаточное расстояние тревела для ретракта (мм)
        /// </summary>
        [JsonProperty]
        public double? RetractMinimumTravelOnCrossing
        {
            get => TrueRetractMinimumTravelOnCrossing;
            set
            {
                if (GetTrueRetractMinimumTravelOnCrossing(value, out var newVal)) return;
                TrueRetractMinimumTravelOnCrossing = newVal;

                OnPropertyChanged();
            }
        }

        private bool GetTrueRetractMinimumTravelOnCrossing(double? value, out double newVal)
        {
            newVal = value ?? SMaterialP.Default.RetractMinimumTravelOnCrossing;

            if (newVal < SMaterialP.Default.RetractMinimumTravelOnCrossingMin ||
                newVal > SMaterialP.Default.RetractMinimumTravelOnCrossingMax)
                newVal = SMaterialP.Default.RetractMinimumTravelOnCrossing;

            return Math.Abs(TrueRetractMinimumTravelOnCrossing - newVal) < 0.0001;
        }

        #endregion

        #region COAST AT END LENGTH

        public double TrueCoastAtEndLength { get; private set; }

        /// <summary>
        /// Длина просушки сопла в конце полигона
        /// </summary>
        [JsonProperty]
        public double? CoastAtEndLength
        {
            get => TrueCoastAtEndLength;
            set
            {
                if (GetTrueCoastAtEndValue(value, out var newVal)) return;
                TrueCoastAtEndLength = newVal;

                OnPropertyChanged();
            }
        }

        private bool GetTrueCoastAtEndValue(double? value, out double newVal)
        {
            newVal = value ?? SMaterialP.Default.CoastAtEndLength;

            if (newVal < SMaterialP.Default.CoastAtEndLengthMin || newVal > SMaterialP.Default.CoastAtEndLengthMax)
                newVal = SMaterialP.Default.CoastAtEndLength;

            return Math.Abs(TrueCoastAtEndLength - newVal) < 0.0001;
        }

        #endregion

        #region WIPE NOZZLE LENGTH
        public double TrueWipeNozzleLength { get; private set; }

        /// <summary>
        /// Длина прохода для вытирания сопла после окончания печати полигона
        /// </summary>
        [JsonProperty]
        public double? WipeNozzleLength
        {
            get => TrueWipeNozzleLength;
            set
            {
                if (GetTrueWipeNozzleLength(value, out var newVal)) return;
                TrueWipeNozzleLength = newVal;

                OnPropertyChanged();
            }
        }

        private bool GetTrueWipeNozzleLength(double? value, out double newValue)
        {
            newValue = value ?? SMaterialP.Default.WipeNozzleLength;

            if (newValue < SMaterialP.Default.WipeNozzleLengthMin || newValue > SMaterialP.Default.WipeNozzleLengthMax)
                newValue = SMaterialP.Default.WipeNozzleLength;

            return Math.Abs(newValue - TrueWipeNozzleLength) < 0.0001;
        }

        #endregion

        #region EXTRA SPEED P

        public int TrueExtraSpeedP { get; private set; }

        /// <summary>
        /// Скорость экструзии (для случая без движения) (мм/сек)
        /// </summary>
        [JsonProperty]
        public int? ExtraSpeedP
        {
            get => TrueExtraSpeedP;
            set
            {
                if (GetTrueExtraSpeedP(value, out var newVal)) return;
                TrueExtraSpeedP = newVal;
                OnPropertyChanged();
            }
        }

        private bool GetTrueExtraSpeedP(int? value, out int newVal)
        {
            newVal = value ?? SMaterialP.Default.ExtraSpeedP;

            if (newVal < SMaterialP.Default.ExtraSpeedPMin || newVal > SMaterialP.Default.ExtraSpeedPMax)
                newVal = SMaterialP.Default.ExtraSpeedP;

            return TrueExtraSpeedP == newVal;
        }

        #endregion

        #region EXTRA LENGTH P

        public double TrueExtraLengthP { get; private set; }

        /// <summary>
        /// Длина экстра-экструзии (мм)
        /// </summary>
        [JsonProperty]
        public double? ExtraLengthP
        {
            get => TrueExtraLengthP;
            set
            {
                if (GetTrueExtraLengthP(value, out var newVal)) return;
                TrueExtraLengthP = newVal;
                OnPropertyChanged();
            }
        }

        private bool GetTrueExtraLengthP(double? value, out double newVal)
        {
            newVal = value ?? SMaterialP.Default.ExtraLengthP;

            if (newVal < SMaterialP.Default.ExtraLengthPMin || newVal > SMaterialP.Default.ExtraLengthPMax) newVal = SMaterialP.Default.ExtraLengthP;

            return Math.Abs(TrueExtraLengthP - newVal) < 0.0001;
        }

        #endregion

        #region Z HOP P

        public double TrueZhopP { get; private set; }

        /// <summary>
        /// Величина поднятия экструдера в случае печати чистым пластиком
        /// </summary>
        [JsonProperty]
        public double? ZhopP
        {
            get => TrueZhopP;
            set
            {
                if (GetTrueZhopP(value, out var newVal)) return;
                TrueZhopP = newVal;

                OnPropertyChanged();
            }
        }

        private bool GetTrueZhopP(double? value, out double newVal)
        {
            newVal = value ?? SMaterialP.Default.ZHopP;

            if (newVal < SMaterialP.Default.ZHopPMin || newVal > SMaterialP.Default.ZHopPMax)
                newVal = SMaterialP.Default.ZHopP;

            return Math.Abs(TrueZhopP - newVal) < 0.0001;
        }

        #endregion

        #region ENABLE PRINT COOLING

        public bool TrueEnablePrintCooling { get; private set; }

        /// <summary>
        /// True, если нужно охлаждать деталь при печати
        /// </summary>
        [JsonProperty]
        public bool? EnablePrintCooling
        {
            get => TrueEnablePrintCooling;
            set
            {
                if (TrueEnablePrintCooling == value) return;
                TrueEnablePrintCooling = value ?? SMaterialP.Default.EnablePrintCooling;

                OnPropertyChanged();
            }
        }

        #endregion

        #region FIRST LAYERS FAN HEIGHT

        public double TrueFirstLayersFanHeight { get; private set; }

        /// <summary>
        /// Высота первых слоев, на которых вентилятор работает в особом режиме
        /// </summary>
        [JsonProperty]
        public double? FirstLayersFanHeight
        {
            get => TrueFirstLayersFanHeight;
            set
            {
                if (GetTrueFirstLayersFanHeight(value, out var newVal)) return;
                TrueFirstLayersFanHeight = newVal;

                OnPropertyChanged();
            }
        }

        private bool GetTrueFirstLayersFanHeight(double? value, out double newVal)
        {
            newVal = value ?? SMaterialP.Default.FirstLayersFanHeight;

            if (newVal < SMaterialP.Default.FirstLayersFanHeightMin || newVal > SMaterialP.Default.FirstLayersFanHeightMax)
                newVal = SMaterialP.Default.FirstLayersFanHeight;

            return Math.Abs(TrueFirstLayersFanHeight - newVal) < 0.0001;
        }

        #endregion

        #region FIRST LAYERS FAN SPEED

        public int TrueFirstLayersFanSpeed { get; private set; }

        /// <summary>
        /// Скоррость работы вентилятора на первом слое
        /// </summary>
        [JsonProperty]
        public int? FirstLayersFanSpeed
        {
            get => TrueFirstLayersFanSpeed;
            set
            {
                if (GetTrueFirstLayersFanSpeed(value, out var newVal)) return;
                TrueFirstLayersFanSpeed = newVal;

                OnPropertyChanged();
            }
        }

        private bool GetTrueFirstLayersFanSpeed(int? value, out int newVal)
        {
            newVal = value ?? SMaterialP.Default.FirstLayersFanSpeed;

            if (newVal < SMaterialP.Default.FirstLayersFanSpeedMin || newVal > SMaterialP.Default.FirstLayersFanSpeedMax)
                newVal = SMaterialP.Default.FirstLayersFanSpeed;

            return Math.Abs(TrueFirstLayersFanSpeed - newVal) < 0.0001;
        }

        #endregion

        #region REGULAR FAN SPEED

        public int TrueRegularFanSpeed { get; private set; }

        /// <summary>
        /// Минимальная скорость обдува
        /// </summary>
        [JsonProperty]
        public int? RegularFanSpeed
        {
            get => TrueRegularFanSpeed;
            set
            {
                if (GetTrueRegularFanSpeed(value, out var newVal)) return;
                TrueRegularFanSpeed = newVal;

                OnPropertyChanged();
            }
        }

        private bool GetTrueRegularFanSpeed(int? value, out int newVal)
        {
            newVal = value ?? SMaterialP.Default.MinimumFanSpeed;

            if (newVal < SMaterialP.Default.MinimumFanSpeedMin || newVal > SMaterialP.Default.MinimumFanSpeedMax)
                newVal = SMaterialP.Default.MinimumFanSpeed;

            return TrueRegularFanSpeed == newVal;
        }

        #endregion

        #region MAXIMUM FAN SPEED

        public int TrueMaximumFanSpeed { get; private set; }

        /// <summary>
        /// Максимальная скорость обдува
        /// </summary>
        [JsonProperty]
        public int? MaximumFanSpeed
        {
            get => TrueMaximumFanSpeed;
            set
            {
                if (GetTrueMaximumFanSpeed(value, out var newVal)) return;
                TrueMaximumFanSpeed = newVal;

                OnPropertyChanged();
            }
        }

        private bool GetTrueMaximumFanSpeed(int? value, out int newVal)
        {
            newVal = value ?? SMaterialP.Default.MaximumFanSpeed;

            if (newVal < SMaterialP.Default.MaximumFanSpeedMin || newVal > SMaterialP.Default.MaximumFanSpeedMax)
                newVal = SMaterialP.Default.MaximumFanSpeed;

            return TrueMaximumFanSpeed == newVal;
        }

        #endregion

        #region MIN LAYER TIME FOR COOLING

        public int TrueMinLayerTimeForCooling { get; private set; }

        /// <summary>
        /// Минимальное время печати слоя, при котором вентиллятор будет охлаждать сильнее
        /// </summary>
        [JsonProperty]
        public int? MinLayerTimeForCooling
        {
            get => TrueMinLayerTimeForCooling;
            set
            {
                if (GetTrueMinLayerTimeForCooling(value, out var newVal)) return;
                TrueMinLayerTimeForCooling = newVal;

                OnPropertyChanged();
            }
        }

        private bool GetTrueMinLayerTimeForCooling(int? value, out int newVal)
        {
            newVal = value ?? SMaterialP.Default.MinLayerTimeForCooling;

            if (newVal < SMaterialP.Default.MinLayerTimeForCoolingMin || newVal > SMaterialP.Default.MinLayerTimeForCoolingMax)
                newVal = SMaterialP.Default.MinLayerTimeForCooling;

            return TrueMinLayerTimeForCooling == newVal;
        }

        #endregion

        #region DENSITY

        public double TrueDensity { get; private set; }

        /// <summary>
        /// Плостность материала (г/см3)
        /// </summary>
        [JsonProperty]
        public double? Density
        {
            get => TrueDensity;
            set
            {
                if (GetTrueDensity(value, out var newVal)) return;
                TrueDensity = newVal;

                OnPropertyChanged();
            }
        }

        private bool GetTrueDensity(double? value, out double newVal)
        {
            newVal = value ?? SMaterialP.Default.Density;

            if (newVal < SMaterialP.Default.DensityMin || newVal > SMaterialP.Default.DensityMax) newVal = SMaterialP.Default.Density;

            return Math.Abs(newVal - TrueDensity) < 0.0001;
        }

        #endregion

        #region COST PER SPOOL

        public double TrueCostPerSpool { get; private set; }

        /// <summary>
        /// Цена за катушку материала
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
            newVal = value ?? SMaterialP.Default.CostPerSpool;

            if (newVal < SMaterialP.Default.CostPerSpoolMin || newVal > SMaterialP.Default.CostPerSpoolMax)
                newVal = SMaterialP.Default.CostPerSpool;

            return Math.Abs(TrueCostPerSpool - newVal) < 0.0001;
        }

        #endregion

        #region SPOOL WEIGHT

        public double TrueSpoolWeight { get; private set; }

        /// <summary>
        /// Масса материала на катушке
        /// </summary>
        [JsonProperty]
        public double? SpoolWeight
        {
            get => TrueSpoolWeight;
            set
            {
                if (GetTrueSpoolWeight(value, out var newVal)) return;
                TrueSpoolWeight = newVal;

                OnPropertyChanged();
            }
        }

        private bool GetTrueSpoolWeight(double? value, out double newVal)
        {
            newVal = value ?? SMaterialP.Default.SpoolWeight;

            if (newVal < SMaterialP.Default.SpoolWeightMin || newVal > SMaterialP.Default.SpoolWeightMax)
                newVal = SMaterialP.Default.SpoolWeight;

            return Math.Abs(newVal - TrueSpoolWeight) < 0.0001;
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
                if (value == TrueVersion) return;
                TrueVersion = value ?? SMaterialP.Default.Version;
                OnPropertyChanged();
            }
        }

        #endregion

        #region IS ANISOPRINT APPROVED

        public bool TrueIsAnisoprintApproved { get; private set; }

        /// <summary>
        /// Поле, указывающее одобрен ли материал компанией Anisoprint
        /// </summary>
        [JsonProperty]
        public bool? IsAnisoprintApproved
        {
            get => TrueIsAnisoprintApproved;
            set
            {
                if (value == TrueIsAnisoprintApproved) return;
                TrueIsAnisoprintApproved = value ?? SMaterialP.Default.IsAnisoprintApproved;
                OnPropertyChanged();
            }
        }
        #endregion

        #region CTOR

        [ExcludeFromCodeCoverage]
        public MaterialPMemento()
        {
        }

        [ExcludeFromCodeCoverage]
        public MaterialPMemento(IMaterialPMemento memento)
        {
            Name = memento.Name;
            PlasticType = memento.PlasticType;
            Color = memento.Color;
            FilamentDiameter = memento.FilamentDiameter;
            ExtrusionMultiplierP = memento.ExtrusionMultiplierP;
            TemperaturePrint = memento.TemperaturePrint;
            TemperatureOnFLs = memento.TemperatureOnFLs;
            TemperatureWait = memento.TemperatureWait;
            BedHeatupTemperature = memento.BedHeatupTemperature;
            BedHeatupOnFLs = memento.BedHeatupOnFLs;
            FirstLayersHeightMM = memento.FirstLayersHeightMM;
            RetractionSpeed = memento.RetractionSpeed;
            RetractionLengthOnTravel = memento.RetractionLengthOnTravel;
            RetractionLengthOnChangingExtruder = memento.RetractionLengthOnChangingExtruder;
            RetractMinimumTravelNoCrossing = memento.RetractMinimumTravelNoCrossing;
            RetractMinimumTravelOnCrossing = memento.RetractMinimumTravelOnCrossing;
            CoastAtEndLength = memento.CoastAtEndLength;
            WipeNozzleLength = memento.WipeNozzleLength;
            ExtraSpeedP = memento.ExtraSpeedP;
            ExtraLengthP = memento.ExtraLengthP;
            ZhopP = memento.ZhopP;
            EnablePrintCooling = memento.EnablePrintCooling;
            FirstLayersFanHeight = memento.FirstLayersFanHeight;
            FirstLayersFanSpeed = memento.FirstLayersFanSpeed;
            RegularFanSpeed = memento.RegularFanSpeed;
            MaximumFanSpeed = memento.MaximumFanSpeed;
            MinLayerTimeForCooling = memento.MinLayerTimeForCooling;
            Density = memento.Density;
            CostPerSpool = memento.CostPerSpool;
            SpoolWeight = memento.SpoolWeight;
            Description = memento.Description;
            IsAnisoprintApproved = memento.IsAnisoprintApproved;
            Version = SMaterialP.Default.Version;
            CreateGUID();
            FillParentGUID(memento.GUID);
        }

        [ExcludeFromCodeCoverage]
        [JsonConstructor]
        public MaterialPMemento(
            string name, 
            string plasticType, 
            Color color, 
            double? filamentDiameter, 
            double? extrusionMultiplierP, 
            int? temperaturePrint, 
            int? temperatureOnFLs, 
            int? temperatureWait, 
            int? bedHeatupTemperature, 
            int? bedHeatupOnFLs, 
            double? firstLayersHeightMM, 
            int? retractionSpeed, 
            double? retractionLengthOnTravel, 
            double? retractionLengthOnChangingExtruder, 
            double? retractMinimumTravelNoCrossing,
            double? retractMinimumTravelOnCrossing,
            double? coastAtEndLength, 
            double? wipeNozzleLength, 
            int? extraSpeedP, 
            double? extraLengthP,
            double? zHopP,
            bool? enablePrintCooling,
            double? firstLayersFanHeight,
            int? firstLayersFanSpeed,
            int? regularFanSpeed,
            int? maximumFanSpeed,
            int? minLayerTimeForCooling,
            double? density, 
            double? costPerSpool, 
            double? spoolWeight, 
            string description, 
            int? version, 
            bool? isAnisoprintApproved,
            Guid? guid,
            Guid? parentGuid)
        {
            Name = name;
            PlasticType = plasticType;
            Color = color;
            FilamentDiameter = filamentDiameter;
            ExtrusionMultiplierP = extrusionMultiplierP;
            TemperaturePrint = temperaturePrint;
            TemperatureOnFLs = temperatureOnFLs;
            TemperatureWait = temperatureWait;
            BedHeatupTemperature = bedHeatupTemperature;
            BedHeatupOnFLs = bedHeatupOnFLs;
            FirstLayersHeightMM = firstLayersHeightMM;
            RetractionSpeed = retractionSpeed;
            RetractionLengthOnTravel = retractionLengthOnTravel;
            RetractionLengthOnChangingExtruder = retractionLengthOnChangingExtruder;
            RetractMinimumTravelNoCrossing = retractMinimumTravelNoCrossing;
            RetractMinimumTravelOnCrossing = retractMinimumTravelOnCrossing;
            CoastAtEndLength = coastAtEndLength;
            WipeNozzleLength = wipeNozzleLength;
            ExtraSpeedP = extraSpeedP;
            ExtraLengthP = extraLengthP;
            ZhopP = zHopP;
            EnablePrintCooling = enablePrintCooling;
            FirstLayersFanHeight = firstLayersFanHeight;
            FirstLayersFanSpeed = firstLayersFanSpeed;
            RegularFanSpeed = regularFanSpeed;
            MaximumFanSpeed = maximumFanSpeed;
            MinLayerTimeForCooling = minLayerTimeForCooling;
            Density = density;
            CostPerSpool = costPerSpool;
            SpoolWeight = spoolWeight;
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
            if (other is IMaterialPMemento mpm)
            {
                FillFromAnother(mpm);
                return;
            }
            throw new InvalidCastException();
        }

        public void FillFromAnother(IMaterialPMemento other)
        {
            Name = other.Name;
            PlasticType = other.PlasticType;
            Color = other.Color;
            FilamentDiameter = other.FilamentDiameter;
            ExtrusionMultiplierP = other.ExtrusionMultiplierP;
            TemperaturePrint = other.TemperaturePrint;
            TemperatureOnFLs = other.TemperatureOnFLs;
            TemperatureWait = other.TemperatureWait;
            BedHeatupTemperature = other.BedHeatupTemperature;
            BedHeatupOnFLs = other.BedHeatupOnFLs;
            FirstLayersHeightMM = other.FirstLayersHeightMM;
            RetractionSpeed = other.RetractionSpeed;
            RetractionLengthOnTravel = other.RetractionLengthOnTravel;
            RetractionLengthOnChangingExtruder = other.RetractionLengthOnChangingExtruder;
            RetractMinimumTravelNoCrossing = other.RetractMinimumTravelNoCrossing;
            RetractMinimumTravelOnCrossing = other.RetractMinimumTravelOnCrossing;
            CoastAtEndLength = other.CoastAtEndLength;
            WipeNozzleLength = other.WipeNozzleLength;
            ExtraSpeedP = other.ExtraSpeedP;
            ExtraLengthP = other.ExtraLengthP;
            ZhopP = other.ZhopP;
            EnablePrintCooling = other.EnablePrintCooling;
            FirstLayersFanHeight = other.FirstLayersFanHeight;
            FirstLayersFanSpeed = other.FirstLayersFanSpeed;
            RegularFanSpeed = other.RegularFanSpeed;
            MaximumFanSpeed = other.MaximumFanSpeed;
            MinLayerTimeForCooling = other.MinLayerTimeForCooling;
            Density = other.Density;
            CostPerSpool = other.CostPerSpool;
            SpoolWeight = other.SpoolWeight;
            Description = other.Description;
            Version = other.Version;
            IsAnisoprintApproved = other.IsAnisoprintApproved;
            GUID = other.GUID;
        }

        #endregion

        #region SET DEFAULT

        /// <summary>
        /// Устанавливает значения по умолчанию
        /// </summary>
        [ExcludeFromCodeCoverage]
        public void SetDefault()
        {
            Name = SMaterialP.Default.Name;
            PlasticType = SMaterialP.Default.PlasticType;
            Color = SMaterialP.Default.Color;
            FilamentDiameter = SMaterialP.Default.FilamentDiameter;
            ExtrusionMultiplierP = SMaterialP.Default.ExtrusionMultiplierP;
            TemperaturePrint = SMaterialP.Default.TemperaturePrint;
            TemperatureOnFLs = SMaterialP.Default.TemperatureOnFLs;
            TemperatureWait = SMaterialP.Default.TemperatureWait;
            BedHeatupTemperature = SMaterialP.Default.BedHeatupTemperature;
            BedHeatupOnFLs = SMaterialP.Default.BedHeatupOnFLs;
            FirstLayersHeightMM = SMaterialP.Default.FirstLayersHeightMM;
            RetractionSpeed = SMaterialP.Default.RetractionSpeed;
            RetractionLengthOnTravel = SMaterialP.Default.RetractionLengthOnTravel;
            RetractionLengthOnChangingExtruder = SMaterialP.Default.RetractionLengthOnChangingExtruder;
            RetractMinimumTravelNoCrossing = SMaterialP.Default.RetractMinimumTravelNoCrossing;
            RetractMinimumTravelOnCrossing = SMaterialP.Default.RetractMinimumTravelOnCrossing;
            CoastAtEndLength = SMaterialP.Default.CoastAtEndLength;
            WipeNozzleLength = SMaterialP.Default.WipeNozzleLength;
            ExtraSpeedP = SMaterialP.Default.ExtraSpeedP;
            ExtraLengthP = SMaterialP.Default.ExtraLengthP;
            ZhopP = SMaterialP.Default.ZHopP;
            EnablePrintCooling = SMaterialP.Default.EnablePrintCooling;
            FirstLayersFanHeight = SMaterialP.Default.FirstLayersFanHeight;
            FirstLayersFanSpeed = SMaterialP.Default.FirstLayersFanSpeed;
            RegularFanSpeed = SMaterialP.Default.MinimumFanSpeed;
            MaximumFanSpeed = SMaterialP.Default.MaximumFanSpeed;
            MinLayerTimeForCooling = SMaterialP.Default.MinLayerTimeForCooling;
            Density = SMaterialP.Default.Density;
            CostPerSpool = SMaterialP.Default.CostPerSpool;
            SpoolWeight = SMaterialP.Default.SpoolWeight;
            Version = SMaterialP.Default.Version;
            IsAnisoprintApproved = SMaterialP.Default.IsAnisoprintApproved;

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

        private void OnPropertyChanging([CallerMemberName] string caller = null)
        {
            var handler = PropertyChanging;
            handler?.Invoke(this, new PropertyChangingEventArgs(caller));
        }

        #endregion

        #region EQUALS

        protected bool Equals(MaterialPMemento other)
        {
            return string.Equals(_name, other._name) && 
                   string.Equals(_plasticType, other._plasticType) && 
                   _color.Equals(other._color) && 
                   TrueFilamentDiameter.Equals(other.TrueFilamentDiameter) && 
                   TrueExtrusionMultiplierP.Equals(other.TrueExtrusionMultiplierP) && 
                   TrueTemperaturePrint == other.TrueTemperaturePrint && 
                   TrueTemperatureOnFLs == other.TrueTemperatureOnFLs && 
                   TrueTemperatureWait == other.TrueTemperatureWait && 
                   TrueBedHeatupTemperature == other.TrueBedHeatupTemperature && 
                   TrueBedHeatupOnFLs == other.TrueBedHeatupOnFLs && 
                   TrueFirstLayersHeightMM.Equals(other.TrueFirstLayersHeightMM) && 
                   TrueRetractionSpeed == other.TrueRetractionSpeed && 
                   TrueRetractionLengthOnTravel.Equals(other.TrueRetractionLengthOnTravel) && 
                   TrueRetractionLengthOnChangingExtruder.Equals(other.TrueRetractionLengthOnChangingExtruder) && 
                   TrueRetractMinimumTravelNoCrossing.Equals(other.TrueRetractMinimumTravelNoCrossing) && 
                   TrueRetractMinimumTravelOnCrossing.Equals(other.TrueRetractMinimumTravelOnCrossing) &&
                   TrueCoastAtEndLength.Equals(other.TrueCoastAtEndLength) && 
                   TrueWipeNozzleLength.Equals(other.TrueWipeNozzleLength) && 
                   TrueExtraSpeedP == other.TrueExtraSpeedP && 
                   TrueExtraLengthP.Equals(other.TrueExtraLengthP) && 
                   TrueZhopP.Equals(other.TrueZhopP) && 
                   TrueEnablePrintCooling == other.TrueEnablePrintCooling && 
                   TrueFirstLayersFanHeight.Equals(other.TrueFirstLayersFanHeight) && 
                   TrueFirstLayersFanSpeed == other.TrueFirstLayersFanSpeed && 
                   TrueRegularFanSpeed == other.TrueRegularFanSpeed && 
                   TrueMaximumFanSpeed == other.TrueMaximumFanSpeed && 
                   TrueMinLayerTimeForCooling == other.TrueMinLayerTimeForCooling && 
                   TrueDensity.Equals(other.TrueDensity) && 
                   TrueCostPerSpool.Equals(other.TrueCostPerSpool) && 
                   TrueSpoolWeight.Equals(other.TrueSpoolWeight) && 
                   string.Equals(_description, other._description) && 
                   TrueVersion == other.TrueVersion && 
                   TrueIsAnisoprintApproved == other.TrueIsAnisoprintApproved;
        }

        [ExcludeFromCodeCoverage]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MaterialPMemento) obj);
        }

        public bool EqualsWithoutNameVersionApproved(ISettingsMemento other)
        {
            if (other is IMaterialPMemento mpm) return EqualsWithoutNameVersionApproved(mpm);
            throw new InvalidCastException();
        }

        public bool EqualsWithoutNameVersionApproved(IMaterialPMemento other)
        {
            return string.Equals(_plasticType, other.PlasticType) &&
                   _color.Equals(other.Color) &&
                   TrueFilamentDiameter.Equals(other.TrueFilamentDiameter) &&
                   TrueExtrusionMultiplierP.Equals(other.TrueExtrusionMultiplierP) &&
                   TrueTemperaturePrint == other.TrueTemperaturePrint &&
                   TrueTemperatureOnFLs == other.TrueTemperatureOnFLs &&
                   TrueTemperatureWait == other.TrueTemperatureWait &&
                   TrueBedHeatupTemperature == other.TrueBedHeatupTemperature &&
                   TrueBedHeatupOnFLs == other.TrueBedHeatupOnFLs &&
                   TrueFirstLayersHeightMM.Equals(other.TrueFirstLayersHeightMM) &&
                   TrueRetractionSpeed == other.TrueRetractionSpeed &&
                   TrueRetractionLengthOnTravel.Equals(other.TrueRetractionLengthOnTravel) &&
                   TrueRetractionLengthOnChangingExtruder.Equals(other.TrueRetractionLengthOnChangingExtruder) &&
                   TrueRetractMinimumTravelNoCrossing.Equals(other.TrueRetractMinimumTravelNoCrossing) &&
                   TrueRetractMinimumTravelOnCrossing.Equals(other.TrueRetractMinimumTravelOnCrossing) &&
                   TrueCoastAtEndLength.Equals(other.TrueCoastAtEndLength) &&
                   TrueWipeNozzleLength.Equals(other.TrueWipeNozzleLength) &&
                   TrueExtraSpeedP == other.TrueExtraSpeedP &&
                   TrueExtraLengthP.Equals(other.TrueExtraLengthP) &&
                   TrueZhopP.Equals(other.ZhopP) &&
                   TrueEnablePrintCooling == other.TrueEnablePrintCooling &&
                   TrueFirstLayersFanHeight.Equals(other.TrueFirstLayersFanHeight) &&
                   TrueFirstLayersFanSpeed == other.TrueFirstLayersFanSpeed &&
                   TrueRegularFanSpeed == other.TrueRegularFanSpeed &&
                   TrueMaximumFanSpeed == other.TrueMaximumFanSpeed &&
                   TrueMinLayerTimeForCooling == other.TrueMinLayerTimeForCooling &&
                   TrueDensity.Equals(other.TrueDensity) &&
                   TrueCostPerSpool.Equals(other.TrueCostPerSpool) &&
                   TrueSpoolWeight.Equals(other.TrueSpoolWeight) &&
                   string.Equals(_description, other.Description);
        }

        [ExcludeFromCodeCoverage]
        private sealed class MaterialPMementoEqualityComparer : IEqualityComparer<MaterialPMemento>
        {
            public bool Equals(MaterialPMemento x, MaterialPMemento y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return string.Equals(x._name, y._name) && 
                       string.Equals(x._plasticType, y._plasticType) && 
                       x._color.Equals(y._color) && 
                       x.TrueFilamentDiameter.Equals(y.TrueFilamentDiameter) && 
                       x.TrueExtrusionMultiplierP.Equals(y.TrueExtrusionMultiplierP) && 
                       x.TrueTemperaturePrint == y.TrueTemperaturePrint && 
                       x.TrueTemperatureOnFLs == y.TrueTemperatureOnFLs && 
                       x.TrueTemperatureWait == y.TrueTemperatureWait && 
                       x.TrueBedHeatupTemperature == y.TrueBedHeatupTemperature && 
                       x.TrueBedHeatupOnFLs == y.TrueBedHeatupOnFLs && 
                       x.TrueFirstLayersHeightMM.Equals(y.TrueFirstLayersHeightMM) && 
                       x.TrueRetractionSpeed == y.TrueRetractionSpeed && 
                       x.TrueRetractionLengthOnTravel.Equals(y.TrueRetractionLengthOnTravel) && 
                       x.TrueRetractionLengthOnChangingExtruder.Equals(y.TrueRetractionLengthOnChangingExtruder) && 
                       x.TrueRetractMinimumTravelNoCrossing.Equals(y.TrueRetractMinimumTravelNoCrossing) &&
                       x.TrueRetractMinimumTravelOnCrossing.Equals(y.TrueRetractMinimumTravelOnCrossing) &&
                       x.TrueCoastAtEndLength.Equals(y.TrueCoastAtEndLength) && 
                       x.TrueWipeNozzleLength.Equals(y.TrueWipeNozzleLength) && 
                       x.TrueExtraSpeedP == y.TrueExtraSpeedP && 
                       x.TrueExtraLengthP.Equals(y.TrueExtraLengthP) && 
                       x.TrueZhopP.Equals(y.TrueZhopP) && 
                       x.TrueEnablePrintCooling == y.TrueEnablePrintCooling && 
                       x.TrueFirstLayersFanHeight.Equals(y.TrueFirstLayersFanHeight) && 
                       x.TrueFirstLayersFanSpeed == y.TrueFirstLayersFanSpeed && 
                       x.TrueRegularFanSpeed == y.TrueRegularFanSpeed && 
                       x.TrueMaximumFanSpeed == y.TrueMaximumFanSpeed && 
                       x.TrueMinLayerTimeForCooling == y.TrueMinLayerTimeForCooling && 
                       x.TrueDensity.Equals(y.TrueDensity) && 
                       x.TrueCostPerSpool.Equals(y.TrueCostPerSpool) && 
                       x.TrueSpoolWeight.Equals(y.TrueSpoolWeight) && 
                       string.Equals(x._description, y._description) && 
                       x.TrueVersion == y.TrueVersion && 
                       x.TrueIsAnisoprintApproved == y.TrueIsAnisoprintApproved;
            }

            public int GetHashCode(MaterialPMemento obj)
            {
                unchecked
                {
                    var hashCode = (obj._name != null ? obj._name.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj._plasticType != null ? obj._plasticType.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ obj._color.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.TrueFilamentDiameter.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.TrueExtrusionMultiplierP.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.TrueTemperaturePrint;
                    hashCode = (hashCode * 397) ^ obj.TrueTemperatureOnFLs;
                    hashCode = (hashCode * 397) ^ obj.TrueTemperatureWait;
                    hashCode = (hashCode * 397) ^ obj.TrueBedHeatupTemperature;
                    hashCode = (hashCode * 397) ^ obj.TrueBedHeatupOnFLs;
                    hashCode = (hashCode * 397) ^ obj.TrueFirstLayersHeightMM.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.TrueRetractionSpeed;
                    hashCode = (hashCode * 397) ^ obj.TrueRetractionLengthOnTravel.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.TrueRetractionLengthOnChangingExtruder.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.TrueRetractMinimumTravelNoCrossing.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.TrueRetractMinimumTravelOnCrossing.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.TrueCoastAtEndLength.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.TrueWipeNozzleLength.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.TrueExtraSpeedP;
                    hashCode = (hashCode * 397) ^ obj.TrueExtraLengthP.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.TrueZhopP.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.TrueEnablePrintCooling.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.TrueFirstLayersFanHeight.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.TrueFirstLayersFanSpeed;
                    hashCode = (hashCode * 397) ^ obj.TrueRegularFanSpeed;
                    hashCode = (hashCode * 397) ^ obj.TrueMaximumFanSpeed;
                    hashCode = (hashCode * 397) ^ obj.TrueMinLayerTimeForCooling;
                    hashCode = (hashCode * 397) ^ obj.TrueDensity.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.TrueCostPerSpool.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.TrueSpoolWeight.GetHashCode();
                    hashCode = (hashCode * 397) ^ (obj._description != null ? obj._description.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ obj.TrueVersion;
                    hashCode = (hashCode * 397) ^ obj.TrueIsAnisoprintApproved.GetHashCode();
                    return hashCode;
                }
            }
        }

        public static IEqualityComparer<MaterialPMemento> MaterialPMementoComparer { get; } = new MaterialPMementoEqualityComparer();

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
        private void FillParentGUID(Guid? guid)
        {
            ParentGUID = guid;
        }

        #endregion
    }
}