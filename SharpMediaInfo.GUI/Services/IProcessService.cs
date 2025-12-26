using System.Threading.Tasks;

namespace SharpMediaInfo.GUI.Services;

/// <summary>
/// Service for starting external processes safely
/// </summary>
public interface IProcessService
{
    /// <summary>
    /// Start a new instance of the application with arguments
    /// </summary>
    /// <param name="arguments">Command line arguments</param>
    /// <returns>True if started successfully</returns>
    Task<bool> StartNewInstanceAsync(string arguments);
    
    /// <summary>
    /// Open a URL in the default browser
    /// </summary>
    /// <param name="url">URL to open</param>
    /// <returns>True if opened successfully</returns>
    Task<bool> OpenUrlAsync(string url);
    
    /// <summary>
    /// Open a file with the default application
    /// </summary>
    /// <param name="filePath">Path to file</param>
    /// <returns>True if opened successfully</returns>
    Task<bool> OpenFileWithDefaultAppAsync(string filePath);
    
    /// <summary>
    /// Get the current application executable path
    /// </summary>
    /// <returns>Path to current executable</returns>
    string GetCurrentExecutablePath();
}
