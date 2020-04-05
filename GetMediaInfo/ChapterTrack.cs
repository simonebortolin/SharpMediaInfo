namespace GetMediaInfoDDLV2
{
    /// <summary>
    /// Contains properties for a ChapterTrack
    /// </summary>
    public class ChapterTrack
    {
        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        public int? Index { get; set; }

        /// <summary>
        /// Gets or sets the timecode.
        /// </summary>
        public string Timecode { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        public string Language { get; set; }
    }
}
