using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using NUnit.Framework;
using Rhino.Mocks;
using Settings;
using Settings.Memento;
using Settings.Session;
using Settings.Stores;

namespace TestSettings.Stores
{
    [TestFixture]
    public class TestSessionStore
    {
        [Test]
        public void SessionStore_Ctor_NoLastSession()
        {
            var fileSerializer = MockRepository.GenerateStub<ISettingFileSerializer>();
            var sessionMemento = MockRepository.GenerateStub<ISessionMemento>();
            var factory = MockRepository.GenerateStub<ISettingsFactory>();
            factory.Stub(a => a.CreateSessionMemento()).Return(sessionMemento);
            var sessionHelperWrapper = MockRepository.GenerateMock<ISessionHelperWrapper>();

            var sessionStore = new SessionStore(fileSerializer, null, null, null, null, factory, sessionHelperWrapper);
            Assert.AreEqual(sessionMemento, sessionStore.Session);
            sessionHelperWrapper.AssertWasCalled(a=>a.SyncronizePrinter(Arg<ISessionMemento>.Is.Equal(sessionMemento), Arg<ISettingsStore<IPrinterMemento>>.Is.Anything, Arg<ISettingsFactory>.Is.Equal(factory)));
            sessionHelperWrapper.AssertWasCalled(a=>a.SyncronizeMaterials(Arg<ISessionMemento>.Is.Equal(sessionMemento), Arg<ISettingsStore<IMaterialPMemento>>.Is.Anything, Arg<ISettingsStore<IMaterialFMemento>>.Is.Anything, Arg<ISettingsFactory>.Is.Equal(factory)));
            sessionHelperWrapper.AssertWasCalled(a=>a.SyncronizeProfile(Arg<ISessionMemento>.Is.Equal(sessionMemento), Arg<ISettingsStore<IProfileMemento>>.Is.Anything, Arg<ISettingsFactory>.Is.Equal(factory)));
        }

        [Test]
        public void SessionStore_PropertyChanged_ExtrudersPMaterials()
        {
            var extruderToMaterial = new ExtruderPMaterial();
            var extruderToMaterialPF = new ExtruderPFMaterial();
            var emptySession = MockRepository.GenerateMock<ISessionMemento>();
            emptySession.Stub(a => a.ExtrudersPMaterials).Return(new List<ExtruderPMaterial> {extruderToMaterial});
            emptySession.Stub(a => a.ExtrudersPFMaterials).Return(new List<ExtruderPFMaterial> {extruderToMaterialPF});
            var factory = MockRepository.GenerateStub<ISettingsFactory>();
            factory.Stub(a => a.CreateSessionMemento()).Return(emptySession);
            var printerStore = MockRepository.GenerateMock<ISettingsStore<IPrinterMemento>>();
            var userPrinter = MockRepository.GenerateStub<IPrinterMemento>();
            printerStore.Stub(a => a.Defaults).Return(new List<IPrinterMemento>());
            printerStore.Stub(a => a.Users).Return(new List<IPrinterMemento> {userPrinter});
            var plasticStore = MockRepository.GenerateMock<ISettingsStore<IMaterialPMemento>>();
            var fiberStore = MockRepository.GenerateMock<ISettingsStore<IMaterialFMemento>>();
            var plasticUser = MockRepository.GenerateStub<IMaterialPMemento>();
            var defaultFiber = MockRepository.GenerateStub<IMaterialFMemento>();
            plasticStore.Stub(a => a.Defaults).Return(new List<IMaterialPMemento>());
            plasticStore.Stub(a => a.Users).Return(new List<IMaterialPMemento> {plasticUser});
            fiberStore.Stub(a => a.Defaults).Return(new List<IMaterialFMemento>());
            fiberStore.Stub(a => a.Users).Return(new List<IMaterialFMemento> {defaultFiber});
            var userProfile = MockRepository.GenerateStub<IProfileMemento>();
            var profileStore = MockRepository.GenerateMock<ISettingsStore<IProfileMemento>>();
            profileStore.Stub(a => a.Defaults).Return(new List<IProfileMemento>());
            profileStore.Stub(a => a.Users).Return(new List<IProfileMemento> {userProfile});

            var serializer = MockRepository.GenerateMock<ISettingFileSerializer>();
            serializer.Stub(a => a.GetLastSession()).Return(null);

            var sessionHelperWrapper = MockRepository.GenerateMock<ISessionHelperWrapper>();
            var sessionStore = new SessionStore(serializer, plasticStore, fiberStore, printerStore, profileStore, factory, sessionHelperWrapper);
            emptySession.Raise(a => a.PropertyChanged +=
                    null,
                emptySession,
                new PropertyChangedEventArgs("ExtrudersPMaterials"));

            serializer.AssertWasCalled(a => a.SaveMemento(Arg<ISettingsMemento>.Is.Anything), options => options.Repeat.Times(2));
        }

        [Test]
        public void SessionStore_PropertyChanged_ExtrudersPFMaterials()
        {
            var extruderToMaterial = new ExtruderPMaterial();
            var extruderToMaterialPF = new ExtruderPFMaterial();
            var emptySession = MockRepository.GenerateMock<ISessionMemento>();
            emptySession.Stub(a => a.ExtrudersPMaterials).Return(new List<ExtruderPMaterial> {extruderToMaterial});
            emptySession.Stub(a => a.ExtrudersPFMaterials).Return(new List<ExtruderPFMaterial> {extruderToMaterialPF});
            var factory = MockRepository.GenerateStub<ISettingsFactory>();
            factory.Stub(a => a.CreateSessionMemento()).Return(emptySession);
            var printerStore = MockRepository.GenerateMock<ISettingsStore<IPrinterMemento>>();
            var userPrinter = MockRepository.GenerateStub<IPrinterMemento>();
            printerStore.Stub(a => a.Defaults).Return(new List<IPrinterMemento>());
            printerStore.Stub(a => a.Users).Return(new List<IPrinterMemento> {userPrinter});
            var plasticStore = MockRepository.GenerateMock<ISettingsStore<IMaterialPMemento>>();
            var plasticUser = MockRepository.GenerateStub<IMaterialPMemento>();
            var fiberStore = MockRepository.GenerateMock<ISettingsStore<IMaterialFMemento>>();
            var defaultFiber = MockRepository.GenerateStub<IMaterialFMemento>();
            plasticStore.Stub(a => a.Defaults).Return(new List<IMaterialPMemento>());
            plasticStore.Stub(a => a.Users).Return(new List<IMaterialPMemento> {plasticUser});
            fiberStore.Stub(a => a.Defaults).Return(new List<IMaterialFMemento>());
            fiberStore.Stub(a => a.Users).Return(new List<IMaterialFMemento> {defaultFiber});
            var userProfile = MockRepository.GenerateStub<IProfileMemento>();
            var profileStore = MockRepository.GenerateMock<ISettingsStore<IProfileMemento>>();
            profileStore.Stub(a => a.Defaults).Return(new List<IProfileMemento>());
            profileStore.Stub(a => a.Users).Return(new List<IProfileMemento> {userProfile});

            var sessionHelperWrapper = MockRepository.GenerateMock<ISessionHelperWrapper>();
            var serializer = MockRepository.GenerateMock<ISettingFileSerializer>();
            serializer.Stub(a => a.GetLastSession()).Return(null);

            var sessionStore = new SessionStore(serializer, plasticStore, fiberStore, printerStore, profileStore, factory, sessionHelperWrapper);
            emptySession.Raise(a => a.PropertyChanged +=
                    null,
                emptySession,
                new PropertyChangedEventArgs("ExtrudersPFMaterials"));

            serializer.AssertWasCalled(a => a.SaveMemento(Arg<ISettingsMemento>.Is.Anything), options => options.Repeat.Times(2));
        }

        [Test]
        public void SessionStore_PropertyChanged_AnyPropertiesToJustSave()
        {
            var extruderToMaterial = new ExtruderPMaterial();
            var extruderToMaterialPF = new ExtruderPFMaterial();
            var emptySession = MockRepository.GenerateMock<ISessionMemento>();
            emptySession.Stub(a => a.ExtrudersPMaterials).Return(new List<ExtruderPMaterial> {extruderToMaterial});
            emptySession.Stub(a => a.ExtrudersPFMaterials).Return(new List<ExtruderPFMaterial> {extruderToMaterialPF});
            var factory = MockRepository.GenerateStub<ISettingsFactory>();
            factory.Stub(a => a.CreateSessionMemento()).Return(emptySession);
            var printerStore = MockRepository.GenerateMock<ISettingsStore<IPrinterMemento>>();
            var userPrinter = MockRepository.GenerateStub<IPrinterMemento>();
            printerStore.Stub(a => a.Defaults).Return(new List<IPrinterMemento>());
            printerStore.Stub(a => a.Users).Return(new List<IPrinterMemento> {userPrinter});
            var plasticStore = MockRepository.GenerateMock<ISettingsStore<IMaterialPMemento>>();
            var fiberStore = MockRepository.GenerateMock<ISettingsStore<IMaterialFMemento>>();
            var plasticUser = MockRepository.GenerateStub<IMaterialPMemento>();
            var defaultFiber = MockRepository.GenerateStub<IMaterialFMemento>();
            plasticStore.Stub(a => a.Defaults).Return(new List<IMaterialPMemento>());
            plasticStore.Stub(a => a.Users).Return(new List<IMaterialPMemento> {plasticUser});
            fiberStore.Stub(a => a.Defaults).Return(new List<IMaterialFMemento>());
            fiberStore.Stub(a => a.Users).Return(new List<IMaterialFMemento> {defaultFiber});
            var userProfile = MockRepository.GenerateStub<IProfileMemento>();
            var profileStore = MockRepository.GenerateMock<ISettingsStore<IProfileMemento>>();
            profileStore.Stub(a => a.Defaults).Return(new List<IProfileMemento>());
            profileStore.Stub(a => a.Users).Return(new List<IProfileMemento> {userProfile});

            var serializer = MockRepository.GenerateMock<ISettingFileSerializer>();
            serializer.Stub(a => a.GetLastSession()).Return(null);
            var sessionHelperWrapper = MockRepository.GenerateMock<ISessionHelperWrapper>();

            var sessionStore = new SessionStore(serializer, plasticStore, fiberStore, printerStore, profileStore, factory, sessionHelperWrapper);
            emptySession.Raise(a => a.PropertyChanged +=
                    null,
                emptySession,
                new PropertyChangedEventArgs("Name"));

            serializer.AssertWasCalled(a => a.SaveMemento(Arg<ISettingsMemento>.Is.Anything), options => options.Repeat.Times(2));
        }

        [Test]
        public void SessionStore_PropertyChanged_ExtrudersSaveAndSyncronyze()
        {
            var extruderToMaterial = new ExtruderPMaterial();
            var extruderToMaterialPF = new ExtruderPFMaterial();
            var emptySession = MockRepository.GenerateMock<ISessionMemento>();
            emptySession.Stub(a => a.ExtrudersPMaterials).Return(new List<ExtruderPMaterial> {extruderToMaterial});
            emptySession.Stub(a => a.ExtrudersPFMaterials).Return(new List<ExtruderPFMaterial> {extruderToMaterialPF});
            var factory = MockRepository.GenerateStub<ISettingsFactory>();
            factory.Stub(a => a.CreateSessionMemento()).Return(emptySession);
            var printerStore = MockRepository.GenerateMock<ISettingsStore<IPrinterMemento>>();
            var userPrinter = MockRepository.GenerateStub<IPrinterMemento>();
            printerStore.Stub(a => a.Defaults).Return(new List<IPrinterMemento>());
            printerStore.Stub(a => a.Users).Return(new List<IPrinterMemento> {userPrinter});
            var plasticStore = MockRepository.GenerateMock<ISettingsStore<IMaterialPMemento>>();
            var fiberStore = MockRepository.GenerateMock<ISettingsStore<IMaterialFMemento>>();
            var plasticUser = MockRepository.GenerateStub<IMaterialPMemento>();
            var defaultFiber = MockRepository.GenerateStub<IMaterialFMemento>();
            plasticStore.Stub(a => a.Defaults).Return(new List<IMaterialPMemento>());
            plasticStore.Stub(a => a.Users).Return(new List<IMaterialPMemento> {plasticUser});
            fiberStore.Stub(a => a.Defaults).Return(new List<IMaterialFMemento>());
            fiberStore.Stub(a => a.Users).Return(new List<IMaterialFMemento> {defaultFiber});
            var userProfile = MockRepository.GenerateStub<IProfileMemento>();
            var profileStore = MockRepository.GenerateMock<ISettingsStore<IProfileMemento>>();
            profileStore.Stub(a => a.Defaults).Return(new List<IProfileMemento>());
            profileStore.Stub(a => a.Users).Return(new List<IProfileMemento> {userProfile});

            var serializer = MockRepository.GenerateMock<ISettingFileSerializer>();
            serializer.Stub(a => a.GetLastSession()).Return(null);
            var sessionHelperWrapper = MockRepository.GenerateMock<ISessionHelperWrapper>();

            var sessionStore = new SessionStore(serializer, plasticStore, fiberStore, printerStore, profileStore, factory, sessionHelperWrapper);
            emptySession.Raise(a => a.PropertyChanged +=
                    null,
                emptySession,
                new PropertyChangedEventArgs("Extruders"));

            serializer.AssertWasCalled(a => a.SaveMemento(Arg<ISettingsMemento>.Is.Anything), options => options.Repeat.Times(2));
        }

        [Test]
        public void SessionStore_CheckMaterialP_HasMaterial()
        {
            var plasticUser = new MaterialPMemento();
            var defaultFiber = new MaterialFMemento();

            var extruderToMaterial = new ExtruderPMaterial {Material = plasticUser};
            var extruderToMaterialPF = new ExtruderPFMaterial();
            var emptySession = MockRepository.GenerateMock<ISessionMemento>();
            emptySession.Stub(a => a.ExtrudersPMaterials).Return(new List<ExtruderPMaterial> {extruderToMaterial});
            emptySession.Stub(a => a.ExtrudersPFMaterials).Return(new List<ExtruderPFMaterial> {extruderToMaterialPF});
            var factory = MockRepository.GenerateStub<ISettingsFactory>();
            factory.Stub(a => a.CreateSessionMemento()).Return(emptySession);
            var printerStore = MockRepository.GenerateMock<ISettingsStore<IPrinterMemento>>();
            var userPrinter = MockRepository.GenerateStub<IPrinterMemento>();
            printerStore.Stub(a => a.Defaults).Return(new List<IPrinterMemento>());
            printerStore.Stub(a => a.Users).Return(new List<IPrinterMemento> {userPrinter});
            var plasticStore = MockRepository.GenerateMock<ISettingsStore<IMaterialPMemento>>();
            var fiberStore = MockRepository.GenerateMock<ISettingsStore<IMaterialFMemento>>();
            plasticStore.Stub(a => a.Defaults).Return(new List<IMaterialPMemento>());
            plasticStore.Stub(a => a.Users).Return(new List<IMaterialPMemento> {plasticUser});
            fiberStore.Stub(a => a.Defaults).Return(new List<IMaterialFMemento>());
            fiberStore.Stub(a => a.Users).Return(new List<IMaterialFMemento> {defaultFiber});
            var userProfile = MockRepository.GenerateStub<IProfileMemento>();
            var profileStore = MockRepository.GenerateMock<ISettingsStore<IProfileMemento>>();
            profileStore.Stub(a => a.Defaults).Return(new List<IProfileMemento>());
            profileStore.Stub(a => a.Users).Return(new List<IProfileMemento> {userProfile});

            var serializer = MockRepository.GenerateStub<ISettingFileSerializer>();
            serializer.Stub(a => a.GetLastSession()).Return(null);
            var sessionHelperWrapper = MockRepository.GenerateMock<ISessionHelperWrapper>();

            var sessionStore = new SessionStore(serializer, plasticStore, fiberStore, printerStore, profileStore, factory, sessionHelperWrapper);
            var chechMaterial = sessionStore.CheckItem(plasticUser);

            Assert.IsTrue(chechMaterial);
        }

        [Test]
        public void SessionStore_CheckMaterialP_HasNoMaterial()
        {
            var plasticUser = new MaterialPMemento();
            var defaultFiber = new MaterialFMemento();

            var extruderToMaterial = new ExtruderPMaterial {Material = plasticUser};
            var extruderToMaterialPF = new ExtruderPFMaterial();
            var emptySession = MockRepository.GenerateMock<ISessionMemento>();
            emptySession.Stub(a => a.ExtrudersPMaterials).Return(new List<ExtruderPMaterial> {extruderToMaterial});
            emptySession.Stub(a => a.ExtrudersPFMaterials).Return(new List<ExtruderPFMaterial> {extruderToMaterialPF});
            var factory = MockRepository.GenerateStub<ISettingsFactory>();
            factory.Stub(a => a.CreateSessionMemento()).Return(emptySession);
            var printerStore = MockRepository.GenerateMock<ISettingsStore<IPrinterMemento>>();
            var userPrinter = MockRepository.GenerateStub<IPrinterMemento>();
            printerStore.Stub(a => a.Defaults).Return(new List<IPrinterMemento>());
            printerStore.Stub(a => a.Users).Return(new List<IPrinterMemento> {userPrinter});
            var plasticStore = MockRepository.GenerateMock<ISettingsStore<IMaterialPMemento>>();
            var fiberStore = MockRepository.GenerateMock<ISettingsStore<IMaterialFMemento>>();
            plasticStore.Stub(a => a.Defaults).Return(new List<IMaterialPMemento>());
            plasticStore.Stub(a => a.Users).Return(new List<IMaterialPMemento> {plasticUser});
            fiberStore.Stub(a => a.Defaults).Return(new List<IMaterialFMemento>());
            fiberStore.Stub(a => a.Users).Return(new List<IMaterialFMemento> {defaultFiber});
            var userProfile = MockRepository.GenerateStub<IProfileMemento>();
            var profileStore = MockRepository.GenerateMock<ISettingsStore<IProfileMemento>>();
            profileStore.Stub(a => a.Defaults).Return(new List<IProfileMemento>());
            profileStore.Stub(a => a.Users).Return(new List<IProfileMemento> {userProfile});

            var serializer = MockRepository.GenerateStub<ISettingFileSerializer>();
            serializer.Stub(a => a.GetLastSession()).Return(null);
            var sessionHelperWrapper = MockRepository.GenerateMock<ISessionHelperWrapper>();

            var sessionStore = new SessionStore(serializer, plasticStore, fiberStore, printerStore, profileStore, factory, sessionHelperWrapper);

            var plasticForTest = new MaterialPMemento();
            plasticForTest.Name = "11";
            var chechMaterial = sessionStore.CheckItem(plasticForTest);
            Assert.IsFalse(chechMaterial);
        }

        [Test]
        public void SessionStore_CheckMaterial_FHasMaterial()
        {
            var plasticUser = new MaterialPMemento();
            var defaultFiber = new MaterialFMemento();

            var extruderToMaterial = new ExtruderPMaterial {Material = plasticUser};
            var extruderToMaterialPF = new ExtruderPFMaterial() {MaterialP = plasticUser, MaterialF = defaultFiber};
            var emptySession = MockRepository.GenerateMock<ISessionMemento>();
            emptySession.Stub(a => a.ExtrudersPMaterials).Return(new List<ExtruderPMaterial> {extruderToMaterial});
            emptySession.Stub(a => a.ExtrudersPFMaterials).Return(new List<ExtruderPFMaterial> {extruderToMaterialPF});
            var factory = MockRepository.GenerateStub<ISettingsFactory>();
            factory.Stub(a => a.CreateSessionMemento()).Return(emptySession);
            var printerStore = MockRepository.GenerateMock<ISettingsStore<IPrinterMemento>>();
            var userPrinter = MockRepository.GenerateStub<IPrinterMemento>();
            printerStore.Stub(a => a.Defaults).Return(new List<IPrinterMemento>());
            printerStore.Stub(a => a.Users).Return(new List<IPrinterMemento> {userPrinter});
            var plasticStore = MockRepository.GenerateMock<ISettingsStore<IMaterialPMemento>>();
            var fiberStore = MockRepository.GenerateMock<ISettingsStore<IMaterialFMemento>>();
            plasticStore.Stub(a => a.Defaults).Return(new List<IMaterialPMemento>());
            plasticStore.Stub(a => a.Users).Return(new List<IMaterialPMemento> {plasticUser});
            fiberStore.Stub(a => a.Defaults).Return(new List<IMaterialFMemento>());
            fiberStore.Stub(a => a.Users).Return(new List<IMaterialFMemento> {defaultFiber});
            var userProfile = MockRepository.GenerateStub<IProfileMemento>();
            var profileStore = MockRepository.GenerateMock<ISettingsStore<IProfileMemento>>();
            profileStore.Stub(a => a.Defaults).Return(new List<IProfileMemento>());
            profileStore.Stub(a => a.Users).Return(new List<IProfileMemento> {userProfile});

            var serializer = MockRepository.GenerateStub<ISettingFileSerializer>();
            serializer.Stub(a => a.GetLastSession()).Return(null);
            var sessionHelperWrapper = MockRepository.GenerateMock<ISessionHelperWrapper>();

            var sessionStore = new SessionStore(serializer, plasticStore, fiberStore, printerStore, profileStore, factory, sessionHelperWrapper);
            var chechMaterial = sessionStore.CheckItem(defaultFiber);

            Assert.IsTrue(chechMaterial);
        }

        [Test]
        public void SessionStore_CheckMaterial_FHasNoMaterial()
        {
            var plasticUser = new MaterialPMemento();
            var defaultFiber = new MaterialFMemento();

            var extruderToMaterial = new ExtruderPMaterial {Material = plasticUser};
            var extruderToMaterialPF = new ExtruderPFMaterial() {MaterialP = plasticUser, MaterialF = defaultFiber};
            var emptySession = MockRepository.GenerateMock<ISessionMemento>();
            emptySession.Stub(a => a.ExtrudersPMaterials).Return(new List<ExtruderPMaterial> {extruderToMaterial});
            emptySession.Stub(a => a.ExtrudersPFMaterials).Return(new List<ExtruderPFMaterial> {extruderToMaterialPF});
            var factory = MockRepository.GenerateStub<ISettingsFactory>();
            factory.Stub(a => a.CreateSessionMemento()).Return(emptySession);
            var printerStore = MockRepository.GenerateMock<ISettingsStore<IPrinterMemento>>();
            var userPrinter = MockRepository.GenerateStub<IPrinterMemento>();
            printerStore.Stub(a => a.Defaults).Return(new List<IPrinterMemento>());
            printerStore.Stub(a => a.Users).Return(new List<IPrinterMemento> {userPrinter});
            var plasticStore = MockRepository.GenerateMock<ISettingsStore<IMaterialPMemento>>();
            var fiberStore = MockRepository.GenerateMock<ISettingsStore<IMaterialFMemento>>();
            plasticStore.Stub(a => a.Defaults).Return(new List<IMaterialPMemento>());
            plasticStore.Stub(a => a.Users).Return(new List<IMaterialPMemento> {plasticUser});
            fiberStore.Stub(a => a.Defaults).Return(new List<IMaterialFMemento>());
            fiberStore.Stub(a => a.Users).Return(new List<IMaterialFMemento> {defaultFiber});
            var userProfile = MockRepository.GenerateStub<IProfileMemento>();
            var profileStore = MockRepository.GenerateMock<ISettingsStore<IProfileMemento>>();
            profileStore.Stub(a => a.Defaults).Return(new List<IProfileMemento>());
            profileStore.Stub(a => a.Users).Return(new List<IProfileMemento> {userProfile});

            var serializer = MockRepository.GenerateStub<ISettingFileSerializer>();
            serializer.Stub(a => a.GetLastSession()).Return(null);
            var sessionHelperWrapper = MockRepository.GenerateMock<ISessionHelperWrapper>();

            var sessionStore = new SessionStore(serializer, plasticStore, fiberStore, printerStore, profileStore, factory, sessionHelperWrapper);

            var anotherFiber = new MaterialFMemento();
            anotherFiber.Name = "11";
            var chechMaterial = sessionStore.CheckItem(anotherFiber);
            Assert.IsFalse(chechMaterial);
        }

        [Test]
        public void SessionStore_CheckExtruderP_SessionHasNotExtruder()
        {
            var plasticUser = new MaterialPMemento();
            var defaultFiber = new MaterialFMemento();

            var extruderToMaterial = new ExtruderPMaterial {Material = plasticUser};
            var extruderToMaterialPF = new ExtruderPFMaterial() {MaterialP = plasticUser, MaterialF = defaultFiber};
            var emptySession = MockRepository.GenerateMock<ISessionMemento>();
            emptySession.Stub(a => a.ExtrudersPMaterials).Return(new List<ExtruderPMaterial> {extruderToMaterial});
            emptySession.Stub(a => a.ExtrudersPFMaterials).Return(new List<ExtruderPFMaterial> {extruderToMaterialPF});
            var factory = MockRepository.GenerateStub<ISettingsFactory>();
            factory.Stub(a => a.CreateSessionMemento()).Return(emptySession);
            var printerStore = MockRepository.GenerateMock<ISettingsStore<IPrinterMemento>>();
            var userPrinter = MockRepository.GenerateStub<IPrinterMemento>();
            printerStore.Stub(a => a.Defaults).Return(new List<IPrinterMemento>());
            printerStore.Stub(a => a.Users).Return(new List<IPrinterMemento> {userPrinter});
            var plasticStore = MockRepository.GenerateMock<ISettingsStore<IMaterialPMemento>>();
            var fiberStore = MockRepository.GenerateMock<ISettingsStore<IMaterialFMemento>>();
            plasticStore.Stub(a => a.Defaults).Return(new List<IMaterialPMemento>());
            plasticStore.Stub(a => a.Users).Return(new List<IMaterialPMemento> {plasticUser});
            fiberStore.Stub(a => a.Defaults).Return(new List<IMaterialFMemento>());
            fiberStore.Stub(a => a.Users).Return(new List<IMaterialFMemento> {defaultFiber});
            var userProfile = MockRepository.GenerateStub<IProfileMemento>();
            var profileStore = MockRepository.GenerateMock<ISettingsStore<IProfileMemento>>();
            profileStore.Stub(a => a.Defaults).Return(new List<IProfileMemento>());
            profileStore.Stub(a => a.Users).Return(new List<IProfileMemento> {userProfile});

            emptySession.Stub(a => a.BrimExtruder).Return(0);
            emptySession.Stub(a => a.SkirtExtruder).Return(1);
            emptySession.Stub(a => a.Inset0Extruder).Return(2);
            emptySession.Stub(a => a.InsetPlasticExtruder).Return(3);
            emptySession.Stub(a => a.InfillSolidExtruder).Return(4);
            emptySession.Stub(a => a.InfillPlasticCellularExtruder).Return(5);
            emptySession.Stub(a => a.SupportExtruder).Return(6);

            var serializer = MockRepository.GenerateStub<ISettingFileSerializer>();
            serializer.Stub(a => a.GetLastSession()).Return(null);
            var sessionHelperWrapper = MockRepository.GenerateMock<ISessionHelperWrapper>();

            var sessionStore = new SessionStore(serializer, plasticStore, fiberStore, printerStore, profileStore, factory, sessionHelperWrapper);

            var checkingExtruder = MockRepository.GenerateStub<IExtruderPMemento>();
            checkingExtruder.ExtruderIndex = 7;

            var hasExtruder = sessionStore.CheckExtruder(checkingExtruder);

            Assert.IsFalse(hasExtruder);
        }

        [Test]
        public void SessionStore_CheckExtruderP_SessionHasExtruderBrim()
        {
            var plasticUser = new MaterialPMemento();
            var defaultFiber = new MaterialFMemento();

            var extruderToMaterial = new ExtruderPMaterial {Material = plasticUser};
            var extruderToMaterialPF = new ExtruderPFMaterial() {MaterialP = plasticUser, MaterialF = defaultFiber};
            var emptySession = MockRepository.GenerateMock<ISessionMemento>();
            emptySession.Stub(a => a.ExtrudersPMaterials).Return(new List<ExtruderPMaterial> {extruderToMaterial});
            emptySession.Stub(a => a.ExtrudersPFMaterials).Return(new List<ExtruderPFMaterial> {extruderToMaterialPF});
            var factory = MockRepository.GenerateStub<ISettingsFactory>();
            factory.Stub(a => a.CreateSessionMemento()).Return(emptySession);
            var printerStore = MockRepository.GenerateMock<ISettingsStore<IPrinterMemento>>();
            var userPrinter = MockRepository.GenerateStub<IPrinterMemento>();
            printerStore.Stub(a => a.Defaults).Return(new List<IPrinterMemento>());
            printerStore.Stub(a => a.Users).Return(new List<IPrinterMemento> {userPrinter});
            var plasticStore = MockRepository.GenerateMock<ISettingsStore<IMaterialPMemento>>();
            var fiberStore = MockRepository.GenerateMock<ISettingsStore<IMaterialFMemento>>();
            plasticStore.Stub(a => a.Defaults).Return(new List<IMaterialPMemento>());
            plasticStore.Stub(a => a.Users).Return(new List<IMaterialPMemento> {plasticUser});
            fiberStore.Stub(a => a.Defaults).Return(new List<IMaterialFMemento>());
            fiberStore.Stub(a => a.Users).Return(new List<IMaterialFMemento> {defaultFiber});
            var userProfile = MockRepository.GenerateStub<IProfileMemento>();
            var profileStore = MockRepository.GenerateMock<ISettingsStore<IProfileMemento>>();
            profileStore.Stub(a => a.Defaults).Return(new List<IProfileMemento>());
            profileStore.Stub(a => a.Users).Return(new List<IProfileMemento> {userProfile});

            emptySession.Stub(a => a.BrimExtruder).Return(0);
            emptySession.Stub(a => a.SkirtExtruder).Return(1);
            emptySession.Stub(a => a.Inset0Extruder).Return(2);
            emptySession.Stub(a => a.InsetPlasticExtruder).Return(3);
            emptySession.Stub(a => a.InfillSolidExtruder).Return(4);
            emptySession.Stub(a => a.InfillPlasticCellularExtruder).Return(5);
            emptySession.Stub(a => a.SupportExtruder).Return(6);

            var serializer = MockRepository.GenerateStub<ISettingFileSerializer>();
            serializer.Stub(a => a.GetLastSession()).Return(null);
            var sessionHelperWrapper = MockRepository.GenerateMock<ISessionHelperWrapper>();

            var sessionStore = new SessionStore(serializer, plasticStore, fiberStore, printerStore, profileStore, factory, sessionHelperWrapper);

            var checkingExtruder = MockRepository.GenerateStub<IExtruderPMemento>();
            checkingExtruder.ExtruderIndex = 0;

            var hasExtruder = sessionStore.CheckExtruder(checkingExtruder);

            Assert.IsTrue(hasExtruder);
        }

        [Test]
        public void SessionStore_CheckExtruderP_SessionHasExtruderSkirt()
        {
            var plasticUser = new MaterialPMemento();
            var defaultFiber = new MaterialFMemento();

            var extruderToMaterial = new ExtruderPMaterial {Material = plasticUser};
            var extruderToMaterialPF = new ExtruderPFMaterial() {MaterialP = plasticUser, MaterialF = defaultFiber};
            var emptySession = MockRepository.GenerateMock<ISessionMemento>();
            emptySession.Stub(a => a.ExtrudersPMaterials).Return(new List<ExtruderPMaterial> {extruderToMaterial});
            emptySession.Stub(a => a.ExtrudersPFMaterials).Return(new List<ExtruderPFMaterial> {extruderToMaterialPF});
            var factory = MockRepository.GenerateStub<ISettingsFactory>();
            factory.Stub(a => a.CreateSessionMemento()).Return(emptySession);
            var printerStore = MockRepository.GenerateMock<ISettingsStore<IPrinterMemento>>();
            var userPrinter = MockRepository.GenerateStub<IPrinterMemento>();
            printerStore.Stub(a => a.Defaults).Return(new List<IPrinterMemento>());
            printerStore.Stub(a => a.Users).Return(new List<IPrinterMemento> {userPrinter});
            var plasticStore = MockRepository.GenerateMock<ISettingsStore<IMaterialPMemento>>();
            var fiberStore = MockRepository.GenerateMock<ISettingsStore<IMaterialFMemento>>();
            plasticStore.Stub(a => a.Defaults).Return(new List<IMaterialPMemento>());
            plasticStore.Stub(a => a.Users).Return(new List<IMaterialPMemento> {plasticUser});
            fiberStore.Stub(a => a.Defaults).Return(new List<IMaterialFMemento>());
            fiberStore.Stub(a => a.Users).Return(new List<IMaterialFMemento> {defaultFiber});
            var userProfile = MockRepository.GenerateStub<IProfileMemento>();
            var profileStore = MockRepository.GenerateMock<ISettingsStore<IProfileMemento>>();
            profileStore.Stub(a => a.Defaults).Return(new List<IProfileMemento>());
            profileStore.Stub(a => a.Users).Return(new List<IProfileMemento> {userProfile});

            emptySession.Stub(a => a.BrimExtruder).Return(0);
            emptySession.Stub(a => a.SkirtExtruder).Return(1);
            emptySession.Stub(a => a.Inset0Extruder).Return(2);
            emptySession.Stub(a => a.InsetPlasticExtruder).Return(3);
            emptySession.Stub(a => a.InfillSolidExtruder).Return(4);
            emptySession.Stub(a => a.InfillPlasticCellularExtruder).Return(5);
            emptySession.Stub(a => a.SupportExtruder).Return(6);

            var serializer = MockRepository.GenerateStub<ISettingFileSerializer>();
            serializer.Stub(a => a.GetLastSession()).Return(null);
            var sessionHelperWrapper = MockRepository.GenerateMock<ISessionHelperWrapper>();

            var sessionStore = new SessionStore(serializer, plasticStore, fiberStore, printerStore, profileStore, factory, sessionHelperWrapper);

            var checkingExtruder = MockRepository.GenerateStub<IExtruderPMemento>();
            checkingExtruder.ExtruderIndex = 1;

            var hasExtruder = sessionStore.CheckExtruder(checkingExtruder);

            Assert.IsTrue(hasExtruder);
        }

        [Test]
        public void SessionStore_CheckExtruderP_SessionHasExtruderInset0()
        {
            var plasticUser = new MaterialPMemento();
            var defaultFiber = new MaterialFMemento();

            var extruderToMaterial = new ExtruderPMaterial {Material = plasticUser};
            var extruderToMaterialPF = new ExtruderPFMaterial() {MaterialP = plasticUser, MaterialF = defaultFiber};
            var emptySession = MockRepository.GenerateMock<ISessionMemento>();
            emptySession.Stub(a => a.ExtrudersPMaterials).Return(new List<ExtruderPMaterial> {extruderToMaterial});
            emptySession.Stub(a => a.ExtrudersPFMaterials).Return(new List<ExtruderPFMaterial> {extruderToMaterialPF});
            var factory = MockRepository.GenerateStub<ISettingsFactory>();
            factory.Stub(a => a.CreateSessionMemento()).Return(emptySession);
            var printerStore = MockRepository.GenerateMock<ISettingsStore<IPrinterMemento>>();
            var userPrinter = MockRepository.GenerateStub<IPrinterMemento>();
            printerStore.Stub(a => a.Defaults).Return(new List<IPrinterMemento>());
            printerStore.Stub(a => a.Users).Return(new List<IPrinterMemento> {userPrinter});
            var plasticStore = MockRepository.GenerateMock<ISettingsStore<IMaterialPMemento>>();
            var fiberStore = MockRepository.GenerateMock<ISettingsStore<IMaterialFMemento>>();
            plasticStore.Stub(a => a.Defaults).Return(new List<IMaterialPMemento>());
            plasticStore.Stub(a => a.Users).Return(new List<IMaterialPMemento> {plasticUser});
            fiberStore.Stub(a => a.Defaults).Return(new List<IMaterialFMemento>());
            fiberStore.Stub(a => a.Users).Return(new List<IMaterialFMemento> {defaultFiber});
            var userProfile = MockRepository.GenerateStub<IProfileMemento>();
            var profileStore = MockRepository.GenerateMock<ISettingsStore<IProfileMemento>>();
            profileStore.Stub(a => a.Defaults).Return(new List<IProfileMemento>());
            profileStore.Stub(a => a.Users).Return(new List<IProfileMemento> {userProfile});

            emptySession.Stub(a => a.BrimExtruder).Return(0);
            emptySession.Stub(a => a.SkirtExtruder).Return(1);
            emptySession.Stub(a => a.Inset0Extruder).Return(2);
            emptySession.Stub(a => a.InsetPlasticExtruder).Return(3);
            emptySession.Stub(a => a.InfillSolidExtruder).Return(4);
            emptySession.Stub(a => a.InfillPlasticCellularExtruder).Return(5);
            emptySession.Stub(a => a.SupportExtruder).Return(6);

            var serializer = MockRepository.GenerateStub<ISettingFileSerializer>();
            serializer.Stub(a => a.GetLastSession()).Return(null);
            var sessionHelperWrapper = MockRepository.GenerateMock<ISessionHelperWrapper>();

            var sessionStore = new SessionStore(serializer, plasticStore, fiberStore, printerStore, profileStore, factory, sessionHelperWrapper);

            var checkingExtruder = MockRepository.GenerateStub<IExtruderPMemento>();
            checkingExtruder.ExtruderIndex = 2;

            var hasExtruder = sessionStore.CheckExtruder(checkingExtruder);

            Assert.IsTrue(hasExtruder);
        }

        [Test]
        public void SessionStore_CheckExtruderP_SessionHasExtruderPlasticExtruder()
        {
            var plasticUser = new MaterialPMemento();
            var defaultFiber = new MaterialFMemento();

            var extruderToMaterial = new ExtruderPMaterial {Material = plasticUser};
            var extruderToMaterialPF = new ExtruderPFMaterial() {MaterialP = plasticUser, MaterialF = defaultFiber};
            var emptySession = MockRepository.GenerateMock<ISessionMemento>();
            emptySession.Stub(a => a.ExtrudersPMaterials).Return(new List<ExtruderPMaterial> {extruderToMaterial});
            emptySession.Stub(a => a.ExtrudersPFMaterials).Return(new List<ExtruderPFMaterial> {extruderToMaterialPF});
            var factory = MockRepository.GenerateStub<ISettingsFactory>();
            factory.Stub(a => a.CreateSessionMemento()).Return(emptySession);
            var printerStore = MockRepository.GenerateMock<ISettingsStore<IPrinterMemento>>();
            var userPrinter = MockRepository.GenerateStub<IPrinterMemento>();
            printerStore.Stub(a => a.Defaults).Return(new List<IPrinterMemento>());
            printerStore.Stub(a => a.Users).Return(new List<IPrinterMemento> {userPrinter});
            var plasticStore = MockRepository.GenerateMock<ISettingsStore<IMaterialPMemento>>();
            var fiberStore = MockRepository.GenerateMock<ISettingsStore<IMaterialFMemento>>();
            plasticStore.Stub(a => a.Defaults).Return(new List<IMaterialPMemento>());
            plasticStore.Stub(a => a.Users).Return(new List<IMaterialPMemento> {plasticUser});
            fiberStore.Stub(a => a.Defaults).Return(new List<IMaterialFMemento>());
            fiberStore.Stub(a => a.Users).Return(new List<IMaterialFMemento> {defaultFiber});
            var userProfile = MockRepository.GenerateStub<IProfileMemento>();
            var profileStore = MockRepository.GenerateMock<ISettingsStore<IProfileMemento>>();
            profileStore.Stub(a => a.Defaults).Return(new List<IProfileMemento>());
            profileStore.Stub(a => a.Users).Return(new List<IProfileMemento> {userProfile});

            emptySession.Stub(a => a.BrimExtruder).Return(0);
            emptySession.Stub(a => a.SkirtExtruder).Return(1);
            emptySession.Stub(a => a.Inset0Extruder).Return(2);
            emptySession.Stub(a => a.InsetPlasticExtruder).Return(3);
            emptySession.Stub(a => a.InfillSolidExtruder).Return(4);
            emptySession.Stub(a => a.InfillPlasticCellularExtruder).Return(5);
            emptySession.Stub(a => a.SupportExtruder).Return(6);

            var serializer = MockRepository.GenerateStub<ISettingFileSerializer>();
            serializer.Stub(a => a.GetLastSession()).Return(null);
            var sessionHelperWrapper = MockRepository.GenerateMock<ISessionHelperWrapper>();

            var sessionStore = new SessionStore(serializer, plasticStore, fiberStore, printerStore, profileStore, factory, sessionHelperWrapper);

            var checkingExtruder = MockRepository.GenerateStub<IExtruderPMemento>();
            checkingExtruder.ExtruderIndex = 3;

            var hasExtruder = sessionStore.CheckExtruder(checkingExtruder);

            Assert.IsTrue(hasExtruder);
        }

        [Test]
        public void SessionStore_CheckExtruderP_SessionHasExtruderInfillSolidExtruder()
        {
            var plasticUser = new MaterialPMemento();
            var defaultFiber = new MaterialFMemento();

            var extruderToMaterial = new ExtruderPMaterial {Material = plasticUser};
            var extruderToMaterialPF = new ExtruderPFMaterial() {MaterialP = plasticUser, MaterialF = defaultFiber};
            var emptySession = MockRepository.GenerateMock<ISessionMemento>();
            emptySession.Stub(a => a.ExtrudersPMaterials).Return(new List<ExtruderPMaterial> {extruderToMaterial});
            emptySession.Stub(a => a.ExtrudersPFMaterials).Return(new List<ExtruderPFMaterial> {extruderToMaterialPF});
            var factory = MockRepository.GenerateStub<ISettingsFactory>();
            factory.Stub(a => a.CreateSessionMemento()).Return(emptySession);
            var printerStore = MockRepository.GenerateMock<ISettingsStore<IPrinterMemento>>();
            var userPrinter = MockRepository.GenerateStub<IPrinterMemento>();
            printerStore.Stub(a => a.Defaults).Return(new List<IPrinterMemento>());
            printerStore.Stub(a => a.Users).Return(new List<IPrinterMemento> {userPrinter});
            var plasticStore = MockRepository.GenerateMock<ISettingsStore<IMaterialPMemento>>();
            var fiberStore = MockRepository.GenerateMock<ISettingsStore<IMaterialFMemento>>();
            plasticStore.Stub(a => a.Defaults).Return(new List<IMaterialPMemento>());
            plasticStore.Stub(a => a.Users).Return(new List<IMaterialPMemento> {plasticUser});
            fiberStore.Stub(a => a.Defaults).Return(new List<IMaterialFMemento>());
            fiberStore.Stub(a => a.Users).Return(new List<IMaterialFMemento> {defaultFiber});
            var userProfile = MockRepository.GenerateStub<IProfileMemento>();
            var profileStore = MockRepository.GenerateMock<ISettingsStore<IProfileMemento>>();
            profileStore.Stub(a => a.Defaults).Return(new List<IProfileMemento>());
            profileStore.Stub(a => a.Users).Return(new List<IProfileMemento> {userProfile});

            emptySession.Stub(a => a.BrimExtruder).Return(0);
            emptySession.Stub(a => a.SkirtExtruder).Return(1);
            emptySession.Stub(a => a.Inset0Extruder).Return(2);
            emptySession.Stub(a => a.InsetPlasticExtruder).Return(3);
            emptySession.Stub(a => a.InfillSolidExtruder).Return(4);
            emptySession.Stub(a => a.InfillPlasticCellularExtruder).Return(5);
            emptySession.Stub(a => a.SupportExtruder).Return(6);

            var serializer = MockRepository.GenerateStub<ISettingFileSerializer>();
            serializer.Stub(a => a.GetLastSession()).Return(null);
            var sessionHelperWrapper = MockRepository.GenerateMock<ISessionHelperWrapper>();

            var sessionStore = new SessionStore(serializer, plasticStore, fiberStore, printerStore, profileStore, factory, sessionHelperWrapper);

            var checkingExtruder = MockRepository.GenerateStub<IExtruderPMemento>();
            checkingExtruder.ExtruderIndex = 4;

            var hasExtruder = sessionStore.CheckExtruder(checkingExtruder);

            Assert.IsTrue(hasExtruder);
        }

        [Test]
        public void SessionStore_CheckExtruderP_SessionHasExtruderInfillPlasticCellularExtruder()
        {
            var plasticUser = new MaterialPMemento();
            var defaultFiber = new MaterialFMemento();

            var extruderToMaterial = new ExtruderPMaterial {Material = plasticUser};
            var extruderToMaterialPF = new ExtruderPFMaterial() {MaterialP = plasticUser, MaterialF = defaultFiber};
            var emptySession = MockRepository.GenerateMock<ISessionMemento>();
            emptySession.Stub(a => a.ExtrudersPMaterials).Return(new List<ExtruderPMaterial> {extruderToMaterial});
            emptySession.Stub(a => a.ExtrudersPFMaterials).Return(new List<ExtruderPFMaterial> {extruderToMaterialPF});
            var factory = MockRepository.GenerateStub<ISettingsFactory>();
            factory.Stub(a => a.CreateSessionMemento()).Return(emptySession);
            var printerStore = MockRepository.GenerateMock<ISettingsStore<IPrinterMemento>>();
            var userPrinter = MockRepository.GenerateStub<IPrinterMemento>();
            printerStore.Stub(a => a.Defaults).Return(new List<IPrinterMemento>());
            printerStore.Stub(a => a.Users).Return(new List<IPrinterMemento> {userPrinter});
            var plasticStore = MockRepository.GenerateMock<ISettingsStore<IMaterialPMemento>>();
            var fiberStore = MockRepository.GenerateMock<ISettingsStore<IMaterialFMemento>>();
            plasticStore.Stub(a => a.Defaults).Return(new List<IMaterialPMemento>());
            plasticStore.Stub(a => a.Users).Return(new List<IMaterialPMemento> {plasticUser});
            fiberStore.Stub(a => a.Defaults).Return(new List<IMaterialFMemento>());
            fiberStore.Stub(a => a.Users).Return(new List<IMaterialFMemento> {defaultFiber});
            var userProfile = MockRepository.GenerateStub<IProfileMemento>();
            var profileStore = MockRepository.GenerateMock<ISettingsStore<IProfileMemento>>();
            profileStore.Stub(a => a.Defaults).Return(new List<IProfileMemento>());
            profileStore.Stub(a => a.Users).Return(new List<IProfileMemento> {userProfile});

            emptySession.Stub(a => a.BrimExtruder).Return(0);
            emptySession.Stub(a => a.SkirtExtruder).Return(1);
            emptySession.Stub(a => a.Inset0Extruder).Return(2);
            emptySession.Stub(a => a.InsetPlasticExtruder).Return(3);
            emptySession.Stub(a => a.InfillSolidExtruder).Return(4);
            emptySession.Stub(a => a.InfillPlasticCellularExtruder).Return(5);
            emptySession.Stub(a => a.SupportExtruder).Return(6);

            var serializer = MockRepository.GenerateStub<ISettingFileSerializer>();
            serializer.Stub(a => a.GetLastSession()).Return(null);
            var sessionHelperWrapper = MockRepository.GenerateMock<ISessionHelperWrapper>();

            var sessionStore = new SessionStore(serializer, plasticStore, fiberStore, printerStore, profileStore, factory, sessionHelperWrapper);

            var checkingExtruder = MockRepository.GenerateStub<IExtruderPMemento>();
            checkingExtruder.ExtruderIndex = 5;

            var hasExtruder = sessionStore.CheckExtruder(checkingExtruder);

            Assert.IsTrue(hasExtruder);
        }

        [Test]
        public void SessionStore_CheckExtruderP_SessionHasExtruderSupportExtruder()
        {
            var plasticUser = new MaterialPMemento();
            var defaultFiber = new MaterialFMemento();

            var extruderToMaterial = new ExtruderPMaterial {Material = plasticUser};
            var extruderToMaterialPF = new ExtruderPFMaterial() {MaterialP = plasticUser, MaterialF = defaultFiber};
            var emptySession = MockRepository.GenerateMock<ISessionMemento>();
            emptySession.Stub(a => a.ExtrudersPMaterials).Return(new List<ExtruderPMaterial> {extruderToMaterial});
            emptySession.Stub(a => a.ExtrudersPFMaterials).Return(new List<ExtruderPFMaterial> {extruderToMaterialPF});
            var factory = MockRepository.GenerateStub<ISettingsFactory>();
            factory.Stub(a => a.CreateSessionMemento()).Return(emptySession);
            var printerStore = MockRepository.GenerateMock<ISettingsStore<IPrinterMemento>>();
            var userPrinter = MockRepository.GenerateStub<IPrinterMemento>();
            printerStore.Stub(a => a.Defaults).Return(new List<IPrinterMemento>());
            printerStore.Stub(a => a.Users).Return(new List<IPrinterMemento> {userPrinter});
            var plasticStore = MockRepository.GenerateMock<ISettingsStore<IMaterialPMemento>>();
            var fiberStore = MockRepository.GenerateMock<ISettingsStore<IMaterialFMemento>>();
            plasticStore.Stub(a => a.Defaults).Return(new List<IMaterialPMemento>());
            plasticStore.Stub(a => a.Users).Return(new List<IMaterialPMemento> {plasticUser});
            fiberStore.Stub(a => a.Defaults).Return(new List<IMaterialFMemento>());
            fiberStore.Stub(a => a.Users).Return(new List<IMaterialFMemento> {defaultFiber});
            var userProfile = MockRepository.GenerateStub<IProfileMemento>();
            var profileStore = MockRepository.GenerateMock<ISettingsStore<IProfileMemento>>();
            profileStore.Stub(a => a.Defaults).Return(new List<IProfileMemento>());
            profileStore.Stub(a => a.Users).Return(new List<IProfileMemento> {userProfile});

            emptySession.Stub(a => a.BrimExtruder).Return(0);
            emptySession.Stub(a => a.SkirtExtruder).Return(1);
            emptySession.Stub(a => a.Inset0Extruder).Return(2);
            emptySession.Stub(a => a.InsetPlasticExtruder).Return(3);
            emptySession.Stub(a => a.InfillSolidExtruder).Return(4);
            emptySession.Stub(a => a.InfillPlasticCellularExtruder).Return(5);
            emptySession.Stub(a => a.SupportExtruder).Return(6);

            var serializer = MockRepository.GenerateStub<ISettingFileSerializer>();
            serializer.Stub(a => a.GetLastSession()).Return(null);
            var sessionHelperWrapper = MockRepository.GenerateMock<ISessionHelperWrapper>();

            var sessionStore = new SessionStore(serializer, plasticStore, fiberStore, printerStore, profileStore, factory, sessionHelperWrapper);

            var checkingExtruder = MockRepository.GenerateStub<IExtruderPMemento>();
            checkingExtruder.ExtruderIndex = 6;

            var hasExtruder = sessionStore.CheckExtruder(checkingExtruder);

            Assert.IsTrue(hasExtruder);
        }

        [Test]
        public void SessionStore_CheckExtruderPF_SessionHasNotExtruder()
        {
            var plasticUser = new MaterialPMemento();
            var defaultFiber = new MaterialFMemento();

            var extruderToMaterial = new ExtruderPMaterial {Material = plasticUser};
            var extruderToMaterialPF = new ExtruderPFMaterial() {MaterialP = plasticUser, MaterialF = defaultFiber};
            var emptySession = MockRepository.GenerateMock<ISessionMemento>();
            emptySession.Stub(a => a.ExtrudersPMaterials).Return(new List<ExtruderPMaterial> {extruderToMaterial});
            emptySession.Stub(a => a.ExtrudersPFMaterials).Return(new List<ExtruderPFMaterial> {extruderToMaterialPF});
            var factory = MockRepository.GenerateStub<ISettingsFactory>();
            factory.Stub(a => a.CreateSessionMemento()).Return(emptySession);
            var printerStore = MockRepository.GenerateMock<ISettingsStore<IPrinterMemento>>();
            var userPrinter = MockRepository.GenerateStub<IPrinterMemento>();
            printerStore.Stub(a => a.Defaults).Return(new List<IPrinterMemento>());
            printerStore.Stub(a => a.Users).Return(new List<IPrinterMemento> {userPrinter});
            var plasticStore = MockRepository.GenerateMock<ISettingsStore<IMaterialPMemento>>();
            var fiberStore = MockRepository.GenerateMock<ISettingsStore<IMaterialFMemento>>();
            plasticStore.Stub(a => a.Defaults).Return(new List<IMaterialPMemento>());
            plasticStore.Stub(a => a.Users).Return(new List<IMaterialPMemento> {plasticUser});
            fiberStore.Stub(a => a.Defaults).Return(new List<IMaterialFMemento>());
            fiberStore.Stub(a => a.Users).Return(new List<IMaterialFMemento> {defaultFiber});
            var userProfile = MockRepository.GenerateStub<IProfileMemento>();
            var profileStore = MockRepository.GenerateMock<ISettingsStore<IProfileMemento>>();
            profileStore.Stub(a => a.Defaults).Return(new List<IProfileMemento>());
            profileStore.Stub(a => a.Users).Return(new List<IProfileMemento> {userProfile});

            emptySession.Stub(a => a.BrimExtruder).Return(0);
            emptySession.Stub(a => a.SkirtExtruder).Return(1);
            emptySession.Stub(a => a.Inset0Extruder).Return(2);
            emptySession.Stub(a => a.InsetPlasticExtruder).Return(3);
            emptySession.Stub(a => a.InfillSolidExtruder).Return(4);
            emptySession.Stub(a => a.InfillPlasticCellularExtruder).Return(5);
            emptySession.Stub(a => a.SupportExtruder).Return(6);
            emptySession.Stub(a => a.InsetFiberExtruder).Return(null);

            var serializer = MockRepository.GenerateStub<ISettingFileSerializer>();
            serializer.Stub(a => a.GetLastSession()).Return(null);
            var sessionHelperWrapper = MockRepository.GenerateMock<ISessionHelperWrapper>();

            var sessionStore = new SessionStore(serializer, plasticStore, fiberStore, printerStore, profileStore, factory, sessionHelperWrapper);

            var checkingExtruder = MockRepository.GenerateStub<IExtruderPFMemento>();
            checkingExtruder.ExtruderIndex = 0;

            var hasExtruder = sessionStore.CheckExtruder(checkingExtruder);

            Assert.IsFalse(hasExtruder);
        }

        [Test]
        public void SessionStore_CheckExtruderPF_SessionHasExtruder()
        {
            var plasticUser = new MaterialPMemento();
            var defaultFiber = new MaterialFMemento();

            var extruderToMaterial = new ExtruderPMaterial {Material = plasticUser};
            var extruderToMaterialPF = new ExtruderPFMaterial() {MaterialP = plasticUser, MaterialF = defaultFiber};
            var emptySession = MockRepository.GenerateMock<ISessionMemento>();
            emptySession.Stub(a => a.ExtrudersPMaterials).Return(new List<ExtruderPMaterial> {extruderToMaterial});
            emptySession.Stub(a => a.ExtrudersPFMaterials).Return(new List<ExtruderPFMaterial> {extruderToMaterialPF});
            var factory = MockRepository.GenerateStub<ISettingsFactory>();
            factory.Stub(a => a.CreateSessionMemento()).Return(emptySession);
            var printerStore = MockRepository.GenerateMock<ISettingsStore<IPrinterMemento>>();
            var userPrinter = MockRepository.GenerateStub<IPrinterMemento>();
            printerStore.Stub(a => a.Defaults).Return(new List<IPrinterMemento>());
            printerStore.Stub(a => a.Users).Return(new List<IPrinterMemento> {userPrinter});
            var plasticStore = MockRepository.GenerateMock<ISettingsStore<IMaterialPMemento>>();
            var fiberStore = MockRepository.GenerateMock<ISettingsStore<IMaterialFMemento>>();
            plasticStore.Stub(a => a.Defaults).Return(new List<IMaterialPMemento>());
            plasticStore.Stub(a => a.Users).Return(new List<IMaterialPMemento> {plasticUser});
            fiberStore.Stub(a => a.Defaults).Return(new List<IMaterialFMemento>());
            fiberStore.Stub(a => a.Users).Return(new List<IMaterialFMemento> {defaultFiber});
            var userProfile = MockRepository.GenerateStub<IProfileMemento>();
            var profileStore = MockRepository.GenerateMock<ISettingsStore<IProfileMemento>>();
            profileStore.Stub(a => a.Defaults).Return(new List<IProfileMemento>());
            profileStore.Stub(a => a.Users).Return(new List<IProfileMemento> {userProfile});

            emptySession.Stub(a => a.BrimExtruder).Return(0);
            emptySession.Stub(a => a.SkirtExtruder).Return(1);
            emptySession.Stub(a => a.Inset0Extruder).Return(2);
            emptySession.Stub(a => a.InsetPlasticExtruder).Return(3);
            emptySession.Stub(a => a.InfillSolidExtruder).Return(4);
            emptySession.Stub(a => a.InfillPlasticCellularExtruder).Return(5);
            emptySession.Stub(a => a.SupportExtruder).Return(6);
            emptySession.Stub(a => a.InsetFiberExtruder).Return(7);

            var serializer = MockRepository.GenerateStub<ISettingFileSerializer>();
            serializer.Stub(a => a.GetLastSession()).Return(null);
            var sessionHelperWrapper = MockRepository.GenerateMock<ISessionHelperWrapper>();

            var sessionStore = new SessionStore(serializer, plasticStore, fiberStore, printerStore, profileStore, factory, sessionHelperWrapper);

            var checkingExtruder = MockRepository.GenerateStub<IExtruderPFMemento>();
            checkingExtruder.ExtruderIndex = 7;

            var hasExtruder = sessionStore.CheckExtruder(checkingExtruder);

            Assert.IsTrue(hasExtruder);
        }

        [Test]
        public void SessionStore_CheckPrinter_NoPrinter()
        {
            var currentSession = MockRepository.GenerateStub<ISessionMemento>();
            currentSession.Printer = null;

            var sessionStore = new SessionStore(currentSession);

            var checkingPrinter = MockRepository.GenerateStub<IPrinterMemento>();
            var res = sessionStore.CheckItem((ISettingsMemento)checkingPrinter);

            Assert.IsFalse(res);
        }

        [Test]
        public void SessionStore_CheckPrinter_PrinterEquals()
        {
            var factory = new SettingsFactory();

            var currentPriner = new PrinterMemento(factory);
            currentPriner.Name = "1";

            var currentSession = MockRepository.GenerateStub<ISessionMemento>();
            currentSession.Printer = currentPriner;

            var sessionStore = new SessionStore(currentSession);

            var checkingPrinter = new PrinterMemento(factory);
            checkingPrinter.Name = "1";

            var res = sessionStore.CheckItem((ISettingsMemento)checkingPrinter);

            Assert.IsTrue(res);
        }

        [Test]
        public void SessionStore_CheckProfile_NoProfile()
        {
            var currentSession = MockRepository.GenerateStub<ISessionMemento>();
            currentSession.Profile = null;

            var sessionStore = new SessionStore(currentSession);

            var checkingProfile = MockRepository.GenerateStub<IProfileMemento>();
            var res = sessionStore.CheckItem((ISettingsMemento)checkingProfile);

            Assert.IsFalse(res);
        }

        [Test]
        public void SessionStore_CheckProfile_PrinterEquals()
        {
            var factory = new SettingsFactory();

            var currentProfile = new ProfileMemento();
            currentProfile.Name = "1";

            var currentSession = MockRepository.GenerateStub<ISessionMemento>();
            currentSession.Profile = currentProfile;

            var sessionStore = new SessionStore(currentSession);

            var checkingProfile = new ProfileMemento();
            checkingProfile.Name = "1";

            var res = sessionStore.CheckItem((ISettingsMemento)checkingProfile);

            Assert.IsTrue(res);
        }

        [Test]
        public void SessionStore_Import_UnsuccessImport()
        {
            var serializer = MockRepository.GenerateStub<ISettingFileSerializer>();
            serializer.Stub(a => a.GetLastSession()).Return(null);


            var sessionMemento = MockRepository.GenerateStub<ISessionMemento>();
            var factory = MockRepository.GenerateStub<ISettingsFactory>();
            factory.Stub(a => a.CreateSessionMemento()).Return(sessionMemento);
            var sessionHelperWrapper = MockRepository.GenerateMock<ISessionHelperWrapper>();

            var sessionStore = new SessionStore(serializer, null, null, null, null, factory, sessionHelperWrapper);
            serializer.Stub(a => a.ImportSession("path")).Throw(new JsonSerializationException());

            Assert.Throws<JsonSerializationException>(() => sessionStore.Import("path"));
        }
    }
}