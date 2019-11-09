using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Aura.Controls.Primitive
{
    public class RangeSlider : UserControl
    {
        private void UpdateSelectionBorder()
        {
            if (_selectionBorder == null || _thL == null || _thR == null) return;

            //  thL    selectionBorder          thR   
            // |--|----------------------------|--

            var sliderRange = Maximum - Minimum;
            //calcs left edge for left thumb on canvas
            var newLeftL = (StartValue - Minimum) / sliderRange * _slider.Width - _thL.Width + Canvas.GetLeft(_slider);
            Canvas.SetLeft(_thL, newLeftL);

            //calcs left edge for RIGHT thumb on canvas
            var newLeftR = (EndValue - Minimum) / sliderRange * _slider.Width + Canvas.GetLeft(_slider);
            Canvas.SetLeft(_thR, newLeftR);

            //calcs left edge for selection border on canvas
            Canvas.SetLeft(_selectionBorder, Canvas.GetLeft(_thL) + _thL.Width / 2);
            _selectionBorder.Width = Canvas.GetLeft(_thR) - Canvas.GetLeft(_thL) - _thL.Width / 2;
        }

        #region LOWER VALUE

        public static readonly DependencyProperty StartValueProperty =
            DependencyProperty.Register("StartValue", typeof(double), typeof(RangeSlider), new PropertyMetadata(0.0, RangeChangedCallback));

        private static void RangeChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RangeSlider)d).UpdateSelectionBorder();
        }

        /// <summary>
        /// Lower edge value on slider
        /// </summary>
        public double StartValue
        {
            get => (double) GetValue(StartValueProperty);
            set => SetValue(StartValueProperty, value);
        }

        #endregion

        #region UPPER VALUE

        public static readonly DependencyProperty EndValueProperty =
            DependencyProperty.Register("EndValue", typeof(double), typeof(RangeSlider), new PropertyMetadata(10.0, RangeChangedCallback));

        /// <summary>
        /// Upper edge value
        /// </summary>
        public double EndValue
        {
            get => (double)GetValue(EndValueProperty);
            set => SetValue(EndValueProperty, value);
        }

        #endregion

        #region MINIMUM

        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double), typeof(RangeSlider), new PropertyMetadata(0.0));

        public double Minimum
        {
            get => (double) GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        #endregion

        #region MAXIMUM

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(RangeSlider), new PropertyMetadata(10.0));

        public double Maximum
        {
            get => (double)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        #endregion

        #region THUMB STYLE

        public static readonly DependencyProperty ThumbStyleProperty =
            DependencyProperty.Register("ThumbStyle", typeof(Style), typeof(RangeSlider));

        public Style ThumbStyle
        {
            get { return (Style)GetValue(ThumbStyleProperty); }
            set { SetValue(ThumbStyleProperty, value); }
        }

        #endregion

        #region TICK FREQUENCY

        public static readonly DependencyProperty TickFrequencyProperty =
            DependencyProperty.Register("TickFrequency", typeof(double), typeof(RangeSlider), new PropertyMetadata(1.0));

        public double TickFrequency
        {
            get { return (double) GetValue(TickFrequencyProperty); }
            set { SetValue(TickFrequencyProperty, value); }
        }

        #endregion

        #region SELECTION COLOR

        public static readonly DependencyProperty SelectionColorBrushProperty = DependencyProperty.Register("SelectionColorBrush",
            typeof(SolidColorBrush), typeof(RangeSlider), new PropertyMetadata(new SolidColorBrush(Colors.White)));

        public SolidColorBrush SelectionColorBrush
        {
            get { return (SolidColorBrush) GetValue(SelectionColorBrushProperty); }
            set { SetValue(SelectionColorBrushProperty, value); }
        }

        #endregion

        private Border _selectionBorder;
        private Thumb _thL;
        private Thumb _thR;
        private Border _slider;

        public RangeSlider()
        {
            this.Loaded += (sender, args) => UpdateSelectionBorder();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _slider = GetTemplateChild("SliderRange") as Border;

            _selectionBorder = GetTemplateChild("BorderSelection") as Border;

            _thL = GetTemplateChild("ThumbLeft") as Thumb;
            _thR = GetTemplateChild("ThumbRight") as Thumb;

            if (_thL != null && _thR != null && _slider != null)
            {
                //sets left and top edges fro controls

                Canvas.SetLeft(_thL, 0);

                var leftStubSlider = GetTemplateChild("LeftStubSlider") as Border;
                Canvas.SetLeft(leftStubSlider, 0);
                Canvas.SetTop(leftStubSlider, _thL.Height / 2 - _slider.Height / 2);

                Canvas.SetLeft(_slider, _thL.Width);
                Canvas.SetTop(_slider, _thL.Height / 2 - _slider.Height / 2);
                _slider.Width = this.Width - _thL.Width - _thR.Width;

                var rightStubSlider = GetTemplateChild("RightStubSlider") as Border;
                Canvas.SetLeft(rightStubSlider, _thL.Width + _slider.Width);
                Canvas.SetTop(rightStubSlider, _thL.Height / 2 - _slider.Height / 2);

                Canvas.SetTop(_selectionBorder, _thL.Height / 2 - _slider.Height / 2);

                Canvas.SetLeft(_thR, _thL.Width + _slider.Width);

                UpdateSelectionBorder();

                var sliderRange = Maximum - Minimum;
                var valueMash = sliderRange / TickFrequency;
                var oneTickPixels = TickFrequency / sliderRange * _slider.Width;
                _thL.DragDelta += (sender, args) =>
                {
                    //by new mouse position calcs new StartValue in appropriate with ticks
                    var p = Mouse.GetPosition(_slider);
                    var mousePos = p.X;
                    double newStartValue;
                    if (mousePos <= oneTickPixels) newStartValue = Minimum;
                    else
                    {
                        var toTicks = mousePos / _slider.Width * valueMash;
                        var reminder = toTicks - Math.Truncate(toTicks);
                        newStartValue = reminder < 0.5 
                            ? toTicks - reminder 
                            : toTicks - reminder + 1;
                        if (newStartValue > EndValue - TickFrequency) newStartValue = EndValue - TickFrequency;
                    }
                    
                    var newLeft = (newStartValue - Minimum) / sliderRange * _slider.Width - _thL.Width + Canvas.GetLeft(_slider);
                    Canvas.SetLeft(_thL, newLeft);

                    StartValue = newStartValue;
                };
                _thR.DragDelta += (sender, args) =>
                {
                    //by new mouse position calcs new EndValue in appropriate with ticks
                    var p = Mouse.GetPosition(_slider);
                    var mousePos = p.X;
                    double newEndValue;
                    if (mousePos >= _slider.Width - oneTickPixels) newEndValue = Maximum;
                    else
                    {
                        var toTicks = mousePos / _slider.Width * valueMash;
                        var reminder = toTicks - Math.Truncate(toTicks);
                        newEndValue = reminder < 0.5 
                            ? toTicks - reminder 
                            : toTicks - reminder + 1;
                        if (newEndValue <= StartValue) newEndValue = StartValue + TickFrequency;
                    }
                    
                    var newLeft = (newEndValue - Minimum) / sliderRange * _slider.Width + Canvas.GetLeft(_slider);
                    Canvas.SetLeft(_thR, newLeft);

                    EndValue = newEndValue;
                };
            }
        }
    }
}
