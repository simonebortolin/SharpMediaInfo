using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SharpMediaInfo.GUI.Models;
using SharpMediaInfo.GUI.Services;
using System.ComponentModel;
using System.Threading.Tasks;

namespace SharpMediaInfo.GUI.ViewModels
{
    public partial class SettingsTabViewModel : AboutTabViewModel
    {
        private readonly IConfigurationService _configurationService;
        private readonly IContextMenuService? _contextMenuService;
        private readonly IDialogService? _dialogService;

        [ObservableProperty]
        private AppConfiguration _configuration;

        [ObservableProperty]
        private bool _isContextMenuInstalled;

        [ObservableProperty]
        private bool _isCheckingContextMenu;

        public SettingsTabViewModel(
            IConfigurationService configurationService, 
            AppConfiguration configuration,
            IContextMenuService? contextMenuService,
            IDialogService? dialogService)
        {
            _configurationService = configurationService;
            _configuration = configuration;
            _contextMenuService = contextMenuService;
            _dialogService = dialogService;
            
            _configuration.PropertyChanged += Configuration_PropertyChanged;

            // Check context menu status on load
            _ = CheckContextMenuStatusAsync();
        }

        private async Task CheckContextMenuStatusAsync()
        {
            if (_contextMenuService == null)
                return;

            IsCheckingContextMenu = true;
            try
            {
                IsContextMenuInstalled = await _contextMenuService.IsInstalledAsync();
            }
            finally
            {
                IsCheckingContextMenu = false;
            }
        }

        private async void Configuration_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            try
            {
                await _configurationService.SaveConfigurationAsync(Configuration);
            }
            catch
            {
                // ignore failures to avoid crashing UI
            }
        }

        [RelayCommand]
        public async Task SaveConfigurationAsync()
        {
            await _configurationService.SaveConfigurationAsync(Configuration);
        }

        [RelayCommand(CanExecute = nameof(CanInstallContextMenu))]
        private async Task InstallContextMenuAsync()
        {
            if (_contextMenuService == null || _dialogService == null)
                return;

            IsCheckingContextMenu = true;
            try
            {
                var exePath = _contextMenuService.GetExecutablePath();
                var success = await _contextMenuService.InstallAsync(exePath);

                if (success)
                {
                    await _dialogService.ShowInformationAsync(
                        "Context menu installed successfully!\n\n" +
                        "You can now right-click any file in Windows Explorer and select 'Analyze with SharpMediaInfo'.\n\n" +
                        "Note: Installed for current user only.\n" +
                        "On Windows 11, look for it under 'Show more options'.");
                    
                    IsContextMenuInstalled = true;
                }
                else
                {
                    await _dialogService.ShowErrorAsync(
                        "Failed to install context menu.\n\n" +
                        "Please check that the application path is correct and try again.");
                }
            }
            finally
            {
                IsCheckingContextMenu = false;
                InstallContextMenuCommand.NotifyCanExecuteChanged();
                UninstallContextMenuCommand.NotifyCanExecuteChanged();
            }
        }

        [RelayCommand(CanExecute = nameof(CanUninstallContextMenu))]
        private async Task UninstallContextMenuAsync()
        {
            if (_contextMenuService == null || _dialogService == null)
                return;

            var confirm = await _dialogService.ShowConfirmationAsync(
                "Are you sure you want to remove the context menu integration?",
                "Uninstall Context Menu");

            if (!confirm)
                return;

            IsCheckingContextMenu = true;
            try
            {
                var success = await _contextMenuService.UninstallAsync();

                if (success)
                {
                    await _dialogService.ShowInformationAsync("Context menu uninstalled successfully!");
                    IsContextMenuInstalled = false;
                }
                else
                {
                    await _dialogService.ShowErrorAsync("Failed to uninstall context menu.\n\nPlease try again.");
                }
            }
            finally
            {
                IsCheckingContextMenu = false;
                InstallContextMenuCommand.NotifyCanExecuteChanged();
                UninstallContextMenuCommand.NotifyCanExecuteChanged();
            }
        }

        private bool CanInstallContextMenu() => !IsCheckingContextMenu && !IsContextMenuInstalled && _contextMenuService != null;
        
        private bool CanUninstallContextMenu() => !IsCheckingContextMenu && IsContextMenuInstalled && _contextMenuService != null;

        partial void OnConfigurationChanged(AppConfiguration value)
        {
            // Config changes are saved explicitly via SaveConfigurationAsync
            // Theme application can be handled at startup or where appropriate
            _ = value;
        }

        public override string Header => "Settings";
    }
}