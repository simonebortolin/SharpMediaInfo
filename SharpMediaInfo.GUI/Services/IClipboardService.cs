namespace SharpMediaInfo.GUI.Services
{
    /// <summary>
    /// Defines methods for copying text to and retrieving text from the system clipboard.
    /// </summary>
    /// <remarks>Implementations of this interface provide clipboard functionality for applications, enabling
    /// text-based data transfer between the application and other programs or system components. Thread safety and
    /// clipboard availability may vary depending on the platform and implementation.</remarks>
    public interface IClipboardService
    {
        /// <summary>
        /// Copies the specified text to the clipboard.
        /// </summary>
        /// <param name="text">The text to copy.</param>
        void CopyText(string text);

        /// <summary>
        /// Gets the current text from the clipboard.
        /// </summary>
        /// <returns>The text from the clipboard, or null if no text is available.</returns>
        string? GetText();
    }
}