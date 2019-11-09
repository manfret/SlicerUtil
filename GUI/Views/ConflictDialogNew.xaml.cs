using System.Windows;
using Aura.ViewModels;

namespace Aura.Views
{
    /// <summary>
    /// Interaction logic for ConflictDialogNew.xaml
    /// </summary>
    public partial class ConflictDialogNew
    {
        public ConflictDialogNew()
        {
            InitializeComponent();
        }

        private ConflictCollectionVM _vm;
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property.Equals(DataContextProperty) && e.NewValue != null && e.NewValue is ConflictCollectionVM vm)
            {
                _vm = vm;
                _vm.RequestCloseDialog += (sender, args) =>
                {
                    this.DialogResult = args.DialogResult;
                    this.Close();
                };
            }
        }
    }

}
