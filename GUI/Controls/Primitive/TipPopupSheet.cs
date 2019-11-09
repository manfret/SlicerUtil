using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Aura.Controls.Primitive
{
    public class TipPopupSheet : Control
    {
        #region LABEL TEXT

        public static DependencyProperty LabelTextProperty =
            DependencyProperty.Register(
                "LabelText",
                typeof(string),
                typeof(TipPopupSheet));

        public string LabelText
        {
            get { return (string) GetValue(LabelTextProperty); }
            set { SetValue(LabelTextProperty, value); }
        }

        #endregion

        #region IMAGE SOURCE

        public static DependencyProperty ImageSourceProperty =
            DependencyProperty.Register(
                "ImageSource",
                typeof(ImageSource),
                typeof(TipPopupSheet));

        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value);}
        }

        #endregion

        #region DESCRIPTION

        public static DependencyProperty DescriptionProperty =
            DependencyProperty.Register(
                "Description",
                typeof(string),
                typeof(TipPopupSheet));

        public string Description
        {
            get { return (string) GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        #endregion

        static TipPopupSheet()
        {
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(TipPopupSheet), new FrameworkPropertyMetadata(typeof(TipPopupSheet)));
        }
    }
}