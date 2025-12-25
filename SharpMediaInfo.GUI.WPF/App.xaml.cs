using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using SharpMediaInfo.GUI.Services;
using SharpMediaInfo.GUI.ViewModels;
using SharpMediaInfo.GUI.WPF.Services;
using SharpMediaInfo.GUI.WPF.Windows;
using System.Windows;
using SharpMediaInfo.GUI.Models;

namespace SharpMediaInfo.GUI.WPF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private ServiceProvider? _serviceProvider;

    public IServiceProvider Services => _serviceProvider!;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        try
        {
            var services = new ServiceCollection();
            
            // Load configuration FIRST
            var tempProvider = services.BuildServiceProvider();
            var cfgService = new ConfigurationService(new WpfFileService());
            var cfg = await cfgService.LoadConfigurationAsync();
            cfgService.Attach(cfg);
            
            // Register the loaded configuration as singleton
            services.AddSingleton(cfg);
            
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();

            ApplyTheme(cfg.AppTheme);

            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            
            // Parse command line arguments for window position
            var args = e.Args;
            if (args.Length >= 2 && args[0] == "--window-position")
            {
                // Format: --window-position X,Y "filepath"
                var positionArg = args[1];
                var coords = positionArg.Split(',');
                
                if (coords.Length == 2 && 
                    int.TryParse(coords[0], out int x) && 
                    int.TryParse(coords[1], out int y))
                {
                    // Position window AFTER it's loaded so we have correct dimensions
                    RoutedEventHandler? loadedHandler = null;
                    loadedHandler = (s, loadedArgs) =>
                    {
                        // Get DPI scaling factor for accurate positioning
                        var source = PresentationSource.FromVisual(mainWindow);
                        double dpiScaleX = 1.0;
                        double dpiScaleY = 1.0;
                        
                        if (source?.CompositionTarget != null)
                        {
                            dpiScaleX = source.CompositionTarget.TransformToDevice.M11;
                            dpiScaleY = source.CompositionTarget.TransformToDevice.M22;
                        }
                        
                        // Convert physical pixels to WPF device-independent units
                        double mouseXInDiu = x / dpiScaleX;
                        double mouseYInDiu = y / dpiScaleY;
                        
                        // Position window with cursor near top-left corner (title bar area)
                        mainWindow.Left = mouseXInDiu - 20;
                        mainWindow.Top = mouseYInDiu - 10;
                        
                        // Ensure window is on screen (using SystemParameters for primary screen bounds)
                        // Note: This uses primary screen. For multi-monitor, we'd need Win32 MonitorFromPoint
                        double screenLeft = SystemParameters.VirtualScreenLeft;
                        double screenTop = SystemParameters.VirtualScreenTop;
                        double screenRight = SystemParameters.VirtualScreenLeft + SystemParameters.VirtualScreenWidth;
                        double screenBottom = SystemParameters.VirtualScreenTop + SystemParameters.VirtualScreenHeight;
                        
                        if (mainWindow.Left < screenLeft) 
                            mainWindow.Left = screenLeft;
                        if (mainWindow.Top < screenTop) 
                            mainWindow.Top = screenTop;
                        if (mainWindow.Left + mainWindow.ActualWidth > screenRight)
                            mainWindow.Left = screenRight - mainWindow.ActualWidth;
                        if (mainWindow.Top + mainWindow.ActualHeight > screenBottom)
                            mainWindow.Top = screenBottom - mainWindow.ActualHeight;
                        
                        // Remove handler after first use to prevent memory leak
                        mainWindow.Loaded -= loadedHandler;
                    };
                    
                    mainWindow.Loaded += loadedHandler;
                }
            }
            
            mainWindow.Show();

            // Handle command line arguments (files to open)
            var filesToOpen = args
                .Where(arg => !arg.StartsWith("--") && System.IO.File.Exists(arg))
                .ToArray();

            if (filesToOpen.Length > 0)
            {
                var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
                
                // Use Dispatcher to ensure UI is ready
                await Dispatcher.InvokeAsync(async () =>
                {
                    await mainViewModel.AddFilesFromDropAsync(filesToOpen);
                });
            }
        }
        catch (System.Exception ex)
        {
            MessageBox.Show($"Error during startup: {ex.Message}\n\n{ex.StackTrace}", 
                "Startup Error", 
                MessageBoxButton.OK, 
                MessageBoxImage.Error);
            Shutdown();
        }
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // Register services
        services.AddSingleton<IFileService, WpfFileService>();
        services.AddSingleton<IDialogService, WpfDialogService>();
        services.AddSingleton<IClipboardService, WpfClipboardService>();
        services.AddSingleton<IConfigurationService, ConfigurationService>();
        services.AddSingleton<IRecentFilesService, RecentFilesService>();
        services.AddSingleton<IMousePositionService, WpfMousePositionService>();

        // Register ViewModels as factory so we can pass configuration
        services.AddSingleton<MainViewModel>(sp =>
        {
            // This will be called AFTER OnStartup has loaded configuration
            var cfgService = sp.GetRequiredService<IConfigurationService>();
            var cfg = sp.GetRequiredService<AppConfiguration>();
            return new MainViewModel(
                sp.GetRequiredService<IFileService>(),
                sp.GetRequiredService<IDialogService>(),
                sp.GetRequiredService<IClipboardService>(),
                cfgService,
                sp.GetRequiredService<IRecentFilesService>(),
                sp.GetRequiredService<IMousePositionService>(),
                cfg);
        });
        
        services.AddSingleton<SettingsTabViewModel>(sp =>
        {
            var cfgService = sp.GetRequiredService<IConfigurationService>();
            var cfg = sp.GetRequiredService<AppConfiguration>();
            return new SettingsTabViewModel(cfgService, cfg);
        });
        
        services.AddSingleton<AboutViewModel>();

        // Register Views
        services.AddTransient<MainWindow>(sp => 
            new MainWindow(sp.GetRequiredService<MainViewModel>()));
    }

    private void ApplyTheme(AppTheme appTheme)
    {
        ThemeMode mode = ThemeMode.System;
        if (appTheme == AppTheme.Light) mode = ThemeMode.Light;
        else if (appTheme == AppTheme.Dark) mode = ThemeMode.Dark;

        Application.Current.ThemeMode = mode;
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider?.Dispose();
        base.OnExit(e);
    }
}
