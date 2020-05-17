namespace GetMediaInfoGUI.Util
{
    /// <summary>
    /// The constants
    /// </summary>
    class Constants
    {
        /// <summary>
        /// The argument that signals the drop event of a tab from the outside.
        /// </summary>
        public const string OutsideTabDropMode = "dropMode";
        /// <summary>
        /// The argument that signals to start the updater immediately before launching the frontend.
        /// </summary>
        public const string ForceUpdateMode = "forceUpdate";

        /// <summary>
        /// The mediainfo rss official url.
        /// </summary>
        public const string MediainfoRssUrl = "https://mediaarea.net/rss/mediainfo_updates.xml";
        /// <summary>
        /// The mediainfo base download URL
        /// </summary>
        public const string MediainfoBaseDownloadUrl = "https://mediaarea.net/download/binary/libmediainfo0/";

        /// <summary>
        /// The name of the configuration file.
        /// </summary>
        public const string ConfigurationFileName = "settings.xml";
        /// <summary>
        /// The name of the history file.
        /// </summary>
        public const string HistoryFileName = "history.xml";
        /// <summary>
        /// The default font family.
        /// </summary>
        public const string DefaultFontFamily = "Sagoe UI";
    }
}
