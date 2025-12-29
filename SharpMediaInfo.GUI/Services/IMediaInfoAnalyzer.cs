using System.Threading.Tasks;
namespace SharpMediaInfo.GUI.Services;

/// <summary>
/// Service for analyzing media files and extracting metadata
/// </summary>
public interface IMediaInfoAnalyzer
{
    /// <summary>
    /// Analyzes a media file and returns MediaInfo
    /// </summary>
    /// <param name="filePath">Path to the media file</param>
    /// <param name="includeCoverData">Whether to include cover/thumbnail data</param>
    /// <returns>MediaInfo object with file metadata</returns>
    IMediaInfo? AnalyzeFile(string filePath, bool includeCoverData);

    /// <summary>
    /// Analyzes a media file asynchronously
    /// </summary>
    /// <param name="filePath">Path to the media file</param>
    /// <param name="includeCoverData">Whether to include cover/thumbnail data</param>
    /// <returns>MediaInfo object with file metadata</returns>
    Task<IMediaInfo?> AnalyzeFileAsync(string filePath, bool includeCoverData);
}
