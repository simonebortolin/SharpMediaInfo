using Avalonia.Platform.Storage;
using SharpMediaInfo.GUI.Models;
using SharpMediaInfo.GUI.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpMediaInfo.GUI.Avalonia.Browser.Services;

/// <summary>
/// <see cref="IFileService"/> for the browser: opening goes through the browser file pickers
/// (files are registered in <see cref="BrowserFileRegistry"/>), saving through the save picker
/// where the browser has one and otherwise as a download.
/// </summary>
public sealed class BrowserFileService : IFileService
{
    private readonly BrowserFileRegistry _registry;

    public BrowserFileService(BrowserFileRegistry registry)
    {
        _registry = registry;
    }

    private static IStorageProvider? StorageProvider => App.GetTopLevel()?.StorageProvider;

    private static List<FilePickerFileType> GetFileFilters(FileType fileType) => fileType switch
    {
        FileType.Text => new() { new("Text Files") { Patterns = new[] { "*.txt" }, MimeTypes = new[] { "text/plain" } } },
        FileType.Image => new() { new("Image Files") { Patterns = new[] { "*.jpg", "*.jpeg", "*.png" }, MimeTypes = new[] { "image/jpeg", "image/png" } } },
        // Media and All: no filter, MediaInfo recognizes far more formats than any list here.
        _ => new() { FilePickerFileTypes.All }
    };

    private async Task<string[]?> PickFilesAsync(string title, bool allowMultiple, FileType fileType)
    {
        if (StorageProvider is not { CanOpen: true } provider)
            return null;

        var files = await provider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = title,
            AllowMultiple = allowMultiple,
            FileTypeFilter = GetFileFilters(fileType)
        });

        return files.Select(_registry.Register).ToArray();
    }

    public async Task<string?> OpenFileAsync(FileType fileType)
        => (await PickFilesAsync("Select File", false, fileType))?.FirstOrDefault();

    public Task<string[]?> OpenFilesAsync(FileType fileType)
        => PickFilesAsync("Select Files", true, fileType);

    public Task<string[]?> OpenFileDialogAsync(FileDialogOptions options)
        => PickFilesAsync(options.Title, options.AllowMultipleSelection, options.FileType);

    public async Task<string?> OpenFolderAsync()
    {
        // Folder picking needs the File System Access API (Chromium based browsers).
        if (StorageProvider is not { CanPickFolder: true } provider)
            return null;

        var folder = (await provider.OpenFolderPickerAsync(new FolderPickerOpenOptions { Title = "Select Folder" })).FirstOrDefault();
        if (folder == null)
            return null;

        var files = new List<IStorageFile>();
        await foreach (var item in folder.GetItemsAsync())
        {
            if (item is IStorageFile file)
                files.Add(file);
        }

        return _registry.RegisterFolder(files.OrderBy(f => f.Name, StringComparer.OrdinalIgnoreCase));
    }

    public bool FileExists(string path) => _registry.FileExists(path);

    public bool DirectoryExists(string path) => _registry.FolderExists(path);

    public string[] GetFilesInDirectory(string path) => _registry.GetFiles(path);

    public async Task<byte[]> ReadBinaryFileAsync(string fileName)
    {
        var file = _registry.GetFile(fileName) ?? throw new FileNotFoundException($"File not found: {fileName}", fileName);
        await using var stream = await file.OpenReadAsync();
        using var buffer = new MemoryStream();
        await stream.CopyToAsync(buffer);
        return buffer.ToArray();
    }

    public async Task<string> ReadTextFileAsync(string fileName)
        => Encoding.UTF8.GetString(await ReadBinaryFileAsync(fileName));

    // Writing "next to" a file (e.g. the auto-saved report) can't happen in a browser: download it.
    public Task WriteTextFileAsync(string content, string fileName)
        => WriteBinaryFileAsync(Encoding.UTF8.GetBytes(content), fileName);

    public Task WriteBinaryFileAsync(byte[] content, string fileName)
    {
        BrowserInterop.Download(Path.GetFileName(fileName), GetMimeType(fileName), content);
        return Task.CompletedTask;
    }

    public string? ShowSaveFileDialog(string title, string filter, string defaultFileName)
        => throw new NotSupportedException("Use SaveFileAsync instead");

    public Task SaveTextFileAsync(string content, FileType fileType, string? defaultFileName = null, string? defaultExtension = null)
        => SaveAsync(Encoding.UTF8.GetBytes(content), "Save File", fileType, defaultFileName, defaultExtension);

    public Task SaveBinaryFileAsync(byte[] content, FileType fileType, string? defaultFileName = null, string? defaultExtension = null)
        => SaveAsync(content, "Save File", fileType, defaultFileName, defaultExtension);

    public Task SaveFileAsync(string content, FileDialogOptions options)
        => SaveAsync(Encoding.UTF8.GetBytes(content), options.Title, options.FileType, options.DefaultFileName, options.DefaultExtension);

    public Task SaveBinaryFileAsync(byte[] content, FileDialogOptions options)
        => SaveAsync(content, options.Title, options.FileType, options.DefaultFileName, options.DefaultExtension);

    private async Task SaveAsync(byte[] content, string title, FileType fileType, string? defaultFileName, string? defaultExtension)
    {
        var fileName = string.IsNullOrEmpty(defaultFileName) ? "file" + defaultExtension : defaultFileName;

        // The save picker needs the File System Access API; elsewhere (Firefox, Safari) download.
        if (StorageProvider is not { CanSave: true } provider)
        {
            await WriteBinaryFileAsync(content, fileName);
            return;
        }

        var file = await provider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = title,
            SuggestedFileName = fileName,
            DefaultExtension = defaultExtension,
            FileTypeChoices = GetFileFilters(fileType)
        });

        if (file == null)
            return;

        await using var stream = await file.OpenWriteAsync();
        await stream.WriteAsync(content);
    }

    private static string GetMimeType(string fileName) => Path.GetExtension(fileName).ToLowerInvariant() switch
    {
        ".txt" => "text/plain",
        ".jpg" or ".jpeg" => "image/jpeg",
        ".png" => "image/png",
        _ => "application/octet-stream"
    };
}
