using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Aura.Controls.Primitive;
using Aura.Managers;
using Aura.Themes;
using Aura.Themes.Localization;
using Microsoft.Win32;
using Newtonsoft.Json;
using Settings;
using Settings.Memento;
using Settings.Stores;
using Unity;
using Util;

namespace Aura.Obsolete
{
    public class MaterialsPPanel : UserControl
    {
        private static ObservableCollection<IMaterialPMemento> _materials;

        private static readonly ISettingsStore<IMaterialPMemento> _materialStore;
        private static readonly ISessionStore _sessionStore;
        private static readonly ISettingsFactory _settingsFactory;
        private static readonly ISettingsManager _settingsManager;

        #region CTORS

        static MaterialsPPanel()
        {
            _settingsFactory = UnityCore.Container.Resolve<ISettingsFactory>();
            _materialStore = UnityCore.Container.Resolve<ISettingsStore<IMaterialPMemento>>();
            _settingsManager = UnityCore.Container.Resolve<ISettingsManager>();
            _sessionStore = UnityCore.Container.Resolve<ISessionStore>();
        }

        public MaterialsPPanel()
        {
            _materials = GetPlastics();
            _materialStore.PropertyChanged += (sender, args) =>
            {
                UpdateListBoxItems();
                if (_listBox != null)
                {
                    var selectedItem = _listBox.SelectedItem as IMaterialPMemento;
                    SetSelectedItem(selectedItem);
                }
            };
        }

        #endregion

        private static ListBox _listBox;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _listBox = GetTemplateChild("PART_ListBoxMainContainer") as ListView;
            if (_listBox != null)
            {
                _listBox.Visibility = Visibility.Visible;
                _listBox.ItemsSource = _materials;
                _listBox.SelectionMode = SelectionMode.Single;

                #region EXPANDER HANDLERS

                void ExpanderRemoved(object sender, RoutedEventArgs e)
                {
                    var memento = (e.OriginalSource as FrameworkElement)?.DataContext as IMaterialPMemento;
                    if (memento == null) return;

                    var isInSession = _sessionStore.CheckMaterial(memento);
                    if (isInSession) MessageBox.Show(Plastic_en_EN.MaterialInSessionText, Plastic_en_EN.MaterialInSessionCaption, MessageBoxButton.OK, MessageBoxImage.Stop);
                    else
                    {
                        _materialStore.Remove(memento);
                        UpdateListBoxItems();
                    }
                }

                void ExpanderDuplicate(object sender, RoutedEventArgs e)
                {
                    var memento = (e.OriginalSource as FrameworkElement)?.DataContext as IMaterialPMemento;
                    if (memento == null) return;
                    var newMaterial = _settingsFactory.CreateNewMaterialPMemento(memento);
                    newMaterial.Name += "(Copy)";
                    _materialStore.Add(newMaterial);
                    UpdateListBoxItems();
                    SetSelectedItem(newMaterial);
                }

                void ExpanderExport(object sender, RoutedEventArgs e)
                {
                    var memento = (e.OriginalSource as FrameworkElement)?.DataContext as IMaterialPMemento;
                    if (memento == null) return;
                    //по клику кнопки - открываем диалог сохранения файла
                    var saveFileDialog = new SaveFileDialog
                    {
                        Filter = Plastic_en_EN.FileFilter,
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
                            _materialStore.Export(saveFileDialog.FileName, new List<IMaterialPMemento> { memento });
                        }
                        catch (Exception )
                        {
                            MessageBox.Show(Common_en_EN.FileWritingExceptionText, Common_en_EN.FileWritingExceptionCaption, MessageBoxButton.OK, MessageBoxImage.Error);
                        }

                    }
                }

                _listBox.AddHandler(AuraExpanderExtended.RemoveEvent, new RoutedEventHandler(ExpanderRemoved));
                _listBox.AddHandler(AuraExpanderExtended.DuplicateEvent, new RoutedEventHandler(ExpanderDuplicate));
                _listBox.AddHandler(AuraExpanderExtended.ExportEvent, new RoutedEventHandler(ExpanderExport));


                #endregion

                #region BTN ADD

                //для кнопки добавления нового материала - добавляем поведение
                var btnAdd = GetTemplateChild("PART_ButtonAddNewMaterial") as ButtonBase;
                if (btnAdd != null)
                {
                    btnAdd.Click += (sender, args) =>
                    {
                        var newMaterial = _settingsFactory.CreateNewMaterialPMemento();
                        _materialStore.Add(newMaterial);
                        UpdateListBoxItems();
                        //добавленный материал сразу раскрываем
                        SetSelectedItem(newMaterial);
                    };
                }

                #endregion

                #region BTN EXPORT

                //для кнопки экспорта настроек добавляем поведение
                var btnExport = GetTemplateChild("PART_ButtonExportMaterial") as ButtonBase;
                if (btnExport != null)
                {
                    btnExport.Click += (sender, args) =>
                    {
                        //по клику кнопки - открываем диалог сохранения файла
                        var saveFileDialog = new SaveFileDialog
                        {
                            Filter = Plastic_en_EN.FileFilter,
                            InitialDirectory = _settingsManager.ExportDirectory,
                            OverwritePrompt = true,
                            CreatePrompt = false,
                            AddExtension = true
                        };
                        if (saveFileDialog.ShowDialog() == true)
                        {
                            _settingsManager.ExportDirectory = Path.GetDirectoryName(saveFileDialog.FileName);
                            //экспортируем материалы полностью
                            _materialStore.ExportAll(saveFileDialog.FileName);
                        }
                    };
                }

                #endregion

                #region BTN IMPORT

                var btnImport = GetTemplateChild("PART_ButtonImportMaterials") as ButtonBase;
                if (btnImport != null)
                {
                    btnImport.Click += (sender, args) =>
                    {
                        var openFileDialog = new OpenFileDialog
                        {
                            Filter = Plastic_en_EN.FileFilter,
                            InitialDirectory = _settingsManager.ImportDirectory
                        };
                        if (openFileDialog.ShowDialog() == true)
                        {
                            _settingsManager.ImportDirectory = Path.GetDirectoryName(openFileDialog.FileName);
                            //импортируем материалы
                            try
                            {
                                var plastics = _materialStore.GetMementosFromFile(openFileDialog.FileName);
                                if (plastics != null && plastics.Any())
                                {
                                    var conflicts = _materialStore.CheckForConflicts(plastics);
                                    var plasticConflictDictionary = new Dictionary<IMaterialPMemento, ConflictMode>();
                                    if (conflicts.conflicts.Any())
                                    {
                                        var conflictDialog = new ConflictDialog(conflicts.conflicts);
                                        if (conflictDialog.ShowDialog() == true)
                                        {
                                            foreach (var plasticConflict in conflictDialog.PlasticConflicts)
                                            {
                                                plasticConflictDictionary.Add(plasticConflict.Essence, plasticConflict.ConflictMode);
                                            }
                                            
                                        }
                                    }
                                    _materialStore.ProcessConflictsAndClears(plasticConflictDictionary, conflicts.clears);
                                }
                            }
                            catch (JsonSerializationException)
                            {
                                MessageBox.Show(Errors_en_EN.FileReading);
                            }

                            UpdateListBoxItems();
                        }
                    };
                }

                #endregion
            }
        }

        #region GET PLASTICS

        private static ObservableCollection<IMaterialPMemento> GetPlastics()
        {
            //для каждого материала привязываем событие удаления и копирования
            //сначала отвязываем материалы от событий, т.к. при дублицировании будет двойной вызов события 
            //(пока событие выполняется  - при привязке необработанного до конца событи оно вызовется снова)
            var defaults = _materialStore.Defaults.OrderBy(a => a.Name).ToList();
            var users = _materialStore.Users.OrderBy(a => a.Name).ToList();
            return new ObservableCollection<IMaterialPMemento>(defaults.Union(users));
        }

        #endregion

        #region UPDATE LIST BOX ITEMS

        private static void UpdateListBoxItems()
        {
            _materials = GetPlastics();
            if (_listBox == null) return;

            _listBox.ItemsSource = null;
            _listBox.ItemsSource = _materials;
        }

        private static void SetSelectedItem(IMaterialPMemento memento)
        {
            if (_listBox == null) return;
            foreach (var listBoxItem in _listBox.Items)
            {
                var materialPMemento = listBoxItem as IMaterialPMemento;
                if (materialPMemento == null || !materialPMemento.Equals(memento)) continue;
                _listBox.SelectedItem = listBoxItem;
            }
        }

        #endregion
    }
}
