using GetMediaInfo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using System.Threading.Tasks;
using GetMediaInfoGUI.Extensions;
using GetMediaInfoGUI.Utils;

namespace GetMediaInfoGUI
{
    /// <summary>
    /// File Tab Class binded with XAML interface
    /// </summary>
    /// <seealso cref="INotifyPropertyChanged" />
    public class FileTab : INotifyPropertyChanged, IDisposable
    {
        #region Global Variables
        /// <summary>
        /// Determine whether the Tab has been initializated.
        /// </summary>
        public bool IsReady { get; set; } = false;

        /// <summary>
        /// The MediaInfo object of the file.
        /// </summary>
        public MediaInfo MediaInfo { get; set; }

        /// <summary>
        /// The path of the file.
        /// </summary>
        public readonly string FilePath;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="FileTab" /> class.
        /// </summary>
        /// <param name="FilePath">The path of the file to analyze.</param>
        /// <exception cref="ArgumentException">The Tab cannot be initializated with a file that does not exists.</exception>
        public FileTab(string FilePath)
        {
            if (!File.Exists(FilePath)) { throw new ArgumentException("The Tab cannot be initializated with a file that does not exists."); }

            this.LoadingIconVisibility = Visibility.Collapsed;
            this.FontFamily = GlobalSettings.ReportFontFamily;
            this.FontSize = GlobalSettings.ReportFontSize;

            this.FilePath = FilePath;
            this.HeaderText = Path.GetFileName(FilePath);
        }

        #endregion

        #region UI Binded Properties

        #region TabHeader
        private string headerText;
        /// <summary>
        /// Gets or sets the header text.
        /// </summary>
        /// <value>
        /// The header text.
        /// </value>
        public string HeaderText
        {
            get
            {
                return headerText;
            }

            set
            {
                if (value.Length > 35 && LoadingIconVisibility == Visibility.Collapsed) { value = value.Substring(0, 30) + "..."; }
                headerText = value;
                NotifyPropertyChanged("HeaderText");
            }
        }

        private Visibility loadingIconVisibility = Visibility.Collapsed;
        /// <summary>
        /// Gets or sets the loading icon visibility.
        /// </summary>
        /// <value>
        /// The loading icon visibility.
        /// </value>
        public Visibility LoadingIconVisibility
        {
            get
            {
                return loadingIconVisibility;
            }

            set
            {
                loadingIconVisibility = value;
                NotifyPropertyChanged("LoadingIconVisibility");
            }
        }
        #endregion

        #region Simple Report
        private Visibility simpleReportVisibility = Visibility.Collapsed;
        /// <summary>
        /// Gets or sets the simple report visibility.
        /// </summary>
        /// <value>
        /// The simple report visibility.
        /// </value>
        public Visibility SimpleReportVisibility
        {
            get
            {
                return simpleReportVisibility;
            }
            set
            {
                simpleReportVisibility = value;
                NotifyPropertyChanged("SimpleReportVisibility");
            }
        }

        private BitmapImage simpleReportCover;
        /// <summary>
        /// Gets or sets the simple report cover.
        /// </summary>
        /// <value>
        /// The simple report cover.
        /// </value>
        public BitmapImage SimpleReportCover
        {
            get
            {
                return simpleReportCover;
            }
            set
            {
                simpleReportCover = value;
                if (simpleReportCover != null)
                {
                    NotifyPropertyChanged("SimpleReportCover");
                }
            }
        }

        private GridLength simpleReportCoverWidth = new GridLength(0);
        /// <summary>
        /// Gets or sets the cover width.
        /// </summary>
        /// <value>
        /// Thecover width.
        /// </value>
        public GridLength SimpleReportCoverWidth
        {
            get
            {
                return simpleReportCoverWidth;
            }

            set
            {
                simpleReportCoverWidth = value;
                NotifyPropertyChanged("SimpleReportCoverWidth");
            }
        }

        private string simpleReportGeneralSection;
        /// <summary>
        /// Gets or sets the simple report general section content.
        /// </summary>
        /// <value>
        /// The simple report general section content.
        /// </value>
        public string SimpleReportGeneralSection
        {
            get
            {
                return simpleReportGeneralSection;
            }

            set
            {
                simpleReportGeneralSection = value;
                NotifyPropertyChanged("SimpleReportGeneralSection");
            }
        }

        private Visibility simpleReportGeneralSectionVisibility = Visibility.Collapsed;
        /// <summary>
        /// Gets or sets the simple report general section visibility.
        /// </summary>
        /// <value>
        /// The simple report general section visibility.
        /// </value>
        public Visibility SimpleReportGeneralSectionVisibility
        {
            get
            {
                return simpleReportGeneralSectionVisibility;
            }

            set
            {
                simpleReportGeneralSectionVisibility = value;
                NotifyPropertyChanged("SimpleReportGeneralSectionVisibility");
            }
        }

        private string simpleReportVideoSection;
        /// <summary>
        /// Gets or sets the simple report video section content.
        /// </summary>
        /// <value>
        /// The simple report video section content.
        /// </value>
        public string SimpleReportVideoSection
        {
            get
            {
                return simpleReportVideoSection;
            }

            set
            {
                simpleReportVideoSection = value;
                NotifyPropertyChanged("SimpleReportVideoSection");
            }
        }

        private Visibility simpleReportVideoSectionVisibility = Visibility.Collapsed;
        /// <summary>
        /// Gets or sets the simple report video section visibility.
        /// </summary>
        /// <value>
        /// The simple report video section visibility.
        /// </value>
        public Visibility SimpleReportVideoSectionVisibility
        {
            get
            {
                return simpleReportVideoSectionVisibility;
            }

            set
            {
                simpleReportVideoSectionVisibility = value;
                NotifyPropertyChanged("SimpleReportVideoSectionVisibility");
            }
        }

        private string simpleReportAudioSection;
        /// <summary>
        /// Gets or sets the simple report audio section content.
        /// </summary>
        /// <value>
        /// The simple report audio section content.
        /// </value>        
        public string SimpleReportAudioSection
        {
            get
            {
                return simpleReportAudioSection;
            }

            set
            {
                simpleReportAudioSection = value;
                NotifyPropertyChanged("SimpleReportAudioSection");
            }
        }

        private Visibility simpleReportAudioSectionVisibility = Visibility.Collapsed;
        /// <summary>
        /// Gets or sets the simple report audio section visibility.
        /// </summary>
        /// <value>
        /// The simple report audio section visibility.
        /// </value>
        public Visibility SimpleReportAudioSectionVisibility
        {
            get
            {
                return simpleReportAudioSectionVisibility;
            }

            set
            {
                simpleReportAudioSectionVisibility = value;
                NotifyPropertyChanged("SimpleReportAudioSectionVisibility");
            }
        }

        private string simpleReportTextSection;
        /// <summary>
        /// Gets or sets the simple report text section content.
        /// </summary>
        /// <value>
        /// The simple report text section content.
        /// </value>
        public string SimpleReportTextSection
        {
            get
            {
                return simpleReportTextSection;
            }

            set
            {
                simpleReportTextSection = value;
                NotifyPropertyChanged("SimpleReportTextSection");
            }
        }

        private Visibility simpleReportTextSectionVisibility = Visibility.Collapsed;
        /// <summary>
        /// Gets or sets the simple report text section visibility.
        /// </summary>
        /// <value>
        /// The simple report text section visibility.
        /// </value>
        public Visibility SimpleReportTextSectionVisibility
        {
            get
            {
                return simpleReportTextSectionVisibility;
            }

            set
            {
                simpleReportTextSectionVisibility = value;
                NotifyPropertyChanged("SimpleReportTextSectionVisibility");
            }
        }

        private string simpleReportMenuSection;
        /// <summary>
        /// Gets or sets the simple report menu section content.
        /// </summary>
        /// <value>
        /// The simple report menu section content.
        /// </value>
        public string SimpleReportMenuSection
        {
            get
            {
                return simpleReportMenuSection;
            }

            set
            {
                simpleReportMenuSection = value;
                NotifyPropertyChanged("SimpleReportMenuSection");
            }
        }

        private Visibility simpleReportMenuSectionVisibility = Visibility.Collapsed;
        /// <summary>
        /// Gets or sets the simple report menu section visibility.
        /// </summary>
        /// <value>
        /// The simple report menu section visibility.
        /// </value>
        public Visibility SimpleReportMenuSectionVisibility
        {
            get
            {
                return simpleReportMenuSectionVisibility;
            }

            set
            {
                simpleReportMenuSectionVisibility = value;
                NotifyPropertyChanged("SimpleReportMenuSectionVisibility");
            }
        }

        private string simpleReportImageSection;
        /// <summary>
        /// Gets or sets the simple report image section content.
        /// </summary>
        /// <value>
        /// The simple report image section content.
        /// </value>
        public string SimpleReportImageSection
        {
            get
            {
                return simpleReportImageSection;
            }

            set
            {
                simpleReportImageSection = value;
                NotifyPropertyChanged("SimpleReportImageSection");
            }
        }

        private Visibility simpleReportImageSectionVisibility = Visibility.Collapsed;
        /// <summary>
        /// Gets or sets the simple report image section visibility.
        /// </summary>
        /// <value>
        /// The simple report image section visibility.
        /// </value>
        public Visibility SimpleReportImageSectionVisibility
        {
            get
            {
                return simpleReportImageSectionVisibility;
            }

            set
            {
                simpleReportImageSectionVisibility = value;
                NotifyPropertyChanged("SimpleReportImageSectionVisibility");
            }
        }
        #endregion

        #region Standard Report
        private string standardReport;
        /// <summary>
        /// Gets or sets the standard report content.
        /// </summary>
        /// <value>
        /// The standard report content.
        /// </value>
        public string StandardReport
        {
            get
            {
                return standardReport;
            }

            set
            {
                standardReport = value;
                NotifyPropertyChanged("StandardReport");
            }
        }

        private Visibility standardReportVisibility = Visibility.Visible;
        /// <summary>
        /// Gets or sets the standard report visibility.
        /// </summary>
        /// <value>
        /// The standard report visibility.
        /// </value>
        public Visibility StandardReportVisibility
        {
            get
            {
                return standardReportVisibility;
            }

            set
            {
                standardReportVisibility = value;
                NotifyPropertyChanged("StandardReportVisibility");
            }
        }

        #endregion

        #region Complete Report
        private string completeReport;
        /// <summary>
        /// Gets or sets the complete report content.
        /// </summary>
        /// <value>
        /// The complete report content.
        /// </value>
        public string CompleteReport
        {
            get
            {
                return completeReport;
            }

            set
            {
                completeReport = value;
                NotifyPropertyChanged("CompleteReport");
            }
        }

        private Visibility completeReportVisibility = Visibility.Collapsed;
        /// <summary>
        /// Gets or sets the complete report visibility.
        /// </summary>
        /// <value>
        /// The complete report visibility.
        /// </value>
        public Visibility CompleteReportVisibility
        {
            get
            {
                return completeReportVisibility;
            }

            set
            {
                completeReportVisibility = value;
                NotifyPropertyChanged("CompleteReportVisibility");
            }
        }

        #endregion
        
        #region Tree Report
        private List<TreeViewItem> treeReport;
        /// <summary>
        /// Gets or sets the tree report content.
        /// </summary>
        /// <value>
        /// The tree report content.
        /// </value>
        public List<TreeViewItem> TreeReport
        {
            get
            {
                return treeReport;
            }

            set
            {
                treeReport = value;
                NotifyPropertyChanged("TreeReport");
            }
        }

        private Visibility treeReportVisibility = Visibility.Collapsed;
        /// <summary>
        /// Gets or sets the tree report visibility.
        /// </summary>
        /// <value>
        /// The tree report visibility.
        /// </value>
        public Visibility TreeReportVisibility
        {
            get
            {
                return treeReportVisibility;
            }

            set
            {
                treeReportVisibility = value;
                NotifyPropertyChanged("TreeReportVisibility");
            }
        }

        #endregion

        #region Utils
        private int selectedTabIndex;
        /// <summary>
        /// Gets or sets the index of the selected tab.
        /// </summary>
        /// <value>
        /// The index of the selected tab.
        /// </value>
        public int SelectedTabIndex
        {
            get
            {
                return selectedTabIndex;
            }

            set
            {
                selectedTabIndex = value;
                NotifyPropertyChanged("SelectedTabIndex");
            }
        }

        private TabItem selectedTab;
        /// <summary>
        /// Gets or sets the selected tab.
        /// </summary>
        /// <value>
        /// The selected tab.
        /// </value>
        public TabItem SelectedTab
        {
            get
            {
                return selectedTab;
            }

            set
            {
                selectedTab = value;
                NotifyPropertyChanged("SelectedTab");
            }
        }

        private double? fontSize;
        /// <summary>
        /// Gets or sets the font size.
        /// </summary>
        /// <value>
        /// The font size.
        /// </value>
        public double? FontSize
        {
            get
            {
                return fontSize;
            }

            set
            {
                fontSize = value ?? 12D;
                NotifyPropertyChanged("FontSize");
            }
        }

        private FontFamily fontFamily;
        /// <summary>
        /// Gets or sets the font family.
        /// </summary>
        /// <value>
        /// The font family.
        /// </value>
        public FontFamily FontFamily
        {
            get
            {
                return fontFamily;
            }

            set
            {
                fontFamily = value ?? new FontFamily("Segoe UI");
                NotifyPropertyChanged("FontFamily");
            }
        }

        private FontStyle fontStyle;
        /// <summary>
        /// Gets or sets the font style.
        /// </summary>
        /// <value>
        /// The font style.
        /// </value>
        public FontStyle FontStyle
        {
            get
            {
                return fontStyle;
            }

            set
            {
                if (value == null) { value = FontStyles.Normal; }
                fontStyle = value;
                NotifyPropertyChanged("FontStyle");
            }
        }

        private FontWeight fontWeight;
        /// <summary>
        /// Gets or sets the font weight.
        /// </summary>
        /// <value>
        /// The font weight.
        /// </value>
        public FontWeight FontWeight
        {
            get
            {
                return fontWeight;
            }

            set
            {
                if (value == null) { value = FontWeights.Normal; }
                fontWeight = value;
                NotifyPropertyChanged("FontWeight");
            }
        }
        #endregion

        #endregion

        #region Helper Methods
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public void Initialize()
        {
            IsReady = false;
            LoadingIconVisibility = Visibility.Visible;
#if DEBUG
            Stopwatch sw = new Stopwatch();
            sw.Start();
#endif
            MediaInfo = new MediaInfo(FilePath, parseCoverData: GlobalSettings.ShowCover, parseReports: true);
#if DEBUG
            sw.Stop();
            Debug.WriteLine("File: " + FilePath + " - Analysis time: " + sw.ElapsedMilliseconds + "ms");
#endif
            CreateSimpleReport();
            GetMediaInfoReports();
            CreateTreeReport();

            UpdateReportsVisibility();
            AutoSaveReportToDisk();

            LoadingIconVisibility = Visibility.Collapsed;
            IsReady = true;
        }

        /// <summary>
        /// Updates the UI based on the global application settings.
        /// </summary>
        public void Refresh()
        {
            IsReady = false;
            LoadingIconVisibility = Visibility.Visible;

            FontFamily = GlobalSettings.ReportFontFamily;
            FontSize = GlobalSettings.ReportFontSize;
            FontWeight = GlobalSettings.Bold ? FontWeights.Bold : FontWeights.Normal;
            FontStyle = GlobalSettings.Italic ? FontStyles.Italic : FontStyles.Normal;

            GetMediaInfoReports();
            CreateSimpleReport();
            CreateTreeReport();
            UpdateReportsVisibility();

            LoadingIconVisibility = Visibility.Collapsed;
            IsReady = true;
        }

        /// <summary>
        /// Generates a BitmapImage from the Base64 cover data from GetMediaInfo.
        /// </summary>
        /// <returns>the generated BitmapImage from MediaInfo data.</returns>
        internal BitmapImage GetCoverData()
        {
            try
            {
                GeneralTrack generalTrack = MediaInfo.General.First();
                if (generalTrack.Cover.GetValueOrDefault() && generalTrack.CoverData.Count > 0)
                {
                    BitmapImage image = new BitmapImage();
                    using (MemoryStream mem = new MemoryStream(MediaInfo.General[0].CoverData[0]))
                    {
                        mem.Position = 0;
                        image.BeginInit();
                        image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.UriSource = null;
                        image.StreamSource = mem;
                        image.EndInit();
                    }
                    image.Freeze();

                    return image;
                }
            }
            catch (Exception ex) {
#if DEBUG
                Debug.WriteLine(ex);
                File.WriteAllBytes("debug_image_data.bin", MediaInfo.General[0].CoverData[0]);
#endif
            }

            return null;
        }

        /// <summary>
        /// Saves the report to disk, if the AutoSaveReport checkbox is checked.
        /// </summary>
        private void AutoSaveReportToDisk()
        {
            if (GlobalSettings.AutoSaveReport)
            {
                string ReportName = Path.ChangeExtension(MediaInfo.General[0].CompleteName, ".txt");
                Task.Factory.StartNew(() => {
                    StreamWriter ReportTxt = new StreamWriter(ReportName);
                    ReportTxt.WriteLine(standardReport);
                    ReportTxt.Close();
                });
            }
        }

        /// <summary>
        /// Generate simple report.
        /// </summary>
        private void CreateSimpleReport()
        {
            if (MediaInfo != null && !GlobalSettings.DisableSimpleReport)
            {
                StringBuilder stringBuilder = new StringBuilder();

                #region GeneralTrack
                foreach (GeneralTrack General in MediaInfo.General)
                {
                    // file name
                    if (!string.IsNullOrWhiteSpace(General.FileName) && !string.IsNullOrWhiteSpace(General.FileExtension))
                        stringBuilder.AppendLine("File Name: " + General.FileName + "." + General.FileExtension);

                    // format
                    if (!string.IsNullOrWhiteSpace(General.Format))
                        stringBuilder.AppendLine("Format: " + General.Format);

                    // file size
                    if (!string.IsNullOrWhiteSpace(General.FileSizeString))
                        stringBuilder.AppendLine("File Size: " + General.FileSizeString);

                    // duration
                    if (!string.IsNullOrWhiteSpace(General.DurationTimecode))
                        stringBuilder.AppendLine("Duration: " + ShowHideMilliseconds(General.DurationTimecode));

                    // frame count
                    if (MediaInfo.Video.Count > 0)
                        stringBuilder.AppendLine("Frame Count: " + MediaInfo.Video[0].FrameCount);

                    // bitrate
                    if (!string.IsNullOrWhiteSpace(General.OverallBitRateFormatted))
                        stringBuilder.AppendLine("Overral Bitrate: " + General.OverallBitRateFormatted);

                    // 3D mode
                    if (MediaInfo.Video.Count > 0)
                        if (!string.IsNullOrEmpty(MediaInfo.Video[0].MultiViewLayout))
                            stringBuilder.AppendLine("3D Mode: " + MediaInfo.Video[0].MultiViewLayout);

                    // title
                    if (!string.IsNullOrWhiteSpace(General.Title))
                        stringBuilder.AppendLine("Title: " + General.Title);

                    // album
                    if ((!string.IsNullOrWhiteSpace(General.Album)) && (General.Title != General.Album))
                        stringBuilder.AppendLine("Album: " + General.Album);

                    // artist
                    if (!string.IsNullOrWhiteSpace(General.AlbumPerformer))
                        stringBuilder.AppendLine("Artist: " + General.AlbumPerformer);

                    // genre
                    if (!string.IsNullOrWhiteSpace(General.Genre))
                        stringBuilder.AppendLine("Genre: " + General.Genre);

                    // date encoded
                    if (!string.IsNullOrWhiteSpace(General.EncodedDate))
                        stringBuilder.AppendLine("Date Encoded: " + General.EncodedDate);

                    // encoded library
                    if (!string.IsNullOrWhiteSpace(General.EncodedLibrary))
                        stringBuilder.AppendLine("Created with: " + General.EncodedLibrary);

                }
                stringBuilder.TrimEnd(Environment.NewLine.ToCharArray());
                SimpleReportGeneralSection = stringBuilder.ToString();
                #endregion

                #region VideoTrack
                stringBuilder.Clear();

                foreach (VideoTrack Video in MediaInfo.Video)
                {
                    stringBuilder.Append("> ");

                    // track language
                    if (string.IsNullOrWhiteSpace(Video.LanguageFull))
                        stringBuilder.Append("Undetermined");
                    else
                        stringBuilder.Append(Video.LanguageFull);

                    // track title
                    if (!string.IsNullOrWhiteSpace(Video.Title))
                        stringBuilder.Append(" - [" + Video.Title + "]");

                    // track duration
                    if (!string.IsNullOrWhiteSpace(Video.DurationTimecode))
                        stringBuilder.Append(" - " + ShowHideMilliseconds(Video.DurationTimecode) + " -");
                    else
                        stringBuilder.Append(" -");

                    // track resolution
                    if (Video.Width.HasValue && Video.Height.HasValue)
                        if (Video.Width > 0 && Video.Height > 0)
                            stringBuilder.Append(" " + Video.Width + "×" + Video.Height + ", ");

                    // track codec
                    if (!string.IsNullOrWhiteSpace(Video.FormatCommercial))
                    {
                        if (!string.IsNullOrEmpty(Video.Format) && !Video.FormatCommercial.Equals(Video.Format))
                        {
                            stringBuilder.Append(Video.Format + "/");
                        }
                        stringBuilder.Append(Video.FormatCommercial);

                        if (!string.IsNullOrWhiteSpace(Video.BitDepthFormatted))
                        {
                            stringBuilder.Append( " " + Video.BitDepthFormatted);
                        }
                    }

                    // track format profile & setting
                    if (!string.IsNullOrWhiteSpace(Video.FormatProfile) && !string.IsNullOrWhiteSpace(Video.FormatSettings))
                        stringBuilder.Append(" (" + Video.FormatProfile + ", " + Video.FormatSettings + "), ");
                    else if (!string.IsNullOrWhiteSpace(Video.FormatProfile))
                        stringBuilder.Append(" (" + Video.FormatProfile + "), ");
                    else if (!string.IsNullOrWhiteSpace(Video.FormatSettings))
                        stringBuilder.Append(" (" + Video.FormatSettings + "), ");
                    else
                        stringBuilder.Append(", ");

                    // track bitrate
                    if (!string.IsNullOrWhiteSpace(Video.BitRateFormatted))
                        stringBuilder.Append(Video.BitRateFormatted.Replace(",", ".") + ", ");

                    // track framerate
                    if (Video.FrameRate.HasValue)
                        stringBuilder.Append(Video.FrameRate.Value.ToString(CultureInfo.CreateSpecificCulture("en-US")) + " fps, ");

                    // track aspect ratio
                    if (!string.IsNullOrWhiteSpace(Video.DisplayAspectRatioFormatted))
                        stringBuilder.Append(Video.DisplayAspectRatioFormatted + ", ");

                    //color space
                    if (!string.IsNullOrWhiteSpace(Video.ColorSpace))
                        stringBuilder.Append(Video.ColorSpace + ", ");

                    //chroma subsampling
                    if (!string.IsNullOrWhiteSpace(Video.ChromaSubsampling))
                        stringBuilder.Append(Video.ChromaSubsampling + ", ");

                    //color primaries
                    if (!string.IsNullOrWhiteSpace(Video.ColourPrimaries))
                        stringBuilder.Append(Video.ColourPrimaries + ", ");

                    stringBuilder.Trim(',', ' ', '-');

                    // track delay
                    if (Video.Delay > 0)
                        stringBuilder.Append(", Delay: " + Video.Delay + "ms");

                    // track default
                    if (Video.Default.GetValueOrDefault())
                        stringBuilder.Append(", Default");

                    // track forced
                    if (Video.Forced.GetValueOrDefault())
                        stringBuilder.Append(", Forced");

                    stringBuilder.TrimEnd(',', ' ', '-');
                    stringBuilder.AppendLine();

                    //display additional infos if file is hdr
                    if (IsHDR(Video))
                    {
                        if (!string.IsNullOrWhiteSpace(Video.HDRFormat))
                        {
                            stringBuilder.AppendLine(" > HDR format info:");

                            stringBuilder.Append("   Format: " + Video.HDRFormat);
                            if (!string.IsNullOrWhiteSpace(Video.HDRFormatCommercial))
                            {
                                stringBuilder.AppendLine(" (" + Video.HDRFormatCommercial + ")");
                            } else
                            {
                                stringBuilder.AppendLine();
                            }                               

                            if (!string.IsNullOrWhiteSpace(Video.HDRFormatLevel))
                                stringBuilder.AppendLine("   Level: " + Video.HDRFormatLevel);

                            if (!string.IsNullOrWhiteSpace(Video.HDRFormatLevel))
                                stringBuilder.AppendLine("   Settings: " + Video.HDRFormatLevel);

                            if (!string.IsNullOrWhiteSpace(Video.HDRFormatVersion))
                                stringBuilder.AppendLine("   Version: " + Video.HDRFormatVersion);
                        }

                        var colourRange = Video.ColourRange;
                        var primaries = Video.ColourPrimaries;
                        var transferChar = Video.TransferCharacteristics;
                        var matrixCoeff = Video.MatrixCoefficients;
                        var masteringPrim = Video.MasteringDisplayColorPrimaries;
                        var masteringLum = Video.MasteringDisplayLuminance;
                        var mcll = Video.MaximumContentLightLevel;
                        var mfall = Video.MaximumFrameAverageLightLevel;

                        if (StringUtils.AnyNonEmpty(colourRange, primaries, transferChar, matrixCoeff, masteringPrim, masteringLum, mcll, mfall))
                        {
                            stringBuilder.AppendLine(" > HDR metadata:");

                            if (!string.IsNullOrWhiteSpace(colourRange))
                                stringBuilder.AppendLine("   Color range: " + Video.ColourRange);

                            if (!string.IsNullOrWhiteSpace(primaries))
                                stringBuilder.AppendLine("   Color primaries: " + Video.ColourPrimaries);

                            if (!string.IsNullOrWhiteSpace(transferChar))
                                stringBuilder.AppendLine("   Transfer characteristics: " + Video.TransferCharacteristics);

                            if (!string.IsNullOrWhiteSpace(matrixCoeff))
                                stringBuilder.AppendLine("   Matrix coefficients: " + Video.MatrixCoefficients);

                            if (!string.IsNullOrWhiteSpace(masteringPrim))
                                stringBuilder.AppendLine("   Mastering display color primaries: " + Video.MasteringDisplayColorPrimaries);

                            if (!string.IsNullOrWhiteSpace(masteringLum))
                                stringBuilder.AppendLine("   Mastering display luminance: " + Video.MasteringDisplayLuminance);

                            if (!string.IsNullOrWhiteSpace(mcll))
                                stringBuilder.AppendLine("   Maximum content light level: " + Video.MaximumContentLightLevel);

                            if (!string.IsNullOrWhiteSpace(mfall))
                                stringBuilder.AppendLine("   Maximum frame-average light level: " + Video.MaximumFrameAverageLightLevel);
                        }
                    }
                }
                stringBuilder.TrimEnd(Environment.NewLine.ToCharArray());
                SimpleReportVideoSection = stringBuilder.ToString();
                #endregion

                #region AudioTrack
                stringBuilder.Clear();

                foreach (AudioTrack Audio in MediaInfo.Audio)
                {
                    //fix for E-AC3 or TrueHD + AC3 info
                    if (Audio.Format.Contains("AC-3"))
                    {
                        if (Audio.FormatWithExtra.Equals("E-AC-3")) //E-AC3
                        {
                            Audio.Format = "EAC-3";
                            Audio.FormatProfile = "";
                        } else if (Audio.FormatAdditionalFeatures.Equals("MLP FBA 16-ch")) //Atmos + AC3
                        {
                            Audio.Format = "";
                            Audio.FormatProfile = "TrueHD Atmos / AC-3";
                            Audio.BitDepthFormatted = " / 16 bits";
                            Audio.SamplingRateFormatted += " / " + Audio.SamplingRateFormatted;
                            Audio.BitRateMode += " / CBR";
                            Audio.BitRateFormatted = Audio.BitRateMaximumFormatted + " / " + Audio.BitRateFormatted;
                            Audio.ChannelsFormatted += " (object-based) / 6 channels";
                        }
                    }
                    //fix for TrueHD info
                    if (Audio.Format.Contains("MLP FBA"))
                    {
                        if (!string.IsNullOrEmpty(Audio.FormatAdditionalFeatures)) //Atmos
                        {
                            Audio.Format = "TrueHD Atmos";
                            Audio.FormatProfile = "";
                            Audio.ChannelsFormatted += " (object-based)";
                        } else //Core
                        {
                            Audio.Format = "TrueHD";
                            Audio.FormatProfile = "";
                        }
                    }
                    //fix for DTS info
                    if (Audio.Format.Contains("DTS"))
                    {
                        if (Audio.FormatAdditionalFeatures.Contains("XLL X"))
                        {
                            Audio.Format = "DTS-X";
                            Audio.ChannelsFormatted += " (object-based)";
                        } else if (Audio.FormatAdditionalFeatures.Contains("XLL"))
                        {
                            Audio.Format = "DTS-HD MA";
                        } else if (Audio.FormatAdditionalFeatures.Contains("XBR"))
                        {
                            Audio.Format = "DTS-HD HR";
                        } else if (Audio.FormatAdditionalFeatures.Contains("ES"))
                        {
                            Audio.Format = "DTS-ES";
                        }
                    }

                    stringBuilder.Append("> ");

                    // track language
                    if (string.IsNullOrEmpty(Audio.LanguageFull))
                        stringBuilder.Append("Undetermined");
                    else
                        stringBuilder.Append(Audio.LanguageFull);

                    // track title
                    if (!string.IsNullOrWhiteSpace(Audio.Title))
                        stringBuilder.Append(" - [" + Audio.Title + "]");

                    // track duration
                    if (!string.IsNullOrWhiteSpace(Audio.DurationTimecode))
                        stringBuilder.Append(" - " + ShowHideMilliseconds(Audio.DurationTimecode) + " - ");
                    else
                        stringBuilder.Append(" - ");

                    // subtrack number
                    int numberofsubtracks = 0;
                    if (!string.IsNullOrWhiteSpace(Audio.FormatProfile))
                        numberofsubtracks = Audio.FormatProfile.Split(new string[] { " / " }, StringSplitOptions.None).Length;
                    else if (!string.IsNullOrWhiteSpace(Audio.Format))
                        numberofsubtracks = Audio.Format.Split(new string[] { " / " }, StringSplitOptions.None).Length;

                    // subtrack infos
                    if (numberofsubtracks > 1)
                    {
                        foreach (int j in Enumerable.Range(0, numberofsubtracks))
                        {
                            // add tab if not are first subtrack
                            if (j > 0)
                                stringBuilder.Append('\t');

                            // subtrack format
                            if (!string.IsNullOrWhiteSpace(GetSubTracks(Audio.Format, j)))
                                stringBuilder.Append(GetSubTracks(Audio.Format, j));

                            // subtrack format profile
                            if (!string.IsNullOrWhiteSpace(GetSubTracks(Audio.FormatProfile, j)))
                            {
                                if (string.IsNullOrWhiteSpace(stringBuilder.ToString().Split('\r').Last()) || 
                                    string.IsNullOrWhiteSpace(Audio.Format))
                                    stringBuilder.Append(GetSubTracks(Audio.FormatProfile, j));
                                else
                                    stringBuilder.Append("-" + GetSubTracks(Audio.FormatProfile, j));
                            }

                            // subtrack bitrate
                            if (!string.IsNullOrWhiteSpace(GetSubTracks(Audio.BitRateFormatted, j)))
                                stringBuilder.Append(", " + GetSubTracks(Audio.BitRateFormatted, j));

                            // subtrack bitrate mode  
                            if (!string.IsNullOrEmpty(GetSubTracks(Audio.BitRateMode, j)))
                                stringBuilder.Append(", " + GetSubTracks(Audio.BitRateMode, j));

                            // subtrack channels
                            if (!string.IsNullOrEmpty(GetSubTracks(Audio.ChannelsFormatted, j)))
                                stringBuilder.Append(", " + GetSubTracks(Audio.ChannelsFormatted, j));

                            // subtrack bit depth
                            if (!string.IsNullOrEmpty(GetSubTracks(Audio.BitDepthFormatted, j)))
                                stringBuilder.Append(", " + GetSubTracks(Audio.BitDepthFormatted, j));

                            // subtrack sampling rate
                            if (!string.IsNullOrEmpty(GetSubTracks(Audio.SamplingRateFormatted, j)))
                                stringBuilder.Append(", " + GetSubTracks(Audio.SamplingRateFormatted, j));

                            stringBuilder.Trim(',', ' ');

                            // only for the radix track
                            if (j == 0)
                            {
                                // track delay
                                if (Audio.Delay > 0)
                                    stringBuilder.Append(", Delay: " + Audio.Delay + "ms");

                                // track default
                                if (Audio.Default.GetValueOrDefault())
                                    stringBuilder.Append(", Default");

                                // track forced
                                if (Audio.Forced.GetValueOrDefault())
                                    stringBuilder.Append(", Forced");
                            }
                            stringBuilder.TrimEnd(',', ' ');
                            stringBuilder.AppendLine();
                        }
                    }
                    else
                    {
                        // track format
                        if (!string.IsNullOrWhiteSpace(Audio.Format))
                            stringBuilder.Append(Audio.Format + " - ");

                        // track format profile
                        if (!string.IsNullOrWhiteSpace(Audio.FormatProfile))
                            stringBuilder.Append(Audio.FormatProfile + ", ");

                        // track bitrate
                        if (!string.IsNullOrWhiteSpace(Audio.BitRateFormatted))
                            stringBuilder.Append(Audio.BitRateFormatted + ", ");

                        // track bitrate mode
                        if (!string.IsNullOrWhiteSpace(Audio.BitRateMode))
                            stringBuilder.Append(Audio.BitRateMode + ", ");

                        // track channels
                        if (!string.IsNullOrWhiteSpace(Audio.ChannelsFormatted))
                            stringBuilder.Append(Audio.ChannelsFormatted + ", ");

                        // track bit depth
                        if (!string.IsNullOrWhiteSpace(Audio.BitDepthFormatted))
                            stringBuilder.Append(Audio.BitDepthFormatted + ", ");

                        // track sample rate
                        if (!string.IsNullOrWhiteSpace(Audio.SamplingRateFormatted))
                            stringBuilder.Append(Audio.SamplingRateFormatted);

                        stringBuilder.Trim(',', ' ');

                        // track delay
                        if (Audio.Delay > 0)
                            stringBuilder.Append(", Delay: " + Audio.Delay + "ms");

                        // track default
                        if (Audio.Default.GetValueOrDefault())
                            stringBuilder.Append(", Default");

                        // track forced
                        if (Audio.Forced.GetValueOrDefault())
                            stringBuilder.Append(", Forced");

                        stringBuilder.TrimEnd(',', ' ');
                        stringBuilder.AppendLine();
                    }
                }
                stringBuilder.TrimEnd(Environment.NewLine.ToCharArray());
                SimpleReportAudioSection = stringBuilder.ToString();
                #endregion

                #region TextTrack
                stringBuilder.Clear();

                foreach (TextTrack Text in MediaInfo.Text)
                {
                    stringBuilder.Append("> ");

                    // track language
                    if (string.IsNullOrWhiteSpace(Text.LanguageFull))
                        stringBuilder.Append("Undetermined");
                    else
                        stringBuilder.Append(Text.LanguageFull);

                    // track title
                    if (!string.IsNullOrWhiteSpace(Text.Title))
                        stringBuilder.Append(" - [" + Text.Title + "]");

                    // track codec
                    if (!string.IsNullOrWhiteSpace(Text.FormatCommercial))
                        stringBuilder.Append(" - " + Text.FormatCommercial);

                    // delay
                    if (Text.Delay.HasValue)
                        if (Text.Delay > 0)
                            stringBuilder.AppendLine(", Delay: " + Text.Delay + "ms");

                    // track default
                    if (Text.Default.GetValueOrDefault())
                        stringBuilder.Append(", Default");

                    // track forced
                    if (Text.Forced.GetValueOrDefault())
                        stringBuilder.Append(", Forced");

                    stringBuilder.Trim();
                    stringBuilder.AppendLine();
                }
                stringBuilder.TrimEnd(Environment.NewLine.ToCharArray());
                SimpleReportTextSection = stringBuilder.ToString();
                #endregion

                #region MenuTrack
                stringBuilder.Clear();

                foreach (MenuTrack Menu in MediaInfo.Menu)
                {
                    // track id and count
                    stringBuilder.Append("> Menu " + Menu.ID + "(" + Menu.ChaptersCount + " chapters)");

                    // track language
                    if (!string.IsNullOrWhiteSpace(Menu.LanguageFull))
                        stringBuilder.Append(" - " + Menu.LanguageFull);

                    // track title
                    if (!string.IsNullOrWhiteSpace(Menu.Title))
                        stringBuilder.Append(" - [" + Menu.Title + "]");

                    // track default
                    if (Menu.Default.GetValueOrDefault())
                        stringBuilder.Append(", Default");

                    // track forced
                    if (Menu.Forced.GetValueOrDefault())
                        stringBuilder.Append(", Forced");

                    stringBuilder.Trim();
                    stringBuilder.AppendLine();

                    foreach (ChapterTrack Chapter in Menu.Chapters)
                    {
                        // chapter index
                        stringBuilder.Append("  " + (Chapter.Index + 1) + " - ");

                        // chapter title     
                        if (!ValidateChapterTitle(Chapter.Timecode, Chapter.Title, ':', '.', ' '))
                            stringBuilder.Append(Chapter.Title.Trim(':') + ": ");

                        // chapter timecode
                        stringBuilder.Append(Chapter.Timecode);

                        // chapter language
                        if (Chapter.Language != ":0" && !string.IsNullOrEmpty(Chapter.Language))
                            stringBuilder.Append(" (" + Chapter.Language + ")");

                        stringBuilder.AppendLine();
                    }
                }
                stringBuilder.TrimEnd(Environment.NewLine.ToCharArray());
                SimpleReportMenuSection = stringBuilder.ToString();
                #endregion

                #region ImageTrack
                stringBuilder.Clear();

                foreach (ImageTrack Image in MediaInfo.Image)
                {
                    stringBuilder.Append("> ");
                    // track format
                    if (!string.IsNullOrWhiteSpace(Image.Format))
                        stringBuilder.Append(Image.Format + " (");

                    // track format compression
                    if (!string.IsNullOrWhiteSpace(Image.FormatCompression))
                        stringBuilder.Append(Image.FormatCompression + " ");

                    // track format profile
                    if (!string.IsNullOrWhiteSpace(Image.FormatProfile))
                        stringBuilder.Append(Image.FormatProfile);

                    stringBuilder.TrimEnd();

                    if (stringBuilder.Last() == '>')
                    {
                        stringBuilder.Append(" ");
                    } else
                    {
                        if (stringBuilder.Last() != '(')
                        {
                            stringBuilder.Append(") - ");
                        }
                        else
                        {
                            stringBuilder.Length -= 2;
                            stringBuilder.Append(" - ");
                        }
                    }
                
                    // track resolution
                    if (Image.Width.HasValue && Image.Height.HasValue)
                        if (Image.Width > 0 && Image.Height > 0)
                            stringBuilder.Append(Image.Width + "×" + Image.Height + ", ");

                    // track compression mode
                    if (!string.IsNullOrWhiteSpace(Image.CompressionMode))
                        stringBuilder.Append(Image.CompressionMode + ", ");

                    // track bit depth
                    if (!string.IsNullOrWhiteSpace(Image.BitDepthFormatted))
                        stringBuilder.Append(Image.BitDepthFormatted + ", ");

                    // track color space
                    if (!string.IsNullOrWhiteSpace(Image.ColorSpace))
                        stringBuilder.Append(Image.ColorSpace + ", ");

                    // track chroma subsampling
                    if (!string.IsNullOrWhiteSpace(Image.ChromaSubsampling))
                        stringBuilder.Append(Image.ChromaSubsampling);

                    stringBuilder.TrimEnd(' ', ',');

                    stringBuilder.AppendLine();
                }
                stringBuilder.TrimEnd(Environment.NewLine.ToCharArray());
                SimpleReportImageSection = stringBuilder.ToString();

                #endregion

                //Cover
                if (GlobalSettings.ShowCover)
                {
                    SimpleReportCover = GetCoverData();
                    if (SimpleReportCover != null)
                    {
                        SimpleReportCoverWidth = new GridLength(pixels: 155);
                    }
                    else
                    {
                        SimpleReportCoverWidth = new GridLength(pixels: 0);
                    }
                }

                //Collapsed if no track available
                SimpleReportGeneralSectionVisibility = (MediaInfo.General.Count == 0) ? Visibility.Collapsed : Visibility.Visible;
                SimpleReportVideoSectionVisibility = (MediaInfo.Video.Count == 0) ? Visibility.Collapsed : Visibility.Visible;
                SimpleReportAudioSectionVisibility = (MediaInfo.Audio.Count == 0) ? Visibility.Collapsed : Visibility.Visible;
                SimpleReportTextSectionVisibility = (MediaInfo.Text.Count == 0) ? Visibility.Collapsed : Visibility.Visible;
                SimpleReportMenuSectionVisibility = (MediaInfo.Menu.Count == 0) ? Visibility.Collapsed : Visibility.Visible;
                SimpleReportImageSectionVisibility = (MediaInfo.Image.Count == 0) ? Visibility.Collapsed: Visibility.Visible;
            }
        }

        /// <summary>
        /// Generate tree report.
        /// </summary>
        private void CreateTreeReport()
        {
            if (MediaInfo != null && !GlobalSettings.DisableTreeReport)
            {
                List<TreeViewItem> treeReport = new List<TreeViewItem>();
                int currIndex = 0;
                int currCategoryIndex = 0;

                #region GeneralTrack
                foreach (GeneralTrack General in MediaInfo.General)
                {
                    // header
                    treeReport.Add(new TreeViewItem { Header = "General" });

                    // file name
                    if (!string.IsNullOrWhiteSpace(General.FileName) && !string.IsNullOrWhiteSpace(General.FileExtension))
                        treeReport[currIndex].Children.Add(new TreeViewItem { Header = "File Name: " + General.FileName + "." + General.FileExtension });

                    // format
                    if (!string.IsNullOrWhiteSpace(General.Format))
                        treeReport[currIndex].Children.Add(new TreeViewItem { Header = "Format: " + General.Format });

                    // file size
                    if (!string.IsNullOrWhiteSpace(General.FileSizeString))
                        treeReport[currIndex].Children.Add(new TreeViewItem { Header = "File Size: " + General.FileSizeString });

                    // duration
                    if (!string.IsNullOrWhiteSpace(ShowHideMilliseconds(General.DurationTimecode)))
                        treeReport[currIndex].Children.Add(new TreeViewItem { Header = "Duration: " + ShowHideMilliseconds(General.DurationTimecode) });

                    // frame count
                    if (MediaInfo.Video.Count > 0)
                        if (MediaInfo.Video[0].FrameCount > 0)
                            treeReport[currIndex].Children.Add(new TreeViewItem { Header = "Frame Count: " + MediaInfo.Video[0].FrameCount });

                    // bitrate
                    if (!string.IsNullOrWhiteSpace(General.OverallBitRateFormatted))
                        treeReport[currIndex].Children.Add(new TreeViewItem { Header = "Overal BitRate: " + General.OverallBitRateFormatted });

                    // 3d mode
                    if (MediaInfo.Video.Count > 0)
                        if (!string.IsNullOrEmpty(MediaInfo.Video[0].MultiViewLayout))
                            treeReport[currIndex].Children.Add(new TreeViewItem { Header = "3D Mode: " + MediaInfo.Video[0].MultiViewLayout });
                    // title
                    if (!string.IsNullOrWhiteSpace(General.Title))
                        treeReport[currIndex].Children.Add(new TreeViewItem { Header = "Title: " + General.Title });

                    // album
                    if ((!string.IsNullOrWhiteSpace(General.Album)) && (General.Title != General.Album))
                        treeReport[currIndex].Children.Add(new TreeViewItem { Header = "Album: " + General.Album });

                    // artist
                    if (!string.IsNullOrWhiteSpace(General.AlbumPerformer))
                        treeReport[currIndex].Children.Add(new TreeViewItem { Header = "Artist: " + General.AlbumPerformer });

                    // genre
                    if (!string.IsNullOrWhiteSpace(General.Genre))
                        treeReport[currIndex].Children.Add(new TreeViewItem { Header = "Genre: " + General.Genre });

                    // cover
                    treeReport[currIndex].Children.Add(new TreeViewItem { Header = "Cover: " + (General.Cover.GetValueOrDefault() ? "Yes" : "No") });

                    // date encoded
                    if (!string.IsNullOrWhiteSpace(General.EncodedDate))
                        treeReport[currIndex].Children.Add(new TreeViewItem { Header = "Date Encoded: " + General.EncodedDate });

                    // encoded library
                    if (!string.IsNullOrWhiteSpace(General.EncodedLibrary))
                        treeReport[currIndex].Children.Add(new TreeViewItem { Header = "Created with: " + General.EncodedLibrary });

                    currIndex++;
                }
                #endregion

                #region VideoTrack
                foreach (VideoTrack Video in MediaInfo.Video)
                {
                    // track header
                    if (MediaInfo.Video.Count == 1)
                        treeReport.Add(new TreeViewItem { Header = "Video" });
                    else
                        treeReport.Add(new TreeViewItem { Header = "Video #" + ++currCategoryIndex });

                    // track language
                    if (string.IsNullOrWhiteSpace(Video.LanguageFull))
                        treeReport[currIndex].Children.Add(new TreeViewItem { Header = "Undetermined Language" });
                    else
                        treeReport[currIndex].Children.Add(new TreeViewItem { Header = "Language: " + Video.LanguageFull });

                    // track title
                    if (!string.IsNullOrWhiteSpace(Video.Title))
                        treeReport[currIndex].Children.Add(new TreeViewItem { Header = "Track Title: " + Video.Title });

                    // track resolution
                    if (Video.Width.HasValue && Video.Height.HasValue)
                        if (Video.Width > 0 && Video.Height > 0)
                            treeReport[currIndex].Children.Add(new TreeViewItem { Header = "Resolution: " + Video.Width + "×" + Video.Height });

                    // track aspect ratio
                    if (!string.IsNullOrWhiteSpace(Video.DisplayAspectRatioFormatted))
                        treeReport[currIndex].Children.Add(new TreeViewItem { Header = "Aspect Ratio: " + Video.DisplayAspectRatioFormatted });

                    // track framerate
                    if (Video.FrameRate.HasValue)
                        treeReport[currIndex].Children.Add(new TreeViewItem { Header = "Framerate: " + Video.FrameRate.Value.ToString(CultureInfo.CreateSpecificCulture("en-US")) + " fps" });

                    // track bitrate
                    if (!string.IsNullOrWhiteSpace(Video.BitRateFormatted))
                        treeReport[currIndex].Children.Add(new TreeViewItem { Header = "BitRate: " + Video.BitRateFormatted });

                    // track codec
                    if (!string.IsNullOrWhiteSpace(Video.FormatCommercial))
                        treeReport[currIndex].Children.Add(new TreeViewItem { Header = "Codec Video: " + (Video.FormatCommercial + " (" + Video.FormatProfile.ToString() + ", " + Video.FormatSettings.ToString()).TrimEnd(',', ' ') + ")" });

                    //color space
                    if (!string.IsNullOrWhiteSpace(Video.ColorSpace))
                        treeReport[currIndex].Children.Add(new TreeViewItem { Header = "Color Space: " + Video.ColorSpace });

                    //chroma subsampling
                    if (!string.IsNullOrWhiteSpace(Video.ChromaSubsampling))
                        treeReport[currIndex].Children.Add(new TreeViewItem { Header = "Chroma Subsampling: " + Video.ChromaSubsampling });

                    //color primaries
                    if (!string.IsNullOrWhiteSpace(Video.ColourPrimaries))
                        treeReport[currIndex].Children.Add(new TreeViewItem { Header = "Color Primaries: " + Video.ColourPrimaries });

                    // track delay
                    if (Video.Delay > 0)
                        treeReport[currIndex].Children.Add(new TreeViewItem { Header = "Delay: " + Video.Delay + "ms" });

                    // track default
                    if (!string.IsNullOrWhiteSpace(Video.DefaultString))
                        treeReport[currIndex].Children.Add(new TreeViewItem { Header = "Default: " + (Video.Default.GetValueOrDefault() ? "Yes" : "No") });

                    // track forced
                    if (!string.IsNullOrWhiteSpace(Video.ForcedString))
                        treeReport[currIndex].Children.Add(new TreeViewItem { Header = "Forced: " + (Video.Forced.GetValueOrDefault() ? "Yes" : "No") });

                    currIndex++;
                }
                #endregion

                #region AudioTrack
                currCategoryIndex = 0;
                foreach (AudioTrack Audio in MediaInfo.Audio)
                {
                    // track header
                    if (MediaInfo.Audio.Count == 1)
                        treeReport.Add(new TreeViewItem { Header = "Audio" });
                    else
                        treeReport.Add(new TreeViewItem { Header = "Audio #" + ++currCategoryIndex });

                    // track language
                    if (string.IsNullOrWhiteSpace(Audio.LanguageFull))
                        treeReport[currIndex].Children.Add(new TreeViewItem { Header = "Undetermined Language" });
                    else
                        treeReport[currIndex].Children.Add(new TreeViewItem { Header = "Language: " + Audio.LanguageFull });

                    // track title
                    if (!string.IsNullOrWhiteSpace(Audio.Title))
                        treeReport[currIndex].Children.Add(new TreeViewItem { Header = "Track Title: " + Audio.Title });

                    // track format
                    if (!string.IsNullOrWhiteSpace(Audio.Format))
                        treeReport[currIndex].Children.Add(new TreeViewItem { Header = "Format: " + Audio.Format.Trim(' ') });

                    // subtracks number
                    int NumberOfSubTracks = 0;
                    if (!string.IsNullOrWhiteSpace(Audio.FormatProfile))
                        NumberOfSubTracks = Audio.FormatProfile.Split(new string[] { " / " }, StringSplitOptions.None).Length;
                    else if (!string.IsNullOrWhiteSpace(Audio.Format))
                        NumberOfSubTracks = Audio.Format.Split(new string[] { " / " }, StringSplitOptions.None).Length;

                    // subtrack infos
                    if (NumberOfSubTracks > 1)
                    {
                        int Start = treeReport[currIndex].Children.Count();

                        foreach (int IndexSubTrack in Enumerable.Range(0, NumberOfSubTracks))
                        {
                            // subtrack format, format profile and header
                            if ((!string.IsNullOrWhiteSpace(GetSubTracks(Audio.Format, IndexSubTrack))) && (!string.IsNullOrWhiteSpace(GetSubTracks(Audio.FormatProfile, IndexSubTrack))))
                                treeReport[currIndex].Children.Add(new TreeViewItem { Header = GetSubTracks(Audio.Format, IndexSubTrack) + "-" + GetSubTracks(Audio.FormatProfile, IndexSubTrack) });
                            else if (!string.IsNullOrWhiteSpace(GetSubTracks(Audio.Format, IndexSubTrack)))
                                treeReport[currIndex].Children.Add(new TreeViewItem { Header = GetSubTracks(Audio.Format, IndexSubTrack) });
                            else if (!string.IsNullOrWhiteSpace(GetSubTracks(Audio.FormatProfile, IndexSubTrack)))
                                treeReport[currIndex].Children.Add(new TreeViewItem { Header = GetSubTracks(Audio.FormatProfile, IndexSubTrack) });
                            else
                                treeReport[currIndex].Children.Add(new TreeViewItem { Header = $"Subtrack {IndexSubTrack + 1}" });

                            string bitratemode = GetSubTracks(Audio.BitRateMode, IndexSubTrack);

                            // subtrack bitrate
                            if (!string.IsNullOrWhiteSpace(GetSubTracks(Audio.BitRateFormatted, IndexSubTrack)))
                            {
                                if (bitratemode.Equals("CBR"))
                                {
                                    treeReport[currIndex].Children[Start + IndexSubTrack].Children.Add(new TreeViewItem { Header = "BitRate: " + GetSubTracks(Audio.BitRateFormatted, IndexSubTrack) });
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(Audio.BitRateMaximumFormatted))
                                        treeReport[currIndex].Children[Start + IndexSubTrack].Children.Add(new TreeViewItem { Header = "BitRate: " + Audio.BitRateMaximumFormatted });
                                }
                            }

                            // subtrack bitrate mode
                            if (!string.IsNullOrWhiteSpace(GetSubTracks(Audio.BitRateMode, IndexSubTrack)))
                                treeReport[currIndex].Children[Start + IndexSubTrack].Children.Add(new TreeViewItem { Header = "BitRate Mode: " + bitratemode });

                            // subtrack channel
                            if (!string.IsNullOrWhiteSpace(GetSubTracks(Audio.ChannelsFormatted, IndexSubTrack)))
                                treeReport[currIndex].Children[Start + IndexSubTrack].Children.Add(new TreeViewItem { Header = "Channel: " + GetSubTracks(Audio.ChannelsFormatted, IndexSubTrack) });

                            // subtrack bit depth
                            if (!string.IsNullOrWhiteSpace(GetSubTracks(Audio.BitDepthFormatted, IndexSubTrack)))
                                treeReport[currIndex].Children[Start + IndexSubTrack].Children.Add(new TreeViewItem { Header = "Bit Depth: " + GetSubTracks(Audio.BitDepthFormatted, IndexSubTrack) });

                            // subtrack sampling rate
                            if (!string.IsNullOrWhiteSpace(GetSubTracks(Audio.SamplingRateFormatted, IndexSubTrack)))
                                treeReport[currIndex].Children[Start + IndexSubTrack].Children.Add(new TreeViewItem { Header = "Sampling Rate: " + GetSubTracks(Audio.SamplingRateFormatted, IndexSubTrack) });
                        }
                    }
                    else
                    {
                        // track profile
                        if (!string.IsNullOrEmpty(Audio.FormatProfile))
                            treeReport[currIndex].Children.Add(new TreeViewItem { Header = "Profile: " + Audio.FormatProfile });

                        // track bitrate
                        if (!string.IsNullOrEmpty(Audio.BitRateFormatted))
                            treeReport[currIndex].Children.Add(new TreeViewItem { Header = "BitRate: " + Audio.BitRateFormatted });

                        // track bitrate mode
                        if (!string.IsNullOrEmpty(Audio.BitRateMode))
                            treeReport[currIndex].Children.Add(new TreeViewItem { Header = "BitRate Mode: " + Audio.BitRateMode });

                        // track channels
                        if (!string.IsNullOrEmpty(Audio.ChannelsFormatted))
                            treeReport[currIndex].Children.Add(new TreeViewItem { Header = "Channels: " + Audio.ChannelsFormatted });

                        // track sampling rate
                        if (!string.IsNullOrEmpty(Audio.SamplingRateFormatted))
                            treeReport[currIndex].Children.Add(new TreeViewItem { Header = "Sampling Rate: " + Audio.SamplingRateFormatted });

                    }

                    // track delay
                    if (Audio.Delay > 0)
                        treeReport[currIndex].Children.Add(new TreeViewItem { Header = "Delay: " + Audio.Delay + "ms" });

                    // track default
                    if (!string.IsNullOrEmpty(Audio.DefaultString))
                        treeReport[currIndex].Children.Add(new TreeViewItem { Header = "Default: " + Audio.DefaultString });

                    // track forced
                    if (!string.IsNullOrEmpty(Audio.ForcedString))
                        treeReport[currIndex].Children.Add(new TreeViewItem { Header = "Forced: " + Audio.ForcedString });

                    currIndex++;
                }
                #endregion

                #region TextTrack 
                currCategoryIndex = 0;
                foreach (TextTrack Text in MediaInfo.Text)
                {
                    // header
                    if (MediaInfo.Text.Count == 1)
                        treeReport.Add(new TreeViewItem { Header = "Text" });
                    else
                        treeReport.Add(new TreeViewItem { Header = "Text #" + ++currCategoryIndex });

                    // track language
                    if (string.IsNullOrEmpty(Text.LanguageFull))
                        treeReport[currIndex].Children.Add(new TreeViewItem { Header = "Undetermined Language" });
                    else
                        treeReport[currIndex].Children.Add(new TreeViewItem { Header = "Language: " + Text.LanguageFull });

                    // track title
                    if (!string.IsNullOrEmpty(Text.Title))
                        treeReport[currIndex].Children.Add(new TreeViewItem { Header = "Track Title: " + Text.Title });

                    // track codec
                    if (!string.IsNullOrWhiteSpace(Text.FormatCommercial))
                        treeReport[currIndex].Children.Add(new TreeViewItem { Header = "Codec: " + Text.FormatCommercial });

                    currIndex++;
                }
                #endregion

                #region MenuTrack
                currCategoryIndex = 0;
                foreach (MenuTrack Menu in MediaInfo.Menu)
                {
                    // header
                    if (MediaInfo.Menu.Count == 1)
                        treeReport.Add(new TreeViewItem { Header = "Menu" });
                    else
                        treeReport.Add(new TreeViewItem { Header = "Menu #" + ++currCategoryIndex });

                    // track count
                    treeReport[currIndex].Children.Add(new TreeViewItem { Header = "Chapters Count: " + Menu.ChaptersCount });

                    if (!string.IsNullOrWhiteSpace(Menu.LanguageFull))
                        treeReport[currIndex].Children.Add(new TreeViewItem { Header = "Language: " + Menu.LanguageFull });

                    // track default
                    if (Menu.Default.GetValueOrDefault())
                        treeReport[currIndex].Children.Add(new TreeViewItem { Header = "Default: Yes" });
                    else
                        treeReport[currIndex].Children.Add(new TreeViewItem { Header = "Default: No" });

                    // track forced
                    if (Menu.Forced.GetValueOrDefault())
                        treeReport[currIndex].Children.Add(new TreeViewItem { Header = "Forced: Yes" });
                    else
                        treeReport[currIndex].Children.Add(new TreeViewItem { Header = "Forced: No" });

                    // chapter list
                    if (Menu.Chapters.Count == 1)
                    {
                        int IndexChapter = treeReport[currIndex].Children.Count();
                        // header
                        treeReport[currIndex].Children.Add(new TreeViewItem { Header = "Chapter" });


                        // chapter title if it is different from the timecode
                        if (!ValidateChapterTitle(Menu.Chapters[0].Timecode, Menu.Chapters[0].Title, ':', '.', ' '))
                            treeReport[currIndex].Children[IndexChapter].Children.Add(new TreeViewItem { Header = "Title: " + Menu.Chapters[0].Title.Trim(':') });

                        // chapter timecode
                        treeReport[currIndex].Children[IndexChapter].Children.Add(new TreeViewItem { Header = "Time: " + Menu.Chapters[0].Timecode });

                        // chapter language
                        if (Menu.Chapters[0].Language != "0" && !string.IsNullOrEmpty(Menu.Chapters[0].Language))
                            treeReport[currIndex].Children[IndexChapter].Children.Add(new TreeViewItem { Header = "Language: " + Menu.Chapters[0].Language });
                        else
                            treeReport[currIndex].Children[IndexChapter].Children.Add(new TreeViewItem { Header = "Language: Undetermined" });
                    }
                    else
                    {
                        int IndexChapters = treeReport[currIndex].Children.Count();
                        int IndexChapter = 0;
                        // header
                        treeReport[currIndex].Children.Add(new TreeViewItem { Header = "Chapters" });

                        foreach (ChapterTrack Chapter in Menu.Chapters)
                        {
                            // chapter header
                            treeReport[currIndex].Children[IndexChapters].Children.Add(new TreeViewItem { Header = "Chapter #" + (Chapter.Index + 1) });

                            // chapter title if it is different from the timecode
                            if (!ValidateChapterTitle(Chapter.Timecode, Chapter.Title, ':', '.', ' '))
                                treeReport[currIndex].Children[IndexChapters].Children[IndexChapter].Children.Add(new TreeViewItem { Header = "Title: " + Chapter.Title.Trim(':') });

                            // chapter timecode
                            treeReport[currIndex].Children[IndexChapters].Children[IndexChapter].Children.Add(new TreeViewItem { Header = "Time: " + Chapter.Timecode });

                            // chapter language
                            if (Chapter.Language != ":0" && !string.IsNullOrEmpty(Chapter.Language))
                                treeReport[currIndex].Children[IndexChapters].Children[IndexChapter].Children.Add(new TreeViewItem { Header = "Language: " + Menu.Chapters[0].Language });
                            else
                                treeReport[currIndex].Children[IndexChapters].Children[IndexChapter].Children.Add(new TreeViewItem { Header = "Language: Undetermined" });

                            IndexChapter++;
                        }
                    }
                    currIndex++;
                }
                #endregion

                #region ImageTrack
                currCategoryIndex = 0;
                foreach (ImageTrack Image in MediaInfo.Image)
                {
                    // header
                    if (MediaInfo.Image.Count == 1)
                        treeReport.Add(new TreeViewItem { Header = "Image" });
                    else
                        treeReport.Add(new TreeViewItem { Header = "Image #" + ++currCategoryIndex });
                    // track format
                    if (!string.IsNullOrWhiteSpace(Image.Format))
                        treeReport[currIndex].Children.Add(new TreeViewItem { Header = "Format: " + Image.Format });

                    // track format compression
                    if (!string.IsNullOrWhiteSpace(Image.FormatCompression))
                        treeReport[currIndex].Children.Add(new TreeViewItem { Header = "Format compression: " + Image.FormatCompression });

                    // track format profile
                    if (!string.IsNullOrWhiteSpace(Image.FormatProfile))
                        treeReport[currIndex].Children.Add(new TreeViewItem { Header = "Format compression: " + Image.FormatProfile });

                    // track resolution
                    if (Image.Width > 0 && Image.Height > 0)
                        treeReport[currIndex].Children.Add(new TreeViewItem { Header = "Resolution: " + Image.Width + "×" + Image.Height });

                    // track compression mode
                    if (!string.IsNullOrWhiteSpace(Image.CompressionMode))
                        treeReport[currIndex].Children.Add(new TreeViewItem { Header = "Compression mode: " + Image.CompressionMode });

                    // track bit depth
                    if (!string.IsNullOrWhiteSpace(Image.BitDepthFormatted))
                        treeReport[currIndex].Children.Add(new TreeViewItem { Header = "Bit depth: " + Image.BitDepthFormatted });

                    // track color space
                    if (!string.IsNullOrWhiteSpace(Image.ColorSpace))
                        treeReport[currIndex].Children.Add(new TreeViewItem { Header = "Color space: " + Image.ColorSpace });

                    // track chroma subsampling
                    if (!string.IsNullOrWhiteSpace(Image.ChromaSubsampling))
                        treeReport[currIndex].Children.Add(new TreeViewItem { Header = "Chroma subsampling: " + Image.ChromaSubsampling });


                    currIndex++;
                }
                #endregion

                TreeReport = treeReport;
            }
        }

        /// <summary>
        /// Fill this istance properties with teh data provided by GetMediaInfo library
        /// </summary>
        private void GetMediaInfoReports()
        {
            if (MediaInfo != null)
            {
                if (GlobalSettings.RemoveJustification)
                {
                    StandardReport = MediaInfo.InfoStandard.Replace("  ", "").Replace(" : ", ": ");
                    CompleteReport = MediaInfo.InfoComplete.Replace("  ", "").Replace(" : ", ": ");
                }
                else
                {
                    StandardReport = MediaInfo.InfoStandard;
                    CompleteReport = MediaInfo.InfoComplete;
                }

                if (!string.IsNullOrEmpty(CompleteReport)) // remove inform from complete report
                {
                    if (CompleteReport.Contains("Inform"))
                    {
                        int InformStart = CompleteReport.IndexOf("Inform");
                        while (InformStart > 0)
                        {
                            int Count = CompleteReport.IndexOf(Environment.NewLine, InformStart) - InformStart;
                            if (Count > 0)
                                CompleteReport = CompleteReport.Remove(InformStart, Count + Environment.NewLine.Length);

                            InformStart = CompleteReport.IndexOf("Inform", InformStart);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Show/Hide reports based on the global settings.
        /// </summary>
        private void UpdateReportsVisibility()
        {
            SimpleReportVisibility = GlobalSettings.DisableSimpleReport ? Visibility.Collapsed : Visibility.Visible;
            CompleteReportVisibility = GlobalSettings.DisableCompleteReport ? Visibility.Collapsed : Visibility.Visible;
            TreeReportVisibility = GlobalSettings.DisableTreeReport ? Visibility.Collapsed : Visibility.Visible;

            SelectedTabIndex = GlobalSettings.DisableSimpleReport ? 1 : 0;
        }

        /// <summary>
        /// Show/Hide milliseconds in durations if EnableMilliseconds checkbox is Checked/Dechecked
        /// </summary>
        /// <param name="time">the timecode string.</param>
        /// <returns>the timecode with/without milliseconds.</returns>
        private string ShowHideMilliseconds(string time)
        {
            return GlobalSettings.ShowMilliseconds ? time : time.Split('.')[0];
        }

        /// <summary>
        /// Get sub tracks information of an input audio track.
        /// </summary>
        /// <param name="str">The MediaInfo properties of a selected track.</param>
        /// <param name="index">Index of sub track to consider.</param>
        /// <returns>String containing the information about the sub track at the specified input index.</returns>
        private string GetSubTracks(string str, int index)
        {
            if (str.Contains(" / "))
            {
                string[] SubTracks = str.Split(new string[] { " / " }, StringSplitOptions.None);
                if (SubTracks.Length > index)
                {
                    if (!string.IsNullOrWhiteSpace(SubTracks[index]))
                    {
                        if (SubTracks[index].Contains("Core"))
                            return string.Empty;
                        else if (SubTracks[index].Contains("HR") || SubTracks[index].Contains("MA"))
                            return "HD " + SubTracks[index];
                        else return SubTracks[index].Trim(' ');
                    }
                }
                else
                {
                    return SubTracks.Last().Trim(' ');
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(str)) return str;
            }
            return string.Empty;
        }

        /// <summary>
        /// Check if the title equals the chapter timecode or viceversa. If the input strings are less than 3 chars long return false.
        /// To avoid that some numbers are mistaken with the timecode with certain cultures, some characters are replaced with "--".
        /// </summary>
        /// <param name="title">The title of chapter.</param>
        /// <param name="timecode">The timecode of chapter.</param>
        /// <returns>True if the title equals the timecode or viceversa.</returns>
        public bool ValidateChapterTitle(string title, string timecode, params char[] ignorechar)
        {
            if (title.Length < 3 || timecode.Length < 3) return false;
            ignorechar.ToList().ForEach(c => { title = title.Replace(c.ToString(), "--"); timecode = timecode.Replace(c.ToString(), "--"); });

            return (title.Contains(timecode) || timecode.Contains(title));
        }

        /// <summary>
        /// Determine whether the input file has HDR content or not.
        /// </summary>
        /// <param name="infos">MediaInfo object o the file.</param>
        /// <returns>True, if the video track is HDR, otherwise false.</returns>
        private bool IsHDR(VideoTrack infos)
        {
            if (infos == null)
            {
                return false;
            }

            if (infos.BitDepth.HasValue && infos.BitDepth.Value >= 10)
            {
                if ((!string.IsNullOrEmpty(infos.FormatCommercial) && infos.FormatCommercial.Contains("HDR")) ||
                    (!string.IsNullOrEmpty(infos.Format) && infos.Format.Contains("HEVC")))
                {
                    if (!string.IsNullOrEmpty(infos.ColourPrimaries) && infos.ColourPrimaries.Contains("BT.2020") ||
                        !string.IsNullOrEmpty(infos.HDRFormat))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            IsReady = false;
            MediaInfo?.Dispose();
            MediaInfo = null;
        }

        #endregion

        #region INotifyPropertyChanged

        /// <summary>
        /// Generated when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notifies the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
