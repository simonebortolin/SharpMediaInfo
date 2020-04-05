using System;
using System.Xml.Serialization;

namespace GetMediaInfoGUI.Configuration
{
    /// <summary>
    /// The program configuration.
    /// </summary>
    [XmlRoot("configuration")]
    public class Config
    {
        #region General
        /// <summary>
        /// Gets or sets a value indicating whether the standard report needs to be automatically saved to disk or not.
        /// </summary>
        public bool AutoSaveReport { set; get; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the simple report is disabled or not.
        /// </summary>
        public bool DisableSimpleReport { set; get; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the complete report is disabled or not.
        /// </summary>
        public bool DisableCompleteReport { set; get; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the tree report is disabled or not.
        /// </summary>
        public bool DisableTreeReport { set; get; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether show or hide millisends.
        /// </summary>
        public bool ShowMilliseconds { set; get; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to remove justification in standard and complete report.
        /// </summary>
        public bool RemoveJustification { set; get; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to show or hide the cover if it's present.
        /// </summary>
        public bool ShowCover { get; set; } = false;
        #endregion

        #region Appearence        
        /// <summary>
        /// Gets or sets a value indicating whether the report text needs to be bold.
        /// </summary>
        public bool ReportBoldStyle { set; get; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the report text needs to be italic.
        /// </summary>
        public bool ReportItalicStyle { set; get; } = false;

        /// <summary>
        /// Gets or sets the size of the report text.
        /// </summary>
        public double ReportFontSize { get; set; } = 12d;

        /// <summary>
        /// Gets or sets the font family of the report text.
        /// </summary>
        public string ReportFontFamily { get; set; } = "Segoe UI";
        #endregion

        #region History
        /// <summary>
        /// Gets or sets the size of the history.
        /// </summary>
        public int HistorySize { get; set; } = 5;
        #endregion

        #region Last update check
        /// <summary>
        /// Gets or sets the last time when a update check was performed.
        /// </summary>
        [XmlElement(DataType = "dateTime")]
        public DateTime LastUpdateCheck { get; set; } = DateTime.MinValue;
        #endregion
    }
}
