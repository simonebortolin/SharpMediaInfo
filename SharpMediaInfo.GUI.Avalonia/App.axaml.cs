using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using SharpMediaInfo.GUI.Avalonia.Services;
using SharpMediaInfo.GUI.Avalonia.Windows;
using SharpMediaInfo.GUI.Models;
using SharpMediaInfo.GUI.Services;
using SharpMediaInfo.GUI.ViewModels;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace SharpMediaInfo.GUI.Avalonia;

public partial class App : Application
{
    private ServiceProvider? _serviceProvider;

    public override async void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            try
            {
                var args = desktop.Args ?? Array.Empty<string>();
                var parsedArgs = CommandLineParser.Parse(args);

                var services = new ServiceCollection();

                // Load configuration FIRST
                var tempFileService = new AvaloniaFileService();
                var cfgService = new ConfigurationService(tempFileService);
                var cfg = await cfgService.LoadConfigurationAsync();
                cfgService.Attach(cfg);

                // Register the loaded configuration as singleton
                services.AddSingleton(cfg);

                ConfigureServices(services);
                _serviceProvider = services.BuildServiceProvider();

                // Apply initial theme
                ApplyTheme(cfg.AppTheme);
                
                // Subscribe to theme changes
                cfg.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(AppConfiguration.AppTheme))
                    {
                        ApplyTheme(cfg.AppTheme);
                    }
                };

                // Single instance check
                var singleInstance = _serviceProvider.GetRequiredService<ISingleInstanceService>();
                
                if (singleInstance.ShouldCheckSingleInstance(args))
                {
                    bool sentToMaster = false;
                    for (int attempt = 0; attempt < 3; attempt++)
                    {
                        if (attempt > 0)
                        {
                            await Task.Delay(200 * attempt);
                        }
                        
                        if (await singleInstance.TrySendToMasterAsync(parsedArgs.Files))
                        {
                            sentToMaster = true;
                            break;
                        }
                    }
                    
                    if (sentToMaster)
                    {
                        desktop.Shutdown();
                        return;
                    }
                }

                var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();

                // Window positioning (simplified - no mouse tracking in v1)
                if (parsedArgs.WindowPosition.HasValue)
                {
                    var (x, y) = parsedArgs.WindowPosition.Value;
                    mainWindow.Position = new PixelPoint(x, y);
                }

                // Start IPC server BEFORE showing window
                await singleInstance.StartServerAsync(async receivedFiles =>
                {
                    await Dispatcher.UIThread.InvokeAsync(async () =>
                    {
                        mainWindow.Activate();
                        
                        if (receivedFiles.Length > 0)
                        {
                            var vm = _serviceProvider.GetRequiredService<MainViewModel>();
                            await vm.AddFilesFromDropAsync(receivedFiles);
                        }
                    });
                });

                desktop.MainWindow = mainWindow;
                mainWindow.Show();

                // Open files from parsed arguments
                if (parsedArgs.Files.Length > 0)
                {
                    var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();

                    await Dispatcher.UIThread.InvokeAsync(async () =>
                    {
                        await mainViewModel.AddFilesFromDropAsync(parsedArgs.Files);
                    });
                }
            }
            catch (System.Exception ex)
            {
                // Show error dialog
                var msgBox = MessageBoxManager
                    .GetMessageBoxStandard("Startup Error",
                        $"Error during startup: {ex.Message}\n\n{ex.StackTrace}",
                        ButtonEnum.Ok,
                        Icon.Error);
                await msgBox.ShowAsync();
                desktop.Shutdown();
            }
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // Register platform-specific services
        services.AddSingleton<IFileService, AvaloniaFileService>();
        services.AddSingleton<IDialogService, AvaloniaDialogService>();
        services.AddSingleton<IClipboardService, AvaloniaClipboardService>();
        services.AddSingleton<IConfigurationService, ConfigurationService>();
        services.AddSingleton<IRecentFilesService, RecentFilesService>();
        services.AddSingleton<IMousePositionService, AvaloniaMousePositionService>();
        services.AddSingleton<ISingleInstanceService, SingleInstanceService>();
        services.AddSingleton<IContextMenuService, AvaloniaContextMenuService>();
        services.AddSingleton<IProcessService, ProcessService>();

        // Register ViewModels as factory
        services.AddSingleton<MainViewModel>(sp =>
        {
            var cfgService = sp.GetRequiredService<IConfigurationService>();
            var cfg = sp.GetRequiredService<AppConfiguration>();
            return new MainViewModel(
                sp.GetRequiredService<IFileService>(),
                sp.GetRequiredService<IDialogService>(),
                sp.GetRequiredService<IClipboardService>(),
                cfgService,
                sp.GetRequiredService<IRecentFilesService>(),
                sp.GetRequiredService<IMousePositionService>(),
                sp.GetRequiredService<IProcessService>(),
                sp,
                cfg);
        });

        // Register ViewModels
        services.AddSingleton<SettingTabViewModel>();
        services.AddSingleton<AboutTabViewModel>();

        // Register Views
        services.AddTransient<MainWindow>();
    }

    internal void ApplyTheme(AppTheme appTheme)
    {
        RequestedThemeVariant = appTheme switch
        {
            AppTheme.Light => ThemeVariant.Light,
            AppTheme.Dark => ThemeVariant.Dark,
            _ => ThemeVariant.Default
        };
    }

    public async void OnExit()
    {
        var singleInstance = _serviceProvider?.GetService<ISingleInstanceService>();
        if (singleInstance != null)
        {
            await singleInstance.StopServerAsync();
        }

        _serviceProvider?.Dispose();
    }
}