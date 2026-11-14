using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpMediaInfo.GUI.Avalonia.Browser.Services;

/// <summary>
/// The browser gives no file paths, only File handles. The view models are path based, so each
/// picked or dropped file gets a stable pseudo path ("/browser/&lt;n&gt;/&lt;name&gt;", whose file name
/// is the real one) mapped to its <see cref="IStorageFile"/>. Picked folders get one too.
/// </summary>
public sealed class BrowserFileRegistry
{
    private const string Root = "/browser/";

    private readonly Dictionary<string, IStorageFile> _files = new(StringComparer.Ordinal);
    private readonly Dictionary<string, string[]> _folders = new(StringComparer.Ordinal);
    private int _next;

    public string Register(IStorageFile file)
    {
        var path = $"{Root}{++_next}/{file.Name}";
        _files[path] = file;
        return path;
    }

    public string RegisterFolder(IEnumerable<IStorageFile> files)
    {
        var path = $"{Root}{++_next}";
        _folders[path] = files.Select(Register).ToArray();
        return path;
    }

    public IStorageFile? GetFile(string path) => _files.GetValueOrDefault(path);

    public bool FileExists(string path) => _files.ContainsKey(path);

    public bool FolderExists(string path) => _folders.ContainsKey(path);

    public string[] GetFiles(string folderPath) => _folders.GetValueOrDefault(folderPath) ?? Array.Empty<string>();
}
