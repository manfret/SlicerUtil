using System;
using System.IO;
using System.Windows.Media;
using Aura.Themes.Localization;
using Settings.Memento;

namespace Aura.Managers
{
    public interface ISettingsManager
    {
        string ImportDirectory { get; set; }

        string ExportDirectory { get; set; }

        string GCodeDirectory { get; set; }

        string ModelDirectory { get; set; }

        string ProjectDirectory { get; set; }

        string FirmwareDirectory { get; set; }

        Color Inset0Color { get; set; }

        Color MicroInfillColor { get; set; }

        Color InsetXPColor { get; set; }

        Color InfillSolidColor { get; set; }

        Color InfillCellularColor { get; set; }

        Color SupportThinColor { get; set; }

        Color SupportThickColor { get; set; }

        Color SkirtColor { get; set; }

        Color BrimColor { get; set; }

        Color WipeColor { get; set; }

        Color InsetXFColor { get; set; }
        
        Color FiberInfillColor { get; set; }

        Color TravelColor { get; set; }

        Color MovePColor { get; set; }

        Color MovePFColor { get; set; }

        Color ExtrudePColor { get; set; }

        Color ExtrudePFColor { get; set; }

        Color RetractColor { get; set; }

        Color CutColor { get; set; }

        bool Inset0Show { get; set; }

        bool MicroInfillShow { get; set; }

        bool InsetXPShow { get; set; }

        bool InfillSolidShow { get; set; }

        bool InfillCellularShow { get; set; }

        bool SupportThinShow { get; set; }

        bool SupportThickShow { get; set; }

        bool SkirtShow { get; set; }

        bool BrimShow { get; set; }

        bool WipeShow { get; set; }
        
        bool InsetXFShow { get; set; } 
        
        bool FiberInfillShow { get; set; } 

        bool TravelShow { get; set; }

        bool MovePShow { get; set; }

        bool MovePFShow { get; set; }

        bool ExtrudePShow { get; set; }

        bool ExtrudePFShow { get; set; }

        bool RetractShow { get; set; }

        bool CutShow { get; set; }

        string GetFilter<T>() where T : ISettingsMemento;

    }

    public class SettingsManager : ISettingsManager
    {
        private readonly string _myDocuments;

        private string _importDirectory;
        public string ImportDirectory
        {
            get
            {
                if (!Directory.Exists(_importDirectory)) ImportDirectory = _myDocuments;
                return _importDirectory;
            }
            set
            {
                _importDirectory = value;
                Properties.Settings.Default.ImportMaterialPath = value;
                Properties.Settings.Default.Save();
            }
        }

        private string _exportDirectory;
        public string ExportDirectory
        {
            get
            {
                if (!Directory.Exists(_exportDirectory)) ExportDirectory = _myDocuments;
                return _exportDirectory;
            }
            set
            {
                _exportDirectory = value;
                Properties.Settings.Default.ExportMaterialPath = value;
                Properties.Settings.Default.Save();
            }
        }

        private string _gCodeDirectory;
        public string GCodeDirectory
        {
            get
            {
                if (!Directory.Exists(_gCodeDirectory)) GCodeDirectory = _myDocuments;
                return _gCodeDirectory;
            }
            set
            {
                _gCodeDirectory = value;
                Properties.Settings.Default.GCodeDirectory = value;
                Properties.Settings.Default.Save();
            }
        }

        private string _modelDirectory;
        public string ModelDirectory
        {
            get
            {
                if (!Directory.Exists(_modelDirectory)) ModelDirectory = _myDocuments;
                return _modelDirectory;
            }
            set
            {
                _modelDirectory = value;
                Properties.Settings.Default.ModelDirectory = value;
                Properties.Settings.Default.Save();
            }
        }

        private string _projectDirectory;
        public string ProjectDirectory
        {
            get
            {
                if (!Directory.Exists(_projectDirectory)) ProjectDirectory = _myDocuments;
                return _projectDirectory;
            }
            set
            {
                _projectDirectory = value;
                Properties.Settings.Default.ProjectDirectory = value;
                Properties.Settings.Default.Save();
            }
        }

        private string _firmwareDirectory;
        public string FirmwareDirectory
        {
            get
            {
                if (!Directory.Exists(_firmwareDirectory)) FirmwareDirectory = _myDocuments;
                return _firmwareDirectory;
            }
            set
            {
                _firmwareDirectory = value;
                Properties.Settings.Default.FirmwareDirectory = value;
                Properties.Settings.Default.Save();
            }
        }

        private Color _inset0Color;
        public Color Inset0Color
        {
            get => _inset0Color;
            set
            {
                _inset0Color = value;
                Properties.Settings.Default.Inset0Color = value;
                Properties.Settings.Default.Save();
            }
        }

        private Color _microInfillColor;
        public Color MicroInfillColor
        {
            get => _microInfillColor;
            set
            {
                _microInfillColor = value;
                Properties.Settings.Default.MicroInfillColor = value;
                Properties.Settings.Default.Save();
            }
        }

        private Color _insetXPColor;
        public Color InsetXPColor
        {
            get => _insetXPColor;
            set
            {
                _insetXPColor = value;
                Properties.Settings.Default.InsetXPColor = value;
                Properties.Settings.Default.Save();
            }
        }

        private Color _infillSolidColor;
        public Color InfillSolidColor
        {
            get => _infillSolidColor;
            set
            {
                _infillSolidColor = value;
                Properties.Settings.Default.InfillSolidColor = value;
                Properties.Settings.Default.Save();
            }
        }

        private Color _infillCellularColor;
        public Color InfillCellularColor
        {
            get => _infillCellularColor;
            set
            {
                _infillCellularColor = value;
                Properties.Settings.Default.InfillCellularColor = value;
                Properties.Settings.Default.Save();
            }
        }

        private Color _supportThinColor;
        public Color SupportThinColor
        {
            get => _supportThinColor;
            set
            {
                _supportThinColor = value;
                Properties.Settings.Default.SupportThinColor = value;
                Properties.Settings.Default.Save();
            }
        }

        private Color _supportThickColor;
        public Color SupportThickColor
        {
            get => _supportThickColor;
            set
            {
                _supportThickColor = value;
                Properties.Settings.Default.SupportThickColor = value;
                Properties.Settings.Default.Save();
            }
        }

        private Color _skirtColor;
        public Color SkirtColor
        {
            get => _skirtColor;
            set
            {
                _skirtColor = value;
                Properties.Settings.Default.SkirtColor = value;
                Properties.Settings.Default.Save();
            }
        }

        private Color _brimColor;
        public Color BrimColor
        {
            get => _brimColor;
            set
            {
                _brimColor = value;
                Properties.Settings.Default.BrimColor = value;
                Properties.Settings.Default.Save();
            }
        }

        private Color _wipeColor;
        public Color WipeColor
        {
            get => _wipeColor;
            set
            {
                _wipeColor = value;
                Properties.Settings.Default.WipeColor = value;
                Properties.Settings.Default.Save();
            }
        }

        private Color _insetXFColor;
        public Color InsetXFColor
        {
            get => _insetXFColor;
            set
            {
                _insetXFColor = value;
                Properties.Settings.Default.InsetXFColor = value;
                Properties.Settings.Default.Save();
            }
        }

        private Color _fiberInfillColor;
        public Color FiberInfillColor
        {
            get => _fiberInfillColor;
            set
            {
                _fiberInfillColor = value;
                Properties.Settings.Default.FiberInfillColor = value;
                Properties.Settings.Default.Save();
            }
        }

        private Color _travelColor;
        public Color TravelColor
        {
            get => _travelColor;
            set
            {
                _travelColor = value;
                Properties.Settings.Default.TravelColor = value;
                Properties.Settings.Default.Save();
            }
        }

        private Color _movePColor;
        public Color MovePColor
        {
            get => _movePColor;
            set
            {
                _movePColor = value;
                Properties.Settings.Default.MovePColor = value;
                Properties.Settings.Default.Save();
            }
        }

        private Color _movePFColor;
        public Color MovePFColor
        {
            get => _movePFColor;
            set
            {
                _movePFColor = value;
                Properties.Settings.Default.MovePFColor = value;
                Properties.Settings.Default.Save();
            }
        }

        private Color _extrudePColor;
        public Color ExtrudePColor
        {
            get => _extrudePColor;
            set
            {
                _extrudePColor = value;
                Properties.Settings.Default.ExtrudePColor = value;
                Properties.Settings.Default.Save();
            }
        }

        private Color _extrudePFColor;
        public Color ExtrudePFColor
        {
            get => _extrudePFColor;
            set
            {
                _extrudePFColor = value;
                Properties.Settings.Default.ExtrudePFColor = value;
                Properties.Settings.Default.Save();
            }
        }

        private Color _retractColor;
        public Color RetractColor
        {
            get => _retractColor;
            set
            {
                _retractColor = value;
                Properties.Settings.Default.RetractColor = value;
                Properties.Settings.Default.Save();
            }
        }

        private Color _cutColor;
        public Color CutColor
        {
            get => _cutColor;
            set
            {
                _cutColor = value;
                Properties.Settings.Default.CutColor = value;
                Properties.Settings.Default.Save();
            }
        }

        private bool _inset0Show;
        public bool Inset0Show
        {
            get => _inset0Show;
            set
            {
                _inset0Show = value;
                Properties.Settings.Default.Inset0Show = value;
                Properties.Settings.Default.Save();
            }
        }

        private bool _microInfillShow;
        public bool MicroInfillShow
        {
            get => _microInfillShow;
            set
            {
                _microInfillShow = value;
                Properties.Settings.Default.MicroInfillShow = value;
                Properties.Settings.Default.Save();
            }
        }

        private bool _insetXPShow;
        public bool InsetXPShow
        {
            get => _insetXPShow;
            set
            {
                _insetXPShow = value;
                Properties.Settings.Default.InsetXPShow = value;
                Properties.Settings.Default.Save();
            }
        }

        private bool _infillSolidShow;
        public bool InfillSolidShow
        {
            get => _infillSolidShow;
            set
            {
                _infillSolidShow = value;
                Properties.Settings.Default.InfillSolidShow = value;
                Properties.Settings.Default.Save();
            }
        }

        private bool _infillCellularShow;
        public bool InfillCellularShow
        {
            get => _infillCellularShow;
            set
            {
                _infillCellularShow = value;
                Properties.Settings.Default.InfillCellularShow = value;
                Properties.Settings.Default.Save();
            }
        }

        private bool _supportThinShow;
        public bool SupportThinShow
        {
            get => _supportThinShow;
            set
            {
                _supportThinShow = value;
                Properties.Settings.Default.SupportThinShow = value;
                Properties.Settings.Default.Save();
            }
        }

        private bool _supportThickShow;
        public bool SupportThickShow
        {
            get => _supportThickShow;
            set
            {
                _supportThickShow = value;
                Properties.Settings.Default.SupportThickShow = value;
                Properties.Settings.Default.Save();
            }
        }

        private bool _skirtShow;
        public bool SkirtShow
        {
            get => _skirtShow;
            set
            {
                _skirtShow = value;
                Properties.Settings.Default.SkirtShow = value;
                Properties.Settings.Default.Save();
            }
        }

        private bool _brimShow;
        public bool BrimShow
        {
            get => _brimShow;
            set
            {
                _brimShow = value;
                Properties.Settings.Default.BrimShow = value;
                Properties.Settings.Default.Save();
            }
        }

        private bool _wipeShow;
        public bool WipeShow
        {
            get => _wipeShow;
            set
            {
                _wipeShow = value;
                Properties.Settings.Default.WipeShow = value;
                Properties.Settings.Default.Save();
            }
        }

        private bool _insetXFShow;
        public bool InsetXFShow
        {
            get => _insetXFShow;
            set
            {
                _insetXFShow = value;
                Properties.Settings.Default.InsetXFShow = value;
                Properties.Settings.Default.Save();
            }
        }

        private bool _fiberInfillShow;
        public bool FiberInfillShow
        {
            get => _fiberInfillShow;
            set
            {
                _fiberInfillShow = value;
                Properties.Settings.Default.InfillFiberShow = value;
                Properties.Settings.Default.Save();
            }
        }

        private bool _travelShow;
        public bool TravelShow
        {
            get => _travelShow;
            set
            {
                _travelShow = value;
                Properties.Settings.Default.TravelShow = value;
                Properties.Settings.Default.Save();
            }
        }

        private bool _movePShow;
        public bool MovePShow
        {
            get => _movePShow;
            set
            {
                _movePShow = value;
                Properties.Settings.Default.MovePShow = value;
                Properties.Settings.Default.Save();
            }
        }

        private bool _movePFShow;
        public bool MovePFShow
        {
            get => _movePFShow;
            set
            {
                _movePFShow = value;
                Properties.Settings.Default.MovePFShow = value;
                Properties.Settings.Default.Save();
            }
        }

        private bool _extrudePShow; 
        public bool ExtrudePShow
        {
            get => _extrudePShow;
            set
            {
                _extrudePShow = value;
                Properties.Settings.Default.ExtrudePShow = value;
                Properties.Settings.Default.Save();
            }
        }

        private bool _extrudePFShow;
        public bool ExtrudePFShow
        {
            get => _extrudePFShow;
            set
            {
                _extrudePFShow = value;
                Properties.Settings.Default.ExtrudePFShow = value;
                Properties.Settings.Default.Save();
            }
        }

        private bool _retractShow;
        public bool RetractShow
        {
            get => _retractShow;
            set
            {
                _retractShow = value;
                Properties.Settings.Default.RetractShow = value;
                Properties.Settings.Default.Save();
            }
        }

        private bool _cutShow;
        public bool CutShow
        {
            get => _cutShow;
            set
            {
                _cutShow = value;
                Properties.Settings.Default.CutShow = value;
                Properties.Settings.Default.Save();
            }
        }

        public SettingsManager()
        {
            _myDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //TODO devDept: Environment can be ambiguous reference between devDept.Eyeshot.Environment and System.Environment.
            //TODO devDept: Environment can be ambiguous reference between devDept.Eyeshot.Environment and System.Environment.
            //Suggestion: _myDocuments = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);

            if (Properties.Settings.Default.NeedUpgrade)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.NeedUpgrade = false;
                Properties.Settings.Default.Save();
            }

            var importMaterialDirectory = Properties.Settings.Default.ImportMaterialPath;
            if (string.IsNullOrEmpty(importMaterialDirectory)) importMaterialDirectory = _myDocuments;
            _importDirectory = importMaterialDirectory;

            var exportMaterialDirectory = Properties.Settings.Default.ExportMaterialPath;
            if (string.IsNullOrEmpty(exportMaterialDirectory)) exportMaterialDirectory = _myDocuments;
            _exportDirectory = exportMaterialDirectory;

            var gCodeExportDirectory = Properties.Settings.Default.GCodeDirectory;
            if (string.IsNullOrEmpty(gCodeExportDirectory)) gCodeExportDirectory = _myDocuments;
            _gCodeDirectory = gCodeExportDirectory;

            var modelDirectory = Properties.Settings.Default.ModelDirectory;
            if (string.IsNullOrEmpty(modelDirectory)) modelDirectory = _myDocuments;
            _modelDirectory = modelDirectory;

            var projectDirectory = Properties.Settings.Default.ProjectDirectory;
            if (string.IsNullOrEmpty(projectDirectory)) projectDirectory = _myDocuments;
            _projectDirectory = projectDirectory;

            var firmwareDirectory = Properties.Settings.Default.FirmwareDirectory;
            if (string.IsNullOrEmpty(firmwareDirectory)) firmwareDirectory = _myDocuments;
            _firmwareDirectory = firmwareDirectory;

            _inset0Color = Properties.Settings.Default.Inset0Color;
            _microInfillColor = Properties.Settings.Default.MicroInfillColor;
            _insetXPColor = Properties.Settings.Default.InsetXPColor;
            _infillSolidColor = Properties.Settings.Default.InfillSolidColor;
            _infillCellularColor = Properties.Settings.Default.InfillCellularColor;
            _supportThinColor = Properties.Settings.Default.SupportThinColor;
            _supportThickColor = Properties.Settings.Default.SupportThickColor;
            _brimColor = Properties.Settings.Default.BrimColor;
            _skirtColor = Properties.Settings.Default.SkirtColor;
            _wipeColor = Properties.Settings.Default.WipeColor;
            _insetXFColor = Properties.Settings.Default.InsetXFColor;
            _fiberInfillColor = Properties.Settings.Default.FiberInfillColor;

            _travelColor = Properties.Settings.Default.TravelColor;
            _movePColor = Properties.Settings.Default.MovePColor;
            _movePFColor = Properties.Settings.Default.MovePFColor;
            _extrudePColor = Properties.Settings.Default.ExtrudePColor;
            _extrudePFColor = Properties.Settings.Default.ExtrudePFColor;
            _retractColor = Properties.Settings.Default.RetractColor;
            _cutColor = Properties.Settings.Default.CutColor;

            _inset0Show = Properties.Settings.Default.Inset0Show;
            _microInfillShow = Properties.Settings.Default.MicroInfillShow;
            _insetXPShow = Properties.Settings.Default.InsetXPShow;
            _infillSolidShow = Properties.Settings.Default.InfillSolidShow;
            _infillCellularShow = Properties.Settings.Default.InfillCellularShow;
            _supportThinShow = Properties.Settings.Default.SupportThinShow;
            _supportThickShow = Properties.Settings.Default.SupportThickShow;
            _brimShow = Properties.Settings.Default.BrimShow;
            _skirtShow = Properties.Settings.Default.SkirtShow;
            _wipeShow = Properties.Settings.Default.WipeShow;
            _insetXFShow = Properties.Settings.Default.InsetXFShow;
            _fiberInfillShow = Properties.Settings.Default.InfillFiberShow;

            TravelShow = Properties.Settings.Default.TravelShow;
            MovePShow = Properties.Settings.Default.MovePShow;
            MovePFShow = Properties.Settings.Default.MovePFShow;
            ExtrudePShow = Properties.Settings.Default.ExtrudePShow;
            ExtrudePFShow = Properties.Settings.Default.ExtrudePFShow;
            RetractShow = Properties.Settings.Default.RetractShow;
            CutShow = Properties.Settings.Default.CutShow;
        }

        public string GetFilter<T>() where T : ISettingsMemento
        {
            var mementoType = typeof(T);
            if (mementoType == typeof(IMaterialPMemento)) return Plastic_en_EN.FileFilter;
            if (mementoType == typeof(IMaterialFMemento)) return Fiber_en_EN.FileFilter;
            if (mementoType == typeof(IPrinterMemento)) return Printer_en_EN.FileFilter;
            if (mementoType == typeof(IProfileMemento)) return Profile_en_EN.FileFilter;
            return null;
        }
    }
}