using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media.Media3D;
using MSClipperLib;
using NUnit.Framework;
using Rhino.Mocks;
using Settings;
using Settings.Memento;

namespace TestSettings.Memento
{
    [TestFixture]
    public class TestSessionMemento
    {
        private ISettingsFactory _settingsFactory = new SettingsFactory();

        [Test]
        public void SessionMemento_Name_InAcceptableRange()
        {
            var raised = false;
            var raisedChanging = false;

            var settingsMemento = new SessionMemento(_settingsFactory);
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
        public void SessionMemento_Name_Same()
        {
            var raised = false;
            var raisedChanging = false;

            var settingsMemento = new SessionMemento(_settingsFactory);

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
        public void SessionMemento_Name_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new SessionMemento(_settingsFactory);
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Name") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.Name = string.Empty;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void SessionMemento_Descriprion_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new SessionMemento(_settingsFactory);
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Descriprion") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.Descriprion = "bzz";
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void SessionMemento_Descriprion_Same()
        {
            var raised = false;

            var settingsMemento = new SessionMemento(_settingsFactory);

            Assert.AreEqual(false, raised);
            settingsMemento.Descriprion = "bzz";
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Descriprion") raised = true;
            };
            settingsMemento.Descriprion = "bzz";
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void SessionMemento_LastPrintingDate_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new SessionMemento(_settingsFactory);
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "LastPrintingDate") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.LastPrintingDate = DateTime.Today;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void SessionMemento_LastPrintingDate_Same()
        {
            var raised = false;

            var settingsMemento = new SessionMemento(_settingsFactory);

            Assert.AreEqual(false, raised);
            settingsMemento.LastPrintingDate = DateTime.Today;
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "LastPrintingDate") raised = true;
            };
            settingsMemento.LastPrintingDate = DateTime.Today;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void SessionMemento_Printer_InAcceptableRange()
        {
            var raised = false;
            var raisedExtrudersPMaterials = false;
            var raisedExtrudersPFMaterials = false;
            var raisedPlastic = false;
            var raisedFiber = false;

            var settingsMemento = new SessionMemento(_settingsFactory);
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Printer") raised = true;
                if (args.PropertyName == "ExtrudersPMaterials") raisedExtrudersPMaterials = true;
                if (args.PropertyName == "ExtrudersPFMaterials") raisedExtrudersPFMaterials = true;
                if (args.PropertyName == "Plastic") raisedPlastic = true;
                if (args.PropertyName == "Fiber") raisedFiber = true;
            };

            Assert.AreEqual(false, raised);
            Assert.AreEqual(false, raisedExtrudersPMaterials);
            Assert.AreEqual(false, raisedExtrudersPFMaterials);
            var printerMemento = new PrinterMemento(_settingsFactory);
            printerMemento.SetDefault();
            printerMemento.AddExtruderPF();
            settingsMemento.Printer = printerMemento;
            Assert.AreEqual(true, raised);
            Assert.AreEqual(true, raisedExtrudersPMaterials);
            Assert.AreEqual(true, raisedExtrudersPFMaterials);

            raisedPlastic = false;
            raisedFiber = false;

            settingsMemento.ExtrudersPMaterials.First().Index = 1;

            Assert.AreEqual(true, raisedPlastic);
            Assert.AreEqual(false, raisedFiber);

            raisedPlastic = false;
            raisedFiber = false;

            settingsMemento.ExtrudersPFMaterials.First().Index = 0;

            Assert.AreEqual(false, raisedPlastic);
            Assert.AreEqual(true, raisedFiber);

            Assert.AreEqual(1, settingsMemento.ExtrudersPMaterials.Count());
            Assert.AreEqual(1, settingsMemento.ExtrudersPFMaterials.Count());

            Assert.AreEqual(0, settingsMemento.Inset0Extruder);
            Assert.AreEqual(0, settingsMemento.InsetPlasticExtruder);
            Assert.AreEqual(0, settingsMemento.InfillSolidExtruder);
            Assert.AreEqual(0, settingsMemento.InfillPlasticCellularExtruder);
            Assert.AreEqual(0, settingsMemento.SupportExtruder);
            Assert.AreEqual(0, settingsMemento.BrimExtruder);
            Assert.AreEqual(0, settingsMemento.SkirtExtruder);
            Assert.AreEqual(1, settingsMemento.InsetFiberExtruder);
        }

        [Test]
        public void SessionMemento_Printer_Null()
        {
            var raised = false;
            var raisedExtrudersPMaterials = false;
            var raisedExtrudersPFMaterials = false;

            var settingsMemento = new SessionMemento(_settingsFactory);
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Printer") raised = true;
                if (args.PropertyName == "ExtrudersPMaterials") raisedExtrudersPMaterials = true;
                if (args.PropertyName == "ExtrudersPFMaterials") raisedExtrudersPFMaterials = true;
            };

            Assert.AreEqual(false, raised);
            Assert.AreEqual(false, raisedExtrudersPMaterials);
            Assert.AreEqual(false, raisedExtrudersPFMaterials);
            settingsMemento.Printer = null;
            Assert.AreEqual(false, raised);
            Assert.AreEqual(false, raisedExtrudersPMaterials);
            Assert.AreEqual(false, raisedExtrudersPFMaterials);
        }

        [Test]
        public void SessionMemento_Profile_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new SessionMemento(_settingsFactory);
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Profile") raised = true;
            };

            Assert.AreEqual(false, raised);
            var profileMemento = new ProfileMemento();
            profileMemento.SetDefault();
            settingsMemento.Profile = profileMemento;
            Assert.AreEqual(true, raised);

            Assert.AreEqual(null, settingsMemento.InsetFiberExtruder);
        }

        [Test]
        public void SessionMemento_Profile_Null()
        {
            var raised = false;

            var settingsMemento = new SessionMemento(_settingsFactory);
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Profile") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.Profile = null;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void SessionMemento_Inset0Extruder_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new SessionMemento(_settingsFactory);
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Inset0Extruder") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.Inset0Extruder = 1;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void SessionMemento_Inset0Extruder_Same()
        {
            var raised = false;

            var settingsMemento = new SessionMemento(_settingsFactory);

            Assert.AreEqual(false, raised);
            settingsMemento.Inset0Extruder = 1;
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Inset0Extruder") raised = true;
            };
            settingsMemento.Inset0Extruder = 1;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void SessionMemento_InsetPlasticExtruder_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new SessionMemento(_settingsFactory);
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "InsetPlasticExtruder") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.InsetPlasticExtruder = 1;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void SessionMemento_InsetPlasticExtruder_Same()
        {
            var raised = false;

            var settingsMemento = new SessionMemento(_settingsFactory);

            Assert.AreEqual(false, raised);
            settingsMemento.InsetPlasticExtruder = 1;
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "InsetPlasticExtruder") raised = true;
            };
            settingsMemento.InsetPlasticExtruder = 1;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void SessionMemento_InfillSolidExtruder_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new SessionMemento(_settingsFactory);
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "InfillSolidExtruder") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.InfillSolidExtruder = 1;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void SessionMemento_InfillSolidExtruder_Same()
        {
            var raised = false;

            var settingsMemento = new SessionMemento(_settingsFactory);

            Assert.AreEqual(false, raised);
            settingsMemento.InfillSolidExtruder = 1;
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "InfillSolidExtruder") raised = true;
            };
            settingsMemento.InfillSolidExtruder = 1;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void SessionMemento_InfillPlasticCellularExtruder_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new SessionMemento(_settingsFactory);
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "InfillPlasticCellularExtruder") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.InfillPlasticCellularExtruder = 1;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void SessionMemento_InfillPlasticCellularExtruder_Same()
        {
            var raised = false;

            var settingsMemento = new SessionMemento(_settingsFactory);

            Assert.AreEqual(false, raised);
            settingsMemento.InfillPlasticCellularExtruder = 1;
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "InfillPlasticCellularExtruder") raised = true;
            };
            settingsMemento.InfillPlasticCellularExtruder = 1;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void SessionMemento_SupportExtruder_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new SessionMemento(_settingsFactory);
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "SupportExtruder") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.SupportExtruder = 1;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void SessionMemento_SupportExtruder_Same()
        {
            var raised = false;

            var settingsMemento = new SessionMemento(_settingsFactory);

            Assert.AreEqual(false, raised);
            settingsMemento.SupportExtruder = 1;
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "SupportExtruder") raised = true;
            };
            settingsMemento.SupportExtruder = 1;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void SessionMemento_BrimExtruder_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new SessionMemento(_settingsFactory);
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "BrimExtruder") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.BrimExtruder = 1;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void SessionMemento_BrimExtruder_Same()
        {
            var raised = false;

            var settingsMemento = new SessionMemento(_settingsFactory);

            Assert.AreEqual(false, raised);
            settingsMemento.BrimExtruder = 1;
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "BrimExtruder") raised = true;
            };
            settingsMemento.BrimExtruder = 1;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void SessionMemento_SkirtExtruder_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new SessionMemento(_settingsFactory);
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "SkirtExtruder") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.SkirtExtruder = 1;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void SessionMemento_SkirtExtruder_Same()
        {
            var raised = false;

            var settingsMemento = new SessionMemento(_settingsFactory);

            Assert.AreEqual(false, raised);
            settingsMemento.SkirtExtruder = 1;
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "SkirtExtruder") raised = true;
            };
            settingsMemento.SkirtExtruder = 1;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void SessionMemento_InsetFiberExtruder_InAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new SessionMemento(_settingsFactory);
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "InsetFiberExtruder") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.InsetFiberExtruder = 1;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void SessionMemento_InsetFiberExtruder_Same()
        {
            var raised = false;

            var settingsMemento = new SessionMemento(_settingsFactory);

            Assert.AreEqual(false, raised);
            settingsMemento.InsetFiberExtruder = 1;
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "InsetFiberExtruder") raised = true;
            };
            settingsMemento.InsetFiberExtruder = 1;
            Assert.AreEqual(false, raised);
        }

        [Test]
        public void SessionMemento_GetWTBox_NoGenerateWT()
        {
            var generateWT = MockRepository.GenerateStub<IGlobalVariableSettingsMemento>();
            generateWT.Stub(a => a.TrueGenerateWipeTower).Return(false);

            var profile = MockRepository.GenerateStub<IProfileMemento>();
            profile.GlobalVariableSettingsMemento = generateWT;

            var sessionMemento = new SessionMemento(_settingsFactory);
            sessionMemento.Profile = profile;
            var (boxMin, boxMax) = sessionMemento.GetWTBox();

            Assert.AreEqual(boxMin, new Point3D(0.0, 0.0, 0.0));
            Assert.AreEqual(boxMax, new Point3D(0.0, 0.0, 0.0));
        }

        [Test]
        public void SessionMemento_GetWTBox_GenerateWT()
        {
            var generateWT = MockRepository.GenerateStub<IGlobalVariableSettingsMemento>();
            generateWT.Stub(a => a.TrueGenerateWipeTower).Return(true);
            generateWT.Stub(a => a.TrueWipeTowerPosX).Return(10);
            generateWT.Stub(a => a.TrueWipeTowerPosY).Return(10);
            generateWT.Stub(a => a.TrueWipeTowerWidth).Return(20);
            generateWT.Stub(a => a.TrueWipeTowerLength).Return(30);
            generateWT.Stub(a => a.TrueBrimWTLoopsCount).Return(2);

            var inset0PlasticSettingsMemento = MockRepository.GenerateStub<IInset0PlasticSettingsMemento>();
            inset0PlasticSettingsMemento.Stub(a => a.TrueEWCoeff).Return(1);

            var extruderP = MockRepository.GenerateStub<IExtruderPMemento>();
            extruderP.Stub(a => a.TrueNozzleDiameter).Return(2);
            extruderP.ExtruderIndex = 0;

            var printer = MockRepository.GenerateStub<IPrinterMemento>();
            printer.Stub(a => a.ExtrudersP).Return(new ObservableCollection<IExtruderPMemento> {extruderP});

            var profile = MockRepository.GenerateStub<IProfileMemento>();
            profile.GlobalVariableSettingsMemento = generateWT;
            profile.Inset0PlasticSettingsMemento = inset0PlasticSettingsMemento;

            var sessionMemento = new SessionMemento(printer, profile, _settingsFactory)
            {
                Inset0Extruder = 0
            };
            var (boxMin, boxMax) = sessionMemento.GetWTBox();

            Assert.AreEqual(boxMin, new Point3D(6.0, 6.0, 0.0));
            Assert.AreEqual(boxMax, new Point3D(34.0, 44.0, 0.0));
        }

        [Test]
        public void SessionMemento_GetSkirtDistance_DontGenerateSKirt()
        {
            var globalVariableSettingsMemento = MockRepository.GenerateStub<IGlobalVariableSettingsMemento>();
            globalVariableSettingsMemento.Stub(a => a.TrueGenerateSkirt).Return(false);

            var profile = MockRepository.GenerateStub<IProfileMemento>();
            profile.GlobalVariableSettingsMemento = globalVariableSettingsMemento;

            var sessionMemento = new SessionMemento(null, profile, _settingsFactory);
            var res = sessionMemento.GetSkirtDistance();

            Assert.AreEqual(0.0, res);
        }

        [Test]
        public void SessionMemento_GetSkirtDistance_GenerateSKir()
        {
            var globalVariableSettingsMemento = MockRepository.GenerateStub<IGlobalVariableSettingsMemento>();
            globalVariableSettingsMemento.Stub(a => a.TrueGenerateSkirt).Return(true);

            var brimSettingsMemento = MockRepository.GenerateStub<IBrimSettingsMemento>();
            brimSettingsMemento.Stub(a => a.TrueEWCoeff).Return(1);

            var skirtSettingsMemento = MockRepository.GenerateStub<ISkirtSettingsMemento>();
            skirtSettingsMemento.Stub(a => a.TrueLoops).Return(2);
            skirtSettingsMemento.Stub(a => a.TrueDistance).Return(2);

            var extruderP = MockRepository.GenerateStub<IExtruderPMemento>();
            extruderP.Stub(a => a.TrueNozzleDiameter).Return(2);
            extruderP.ExtruderIndex = 0;

            var printer = MockRepository.GenerateStub<IPrinterMemento>();
            printer.Stub(a => a.ExtrudersP).Return(new ObservableCollection<IExtruderPMemento> { extruderP });

            var profile = MockRepository.GenerateStub<IProfileMemento>();
            profile.GlobalVariableSettingsMemento = globalVariableSettingsMemento;
            profile.BrimSettingsMemento = brimSettingsMemento;
            profile.SkirtSettingsMemento = skirtSettingsMemento;

            var sessionMemento = new SessionMemento(printer, profile, _settingsFactory)
            {
                SkirtExtruder = 0
            };
            var res = sessionMemento.GetSkirtDistance();

            Assert.AreEqual(6.0, res);
        }

        [Test]
        public void SessionMemento_GetBrimDistance_DontGenerateBrim()
        {
            var globalVariableSettingsMemento = MockRepository.GenerateStub<IGlobalVariableSettingsMemento>();
            globalVariableSettingsMemento.Stub(a => a.TrueGenerateBrim).Return(false);

            var profile = MockRepository.GenerateStub<IProfileMemento>();
            profile.GlobalVariableSettingsMemento = globalVariableSettingsMemento;

            var sessionMemento = new SessionMemento(null, profile, _settingsFactory);
            var res = sessionMemento.GetBrimDistance();

            Assert.AreEqual(0.0, res);
        }

        [Test]
        public void SessionMemento_GetSkirtDistance_GenerateBrim()
        {
            var globalVariableSettingsMemento = MockRepository.GenerateStub<IGlobalVariableSettingsMemento>();
            globalVariableSettingsMemento.Stub(a => a.TrueGenerateBrim).Return(true);

            var brimSettingsMemento = MockRepository.GenerateStub<IBrimSettingsMemento>();
            brimSettingsMemento.Stub(a => a.TrueEWCoeff).Return(1);
            brimSettingsMemento.Stub(a => a.TrueLoops).Return(1);


            var extruderP = MockRepository.GenerateStub<IExtruderPMemento>();
            extruderP.Stub(a => a.TrueNozzleDiameter).Return(2);
            extruderP.ExtruderIndex = 0;

            var printer = MockRepository.GenerateStub<IPrinterMemento>();
            printer.Stub(a => a.ExtrudersP).Return(new ObservableCollection<IExtruderPMemento> { extruderP });

            var profile = MockRepository.GenerateStub<IProfileMemento>();
            profile.GlobalVariableSettingsMemento = globalVariableSettingsMemento;
            profile.BrimSettingsMemento = brimSettingsMemento;

            var sessionMemento = new SessionMemento(printer, profile, _settingsFactory)
            {
                BrimExtruder = 0
            };
            var res = sessionMemento.GetBrimDistance();

            Assert.AreEqual(2.0, res);
        }
    }
}