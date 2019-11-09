using System;
using System.ComponentModel;
using NUnit.Framework;
using Rhino.Mocks;
using Settings.Memento;
using Settings.ValidValues;

namespace TestSettings.Memento
{
    [TestFixture]
    public class TestProfileMemento
    {
        [Test]
        public void ProfileMemento_Name_InAcceptableRange()
        {
            var raised = false;
            var raisedChanging = false;

            var settingsMemento = new ProfileMemento();
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
        public void ProfileMemento_Name_Same()
        {
            var raised = false;
            var raisedChanging = false;

            var settingsMemento = new ProfileMemento();

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
        public void ProfileMemento_Name_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new ProfileMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Name") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.Name = string.Empty;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void ProfileMemento_LayupRule_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new ProfileMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "LayupRule") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.LayupRule = new LayupRule();
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void ProfileMemento_LayupRule_PropertyChanged()
        {
            var lr = MockRepository.GenerateStub<ILayupRule>();

            var raised = false;

            var settingsMemento = new ProfileMemento();
            settingsMemento.LayupRule = lr;
            Assert.AreEqual(false, raised);
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "LayupRule") raised = true;
            };
            lr.Raise(a=>a.PropertyChanged += (sender, args) => {}, null, null);
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void ProfileMemento_GlobalSettingsMemento_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new ProfileMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "GlobalSettingsMemento") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.GlobalSettingsMemento = new GlobalSettingsMemento();
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void ProfileMemento_GlobalSettingsMemento_PropertyChanged()
        {
            var item = MockRepository.GenerateStub<IGlobalSettingsMemento>();
            item.Stub(a => a.TrueAllowGenerateFiber).Return(false);
            var lr = new LayupRule
            {
                GenerateFiberPerimeters = true,
                GenerateFiberInfill = true
            };

            var raised = false;

            var settingsMemento = new ProfileMemento
            {
                GlobalSettingsMemento = item,
                LayupRule = lr
            };
            Assert.AreEqual(false, raised);
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "GlobalSettingsMemento") raised = true;
            };
            item.Raise(a => a.PropertyChanged += (sender, args) => { }, null, new PropertyChangedEventArgs("AllowGenerateFiber"));
            Assert.AreEqual(true, raised);
            Assert.IsFalse(lr.GenerateFiberPerimeters);
            Assert.IsFalse(lr.GenerateFiberInfill);
        }

        [Test]
        public void ProfileMemento_GlobalVariableSettingsMemento_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new ProfileMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "GlobalVariableSettingsMemento") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.GlobalVariableSettingsMemento = new GlobalVariableSettingsMemento();
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void ProfileMemento_GlobalVariableSettingsMemento_PropertyChanged()
        {
            var raised = false;

            var item = MockRepository.GenerateStub<IGlobalVariableSettingsMemento>();

            var settingsMemento = new ProfileMemento();
            settingsMemento.GlobalVariableSettingsMemento = item;
            Assert.AreEqual(false, raised);
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "GlobalVariableSettingsMemento") raised = true;
            };
            item.Raise(a => a.PropertyChanged += (sender, args) => { }, null, null);
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void ProfileMemento_Inset0PlasticSettingsMemento_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new ProfileMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Inset0PlasticSettingsMemento") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.Inset0PlasticSettingsMemento = new Inset0PlasticSettingsMemento();
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void ProfileMemento_Inset0PlasticSettingsMemento_PropertyChanged()
        {
            var raised = false;

            var item = MockRepository.GenerateStub<IInset0PlasticSettingsMemento>();

            var settingsMemento = new ProfileMemento();
            settingsMemento.Inset0PlasticSettingsMemento = item;
            Assert.AreEqual(false, raised);
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Inset0PlasticSettingsMemento") raised = true;
            };
            item.Raise(a => a.PropertyChanged += (sender, args) => { }, null, null);
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void ProfileMemento_InsetXPlasticSettingsMemento_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new ProfileMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "InsetXPlasticSettingsMemento") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.InsetXPlasticSettingsMemento = new InsetXPlasticSettingsMemento();
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void ProfileMemento_InsetXPlasticSettingsMemento_PropertyChanged()
        {
            var raised = false;

            var item = MockRepository.GenerateStub<IInsetXPlasticSettingsMemento>();

            var settingsMemento = new ProfileMemento();
            settingsMemento.InsetXPlasticSettingsMemento = item;
            Assert.AreEqual(false, raised);
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "InsetXPlasticSettingsMemento") raised = true;
            };
            item.Raise(a => a.PropertyChanged += (sender, args) => { }, null, null);
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void ProfileMemento_InsetXFiberSettingsMemento_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new ProfileMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "InsetXFiberSettingsMemento") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.InsetXFiberSettingsMemento = new InsetXFiberSettingsMemento();
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void ProfileMemento_InsetXFiberSettingsMemento_PropertyChanged()
        {
            var raised = false;

            var item = MockRepository.GenerateStub<IInsetXFiberSettingsMemento>();

            var settingsMemento = new ProfileMemento();
            settingsMemento.InsetXFiberSettingsMemento = item;
            Assert.AreEqual(false, raised);
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "InsetXFiberSettingsMemento") raised = true;
            };
            item.Raise(a => a.PropertyChanged += (sender, args) => { }, null, null);
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void ProfileMemento_InfillXFiberSolidSettingsMemento_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new ProfileMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "InfillXFiberSolidSettingsMemento") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.InfillXFiberSolidSettingsMemento = new InfillXFiberSolidSettingsMemento();
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void ProfileMemento_InfillXFiberSolidSettingsMemento_PropertyChanged()
        {
            var raised = false;

            var item = MockRepository.GenerateStub<IInfillXFiberSolidSettingsMemento>();

            var settingsMemento = new ProfileMemento();
            settingsMemento.InfillXFiberSolidSettingsMemento = item;
            Assert.AreEqual(false, raised);
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "InfillXFiberSolidSettingsMemento") raised = true;
            };
            item.Raise(a => a.PropertyChanged += (sender, args) => { }, null, null);
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void ProfileMemento_InfillXFiberCellularSettingsMemento_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new ProfileMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "InfillXFiberCellularSettingsMemento") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.InfillXFiberCellularSettingsMemento = new InfillXFiberCellularSettingsMemento();
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void ProfileMemento_InfillXFiberCellularSettingsMemento_PropertyChanged()
        {
            var raised = false;

            var item = MockRepository.GenerateStub<IInfillXFiberCellularSettingsMemento>();

            var settingsMemento = new ProfileMemento();
            settingsMemento.InfillXFiberCellularSettingsMemento = item;
            Assert.AreEqual(false, raised);
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "InfillXFiberCellularSettingsMemento") raised = true;
            };
            item.Raise(a => a.PropertyChanged += (sender, args) => { }, null, null);
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void ProfileMemento_FiberPrintingSettingsMemento_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new ProfileMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "FiberPrintingSettingsMemento") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.FiberPrintingSettingsMemento = new FiberPrintingSettingsMemento();
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void ProfileMemento_FiberPrintingSettingsMemento_PropertyChanged()
        {
            var raised = false;

            var item = MockRepository.GenerateStub<IFiberPrintingSettingsMemento>();

            var settingsMemento = new ProfileMemento();
            settingsMemento.FiberPrintingSettingsMemento = item;
            Assert.AreEqual(false, raised);
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "FiberPrintingSettingsMemento") raised = true;
            };
            item.Raise(a => a.PropertyChanged += (sender, args) => { }, null, null);
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void ProfileMemento_InfillSettingsMemento_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new ProfileMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "InfillSettingsMemento") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.InfillSettingsMemento = new InfillSettingsMemento();
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void ProfileMemento_InfillSettingsMemento_PropertyChanged()
        {
            var raised = false;

            var item = MockRepository.GenerateStub<IInfillSettingsMemento>();

            var settingsMemento = new ProfileMemento();
            settingsMemento.InfillSettingsMemento = item;
            Assert.AreEqual(false, raised);
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "InfillSettingsMemento") raised = true;
            };
            item.Raise(a => a.PropertyChanged += (sender, args) => { }, null, null);
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void ProfileMemento_InfillPlasticSolidSettingsMemento_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new ProfileMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "InfillPlasticSolidSettingsMemento") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.InfillPlasticSolidSettingsMemento = new InfillPlasticSolidSettingsMemento();
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void ProfileMemento_InfillPlasticSolidSettingsMemento_PropertyChanged()
        {
            var raised = false;

            var item = MockRepository.GenerateStub<IInfillPlasticSolidSettingsMemento>();

            var settingsMemento = new ProfileMemento();
            settingsMemento.InfillPlasticSolidSettingsMemento = item;
            Assert.AreEqual(false, raised);
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "InfillPlasticSolidSettingsMemento") raised = true;
            };
            item.Raise(a => a.PropertyChanged += (sender, args) => { }, null, null);
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void ProfileMemento_InfillPlasticCellularSettingsMemento_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new ProfileMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "InfillPlasticCellularSettingsMemento") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.InfillPlasticCellularSettingsMemento = new InfillPlasticCellularSettingsMemento();
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void ProfileMemento_InfillPlasticCellularSettingsMemento_PropertyChanged()
        {
            var raised = false;

            var item = MockRepository.GenerateStub<IInfillPlasticCellularSettingsMemento>();

            var settingsMemento = new ProfileMemento();
            settingsMemento.InfillPlasticCellularSettingsMemento = item;
            Assert.AreEqual(false, raised);
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "InfillPlasticCellularSettingsMemento") raised = true;
            };
            item.Raise(a => a.PropertyChanged += (sender, args) => { }, null, null);
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void ProfileMemento_BrimSettingsMemento_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new ProfileMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "BrimSettingsMemento") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.BrimSettingsMemento = new BrimSettingsMemento();
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void ProfileMemento_BrimSettingsMemento_PropertyChanged()
        {
            var raised = false;

            var item = MockRepository.GenerateStub<IBrimSettingsMemento>();

            var settingsMemento = new ProfileMemento();
            settingsMemento.BrimSettingsMemento = item;
            Assert.AreEqual(false, raised);
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "BrimSettingsMemento") raised = true;
            };
            item.Raise(a => a.PropertyChanged += (sender, args) => { }, null, null);
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void ProfileMemento_SkirtSettingsMemento_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new ProfileMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "SkirtSettingsMemento") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.SkirtSettingsMemento = new SkirtSettingsMemento();
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void ProfileMemento_SkirtSettingsMemento_PropertyChanged()
        {
            var raised = false;

            var item = MockRepository.GenerateStub<ISkirtSettingsMemento>();

            var settingsMemento = new ProfileMemento();
            settingsMemento.SkirtSettingsMemento = item;
            Assert.AreEqual(false, raised);
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "SkirtSettingsMemento") raised = true;
            };
            item.Raise(a => a.PropertyChanged += (sender, args) => { }, null, null);
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void ProfileMemento_InsetSettingsMemento_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new ProfileMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "InsetSettingsMemento") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.InsetSettingsMemento = new InsetSettingsMemento();
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void ProfileMemento_InsetSettingsMemento_PropertyChanged()
        {
            var raised = false;

            var item = MockRepository.GenerateStub<IInsetSettingsMemento>();

            var settingsMemento = new ProfileMemento();
            settingsMemento.InsetSettingsMemento = item;
            Assert.AreEqual(false, raised);
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "InsetSettingsMemento") raised = true;
            };
            item.Raise(a => a.PropertyChanged += (sender, args) => { }, null, null);
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void ProfileMemento_SSMemento_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new ProfileMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "SSMemento") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.SSMemento = new SSMemento();
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void ProfileMemento_SSMemento_PropertyChanged()
        {
            var raised = false;

            var item = MockRepository.GenerateStub<ISSMemento>();

            var settingsMemento = new ProfileMemento();
            settingsMemento.SSMemento = item;
            Assert.AreEqual(false, raised);
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "SSMemento") raised = true;
            };
            item.Raise(a => a.PropertyChanged += (sender, args) => { }, null, null);
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void ProfileMemento_Version_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new ProfileMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Version") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.Version = 1;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void ProfileMemento_IsAnisoprintApproved_InNotAcceptableRange()
        {
            var raised = false;

            var settingsMemento = new ProfileMemento();
            settingsMemento.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "IsAnisoprintApproved") raised = true;
            };

            Assert.AreEqual(false, raised);
            settingsMemento.IsAnisoprintApproved = true;
            Assert.AreEqual(true, raised);
        }

        [Test]
        public void ProfileMemento_EqualsWithoutNameVersionApproved_DifferentNames()
        {
            var settingsMemento1 = new ProfileMemento();
            settingsMemento1.SetDefault();

            var settingsMemento2 = new ProfileMemento();
            settingsMemento2.SetDefault();
            settingsMemento2.Name = "NewName";
            settingsMemento2.Version = 2;

            Assert.IsTrue(settingsMemento1.EqualsWithoutNameVersionApproved((ISettingsMemento)settingsMemento2));
        }

        [Test]
        public void ProfileMemento_EqualsWithoutNameVersionApproved_DifferentFields()
        {
            var settingsMemento1 = new ProfileMemento();
            settingsMemento1.SetDefault();

            var settingsMemento2 = new ProfileMemento();
            settingsMemento2.SetDefault();
            settingsMemento2.Name = "NewName";
            settingsMemento2.Version = 2;
            settingsMemento2.BrimSettingsMemento = new BrimSettingsMemento(){EWCoeff = 1.1};

            Assert.IsFalse(settingsMemento1.EqualsWithoutNameVersionApproved((ISettingsMemento)settingsMemento2));
        }

        [Test]
        public void ProfileMemento_EqualsWithoutNameVersionApproved_NotMaterialP()
        {

            var settingsMemento2 = new ProfileMemento();
            settingsMemento2.SetDefault();
            settingsMemento2.Name = "NewName";
            settingsMemento2.Version = 2;
            settingsMemento2.BrimSettingsMemento = new BrimSettingsMemento() { EWCoeff = 1.1 };

            ISettingsMemento settingsMemento1 = null;

            Assert.Throws<InvalidCastException>(() => settingsMemento2.EqualsWithoutNameVersionApproved(settingsMemento1));
        }

        [Test]
        public void ProfileMemento_FillFromAnother_DifferentNames()
        {
            var settingsMemento1 = new ProfileMemento();
            settingsMemento1.SetDefault();

            var settingsMemento2 = new ProfileMemento();
            settingsMemento2.SetDefault();
            settingsMemento2.Name = "NewName";
            settingsMemento2.Version = 2;

            settingsMemento1.FillFromAnother((ISettingsMemento)settingsMemento2);

            Assert.IsTrue(settingsMemento1.Equals(settingsMemento2));
        }

        [Test]
        public void ProfileMemento_FillFromAnother_NotMaterialP()
        {

            var settingsMemento2 = new ProfileMemento();
            settingsMemento2.SetDefault();

            ISettingsMemento settingsMemento1 = null;

            Assert.Throws<InvalidCastException>(() => settingsMemento2.FillFromAnother(settingsMemento1));
        }
    }
}