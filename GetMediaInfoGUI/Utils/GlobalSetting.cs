using System.Windows.Media;

namespace GetMediaInfoGUI
{
    /// <summary>
    /// Struct that contains all global settings
    /// </summary>
    public class GlobalSettings
    {
        #region General        
        /// <summary>
        /// Gets or sets a value indicating whether the standard report needs to be automatically saved to disk or not.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the standard report needs to be automatically saved to disk; otherwise, <c>false</c>.
        /// </value>
        public static bool AutoSaveReport { set; get; }

        /// <summary>
        /// Gets or sets a value indicating whether the simple report is disabled or not.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the simple report is disabled; otherwise, <c>false</c>.
        /// </value>
        public static bool DisableSimpleReport { set; get; }

        /// <summary>
        /// Gets or sets a value indicating whether the complete report is disabled or not.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the complete report is disabled; otherwise, <c>false</c>.
        /// </value>
        public static bool DisableCompleteReport { set; get; }

        /// <summary>
        /// Gets or sets a value indicating whether the tree report is disabled or not.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the tree report is disabled; otherwise, <c>false</c>.
        /// </value>
        public static bool DisableTreeReport { set; get; }

        /// <summary>
        /// Gets or sets a value indicating whether show or hide millisends.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the milliseconds need to be shown; otherwise, <c>false</c>.
        /// </value>
        public static bool ShowMilliseconds { set; get; }

        /// <summary>
        /// Gets or sets a value indicating whether to remove justification in standard and complete report.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the justification must be removed; otherwise, <c>false</c>.
        /// </value>
        public static bool RemoveJustification { set; get; }

        /// <summary>
        /// Gets or sets a value indicating whether to show or hide the cover if it's present.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the cover must be shown if available; otherwise, <c>false</c>.
        /// </value>
        public static bool ShowCover { get; set; }
        #endregion

        #region Appearence        
        /// <summary>
        /// Gets or sets a value indicating whether the report text needs to be bold.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the report text needs to be bold; otherwise, <c>false</c>.
        /// </value>
        public static bool Bold { set; get; }

        /// <summary>
        /// Gets or sets a value indicating whether the report text needs to be italic.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the report text needs to be italic; otherwise, <c>false</c>.
        /// </value>
        public static bool Italic { set; get; }

        private static double? reportFontSize;
        /// <summary>
        /// Gets or sets the size of the report text.
        /// </summary>
        /// <value>
        /// The size of the report text.
        /// </value>
        public static double? ReportFontSize
        {
            get
            {
                return reportFontSize ?? 12D;
            }

            set
            {
                reportFontSize = value;
            }
        }

        private static FontFamily reportFontFamily;
        /// <summary>
        /// Gets or sets the font family of the report text.
        /// </summary>
        /// <value>
        /// The font family of the report text.
        /// </value>
        public static FontFamily ReportFontFamily
        {
            get
            {
                return reportFontFamily ?? new FontFamily("Segoe UI");
            }

            set
            {
                reportFontFamily = value;
            }
        }
        #endregion

        #region History        
        /// <summary>
        /// Gets or sets the size of the history.
        /// </summary>
        /// <value>
        /// The size of the history.
        /// </value>
        public static int HistorySize { get; set; }

        private static FixedSizedQueue<string> history;

        /// <summary>
        /// Gets or sets the recent files collection.
        /// </summary>
        /// <value>
        /// The recent files collection.
        /// </value>
        public static FixedSizedQueue<string> History
        {
            get
            {
                history = history ?? new FixedSizedQueue<string>(5);
                return history;
            }
            set
            {
                history = value;
            }
        }
        #endregion
    }
}
