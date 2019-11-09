using System.Windows;
using System.Windows.Controls;
using Prism.Commands;

namespace Aura.Views
{
    /// <summary>
    /// Interaction logic for ExpanderExtended.xaml
    /// </summary>
    public partial class ExpanderExtended : Expander
    {
        public static readonly DependencyProperty RemoveCommandProperty =
            DependencyProperty.Register("RemoveCommand", typeof(DelegateCommand), typeof(ExpanderExtended));

        public DelegateCommand RemoveCommand
        {
            get => (DelegateCommand) GetValue(RemoveCommandProperty);
            set => SetValue(RemoveCommandProperty, value);
        }

        public static readonly DependencyProperty DuplicateCommandProperty =
            DependencyProperty.Register("DuplicateCommand", typeof(DelegateCommand), typeof(ExpanderExtended));

        public DelegateCommand DuplicateCommand
        {
            get => (DelegateCommand) GetValue(DuplicateCommandProperty);
            set => SetValue(DuplicateCommandProperty, value);
        }

        public static readonly DependencyProperty ExportCommandProperty =
            DependencyProperty.Register("ExportCommand", typeof(DelegateCommand), typeof(ExpanderExtended));

        public DelegateCommand ExportCommand
        {
            get => (DelegateCommand) GetValue(ExportCommandProperty);
            set => SetValue(ExportCommandProperty, value);
        }

        public ExpanderExtended()
        {
            InitializeComponent();
        }
    }
}
