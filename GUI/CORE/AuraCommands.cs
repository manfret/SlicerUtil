using System.Windows.Input;

namespace Aura.CORE
{
    internal static class AuraCommands
    {
        internal static RoutedUICommand OpenAbout { get; }

        static AuraCommands()
        {
            OpenAbout = new RoutedUICommand("Open about", "OpenABout", typeof(MainWindow));
        }
    }
}