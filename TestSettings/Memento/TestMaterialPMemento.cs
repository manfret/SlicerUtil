using System;
using System.Windows.Media;
using NUnit.Framework;
using Settings;
using Settings.Memento;
using Settings.ValidValues;

namespace TestSettings.Memento
{
    [TestFixture]
    public class TestMaterialPMemento
    {
        [Test]
        public void MaterialPMemento_Name_InAcceptableRange()
        {
            var raised = false;
            var raisedChanging = false;

            var settingsMemento = new MaterialPMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Name") raised = true;
            };
            settingsMemento.PropertyChanging += (sender, args) =>
            {
                if (args.PropertyName == "Name") raisedChanging = true;
            };

            Assert.AreEqual(false, raised);
            Assert.AreEqual(false, raisedChanging);
            settingsMemento.Name = "bzz";
            Assert.AreEqual(true, raised);
            Assert.AreEqual(true, raisedChanging);
        }

        [Test]
        public void MaterialPMemento_Name_Same()
        {
            var raised = false;
            var raisedChanging = false;

            var settingsMemento = new MaterialPMemento();

            Assert.AreEqual(false, raised);
            Assert.AreEqual(false, raisedChanging);
            settingsMemento.Name = "bzz";
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Name") raised = true;
            };
            settingsMemento.PropertyChanging += (sender, args) =>
            {
                if (args.PropertyName == "Name") raisedChanging = true;
            };
            settingsMemento.Name = "bzz";
            Assert.AreEqual(false, raised);
            Assert.AreEqual(false, raisedChanging);
        }

        [Test]
        public void MaterialPMemento_Name_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Name") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.Name = string.Empty;
            Assert.AreEqual(SMaterialP.Default.Name, settingsMemento.Name);
        }

        [Test]
        public void MaterialPMemento_PlasticType_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "PlasticType") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.PlasticType = "bzz";
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void MaterialPMemento_PlasticType_Same()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();

            Assert.AreEqual(false, raised);
            settingsMemento.PlasticType = "bzz";
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "PlasticType") raised = true;
            };
            settingsMemento.PlasticType = "bzz";
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void MaterialPMemento_PlasticType_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "PlasticType") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.PlasticType = string.Empty;
            Assert.AreEqual(SMaterialP.Default.PlasticType, settingsMemento.PlasticType);
        }

        [Test]
        public void MaterialPMemento_Color_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Color") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.Color = Colors.AliceBlue;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void MaterialPMemento_Color_Same()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();

            Assert.AreEqual(false, raised);
            settingsMemento.Color = Colors.AliceBlue;
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Color") raised = true;
            };
            settingsMemento.Color = Colors.AliceBlue;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void MaterialPMemento_FilamentDiameter_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "FilamentDiameter") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.FilamentDiameter = 0.1;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void MaterialPMemento_FilamentDiameter_Same()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();

            Assert.AreEqual(false, raised);
            settingsMemento.FilamentDiameter = 0.1;
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "FilamentDiameter") raised = true;
            };
            settingsMemento.FilamentDiameter = 0.1;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void MaterialPMemento_FilamentDiameter_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "FilamentDiameter") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.FilamentDiameter = SMaterialP.Default.FilamentDiameterMin - 1;
            Assert.AreEqual(SMaterialP.Default.FilamentDiameter, settingsMemento.FilamentDiameter);
        }

        [Test]
        public void MaterialPMemento_ExtrusionMultiplierP_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "ExtrusionMultiplierP") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.ExtrusionMultiplierP = 0.1;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void MaterialPMemento_ExtrusionMultiplierP_Same()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();

            Assert.AreEqual(false, raised);
            settingsMemento.ExtrusionMultiplierP = 0.1;
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "ExtrusionMultiplierP") raised = true;
            };
            settingsMemento.ExtrusionMultiplierP = 0.1;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void MaterialPMemento_ExtrusionMultiplierP_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "ExtrusionMultiplierP") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.ExtrusionMultiplierP = SMaterialP.Default.ExtrusionMultiplierPMin - 1;
            Assert.AreEqual(SMaterialP.Default.ExtrusionMultiplierP, settingsMemento.ExtrusionMultiplierP);
        }

        [Test]
        public void MaterialPMemento_TemperaturePrint_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "TemperaturePrint") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.TemperaturePrint = 50;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void MaterialPMemento_TemperaturePrint_same()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();

            Assert.AreEqual(false, raised);
            settingsMemento.TemperaturePrint = 50;
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "TemperaturePrint") raised = true;
            };
            settingsMemento.TemperaturePrint = 50;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void MaterialPMemento_TemperaturePrint_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "TemperaturePrint") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.TemperaturePrint = SMaterialP.Default.TemperaturePrintMin - 1;
            Assert.AreEqual(SMaterialP.Default.TemperaturePrint, settingsMemento.TemperaturePrint);
        }

        [Test]
        public void MaterialPMemento_TemperatureOnFLs_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "TemperatureOnFLs") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.TemperatureOnFLs = 50;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void MaterialPMemento_TemperatureOnFLs_Same()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();

            Assert.AreEqual(false, raised);
            settingsMemento.TemperatureOnFLs = 50;
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "TemperatureOnFLs") raised = true;
            };
            settingsMemento.TemperatureOnFLs = 50;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void MaterialPMemento_TemperatureOnFLs_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "TemperatureOnFLs") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.TemperatureOnFLs = SMaterialP.Default.TemperatureOnFLsMin - 1;
            Assert.AreEqual(SMaterialP.Default.TemperatureOnFLs, settingsMemento.TemperatureOnFLs);
        }

        [Test]
        public void MaterialPMemento_TemperatureWait_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "TemperatureWait") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.TemperatureWait = 50;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void MaterialPMemento_TemperatureWait_Same()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();

            Assert.AreEqual(false, raised);
            settingsMemento.TemperatureWait = 50;
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "TemperatureWait") raised = true;
            };
            settingsMemento.TemperatureWait = 50;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void MaterialPMemento_TemperatureWait_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "TemperatureWait") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.TemperatureWait = SMaterialP.Default.TemperatureWaitMin - 1;
            Assert.AreEqual(SMaterialP.Default.TemperatureWait, settingsMemento.TemperatureWait);
        }

        [Test]
        public void MaterialPMemento_BedHeatupTemperature_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "BedHeatupTemperature") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.BedHeatupTemperature = 50;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void MaterialPMemento_BedHeatupTemperature_Same()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();

            Assert.AreEqual(false, raised);
            settingsMemento.BedHeatupTemperature = 50;
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "BedHeatupTemperature") raised = true;
            };
            settingsMemento.BedHeatupTemperature = 50;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void MaterialPMemento_BedHeatupTemperature_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "BedHeatupTemperature") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.BedHeatupTemperature = SMaterialP.Default.BedHeatupTemperatureMin - 1;
            Assert.AreEqual(SMaterialP.Default.BedHeatupTemperature, settingsMemento.BedHeatupTemperature);
        }

        [Test]
        public void MaterialPMemento_BedHeatupOnFLs_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "BedHeatupOnFLs") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.BedHeatupOnFLs = 50;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void MaterialPMemento_BedHeatupOnFLs_Same()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();

            Assert.AreEqual(false, raised);
            settingsMemento.BedHeatupOnFLs = 50;
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "BedHeatupOnFLs") raised = true;
            };
            settingsMemento.BedHeatupOnFLs = 50;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void MaterialPMemento_BedHeatupOnFLs_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "BedHeatupOnFLs") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.BedHeatupOnFLs = SMaterialP.Default.BedHeatupOnFLsMin - 1;
            Assert.AreEqual(SMaterialP.Default.BedHeatupOnFLs, settingsMemento.BedHeatupOnFLs);
        }

        [Test]
        public void MaterialPMemento_FirstLayersHeightMM_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "FirstLayersHeightMM") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.FirstLayersHeightMM = 50;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void MaterialPMemento_FirstLayersHeightMM_Same()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();

            Assert.AreEqual(false, raised);
            settingsMemento.FirstLayersHeightMM = 50;
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "FirstLayersHeightMM") raised = true;
            };
            settingsMemento.FirstLayersHeightMM = 50;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void MaterialPMemento_FirstLayersHeightMM_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "BedHeatupOnFLs") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.FirstLayersHeightMM = SMaterialP.Default.FirstLayersHeightMMMin - 1;
            Assert.AreEqual(SMaterialP.Default.FirstLayersHeightMM, settingsMemento.FirstLayersHeightMM);
        }

        [Test]
        public void MaterialPMemento_RetractionSpeed_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "RetractionSpeed") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.RetractionSpeed = 50;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void MaterialPMemento_RetractionSpeed_Same()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();

            Assert.AreEqual(false, raised);
            settingsMemento.RetractionSpeed = 50;
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "RetractionSpeed") raised = true;
            };
            settingsMemento.RetractionSpeed = 50;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void MaterialPMemento_RetractionSpeed_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "RetractionSpeed") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.RetractionSpeed = SMaterialP.Default.RetractionSpeedMin - 1;
            Assert.AreEqual(SMaterialP.Default.RetractionSpeed, settingsMemento.RetractionSpeed);
        }

        [Test]
        public void MaterialPMemento_RetractionLengthOnTravel_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "RetractionLengthOnTravel") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.RetractionLengthOnTravel = 50;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void MaterialPMemento_RetractionLengthOnTravel_Same()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();

            Assert.AreEqual(false, raised);
            settingsMemento.RetractionLengthOnTravel = 50;
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "RetractionLengthOnTravel") raised = true;
            };
            settingsMemento.RetractionLengthOnTravel = 50;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void MaterialPMemento_RetractionLengthOnTravel_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "RetractionLengthOnTravel") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.RetractionLengthOnTravel = SMaterialP.Default.RetractionLengthOnTravelMin - 1;
            Assert.AreEqual(SMaterialP.Default.RetractionLengthOnTravel, settingsMemento.RetractionLengthOnTravel);
        }

        [Test]
        public void MaterialPMemento_RetractionLengthOnChangingExtruder_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "RetractionLengthOnChangingExtruder") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.RetractionLengthOnChangingExtruder = 50;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void MaterialPMemento_RetractionLengthOnChangingExtruder_Same()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();

            Assert.AreEqual(false, raised);
            settingsMemento.RetractionLengthOnChangingExtruder = 50;
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "RetractionLengthOnChangingExtruder") raised = true;
            };
            settingsMemento.RetractionLengthOnChangingExtruder = 50;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void MaterialPMemento_RetractionLengthOnChangingExtruder_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "RetractionLengthOnChangingExtruder") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.RetractionLengthOnChangingExtruder = SMaterialP.Default.RetractionLengthOnChangingExtruderMin - 1;
            Assert.AreEqual(SMaterialP.Default.RetractionLengthOnChangingExtruder, settingsMemento.RetractionLengthOnChangingExtruder);
        }

        [Test]
        public void MaterialPMemento_RetractMinimumTravelNoCrossing_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "RetractMinimumTravelNoCrossing") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.RetractMinimumTravelNoCrossing = 50;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void MaterialPMemento_RetractMinimumTravelNoCrossing_Same()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();

            Assert.AreEqual(false, raised);
            settingsMemento.RetractMinimumTravelNoCrossing = 50;
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "RetractMinimumTravelNoCrossing") raised = true;
            };
            settingsMemento.RetractMinimumTravelNoCrossing = 50;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void MaterialPMemento_RetractMinimumTravelNoCrossing_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "RetractMinimumTravelNoCrossing") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.RetractMinimumTravelNoCrossing = SMaterialP.Default.RetractMinimumTravelNoCrossingMin - 1;
            Assert.AreEqual(SMaterialP.Default.RetractMinimumTravelNoCrossing, settingsMemento.RetractMinimumTravelNoCrossing);
        }

        [Test]
        public void MaterialPMemento_RetractMinimumTravelOnCrossing_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "RetractMinimumTravelOnCrossing") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.RetractMinimumTravelOnCrossing = 50;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void MaterialPMemento_RetractMinimumTravelOnCrossing_Same()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();

            Assert.AreEqual(false, raised);
            settingsMemento.RetractMinimumTravelOnCrossing = 50;
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "RetractMinimumTravelOnCrossing") raised = true;
            };
            settingsMemento.RetractMinimumTravelOnCrossing = 50;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void MaterialPMemento_RetractMinimumTravelOnCrossing_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "RetractMinimumTravelOnCrossing") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.RetractMinimumTravelOnCrossing = SMaterialP.Default.RetractMinimumTravelOnCrossingMin - 1;
            Assert.AreEqual(SMaterialP.Default.RetractMinimumTravelOnCrossing, settingsMemento.RetractMinimumTravelOnCrossing);
        }

        [Test]
        public void MaterialPMemento_CoastAtEndLength_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "CoastAtEndLength") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.CoastAtEndLength = 50;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void MaterialPMemento_CoastAtEndLength_Same()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();

            Assert.AreEqual(false, raised);
            settingsMemento.CoastAtEndLength = 50;
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "CoastAtEndLength") raised = true;
            };
            settingsMemento.CoastAtEndLength = 50;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void MaterialPMemento_CoastAtEndLength_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "CoastAtEndLength") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.CoastAtEndLength = SMaterialP.Default.CoastAtEndLengthMin - 1;
            Assert.AreEqual(SMaterialP.Default.CoastAtEndLength, settingsMemento.CoastAtEndLength);
        }

        [Test]
        public void MaterialPMemento_WipeNozzleLength_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "WipeNozzleLength") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.WipeNozzleLength = 50;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void MaterialPMemento_WipeNozzleLength_Same()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();

            Assert.AreEqual(false, raised);
            settingsMemento.WipeNozzleLength = 50;
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "WipeNozzleLength") raised = true;
            };
            settingsMemento.WipeNozzleLength = 50;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void MaterialPMemento_WipeNozzleLength_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "WipeNozzleLength") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.WipeNozzleLength = SMaterialP.Default.WipeNozzleLengthMin - 1;
            Assert.AreEqual(SMaterialP.Default.WipeNozzleLength, settingsMemento.WipeNozzleLength);
        }

        [Test]
        public void MaterialPMemento_ExtraSpeedP_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "ExtraSpeedP") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.ExtraSpeedP = 50;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void MaterialPMemento_ExtraSpeedP_Same()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();

            Assert.AreEqual(false, raised);
            settingsMemento.ExtraSpeedP = 50;
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "ExtraSpeedP") raised = true;
            };
            settingsMemento.ExtraSpeedP = 50;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void MaterialPMemento_ExtraSpeedP_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "ExtraSpeedP") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.ExtraSpeedP = SMaterialP.Default.ExtraSpeedPMin - 1;
            Assert.AreEqual(SMaterialP.Default.ExtraSpeedP, settingsMemento.ExtraSpeedP);
        }

        [Test]
        public void MaterialPMemento_ExtraLengthP_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "ExtraLengthP") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.ExtraLengthP = 50;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void MaterialPMemento_ExtraLengthP_Same()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();

            Assert.AreEqual(false, raised);
            settingsMemento.ExtraLengthP = 50;
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "ExtraLengthP") raised = true;
            };
            settingsMemento.ExtraLengthP = 50;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void MaterialPMemento_ExtraLengthP_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "ExtraLengthP") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.ExtraLengthP = SMaterialP.Default.ExtraLengthPMin - 1;
            Assert.AreEqual(SMaterialP.Default.ExtraLengthP, settingsMemento.ExtraLengthP);
        }

        [Test]
        public void MaterialPMemento_Density_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Density") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.Density = 50;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void MaterialPMemento_Density_Same()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();

            Assert.AreEqual(false, raised);
            settingsMemento.Density = 50;
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Density") raised = true;
            };
            settingsMemento.Density = 50;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void MaterialPMemento_Density_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Density") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.Density = SMaterialP.Default.DensityMin - 1;
            Assert.AreEqual(SMaterialP.Default.Density, settingsMemento.Density);
        }

        [Test]
        public void MaterialPMemento_CostPerSpool_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "CostPerSpool") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.CostPerSpool = 50;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void MaterialPMemento_CostPerSpool_Same()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();

            Assert.AreEqual(false, raised);
            settingsMemento.CostPerSpool = 50;
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "CostPerSpool") raised = true;
            };
            settingsMemento.CostPerSpool = 50;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void MaterialPMemento_CostPerSpool_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "CostPerSpool") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.CostPerSpool = SMaterialP.Default.CostPerSpoolMin - 1;
            Assert.AreEqual(SMaterialP.Default.CostPerSpool, settingsMemento.CostPerSpool);
        }

        [Test]
        public void MaterialPMemento_SpoolWeight_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "SpoolWeight") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.SpoolWeight = 50;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void MaterialPMemento_SpoolWeight_Same()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();

            Assert.AreEqual(false, raised);
            settingsMemento.SpoolWeight = 50;
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "SpoolWeight") raised = true;
            };
            settingsMemento.SpoolWeight = 50;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void MaterialPMemento_SpoolWeight_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "SpoolWeight") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.SpoolWeight = SMaterialP.Default.SpoolWeightMin - 1;
            Assert.AreEqual(SMaterialP.Default.SpoolWeight, settingsMemento.SpoolWeight);
        }

        [Test]
        public void MaterialPMemento_Description_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Description") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.Description = "aa";
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void MaterialPMemento_Description_Same()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();

            Assert.AreEqual(false, raised);
            settingsMemento.Description = "aa";
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Description") raised = true;
            };
            settingsMemento.Description = "aa";
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void MaterialPMemento_Version_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Version") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.Version = 1;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void MaterialPMemento_Version_Same()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();

            Assert.AreEqual(false, raised);
            settingsMemento.Version = 1;
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Version") raised = true;
            };
            settingsMemento.Version = 1;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void MaterialPMemento_IsAnisoprintApproved_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "IsAnisoprintApproved") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.IsAnisoprintApproved = true;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void MaterialPMemento_IsAnisoprintApproved_Same()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();

            Assert.AreEqual(false, raised);
            settingsMemento.IsAnisoprintApproved = true;
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "IsAnisoprintApproved") raised = true;
            };
            settingsMemento.IsAnisoprintApproved = true;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void MaterialPMemento_ZHopP_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "ZhopP") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.ZhopP = 0.5;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void MaterialPMemento_ZHopP_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "ZhopP") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.ZhopP = SMaterialP.Default.ZHopPMin - 1;
            Assert.AreEqual(SMaterialP.Default.ZHopP, settingsMemento.ZhopP);
        }


        [Test]
        public void MaterialPMemento_EnablePrintCooling()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "EnablePrintCooling") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.EnablePrintCooling = true;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void MaterialPMemento_Same()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();

            Assert.AreEqual(false, raised);
            settingsMemento.EnablePrintCooling = true;
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "EnablePrintCooling") raised = true;
            };
            settingsMemento.EnablePrintCooling = true;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void MaterialPMemento_FirstLayersFanHeight_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "FirstLayersFanHeight") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.FirstLayersFanHeight = 0.5;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void MaterialPMemento_FirstLayersFanHeight_Same()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();

            Assert.AreEqual(false, raised);
            settingsMemento.FirstLayersFanHeight = 0.5;
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "FirstLayersFanHeight") raised = true;
            };
            settingsMemento.FirstLayersFanHeight = 0.5;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void MaterialPMemento_FirstLayersFanHeight_NotInAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "FirstLayersFanHeight") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.FirstLayersFanHeight = -2.5;
            Assert.AreEqual(SMaterialP.Default.FirstLayersFanHeight, settingsMemento.FirstLayersFanHeight);
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void MaterialPMemento_FirstLayersFanSpeed_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "FirstLayersFanSpeed") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.FirstLayersFanSpeed = 50;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void MaterialPMemento_FirstLayersFanSpeed_Same()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();

            Assert.AreEqual(false, raised);
            settingsMemento.FirstLayersFanSpeed = 50;
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "FirstLayersFanSpeed") raised = true;
            };
            settingsMemento.FirstLayersFanSpeed = 50;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void MaterialPMemento_FirstLayersFanSpeed_NotInAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "FirstLayersFanSpeed") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.FirstLayersFanSpeed = -50;
            Assert.AreEqual(SMaterialP.Default.FirstLayersFanSpeed, settingsMemento.FirstLayersFanSpeed);
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void MaterialPMemento_MinimumFanSpeed_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "RegularFanSpeed") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.MaximumFanSpeed = 100;
            settingsMemento.RegularFanSpeed = 50;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void MaterialPMemento_MinimumFanSpeed_InAcceptableRange_Same()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();

            Assert.AreEqual(false, raised);
            settingsMemento.MaximumFanSpeed = 100;
            settingsMemento.RegularFanSpeed = 50;
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "RegularFanSpeed") raised = true;
            };
            settingsMemento.RegularFanSpeed = 50;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void MaterialPMemento_MinimumFanSpeed_NotInAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "RegularFanSpeed") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.MaximumFanSpeed = 100;
            settingsMemento.RegularFanSpeed = -50;
            Assert.AreEqual(SMaterialP.Default.MinimumFanSpeed, settingsMemento.RegularFanSpeed);
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void MaterialPMemento_MaximumFanSpeed_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "MaximumFanSpeed") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.MaximumFanSpeed = 100;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void MaterialPMemento_MaximumFanSpeed_Same()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();

            Assert.AreEqual(false, raised);
            settingsMemento.MaximumFanSpeed = 100;
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "MaximumFanSpeed") raised = true;
            };
            settingsMemento.MaximumFanSpeed = 100;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void MaterialPMemento_MaximumFanSpeed_NotInAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "MaximumFanSpeed") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.MaximumFanSpeed = -100;
            Assert.AreEqual(SMaterialP.Default.MaximumFanSpeed, settingsMemento.MaximumFanSpeed);
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void MaterialPMemento_MinLayerTimeForCooling_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "MinLayerTimeForCooling") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.MinLayerTimeForCooling = 100;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void MaterialPMemento_MinLayerTimeForCooling_Same()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();

            Assert.AreEqual(false, raised);
            settingsMemento.MinLayerTimeForCooling = 100;
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "MinLayerTimeForCooling") raised = true;
            };
            settingsMemento.MinLayerTimeForCooling = 100;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void MaterialPMemento_MinLayerTimeForCooling_NotInAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "MinLayerTimeForCooling") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.MinLayerTimeForCooling = -100;
            Assert.AreEqual(SMaterialP.Default.MinLayerTimeForCooling, settingsMemento.MinLayerTimeForCooling);
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void MaterialPMemento_GUID_Same()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();

            Assert.AreEqual(false, raised);
            var guid = Guid.NewGuid();
            settingsMemento.GUID = guid;
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "GUID") raised = true;
            };
            settingsMemento.GUID = guid;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void MaterialPMemento_ParentGUID_Same()
        {
            var raised = false;

            var settingsMemento = new MaterialPMemento();

            Assert.AreEqual(false, raised);
            var guid = Guid.NewGuid();
            settingsMemento.ParentGUID = guid;
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "ParentGUID") raised = true;
            };
            settingsMemento.ParentGUID = guid;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void MaterialPMemento_EqualsWithoutNameVersionApproved_DifferentNames()
        {
            var settingsMemento1 = new MaterialPMemento();
            settingsMemento1.SetDefault();

            var settingsMemento2 = new MaterialPMemento();
            settingsMemento2.SetDefault();
            settingsMemento2.Name = "NewName";
            settingsMemento2.Version = 2;

            Assert.IsTrue(settingsMemento1.EqualsWithoutNameVersionApproved((ISettingsMemento)settingsMemento2));
        }

        [Test]
        public void MaterialPMemento_EqualsWithoutNameVersionApproved_DifferentFields()
        {
            var settingsMemento1 = new MaterialPMemento();
            settingsMemento1.SetDefault();

            var settingsMemento2 = new MaterialPMemento();
            settingsMemento2.SetDefault();
            settingsMemento2.Name = "NewName";
            settingsMemento2.Version = 2;
            settingsMemento2.Density = 2;

            Assert.IsFalse(settingsMemento1.EqualsWithoutNameVersionApproved((ISettingsMemento)settingsMemento2));
        }

        [Test]
        public void MaterialPMemento_EqualsWithoutNameVersionApproved_NotMaterialP()
        {

            var settingsMemento2 = new MaterialPMemento();
            settingsMemento2.SetDefault();
            settingsMemento2.Name = "NewName";
            settingsMemento2.Version = 2;
            settingsMemento2.Density = 2;

            ISettingsMemento settingsMemento1 = null;

            Assert.Throws<InvalidCastException>(() => settingsMemento2.EqualsWithoutNameVersionApproved(settingsMemento1));
        }

        [Test]
        public void MaterialPMemento_FillFromAnother_DifferentNames()
        {
            var settingsMemento1 = new MaterialPMemento();
            settingsMemento1.SetDefault();

            var settingsMemento2 = new MaterialPMemento();
            settingsMemento2.SetDefault();
            settingsMemento2.Name = "NewName";
            settingsMemento2.Version = 2;

            settingsMemento1.FillFromAnother((ISettingsMemento)settingsMemento2);

            Assert.IsTrue(settingsMemento1.Equals(settingsMemento2));
        }

        [Test]
        public void MaterialPMemento_FillFromAnother_NotMaterialP()
        {

            var settingsMemento2 = new MaterialPMemento();
            settingsMemento2.SetDefault();

            ISettingsMemento settingsMemento1 = null;

            Assert.Throws<InvalidCastException>(() => settingsMemento2.FillFromAnother(settingsMemento1));
        }
    }
}