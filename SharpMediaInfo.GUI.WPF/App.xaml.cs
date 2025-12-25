using System;
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
            mainWindow.Show();
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
