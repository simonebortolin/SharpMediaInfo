using System.Threading.Tasks;
using System;
using System.IO;
namespace SharpMediaInfo.GUI.Services;

/// <summary>
/// Default implementation of IMediaInfoAnalyzer using MediaInfoBuilder
/// </summary>
public class MediaInfoAnalyzer : IMediaInfoAnalyzer
{
    /// <inheritdoc/>
    public IMediaInfo? AnalyzeFile(string filePath, bool includeCoverData)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"File not found: {filePath}", filePath);

        var builder = MediaInfoBuilder.FromFile(filePath)
            .WithCoverData(includeCoverData)
            .WithReports();

        return builder.Build();
    }

    /// <inheritdoc/>
    public Task<IMediaInfo?> AnalyzeFileAsync(string filePath, bool includeCoverData)
    {
        return Task.Run(() => AnalyzeFile(filePath, includeCoverData));
    }
}
