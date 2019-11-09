#define ASYNC

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Aura.Controls;
using Aura.CORE.AuraFile;
using Aura.Managers;
using Aura.Themes;
using Aura.Themes.Localization;
using Aura.Views;
using LayersStruct.FirstLayer;
using LayersStruct.MacroLayer.MicroLayer;
using Microsoft.Win32;
using PostProcessor.Blocks;
using Prism.Commands;
using Settings;
using Settings.Memento;
using Settings.Stores;

namespace Aura.ViewModels
{
    public sealed class SessionVM : INotifyPropertyChanged
    {
        private ISessionStore _sessionStore;
        private ISettingsStore<IMaterialPMemento> _materialPStore;
        private ISettingsStore<IMaterialFMemento> _materialFStore;
        private ISettingsStore<IPrinterMemento> _printerStore;
        private ISettingsStore<IProfileMemento> _profileStore;
        private ISlicingManager _slicingManager;
        private ISettingsManager _settingsManager;
        private IAuraViewportManager _auraViewportManager;

        private SettingsCollectionPlasticVM _plasticsVM;
        private SettingsCollectionFiberVM _fibersVM;
        private SettingsCollectionPrinterVM _printersVM;
        private SettingsCollectionProfileVM _profilesVM;
        private ModelsCollectionVM _modelsCollectionVM;

        #region SESSION MEMENTO

        private ISessionMemento _sessionMemento;

        public ISessionMemento SessionMemento
        {
            get => _sessionMemento;
            private set
            {
                if (_sessionMemento != null) _sessionMemento.PropertyChanged -= SessionMementoOnPropertyChanged;
                if (_sessionMemento?.Printer != null) _sessionMemento.Printer.PropertyChanged -= PrinterOnPropertyChanged;
                if (_sessionMemento?.Profile?.GlobalVariableSettingsMemento != null) _sessionMemento.Profile.GlobalVariableSettingsMemento.PropertyChanged -= GlobalVariableMementoOnPropertyChanged;
                if (_sessionMemento?.Profile?.BrimSettingsMemento != null) _sessionMemento.Profile.BrimSettingsMemento.PropertyChanged -= BrimSkirtSettingsOnPropertyChanged;
                if (_sessionMemento?.Profile?.SkirtSettingsMemento != null) _sessionMemento.Profile.SkirtSettingsMemento.PropertyChanged -= BrimSkirtSettingsOnPropertyChanged;
                _sessionMemento = value;
                if (_sessionMemento != null) _sessionMemento.PropertyChanged += SessionMementoOnPropertyChanged;
                if (_sessionMemento?.Printer != null) _sessionMemento.Printer.PropertyChanged += PrinterOnPropertyChanged;
                if (_sessionMemento?.Profile?.GlobalVariableSettingsMemento != null) _sessionMemento.Profile.GlobalVariableSettingsMemento.PropertyChanged += GlobalVariableMementoOnPropertyChanged;
                if (_sessionMemento?.Profile?.BrimSettingsMemento != null) _sessionMemento.Profile.BrimSettingsMemento.PropertyChanged += BrimSkirtSettingsOnPropertyChanged;
                if (_sessionMemento?.Profile?.SkirtSettingsMemento != null) _sessionMemento.Profile.SkirtSettingsMemento.PropertyChanged += BrimSkirtSettingsOnPropertyChanged;
                OnPropertyChanged();
            }
        }

        private void BrimSkirtSettingsOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _auraViewportManager.CheckIntersections();
        }

        private void GlobalVariableMementoOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "GenerateBrim" || e.PropertyName == "GenerateSkirt")
            {
                _auraViewportManager.CheckIntersections();
            }
        }

        private void SessionMementoOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _slicingStatusSemaphor.RaiseToBase();
            SessionProjectChanged = true;
            ResetStates();
            if (e.PropertyName == "Printer")
            {
                UpdateTable();
            }
        }

        private void PrinterOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Width" || e.PropertyName == "Height" || e.PropertyName == "Length")
            {
                ResetStates();
                UpdateTable();
            }
        }

        #endregion

        #region PLASTICS

        private ObservableCollection<IMaterialPMemento> _plastics;

        public ObservableCollection<IMaterialPMemento> Plastics
        {
            get => _plastics;
            set
            {
                _plastics = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region FIBERS

        private ObservableCollection<IMaterialFMemento> _fibers;

        public ObservableCollection<IMaterialFMemento> Fibers
        {
            get => _fibers;
            set
            {
                _fibers = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region PRINTERS

        private ObservableCollection<IPrinterMemento> _printers;

        public ObservableCollection<IPrinterMemento> Printers
        {
            get => _printers;
            set
            {
                _printers = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region PROFILES

        private ObservableCollection<IProfileMemento> _profiles;

        public ObservableCollection<IProfileMemento> Profiles
        {
            get => _profiles;
            set
            {
                _profiles = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region UTILITY COLLECTIONS
        public ObservableCollection<FiberInfillTypeData> FiberInfillTypeCollection { get; private set; }

        #endregion

        #region PRINT INFORMATION

        private PrintInformationVM _printInformationVM;

        public PrintInformationVM PrintInformationVM
        {
            get => _printInformationVM;
            private set
            {
                _printInformationVM?.OnUnload();
                _printInformationVM = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region CAN BE GENERATED

        private bool _canBeGenerated;

        public bool CanBeGenerated
        {
            get => _canBeGenerated;
            set
            {
                if (value == _canBeGenerated) return;
                _canBeGenerated = value;
                if (!_canBeGenerated) PrintInformationVM = null;
                OnPropertyChanged();
            }
        }

        private void ResetStates()
        {
            var (res, message) = GetGenerationInfo();
            CanBeGenerated = res;
            ToolTipGenerationHelp = res ? null : new ToolTip {Content = message};
            SlicingStatusSemaphor.RaiseToBase();
        }

        private (bool res, string message) GetGenerationInfo()
        {
            if (!_modelsCollectionVM.HasActiveItems) return (false, Common_en_EN.MessageNoModel);
            if (_modelsCollectionVM.CollisionTypes != null && _modelsCollectionVM.CollisionTypes.Any())
            {
                if (_modelsCollectionVM.CollisionTypes.Contains(CollisionType.WITHIN_THEMSELVES)) return (false, Common_en_EN.MessageHasIntersectionBetweenThemselves);
                if (_modelsCollectionVM.CollisionTypes.Contains(CollisionType.WITH_PRINT_AREA)) return (false, Common_en_EN.MessageHasIntersectionWithPrintArea);
                if (_modelsCollectionVM.CollisionTypes.Contains(CollisionType.BRIM_OR_SKIRT)) return (false, Common_en_EN.MessageHasIntersectionBrimSkirtWithPrintArea);
                if (_modelsCollectionVM.CollisionTypes.Contains(CollisionType.WITH_WT)) return (false, Common_en_EN.MessageHasIntersectionWithWipeTower);
            }
            if (SessionMemento.Profile.GlobalSettingsMemento.TrueAllowGenerateFiber &&
                (SessionMemento.InsetFiberExtruder == null || SessionMemento.InfillFiberExtruder == null))
                return (false, Common_en_EN.MessageNoFiberExtruder);
            return (true, null);
        }

        public ToolTip _toolTipGenerationHelp;

        public ToolTip ToolTipGenerationHelp
        {
            get => _toolTipGenerationHelp;
            set
            {
                if (Equals(value, _toolTipGenerationHelp)) return;
                _toolTipGenerationHelp = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region SLICING FINISHED

        private bool _slicingFinished;

        public bool SlicingFinished
        {
            get => _slicingFinished;
            set
            {
                _slicingFinished = value;
                if (_slicingFinished)
                {
                    _slicingStatusSemaphor.RaiseToFinish();
                    SlicingInProcess = false;
                }
                ViewCodeCommand?.RaiseCanExecuteChanged();
                CancelCommand?.RaiseCanExecuteChanged();
                SaveToFileCommand?.RaiseCanExecuteChanged();
                OnPropertyChanged();
            }
        }

        #endregion

        #region SLICING PROGRESS PERCENT

        private int _slicingProgressPercent;

        public int SlicingProgressPercent
        {
            get => _slicingProgressPercent;
            set
            {
                _slicingProgressPercent = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region SEMAPHOR

        public sealed class StatusSemaphor
        {
            internal void RaiseToBase()
            {
                ToBase?.Invoke(this, null);
            }

            internal void RaiseToFinish()
            {
                ToFinish?.Invoke(this, null);
            }

            public event EventHandler ToBase;
            public event EventHandler ToFinish;
        }

        private StatusSemaphor _slicingStatusSemaphor;

        public StatusSemaphor SlicingStatusSemaphor
        {
            get => _slicingStatusSemaphor;
            private set
            {
                if (Equals(_slicingStatusSemaphor, value)) return;
                if (_slicingStatusSemaphor != null) _slicingStatusSemaphor.ToBase -= SlicingStatusSemaphorOnToBase;
                _slicingStatusSemaphor = value;
                _slicingStatusSemaphor.ToBase += SlicingStatusSemaphorOnToBase;
                OnPropertyChanged();
            }
        }

        private void SlicingStatusSemaphorOnToBase(object sender, EventArgs e)
        {
            SlicingProgressPercent = 0;
            PrintInformationVM = null;
            SlicingFinished = false;
        }

        #endregion

        #region HAS LAYUP RULES

        private bool _hasLayupRules;

        public bool HasLayupRules
        {
            get => _hasLayupRules;
            set
            {
                if (_hasLayupRules == value) return;
                _hasLayupRules = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public DelegateCommand GenerateCommand { get; private set; }
        public DelegateCommand CancelCommand { get; private set; }
        public DelegateCommand SaveToFileCommand { get; private set; }
        public DelegateCommand ViewCodeCommand { get; private set; }

        public DelegateCommand EditPrinterCommand { get; private set; }
        public DelegateCommand EditProfileCommand { get; private set; }
        public DelegateCommand<IMaterialPMemento> EditMaterialPCommand { get; private set; }
        public DelegateCommand<IMaterialFMemento> EditMaterialFCommand { get; private set; }

        public DelegateCommand OpenProjectCommand { get; private set; }
        public DelegateCommand OpenNewProjectCommand { get; private set; }
        public DelegateCommand SaveProjectAsCommand { get; private set; }
        public DelegateCommand SaveCurrentProjectCommand { get; private set; }

        public event EventHandler<IPrinterMemento> EditPrinterChoosen;
        public event EventHandler<IProfileMemento> EditProfileChoosen;
        public event EventHandler<IMaterialPMemento> EditMaterialPChoosen;
        public event EventHandler<IMaterialFMemento> EditMaterialFChoosen;
        public event EventHandler<(IFirstLayer fl, List<IMicroLayer> microLayers, List<IPPBlock> blocks)> OpenCodeViewer;

        public SessionVM()
        {
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

            GenerateCommand = new DelegateCommand(GenerateExecuteMethod, () => !SlicingInProcess);
            CancelCommand = new DelegateCommand(CancelExecuteMethod, () => SlicingInProcess);
            SaveToFileCommand = new DelegateCommand(SaveToFileExecuteMethod, () => SlicingFinished);
            ViewCodeCommand = new DelegateCommand(ViewCodeExecuteMethod, () => SlicingFinished);

            EditPrinterCommand = new DelegateCommand(EditPrinterMethod);
            EditProfileCommand = new DelegateCommand(EditProfileMethod);
            EditMaterialPCommand = new DelegateCommand<IMaterialPMemento>(EditMaterialPMethod);
            EditMaterialFCommand = new DelegateCommand<IMaterialFMemento>(EditMaterialFMethod);

            OpenProjectCommand = new DelegateCommand(OpenProjectExecuteMethod);
            OpenNewProjectCommand = new DelegateCommand(CreateNewProjectExecute);
            SaveProjectAsCommand = new DelegateCommand(SaveProjectAsExecuteMethod);
            SaveCurrentProjectCommand = new DelegateCommand(SaveCurrentProjectExecuteMethod);

            this.SlicingStatusSemaphor = new StatusSemaphor();
        }

        public ProjectFileData ProjectFileData { get; private set; }

        public event EventHandler ProjectChanged;
        //public event EventHandler TableChanged;

        private bool _slicingInProcess;
        private bool SlicingInProcess
        {
            get => _slicingInProcess;
            set
            {
                _slicingInProcess = value;
                GenerateCommand?.RaiseCanExecuteChanged();
                CancelCommand?.RaiseCanExecuteChanged();
                if (_modelsCollectionVM.SlicingInProcess != SlicingInProcess) _modelsCollectionVM.SlicingInProcess = SlicingInProcess;
            }
        }

        #region SESSION PROJECT CHANGED

        private bool _sessionProjectChanged;

        public bool SessionProjectChanged
        {
            get => _sessionProjectChanged;
            private set
            {
                var oldState = _sessionProjectChanged;
                _sessionProjectChanged = value;
                if (_sessionProjectChanged && _modelsCollectionVM.SlicingInProcess && CancelCommand.CanExecute()) CancelCommand.Execute();
                if (oldState != _sessionProjectChanged) ProjectChanged?.Invoke(this, null);
            }
        }

        #endregion

        #region CREATE NEW PROJECT EXECUTE

        private void CreateNewProjectExecute()
        {
            if (SessionProjectChanged)
            {
                var res = MessageBox.Show(Common_en_EN.OpenProjectHasChanges.Replace("{PROJECT_NAME}", ProjectFileData.ProjectName),
                    Common_en_EN.OpenProjectTitle, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                switch (res)
                {
                    case MessageBoxResult.Cancel:
                        return;
                    case MessageBoxResult.Yes:
                        SaveProjectAsExecuteMethod();
                        break;
                }
            }

            _modelsCollectionVM.Clear();
            ResetStates();
            ProjectFileData = new ProjectFileData { ProjectName = Common_en_EN.DefaultProjectName };
            SessionProjectChanged = false;
        }

        #endregion

        #region OPEN PROJECT EXECUTE

        private void OpenProjectExecuteMethod()
        {
            if (SessionProjectChanged)
            {
                var res = MessageBox.Show(Common_en_EN.OpenProjectHasChanges.Replace("{PROJECT_NAME}", ProjectFileData.ProjectName),
                    Common_en_EN.OpenProjectTitle, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                switch (res)
                {
                    case MessageBoxResult.Cancel:
                        return;
                    case MessageBoxResult.Yes:
                        SaveProjectAsExecuteMethod();
                        break;
                }
            }

            var openFileDialog = new OpenFileDialog
            {
                InitialDirectory = _settingsManager.ProjectDirectory,
                Filter = Common_en_EN.ProjectFilter
            };
            if (openFileDialog.ShowDialog() == true)
            {
                _settingsManager.ProjectDirectory = Path.GetDirectoryName(openFileDialog.FileName);
                ProjectFileData.ProjectPath = openFileDialog.FileName;
                OpenProject(ProjectFileData.ProjectPath);
            }
        }

        public void OpenProject(string path)
        {
            var auraMainFile = AuraMainPrinting.ReadFile(path);
            if (auraMainFile == null) return;

            var (plastics, fibers) = AuraMainPrinting.GetMaterialInExtruder(auraMainFile);
            var allPlastics = plastics != null && plastics.Any() ? plastics.Select(a => a.Key).ToList() : null;
            var allFibers = fibers != null && fibers.Any() ? fibers.Select(a => a.Key).ToList() : null;

            var (plsticConflicts, plasticClears) = _materialPStore.CheckForConflicts(allPlastics);
            var (fiberConflicts, fiberClears) = _materialFStore.CheckForConflicts(allFibers);
            var (printerConflicts, printerClears) = _printerStore.CheckForConflicts(new List<IPrinterMemento> { auraMainFile.SessionMemento.Printer });
            var (profileConflicts, profileClears) = _profileStore.CheckForConflicts(new List<IProfileMemento> { auraMainFile.SessionMemento.Profile });

            var cdVM = new ConflictCollectionVM();
            var hasPlasticConflicts = plsticConflicts != null && plsticConflicts.Any();
            var hasFiberConflicts = fiberConflicts != null && fiberConflicts.Any();
            var hasPrintersConflicts = printerConflicts != null && printerConflicts.Any();
            var hasProfilesConflicts = profileConflicts != null && profileConflicts.Any();

            if (hasPlasticConflicts) cdVM.Initialize(plsticConflicts);
            if (hasFiberConflicts) cdVM.Initialize(fiberConflicts);
            if (hasPrintersConflicts) cdVM.Initialize(printerConflicts);
            if (hasProfilesConflicts) cdVM.Initialize(profileConflicts);

            if (!cdVM.Empty)
            {
                var conflictDialog = new ConflictDialogNew { DataContext = cdVM };
                if (conflictDialog.ShowDialog() == true)
                {
                    if (hasPlasticConflicts) _materialPStore.ProcessConflicts(cdVM.GetConflicts<IMaterialPMemento>());
                    if (hasFiberConflicts) _materialFStore.ProcessConflicts(cdVM.GetConflicts<IMaterialFMemento>());
                    if (hasPrintersConflicts) _printerStore.ProcessConflicts(cdVM.GetConflicts<IPrinterMemento>());
                    if (hasProfilesConflicts) _profileStore.ProcessConflicts(cdVM.GetConflicts<IProfileMemento>());
                }
                else return;
            }

            if (plasticClears != null && plasticClears.Any())
            {
                _materialPStore.ProcessClears(plasticClears);
                AuraMainPrinting.UpdatePlastics(auraMainFile, plastics);
            }

            if (fiberClears != null && fiberClears.Any())
            {
                _materialFStore.ProcessClears(fiberClears);
                AuraMainPrinting.UpdateFibers(auraMainFile, fibers);
            }
            if (printerClears != null && printerClears.Any()) _printerStore.ProcessClears(printerClears);
            if (profileClears != null && profileClears.Any()) _profileStore.ProcessClears(profileClears);

            ProjectFileData.ProjectPath = path;
            _sessionStore.Session = auraMainFile.SessionMemento;
            _modelsCollectionVM.LoadModels(auraMainFile.ModelsAndLayups);
            SessionProjectChanged = false;

            UpdateTable();
        }

        #endregion

        #region SAVE PROJECT AS EXECUTE

        private void SaveProjectAsExecuteMethod()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = Common_en_EN.ProjectFilter,
                InitialDirectory = _settingsManager.ProjectDirectory,
                FileName = (ProjectFileData.ProjectName == null || !ProjectFileData.ProjectName.Any() ||
                            ProjectFileData.ProjectName == Common_en_EN.DefaultProjectName)
                    ? GetProjectName()
                    : ProjectFileData.ProjectName
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                ProjectFileData.ProjectPath = saveFileDialog.FileName;
                var auraMainPrinting = new AuraMainPrinting();
                _settingsManager.ProjectDirectory = Path.GetDirectoryName(ProjectFileData.ProjectPath);
                auraMainPrinting.SaveFile(_modelsCollectionVM.GetItems(), _sessionStore.Session, ProjectFileData.ProjectPath);
                SessionProjectChanged = false;
            }
        }

        #endregion

        #region SAVE CURRENT PROJECT EXECUTE 

        private void SaveCurrentProjectExecuteMethod()
        {
            if (ProjectFileData.ProjectPath == null)
            {
                SaveProjectAsExecuteMethod();
                return;
            }

            var auraMainPrinting = new AuraMainPrinting();
            auraMainPrinting.SaveFile(_modelsCollectionVM.GetItems(), _sessionStore.Session, ProjectFileData.ProjectPath);
            SessionProjectChanged = false;
        }

        public void SaveCurrentProjectSilent(string path)
        {
            var auraMainPrinting = new AuraMainPrinting();
            auraMainPrinting.SaveFile(_modelsCollectionVM.GetItems(), _sessionStore.Session, path);
        }

        #endregion

        #region EDIT MATERIAL F METHOD

        private void EditMaterialFMethod(IMaterialFMemento obj)
        {
            EditMaterialFChoosen?.Invoke(this, obj);
        }

        #endregion

        #region EDIT MATERIAL P METHOD

        private void EditMaterialPMethod(IMaterialPMemento obj)
        {
            EditMaterialPChoosen?.Invoke(this, obj);
        }

        #endregion

        #region EDIT PROFILE METHOD

        private void EditProfileMethod()
        {
            EditProfileChoosen?.Invoke(this, SessionMemento.Profile);
        }

        #endregion

        #region EDIT PRINTER

        private void EditPrinterMethod()
        {
            EditPrinterChoosen?.Invoke(this, SessionMemento.Printer);
        }

        #endregion

        #region VIEW CODE EXECUTE METHOD

        private void ViewCodeExecuteMethod()
        {
            OpenCodeViewer?.Invoke(this, (_generated.Value.FirstLayer, _generated.Value.MicroLayers, _generated.Value.GeneratedBlocks));
        }

        #endregion

        #region SAVE TO FILE EXECUTE

        private void SaveToFileExecuteMethod()
        {
            var nameSuggestion = GetProjectName();
            var saveFileDialog = new SaveFileDialog
            {
                InitialDirectory = _settingsManager.GCodeDirectory,
                Filter = Common_en_EN.GCodeFilter,
                FileName = nameSuggestion
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                _settingsManager.GCodeDirectory = Path.GetDirectoryName(saveFileDialog.FileName);
                var fileName = saveFileDialog.FileName;
                try
                {
                    using (var file = File.CreateText(fileName))
                    {
                        file.Write(_generated.Value.GCode);
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show(Common_en_EN.FileWritingExceptionText, Common_en_EN.FileWritingExceptionCaption, MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        #endregion

        #region CANCEL EXECUTE

        private void CancelExecuteMethod()
        {
            SlicingInProcess = false;
            _cancellationTokenSource.Cancel();
        }

        #endregion

        #region GENERATE EXECUTE

        private async Task GenerateExecuteMethodAsync()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var generationData = new GenerationData();
            generationData.PercentProgressChanged += GenerationDataOnPercentProgressChanged;
            double? time = null;

#if ASYNC

            var res = await _slicingManager.GenerateAsync(_modelsCollectionVM.GetItems(), SessionMemento, generationData,
                _cancellationTokenSource);
            SlicingFinished = true;

            _generated = null;
            if (res.e != null)
            {
                if (!HasCancelled(res.e))
                {
                    var errorDialog = new ErrorDialog(res.e);
                    errorDialog.ShowDialog();
                }
                else
                {
                    SlicingStatusSemaphor.RaiseToBase();
                }

                return;
            }
            else
            {
                _generated = res.generated;
                time = res.time;
            }

#else
            var res = _slicingManager.GenerateSync(_modelsCollectionVM.GetItems(), SessionMemento, generationData, _cancellationTokenSource);
            SlicingFinished = true;
            _generated = res.generated;
            time = res.time;
#endif
            if (!time.HasValue || !_generated.HasValue)
            {
                var errorDialog = new ErrorDialog(new Exception("No time or generated value"));
                errorDialog.ShowDialog();
                return;
            }

            var printInformationVm = new PrintInformationVM();
            var timeSpan = TimeSpan.FromSeconds(time.Value);
            printInformationVm.Initialize(generationData.PlasticConsumption, generationData.CompositeConsumption, timeSpan.Hours + timeSpan.Days * 24,
                timeSpan.Minutes);
            PrintInformationVM = printInformationVm;
            _cancellationTokenSource = null;
        }


        private async void GenerateExecuteMethod()
        {
            SlicingInProcess = true;
#if ASYNC
            await GenerateExecuteMethodAsync();
#else
            GenerateExecuteMethodAsync();
#endif
        }

        private void GenerationDataOnPercentProgressChanged(object sender, EventArgs e)
        {
            SlicingProgressPercent = (sender as GenerationData).PercentProgress;
        }

        private bool HasCancelled(Exception e)
        {
            if (e is AggregateException ae)
            {
                foreach (var innerException in ae.InnerExceptions)
                {
                    if (HasCancelled(innerException)) return true;
                }
            }
            else
            {
                if (e is OperationCanceledException) return true;
            }

            return false;
        }

        #endregion

        private CancellationTokenSource _cancellationTokenSource;
        private Generated? _generated;

        #region INITIALIZE

        public void Initialize(ISessionStore sessionStore,
            ISettingsStore<IMaterialPMemento> materialPStore,
            ISettingsStore<IMaterialFMemento> materialFStore,
            ISettingsStore<IPrinterMemento> printerStore,
            ISettingsStore<IProfileMemento> profileStore,
            ISlicingManager slicingManager,
            IAuraViewportManager auraViewportManager,
            ISettingsManager settingsManager,
            SettingsCollectionPlasticVM plasticVM,
            SettingsCollectionFiberVM fiberVM,
            SettingsCollectionPrinterVM printerVM,
            SettingsCollectionProfileVM profileVM,
            ModelsCollectionVM modelsCollectionVM)
        {
            _sessionStore = sessionStore;
            _materialPStore = materialPStore;
            _materialFStore = materialFStore;
            _printerStore = printerStore;
            _profileStore = profileStore;
            _sessionStore.SessionChanged += (sender, args) => SessionMemento = _sessionStore.Session;
            _slicingManager = slicingManager;
            _auraViewportManager = auraViewportManager;
            _settingsManager = settingsManager;
            SessionMemento = _sessionStore.Session;
            _plasticsVM = plasticVM;
            _fibersVM = fiberVM;
            _printersVM = printerVM;
            _profilesVM = profileVM;
            _modelsCollectionVM = modelsCollectionVM;
            _modelsCollectionVM.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "ActiveItems" || args.PropertyName == "CollisionTypes")
                {
                    ResetStates();
                    SessionProjectChanged = true;
                }
            };
            _modelsCollectionVM.ModelsChanged += (sender, args) =>
            {
                HasLayupRules = _modelsCollectionVM.ActiveItems.Count(a => a.HasData) > 0;
                SlicingStatusSemaphor.RaiseToBase();
                SessionProjectChanged = true;
            };

            Plastics = _plasticsVM.RealItems;
            Fibers = _fibersVM.RealItems;
            Printers = _printersVM.RealItems;
            Profiles = _profilesVM.RealItems;

            ProjectFileData = new ProjectFileData {ProjectName = Common_en_EN.DefaultProjectName};
            ResetStates();
        }

        #endregion

        private void UpdateTable()
        {
            if (!_auraViewportManager.NeedUpdateTable(_sessionStore.Session.Printer.TrueWidth, _sessionStore.Session.Printer.TrueLength)) return;
            _auraViewportManager.UpdateTable();
        }

        #region GET PROJECT NAME

        private string GetProjectName()
        {
            var projNameFromModels = string.Empty;
            var allUniqueNames = _modelsCollectionVM.Select(a => a.EntityName).Distinct();
            foreach (var uniqueName in allUniqueNames)
            {
                projNameFromModels += uniqueName + "_";
            }

            return projNameFromModels.TrimEnd('_');
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

        public struct Generated
        {
            public string GCode { get; set; }
            public List<IPPBlock> GeneratedBlocks { get; set; }
            public IFirstLayer FirstLayer { get; set; }
            public List<IMicroLayer> MicroLayers { get; set; }
        }
    }

    #region PROJECT FILE DATA

    public class ProjectFileData : INotifyPropertyChanged
    {
        private string _projectName;

        public string ProjectName
        {
            get => _projectName;
            set
            {
                _projectName = value;
                OnPropertyChanged();
            }
        }

        private string _projectPath;

        public string ProjectPath
        {
            get => _projectPath;
            set
            {
                _projectPath = value;
                ProjectName = Path.GetFileName(_projectPath);
                OnPropertyChanged();
            }
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

    #endregion
}