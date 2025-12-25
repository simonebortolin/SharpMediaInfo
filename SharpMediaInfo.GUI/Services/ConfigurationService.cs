using SharpMediaInfo.GUI.Models;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;
using System;
using System.ComponentModel;

namespace SharpMediaInfo.GUI.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private const string ConfigFilePath = "appsettings.json";
        private readonly IFileService _fileService;
        private bool _attached;

        public ConfigurationService(IFileService fileService)
        {
            _fileService = fileService;
        }

        private static string GetConfigPath()
        {
            var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SharpMediaInfo");
            Directory.CreateDirectory(folder);
            return Path.Combine(folder, "appsettings.json");
        }

        public async Task<AppConfiguration> LoadConfigurationAsync()
        {
            var path = GetConfigPath();
            if (_fileService.FileExists(path))
            {
                var json = await _fileService.ReadTextFileAsync(path);
                return JsonSerializer.Deserialize<AppConfiguration>(json) ?? new AppConfiguration();
            }

            return new AppConfiguration();
        }

        public async Task SaveConfigurationAsync(AppConfiguration configuration)
        {
            var path = GetConfigPath();
            var json = JsonSerializer.Serialize(configuration, new JsonSerializerOptions { WriteIndented = true });
            await _fileService.WriteTextFileAsync(json, path);
        }

        public void Attach(AppConfiguration configuration)
        {
            if (_attached || configuration == null)
                return;

            configuration.PropertyChanged += async (_, __) =>
            {
                try
                {
                    await SaveConfigurationAsync(configuration);
                }
                catch
                {
                    // ignore to avoid crashing UI
                }
            };

            _attached = true;
        }
    }
}