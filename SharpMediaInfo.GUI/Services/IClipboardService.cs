namespace SharpMediaInfo.GUI.Services
{
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