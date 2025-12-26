using System;
using System.Threading.Tasks;

namespace SharpMediaInfo.GUI.Services;

/// <summary>
/// Service for managing single instance behavior and inter-process communication
/// </summary>
public interface ISingleInstanceService
{
    /// <summary>
    /// Check if we should enforce single instance at startup.
    /// Returns false if launched with --window-position (drag & drop mode)
    /// </summary>
    /// <param name="args">Command line arguments</param>
    /// <returns>True if single instance check should be performed</returns>
    bool ShouldCheckSingleInstance(string[] args);
    
    /// <summary>
    /// Try to send files to existing master instance via Named Pipe.
    /// If successful, caller should shutdown.
    /// </summary>
    /// <param name="filePaths">File paths to send</param>
    /// <returns>True if sent successfully (caller should shutdown), false if no master found</returns>
    Task<bool> TrySendToMasterAsync(string[] filePaths);
    
    /// <summary>
    /// Start IPC server - becomes master or secondary with election monitoring.
    /// ALL instances participate in election (even drag & drop instances).
    /// </summary>
    /// <param name="onFilesReceived">Callback when files are received from another instance</param>
    Task StartServerAsync(Func<string[], Task> onFilesReceived);
    
    /// <summary>
    /// Stop IPC server gracefully
    /// </summary>
    Task StopServerAsync();
}
