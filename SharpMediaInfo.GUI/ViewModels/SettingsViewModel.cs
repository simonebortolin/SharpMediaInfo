using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SharpMediaInfo.GUI.Models;
using SharpMediaInfo.GUI.Services;
using System.ComponentModel;
using System.Threading.Tasks;

namespace SharpMediaInfo.GUI.ViewModels
{
    public partial class SettingsTabViewModel : AboutViewModel
    {
        private readonly IConfigurationService _configurationService;

        [ObservableProperty]
        private AppConfiguration _configuration;

        public SettingsTabViewModel(IConfigurationService configurationService, AppConfiguration configuration)
        {
            _configurationService = configurationService;
            _configuration = configuration;
            _configuration.PropertyChanged += Configuration_PropertyChanged;
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

        partial void OnConfigurationChanged(AppConfiguration value)
        {
            // Config changes are saved explicitly via SaveConfigurationAsync
            // Theme application can be handled at startup or where appropriate
            _ = value;
        }

        public string Header => "Settings";

        public Task<bool> OnCloseAsync()
        {
            return Task.FromResult(true);
        }
    }
}