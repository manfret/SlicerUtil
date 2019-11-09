using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Aura.Themes.Localization;
using Aura.ViewModels;
using Prism.Commands;
using DelegateCommand = Prism.Commands.DelegateCommand;

namespace Aura.Views
{
    /// <summary>
    /// Interaction logic for CancellationButton.xaml
    /// </summary>
    public partial class StartCancelFinishButton
    {
        #region MAIN COLOR BRUSH

        public static readonly DependencyProperty MainColorBrushProperty = DependencyProperty.Register(
            "MainColorBrush", typeof(Brush), typeof(StartCancelFinishButton),
            new PropertyMetadata(new SolidColorBrush(new Color {A = 255, R = 125, G = 125, B = 125}), DisabledColorBrushChangedCallback));

        public Brush MainColorBrush
        {
            get => (Brush) GetValue(MainColorBrushProperty);
            set
            {
                SetValue(MainColorBrushProperty, value);
                FinalColorBrush = IsInteractionEnabled ? MainColorBrush : DisabledColorBrush;
            }
        }

        #endregion

        #region DISABLED COLOR BRUSH

        public static readonly DependencyProperty DisabledColorBrushProperty = DependencyProperty.Register(
            "DisabledColorBrush", typeof(Brush), typeof(StartCancelFinishButton),
            new PropertyMetadata(new SolidColorBrush(new Color {A = 255, R = 200, G = 200, B = 200}), DisabledColorBrushChangedCallback));

        private static void DisabledColorBrushChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((StartCancelFinishButton)d).OnDisabledColorBrushChangedCallback(d, e);
        }

        private void OnDisabledColorBrushChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FinalColorBrush = IsInteractionEnabled ? MainColorBrush : DisabledColorBrush;
        }

        public Brush DisabledColorBrush
        {
            get => (Brush) GetValue(DisabledColorBrushProperty);
            set => SetValue(DisabledColorBrushProperty, value);
        }

        #endregion

        #region HIGHLIGHT BRUSH

        public static readonly DependencyProperty HighlightBrushProperty = DependencyProperty.Register(
            "HighlightBrush", typeof(Brush), typeof(StartCancelFinishButton),
            new PropertyMetadata(new SolidColorBrush(new Color {A = 255, R = 125, G = 125, B = 125})));

        public Brush HighlightBrush
        {
            get => (Brush) GetValue(HighlightBrushProperty);
            set => SetValue(HighlightBrushProperty, value);
        }

        #endregion

        #region FINAL COLOR BRUSH

        public static readonly DependencyProperty FinalColorBrushProperty =
            DependencyProperty.Register("FinalColorBrush", typeof(Brush), typeof(StartCancelFinishButton));

        public Brush FinalColorBrush
        {
            get => (Brush) GetValue(FinalColorBrushProperty);
            set => SetValue(FinalColorBrushProperty, value);
        }

        #endregion

        #region START TEXT

        public static readonly DependencyProperty StartTextProperty = DependencyProperty.Register(
            "StartText", typeof(string), typeof(StartCancelFinishButton), new PropertyMetadata(null, TextChangedCallback));

        private static void TextChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((StartCancelFinishButton)d).OnTextChangedCallback(d, e);
        }

        private void OnTextChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FinalText = GetFinalString();
        }

        public string StartText
        {
            get => (string) GetValue(StartTextProperty);
            set => SetValue(StartTextProperty, value);
    }

        #endregion

        #region CANCEL TEXT

        public static readonly DependencyProperty CancelTextProperty = DependencyProperty.Register(
            "CancelText", typeof(string), typeof(StartCancelFinishButton), new PropertyMetadata(null, TextChangedCallback));

        public string CancelText
        {
            get { return (string) GetValue(CancelTextProperty); }
            set { SetValue(CancelTextProperty, value); }
        }

        #endregion

        #region FINISH TEXT

        public static readonly DependencyProperty FinishTextProperty = DependencyProperty.Register(
            "FinishText", typeof(string), typeof(StartCancelFinishButton), new PropertyMetadata(null, TextChangedCallback));

        public string FinishText
        {
            get => (string) GetValue(FinishTextProperty);
            set => SetValue(FinishTextProperty, value);
        }

        #endregion

        #region FINAL TEXT

        public static readonly DependencyProperty FinalTextProperty =
            DependencyProperty.Register("FinalText", typeof(string), typeof(StartCancelFinishButton));

        public string FinalText
        {
            get => (string) GetValue(FinalTextProperty);
            set => SetValue(FinalTextProperty, value);
        }

        private string GetFinalString()
        {
            switch (CurrentState)
            {
                case State.BASE:
                    return StartText;
                case State.PROCESS:
                    return CancelText;
                case State.FINISH:
                    return FinishText;
            }

            return null;
        }

        #endregion

        #region PROGRESS PERCENTAGE

        public static readonly DependencyProperty ProgressPercentageProperty =
            DependencyProperty.Register("ProgressPercentage", typeof(int), typeof(StartCancelFinishButton));

        public int ProgressPercentage
        {
            get => (int) GetValue(ProgressPercentageProperty);
            set => SetValue(ProgressPercentageProperty, value);
        }

        #endregion

        #region START COMMAND

        public static readonly DependencyProperty StartCommandProperty =
            DependencyProperty.Register("StartCommand", typeof(DelegateCommand), typeof(StartCancelFinishButton));

        public DelegateCommand StartCommand
        {
            get => (DelegateCommand) GetValue(StartCommandProperty);
            set => SetValue(StartCommandProperty, value);
        }

        #endregion

        #region CANCEL COMMAND

        public static readonly DependencyProperty CancelCommandProperty =
            DependencyProperty.Register("CancelCommand", typeof(DelegateCommand), typeof(StartCancelFinishButton));

        public DelegateCommand CancelCommand
        {
            get => (DelegateCommand) GetValue(CancelCommandProperty);
            set => SetValue(CancelCommandProperty, value);
        }

        #endregion

        #region FINISH COMMAND

        public static readonly DependencyProperty FinishCommandProperty =
            DependencyProperty.Register("FinishCommand", typeof(DelegateCommand), typeof(StartCancelFinishButton));

        public DelegateCommand FinishCommand
        {
            get => (DelegateCommand) GetValue(FinishCommandProperty);
            set => SetValue(FinishCommandProperty, value);
        }

        #endregion

        #region CLICK COMMAND

        public static readonly DependencyProperty ClickCommandProperty =
            DependencyProperty.Register("ClickCommand", typeof(DelegateCommand), typeof(StartCancelFinishButton));

        public DelegateCommand ClickCommand
        {
            get => (DelegateCommand)GetValue(ClickCommandProperty);
            private set => SetValue(ClickCommandProperty, value);
        }

        #endregion

        #region IS ENABLED

        public static readonly DependencyProperty IsInteractionEnabledProperty =
            DependencyProperty.Register("IsInteractionEnabled", typeof(bool), typeof(StartCancelFinishButton), new PropertyMetadata(false, IsClickEnabledChangedCallback));

        private static void IsClickEnabledChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((StartCancelFinishButton)d).OnIsInteractionEnabledChangedCallback(d, e);
        }

        private static readonly ToolTip _disabledGenerationTooltip = new ToolTip {Content = Common_en_EN.GenerationDisabledTooltip};

        private void OnIsInteractionEnabledChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IsEnabled = IsInteractionEnabled;
            FinalColorBrush = IsInteractionEnabled ? MainColorBrush : DisabledColorBrush;
            ClickCommand?.RaiseCanExecuteChanged();
        }

        public bool IsInteractionEnabled
        {
            get => (bool) GetValue(IsInteractionEnabledProperty);
            set => SetValue(IsInteractionEnabledProperty, value);
        }

        #endregion

        #region SEMAPHOR

        public static readonly DependencyProperty StatusSemaphorProperty = DependencyProperty.Register("StatusSemaphor",
            typeof(SessionVM.StatusSemaphor), typeof(StartCancelFinishButton), new PropertyMetadata(null, NeedUpdateProgressChangedCallback));

        private static void NeedUpdateProgressChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((StartCancelFinishButton)d).OnNeedUpdateProgressChangedCallback(d, e);
        }

        private void OnNeedUpdateProgressChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (StatusSemaphor == null) return;
            StatusSemaphor.ToBase += (sender, args) => CurrentState = State.BASE;
            StatusSemaphor.ToFinish += (sender, args) => CurrentState = State.FINISH;
        }

        public SessionVM.StatusSemaphor StatusSemaphor
        {
            get => (SessionVM.StatusSemaphor) GetValue(StatusSemaphorProperty);
            set => SetValue(StatusSemaphorProperty, value);
        }

        #endregion

        public StartCancelFinishButton()
        {
            InitializeComponent();
            IsEnabled = false;
            ClickCommand = new DelegateCommand(ClickExecuteMethod, () => IsInteractionEnabled);
            CurrentState = _currentState;
        }

        #region CURRENT STATE

        private static State _currentState = State.BASE;
        private State CurrentState
        {
            get => _currentState;
            set
            {
                _currentState = value;
                FinalText = GetFinalString();
            }
        }

        #endregion

        private void ClickExecuteMethod()
        {
            switch (CurrentState)
            {
                case State.BASE:
                    CurrentState = State.PROCESS;
                    StartCommand.Execute();
                    break;
                case State.PROCESS:
                    CurrentState = State.BASE;
                    //ProgressPercentage = 0;
                    CancelCommand.Execute();
                    break;
                case State.FINISH:
                    FinishCommand.Execute();
                    break;
            }
        }

        private enum State
        {
            BASE,
            PROCESS,
            FINISH
        }
    }

    public class LastPercentageToGridLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var percent = (int) value;
            return new GridLength(100 - percent, GridUnitType.Star);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DigitToColumnLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new GridLength(int.Parse(value.ToString()), GridUnitType.Star);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}