using System.Threading.Tasks;
using SharpMediaInfo.GUI.Models;

namespace SharpMediaInfo.GUI.Services;

/// <summary>
/// Interface for services that provide file system operations
/// </summary>
public interface IFileService
{
    /// <summary>
    /// Opens file picker dialog for single file selection
    /// </summary>
    /// <returns>File name</returns>
    Task<string?> OpenFileAsync(FileType fileType);

    /// <summary>
    /// Opens file picker dialog for multiple file selection
    /// </summary>
    /// <returns>File names</returns>
    Task<string[]?> OpenFilesAsync(FileType fileType);

    /// <summary>
    /// Opens folder picker dialog
    /// </summary>
    /// <returns>Folder name</returns>
    Task<string?> OpenFolderAsync();

    /// <summary>
    /// Asynchronously writes the specified binary content to a file with the given name.
    /// </summary>
    /// <param name="content">The binary data to write to the file. Cannot be null.</param>
    /// <param name="fileName">The name of the file to which the content will be written. Cannot be null or empty.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    Task WriteBinaryFileAsync(byte[] content, string fileName);

    /// <summary>
    /// Asynchronously writes the specified text content to a file with the given name, overwriting the file if it
    /// already exists.
    /// </summary>
    /// <param name="content">The text content to write to the file. Can be empty but cannot be null.</param>
    /// <param name="fileName">The name of the file to which the content will be written. Must be a valid file path and cannot be null or
    /// empty.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    Task WriteTextFileAsync(string content, string fileName);

    /// <summary>
    /// Asynchronously reads all text from the specified file.
    /// </summary>
    /// <param name="fileName">The path to the file to read. The path can be relative or absolute. Cannot be null or empty.</param>
    /// <returns>A task that represents the asynchronous read operation. The task result contains the contents of the file as a
    /// string.</returns>
    Task<string> ReadTextFileAsync(string fileName);

    /// <summary>
    /// Asynchronously reads the contents of a binary file and returns the data as a byte array.
    /// </summary>
    /// <param name="fileName">The path to the file to read. This can be either a relative or absolute path. Cannot be null or empty.</param>
    /// <returns>A task that represents the asynchronous read operation. The task result contains a byte array with the contents
    /// of the file.</returns>
    Task<byte[]> ReadBinaryFileAsync(string fileName);

    /// <summary>
    /// Checks if a file exists
    /// </summary>
    bool FileExists(string path);

    /// <summary>
    /// Checks if a directory exists
    /// </summary>
    bool DirectoryExists(string path);

    /// <summary>
    /// Gets all files in a directory
    /// </summary>
    string[] GetFilesInDirectory(string path);

    /// <summary>
    /// Shows a Save File dialog and returns the selected file path.
    /// </summary>
    /// <param name="title">The title of the dialog.</param>
    /// <param name="filter">The file filter (e.g., "Text Files|*.txt").</param>
    /// <param name="defaultFileName">The default file name.</param>
    /// <returns>The selected file path, or null if canceled.</returns>
    string? ShowSaveFileDialog(string title, string filter, string defaultFileName);

    /// <summary>
    /// Saves text content to a file with a specific file type.
    /// </summary>
    Task SaveTextFileAsync(string content, FileType fileType, string? defaultFileName = null, string? defaultExtension = null);

    /// <summary>
    /// Saves binary content to a file with a specific file type.
    /// </summary>
    Task SaveBinaryFileAsync(byte[] content, FileType fileType, string? defaultFileName = null, string? defaultExtension = null);

    /// <summary>
    /// Opens a file dialog with the specified options.
    /// </summary>
    Task<string[]?> OpenFileDialogAsync(FileDialogOptions options);

    /// <summary>
    /// Saves a file with the specified options.
    /// </summary>
    Task SaveFileAsync(string content, FileDialogOptions options);

    /// <summary>
    /// Saves binary content to a file with the specified options.
    /// </summary>
    Task SaveBinaryFileAsync(byte[] content, FileDialogOptions options);
}
