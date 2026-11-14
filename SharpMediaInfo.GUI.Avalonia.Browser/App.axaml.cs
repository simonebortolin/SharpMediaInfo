using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Microsoft.Extensions.DependencyInjection;
using SharpMediaInfo.GUI.Avalonia.Browser.Services;
using SharpMediaInfo.GUI.Avalonia.Browser.Views;
using SharpMediaInfo.GUI.Avalonia.Services;
using SharpMediaInfo.GUI.Models;
using SharpMediaInfo.GUI.Services;
using SharpMediaInfo.GUI.ViewModels;

namespace SharpMediaInfo.GUI.Avalonia.Browser;

public partial class App : Application
{
    private ServiceProvider? _serviceProvider;

    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override async void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is ISingleViewApplicationLifetime singleView)
        {
            // Settings live in localStorage.
            var cfgService = new BrowserConfigurationService();
            var cfg = await cfgService.LoadConfigurationAsync();

            // Only the fonts bundled with the app exist here (no system fonts in the browser).
            var fontService = new AvaloniaFontService();
            if (!fontService.IsAvailable(cfg.ReportFontFamily))
                cfg.ReportFontFamily = fontService.GetDefaultReportFont();
            cfg.RemoveJustification = !fontService.IsMonospace(cfg.ReportFontFamily);

            var services = new ServiceCollection();
            services.AddSingleton(cfg);
            services.AddSingleton<IFontService>(fontService);
            services.AddSingleton<IConfigurationService>(cfgService);
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();

            ApplyTheme(cfg.AppTheme);
            cfg.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(AppConfiguration.AppTheme))
                    ApplyTheme(cfg.AppTheme);
            };

            singleView.MainView = _serviceProvider.GetRequiredService<MainView>();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<BrowserFileRegistry>();
        services.AddSingleton<IFileService, BrowserFileService>();
        services.AddSingleton<IMediaInfoAnalyzer, BrowserMediaInfoAnalyzer>();
        services.AddSingleton<IDialogService, AvaloniaDialogService>();
        services.AddSingleton<IClipboardService, AvaloniaClipboardService>();
        services.AddSingleton<IRecentFilesService, BrowserRecentFilesService>();
        services.AddSingleton<IProcessService, BrowserProcessService>();
        services.AddSingleton<IMousePositionService, BrowserMousePositionService>();

        services.AddSingleton(sp => new MainViewModel(
            sp.GetRequiredService<IFileService>(),
            sp.GetRequiredService<IDialogService>(),
            sp.GetRequiredService<IClipboardService>(),
            sp.GetRequiredService<IConfigurationService>(),
            sp.GetRequiredService<IRecentFilesService>(),
            sp.GetRequiredService<IMousePositionService>(),
            sp.GetRequiredService<IProcessService>(),
            sp,
            sp.GetRequiredService<IMediaInfoAnalyzer>(),
            sp.GetRequiredService<AppConfiguration>()));

        // No shell context menu in a browser.
        services.AddSingleton(sp => new SettingTabViewModel(
            sp.GetRequiredService<IConfigurationService>(),
            sp.GetRequiredService<AppConfiguration>(),
            contextMenuService: null,
            sp.GetRequiredService<IDialogService>(),
            sp.GetRequiredService<IFontService>()));
        services.AddSingleton(sp => new AboutTabViewModel(sp.GetRequiredService<IProcessService>()));

        services.AddTransient(sp => new MainView(sp.GetRequiredService<MainViewModel>(), sp.GetRequiredService<BrowserFileRegistry>()));
    }

    private void ApplyTheme(AppTheme appTheme)
    {
        RequestedThemeVariant = appTheme switch
        {
            AppTheme.Light => ThemeVariant.Light,
            AppTheme.Dark => ThemeVariant.Dark,
            _ => ThemeVariant.Default
        };
    }

    /// <summary>The top level hosting the app (pickers, clipboard, launcher), once it is shown.</summary>
    internal static TopLevel? GetTopLevel()
        => Current?.ApplicationLifetime is ISingleViewApplicationLifetime { MainView: { } view }
            ? TopLevel.GetTopLevel(view)
            : null;
}
