using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace SharpMediaInfo.GUI.Services
{
    public class RecentFilesService : IRecentFilesService
    {
        private const string RecentFilesFileName = "recent_files.json";
        private readonly IFileService _fileService;

        public RecentFilesService(IFileService fileService)
        {
            _fileService = fileService;
        }

        // Stored per-user (alongside the app configuration), never in the working directory.
        private static string GetRecentFilesPath()
        {
            var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SharpMediaInfo");
            Directory.CreateDirectory(folder);
            return Path.Combine(folder, RecentFilesFileName);
        }

        public async Task<ObservableCollection<string>> LoadAsync(int maxItems)
        {
            var path = GetRecentFilesPath();
            var files = new ObservableCollection<string>();
            if (_fileService.FileExists(path))
            {
                try
                {
                    var json = await _fileService.ReadTextFileAsync(path);
                    files = JsonSerializer.Deserialize<ObservableCollection<string>>(json) ?? new ObservableCollection<string>();
                }
                catch
                {
                    // ignore
                }
            }

            Trim(files, maxItems);
            await SaveInternalAsync(files);
            return files;
        }

        public async Task AddAsync(string filePath, ObservableCollection<string> current, int maxItems)
        {
            if (current.Contains(filePath))
            {
                current.Remove(filePath);
            }
            current.Insert(0, filePath);
            Trim(current, maxItems);
            await SaveInternalAsync(current);
        }

        public async Task ClearAsync(ObservableCollection<string> current)
        {
            current.Clear();
            await SaveInternalAsync(current);
        }

        private void Trim(ObservableCollection<string> list, int maxItems)
        {
            var max = maxItems < 1 ? 1 : maxItems;
            while (list.Count > max)
            {
                list.RemoveAt(list.Count - 1);
            }
        }

        private async Task SaveInternalAsync(ObservableCollection<string> list)
        {
            try
            {
                var json = JsonSerializer.Serialize(list);
                await _fileService.WriteTextFileAsync(json, GetRecentFilesPath());
            }
            catch (System.Exception e)
            {
                Debug.WriteLine(e);
            }
        }
    }
}
