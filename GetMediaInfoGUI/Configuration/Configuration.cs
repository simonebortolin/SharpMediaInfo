using System.Windows.Media;
using System.Xml.Serialization;

namespace GetMediaInfoGUI.Configuration
{
    /// <summary>
    /// The program configuration.
    /// </summary>
    [XmlRoot("configuration")]
    public class Configuration
    {
        #region General
        /// <summary>
        /// Gets or sets a value indicating whether the standard report needs to be automatically saved to disk or not.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the standard report needs to be automatically saved to disk; otherwise, <c>false</c>.
        /// </value>
        public bool AutoSaveReport { set; get; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the simple report is disabled or not.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the simple report is disabled; otherwise, <c>false</c>.
        /// </value>
        public bool DisableSimpleReport { set; get; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the complete report is disabled or not.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the complete report is disabled; otherwise, <c>false</c>.
        /// </value>
        public bool DisableCompleteReport { set; get; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the tree report is disabled or not.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the tree report is disabled; otherwise, <c>false</c>.
        /// </value>
        public bool DisableTreeReport { set; get; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether show or hide millisends.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the milliseconds need to be shown; otherwise, <c>false</c>.
        /// </value>
        public bool ShowMilliseconds { set; get; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to remove justification in standard and complete report.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the justification must be removed; otherwise, <c>false</c>.
        /// </value>
        public bool RemoveJustification { set; get; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to show or hide the cover if it's present.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the cover must be shown if available; otherwise, <c>false</c>.
        /// </value>
        public bool ShowCover { get; set; } = false;
        #endregion

        #region Appearence        
        /// <summary>
        /// Gets or sets a value indicating whether the report text needs to be bold.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the report text needs to be bold; otherwise, <c>false</c>.
        /// </value>
        public bool Bold { set; get; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the report text needs to be italic.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the report text needs to be italic; otherwise, <c>false</c>.
        /// </value>
        public bool Italic { set; get; } = false;

        /// <summary>
        /// Gets or sets the size of the report text.
        /// </summary>
        /// <value>
        /// The size of the report text.
        /// </value>
        public double ReportFontSize { get; set; } = 12d;

        /// <summary>
        /// Gets or sets the font family of the report text.
        /// </summary>
        /// <value>
        /// The font family of the report text.
        /// </value>
        public string ReportFontFamily { get; set; } = "Segoe UI";
        #endregion

        #region History
        /// <summary>
        /// Gets or sets the size of the history.
        /// </summary>
        /// <value>
        /// The size of the history.
        /// </value>
        public int HistorySize { get; set; } = 5;
        #endregion
    }
}
