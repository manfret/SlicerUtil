using System;
using System.Windows.Forms;

namespace Aura.ViewModels
{
    public interface IDialogResultVMHelper
    {
        event EventHandler<RequestCloseDialogEventArgs> RequestCloseDialog;
    }

    public class RequestCloseDialogEventArgs : EventArgs
    {
        public bool DialogResult { get; set; }
        public RequestCloseDialogEventArgs(bool dialogresult)
        {
            this.DialogResult = dialogresult;
        }
    }
}