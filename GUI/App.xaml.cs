using System;
using System.IO;
using System.Windows;

namespace Aura
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void AppStartupMainMAGI(object sender, StartupEventArgs e)
        {
            var arguments = Environment.GetCommandLineArgs();

            if (arguments.GetLength(0) > 1)
            {
                if (arguments[1].EndsWith(".auproj"))
                {
                    Aura.MainWindow.InputProject = arguments[1];
                }
                else
                {
                    var extension = Path.GetExtension(arguments[1].ToLower());
                    switch (extension)
                    {
                        case ".stl":
                        case ".step":
                        case ".stp":
                        case ".3ds":
                        case ".obj":
                            Aura.MainWindow.InputModel = arguments[1];
                            break;
                    }
                }
            }
            else
            {
                // Call the view "welcome page application"
            }
        }
    }
}
