using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Aura.Managers;
using Aura.Themes.Localization;
using Aura.Views;
using Microsoft.Win32;
using Newtonsoft.Json;
using Prism.Commands;
using Settings;
using Settings.Memento;
using Settings.Stores;

namespace Aura.ViewModels
{
    public interface ISettingsCollectionVM : INotifyPropertyChanged
    {
        IList Items { get; }
        object SelectedItem { get; set; }

        string AddItemText { get; }
        string ImportTip { get; }
        string ExportTip { get; }

        DelegateCommand AddItemCommand { get; }
        DelegateCommand ExportCommand { get; }
        DelegateCommand ImportCommand { get; }
    }

    public abstract class SettingsCollectionVM<TModel> : ISettingsCollectionVM
        where TModel : class, ISettingsMemento
        
    {
        private ISettingsStore<TModel> _settingsStore;
        private ISessionStore _sessionStore;

        private ISettingsFactory _settingsFactory;
        private ISettingsManager _settingsManager;
        public DelegateCommand AddItemCommand { get; private set; }
        public DelegateCommand ExportCommand { get; private set; }
        public DelegateCommand ImportCommand { get; private set; }

        public abstract string AddItemText { get; }
        public abstract string ImportTip { get; }
        public abstract string ExportTip { get; }

        public IList Items { get; }

        #region REAL ITEMS
        public ObservableCollection<TModel> RealItems { get; private set; }

        #endregion

        private ObservableCollection<SettingsItemVM<TModel>> _myList => (ObservableCollection<SettingsItemVM<TModel>>) Items;

        #region SELECTED ITEM

        private object _selectedItem;
        public object SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
            }
        }

        public void Select(TModel model)
        {
            foreach (var item1 in Items)
            {
                var vmItem = item1 as SettingsItemVM<TModel>;
                if (vmItem.Model.Equals(model))
                {
                    SelectedItem = vmItem;
                    return;
                }
            }
        }

        #endregion

        public SettingsCollectionVM()
        {
            AddItemCommand = new DelegateCommand(AddItemExecuteMethod, () => true);
            ExportCommand = new DelegateCommand(ExportExecuteMethod, () => true);
            ImportCommand = new DelegateCommand(ImportExecuteMethod, () => true);
            Items = new ObservableCollection<SettingsItemVM<TModel>>();
            RealItems = new ObservableCollection<TModel>();
            SelectedItem = null;
        }

        #region EXPORT

        private void ExportExecuteMethod()
        {
            if (Items == null || Items.Count == 0) return;
            //по клику кнопки - открываем диалог сохранения файла
            var saveFileDialog = new SaveFileDialog
            {
                Filter = _settingsManager.GetFilter<TModel>(),
                InitialDirectory = _settingsManager.ExportDirectory,
                OverwritePrompt = true,
                CreatePrompt = false,
                AddExtension = true
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                _settingsManager.ExportDirectory = Path.GetDirectoryName(saveFileDialog.FileName);
                try
                {
                    //экспортируем материалы полностью
                    _settingsStore.Export(saveFileDialog.FileName, _myList.Select(a=>a.Model).ToList());
                }
                catch (Exception)
                {
                    MessageBox.Show(Common_en_EN.FileWritingExceptionText, Common_en_EN.FileWritingExceptionCaption, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        #endregion

        #region IMPORT

        private void ImportExecuteMethod()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = _settingsManager.GetFilter<TModel>(),
                InitialDirectory = _settingsManager.ImportDirectory
            };
            if (openFileDialog.ShowDialog() == true)
            {
                _settingsManager.ImportDirectory = Path.GetDirectoryName(openFileDialog.FileName);
                //импортируем материалы
                try
                {
                    var items = _settingsStore.GetMementosFromFile(openFileDialog.FileName);
                    if (items != null && items.Any())
                    {
                        var (conflicts, clears) = _settingsStore.CheckForConflicts(items);
                        var conflictDictionary = new Dictionary<TModel, ConflictMode>();
                        if (conflicts.Any())
                        {
                            var cdVM = new ConflictCollectionVM();
                            cdVM.Initialize(conflicts);
                            var conflictDialog = new ConflictDialogNew { DataContext = cdVM };
                            if (conflictDialog.ShowDialog() == true)
                            {
                                foreach (var essenseConflict in cdVM.GetConflicts<TModel>())
                                {
                                    conflictDictionary.Add(essenseConflict.Key, essenseConflict.Value);
                                }
                            }
                        }
                        if (conflictDictionary.Any()) _settingsStore.ProcessConflicts(conflictDictionary);
                        if (clears != null && clears.Any()) _settingsStore.ProcessClears(clears);
                    }
                }
                catch (JsonSerializationException)
                {
                    MessageBox.Show(Errors_en_EN.FileReading);
                }
            }
        }

        #endregion

        #region ADD

        private void AddItemExecuteMethod()
        {
            var newItem = _settingsFactory.CreateItem<TModel>();
            _settingsStore.Add(newItem);
        }

        #endregion

        #region INITIALIZE

        public void Initialize(ISettingsStore<TModel> settingsStore,
            ISessionStore sessionStore,
            ISettingsFactory settingsFactory,
            ISettingsManager settingsManager)
        {
            _settingsStore = settingsStore;
            _sessionStore = sessionStore;
            _settingsFactory = settingsFactory;
            _settingsManager = settingsManager;

            Items.Clear();
            RealItems.Clear();
            
            var models = GetModels();
            if (models != null)
            {
                foreach (var model in models)
                {
                    AddVM(model);
                    RealItems.Add(model);
                }
            }

            _settingsStore.Added += (sender, model) =>
            {
                var vm = AddVM(model);
                SelectedItem = vm;
                RealItems.Add(model);
            };
            _settingsStore.Removed += (sender, model) =>
            {
                RemoveVM(model);
                RealItems.Remove(model);
            };
        }

        private List<TModel> GetModels()
        {
            var defaults = _settingsStore.Defaults.OrderBy(a => a.Name);
            var users = _settingsStore.Users.OrderBy(a => a.Name);
            return new List<TModel>(defaults.Union(users));
        }

        protected abstract SettingsItemVM<TModel> CreateVM();

        private SettingsItemVM<TModel> AddVM(TModel model)
        {
            var vm = CreateVM();
            vm.Initialize(_settingsStore, _sessionStore, _settingsFactory, _settingsManager, model);
            Items.Add(vm);
            return vm;
        }

        private void RemoveVM(TModel model)
        {
            var item = Items.Cast<SettingsItemVM<TModel>>().Single(a => a.Model.Equals(model));
            if (SelectedItem != null && SelectedItem.Equals(item)) SelectedItem = null;
            Items.Remove(item);
        }

        #endregion

        #region ON PROPERTY CHANGED

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string caller = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(caller));
        }

        #endregion
    }

    public class SettingsCollectionPlasticVM : SettingsCollectionVM<IMaterialPMemento>
    {
        public override string AddItemText => Plastic_en_EN.AddNewMaterial;
        public override string ImportTip => Plastic_en_EN.TipImportMaterials;
        public override string ExportTip => Plastic_en_EN.TipExportMaterials;

        protected override SettingsItemVM<IMaterialPMemento> CreateVM()
        {
            return new SettingsItemVMPlastic();
        }
    }

    public class SettingsCollectionFiberVM : SettingsCollectionVM<IMaterialFMemento>
    {
        public override string AddItemText => Fiber_en_EN.AddNewFiber;
        public override string ImportTip => Plastic_en_EN.TipImportMaterials;
        public override string ExportTip => Plastic_en_EN.TipExportMaterials;

        protected override SettingsItemVM<IMaterialFMemento> CreateVM()
        {
            return new SettingsItemVMFiber();
        }
    }

    public class SettingsCollectionPrinterVM : SettingsCollectionVM<IPrinterMemento>
    {
        public override string AddItemText => Printer_en_EN.AddNewPrinter;
        public override string ImportTip => Printer_en_EN.TipImportPrinters;
        public override string ExportTip => Printer_en_EN.TipExportPrinters;

        protected override SettingsItemVM<IPrinterMemento> CreateVM()
        {
            return new SettingsItemVMPrinter();
        }
    }

    public class SettingsCollectionProfileVM : SettingsCollectionVM<IProfileMemento>
    {
        public override string AddItemText => Profile_en_EN.AddNewProfile;
        public override string ImportTip => Profile_en_EN.TipImportProfiles;
        public override string ExportTip => Profile_en_EN.TipExportProfiles;

        protected override SettingsItemVM<IProfileMemento> CreateVM()
        {
            return new SettingsItemVMProfile();
        }
    }
}