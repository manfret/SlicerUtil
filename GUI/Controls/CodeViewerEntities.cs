using System.Linq;
using System.Windows;
using System.Windows.Media;
using PostProcessor.Properties;

namespace Aura.Obsolete
{
    public sealed partial class CodeViewer
    {
        //метод изменения отображения для геометрической сущности
        private void OnShowTypeChanged(bool newValue, EntityType type)
        {
            if (_geometryLayers == null || !_geometryLayers.Any()) return;
            //находим слой, в котором размещены соответсвующие сущности
            var layers = _geometryLayers.Where(a => a.Key == type).Select(b => b.Value).ToList();
            foreach (var layer in layers)
            {
                layer.Visible = newValue;
            }

            _auraViewportLayout.Invalidate();
        }

        //метод изменгения цвета для геометрической сущности
        private void OnColorChanged(EntityType type, Color newValue)
        {
            _entityColors[type] = newValue;

            if (_geometryDatas == null || !_geometryDatas.Any()) return;
            //находим сущности, которым нужно задать новый цвет
            var needNewColor = _geometryDatas.Where(a => a.Key.entityType == type).ToList();
            var newColor = System.Drawing.Color.FromArgb(_entityColors[type].A, _entityColors[type].R, _entityColors[type].G, _entityColors[type].B);
            //их базовым сущностям (LinearPath) устанавливаем цвет
            needNewColor.ForEach(a=>a.Value.Color = newColor);

            _auraViewportLayout.Invalidate();
        }

        #region INSET 0

        public static readonly DependencyProperty Inset0ColorProperty =
            DependencyProperty.Register("Inset0Color", typeof(Color), typeof(CodeViewer), new PropertyMetadata(Inset0ColorChangedCallback));

        private static void Inset0ColorChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CodeViewer) d).OnInset0ColorChangedCallback(e);
        }

        private void OnInset0ColorChangedCallback(DependencyPropertyChangedEventArgs e)
        {
            OnColorChanged(EntityType.INSET0, (Color) e.NewValue);
            _settingsManager.Inset0Color = (Color)e.NewValue;
        }

        public Color Inset0Color
        {
            get => (Color) GetValue(Inset0ColorProperty);
            set => SetValue(Inset0ColorProperty, value);
        }

        public static readonly DependencyProperty Inset0ShowProperty =
            DependencyProperty.Register("Inset0Show", typeof(bool), typeof(CodeViewer), new PropertyMetadata(Inset0ShowChanged));

        private static void Inset0ShowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CodeViewer)d).OnInset0ShowChanged(d, e);
        }

        private void OnInset0ShowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var newValue = (bool)e.NewValue;
            ((CodeViewer)d).OnShowTypeChanged(newValue, EntityType.INSET0);
            _settingsManager.Inset0Show = (bool)e.NewValue;
        }

        public bool Inset0Show
        {
            get => (bool) GetValue(Inset0ShowProperty);
            set => SetValue(Inset0ShowProperty, value);
        }

        #endregion

        #region MICRO INFILL

        public static readonly DependencyProperty MicroInfillColorProperty =
            DependencyProperty.Register("MicroInfillColor", typeof(Color), typeof(CodeViewer),
                new PropertyMetadata(MicroInfillColorChangedCallback));

        private static void MicroInfillColorChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CodeViewer)d).OnMicroInfillColorChangedCallback(e);
        }

        private void OnMicroInfillColorChangedCallback(DependencyPropertyChangedEventArgs e)
        {
            OnColorChanged(EntityType.MICRO_INFILL_P, (Color) e.NewValue);
            _settingsManager.MicroInfillColor = (Color)e.NewValue;
        }

        public Color MicroInfillColor
        {
            get => (Color) GetValue(MicroInfillColorProperty);
            set => SetValue(MicroInfillColorProperty, value);
        }

        public static readonly DependencyProperty MicroInfillShowProperty =
            DependencyProperty.Register("MicroInfillShow", typeof(bool), typeof(CodeViewer), new PropertyMetadata(MicroInfillShowChanged));

        private static void MicroInfillShowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CodeViewer)d).OnMicroInfillShowChanged(d, e);
        }

        private void OnMicroInfillShowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var newValue = (bool)e.NewValue;
            ((CodeViewer)d).OnShowTypeChanged(newValue, EntityType.MICRO_INFILL_P);
            _settingsManager.MicroInfillShow = newValue;
        }

        public bool MicroInfillShow
        {
            get => (bool) GetValue(MicroInfillShowProperty);
            set => SetValue(MicroInfillShowProperty, value);
        }

        #endregion

        #region INSET XP

        public static readonly DependencyProperty InsetXPColorProperty =
            DependencyProperty.Register("InsetXPColor", typeof(Color), typeof(CodeViewer),
                new PropertyMetadata(InsetXPColorChangedCallback));

        private static void InsetXPColorChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CodeViewer) d).OnInsetXPColorChangedCallback(e);
        }

        private void OnInsetXPColorChangedCallback(DependencyPropertyChangedEventArgs e)
        {
            OnColorChanged(EntityType.INSET_XP, (Color) e.NewValue);
            _settingsManager.InsetXPColor = (Color)e.NewValue;
        }

        public Color InsetXPColor
        {
            get => (Color) GetValue(InsetXPColorProperty);
            set => SetValue(InsetXPColorProperty, value);
        }

        public static readonly DependencyProperty InsetXPShowProperty =
            DependencyProperty.Register("InsetXPShow", typeof(bool), typeof(CodeViewer), new PropertyMetadata(InsetXPShowChanged));

        private static void InsetXPShowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CodeViewer)d).OnInsetXPShowChanged(d, e);
        }

        private void OnInsetXPShowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var newValue = (bool) e.NewValue;
            ((CodeViewer) d).OnShowTypeChanged(newValue, EntityType.INSET_XP);
            _settingsManager.InsetXPShow = newValue;
        }

        public bool InsetXPShow
        {
            get => (bool) GetValue(InsetXPShowProperty);
            set => SetValue(InsetXPShowProperty, value);
        }

        #endregion

        #region INFILL SOLID

        public static readonly DependencyProperty InfillSolidColorProperty =
            DependencyProperty.Register("InfillSolidColor", typeof(Color), typeof(CodeViewer),
                new PropertyMetadata(InfillSolidColorChangedCallback));

        private static void InfillSolidColorChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CodeViewer) d).OnInfillSolidColorChangedCallback(e);
        }

        private void OnInfillSolidColorChangedCallback(DependencyPropertyChangedEventArgs e)
        {
            OnColorChanged(EntityType.INFILL_SOLID_P, (Color) e.NewValue);
            _settingsManager.InfillSolidColor = (Color)e.NewValue;
        }

        public Color InfillSolidColor
        {
            get => (Color) GetValue(InfillSolidColorProperty);
            set => SetValue(InfillSolidColorProperty, value);
        }

        public static readonly DependencyProperty InfillSolidShowProperty =
            DependencyProperty.Register("InfillSolidShow", typeof(bool), typeof(CodeViewer), new PropertyMetadata(InfillSolidShowChanged));

        private static void InfillSolidShowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CodeViewer)d).OnInfillSolidShowChanged(d, e);
        }

        private void OnInfillSolidShowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var newValue = (bool) e.NewValue;
            ((CodeViewer) d).OnShowTypeChanged(newValue, EntityType.INFILL_SOLID_P);
            _settingsManager.InfillSolidShow = newValue;
        }

        public bool InfillSolidShow
        {
            get => (bool) GetValue(InfillSolidShowProperty);
            set => SetValue(InfillSolidShowProperty, value);
        }

        #endregion

        #region INFILL CELLULAR 

        public static readonly DependencyProperty InfillCellularColorProperty =
            DependencyProperty.Register("InfillCellularColor", typeof(Color), typeof(CodeViewer),
                new PropertyMetadata(InfillCellularColorChangedCallback));

        private static void InfillCellularColorChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CodeViewer) d).OnInfillCellularColorChangedCallback(e);
        }

        private void OnInfillCellularColorChangedCallback(DependencyPropertyChangedEventArgs e)
        {
            OnColorChanged(EntityType.INFILL_CELLULAR_P, (Color) e.NewValue);
            _settingsManager.InfillCellularColor = (Color)e.NewValue;
        }

        public Color InfillCellularColor
        {
            get => (Color) GetValue(InfillCellularColorProperty);
            set => SetValue(InfillCellularColorProperty, value);
        }

        public static readonly DependencyProperty InfillCellularShowProperty =
            DependencyProperty.Register("InfillCellularShow", typeof(bool), typeof(CodeViewer),
                new PropertyMetadata(InfillCellularShowChanged));

        private static void InfillCellularShowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CodeViewer)d).OnInfillCellularShowChanged(d, e);
        }

        private void OnInfillCellularShowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var newValue = (bool) e.NewValue;
            ((CodeViewer) d).OnShowTypeChanged(newValue, EntityType.INFILL_CELLULAR_P);
            _settingsManager.InfillCellularShow = newValue;
        }

        public bool InfillCellularShow
        {
            get => (bool) GetValue(InfillCellularShowProperty);
            set => SetValue(InfillCellularShowProperty, value);
        }

        #endregion

        #region SUPPORT THIN

        public static readonly DependencyProperty SupportThinColorProperty =
            DependencyProperty.Register("SupportThinColor", typeof(Color), typeof(CodeViewer),
                new PropertyMetadata(SupportThinColorChangedCallback));

        private static void SupportThinColorChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CodeViewer) d).OnSupportThinColorChangedCallback(e);
        }

        private void OnSupportThinColorChangedCallback(DependencyPropertyChangedEventArgs e)
        {
            OnColorChanged(EntityType.SUPPORT_THIN, (Color) e.NewValue);
            _settingsManager.SupportThinColor = (Color)e.NewValue;
        }

        public Color SupportThinColor
        {
            get => (Color) GetValue(SupportThinColorProperty);
            set => SetValue(SupportThinColorProperty, value);
        }

        public static readonly DependencyProperty SupportThinShowProperty =
            DependencyProperty.Register("SupportThinShow", typeof(bool), typeof(CodeViewer), new PropertyMetadata(SupportThinShowChanged));

        private static void SupportThinShowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CodeViewer)d).OnSupportThinShowChanged(d, e);
        }

        private void OnSupportThinShowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var newValue = (bool) e.NewValue;
            ((CodeViewer) d).OnShowTypeChanged(newValue, EntityType.SUPPORT_THIN);
            _settingsManager.SupportThinShow = newValue;
        }

        public bool SupportThinShow
        {
            get => (bool) GetValue(SupportThinShowProperty);
            set => SetValue(SupportThinShowProperty, value);
        }

        #endregion

        #region SUPPORT THICK

        public static readonly DependencyProperty SupportThickColorProperty =
            DependencyProperty.Register("SupportThickColor", typeof(Color), typeof(CodeViewer),
                new PropertyMetadata(SupportThickColorChangedCallback));

        private static void SupportThickColorChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CodeViewer) d).OnSupportThickColorChangedCallback(e);
        }

        private void OnSupportThickColorChangedCallback(DependencyPropertyChangedEventArgs e)
        {
            OnColorChanged(EntityType.SUPPORT_THICK, (Color) e.NewValue);
            _settingsManager.SupportThickColor = (Color)e.NewValue;
        }

        public Color SupportThickColor
        {
            get => (Color) GetValue(SupportThickColorProperty);
            set => SetValue(SupportThickColorProperty, value);
        }

        public static readonly DependencyProperty SupportThickShowProperty =
            DependencyProperty.Register("SupportThickShow", typeof(bool), typeof(CodeViewer),
                new PropertyMetadata(SupportThickShowChanged));

        private static void SupportThickShowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CodeViewer)d).OnSupportThickShowChanged(d, e);
        }

        private void OnSupportThickShowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var newValue = (bool) e.NewValue;
            ((CodeViewer) d).OnShowTypeChanged(newValue, EntityType.SUPPORT_THICK);
            _settingsManager.SupportThickShow = newValue;
        }

        public bool SupportThickShow
        {
            get => (bool) GetValue(SupportThickShowProperty);
            set => SetValue(SupportThickShowProperty, value);
        }

        #endregion

        #region SKIRT

        public static readonly DependencyProperty SkirtColorProperty =
            DependencyProperty.Register("SkirtColor", typeof(Color), typeof(CodeViewer), new PropertyMetadata(SkirtColorChangedCallback));

        private static void SkirtColorChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CodeViewer) d).OnSkirtColorChangedCallback(e);
        }

        private void OnSkirtColorChangedCallback(DependencyPropertyChangedEventArgs e)
        {
            OnColorChanged(EntityType.SKIRT, (Color) e.NewValue);
            _settingsManager.SkirtColor = (Color)e.NewValue;
        }

        public Color SkirtColor
        {
            get => (Color) GetValue(SkirtColorProperty);
            set => SetValue(SkirtColorProperty, value);
        }

        public static readonly DependencyProperty SkirtShowProperty =
            DependencyProperty.Register("SkirtShow", typeof(bool), typeof(CodeViewer), new PropertyMetadata(SkirtShowChanged));

        private static void SkirtShowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CodeViewer)d).OnSkirtShowChanged(d, e);
        }

        private void OnSkirtShowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var newValue = (bool)e.NewValue;
            ((CodeViewer)d).OnShowTypeChanged(newValue, EntityType.SKIRT);
            _settingsManager.SkirtShow = newValue;
        }

        public bool SkirtShow
        {
            get => (bool) GetValue(SkirtShowProperty);
            set => SetValue(SkirtShowProperty, value);
        }

        #endregion

        #region BRIM

        public static readonly DependencyProperty BrimColorProperty =
            DependencyProperty.Register("BrimColor", typeof(Color), typeof(CodeViewer), new PropertyMetadata(BrimColorChangedCallback));

        private static void BrimColorChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CodeViewer) d).OnBrimColorChangedCallback(e);
        }

        private void OnBrimColorChangedCallback(DependencyPropertyChangedEventArgs e)
        {
            OnColorChanged(EntityType.BRIM, (Color) e.NewValue);
            _settingsManager.BrimColor = (Color)e.NewValue;
        }

        public Color BrimColor
        {
            get => (Color) GetValue(BrimColorProperty);
            set => SetValue(BrimColorProperty, value);
        }

        public static readonly DependencyProperty BrimShowProperty =
            DependencyProperty.Register("BrimShow", typeof(bool), typeof(CodeViewer), new PropertyMetadata(BrimShowChanged));

        private static void BrimShowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CodeViewer)d).OnBrimShowChanged(d, e);
        }

        private void OnBrimShowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var newValue = (bool) e.NewValue;
            ((CodeViewer) d).OnShowTypeChanged(newValue, EntityType.BRIM);
            _settingsManager.BrimShow = newValue;
        }

        public bool BrimShow
        {
            get => (bool) GetValue(BrimShowProperty);
            set => SetValue(BrimShowProperty, value);
        }

        #endregion

        #region WIPE

        public static readonly DependencyProperty WipeTowerColorProperty =
            DependencyProperty.Register("WipeTowerColor", typeof(Color), typeof(CodeViewer),
                new PropertyMetadata(WipeTowerColorChangedCallback));

        private static void WipeTowerColorChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CodeViewer) d).OnWipeTowerColorChangedCallback(e);
        }

        private void OnWipeTowerColorChangedCallback(DependencyPropertyChangedEventArgs e)
        {
            OnColorChanged(EntityType.WIPE_TOWER, (Color) e.NewValue);
            _settingsManager.WipeColor = (Color)e.NewValue;
        }

        public Color WipeTowerColor
        {
            get => (Color) GetValue(WipeTowerColorProperty);
            set => SetValue(WipeTowerColorProperty, value);
        }

        public static readonly DependencyProperty WipeTowerShowProperty =
            DependencyProperty.Register("WipeTowerShow", typeof(bool), typeof(CodeViewer), new PropertyMetadata(WipeTowerShowChanged));

        private static void WipeTowerShowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CodeViewer)d).OnWipeTowerShowChanged(d, e);
        }

        private void OnWipeTowerShowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var newValue = (bool) e.NewValue;
            ((CodeViewer) d).OnShowTypeChanged(newValue, EntityType.WIPE_TOWER);
            _settingsManager.WipeShow = newValue;
        }

        public bool WipeTowerShow
        {
            get => (bool) GetValue(WipeTowerShowProperty);
            set => SetValue(WipeTowerShowProperty, value);
        }

        #endregion

        #region INSET XF

        public static readonly DependencyProperty InsetXFColorProperty =
            DependencyProperty.Register("InsetXFColor", typeof(Color), typeof(CodeViewer),
                new PropertyMetadata(InsetXFColorChangedCallback));

        private static void InsetXFColorChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CodeViewer) d).OnInsetXFColorChangedCallback(e);
        }

        private void OnInsetXFColorChangedCallback(DependencyPropertyChangedEventArgs e)
        {
            OnColorChanged(EntityType.INSET_XF, (Color) e.NewValue);
            _settingsManager.InsetXFColor = (Color)e.NewValue;
        }

        public Color InsetXFColor
        {
            get => (Color) GetValue(InsetXFColorProperty);
            set => SetValue(InsetXFColorProperty, value);
        }

        public static readonly DependencyProperty InsetXFShowProperty =
            DependencyProperty.Register("InsetXFShow", typeof(bool), typeof(CodeViewer), new PropertyMetadata(InsetXFShowChanged));

        private static void InsetXFShowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CodeViewer)d).OnInsetXFShowChanged(d, e);
        }

        private void OnInsetXFShowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var newValue = (bool) e.NewValue;
            ((CodeViewer) d).OnShowTypeChanged(newValue, EntityType.INSET_XF);
            _settingsManager.InsetXFShow = newValue;
        }

        public bool InsetXFShow
        {
            get => (bool) GetValue(InsetXFShowProperty);
            set => SetValue(InsetXFShowProperty, value);
        }

        public static readonly DependencyProperty InsetXFEnableProperty =
            DependencyProperty.Register("InsetXFEnable", typeof(bool), typeof(CodeViewer), new PropertyMetadata(false));

        private bool InsetXFEnable
        {
            get => (bool) GetValue(InsetXFEnableProperty);
            set => SetValue(InsetXFEnableProperty, value);
        }

        #endregion

        #region INFILL FIBER

        public static readonly DependencyProperty FiberInfillColorProperty =
            DependencyProperty.Register("FiberInfillColor", typeof(Color), typeof(CodeViewer),
                new PropertyMetadata(FiberInfillColorChangedCallback));

        private static void FiberInfillColorChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CodeViewer)d).OnFiberInfillColorChangedCallback(e);
        }

        private void OnFiberInfillColorChangedCallback(DependencyPropertyChangedEventArgs e)
        {
            OnColorChanged(EntityType.INFILL_FIBER, (Color)e.NewValue);
            _settingsManager.FiberInfillColor = (Color)e.NewValue;
        }

        public Color FiberInfillColor
        {
            get => (Color)GetValue(FiberInfillColorProperty);
            set => SetValue(FiberInfillColorProperty, value);
        }

        public static readonly DependencyProperty FiberInfillShowProperty =
            DependencyProperty.Register("FiberInfillShow", typeof(bool), typeof(CodeViewer), new PropertyMetadata(FiberInfillShowChanged));

        private static void FiberInfillShowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CodeViewer)d).OnFiberInfillShowChanged(d, e);
        }

        private void OnFiberInfillShowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var newValue = (bool)e.NewValue;
            ((CodeViewer)d).OnShowTypeChanged(newValue, EntityType.INFILL_FIBER);
            _settingsManager.FiberInfillShow = newValue;
        }

        public bool FiberInfillShow
        {
            get => (bool)GetValue(FiberInfillShowProperty);
            set => SetValue(FiberInfillShowProperty, value);
        }

        public static readonly DependencyProperty FiberInfillEnableProperty =
            DependencyProperty.Register("FiberInfillEnable", typeof(bool), typeof(CodeViewer), new PropertyMetadata(false));

        private bool FiberInfillEnable
        {
            get => (bool)GetValue(FiberInfillEnableProperty);
            set => SetValue(FiberInfillEnableProperty, value);
        }

        #endregion

        //метод изменения отображения для типов блоков
        private void OnShowTypeChanged(bool newValue, BlockType type)
        {
            if (_blockLayers == null || !_blockLayers.Any()) return;
            //находим слой, в котором размещены соответсвующие сущности
            var layers = _blockLayers.Where(a => a.Key == type).Select(b => b.Value).ToList();
            foreach (var layer in layers)
            {
                layer.Visible = newValue;
            }

            _auraViewportLayout.Invalidate();
        }

        //метод изменения цвета для типа блоков
        private void OnColorChanged(BlockType type, Color newValue)
        {
            _blockColors[type] = newValue;

            switch (type)
            {
                //для этих типов есть своя сущность - линия, поэтому нужно перекрасить соответствующий слой
                case BlockType.TRAVEL:
                case BlockType.MOVE_P:
                case BlockType.MOVE_PF:
                    if (_blockLayers == null || !_blockLayers.Any() || _noBaseBlocks == null || !_noBaseBlocks.Any()) break;
                    var newColor = System.Drawing.Color.FromArgb(_blockColors[type].A, _blockColors[type].R, _blockColors[type].G, _blockColors[type].B);
                    var needNewColor = _noBaseBlocks.Where(a => a.Key.blockType == type).ToList();
                    needNewColor.ForEach(a => a.Value.Color = newColor);
                    //needNewColor.ForEach(a => a.Value.block.Entities.ForEach(b=>b.Color = newColor));
                    break;
                //для этих типа общий исходный объект, а ссылка BlockReference разная, с разной матрицей трансляции
                //поэтому перекрашиваем источник - меш
                case BlockType.EXTRUDE_P:
                case BlockType.EXTRUDE_PF:
                case BlockType.RETRACT:
                case BlockType.CUT:
                    if (_sourceBlocks == null || !_sourceBlocks.Any()) break;
                    foreach (var mesh in _sourceBlocks[type].sourceMeshes)
                    {
                        mesh.Color = System.Drawing.Color.FromArgb(_blockColors[type].A, _blockColors[type].R, _blockColors[type].G, _blockColors[type].B);
                    }

                    break;
            }

            _auraViewportLayout.Invalidate();
        }

        #region TRAVEL

        public static readonly DependencyProperty TravelColorProperty =
            DependencyProperty.Register("TravelColor", typeof(Color), typeof(CodeViewer),
                new PropertyMetadata(TravelsColorChangedCallback));

        private static void TravelsColorChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CodeViewer) d).OnTravelsColorChangedCallback(e);
        }

        private void OnTravelsColorChangedCallback(DependencyPropertyChangedEventArgs e)
        {
            OnColorChanged(BlockType.TRAVEL, (Color) e.NewValue);
            _settingsManager.TravelColor = (Color)e.NewValue;
        }

        public Color TravelColor
        {
            get => (Color) GetValue(TravelColorProperty);
            set => SetValue(TravelColorProperty, value);
        }

        public static readonly DependencyProperty TravelShowProperty =
            DependencyProperty.Register("TravelShow", typeof(bool), typeof(CodeViewer), new PropertyMetadata(TravelShowChanged));

        private static void TravelShowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CodeViewer)d).OnTravelShowChanged(d, e);
        }

        private void OnTravelShowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var newValue = (bool) e.NewValue;
            ((CodeViewer) d).OnShowTypeChanged(newValue, BlockType.TRAVEL);
            _settingsManager.TravelShow = newValue;
        }

        public bool TravelShow
        {
            get => (bool) GetValue(TravelShowProperty);
            set => SetValue(TravelShowProperty, value);
        }

        #endregion

        #region MOVE P

        public static readonly DependencyProperty MovePColorProperty =
            DependencyProperty.Register("MovePColor", typeof(Color), typeof(CodeViewer), new PropertyMetadata(MovePColorChangedCallback));

        private static void MovePColorChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CodeViewer) d).OnMovePColorChangedCallback(e);
        }

        private void OnMovePColorChangedCallback(DependencyPropertyChangedEventArgs e)
        {
            OnColorChanged(BlockType.MOVE_P, (Color) e.NewValue);
            _settingsManager.MovePColor = (Color)e.NewValue;
        }

        public Color MovePColor
        {
            get => (Color) GetValue(MovePColorProperty);
            set => SetValue(MovePColorProperty, value);
        }

        public static readonly DependencyProperty MovePShowProperty =
            DependencyProperty.Register("MovePShow", typeof(bool), typeof(CodeViewer), new PropertyMetadata(MovePShowChanged));

        private static void MovePShowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CodeViewer)d).OnMovePShowChanged(d, e);
        }

        private  void OnMovePShowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var newValue = (bool) e.NewValue;
            ((CodeViewer) d).OnShowTypeChanged(newValue, BlockType.MOVE_P);
            _settingsManager.MovePShow = newValue;;
        }

        public bool MovePShow
        {
            get => (bool) GetValue(MovePShowProperty);
            set => SetValue(MovePShowProperty, value);
        }

        #endregion

        #region MOVE PF

        public static readonly DependencyProperty MovePFColorProperty =
            DependencyProperty.Register("MovePFColor", typeof(Color), typeof(CodeViewer), new PropertyMetadata(MovePFColorChangedCallback));

        private static void MovePFColorChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CodeViewer)d).OnMovePFColorChangedCallback(e);
        }

        private void OnMovePFColorChangedCallback(DependencyPropertyChangedEventArgs e)
        {
            OnColorChanged(BlockType.MOVE_PF, (Color)e.NewValue);
            _settingsManager.MovePFColor = (Color)e.NewValue;
        }

        public Color MovePFColor
        {
            get => (Color) GetValue(MovePFColorProperty);
            set => SetValue(MovePFColorProperty, value);
        }

        public static readonly DependencyProperty MovePFShowProperty =
            DependencyProperty.Register("MovePFShow", typeof(bool), typeof(CodeViewer), new PropertyMetadata(MovePFShowChanged));

        private static void MovePFShowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CodeViewer)d).OnMovePFShowChanged(d, e);
        }

        private  void OnMovePFShowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var newValue = (bool) e.NewValue;
            ((CodeViewer) d).OnShowTypeChanged(newValue, BlockType.MOVE_PF);
            _settingsManager.MovePFShow = newValue;
        }

        public bool MovePFShow
        {
            get => (bool) GetValue(MovePFShowProperty);
            set => SetValue(MovePFShowProperty, value);
        }

        #endregion

        #region EXTRUDE P

        public static readonly DependencyProperty ExtrudePColorProperty =
            DependencyProperty.Register("ExtrudePColor", typeof(Color), typeof(CodeViewer),
                new PropertyMetadata(ExtrudePColorChangedCallback));

        private static void ExtrudePColorChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CodeViewer) d).OnExtrudePColorChangedCallback(e);
        }

        private void OnExtrudePColorChangedCallback(DependencyPropertyChangedEventArgs e)
        {
            OnColorChanged(BlockType.EXTRUDE_P, (Color) e.NewValue);
            _settingsManager.ExtrudePColor = (Color)e.NewValue;
        }

        public Color ExtrudePColor
        {
            get => (Color) GetValue(ExtrudePColorProperty);
            set => SetValue(ExtrudePColorProperty, value);
        }

        public static readonly DependencyProperty ExtrudePShowProperty =
            DependencyProperty.Register("ExtrudePShow", typeof(bool), typeof(CodeViewer), new PropertyMetadata(ExtrudePShowChanged));

        private static void ExtrudePShowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CodeViewer)d).OnExtrudePShowChanged(d, e);
        }

        private void OnExtrudePShowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var newValue = (bool) e.NewValue;
            ((CodeViewer) d).OnShowTypeChanged(newValue, BlockType.EXTRUDE_P);
            _settingsManager.ExtrudePShow = newValue;
        }

        public bool ExtrudePShow
        {
            get => (bool) GetValue(ExtrudePShowProperty);
            set => SetValue(ExtrudePShowProperty, value);
        }

        #endregion

        #region EXTRUDE PF

        public static readonly DependencyProperty ExtrudePFColorProperty =
            DependencyProperty.Register("ExtrudePFColor", typeof(Color), typeof(CodeViewer),
                new PropertyMetadata(ExtrudePFColorChangedCallback));

        private static void ExtrudePFColorChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CodeViewer) d).OnExtrudePFColorChangedCallback(e);
        }

        private void OnExtrudePFColorChangedCallback(DependencyPropertyChangedEventArgs e)
        {
            OnColorChanged(BlockType.EXTRUDE_PF, (Color) e.NewValue);
            _settingsManager.ExtrudePFColor = (Color)e.NewValue;
        }

        public Color ExtrudePFColor
        {
            get => (Color) GetValue(ExtrudePFColorProperty);
            set => SetValue(ExtrudePFColorProperty, value);
        }

        public static readonly DependencyProperty ExtrudePFShowProperty =
            DependencyProperty.Register("ExtrudePFShow", typeof(bool), typeof(CodeViewer), new PropertyMetadata(ExtrudePFShowChanged));

        private static void ExtrudePFShowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CodeViewer)d).OnExtrudePFShowChanged(d, e);
        }

        private void OnExtrudePFShowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var newValue = (bool) e.NewValue;
            ((CodeViewer) d).OnShowTypeChanged(newValue, BlockType.EXTRUDE_PF);
            _settingsManager.ExtrudePFShow = newValue;
        }

        public bool ExtrudePFShow
        {
            get => (bool) GetValue(ExtrudePFShowProperty);
            set => SetValue(ExtrudePFShowProperty, value);
        }

        #endregion

        #region RETRACT

        public static readonly DependencyProperty RetractColorProperty =
            DependencyProperty.Register("RetractColor", typeof(Color), typeof(CodeViewer),
                new PropertyMetadata(RetractColorChangedCallback));

        private static void RetractColorChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CodeViewer) d).OnRetractColorChangedCallback(e);
        }

        private void OnRetractColorChangedCallback(DependencyPropertyChangedEventArgs e)
        {
            OnColorChanged(BlockType.RETRACT, (Color) e.NewValue);
            _settingsManager.RetractColor = (Color)e.NewValue;
        }

        public Color RetractColor
        {
            get => (Color) GetValue(RetractColorProperty);
            set => SetValue(RetractColorProperty, value);
        }

        public static readonly DependencyProperty RetractShowProperty =
            DependencyProperty.Register("RetractShow", typeof(bool), typeof(CodeViewer), new PropertyMetadata(RetractShowChanged));

        private static void RetractShowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CodeViewer)d).OnRetractShowChanged(d, e);
        }

        private void OnRetractShowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var newValue = (bool) e.NewValue;
            ((CodeViewer) d).OnShowTypeChanged(newValue, BlockType.RETRACT);
            _settingsManager.RetractShow = newValue;
        }

        public bool RetractShow
        {
            get => (bool) GetValue(RetractShowProperty);
            set => SetValue(RetractShowProperty, value);
        }

        #endregion

        #region CUT

        public static readonly DependencyProperty CutColorProperty =
            DependencyProperty.Register("CutColor", typeof(Color), typeof(CodeViewer), new PropertyMetadata(CutColorChangedCallback));

        private static void CutColorChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CodeViewer) d).OnCutColorChangedCallback(e);
        }

        private void OnCutColorChangedCallback(DependencyPropertyChangedEventArgs e)
        {
            OnColorChanged(BlockType.CUT, (Color) e.NewValue);
            _settingsManager.CutColor = (Color)e.NewValue;
        }

        public Color CutColor
        {
            get => (Color) GetValue(CutColorProperty);
            set => SetValue(CutColorProperty, value);
        }

        public static readonly DependencyProperty CutShowProperty =
            DependencyProperty.Register("CutShow", typeof(bool), typeof(CodeViewer), new PropertyMetadata(CutShowChanged));

        private static void CutShowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CodeViewer)d).OnCutShowChanged(d, e);
        }

        private void OnCutShowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var newValue = (bool) e.NewValue;
            ((CodeViewer) d).OnShowTypeChanged(newValue, BlockType.CUT);
            _settingsManager.CutShow = newValue;
        }

        public bool CutShow
        {
            get => (bool) GetValue(CutShowProperty);
            set => SetValue(CutShowProperty, value);
        }

        #endregion
    }
}