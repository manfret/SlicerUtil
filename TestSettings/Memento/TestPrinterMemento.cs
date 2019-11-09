using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using Settings;
using Settings.Memento;
using Settings.ValidValues;

namespace TestSettings.Memento
{
    [TestFixture]
    public class TestPrinterMemento
    {
        [Test]
        public void PrinterMemento_Name_InAcceptableRange()
        {
            var raised = false;
            var raisedChanging = false;

            var settingsMemento = new PrinterMemento((ISettingsFactory) null);
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
        public void PrinterMemento_Name_Same()
        {
            var raised = false;
            var raisedChanging = false;

            var settingsMemento = new PrinterMemento((ISettingsFactory)null);

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
        public void PrinterMemento_Name_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new PrinterMemento((ISettingsFactory)null);
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Name") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.Name = string.Empty;
            Assert.AreEqual(SPrinter.Default.Name, settingsMemento.Name);
        }

        [Test]
        public void PrinterMemento_TravelSpeedXY_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new PrinterMemento((ISettingsFactory)null);
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "TravelSpeedXY") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.TravelSpeedXY = 10;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void PrinterMemento_TravelSpeedXY_Same()
        {
            var raised = false;

            var settingsMemento = new PrinterMemento((ISettingsFactory)null);

            Assert.AreEqual(false, raised);
            settingsMemento.TravelSpeedXY = 10;
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "TravelSpeedXY") raised = true;
            };
            settingsMemento.TravelSpeedXY = 10;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void PrinterMemento_TravelSpeedXY_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new PrinterMemento((ISettingsFactory)null);
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "TravelSpeedXY") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.TravelSpeedXY = SPrinter.Default.TravelSpeedXYMin - 1;
            Assert.AreEqual(SPrinter.Default.TravelSpeedXY, settingsMemento.TravelSpeedXY);
        }

        [Test]
        public void PrinterMemento_TravelSpeedZ_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new PrinterMemento((ISettingsFactory)null);
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "TravelSpeedZ") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.TravelSpeedZ = 10;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void PrinterMemento_TravelSpeedZ_Same()
        {
            var raised = false;

            var settingsMemento = new PrinterMemento((ISettingsFactory)null);

            Assert.AreEqual(false, raised);
            settingsMemento.TravelSpeedZ = 10;
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "TravelSpeedZ") raised = true;
            };
            settingsMemento.TravelSpeedZ = 10;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void PrinterMemento_TravelSpeedZ_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new PrinterMemento((ISettingsFactory)null);
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "TravelSpeedZ") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.TravelSpeedZ = SPrinter.Default.TravelSpeedZMin - 1;
            Assert.AreEqual(SPrinter.Default.TravelSpeedZ, settingsMemento.TravelSpeedZ);
        }

        [Test]
        public void PrinterMemento_Width_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new PrinterMemento((ISettingsFactory)null);
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Width") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.Width = 10;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void PrinterMemento_Width_Same()
        {
            var raised = false;

            var settingsMemento = new PrinterMemento((ISettingsFactory)null);

            Assert.AreEqual(false, raised);
            settingsMemento.Width = 10;
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Width") raised = true;
            };
            settingsMemento.Width = 10;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void PrinterMemento_Width_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new PrinterMemento((ISettingsFactory)null);
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Width") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.Width = SPrinter.Default.WidthMin - 1;
            Assert.AreEqual(SPrinter.Default.Width, settingsMemento.Width);
        }

        [Test]
        public void PrinterMemento_Length_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new PrinterMemento((ISettingsFactory)null);
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Length") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.Length = 10;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void PrinterMemento_Length_Same()
        {
            var raised = false;

            var settingsMemento = new PrinterMemento((ISettingsFactory)null);

            Assert.AreEqual(false, raised);
            settingsMemento.Length = 10;
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Length") raised = true;
            };
            settingsMemento.Length = 10;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void PrinterMemento_Length_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new PrinterMemento((ISettingsFactory)null);
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Length") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.Length = SPrinter.Default.LengthMin - 1;
            Assert.AreEqual(SPrinter.Default.Length, settingsMemento.Length);
        }

        [Test]
        public void PrinterMemento_Height_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new PrinterMemento((ISettingsFactory)null);
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Height") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.Height = 10;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void PrinterMemento_Height_Same()
        {
            var raised = false;

            var settingsMemento = new PrinterMemento((ISettingsFactory)null);

            Assert.AreEqual(false, raised);
            settingsMemento.Height = 10;
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Height") raised = true;
            };
            settingsMemento.Height = 10;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void PrinterMemento_Height_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new PrinterMemento((ISettingsFactory)null);
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Height") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.Height = SPrinter.Default.HeightMin - 1;
            Assert.AreEqual(SPrinter.Default.Height, settingsMemento.Height);
        }

        [Test]
        public void PrinterMemento_HasHeatedTable_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new PrinterMemento((ISettingsFactory)null);
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "HasHeatedTable") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.HasHeatedTable = true;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void PrinterMemento_HasHeatedTable_Same()
        {
            var raised = false;

            var settingsMemento = new PrinterMemento((ISettingsFactory)null);

            Assert.AreEqual(false, raised);
            settingsMemento.HasHeatedTable = true;
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "HasHeatedTable") raised = true;
            };
            settingsMemento.HasHeatedTable = true;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void PrinterMemento_AdditionalRetractMM_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new PrinterMemento((ISettingsFactory)null);
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "AdditionalRetractMM") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.AdditionalRetractMM = 10;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void PrinterMemento_AdditionalRetractMM_Same()
        {
            var raised = false;

            var settingsMemento = new PrinterMemento((ISettingsFactory)null);

            Assert.AreEqual(false, raised);
            settingsMemento.AdditionalRetractMM = 10;
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "AdditionalRetractMM") raised = true;
            };
            settingsMemento.AdditionalRetractMM = 10;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void PrinterMemento_AdditionalRetractMM_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new PrinterMemento((ISettingsFactory)null);
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "AdditionalRetractMM") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.AdditionalRetractMM = SPrinter.Default.AdditionalRetractMMMin - 1;
            Assert.AreEqual(SPrinter.Default.AdditionalRetractMM, settingsMemento.AdditionalRetractMM);
        }

        [Test]
        public void PrinterMemento_OnChangeExtruderUpMM_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new PrinterMemento((ISettingsFactory)null);
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "OnChangeExtruderUpMM") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.OnChangeExtruderUpMM = 10;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void PrinterMemento_OnChangeExtruderUpMM_Same()
        {
            var raised = false;

            var settingsMemento = new PrinterMemento((ISettingsFactory)null);

            Assert.AreEqual(false, raised);
            settingsMemento.OnChangeExtruderUpMM = 10;
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "OnChangeExtruderUpMM") raised = true;
            };
            settingsMemento.OnChangeExtruderUpMM = 10;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void PrinterMemento_OnChangeExtruderUpMM_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new PrinterMemento((ISettingsFactory)null);
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "OnChangeExtruderUpMM") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.OnChangeExtruderUpMM = SPrinter.Default.OnChangeExtruderUpMMMin - 1;
            Assert.AreEqual(SPrinter.Default.OnChangeExtruderUpMM, settingsMemento.OnChangeExtruderUpMM);
        }

        [Test]
        public void PrinterMemento_StartGCode_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new PrinterMemento((ISettingsFactory)null);
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "StartGCode") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.StartGCode = "bzz";
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void PrinterMemento_StartGCode_Same()
        {
            var raised = false;

            var settingsMemento = new PrinterMemento((ISettingsFactory)null);

            Assert.AreEqual(false, raised);
            settingsMemento.StartGCode = "bzz";
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "StartGCode") raised = true;
            };
            settingsMemento.StartGCode = "bzz";
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void PrinterMemento_EndGCode_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new PrinterMemento((ISettingsFactory)null);
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "EndGCode") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.EndGCode = "bzz";
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void PrinterMemento_EndGCode_Same()
        {
            var raised = false;

            var settingsMemento = new PrinterMemento((ISettingsFactory)null);

            Assert.AreEqual(false, raised);
            settingsMemento.EndGCode = "bzz";
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "EndGCode") raised = true;
            };
            settingsMemento.EndGCode = "bzz";
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void PrinterMemento_Description_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new PrinterMemento((ISettingsFactory)null);
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Description") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.Description = "bzz";
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void PrinterMemento_Description_Same()
        {
            var raised = false;

            var settingsMemento = new PrinterMemento((ISettingsFactory)null);

            Assert.AreEqual(false, raised);
            settingsMemento.Description = "bzz";
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Description") raised = true;
            };
            settingsMemento.Description = "bzz";
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void PrinterMemento_IsAnisoprintApproved_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new PrinterMemento((ISettingsFactory)null);
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "IsAnisoprintApproved") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.IsAnisoprintApproved = true;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void PrinterMemento_IsAnisoprintApproved_Same()
        {
            var raised = false;

            var settingsMemento = new PrinterMemento((ISettingsFactory)null);

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
        public void PrinterMemento_Version_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new PrinterMemento((ISettingsFactory)null);
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Version") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.Version = 1;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void PrinterMemento_Version_Same()
        {
            var raised = false;

            var settingsMemento = new PrinterMemento((ISettingsFactory)null);

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
        public void PrinterMemento_ExtrudersF_AddExtruderP()
        {
            var raisedP = false;
            var raisedF = false;

            var settingsMemento = new PrinterMemento((ISettingsFactory)null);
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "ExtrudersP") raisedP = true;
                if (args.PropertyName == "ExtrudersPF") raisedF = true;
            };

            Assert.AreEqual(false, raisedP);
            Assert.AreEqual(false, raisedF);
            var newExtruder = settingsMemento.AddExtruderPF();
            Assert.AreEqual(false, raisedP);
            Assert.AreEqual(true, raisedF);

            Assert.AreEqual(0, newExtruder.ExtruderIndex);
        }

        [Test]
        public void PrinterMemento_RemovePF_AddExtruderP()
        {
            var raisedP = false;
            var raisedF = false;

            var settingsMemento = new PrinterMemento((ISettingsFactory)null);

            var newExtruder = settingsMemento.AddExtruderPF();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "ExtrudersP") raisedP = true;
                if (args.PropertyName == "ExtrudersPF") raisedF = true;
            };
            Assert.AreEqual(false, raisedP);
            Assert.AreEqual(false, raisedF);
            settingsMemento.RemoveExtruder(newExtruder);
            Assert.AreEqual(false, raisedP);
            Assert.AreEqual(true, raisedF);

            Assert.AreEqual(0, settingsMemento.ExtrudersPF.Count());
        }

        [Test]
        public void PrinterMemento_UseAccelerations_Set()
        {
            var useAccelerationRaised = false;
            var accelerationSettingsMemento = new PrinterMemento((ISettingsFactory)null);
            accelerationSettingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "UseAccelerations") useAccelerationRaised = true;
            };

            Assert.AreEqual(false, useAccelerationRaised);
            accelerationSettingsMemento.UseAccelerations = true;
            Assert.AreEqual(true, useAccelerationRaised);
        }

        [Test]
        public void PrinterMemento_UseAccelerations_Same()
        {
            var useAccelerationRaised = false;
            var accelerationSettingsMemento = new PrinterMemento((ISettingsFactory)null);

            Assert.AreEqual(false, useAccelerationRaised);
            accelerationSettingsMemento.UseAccelerations = true;
            accelerationSettingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "UseAccelerations") useAccelerationRaised = true;
            };
            accelerationSettingsMemento.UseAccelerations = true;
            Assert.AreEqual(false, useAccelerationRaised);
        }


        [Test]
        public void PrinterMemento_Inset0Acceleration_Set_NotInAcceptableRange()
        {
            var inset0AccelerationRaised = false;
            var accelerationSettingsMemento = new PrinterMemento((ISettingsFactory)null);
            accelerationSettingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Inset0Acceleration") inset0AccelerationRaised = true;
            };

            Assert.AreEqual(false, inset0AccelerationRaised);
            accelerationSettingsMemento.Inset0Acceleration = -10;
            Assert.AreEqual(true, inset0AccelerationRaised);
            Assert.AreEqual(SPrinter.Default.Inset0Acceleration, accelerationSettingsMemento.Inset0Acceleration);
        }

        [Test]
        public void PrinterMemento_Inset0Acceleration_Set_InAcceptableRange()
        {
            var inset0AccelerationRaised = false;
            var accelerationSettingsMemento = new PrinterMemento((ISettingsFactory)null);
            accelerationSettingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Inset0Acceleration") inset0AccelerationRaised = true;
            };

            Assert.AreEqual(false, inset0AccelerationRaised);
            accelerationSettingsMemento.Inset0Acceleration = 50;
            Assert.AreEqual(true, inset0AccelerationRaised);
            Assert.AreEqual(50, accelerationSettingsMemento.Inset0Acceleration);
        }

        [Test]
        public void PrinterMemento_Inset0Acceleration_Set_Same()
        {
            var inset0AccelerationRaised = false;
            var accelerationSettingsMemento = new PrinterMemento((ISettingsFactory)null);

            Assert.AreEqual(false, inset0AccelerationRaised);
            accelerationSettingsMemento.Inset0Acceleration = 50;
            accelerationSettingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Inset0Acceleration") inset0AccelerationRaised = true;
            };
            accelerationSettingsMemento.Inset0Acceleration = 50;
            Assert.AreEqual(false, inset0AccelerationRaised);
        }

        [Test]
        public void PrinterMemento_OthersAcceleration_Set_NotInAcceptableRange()
        {
            var inset0AccelerationRaised = false;
            var accelerationSettingsMemento = new PrinterMemento((ISettingsFactory)null);
            accelerationSettingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "OthersAcceleration") inset0AccelerationRaised = true;
            };

            Assert.AreEqual(false, inset0AccelerationRaised);
            accelerationSettingsMemento.OthersAcceleration = -10;
            Assert.AreEqual(true, inset0AccelerationRaised);
            Assert.AreEqual(SPrinter.Default.OtherAcceleration, accelerationSettingsMemento.OthersAcceleration);
        }

        [Test]
        public void PrinterMemento_OthersAcceleration_Set_InAcceptableRange()
        {
            var inset0AccelerationRaised = false;
            var accelerationSettingsMemento = new PrinterMemento((ISettingsFactory)null);
            accelerationSettingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "OthersAcceleration") inset0AccelerationRaised = true;
            };

            Assert.AreEqual(false, inset0AccelerationRaised);
            accelerationSettingsMemento.OthersAcceleration = 50;
            Assert.AreEqual(true, inset0AccelerationRaised);
            Assert.AreEqual(50, accelerationSettingsMemento.OthersAcceleration);
        }

        [Test]
        public void PrinterMemento_OthersAcceleration_Set_Same()
        {
            var inset0AccelerationRaised = false;
            var accelerationSettingsMemento = new PrinterMemento((ISettingsFactory)null);

            Assert.AreEqual(false, inset0AccelerationRaised);
            accelerationSettingsMemento.OthersAcceleration = 50;
            accelerationSettingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "OthersAcceleration") inset0AccelerationRaised = true;
            };

            accelerationSettingsMemento.OthersAcceleration = 50;
            Assert.AreEqual(false, inset0AccelerationRaised);
            Assert.AreEqual(50, accelerationSettingsMemento.OthersAcceleration);
        }

        [Test]
        public void PrinterMemento_UseJerks_Set()
        {
            var propertyChangedRaised = false;
            var accelerationSettingsMemento = new PrinterMemento((ISettingsFactory)null);
            accelerationSettingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "UseJerks") propertyChangedRaised = true;
            };

            Assert.AreEqual(false, propertyChangedRaised);
            accelerationSettingsMemento.UseJerks = true;
            Assert.AreEqual(true, propertyChangedRaised);
        }

        [Test]
        public void PrinterMemento_UseJerks_Set_Same()
        {
            var propertyChangedRaised = false;
            var accelerationSettingsMemento = new PrinterMemento((ISettingsFactory)null);

            Assert.AreEqual(false, propertyChangedRaised);
            accelerationSettingsMemento.UseJerks = true;
            accelerationSettingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "UseJerks") propertyChangedRaised = true;
            };
            accelerationSettingsMemento.UseJerks = true;
            Assert.AreEqual(false, propertyChangedRaised);
        }

        [Test]
        public void PrinterMemento_Inset0Jerk_Set_NotInAcceptableRange()
        {
            var raised = false;
            var accelerationSettingsMemento = new PrinterMemento((ISettingsFactory)null);
            accelerationSettingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Inset0Jerk") raised = true;
            };

            Assert.AreEqual(false, raised);
            accelerationSettingsMemento.Inset0Jerk = -10;
            Assert.AreEqual(true, raised);
            Assert.AreEqual(SPrinter.Default.Inset0Jerk, accelerationSettingsMemento.Inset0Jerk);
        }

        [Test]
        public void PrinterMemento_Inset0Jerk_Set_InAcceptableRange()
        {
            var raised = false;
            var accelerationSettingsMemento = new PrinterMemento((ISettingsFactory)null);
            accelerationSettingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Inset0Jerk") raised = true;
            };

            Assert.AreEqual(false, raised);
            accelerationSettingsMemento.Inset0Jerk = 50;
            Assert.AreEqual(true, raised);
            Assert.AreEqual(50, accelerationSettingsMemento.Inset0Jerk);
        }

        [Test]
        public void PrinterMemento_Inset0Jerk_Set_Same()
        {
            var raised = false;
            var accelerationSettingsMemento = new PrinterMemento((ISettingsFactory)null);

            Assert.AreEqual(false, raised);
            accelerationSettingsMemento.Inset0Jerk = 50;
            accelerationSettingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Inset0Jerk") raised = true;
            };
            accelerationSettingsMemento.Inset0Jerk = 50;
            Assert.AreEqual(false, raised);
            Assert.AreEqual(50, accelerationSettingsMemento.Inset0Jerk);
        }

        [Test]
        public void PrinterMemento_OthersJerk_Set_NotInAcceptableRange()
        {
            var raised = false;
            var accelerationSettingsMemento = new PrinterMemento((ISettingsFactory)null);
            accelerationSettingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "OthersJerk") raised = true;
            };

            Assert.AreEqual(false, raised);
            accelerationSettingsMemento.OthersJerk = -10;
            Assert.AreEqual(true, raised);
            Assert.AreEqual(SPrinter.Default.OtherJerk, accelerationSettingsMemento.OthersJerk);
        }

        [Test]
        public void PrinterMemento_OthersJerk_Set_InAcceptableRange()
        {
            var raised = false;
            var accelerationSettingsMemento = new PrinterMemento((ISettingsFactory)null);
            accelerationSettingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "OthersJerk") raised = true;
            };

            Assert.AreEqual(false, raised);
            accelerationSettingsMemento.OthersJerk = 50;
            Assert.AreEqual(true, raised);
            Assert.AreEqual(50, accelerationSettingsMemento.OthersJerk);
        }

        [Test]
        public void PrinterMemento_OthersJerk_Set_Same()
        {
            var raised = false;
            var accelerationSettingsMemento = new PrinterMemento((ISettingsFactory)null);

            Assert.AreEqual(false, raised);
            accelerationSettingsMemento.OthersJerk = 50;
            accelerationSettingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "OthersJerk") raised = true;
            };
            accelerationSettingsMemento.OthersJerk = 50;
            Assert.AreEqual(false, raised);
            Assert.AreEqual(50, accelerationSettingsMemento.OthersJerk);
        }

        [Test]
        public void PrinterMemento_HomeXPosition_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new PrinterMemento((ISettingsFactory)null);
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "HomeXPosition") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.HomeXPosition = 10;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void PrinterMemento_HomeXPosition_Same()
        {
            var raised = false;

            var settingsMemento = new PrinterMemento((ISettingsFactory)null);

            Assert.AreEqual(false, raised);
            settingsMemento.HomeXPosition = 10;
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "HomeXPosition") raised = true;
            };
            settingsMemento.HomeXPosition = 10;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void PrinterMemento_HomeXPosition_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new PrinterMemento((ISettingsFactory)null);
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "HomeXPosition") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.HomeXPosition = SPrinter.Default.HomeXPositionMin - 1;
            Assert.AreEqual(SPrinter.Default.HomeXPosition, settingsMemento.HomeXPosition);
        }

        [Test]
        public void PrinterMemento_HomeYPosition_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new PrinterMemento((ISettingsFactory)null);
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "HomeYPosition") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.HomeYPosition = 10;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void PrinterMemento_HomeYPosition_Same()
        {
            var raised = false;

            var settingsMemento = new PrinterMemento((ISettingsFactory)null);

            Assert.AreEqual(false, raised);
            settingsMemento.HomeYPosition = 10;
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "HomeYPosition") raised = true;
            };
            settingsMemento.HomeYPosition = 10;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void PrinterMemento_HomeYPosition_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new PrinterMemento((ISettingsFactory)null);
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "HomeYPosition") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.HomeYPosition = SPrinter.Default.HomeYPositionMin - 1;
            Assert.AreEqual(SPrinter.Default.HomeYPosition, settingsMemento.HomeYPosition);
        }

        [Test]
        public void PrinterMemento_HomeZPosition_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new PrinterMemento((ISettingsFactory)null);
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "HomeZPosition") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.HomeZPosition = 10;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void PrinterMemento_HomeZPosition_Same()
        {
            var raised = false;

            var settingsMemento = new PrinterMemento((ISettingsFactory)null);

            Assert.AreEqual(false, raised);
            settingsMemento.HomeZPosition = 10;
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "HomeZPosition") raised = true;
            };
            settingsMemento.HomeZPosition = 10;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void PrinterMemento_HomeZPosition_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new PrinterMemento((ISettingsFactory)null);
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "HomeZPosition") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.HomeZPosition = SPrinter.Default.HomeZPositionMin - 1;
            Assert.AreEqual(SPrinter.Default.HomeZPosition, settingsMemento.HomeZPosition);
        }

        [Test]
        public void PrinterMemento_GUID_Same()
        {
            var raised = false;

            var settingsMemento = new PrinterMemento((ISettingsFactory)null);

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
        public void PrinterMemento_ParentGUID_Same()
        {
            var raised = false;

            var settingsMemento = new PrinterMemento((ISettingsFactory)null);

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
        public void PrinterMemento_AddExtruderPF_AddExtruderP()
        {
            var extruderPF1 = MockRepository.GenerateStub<IExtruderPFMemento>();
            var extruderP2 = MockRepository.GenerateStub<IExtruderPMemento>();

            var fact = MockRepository.GenerateStub<ISettingsFactory>();
            fact.Stub(a => a.CreateNewExtruderPF()).Return(extruderPF1).Repeat.Once();
            fact.Stub(a => a.CreateNewExtruderP()).Return(extruderP2).Repeat.Once();

            var printerMemento = new PrinterMemento(fact);
            var raisedExtruderP = false;
            var raisedExtruderPF = false;
            printerMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "ExtrudersP") raisedExtruderP = true;
                if (args.PropertyName == "ExtrudersPF") raisedExtruderPF = true;
            };
            printerMemento.AddExtruderPF();
            printerMemento.AddExtruderP();

            Assert.IsTrue(raisedExtruderP);
            Assert.IsTrue(raisedExtruderPF);

            Assert.AreEqual(0, printerMemento.ExtrudersP.First().ExtruderIndex);
            Assert.AreEqual(1, printerMemento.ExtrudersPF.First().ExtruderIndex);
        }

        [Test]
        public void PrinterMemento_AddExtruderPF_AddExtruderP_RemoveExtruderP_RemoveExtruderPF()
        {
            var extruderPF0 = new ExtruderPFMemento();
            var extruderPF1 = new ExtruderPFMemento();
            var extruderP0 = new ExtruderPMemento();
            var extruderP1 = new ExtruderPMemento();

            var fact = MockRepository.GenerateStub<ISettingsFactory>();
            fact.Stub(a => a.CreateNewExtruderPF()).Return(extruderPF0).Repeat.Once();
            fact.Stub(a => a.CreateNewExtruderPF()).Return(extruderPF1).Repeat.Once();
            fact.Stub(a => a.CreateNewExtruderP()).Return(extruderP0).Repeat.Once();
            fact.Stub(a => a.CreateNewExtruderP()).Return(extruderP1).Repeat.Once();

            var printerMemento = new PrinterMemento(fact);
            printerMemento.AddExtruderPF();
            printerMemento.AddExtruderP();
            printerMemento.AddExtruderP();
            printerMemento.AddExtruderPF();
            var raisedExtruderP = false;
            var raisedExtruderPF = false;
            printerMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "ExtrudersP") raisedExtruderP = true;
                if (args.PropertyName == "ExtrudersPF") raisedExtruderPF = true;
            };
            raisedExtruderP = false;
            raisedExtruderPF = false;

            printerMemento.RemoveExtruder(extruderP0);

            Assert.IsTrue(raisedExtruderP);
            Assert.IsTrue(raisedExtruderPF);

            raisedExtruderP = false;
            raisedExtruderPF = false;

            printerMemento.RemoveExtruder(printerMemento.ExtrudersPF.First());

            Assert.IsFalse(raisedExtruderP);
            Assert.IsTrue(raisedExtruderPF);

            raisedExtruderP = false;
            raisedExtruderPF = false;

            printerMemento.RemoveExtruder(printerMemento.ExtrudersPF.First());

            Assert.IsFalse(raisedExtruderP);
            Assert.IsTrue(raisedExtruderPF);

            Assert.AreEqual(1, printerMemento.ExtrudersP.Count);
            Assert.AreEqual(0, printerMemento.ExtrudersPF.Count);
        }

        [Test]
        public void PrinterMemento_FillFromAnother_NotAPrinterMemento()
        {
            var printerMemento = new PrinterMemento((ISettingsFactory)null);

            ISettingsMemento settingsItem = new MaterialPMemento();
            Assert.Throws<InvalidCastException>(() => printerMemento.FillFromAnother(settingsItem));
        }

        [Test]
        public void PrinterMemento_FillFromAnother_IsOK()
        {
            var settingsFactory = new SettingsFactory();
            var printerMemento = new PrinterMemento(settingsFactory);
            printerMemento.SetDefault();

            var printerMementoRecipient = new PrinterMemento(settingsFactory);

            Assert.IsFalse(printerMemento.Equals(printerMementoRecipient));
            printerMementoRecipient.FillFromAnother((ISettingsMemento)printerMemento);
            Assert.IsTrue(printerMemento.Equals(printerMementoRecipient));
        }

        [Test]
        public void PrinterMemento_EqualsWithoutNameVersionApproved_NotAPrinterMemento()
        {
            var printerMemento = new PrinterMemento((ISettingsFactory)null);

            ISettingsMemento settingsItem = new MaterialPMemento();
            Assert.Throws<InvalidCastException>(() => printerMemento.EqualsWithoutNameVersionApproved(settingsItem));
        }

        [Test]
        public void PrinterMemento_EqualsWithoutNameVersionApproved_IsOK()
        {
            var settingsFactory = new SettingsFactory();
            var printerMemento = new PrinterMemento(settingsFactory);
            printerMemento.SetDefault();

            var printerMementoRecipient = new PrinterMemento(settingsFactory);

            var res = printerMementoRecipient.EqualsWithoutNameVersionApproved((ISettingsMemento)printerMemento);
            Assert.IsFalse(res);
        }
    }
}