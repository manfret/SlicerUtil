using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace Aura.Managers
{
    public interface IDialogManager
    {
        (bool? res, string fileName) OpenSaveDialog(string filter,
            string initialDirectory,
            bool overridePrompt,
            bool createPrompt,
            bool addExtension,
            string fileName = null);
    }

    public class DialogManager : IDialogManager
    {
        public (bool? res, string fileName) OpenSaveDialog(string filter, string initialDirectory, bool overridePrompt, bool createPrompt, bool addExtension,
            string fileName = null)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = filter,
                InitialDirectory = initialDirectory,
                OverwritePrompt = overridePrompt,
                CreatePrompt = createPrompt,
                AddExtension = addExtension,
            };
            if (fileName != null) saveFileDialog.FileName = fileName;
            return (saveFileDialog.ShowDialog(), saveFileDialog.FileName);
        }
    }
}