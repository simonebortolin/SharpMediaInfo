using System;
using System.Runtime.InteropServices.JavaScript;

namespace SharpMediaInfo.GUI.Avalonia.Browser.Services;

/// <summary>Functions exported to .NET by wwwroot/main.js (setModuleImports).</summary>
internal static partial class BrowserInterop
{
    private const string Module = "main.js";

    /// <summary>localStorage.getItem; null when missing or when storage is unavailable.</summary>
    [JSImport("storage.getItem", Module)]
    internal static partial string? GetItem(string key);

    /// <summary>localStorage.setItem; false when storage is unavailable or full.</summary>
    [JSImport("storage.setItem", Module)]
    internal static partial bool SetItem(string key, string value);

    /// <summary>Hands the bytes to the browser as a download.</summary>
    [JSImport("download", Module)]
    internal static partial void Download(string fileName, string mimeType, [JSMarshalAs<JSType.MemoryView>] ArraySegment<byte> content);
}
