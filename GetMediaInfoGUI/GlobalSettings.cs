using System.Windows.Media;

namespace GetMediaInfoGUI
{
    public class GlobalSettings
    {
        private static double? reportFontSize;
        private static FontFamily reportFontFamily;
        private static FixedSizedQueue<string> history;

        public static bool AutoSaveReport { set; get; }

        public static bool DisableSimpleReport { set; get; }

        public static bool DisableCompleteReport { set; get; }

        public static bool DisableTreeReport { set; get; }

        public static bool ShowMilliseconds { set; get; }

        public static bool RemoveJustification { set; get; }

        public static bool ShowCover { get; set; }

        public static bool Bold { set; get; }

        public static bool Italic { set; get; }

        public static double? ReportFontSize
        {
            get
            {
                return new double?(reportFontSize ?? 12.0);
            }
            set
            {
                reportFontSize = value;
            }
        }

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

        public static int HistorySize { get; set; }

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
    }
}
