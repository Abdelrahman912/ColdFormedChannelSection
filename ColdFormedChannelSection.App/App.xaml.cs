using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace ColdFormedChannelSection.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var modules = Process.GetCurrentProcess().Modules;
            RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
        }
    }
}
