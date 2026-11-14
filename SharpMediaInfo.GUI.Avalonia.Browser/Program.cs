using Avalonia;
using Avalonia.Browser;
using System.Threading.Tasks;

namespace SharpMediaInfo.GUI.Avalonia.Browser;

internal sealed class Program
{
    // Renders into <div id="out"> of wwwroot/index.html.
    private static Task Main(string[] args) => BuildAvaloniaApp()
        .WithInterFont()
        .StartBrowserAppAsync("out");

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>();
}
