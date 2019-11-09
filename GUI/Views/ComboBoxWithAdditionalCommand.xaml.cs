using System.Windows;
using Prism.Commands;

namespace Aura.Views
{
    /// <summary>
    /// Interaction logic for ComboBoxWithAdditionalCommand.xaml
    /// </summary>
    public partial class ComboBoxWithAdditionalCommand
    {
        public static readonly DependencyProperty AdditionalCommandProperty =
            DependencyProperty.Register("AdditionalCommand", typeof(DelegateCommandBase), typeof(ComboBoxWithAdditionalCommand));

        public DelegateCommandBase AdditionalCommand
        {
            get => (DelegateCommandBase) GetValue(AdditionalCommandProperty);
            set => SetValue(AdditionalCommandProperty, value);
        }

        public ComboBoxWithAdditionalCommand()
        {
            InitializeComponent();
        }
    }
}
