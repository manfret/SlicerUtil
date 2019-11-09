using System;
using NUnit.Framework;
using Settings.Memento;
using Settings.ValidValues;

namespace TestSettings.Memento
{
    [TestFixture]
    public class TestMaterialFMemento
    {
        [Test]
        public void MaterialFMemento_Name_InAcceptableRange()
        {
            var raised = false;
            var raisedChanging = false;

            var settingsMemento = new MaterialFMemento();
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
        public void MaterialFMemento_Name_Same()
        {
            var raised = false;
            var raisedChanging = false;

            var settingsMemento = new MaterialFMemento();


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
        public void MaterialFMemento_Name_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialFMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Name") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.Name = string.Empty;
            Assert.AreEqual(SMaterialF.Default.Name, settingsMemento.Name);
        }

        [Test]
        public void MaterialFMemento_FiberType_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialFMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "FiberType") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.FiberType = "bzz";
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void MaterialFMemento_FiberType_Same()
        {
            var raised = false;

            var settingsMemento = new MaterialFMemento();

            Assert.AreEqual(false, raised);
            settingsMemento.FiberType = "bzz";
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "FiberType") raised = true;
            };
            settingsMemento.FiberType = "bzz";
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void MaterialFMemento_FiberType_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialFMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "FiberType") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.FiberType = string.Empty;
            Assert.AreEqual(SMaterialF.Default.FiberType, settingsMemento.FiberType);
        }

        [Test]
        public void MaterialFMemento_FiberDiameter_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialFMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "FiberDiameter") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.FiberDiameter = 0.5;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void MaterialFMemento_FiberDiameter_Same()
        {
            var raised = false;

            var settingsMemento = new MaterialFMemento();

            Assert.AreEqual(false, raised);
            settingsMemento.FiberDiameter = 0.5;
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "FiberDiameter") raised = true;
            };
            settingsMemento.FiberDiameter = 0.5;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void MaterialFMemento_FiberDiameter_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialFMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "FiberDiameter") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.FiberDiameter = -1;
            Assert.AreEqual(SMaterialF.Default.FiberDiameter, settingsMemento.FiberDiameter);
        }

        [Test]
        public void MaterialFMemento_ExtraSpeedF_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialFMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "ExtraSpeedF") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.ExtraSpeedF = 50;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void MaterialFMemento_ExtraSpeedF_Same()
        {
            var raised = false;

            var settingsMemento = new MaterialFMemento();

            Assert.AreEqual(false, raised);
            settingsMemento.ExtraSpeedF = 50;
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "ExtraSpeedF") raised = true;
            };
            settingsMemento.ExtraSpeedF = 50;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void MaterialFMemento_ExtraSpeedF_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialFMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "ExtraSpeedF") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.ExtraSpeedF = SMaterialF.Default.ExtraSpeedFMin - 1;
            Assert.AreEqual(SMaterialF.Default.ExtraSpeedF, settingsMemento.ExtraSpeedF);
        }

        [Test]
        public void MaterialFMemento_FanSpeed_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialFMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "FanSpeed") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.FanSpeed = 50;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void MaterialFMemento_FanSpeed_Same()
        {
            var raised = false;

            var settingsMemento = new MaterialFMemento();

            Assert.AreEqual(false, raised);
            settingsMemento.FanSpeed = 50;
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "FanSpeed") raised = true;
            };
            settingsMemento.FanSpeed = 50;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void MaterialFMemento_FanSpeed_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialFMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "FanSpeed") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.FanSpeed = SMaterialF.Default.FanSpeedMin - 1;
            Assert.AreEqual(SMaterialF.Default.FanSpeed, settingsMemento.FanSpeed);
        }

        [Test]
        public void MaterialFMemento_LinearDensity_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialFMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "LinearDensity") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.LinearDensity = 50;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void MaterialFMemento_LinearDensity_Same()
        {
            var raised = false;

            var settingsMemento = new MaterialFMemento();

            Assert.AreEqual(false, raised);
            settingsMemento.LinearDensity = 50;
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "LinearDensity") raised = true;
            };
            settingsMemento.LinearDensity = 50;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void MaterialFMemento_LinearDensity_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialFMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "LinearDensity") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.LinearDensity = SMaterialF.Default.LinearDensityMin - 1;
            Assert.AreEqual(SMaterialF.Default.LinearDensity, settingsMemento.LinearDensity);
        }

        [Test]
        public void MaterialFMemento_CostPerSpool_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialFMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "CostPerSpool") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.CostPerSpool = 50;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void MaterialFMemento_CostPerSpool_Same()
        {
            var raised = false;

            var settingsMemento = new MaterialFMemento();

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
        public void MaterialFMemento_CostPerSpool_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialFMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "CostPerSpool") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.CostPerSpool = SMaterialF.Default.CostPerSpoolMin - 1;
            Assert.AreEqual(SMaterialF.Default.CostPerSpool, settingsMemento.CostPerSpool);
        }

        [Test]
        public void MaterialFMemento_LengthPerSpool_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialFMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "LengthPerSpool") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.LengthPerSpool = 50;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void MaterialFMemento_LengthPerSpool_Same()
        {
            var raised = false;

            var settingsMemento = new MaterialFMemento();

            Assert.AreEqual(false, raised);
            settingsMemento.LengthPerSpool = 50;
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "LengthPerSpool") raised = true;
            };
            settingsMemento.LengthPerSpool = 50;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void MaterialFMemento_LengthPerSpool_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialFMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "LengthPerSpool") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.LengthPerSpool = SMaterialF.Default.LengthPerSpoolMin - 1;
            Assert.AreEqual(SMaterialF.Default.LengthPerSpool, settingsMemento.LengthPerSpool);
        }

        [Test]
        public void MaterialFMemento_Description_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialFMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Description") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.Description = "111";
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void MaterialFMemento_Description_Same()
        {
            var raised = false;

            var settingsMemento = new MaterialFMemento();

            Assert.AreEqual(false, raised);
            settingsMemento.Description = "111";
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Description") raised = true;
            };
            settingsMemento.Description = "111";
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void MaterialFMemento_Version_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialFMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Version") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.Version = 1;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void MaterialFMemento_Version_Same()
        {
            var raised = false;

            var settingsMemento = new MaterialFMemento();

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
        public void MaterialFMemento_IsAnisoprintApproved_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialFMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "IsAnisoprintApproved") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.IsAnisoprintApproved = true;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void MaterialFMemento_IsAnisoprintApproved_Same()
        {
            var raised = false;

            var settingsMemento = new MaterialFMemento();

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
        public void MaterialFMemento_ZHopF_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialFMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "ZHopF") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.ZHopF = 0.5;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void MaterialFMemento_ZHopF_Same()
        {
            var raised = false;

            var settingsMemento = new MaterialFMemento();

            Assert.AreEqual(false, raised);
            settingsMemento.ZHopF = 0.5;
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "ZHopF") raised = true;
            };
            settingsMemento.ZHopF = 0.5;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void MaterialFMemento_ZHopF_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialFMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "ZHopF") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.ZHopF = SMaterialF.Default.ZHopFMin - 1;
            Assert.AreEqual(SMaterialF.Default.ZHopF, settingsMemento.ZHopF);
        }

        [Test]
        public void MaterialFMemento_ZHopFPauseAdhesion_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialFMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "ZHopFPauseAdhesion") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.ZHopFPauseAdhesion = 0.5;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void MaterialFMemento_ZHopFPauseAdhesion_Same()
        {
            var raised = false;

            var settingsMemento = new MaterialFMemento();

            Assert.AreEqual(false, raised);
            settingsMemento.ZHopFPauseAdhesion = 0.5;
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "ZHopFPauseAdhesion") raised = true;
            };
            settingsMemento.ZHopFPauseAdhesion = 0.5;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void MaterialFMemento_ZHopFPauseAdhesion_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialFMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "ZHopFPauseAdhesion") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.ZHopFPauseAdhesion = SMaterialF.Default.ZHopFPauseAdhesionMin - 1;
            Assert.AreEqual(SMaterialF.Default.ZHopFPauseAdhesion, settingsMemento.ZHopFPauseAdhesion);
        }

        [Test]
        public void MaterialFMemento_EndPolygonEmptyDistanceMM_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialFMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "EndPolygonEmptyDistanceMM") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.EndPolygonEmptyDistanceMM = 0.5;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void MaterialFMemento_EndPolygonEmptyDistanceMM_Same()
        {
            var raised = false;

            var settingsMemento = new MaterialFMemento();

            Assert.AreEqual(false, raised);
            settingsMemento.EndPolygonEmptyDistanceMM = 0.5;
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "EndPolygonEmptyDistanceMM") raised = true;
            };
            settingsMemento.EndPolygonEmptyDistanceMM = 0.5;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void MaterialFMemento_EndPolygonEmptyDistanceMM_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new MaterialFMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "EndPolygonEmptyDistanceMM") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.EndPolygonEmptyDistanceMM = SMaterialF.Default.EndPolygonEmptyDistanceMMMin - 1;
            Assert.AreEqual(SMaterialF.Default.EndPolygonEmptyDistanceMM, settingsMemento.EndPolygonEmptyDistanceMM);
        }

        [Test]
        public void InsetXFiberSettingsMemento_Speed_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new InsetXFiberSettingsMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "SpeedCoefficient") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.SpeedCoefficient = 50;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void InsetXFiberSettingsMemento_Speed_Same()
        {
            var raised = false;

            var settingsMemento = new InsetXFiberSettingsMemento();

            Assert.AreEqual(false, raised);
            settingsMemento.SpeedCoefficient = 50;
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "SpeedCoefficient") raised = true;
            };
            settingsMemento.SpeedCoefficient = 50;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void InsetXFiberSettingsMemento_Speed_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new InsetXFiberSettingsMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Speed") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.SpeedCoefficient = SInsetF.Default.PrintingSpeedCoefficientMin - 1;
            Assert.AreEqual(SInsetF.Default.PrintingSpeedCoefficient, settingsMemento.SpeedCoefficient);
        }

        [Test]
        public void MaterialFMemento_DoPlasticRetract()
        {
            var raised = false;

            var materialFMemento = new MaterialFMemento();
            materialFMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "DoPlasticRetract") raised = true;
            };

            Assert.AreEqual(false, raised);
            materialFMemento.DoPlasticRetract = true;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void MaterialFMemento_DoPlasticRetract_Same()
        {
            var raised = false;

            var materialFMemento = new MaterialFMemento();

            Assert.AreEqual(false, raised);
            materialFMemento.DoPlasticRetract = true;
            materialFMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "DoPlasticRetract") raised = true;
            };
            materialFMemento.DoPlasticRetract = true;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void MaterialFMemento_GUID_Same()
        {
            var raised = false;

            var settingsMemento = new MaterialFMemento();

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
        public void MaterialFMemento_ParentGUID_Same()
        {
            var raised = false;

            var settingsMemento = new MaterialFMemento();

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
        public void MaterialFMemento_EqualsWithoutNameVersionApproved_DifferentNames()
        {
            var settingsMemento1 = new MaterialFMemento();
            settingsMemento1.SetDefault();

            var settingsMemento2 = new MaterialFMemento();
            settingsMemento2.SetDefault();
            settingsMemento2.Name = "NewName";
            settingsMemento2.Version = 2;

            Assert.IsTrue(settingsMemento1.EqualsWithoutNameVersionApproved((ISettingsMemento)settingsMemento2));
        }

        [Test]
        public void MaterialFMemento_EqualsWithoutNameVersionApproved_DifferentFields()
        {
            var settingsMemento1 = new MaterialFMemento();
            settingsMemento1.SetDefault();

            var settingsMemento2 = new MaterialFMemento();
            settingsMemento2.SetDefault();
            settingsMemento2.Name = "NewName";
            settingsMemento2.Version = 2;
            settingsMemento2.FanSpeed = 2;

            Assert.IsFalse(settingsMemento1.EqualsWithoutNameVersionApproved((ISettingsMemento)settingsMemento2));
        }

        [Test]
        public void MaterialFMemento_EqualsWithoutNameVersionApproved_NotMaterialP()
        {

            var settingsMemento2 = new MaterialFMemento();
            settingsMemento2.SetDefault();
            settingsMemento2.Name = "NewName";
            settingsMemento2.Version = 2;
            settingsMemento2.FanSpeed = 2;

            ISettingsMemento settingsMemento1 = null;

            Assert.Throws<InvalidCastException>(() => settingsMemento2.EqualsWithoutNameVersionApproved(settingsMemento1));
        }

        [Test]
        public void MaterialFMemento_FillFromAnother_DifferentNames()
        {
            var settingsMemento1 = new MaterialFMemento();
            settingsMemento1.SetDefault();

            var settingsMemento2 = new MaterialFMemento();
            settingsMemento2.SetDefault();
            settingsMemento2.Name = "NewName";
            settingsMemento2.FanSpeed = 2;

            settingsMemento1.FillFromAnother((ISettingsMemento)settingsMemento2);

            Assert.IsTrue(settingsMemento1.Equals(settingsMemento2));
        }

        [Test]
        public void MaterialFMemento_FillFromAnother_NotMaterialP()
        {

            var settingsMemento2 = new MaterialFMemento();
            settingsMemento2.SetDefault();

            ISettingsMemento settingsMemento1 = null;

            Assert.Throws<InvalidCastException>(() => settingsMemento2.FillFromAnother(settingsMemento1));
        }
    }
}