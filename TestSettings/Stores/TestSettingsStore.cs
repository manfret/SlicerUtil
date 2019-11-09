using System.Collections.Generic;
using Newtonsoft.Json;
using NUnit.Framework;
using Rhino.Mocks;
using Settings;
using Settings.Memento;
using Settings.Stores;

namespace TestSettings.Stores
{
    [TestFixture]
    public class TestSettingsStore
    {
        [Test]
        public void SettingStore_ctor_IsOk()
        {
            var userS = MockRepository.GenerateMock<ISettingsMemento>();
            var defaultS = MockRepository.GenerateMock<ISettingsMemento>();

            var serializer = MockRepository.GenerateMock<ISettingFileSerializer>();
            serializer.Stub(a => a.GetDefaults<ISettingsMemento>()).Return(new List<ISettingsMemento>() {userS});
            serializer.Stub(a => a.GetUsers<ISettingsMemento>()).Return(new List<ISettingsMemento>() {defaultS});

            var store = new SettingsStore<ISettingsMemento>(serializer);
            Assert.AreEqual(1, store.Defaults.Count);
            Assert.AreEqual(1, store.Users.Count);
        }

        [Test]
        public void SettingsStore_AddToUsers_AllCases()
        {
            var memento = MockRepository.GenerateStub<ISettingsMemento>();
            memento.Name = "11";

            var fileSerializer = MockRepository.GenerateMock<ISettingFileSerializer>();
            fileSerializer.Stub(a => a.FileExists(Arg<string>.Is.Equal("11"))).Return(true);

            var store = new SettingsStore<ISettingsMemento>(fileSerializer);

            Assert.AreEqual(0, store.Users.Count);

            store.Add(memento);
            Assert.AreEqual(1, store.Users.Count);

            var newMemento = MockRepository.GenerateStub<ISettingsMemento>();
            newMemento.Name = "11(1)";
            store.Add(memento);
            Assert.AreEqual(2, store.Users.Count);
            fileSerializer.AssertWasCalled(
                a => a.SaveMemento(
                    Arg<ISettingsMemento>.Is.Anything));
            Assert.AreEqual("11(1)", newMemento.Name);
        }

        [Test]
        public void SettingsStore_ExportAllPToFile_GetAll()
        {
            var memento1 = MockRepository.GenerateStub<ISettingsMemento>();
            memento1.Name = "11";

            var memento2 = MockRepository.GenerateStub<ISettingsMemento>();
            memento2.Name = "11";

            var fileSerializer = MockRepository.GenerateMock<ISettingFileSerializer>();

            var materialStore = new SettingsStore<ISettingsMemento>(fileSerializer);
            materialStore.Add(memento1);
            materialStore.Add(memento2);
            materialStore.ExportAll("path");


            fileSerializer.AssertWasCalled(
                a => a.ExportMementos(
                    Arg<IEnumerable<ISettingsMemento>>.Is.Anything,
                    Arg<string>.Is.Equal("path")));
        }

        [Test]
        public void SettingsStore_Export_IsOk()
        {
            var memento1 = MockRepository.GenerateStub<ISettingsMemento>();
            memento1.Name = "11";

            var memento2 = MockRepository.GenerateStub<ISettingsMemento>();
            memento2.Name = "11";

            var fileSerializer = MockRepository.GenerateMock<ISettingFileSerializer>();

            var materialStore = new SettingsStore<ISettingsMemento>(fileSerializer);
            materialStore.Export("path", new List<ISettingsMemento>() {memento1, memento2});

            fileSerializer.AssertWasCalled(
                a => a.ExportMementos(Arg<List<ISettingsMemento>>.Is.Anything, Arg<string>.Is.Equal("path")));
        }

        [Test]
        public void SettingsStore_ImportFromFile_AllNew()
        {
            var memento1 = MockRepository.GenerateStub<ISettingsMemento>();
            memento1.Name = "11";

            var memento2 = MockRepository.GenerateStub<ISettingsMemento>();
            memento2.IsAnisoprintApproved = true;
            memento2.Name = "11";

            var fileSerializer = MockRepository.GenerateMock<ISettingFileSerializer>();
            fileSerializer.Stub(a => a.Import<ISettingsMemento>(Arg<string>.Is.Anything))
                .Return(new List<ISettingsMemento> {memento1, memento2});

            var materialStore = new SettingsStore<ISettingsMemento>(fileSerializer);
            var materials = materialStore.GetMementosFromFile("path");

            Assert.AreEqual(2, materials.Count);
        }

        [Test]
        public void SettingsStore_ImportFromFile_Unsuccess()
        {
            var memento1 = MockRepository.GenerateStub<ISettingsMemento>();
            memento1.Name = "11";

            var memento2 = MockRepository.GenerateStub<ISettingsMemento>();
            memento2.IsAnisoprintApproved = true;
            memento2.Name = "11";

            var fileSerializer = MockRepository.GenerateMock<ISettingFileSerializer>();
            fileSerializer.Stub(a => a.Import<ISettingsMemento>(Arg<string>.Is.Anything))
                .Throw(new JsonSerializationException());

            var materialStore = new SettingsStore<ISettingsMemento>(fileSerializer);

            Assert.Throws<JsonSerializationException>(() => materialStore.GetMementosFromFile("path"));
        }

        [Test]
        public void SettingsStore_ImportFromFile_HasDuplicate()
        {
            var memento1 = MockRepository.GenerateStub<ISettingsMemento>();
            memento1.Name = "11";

            var memento2 = MockRepository.GenerateStub<ISettingsMemento>();
            memento2.IsAnisoprintApproved = true;
            memento2.Name = "11";

            var fileSerializer = MockRepository.GenerateMock<ISettingFileSerializer>();
            fileSerializer.Stub(a => a.Import<ISettingsMemento>(Arg<string>.Is.Anything))
                .Return(new List<ISettingsMemento> {memento1, memento2});

            var materialStore = new SettingsStore<ISettingsMemento>(fileSerializer);
            materialStore.Add(memento1);
            var materials = materialStore.GetMementosFromFile("path");

            Assert.AreEqual(2, materials.Count);
        }

        [Test]
        public void SettingsStore_Remove_InDefault()
        {
            var memento1 = MockRepository.GenerateStub<ISettingsMemento>();
            memento1.Name = "11";

            var fileSerializer = MockRepository.GenerateMock<ISettingFileSerializer>();
            var materialStore = new SettingsStore<ISettingsMemento>(fileSerializer);

            materialStore.Defaults.Add(memento1);
            Assert.AreEqual(1, materialStore.Defaults.Count);

            materialStore.Remove(memento1);
            Assert.AreEqual(0, materialStore.Defaults.Count);
            fileSerializer.AssertWasCalled(a => a.RemoveMemento(Arg<ISettingsMemento>.Is.Equal(memento1)));
        }

        [Test]
        public void SettingsStore_Remove_InUsers()
        {
            var memento1 = MockRepository.GenerateStub<ISettingsMemento>();
            memento1.Name = "11";

            var fileSerializer = MockRepository.GenerateMock<ISettingFileSerializer>();
            var materialStore = new SettingsStore<ISettingsMemento>(fileSerializer);

            materialStore.Add(memento1);
            Assert.AreEqual(1, materialStore.Users.Count);

            materialStore.Remove(memento1);
            Assert.AreEqual(0, materialStore.Users.Count);
            fileSerializer.AssertWasCalled(a => a.RemoveMemento(Arg<ISettingsMemento>.Is.Equal(memento1)));
        }

        [Test]
        public void SettingsStore_GetByTemplate_InDefaults()
        {
            var pd = MockRepository.GenerateStub<ISettingsMemento>();
            pd.Name = "Default";
            var pu = MockRepository.GenerateStub<ISettingsMemento>();
            pu.Name = "User";

            var fileSerializer = MockRepository.GenerateMock<ISettingFileSerializer>();
            fileSerializer.Stub(a => a.GetDefaults<ISettingsMemento>()).Return(new List<ISettingsMemento>{ pd });
            fileSerializer.Stub(a => a.GetUsers<ISettingsMemento>()).Return(new List<ISettingsMemento>{ pu });
            var materialStore = new SettingsStore<ISettingsMemento>(fileSerializer);

            var found = materialStore.GetByTemplate(pd);

            Assert.IsNotNull(found);
            Assert.AreEqual("Default", found.Name);
        }

        [Test]
        public void SettingsStore_GetByTemplate_InUsers()
        {
            var pd = MockRepository.GenerateStub<ISettingsMemento>();
            pd.Name = "Default";
            var pu = MockRepository.GenerateStub<ISettingsMemento>();
            pu.Name = "User";

            var fileSerializer = MockRepository.GenerateMock<ISettingFileSerializer>();
            var materialStore = new SettingsStore<ISettingsMemento>(fileSerializer);
            materialStore.Add(pd);
            materialStore.Add(pu);

            var found = materialStore.GetByTemplate(pu);

            Assert.IsNotNull(found);
            Assert.AreEqual("User", found.Name);
        }


        [Test]
        public void SettingsStore_GetByTemplate_NotFound()
        {
            var pd = MockRepository.GenerateStub<ISettingsMemento>();
            pd.Name = "Default";
            var pu = MockRepository.GenerateStub<ISettingsMemento>();
            pu.Name = "User";

            var fileSerializer = MockRepository.GenerateMock<ISettingFileSerializer>();
            var materialStore = new SettingsStore<ISettingsMemento>(fileSerializer);
            materialStore.Add(pd);
            materialStore.Add(pu);

            var pt = MockRepository.GenerateStub<ISettingsMemento>();

            var found = materialStore.GetByTemplate(pt);

            Assert.IsNull(found);
        }

        [Test]
        public void SettingsStore_GetByTemplateAnotherName_InDefaults()
        {
            var pd = MockRepository.GenerateStub<ISettingsMemento>();
            pd.Name = "Default";
            var pu = MockRepository.GenerateStub<ISettingsMemento>();
            pu.Name = "User";

            var pdForCheck = MockRepository.GenerateStub<ISettingsMemento>();
            pdForCheck.Name = "Default1";

            pd.Stub(a => a.EqualsWithoutNameVersionApproved(Arg<ISettingsMemento>.Is.Equal(pdForCheck))).Return(true);

            var fileSerializer = MockRepository.GenerateMock<ISettingFileSerializer>();
            fileSerializer.Stub(a => a.GetDefaults<ISettingsMemento>()).Return(new List<ISettingsMemento> { pd });
            fileSerializer.Stub(a => a.GetUsers<ISettingsMemento>()).Return(new List<ISettingsMemento> { pu });
            var materialStore = new SettingsStore<ISettingsMemento>(fileSerializer);

            var found = materialStore.GetByTemplateAnotherName(pdForCheck);

            Assert.IsNotNull(found);
            Assert.AreEqual("Default", found.Name);
        }

        [Test]
        public void SettingsStore_GetByTemplateAnotherName_InUsers()
        {
            var pd = MockRepository.GenerateStub<ISettingsMemento>();
            pd.Name = "Default";
            var pu = MockRepository.GenerateStub<ISettingsMemento>();
            pu.Name = "User";

            var puForCheck = MockRepository.GenerateStub<ISettingsMemento>();
            puForCheck.Name = "User1";

            pu.Stub(a => a.EqualsWithoutNameVersionApproved(Arg<ISettingsMemento>.Is.Equal(puForCheck))).Return(true);

            var fileSerializer = MockRepository.GenerateMock<ISettingFileSerializer>();
            fileSerializer.Stub(a => a.GetDefaults<ISettingsMemento>()).Return(new List<ISettingsMemento> { pd });
            fileSerializer.Stub(a => a.GetUsers<ISettingsMemento>()).Return(new List<ISettingsMemento> { pu });
            var materialStore = new SettingsStore<ISettingsMemento>(fileSerializer);

            var found = materialStore.GetByTemplateAnotherName(puForCheck);

            Assert.IsNotNull(found);
            Assert.AreEqual("User", found.Name);
        }


        [Test]
        public void SettingsStore_GetByTemplateAnotherName_NotFound()
        {
            var pd = MockRepository.GenerateStub<ISettingsMemento>();
            pd.Name = "Default";
            var pu = MockRepository.GenerateStub<ISettingsMemento>();
            pu.Name = "User";

            var pForCheck = MockRepository.GenerateStub<ISettingsMemento>();
            pForCheck.Name = "User1";

            pd.Stub(a => a.EqualsWithoutNameVersionApproved(Arg<ISettingsMemento>.Is.Equal(pForCheck))).Return(false);
            pu.Stub(a => a.EqualsWithoutNameVersionApproved(Arg<ISettingsMemento>.Is.Equal(pForCheck))).Return(false);

            var fileSerializer = MockRepository.GenerateMock<ISettingFileSerializer>();
            fileSerializer.Stub(a => a.GetDefaults<ISettingsMemento>()).Return(new List<ISettingsMemento> { pd });
            fileSerializer.Stub(a => a.GetUsers<ISettingsMemento>()).Return(new List<ISettingsMemento> { pu });
            var materialStore = new SettingsStore<ISettingsMemento>(fileSerializer);

            var pt = MockRepository.GenerateStub<ISettingsMemento>();

            var found = materialStore.GetByTemplateAnotherName(pt);

            Assert.IsNull(found);
        }

        [Test]
        public void SettingsStore_CheckForConflicts_HasSameNoConflict()
        {
            var user = MockRepository.GenerateStub<ISettingsMemento>();
            user.Name = "User";
            user.IsAnisoprintApproved = false;

            var fileSerializer = MockRepository.GenerateMock<ISettingFileSerializer>();
            var materialStore = new SettingsStore<ISettingsMemento>(fileSerializer);

            materialStore.Add(user);

            var (conflicts, clears) = materialStore.CheckForConflicts(new List<ISettingsMemento> { user });
            Assert.AreEqual(0, conflicts.Count);
            Assert.AreEqual(0, clears.Count);
        }

        [Test]
        public void SettingsStore_CheckForConflicts_HasSameHasConflict()
        {
            var user = new MaterialPMemento
            {
                Name = "User",
                IsAnisoprintApproved = false,
                BedHeatupTemperature = 20
            };

            var user1 = new MaterialPMemento
            {
                Name = "User",
                IsAnisoprintApproved = false,
                BedHeatupTemperature = 25
            };

            var fileSerializer = MockRepository.GenerateMock<ISettingFileSerializer>();
            var materialStore = new SettingsStore<ISettingsMemento>(fileSerializer);

            materialStore.Add(user);

            var (conflicts, clears) = materialStore.CheckForConflicts(new List<ISettingsMemento> { user1 });
            Assert.AreEqual(1, conflicts.Count);
            Assert.AreEqual(0, clears.Count);
        }

        [Test]
        public void SettingsStore_CheckForConflicts_NoConflictHasClear()
        {
            var user = new MaterialPMemento
            {
                Name = "User",
                IsAnisoprintApproved = false,
                BedHeatupTemperature = 20
            };

            var user1 = new MaterialPMemento
            {
                Name = "User1",
                IsAnisoprintApproved = false,
                BedHeatupTemperature = 25
            };

            var fileSerializer = MockRepository.GenerateMock<ISettingFileSerializer>();
            var materialStore = new SettingsStore<ISettingsMemento>(fileSerializer);

            materialStore.Add(user);

            var (conflicts, clears) = materialStore.CheckForConflicts(new List<ISettingsMemento> { user1 });
            Assert.AreEqual(0, conflicts.Count);
            Assert.AreEqual(1, clears.Count);
        }

        [Test]
        public void SettingsStore_CheckConflicts_NoConflictsForDefaults()
        {
            var user = MockRepository.GenerateStub<ISettingsMemento>();
            user.Name = "User";
            user.IsAnisoprintApproved = false;

            var fileSerializer = MockRepository.GenerateMock<ISettingFileSerializer>();
            var materialStore = new SettingsStore<ISettingsMemento>(fileSerializer);

            materialStore.Add(user);

            var forCheck = MockRepository.GenerateStub<ISettingsMemento>();
            forCheck.Name = "Default";
            forCheck.IsAnisoprintApproved = true;
            forCheck.Stub(a => a.TrueIsAnisoprintApproved).Return(true);

            var (conflicts, clears) = materialStore.CheckForConflicts(new List<ISettingsMemento>() { forCheck });
            Assert.AreEqual(0, conflicts.Count);
            Assert.AreEqual(0, clears.Count);
        }

        [Test]
        public void SettingsStore_ProcessConflictsAndClears_HasConflicts_FillFromOldToNew()
        {
            var userItem = MockRepository.GenerateMock<ISettingsMemento>();
            userItem.Stub(a=>a.Name).Return("User");
            userItem.Stub(a=>a.IsAnisoprintApproved).Return(false);

            var fileSerializer = MockRepository.GenerateMock<ISettingFileSerializer>();
            var settingsStore = new SettingsStore<ISettingsMemento>(fileSerializer);

            settingsStore.Add(userItem);

            var conflict = MockRepository.GenerateStub<ISettingsMemento>();
            conflict.Name = "User";
            conflict.IsAnisoprintApproved = false;

            var conflicts = new Dictionary<ISettingsMemento, ConflictMode> { { conflict, ConflictMode.FILL_OLD_FROM_NEW } };
            settingsStore.ProcessConflicts(conflicts);

            userItem.AssertWasCalled(a=>a.FillFromAnother(Arg<ISettingsMemento>.Is.Equal(conflict)));
        }

        [Test]
        public void SettingsStore_ProcessConflictsAndClears_HasConflicts_AddToNew()
        {
            var userItem = MockRepository.GenerateMock<ISettingsMemento>();
            userItem.Stub(a => a.Name).Return("User");
            userItem.Stub(a => a.IsAnisoprintApproved).Return(false);

            var fileSerializer = MockRepository.GenerateMock<ISettingFileSerializer>();
            var settingsStore = new SettingsStore<ISettingsMemento>(fileSerializer);

            settingsStore.Add(userItem);

            var conflict = MockRepository.GenerateStub<ISettingsMemento>();
            conflict.Name = "User";
            conflict.IsAnisoprintApproved = false;

            var conflicts = new Dictionary<ISettingsMemento, ConflictMode> { { conflict, ConflictMode.SAVE_NEW } };
            settingsStore.ProcessConflicts(conflicts);

            Assert.AreEqual(2, settingsStore.Users.Count);
        }

        [Test]
        public void SettingsStore_ProcessConflictsAndClears_HasClears()
        {
            var userItem = MockRepository.GenerateMock<ISettingsMemento>();
            userItem.Stub(a => a.Name).Return("User");
            userItem.Stub(a => a.IsAnisoprintApproved).Return(false);

            var fileSerializer = MockRepository.GenerateMock<ISettingFileSerializer>();
            var materialStore = new SettingsStore<ISettingsMemento>(fileSerializer);

            materialStore.Add(userItem);

            var clear = MockRepository.GenerateStub<ISettingsMemento>();
            clear.Name = "User";
            clear.IsAnisoprintApproved = false;

            var conflicts = new Dictionary<ISettingsMemento, ConflictMode> { };
            materialStore.ProcessClears(new List<ISettingsMemento> { clear });

            Assert.AreEqual(2, materialStore.Users.Count);
        }
    }
}