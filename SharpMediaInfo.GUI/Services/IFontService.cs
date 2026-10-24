namespace SharpMediaInfo.GUI.Services
{
    /// <summary>
    /// Platform-specific font queries used to keep the report rendering sensible across
    /// operating systems (the shared layer has no access to the font system).
    /// </summary>
    public interface IFontService
    {
        /// <summary>Returns true if a font family with the given name is installed.</summary>
        bool IsAvailable(string? fontFamily);

        /// <summary>
        /// Returns true if the font renders every character with the same advance width
        /// (i.e. it is monospaced). Unknown/unavailable fonts are treated as non-monospace.
        /// </summary>
        bool IsMonospace(string? fontFamily);

        /// <summary>
        /// Returns a monospaced font that is actually installed on this machine, used as the
        /// default for reports (MediaInfo output is column-aligned and only reads well in one).
        /// </summary>
        string GetDefaultReportFont();
    }
}
