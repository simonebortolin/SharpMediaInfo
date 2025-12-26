using System;
using System.Linq;
using System.Threading.Tasks;
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
            var parsedArgs = CommandLineParser.Parse(e.Args);

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

            // ✨ IPC: Start server FIRST (before checking for existing instance)
            var singleInstance = _serviceProvider.GetRequiredService<ISingleInstanceService>();
            
            // Check if we should send to existing master instance
            if (singleInstance.ShouldCheckSingleInstance(e.Args))
            {
                // Retry logic: try multiple times with increasing delay
                bool sentToMaster = false;
                for (int attempt = 0; attempt < 3; attempt++)
                {
                    if (attempt > 0)
                    {
                        await Task.Delay(200 * attempt); // 200ms, 400ms
                    }
                    
                    if (await singleInstance.TrySendToMasterAsync(parsedArgs.Files))
                    {
                        sentToMaster = true;
                        break;
                    }
                }
                
                if (sentToMaster)
                {
                    // Sent to master! Close this instance
                    Shutdown();
                    return;
                }
            }

            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();

            // ✨ Parse window position
            if (parsedArgs.WindowPosition.HasValue)
            {
                var (x, y) = parsedArgs.WindowPosition.Value;

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

                    // Ensure window is on screen
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

            // ✨ IPC: Start server BEFORE showing window
            // This ensures the server is ready before the window is visible
            await singleInstance.StartServerAsync(async receivedFiles =>
            {
                // ✅ ALWAYS run on UI thread
                await Dispatcher.InvokeAsync(async () =>
                {
                    // Bring window to foreground
                    if (mainWindow.WindowState == WindowState.Minimized)
                        mainWindow.WindowState = WindowState.Normal;
                    
                    mainWindow.Activate();
                    mainWindow.Topmost = true;  // Force to top
                    mainWindow.Topmost = false; // Remove topmost
                    mainWindow.Focus();

                    // Open received files (now on UI thread!)
                    if (receivedFiles.Length > 0)
                    {
                        var vm = _serviceProvider.GetRequiredService<MainViewModel>();
                        await vm.AddFilesFromDropAsync(receivedFiles);
                    }
                });
            });

            // Now show the window
            mainWindow.Show();

            // Open files from parsed arguments
            if (parsedArgs.Files.Length > 0)
            {
                var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();

                // Use Dispatcher to ensure UI is ready
                await Dispatcher.InvokeAsync(async () =>
                {
                    await mainViewModel.AddFilesFromDropAsync(parsedArgs.Files);
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
        services.AddSingleton<ISingleInstanceService, SingleInstanceService>();
        services.AddSingleton<IContextMenuService, WpfContextMenuService>();

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
                sp,
                cfg);
        });
        
        services.AddSingleton<SettingsTabViewModel>();
        
        services.AddSingleton<AboutTabViewModel>();

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

    protected override async void OnExit(ExitEventArgs e)
    {
        var singleInstance = _serviceProvider?.GetService<ISingleInstanceService>();
        if (singleInstance != null)
        {
            await singleInstance.StopServerAsync();
        }

        _serviceProvider?.Dispose();
        base.OnExit(e);
    }
}
