using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using Aura.Controls;
using Aura.Managers;
using Aura.Themes.Localization;
using Microsoft.Win32;
using Prism.Commands;
using Settings;
using Settings.Entities.Infill;
using Settings.Memento;
using Settings.Stores;
using Util.GeometryBasics;

namespace Aura.ViewModels
{
    public abstract class SettingsItemVM<T> : INotifyPropertyChanged
        where T : class, ISettingsMemento
    {
        protected ISettingsStore<T> _settingsStore;
        protected ISessionStore _sessionStore;
        private ISettingsFactory _settingsFactory;
        private ISettingsManager _settingsManager;

        #region MODEL

        private T _model;

        public T Model
        {
            get => _model;
            private set
            {
                _model = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public DelegateCommand RemoveItemCommand { get; private set; }
        public DelegateCommand DuplicateItemCommand { get; private set; }
        public DelegateCommand ExportItemCommand { get; private set; }

        public SettingsItemVM()
        {
            RemoveItemCommand = new DelegateCommand(RemoveItemExecuteMethod, () => ModelCanBeRemoved(_sessionStore));
            DuplicateItemCommand = new DelegateCommand(DuplicateItemExecuteMethod, () => true);
            ExportItemCommand = new DelegateCommand(ExportItemExecuteMethod, () => true);
        }

        protected abstract string GetItemInSessionText();
        protected abstract string GetItemInSessionCaption();

        private void RemoveItemExecuteMethod()
        {
            var inSession = _sessionStore.CheckItem(Model);
            if (inSession) MessageBox.Show(GetItemInSessionText(), GetItemInSessionCaption(), MessageBoxButton.OK, MessageBoxImage.Stop);
            else _settingsStore.Remove(Model);
        }

        private bool ModelCanBeRemoved(ISessionStore store)
        {
            if (store == null || Model == null || Model.TrueIsAnisoprintApproved) return false;
            return !store.CheckItem(Model);
        }

        private void DuplicateItemExecuteMethod()
        {
            var duplicate = _settingsFactory.CreateItem(Model);
            duplicate.Name += "(Copy)";
            _settingsStore.Add(duplicate);
        }

        private void ExportItemExecuteMethod()
        {
            //по клику кнопки - открываем диалог сохранения файла
            var saveFileDialog = new SaveFileDialog
            {
                Filter = _settingsManager.GetFilter<T>(),
                InitialDirectory = _settingsManager.ExportDirectory,
                OverwritePrompt = true,
                CreatePrompt = false,
                AddExtension = true,
                FileName = Model.Name
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                _settingsManager.ExportDirectory = Path.GetDirectoryName(saveFileDialog.FileName);
                try
                {
                    //экспортируем материалы полностью
                    _settingsStore.Export(saveFileDialog.FileName, new List<T> { Model });
                }
                catch (Exception)
                {
                    MessageBox.Show(Common_en_EN.FileWritingExceptionText, Common_en_EN.FileWritingExceptionCaption, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public virtual void Initialize(ISettingsStore<T> settingsStore, 
            ISessionStore sessionStore, 
            ISettingsFactory settingsFactory, 
            ISettingsManager settingsManager, 
            T item)
        {
            _settingsStore = settingsStore;
            _sessionStore = sessionStore;
            _settingsStore.PropertyChanged += (sender, args) => RemoveItemCommand?.RaiseCanExecuteChanged();
            _settingsFactory = settingsFactory;
            _settingsManager = settingsManager;
            Model = item;
        }

        #region ON PROPERTY CHANGED

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string caller = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(caller));
        }

        #endregion
    }

    public class SettingsItemVMPlastic : SettingsItemVM<IMaterialPMemento>
    {
        protected override string GetItemInSessionText() => Plastic_en_EN.MaterialInSessionText;
        protected override string GetItemInSessionCaption() => Plastic_en_EN.MaterialInSessionCaption;
    }

    public class SettingsItemVMFiber : SettingsItemVM<IMaterialFMemento>
    {
        protected override string GetItemInSessionText() => Plastic_en_EN.MaterialInSessionText;
        protected override string GetItemInSessionCaption() => Plastic_en_EN.MaterialInSessionCaption;
    }

    public class SettingsItemVMPrinter : SettingsItemVM<IPrinterMemento>
    {
        public ExtrudersVM ExtrudersVM { get; private set; }
        protected override string GetItemInSessionText() => Printer_en_EN.PrinterInSessionText;
        protected override string GetItemInSessionCaption() => Printer_en_EN.PrinterInSessionCaption;

        public override void Initialize(ISettingsStore<IPrinterMemento> settingsStore, ISessionStore sessionStore, ISettingsFactory settingsFactory,
            ISettingsManager settingsManager, IPrinterMemento item)
        {
            base.Initialize(settingsStore, sessionStore, settingsFactory, settingsManager, item);
            ExtrudersVM = new ExtrudersVM();
            ExtrudersVM.Initialize(_sessionStore, Model);
        }
    }

    public class SettingsItemVMProfile : SettingsItemVM<IProfileMemento>
    {
        public ObservableCollection<InfillTypeData> InfillTypesCollection { get; private set; }
        public ObservableCollection<FiberInfillTypeData> FiberInfillTypeCollection { get; private set; }
        public ObservableCollection<AnisogridRibPlacementData> AnisogridRibPlacementCollection { get; private set; }

        protected override string GetItemInSessionText() => Profile_en_EN.ProfileInSessionText;
        protected override string GetItemInSessionCaption() => Profile_en_EN.ProfileInSessionCaption;

        public SettingsItemVMProfile()
        {
            #region INFILL TYPES

            InfillTypesCollection = new ObservableCollection<InfillTypeData>();
            var itLines = new InfillTypeData
            {
                InfillType = INFILL_TYPE.LINE,
                InfillTypeCaption = InfillPlasticCellular_en_EN.TypeLine,
            };
            InfillTypesCollection.Add(itLines);
            var itGrid = new InfillTypeData
            {
                InfillType = INFILL_TYPE.GRID,
                InfillTypeCaption = InfillPlasticCellular_en_EN.TypeGrid
            };
            InfillTypesCollection.Add(itGrid);
            var itTriangles = new InfillTypeData
            {
                InfillType = INFILL_TYPE.TRIANGLES,
                InfillTypeCaption = InfillPlasticCellular_en_EN.TypeTriangles
            };
            InfillTypesCollection.Add(itTriangles);

            #endregion

            #region FIBER INFILL TYPE

            FiberInfillTypeCollection = new ObservableCollection<FiberInfillTypeData>();
            var fiSolid = new FiberInfillTypeData
            {
                InfillType = FiberInfillType.SOLID,
                InfillTypeCaption = InfillFiber_en_EN.TypeSolid
            };
            FiberInfillTypeCollection.Add(fiSolid);
            var fiRombic = new FiberInfillTypeData
            {
                InfillType = FiberInfillType.CELLULAR_RHOMBIC,
                InfillTypeCaption = InfillFiber_en_EN.TypeRombicGrid
            };
            FiberInfillTypeCollection.Add(fiRombic);
            var fiIsogrid = new FiberInfillTypeData
            {
                InfillType = FiberInfillType.CELLULAR_ISOGRID,
                InfillTypeCaption = InfillFiber_en_EN.TypeIsogrid
            };
            FiberInfillTypeCollection.Add(fiIsogrid);
            var fiAnisogrid = new FiberInfillTypeData
            {
                InfillType = FiberInfillType.CELLULAR_ANISOGRID,
                InfillTypeCaption = InfillFiber_en_EN.TypeAnisogrid
            };
            FiberInfillTypeCollection.Add(fiAnisogrid);

            #endregion

            #region TRANSVERSE RIB PLACEMENT

            AnisogridRibPlacementCollection = new ObservableCollection<AnisogridRibPlacementData>();
            var pNearApex = new AnisogridRibPlacementData
            {
                AnisogridRibPlacement = AnisogridRibPlacement.NEAR_APEX,
                AnisogridRibPlacementCaption = InfillFiber_en_EN.RibPlacementTypeNearApex
            };
            AnisogridRibPlacementCollection.Add(pNearApex);
            var pMiddle = new AnisogridRibPlacementData
            {
                AnisogridRibPlacement = AnisogridRibPlacement.MIDDLE,
                AnisogridRibPlacementCaption = InfillFiber_en_EN.RibPlacementTypeMiddle
            };
            AnisogridRibPlacementCollection.Add(pMiddle);

            #endregion
        }
    }
}