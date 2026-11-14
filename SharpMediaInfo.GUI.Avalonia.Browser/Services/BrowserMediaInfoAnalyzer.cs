using SharpMediaInfo.GUI.Services;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SharpMediaInfo.GUI.Avalonia.Browser.Services;

/// <summary>
/// Analyzes a picked browser file in place: MediaInfo reads it in chunks (and seeks in it) through
/// the File's stream, so even large videos are never loaded whole into memory.
/// </summary>
public sealed class BrowserMediaInfoAnalyzer : IMediaInfoAnalyzer
{
    private readonly BrowserFileRegistry _registry;

    public BrowserMediaInfoAnalyzer(BrowserFileRegistry registry)
    {
        _registry = registry;
    }

    // Browser file reads are asynchronous only.
    public IMediaInfo? AnalyzeFile(string filePath, bool includeCoverData)
        => throw new NotSupportedException("Use AnalyzeFileAsync in the browser");

    public async Task<IMediaInfo?> AnalyzeFileAsync(string filePath, bool includeCoverData)
    {
        var file = _registry.GetFile(filePath) ?? throw new FileNotFoundException($"File not found: {filePath}", filePath);

        await using var stream = new SeekableBlobStream(await file.OpenReadAsync());
        return await MediaInfoBuilder.FromStream(stream)
            .WithCoverData(includeCoverData)
            .WithReports()
            .BuildAsync();
    }
}
