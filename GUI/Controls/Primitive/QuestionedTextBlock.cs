using System;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace Aura.Controls.Primitive
{
    public class QuestionedTextBlock : Control
    {
        #region CAPTION PROPERTY

        public static DependencyProperty CaptionProperty =
            DependencyProperty.Register(
                "Caption",
                typeof(string),
                typeof(QuestionedTextBlock));

        public string Caption
        {
            get { return (string)GetValue(CaptionProperty); }
            set { SetValue(CaptionProperty, value); }
        }

        #endregion

        #region CAPTION STYLE PROPERTY

        public static DependencyProperty CaptionStyleProperty =
            DependencyProperty.Register(
                "CaptionStyle",
                typeof(Style),
                typeof(QuestionedTextBlock));

        public Style CaptionStyle
        {
            get { return (Style)GetValue(CaptionStyleProperty); }
            set { SetValue(CaptionStyleProperty, value); }
        }

        #endregion

        #region TIP LABEL TEXT

        public static DependencyProperty TipLabelTextProperty =
            DependencyProperty.Register(
                "TipLabelText",
                typeof(string),
                typeof(QuestionedTextBlock));

        public string TipLabelText
        {
            get => (string)GetValue(TipLabelTextProperty);
            set => SetValue(TipLabelTextProperty, value);
        }

        #endregion

        #region TIP IMAGE SOURCE

        public static DependencyProperty TipImageBitmapProperty =
            DependencyProperty.Register(
                "TipImageBitmap",
                typeof(Bitmap),
                typeof(QuestionedTextBlock));

        public Bitmap TipImageBitmap
        {
            get => (Bitmap)GetValue(TipImageBitmapProperty);
            set => SetValue(TipImageBitmapProperty, value);
        }

        #endregion

        #region TIP DESCRIPTION

        public static DependencyProperty TipDescriptionProperty =
            DependencyProperty.Register(
                "TipDescription",
                typeof(string),
                typeof(QuestionedTextBlock));

        public string TipDescription
        {
            get => (string)GetValue(TipDescriptionProperty);
            set => SetValue(TipDescriptionProperty, value);
        }

        #endregion

        private TipPopupSheet _tipPopupSheet;

        static QuestionedTextBlock()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(QuestionedTextBlock), new FrameworkPropertyMetadata(typeof(QuestionedTextBlock)));
        }

        private Button _questionButtonWithTip;
        private Popup _popup;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _questionButtonWithTip = GetTemplateChild("PART_QuestionButton") as Button;
            if (_questionButtonWithTip != null)
            {
                if (TipDescription == null && TipLabelText == null && TipImageBitmap == null) _questionButtonWithTip.Width = 0;
                else
                {
                    var tipPoputSheet = new TipPopupSheet();
                    if (TipLabelText != null && TipLabelText.Any()) tipPoputSheet.LabelText = TipLabelText;
                    if (TipImageBitmap != null)
                    {
                        tipPoputSheet.ImageSource = Imaging.CreateBitmapSourceFromHBitmap(TipImageBitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    }
                    if (TipDescription != null && TipDescription.Any()) tipPoputSheet.Description = TipDescription;

                    _popup = new Popup
                    {
                        Child = tipPoputSheet,
                        PlacementTarget = this,
                        VerticalOffset = -15,
                        Placement = PlacementMode.Left,
                        PopupAnimation = PopupAnimation.Fade,
                        AllowsTransparency = true,
                        StaysOpen = true,
                    };
                    _questionButtonWithTip.Click += (sender, args) => _popup.IsOpen = !_popup.IsOpen;
                }
            }
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            if (_questionButtonWithTip != null) _questionButtonWithTip.Visibility = Visibility.Visible;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (_questionButtonWithTip != null)  _questionButtonWithTip.Visibility = Visibility.Hidden;
            if (_popup != null) _popup.IsOpen = false;
        }
    }
}