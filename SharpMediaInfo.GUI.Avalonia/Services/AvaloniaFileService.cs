using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using SharpMediaInfo.GUI.Models;
using SharpMediaInfo.GUI.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SharpMediaInfo.GUI.Avalonia.Services;

public class AvaloniaFileService : IFileService
{
    private Window? GetMainWindow()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            return desktop.MainWindow;
        }
        return null;
    }

    private static List<FilePickerFileType> GetFileFilters(FileType fileType)
    {
        return fileType switch
        {
            FileType.Media => new List<FilePickerFileType>
            {
                new("Media Files") { Patterns = new[] { "*.mkv", "*.mp4", "*.avi", "*.mov", "*.wmv", "*.flv", "*.webm", "*.m4v", "*.mpg", "*.mpeg", "*.3gp", "*.mp3", "*.flac", "*.wav", "*.aac", "*.ogg", "*.wma", "*.m4a" } },
                new("All Files") { Patterns = new[] { "*" } }
            },
            FileType.Text => new List<FilePickerFileType>
            {
                new("Text Files") { Patterns = new[] { "*.txt" } },
                new("All Files") { Patterns = new[] { "*" } }
            },
            FileType.Image => new List<FilePickerFileType>
            {
                new("Image Files") { Patterns = new[] { "*.jpg", "*.jpeg", "*.png", "*.bmp", "*.gif", "*.webp" } },
                new("All Files") { Patterns = new[] { "*" } }
            },
            _ => new List<FilePickerFileType>
            {
                new("All Files") { Patterns = new[] { "*" } }
            }
        };
    }

    public async Task<string?> OpenFileAsync(FileType fileType)
    {
        var window = GetMainWindow();
        if (window == null) return null;

        var result = await window.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select File",
            AllowMultiple = false,
            FileTypeFilter = GetFileFilters(fileType)
        });

        return result?.FirstOrDefault()?.Path.LocalPath;
    }

    public async Task<string[]?> OpenFilesAsync(FileType fileType)
    {
        var window = GetMainWindow();
        if (window == null) return null;

        var result = await window.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select Files",
            AllowMultiple = true,
            FileTypeFilter = GetFileFilters(fileType)
        });

        return result?.Select(f => f.Path.LocalPath).ToArray();
    }

    public async Task<string?> OpenFolderAsync()
    {
        var window = GetMainWindow();
        if (window == null) return null;

        var result = await window.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Select Folder",
            AllowMultiple = false
        });

        return result?.FirstOrDefault()?.Path.LocalPath;
    }

    public async Task WriteBinaryFileAsync(byte[] content, string fileName)
    {
        await File.WriteAllBytesAsync(fileName, content);
    }

    public async Task WriteTextFileAsync(string content, string fileName)
    {
        await File.WriteAllTextAsync(fileName, content);
    }

    public async Task<string> ReadTextFileAsync(string fileName)
    {
        return await File.ReadAllTextAsync(fileName);
    }

    public async Task<byte[]> ReadBinaryFileAsync(string fileName)
    {
        return await File.ReadAllBytesAsync(fileName);
    }

    public bool FileExists(string path)
    {
        return File.Exists(path);
    }

    public bool DirectoryExists(string path)
    {
        return Directory.Exists(path);
    }

    public string[] GetFilesInDirectory(string path)
    {
        return Directory.GetFiles(path);
    }

    public string? ShowSaveFileDialog(string title, string filter, string defaultFileName)
    {
        // Synchronous version not ideal for Avalonia, use async version instead
        throw new NotImplementedException("Use SaveFileAsync instead");
    }

    public async Task SaveTextFileAsync(string content, FileType fileType, string? defaultFileName = null, string? defaultExtension = null)
    {
        var window = GetMainWindow();
        if (window == null) return;

        var result = await window.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save File",
            SuggestedFileName = defaultFileName,
            DefaultExtension = defaultExtension,
            FileTypeChoices = GetFileFilters(fileType)
        });

        if (result != null)
        {
            await File.WriteAllTextAsync(result.Path.LocalPath, content);
        }
    }

    public async Task SaveBinaryFileAsync(byte[] content, FileType fileType, string? defaultFileName = null, string? defaultExtension = null)
    {
        var window = GetMainWindow();
        if (window == null) return;

        var result = await window.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save File",
            SuggestedFileName = defaultFileName,
            DefaultExtension = defaultExtension,
            FileTypeChoices = GetFileFilters(fileType)
        });

        if (result != null)
        {
            await File.WriteAllBytesAsync(result.Path.LocalPath, content);
        }
    }

    public async Task<string[]?> OpenFileDialogAsync(FileDialogOptions options)
    {
        var window = GetMainWindow();
        if (window == null) return null;

        var filePickerOptions = new FilePickerOpenOptions
        {
            Title = options.Title,
            AllowMultiple = options.AllowMultipleSelection,
            FileTypeFilter = GetFileFilters(options.FileType)
        };

        var result = await window.StorageProvider.OpenFilePickerAsync(filePickerOptions);
        
        return result?.Select(f => f.Path.LocalPath).ToArray();
    }

    public async Task SaveFileAsync(string content, FileDialogOptions options)
    {
        var window = GetMainWindow();
        if (window == null) return;

        var filePickerOptions = new FilePickerSaveOptions
        {
            Title = options.Title,
            DefaultExtension = options.DefaultExtension,
            SuggestedFileName = options.DefaultFileName,
            FileTypeChoices = GetFileFilters(options.FileType)
        };

        var result = await window.StorageProvider.SaveFilePickerAsync(filePickerOptions);
        
        if (result != null)
        {
            await File.WriteAllTextAsync(result.Path.LocalPath, content);
        }
    }

    public async Task SaveBinaryFileAsync(byte[] content, FileDialogOptions options)
    {
        var window = GetMainWindow();
        if (window == null) return;

        var filePickerOptions = new FilePickerSaveOptions
        {
            Title = options.Title,
            DefaultExtension = options.DefaultExtension,
            SuggestedFileName = options.DefaultFileName,
            FileTypeChoices = GetFileFilters(options.FileType)
        };

        var result = await window.StorageProvider.SaveFilePickerAsync(filePickerOptions);
        
        if (result != null)
        {
            await File.WriteAllBytesAsync(result.Path.LocalPath, content);
        }
    }

    public Task<string> GetConfigurationDirectoryAsync()
    {
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var configPath = Path.Combine(appDataPath, "SharpMediaInfo");
        
        if (!Directory.Exists(configPath))
            Directory.CreateDirectory(configPath);
        
        return Task.FromResult(configPath);
    }
}
