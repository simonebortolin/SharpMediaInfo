using SharpMediaInfo.GUI.Models;
using SharpMediaInfo.GUI.Services;
using Microsoft.Win32;
using System.IO;
using System.Threading.Tasks;

namespace SharpMediaInfo.GUI.WPF.Services;

/// <summary>
/// WPF implementation of file service
/// </summary>
public class WpfFileService : IFileService
{
    private string GetFilter(FileType fileType)
    {
        return fileType switch
        {
            FileType.Media => "Media Files|*.*",
            FileType.Text => "Text Files|*.txt",
            FileType.Image => "Image Files|*.jpeg;*.png;*.bmp",
            FileType.All => "All Files|*.*",
            _ => "All Files|*.*"
        };
    }

    public Task<string?> OpenFileAsync(FileType fileType)
    {
        var dialog = new OpenFileDialog
        {
            Filter = GetFilter(fileType),
            Title = "Select a file to analyze",
            Multiselect = false
        };

        return Task.FromResult(dialog.ShowDialog() == true ? dialog.FileName : null);
    }

    public Task<string[]?> OpenFilesAsync(FileType fileType)
    {
        var dialog = new OpenFileDialog
        {
            Filter = GetFilter(fileType),
            Title = "Select files to analyze",
            Multiselect = true
        };

        return Task.FromResult(dialog.ShowDialog() == true ? dialog.FileNames : null);
    }

    public Task<string?> OpenFolderAsync()
    {
        var dialog = new OpenFolderDialog
        {
            Title = "Select a Directory"
        };

        return Task.FromResult(dialog.ShowDialog() == true ? dialog.FolderName : null);
    }

    public async Task WriteTextFileAsync(string content, string fileName)
    {
        await File.WriteAllTextAsync(fileName, content);
    }

    public async Task SaveTextFileAsync(string content, FileType fileType, string? defaultFileName = null, string? defaultExtension = null)
    {
        var dialog = new SaveFileDialog
        {
            Filter = GetFilter(fileType),
            Title = "Choose where to save the file",
            DefaultExt = defaultExtension ?? ".txt",
            FileName = defaultFileName ?? "file.txt"
        };

        if (dialog.ShowDialog() == true)
        {
            await File.WriteAllTextAsync(dialog.FileName, content);
        }
    }

    public async Task WriteBinaryFileAsync(byte[] content, string fileName)
    {
        await File.WriteAllBytesAsync(fileName, content);
    }

    public async Task SaveBinaryFileAsync(byte[] content, FileType fileType, string? defaultFileName = null, string? defaultExtension = null)
    {
        var dialog = new SaveFileDialog
        {
            Filter = GetFilter(fileType),
            Title = "Choose where to save the file",
            DefaultExt = defaultExtension ?? ".bin",
            FileName = defaultFileName ?? "file.bin"
        };

        if (dialog.ShowDialog() == true)
        {
            await File.WriteAllBytesAsync(dialog.FileName, content);
        }
    }

    public bool FileExists(string path) => File.Exists(path);

    public bool DirectoryExists(string path) => Directory.Exists(path);

    public string[] GetFilesInDirectory(string path) => Directory.GetFiles(path);

    public Task<string> ReadTextFileAsync(string fileName)
    {
        return File.ReadAllTextAsync(fileName);
    }

    public Task<byte[]> ReadBinaryFileAsync(string fileName)
    {
        return File.ReadAllBytesAsync(fileName);
    }

    public string? ShowSaveFileDialog(string title, string filter, string defaultFileName)
    {
        var saveFileDialog = new SaveFileDialog
        {
            Title = title,
            Filter = filter,
            FileName = defaultFileName
        };

        return saveFileDialog.ShowDialog() == true ? saveFileDialog.FileName : null;
    }

    public Task<string[]?> OpenFileDialogAsync(FileDialogOptions options)
    {
        var dialog = new OpenFileDialog
        {
            Title = options.Title,
            Filter = GetFilter(options.FileType),
            Multiselect = options.AllowMultipleSelection
        };

        return Task.FromResult(dialog.ShowDialog() == true ? dialog.FileNames : null);
    }

    public async Task SaveFileAsync(string content, FileDialogOptions options)
    {
        var dialog = new SaveFileDialog
        {
            Title = options.Title,
            Filter = GetFilter(options.FileType),
            FileName = options.DefaultFileName,
            DefaultExt = options.DefaultExtension
        };

        if (dialog.ShowDialog() == true)
        {
            await File.WriteAllTextAsync(dialog.FileName, content);
        }
    }

    public async Task SaveBinaryFileAsync(byte[] content, FileDialogOptions options)
    {
        var dialog = new SaveFileDialog
        {
            Title = options.Title,
            Filter = GetFilter(options.FileType),
            FileName = options.DefaultFileName,
            DefaultExt = options.DefaultExtension
        };

        if (dialog.ShowDialog() == true)
        {
            await File.WriteAllBytesAsync(dialog.FileName, content);
        }
    }
}
