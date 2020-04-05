using GetMediaInfo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GetMediaInfoGUI
{
    public class FileTab : INotifyPropertyChanged
    {
        private Visibility loadingIconVisibility = Visibility.Collapsed;
        private Visibility simpleReportVisibility = Visibility.Collapsed;
        private GridLength simpleReportCoverWidth = new GridLength(0.0);
        private Visibility simpleReportGeneralSectionVisibility = Visibility.Collapsed;
        private Visibility simpleReportVideoSectionVisibility = Visibility.Collapsed;
        private Visibility simpleReportAudioSectionVisibility = Visibility.Collapsed;
        private Visibility simpleReportTextSectionVisibility = Visibility.Collapsed;
        private Visibility simpleReportMenuSectionVisibility = Visibility.Collapsed;
        private Visibility simpleReportImageSectionVisibility = Visibility.Collapsed;
        private Visibility completeReportVisibility = Visibility.Collapsed;
        private Visibility treeReportVisibility = Visibility.Collapsed;
        public readonly string FilePath;
        private string headerText;
        private BitmapImage simpleReportCover;
        private string simpleReportGeneralSection;
        private string simpleReportVideoSection;
        private string simpleReportAudioSection;
        private string simpleReportTextSection;
        private string simpleReportMenuSection;
        private string simpleReportImageSection;
        private string standardReport;
        private Visibility standardReportVisibility;
        private string completeReport;
        private List<TreeViewItem> treeReport;
        private int selectedTabIndex;
        private TabItem selectedTab;
        private double? fontSize;
        private FontFamily fontFamily;
        private FontStyle fontStyle;
        private FontWeight fontWeight;

        public bool IsReady { get; set; }

        public MediaInfo MediaInfo { get; set; }

        public FileTab(string FilePath)
        {
            if (!File.Exists(FilePath))
                throw new ArgumentException("The Tab cannot be initializated with a file that does not exists.");
            LoadingIconVisibility = Visibility.Collapsed;
            FontFamily = GlobalSettings.ReportFontFamily;
            FontSize = GlobalSettings.ReportFontSize;
            this.FilePath = FilePath;
            HeaderText = Path.GetFileName(FilePath);
        }

        public string HeaderText
        {
            get
            {
                return headerText;
            }
            set
            {
                if (value.Length > 35 && LoadingIconVisibility == Visibility.Collapsed)
                    value = value.Substring(0, 30) + "...";
                headerText = value;
                NotifyPropertyChanged(nameof(HeaderText));
            }
        }

        public Visibility LoadingIconVisibility
        {
            get
            {
                return loadingIconVisibility;
            }
            set
            {
                loadingIconVisibility = value;
                NotifyPropertyChanged(nameof(LoadingIconVisibility));
            }
        }

        public Visibility SimpleReportVisibility
        {
            get
            {
                return simpleReportVisibility;
            }
            set
            {
                simpleReportVisibility = value;
                NotifyPropertyChanged(nameof(SimpleReportVisibility));
            }
        }

        public BitmapImage SimpleReportCover
        {
            get
            {
                return simpleReportCover;
            }
            set
            {
                simpleReportCover = value;
                if (simpleReportCover == null)
                    return;
                NotifyPropertyChanged(nameof(SimpleReportCover));
            }
        }

        public GridLength SimpleReportCoverWidth
        {
            get
            {
                return simpleReportCoverWidth;
            }
            set
            {
                simpleReportCoverWidth = value;
                NotifyPropertyChanged(nameof(SimpleReportCoverWidth));
            }
        }

        public string SimpleReportGeneralSection
        {
            get
            {
                return simpleReportGeneralSection;
            }
            set
            {
                simpleReportGeneralSection = value;
                NotifyPropertyChanged(nameof(SimpleReportGeneralSection));
            }
        }

        public Visibility SimpleReportGeneralSectionVisibility
        {
            get
            {
                return simpleReportGeneralSectionVisibility;
            }
            set
            {
                simpleReportGeneralSectionVisibility = value;
                NotifyPropertyChanged(nameof(SimpleReportGeneralSectionVisibility));
            }
        }

        public string SimpleReportVideoSection
        {
            get
            {
                return simpleReportVideoSection;
            }
            set
            {
                simpleReportVideoSection = value;
                NotifyPropertyChanged(nameof(SimpleReportVideoSection));
            }
        }

        public Visibility SimpleReportVideoSectionVisibility
        {
            get
            {
                return simpleReportVideoSectionVisibility;
            }
            set
            {
                simpleReportVideoSectionVisibility = value;
                NotifyPropertyChanged(nameof(SimpleReportVideoSectionVisibility));
            }
        }

        public string SimpleReportAudioSection
        {
            get
            {
                return simpleReportAudioSection;
            }
            set
            {
                simpleReportAudioSection = value;
                NotifyPropertyChanged(nameof(SimpleReportAudioSection));
            }
        }

        public Visibility SimpleReportAudioSectionVisibility
        {
            get
            {
                return simpleReportAudioSectionVisibility;
            }
            set
            {
                simpleReportAudioSectionVisibility = value;
                NotifyPropertyChanged(nameof(SimpleReportAudioSectionVisibility));
            }
        }

        public string SimpleReportTextSection
        {
            get
            {
                return simpleReportTextSection;
            }
            set
            {
                simpleReportTextSection = value;
                NotifyPropertyChanged(nameof(SimpleReportTextSection));
            }
        }

        public Visibility SimpleReportTextSectionVisibility
        {
            get
            {
                return simpleReportTextSectionVisibility;
            }
            set
            {
                simpleReportTextSectionVisibility = value;
                NotifyPropertyChanged(nameof(SimpleReportTextSectionVisibility));
            }
        }

        public string SimpleReportMenuSection
        {
            get
            {
                return simpleReportMenuSection;
            }
            set
            {
                simpleReportMenuSection = value;
                NotifyPropertyChanged(nameof(SimpleReportMenuSection));
            }
        }

        public Visibility SimpleReportMenuSectionVisibility
        {
            get
            {
                return simpleReportMenuSectionVisibility;
            }
            set
            {
                simpleReportMenuSectionVisibility = value;
                NotifyPropertyChanged(nameof(SimpleReportMenuSectionVisibility));
            }
        }

        public string SimpleReportImageSection
        {
            get
            {
                return simpleReportImageSection;
            }
            set
            {
                simpleReportImageSection = value;
                NotifyPropertyChanged(nameof(SimpleReportImageSection));
            }
        }

        public Visibility SimpleReportImageSectionVisibility
        {
            get
            {
                return simpleReportImageSectionVisibility;
            }
            set
            {
                simpleReportImageSectionVisibility = value;
                NotifyPropertyChanged(nameof(SimpleReportImageSectionVisibility));
            }
        }

        public string StandardReport
        {
            get
            {
                return standardReport;
            }
            set
            {
                standardReport = value;
                NotifyPropertyChanged(nameof(StandardReport));
            }
        }

        public Visibility StandardReportVisibility
        {
            get
            {
                return standardReportVisibility;
            }
            set
            {
                standardReportVisibility = value;
                NotifyPropertyChanged(nameof(StandardReportVisibility));
            }
        }

        public string CompleteReport
        {
            get
            {
                return completeReport;
            }
            set
            {
                completeReport = value;
                NotifyPropertyChanged(nameof(CompleteReport));
            }
        }

        public Visibility CompleteReportVisibility
        {
            get
            {
                return completeReportVisibility;
            }
            set
            {
                completeReportVisibility = value;
                NotifyPropertyChanged(nameof(CompleteReportVisibility));
            }
        }

        public List<TreeViewItem> TreeReport
        {
            get
            {
                return treeReport;
            }
            set
            {
                treeReport = value;
                NotifyPropertyChanged(nameof(TreeReport));
            }
        }

        public Visibility TreeReportVisibility
        {
            get
            {
                return treeReportVisibility;
            }
            set
            {
                treeReportVisibility = value;
                NotifyPropertyChanged(nameof(TreeReportVisibility));
            }
        }

        public int SelectedTabIndex
        {
            get
            {
                return selectedTabIndex;
            }
            set
            {
                selectedTabIndex = value;
                NotifyPropertyChanged(nameof(SelectedTabIndex));
            }
        }

        public TabItem SelectedTab
        {
            get
            {
                return selectedTab;
            }
            set
            {
                selectedTab = value;
                NotifyPropertyChanged(nameof(SelectedTab));
            }
        }

        public double? FontSize
        {
            get
            {
                return fontSize;
            }
            set
            {
                fontSize = new double?(value ?? 12.0);
                NotifyPropertyChanged(nameof(FontSize));
            }
        }

        public FontFamily FontFamily
        {
            get
            {
                return fontFamily;
            }
            set
            {
                fontFamily = value ?? new FontFamily("Segoe UI");
                NotifyPropertyChanged(nameof(FontFamily));
            }
        }

        public FontStyle FontStyle
        {
            get
            {
                return fontStyle;
            }
            set
            {
                fontStyle = value;
                NotifyPropertyChanged(nameof(FontStyle));
            }
        }

        public FontWeight FontWeight
        {
            get
            {
                return fontWeight;
            }
            set
            {
                fontWeight = value;
                NotifyPropertyChanged(nameof(FontWeight));
            }
        }

        public void Initialize()
        {
            IsReady = false;
            LoadingIconVisibility = Visibility.Visible;
            MediaInfo = new MediaInfo(FilePath, GlobalSettings.ShowCover, true);
            CreateSimpleReport();
            GetMediaInfoReports();
            CreateTreeReport();
            UpdateReportsVisibility();
            AutoSaveReportToDisk();
            LoadingIconVisibility = Visibility.Collapsed;
            IsReady = true;
        }

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

        internal BitmapImage GetCoverData()
        {
            try
            {
                GeneralTrack generalTrack = MediaInfo.General.First<GeneralTrack>();
                if (generalTrack.Cover.GetValueOrDefault())
                {
                    if (generalTrack.CoverData.Count > 0)
                    {
                        BitmapImage bitmapImage = new BitmapImage();
                        using (MemoryStream memoryStream = new MemoryStream(MediaInfo.General[0].CoverData[0]))
                        {
                            memoryStream.Position = 0L;
                            bitmapImage.BeginInit();
                            bitmapImage.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                            bitmapImage.UriSource = null;
                            bitmapImage.StreamSource = memoryStream;
                            bitmapImage.EndInit();
                        }
                        bitmapImage.Freeze();
                        return bitmapImage;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        private void AutoSaveReportToDisk()
        {
            if (!GlobalSettings.AutoSaveReport)
                return;
            string ReportName = Path.ChangeExtension(MediaInfo.General[0].CompleteName, ".txt");
            Task.Factory.StartNew(() =>
            {
                StreamWriter streamWriter = new StreamWriter(ReportName);
                streamWriter.WriteLine(standardReport);
                streamWriter.Close();
            });
        }

        private void CreateSimpleReport()
        {
            if (MediaInfo == null || GlobalSettings.DisableSimpleReport)
                return;
            StringBuilder StringBuilder = new StringBuilder();
            foreach (GeneralTrack generalTrack in MediaInfo.General)
            {
                if (!string.IsNullOrWhiteSpace(generalTrack.FileName) && !string.IsNullOrWhiteSpace(generalTrack.FileExtension))
                    StringBuilder.AppendLine("File Name: " + generalTrack.FileName + "." + generalTrack.FileExtension);
                if (!string.IsNullOrWhiteSpace(generalTrack.Format))
                    StringBuilder.AppendLine("Format: " + generalTrack.Format.OrIfNullOrEmpty(generalTrack.FileExtension.ToUpper()).AppendIfNotNullOrEmpty((generalTrack.FormatProfile + ", " + generalTrack.FormatSettings).Trim(',', ' '), " (", ")"));
                if (!string.IsNullOrWhiteSpace(generalTrack.FileSizeString))
                    StringBuilder.AppendLine("File Size: " + generalTrack.FileSizeString);
                if (!string.IsNullOrWhiteSpace(generalTrack.DurationTimecode))
                    StringBuilder.AppendLine("Duration: " + ShowHideMilliseconds(generalTrack.DurationTimecode));
                if (MediaInfo.Video.Count > 0)
                    StringBuilder.AppendLine("Frame Count: " + MediaInfo.Video[0].FrameCount);
                if (!string.IsNullOrWhiteSpace(generalTrack.OverallBitRateFormatted))
                    StringBuilder.AppendLine("Overral Bitrate: " + generalTrack.OverallBitRateFormatted);
                if (MediaInfo.Video.Count > 0 && !string.IsNullOrEmpty(MediaInfo.Video[0].MultiViewLayout))
                    StringBuilder.AppendLine("3D Mode: " + MediaInfo.Video[0].MultiViewLayout);
                if (!string.IsNullOrWhiteSpace(generalTrack.Title))
                    StringBuilder.AppendLine("Title: " + generalTrack.Title);
                if (!string.IsNullOrWhiteSpace(generalTrack.Album) && generalTrack.Title != generalTrack.Album)
                    StringBuilder.AppendLine("Album: " + generalTrack.Album);
                if (!string.IsNullOrWhiteSpace(generalTrack.AlbumPerformer))
                    StringBuilder.AppendLine("Artist: " + generalTrack.AlbumPerformer);
                if (!string.IsNullOrWhiteSpace(generalTrack.Genre))
                    StringBuilder.AppendLine("Genre: " + generalTrack.Genre);
                if (!string.IsNullOrWhiteSpace(generalTrack.EncodedDate))
                    StringBuilder.AppendLine("Date Encoded: " + generalTrack.EncodedDate);
                if (!string.IsNullOrWhiteSpace(generalTrack.EncodedLibrary))
                    StringBuilder.AppendLine("Created with: " + generalTrack.EncodedLibrary);
            }
            StringBuilder.TrimEnd(Environment.NewLine.ToCharArray());
            SimpleReportGeneralSection = StringBuilder.ToString();
            StringBuilder.Clear();
            foreach (VideoTrack infos in MediaInfo.Video)
            {
                StringBuilder.Append("> ");
                if (string.IsNullOrWhiteSpace(infos.LanguageFull))
                    StringBuilder.Append("Undetermined");
                else
                    StringBuilder.Append(infos.LanguageFull);
                if (!string.IsNullOrWhiteSpace(infos.Title))
                    StringBuilder.Append(" - [" + infos.Title + "]");
                if (!string.IsNullOrWhiteSpace(infos.DurationTimecode))
                    StringBuilder.Append(" - " + ShowHideMilliseconds(infos.DurationTimecode) + " -");
                else
                    StringBuilder.Append(" -");
                int? nullable = infos.Width;
                if (nullable.HasValue)
                {
                    nullable = infos.Height;
                    if (nullable.HasValue)
                    {
                        nullable = infos.Width;
                        int num1 = 0;
                        if ((nullable.GetValueOrDefault() > num1 ? (nullable.HasValue ? 1 : 0) : 0) != 0)
                        {
                            nullable = infos.Height;
                            int num2 = 0;
                            if ((nullable.GetValueOrDefault() > num2 ? (nullable.HasValue ? 1 : 0) : 0) != 0)
                                StringBuilder.Append(" " + infos.Width + "×" + infos.Height + ", ");
                        }
                    }
                }
                if (!string.IsNullOrWhiteSpace(infos.FormatCommercial))
                {
                    if (!string.IsNullOrEmpty(infos.Format) && !infos.FormatCommercial.Equals(infos.Format))
                        StringBuilder.Append(infos.Format + "/");
                    StringBuilder.Append(infos.FormatCommercial);
                    if (!string.IsNullOrWhiteSpace(infos.BitDepthFormatted))
                        StringBuilder.Append(" " + infos.BitDepthFormatted);
                }
                if (!string.IsNullOrWhiteSpace(infos.FormatProfile) && !string.IsNullOrWhiteSpace(infos.FormatSettings))
                    StringBuilder.Append(" (" + infos.FormatProfile + ", " + infos.FormatSettings + "), ");
                else if (!string.IsNullOrWhiteSpace(infos.FormatProfile))
                    StringBuilder.Append(" (" + infos.FormatProfile + "), ");
                else if (!string.IsNullOrWhiteSpace(infos.FormatSettings))
                    StringBuilder.Append(" (" + infos.FormatSettings + "), ");
                else
                    StringBuilder.Append(", ");
                if (!string.IsNullOrWhiteSpace(infos.BitRateFormatted))
                    StringBuilder.Append(infos.BitRateFormatted.Replace(",", ".") + ", ");
                double? frameRate = infos.FrameRate;
                if (frameRate.HasValue)
                {
                    StringBuilder stringBuilder = StringBuilder;
                    frameRate = infos.FrameRate;
                    string str = frameRate.Value.ToString(CultureInfo.CreateSpecificCulture("en-US")) + " fps, ";
                    stringBuilder.Append(str);
                }
                if (!string.IsNullOrWhiteSpace(infos.DisplayAspectRatioFormatted))
                    StringBuilder.Append(infos.DisplayAspectRatioFormatted + ", ");
                if (!string.IsNullOrWhiteSpace(infos.ColorSpace))
                    StringBuilder.Append(infos.ColorSpace + ", ");
                if (!string.IsNullOrWhiteSpace(infos.ChromaSubsampling))
                    StringBuilder.Append(infos.ChromaSubsampling + ", ");
                if (!string.IsNullOrWhiteSpace(infos.ColourPrimaries))
                    StringBuilder.Append(infos.ColourPrimaries + ", ");
                StringBuilder.Trim(',', ' ', '-');
                nullable = infos.Delay;
                int num3 = 0;
                if ((nullable.GetValueOrDefault() > num3 ? (nullable.HasValue ? 1 : 0) : 0) != 0)
                    StringBuilder.Append(", Delay: " + infos.Delay + "ms");
                if (infos.Default.GetValueOrDefault())
                    StringBuilder.Append(", Default");
                if (infos.Forced.GetValueOrDefault())
                    StringBuilder.Append(", Forced");
                StringBuilder.TrimEnd(',', ' ', '-');
                StringBuilder.AppendLine();
                if (IsHDR(infos))
                {
                    StringBuilder.AppendLine(" > HDR metadata:");
                    if (!string.IsNullOrWhiteSpace(infos.ColourRange))
                        StringBuilder.AppendLine("     Color range: " + infos.ColourRange);
                    if (!string.IsNullOrWhiteSpace(infos.ChromaSubsampling))
                        StringBuilder.AppendLine("     Chroma subsampling: " + infos.ChromaSubsampling);
                    if (!string.IsNullOrWhiteSpace(infos.TransferCharacteristics))
                        StringBuilder.AppendLine("     Transfer characteristics: " + infos.TransferCharacteristics);
                    if (!string.IsNullOrWhiteSpace(infos.MatrixCoefficients))
                        StringBuilder.AppendLine("     Matrix coefficients: " + infos.MatrixCoefficients);
                    if (!string.IsNullOrWhiteSpace(infos.MasteringDisplayColorPrimaries))
                    {
                        string str1 = "     Mastering display color primaries:";
                        StringBuilder.AppendLine(str1);
                        string displayColorPrimaries = infos.MasteringDisplayColorPrimaries;
                        char[] separator = new char[1] { ',' };
                        int num1 = 1;
                        foreach (string str2 in displayColorPrimaries.Split(separator, (StringSplitOptions)num1))
                            StringBuilder.AppendLine("     " + str2.Replace("White point", "WP").Trim());
                    }
                    if (!string.IsNullOrWhiteSpace(infos.MasteringDisplayLuminance))
                    {
                        string str1 = "     Mastering display luminance:";
                        StringBuilder.AppendLine(str1);
                        string displayLuminance = infos.MasteringDisplayLuminance;
                        char[] separator = new char[1] { ',' };
                        int num1 = 1;
                        foreach (string str2 in displayLuminance.Split(separator, (StringSplitOptions)num1))
                            StringBuilder.AppendLine("     " + str2.Trim());
                    }
                    if (!string.IsNullOrWhiteSpace(infos.MaximumContentLightLevel))
                        StringBuilder.AppendLine("     Maximum content light level (MCLL): " + infos.MaximumContentLightLevel);
                    if (!string.IsNullOrWhiteSpace(infos.MaximumFrameAverageLightLevel))
                        StringBuilder.AppendLine("     Maximum frame-average light level (FALL): " + infos.MaximumFrameAverageLightLevel);
                }
            }
            StringBuilder.TrimEnd(Environment.NewLine.ToCharArray());
            SimpleReportVideoSection = StringBuilder.ToString();
            StringBuilder.Clear();
            foreach (AudioTrack audioTrack in MediaInfo.Audio)
            {
                if (audioTrack.FormatProfile.Contains("Atmos") && audioTrack.FormatProfile.Contains("AC-3"))
                {
                    audioTrack.Format = "";
                    audioTrack.FormatProfile = "Atmos / TrueHD / AC-3";
                    audioTrack.BitDepthFormatted = "24 bit / 24 bit / 16 bit";
                    string samplingRateFormatted = audioTrack.SamplingRateFormatted;
                    if (samplingRateFormatted.StartsWith(" / "))
                        audioTrack.SamplingRateFormatted = samplingRateFormatted.Substring(samplingRateFormatted.LastIndexOf(" / ") + 3, samplingRateFormatted.Length - (samplingRateFormatted.LastIndexOf(" / ") + 3)) + samplingRateFormatted;
                    string bitRateMode = audioTrack.BitRateMode;
                    if (bitRateMode.Split('/').Length != 3 && bitRateMode.Contains<char>('/'))
                        audioTrack.BitRateMode = bitRateMode.Substring(0, bitRateMode.IndexOf(" / ")) + " / " + bitRateMode;
                    string bitRateFormatted = audioTrack.BitRateFormatted;
                    if (bitRateFormatted.Split('/').Length != 3 && bitRateMode.Contains<char>('/'))
                        audioTrack.BitRateFormatted = " / / " + bitRateFormatted;
                }
                else if (audioTrack.FormatProfile.Contains("Atmos") && audioTrack.Format.Contains("TrueHD"))
                {
                    audioTrack.Format = "";
                    audioTrack.FormatProfile = "Atmos / TrueHD";
                    audioTrack.BitDepthFormatted = "24 bit / 24 bit";
                    string samplingRateFormatted = audioTrack.SamplingRateFormatted;
                    if (samplingRateFormatted.StartsWith(" /"))
                        audioTrack.SamplingRateFormatted = samplingRateFormatted.Replace(" / ", "") + samplingRateFormatted;
                    string bitRateMode = audioTrack.BitRateMode;
                    if (bitRateMode.Split('/').Length != 2 && bitRateMode.Contains<char>('/'))
                        audioTrack.BitRateMode = bitRateMode.Substring(0, bitRateMode.IndexOf(" / ")) + " / " + bitRateMode;
                    string bitRateFormatted = audioTrack.BitRateFormatted;
                    if (bitRateFormatted.Split('/').Length != 2 && bitRateMode.Contains<char>('/'))
                        audioTrack.BitRateFormatted = " / " + bitRateFormatted;
                }
                StringBuilder.Append("> ");
                if (string.IsNullOrEmpty(audioTrack.LanguageFull))
                    StringBuilder.Append("Undetermined");
                else
                    StringBuilder.Append(audioTrack.LanguageFull);
                if (!string.IsNullOrWhiteSpace(audioTrack.Title))
                    StringBuilder.Append(" - [" + audioTrack.Title + "]");
                if (!string.IsNullOrWhiteSpace(audioTrack.DurationTimecode))
                    StringBuilder.Append(" - " + ShowHideMilliseconds(audioTrack.DurationTimecode) + " - ");
                else
                    StringBuilder.Append(" - ");
                int count = 0;
                if (!string.IsNullOrWhiteSpace(audioTrack.FormatProfile))
                    count = audioTrack.FormatProfile.Split(new string[1]
                    {
            " / "
                    }, StringSplitOptions.None).Length;
                else if (!string.IsNullOrWhiteSpace(audioTrack.Format))
                    count = audioTrack.Format.Split(new string[1]
                    {
            " / "
                    }, StringSplitOptions.None).Length;
                int? delay;
                if (count > 1)
                {
                    foreach (int index in Enumerable.Range(0, count))
                    {
                        if (index > 0)
                            StringBuilder.Append('\t');
                        if (!string.IsNullOrWhiteSpace(GetSubTracks(audioTrack.Format, index)))
                            StringBuilder.Append(GetSubTracks(audioTrack.Format, index));
                        if (!string.IsNullOrWhiteSpace(GetSubTracks(audioTrack.FormatProfile, index)))
                        {
                            if (string.IsNullOrWhiteSpace(StringBuilder.ToString().Split('\r').Last<string>()) || string.IsNullOrWhiteSpace(audioTrack.Format))
                                StringBuilder.Append(GetSubTracks(audioTrack.FormatProfile, index));
                            else
                                StringBuilder.Append("-" + GetSubTracks(audioTrack.FormatProfile, index));
                        }
                        if (!string.IsNullOrWhiteSpace(GetSubTracks(audioTrack.BitRateFormatted, index)))
                            StringBuilder.Append(", " + GetSubTracks(audioTrack.BitRateFormatted, index));
                        if (!string.IsNullOrEmpty(GetSubTracks(audioTrack.BitRateMode, index)))
                            StringBuilder.Append(", " + GetSubTracks(audioTrack.BitRateMode, index));
                        if (!string.IsNullOrEmpty(GetSubTracks(audioTrack.ChannelsFormatted, index)))
                            StringBuilder.Append(", " + GetSubTracks(audioTrack.ChannelsFormatted, index));
                        if (!string.IsNullOrEmpty(GetSubTracks(audioTrack.BitDepthFormatted, index)))
                            StringBuilder.Append(", " + GetSubTracks(audioTrack.BitDepthFormatted, index));
                        if (!string.IsNullOrEmpty(GetSubTracks(audioTrack.SamplingRateFormatted, index)))
                            StringBuilder.Append(", " + GetSubTracks(audioTrack.SamplingRateFormatted, index));
                        StringBuilder.Trim(',', ' ');
                        if (index == 0)
                        {
                            delay = audioTrack.Delay;
                            int num = 0;
                            if ((delay.GetValueOrDefault() > num ? (delay.HasValue ? 1 : 0) : 0) != 0)
                                StringBuilder.Append(", Delay: " + audioTrack.Delay + "ms");
                            if (audioTrack.Default.GetValueOrDefault())
                                StringBuilder.Append(", Default");
                            if (audioTrack.Forced.GetValueOrDefault())
                                StringBuilder.Append(", Forced");
                        }
                        StringBuilder.TrimEnd(',', ' ');
                        StringBuilder.AppendLine();
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(audioTrack.Format))
                        StringBuilder.Append(audioTrack.Format + " - ");
                    if (!string.IsNullOrWhiteSpace(audioTrack.FormatProfile))
                        StringBuilder.Append(audioTrack.FormatProfile + ", ");
                    if (!string.IsNullOrWhiteSpace(audioTrack.BitRateFormatted))
                        StringBuilder.Append(audioTrack.BitRateFormatted + ", ");
                    if (!string.IsNullOrWhiteSpace(audioTrack.BitRateMode))
                        StringBuilder.Append(audioTrack.BitRateMode + ", ");
                    if (!string.IsNullOrWhiteSpace(audioTrack.ChannelsFormatted))
                        StringBuilder.Append(audioTrack.ChannelsFormatted + ", ");
                    if (!string.IsNullOrWhiteSpace(audioTrack.BitDepthFormatted))
                        StringBuilder.Append(audioTrack.BitDepthFormatted + ", ");
                    if (!string.IsNullOrWhiteSpace(audioTrack.SamplingRateFormatted))
                        StringBuilder.Append(audioTrack.SamplingRateFormatted);
                    StringBuilder.Trim(',', ' ');
                    delay = audioTrack.Delay;
                    int num = 0;
                    if ((delay.GetValueOrDefault() > num ? (delay.HasValue ? 1 : 0) : 0) != 0)
                        StringBuilder.Append(", Delay: " + audioTrack.Delay + "ms");
                    if (audioTrack.Default.GetValueOrDefault())
                        StringBuilder.Append(", Default");
                    if (audioTrack.Forced.GetValueOrDefault())
                        StringBuilder.Append(", Forced");
                    StringBuilder.TrimEnd(',', ' ');
                    StringBuilder.AppendLine();
                }
            }
            StringBuilder.TrimEnd(Environment.NewLine.ToCharArray());
            SimpleReportAudioSection = StringBuilder.ToString();
            StringBuilder.Clear();
            foreach (TextTrack textTrack in MediaInfo.Text)
            {
                StringBuilder.Append("> ");
                if (string.IsNullOrWhiteSpace(textTrack.LanguageFull))
                    StringBuilder.Append("Undetermined");
                else
                    StringBuilder.Append(textTrack.LanguageFull);
                if (!string.IsNullOrWhiteSpace(textTrack.Title))
                    StringBuilder.Append(" - [" + textTrack.Title + "]");
                if (!string.IsNullOrWhiteSpace(textTrack.FormatCommercial))
                    StringBuilder.Append(" - " + textTrack.FormatCommercial);
                int? delay = textTrack.Delay;
                if (delay.HasValue)
                {
                    delay = textTrack.Delay;
                    int num = 0;
                    if ((delay.GetValueOrDefault() > num ? (delay.HasValue ? 1 : 0) : 0) != 0)
                        StringBuilder.AppendLine(", Delay: " + textTrack.Delay + "ms");
                }
                if (textTrack.Default.GetValueOrDefault())
                    StringBuilder.Append(", Default");
                if (textTrack.Forced.GetValueOrDefault())
                    StringBuilder.Append(", Forced");
                StringBuilder.Trim();
                StringBuilder.AppendLine();
            }
            StringBuilder.TrimEnd(Environment.NewLine.ToCharArray());
            SimpleReportTextSection = StringBuilder.ToString();
            StringBuilder.Clear();
            foreach (MenuTrack menuTrack in MediaInfo.Menu)
            {
                StringBuilder.Append("> Menu " + menuTrack.ID + "(" + menuTrack.ChaptersCount + " chapters)");
                if (!string.IsNullOrWhiteSpace(menuTrack.LanguageFull))
                    StringBuilder.Append(" - " + menuTrack.LanguageFull);
                if (!string.IsNullOrWhiteSpace(menuTrack.Title))
                    StringBuilder.Append(" - [" + menuTrack.Title + "]");
                if (menuTrack.Default.GetValueOrDefault())
                    StringBuilder.Append(", Default");
                if (menuTrack.Forced.GetValueOrDefault())
                    StringBuilder.Append(", Forced");
                StringBuilder.Trim();
                StringBuilder.AppendLine();
                foreach (ChapterTrack chapter in menuTrack.Chapters)
                {
                    StringBuilder stringBuilder = StringBuilder;
                    string str1 = "  ";
                    int? index = chapter.Index;
                    var local = (ValueType)(index.HasValue ? new int?(index.GetValueOrDefault() + 1) : new int?());
                    string str2 = " - ";
                    string str3 = str1 + local + str2;
                    stringBuilder.Append(str3);
                    if (!ValidateChapterTitle(chapter.Timecode, chapter.Title, ':', '.', ' '))
                        StringBuilder.Append(chapter.Title.Trim(':') + ": ");
                    StringBuilder.Append(chapter.Timecode);
                    if (chapter.Language != ":0" && !string.IsNullOrEmpty(chapter.Language))
                        StringBuilder.Append(" (" + chapter.Language + ")");
                    StringBuilder.AppendLine();
                }
            }
            StringBuilder.TrimEnd(Environment.NewLine.ToCharArray());
            SimpleReportMenuSection = StringBuilder.ToString();
            StringBuilder.Clear();
            foreach (ImageTrack imageTrack in MediaInfo.Image)
            {
                StringBuilder.Append("> ");
                if (!string.IsNullOrWhiteSpace(imageTrack.Format))
                    StringBuilder.Append(imageTrack.Format + " (");
                if (!string.IsNullOrWhiteSpace(imageTrack.FormatCompression))
                    StringBuilder.Append(imageTrack.FormatCompression + " ");
                if (!string.IsNullOrWhiteSpace(imageTrack.FormatProfile))
                    StringBuilder.Append(imageTrack.FormatProfile);
                StringBuilder.TrimEnd();
                if (StringBuilder.Last() == '>')
                {

                }
                else if (StringBuilder.Last() != '(')
                {
                    StringBuilder.Append(") - ");
                }
                else
                {
                    StringBuilder.Length -= 2;
                    StringBuilder.Append(" - ");
                }
                int? nullable = imageTrack.Width;
                if (nullable.HasValue)
                {
                    nullable = imageTrack.Height;
                    if (nullable.HasValue)
                    {
                        nullable = imageTrack.Width;
                        int num1 = 0;
                        if ((nullable.GetValueOrDefault() > num1 ? (nullable.HasValue ? 1 : 0) : 0) != 0)
                        {
                            nullable = imageTrack.Height;
                            int num2 = 0;
                            if ((nullable.GetValueOrDefault() > num2 ? (nullable.HasValue ? 1 : 0) : 0) != 0)
                                StringBuilder.Append(imageTrack.Width.ToString() + "×" + imageTrack.Height + ", ");
                        }
                    }
                }
                if (!string.IsNullOrWhiteSpace(imageTrack.CompressionMode))
                    StringBuilder.Append(imageTrack.CompressionMode + ", ");
                if (!string.IsNullOrWhiteSpace(imageTrack.BitDepthFormatted))
                    StringBuilder.Append(imageTrack.BitDepthFormatted + ", ");
                if (!string.IsNullOrWhiteSpace(imageTrack.ColorSpace))
                    StringBuilder.Append(imageTrack.ColorSpace + ", ");
                if (!string.IsNullOrWhiteSpace(imageTrack.ChromaSubsampling))
                    StringBuilder.Append(imageTrack.ChromaSubsampling);
                StringBuilder.TrimEnd(' ', ',');
                StringBuilder.AppendLine();
            }
            StringBuilder.TrimEnd(Environment.NewLine.ToCharArray());
            SimpleReportImageSection = StringBuilder.ToString();
            if (GlobalSettings.ShowCover)
            {
                SimpleReportCover = GetCoverData();
                SimpleReportCoverWidth = SimpleReportCover == null ? new GridLength(0.0) : new GridLength(155.0);
            }
            SimpleReportGeneralSectionVisibility = MediaInfo.General.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
            SimpleReportVideoSectionVisibility = MediaInfo.Video.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
            SimpleReportAudioSectionVisibility = MediaInfo.Audio.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
            SimpleReportTextSectionVisibility = MediaInfo.Text.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
            SimpleReportMenuSectionVisibility = MediaInfo.Menu.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
            SimpleReportImageSectionVisibility = MediaInfo.Image.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        private void CreateTreeReport()
        {
            if (MediaInfo == null || GlobalSettings.DisableTreeReport)
                return;
            List<TreeViewItem> treeViewItemList = new List<TreeViewItem>();
            int index1 = 0;
            int num1 = 0;
            foreach (GeneralTrack generalTrack in MediaInfo.General)
            {
                treeViewItemList.Add(new TreeViewItem()
                {
                    Header = "General"
                });
                if (!string.IsNullOrWhiteSpace(generalTrack.FileName) && !string.IsNullOrWhiteSpace(generalTrack.FileExtension))
                    treeViewItemList[index1].Children.Add(new TreeViewItem()
                    {
                        Header = "File Name: " + generalTrack.FileName + "." + generalTrack.FileExtension
                    });
                if (!string.IsNullOrWhiteSpace(generalTrack.Format))
                    treeViewItemList[index1].Children.Add(new TreeViewItem()
                    {
                        Header = "Format: " + generalTrack.Format
                    });
                if (!string.IsNullOrWhiteSpace(generalTrack.FileSizeString))
                    treeViewItemList[index1].Children.Add(new TreeViewItem()
                    {
                        Header = "File Size: " + generalTrack.FileSizeString
                    });
                if (!string.IsNullOrWhiteSpace(ShowHideMilliseconds(generalTrack.DurationTimecode)))
                    treeViewItemList[index1].Children.Add(new TreeViewItem()
                    {
                        Header = "Duration: " + ShowHideMilliseconds(generalTrack.DurationTimecode)
                    });
                if (MediaInfo.Video.Count > 0)
                {
                    int? frameCount = MediaInfo.Video[0].FrameCount;
                    int num2 = 0;
                    if ((frameCount.GetValueOrDefault() > num2 ? (frameCount.HasValue ? 1 : 0) : 0) != 0)
                        treeViewItemList[index1].Children.Add(new TreeViewItem()
                        {
                            Header = "Frame Count: " + MediaInfo.Video[0].FrameCount
                        });
                }
                if (!string.IsNullOrWhiteSpace(generalTrack.OverallBitRateFormatted))
                    treeViewItemList[index1].Children.Add(new TreeViewItem()
                    {
                        Header = "Overal BitRate: " + generalTrack.OverallBitRateFormatted
                    });
                if (MediaInfo.Video.Count > 0 && !string.IsNullOrEmpty(MediaInfo.Video[0].MultiViewLayout))
                    treeViewItemList[index1].Children.Add(new TreeViewItem()
                    {
                        Header = "3D Mode: " + MediaInfo.Video[0].MultiViewLayout
                    });
                if (!string.IsNullOrWhiteSpace(generalTrack.Title))
                    treeViewItemList[index1].Children.Add(new TreeViewItem()
                    {
                        Header = "Title: " + generalTrack.Title
                    });
                if (!string.IsNullOrWhiteSpace(generalTrack.Album) && generalTrack.Title != generalTrack.Album)
                    treeViewItemList[index1].Children.Add(new TreeViewItem()
                    {
                        Header = "Album: " + generalTrack.Album
                    });
                if (!string.IsNullOrWhiteSpace(generalTrack.AlbumPerformer))
                    treeViewItemList[index1].Children.Add(new TreeViewItem()
                    {
                        Header = "Artist: " + generalTrack.AlbumPerformer
                    });
                if (!string.IsNullOrWhiteSpace(generalTrack.Genre))
                    treeViewItemList[index1].Children.Add(new TreeViewItem()
                    {
                        Header = "Genre: " + generalTrack.Genre
                    });
                treeViewItemList[index1].Children.Add(new TreeViewItem()
                {
                    Header = "Cover: " + (generalTrack.Cover.GetValueOrDefault() ? "Yes" : "No")
                });
                if (!string.IsNullOrWhiteSpace(generalTrack.EncodedDate))
                    treeViewItemList[index1].Children.Add(new TreeViewItem()
                    {
                        Header = "Date Encoded: " + generalTrack.EncodedDate
                    });
                if (!string.IsNullOrWhiteSpace(generalTrack.EncodedLibrary))
                    treeViewItemList[index1].Children.Add(new TreeViewItem()
                    {
                        Header = "Created with: " + generalTrack.EncodedLibrary
                    });
                ++index1;
            }
            foreach (VideoTrack videoTrack in MediaInfo.Video)
            {
                if (MediaInfo.Video.Count == 1)
                    treeViewItemList.Add(new TreeViewItem()
                    {
                        Header = "Video"
                    });
                else
                    treeViewItemList.Add(new TreeViewItem()
                    {
                        Header = "Video #" + ++num1
                    });
                if (string.IsNullOrWhiteSpace(videoTrack.LanguageFull))
                    treeViewItemList[index1].Children.Add(new TreeViewItem()
                    {
                        Header = "Undetermined Language"
                    });
                else
                    treeViewItemList[index1].Children.Add(new TreeViewItem()
                    {
                        Header = "Language: " + videoTrack.LanguageFull
                    });
                if (!string.IsNullOrWhiteSpace(videoTrack.Title))
                    treeViewItemList[index1].Children.Add(new TreeViewItem()
                    {
                        Header = "Track Title: " + videoTrack.Title
                    });
                int? nullable = videoTrack.Width;
                if (nullable.HasValue)
                {
                    nullable = videoTrack.Height;
                    if (nullable.HasValue)
                    {
                        nullable = videoTrack.Width;
                        int num2 = 0;
                        if ((nullable.GetValueOrDefault() > num2 ? (nullable.HasValue ? 1 : 0) : 0) != 0)
                        {
                            nullable = videoTrack.Height;
                            int num3 = 0;
                            if ((nullable.GetValueOrDefault() > num3 ? (nullable.HasValue ? 1 : 0) : 0) != 0)
                                treeViewItemList[index1].Children.Add(new TreeViewItem()
                                {
                                    Header = "Resolution: " + videoTrack.Width + "×" + videoTrack.Height
                                });
                        }
                    }
                }
                if (!string.IsNullOrWhiteSpace(videoTrack.DisplayAspectRatioFormatted))
                    treeViewItemList[index1].Children.Add(new TreeViewItem()
                    {
                        Header = "Aspect Ratio: " + videoTrack.DisplayAspectRatioFormatted
                    });
                double? frameRate = videoTrack.FrameRate;
                if (frameRate.HasValue)
                {
                    IList<TreeViewItem> children = treeViewItemList[index1].Children;
                    TreeViewItem treeViewItem = new TreeViewItem();
                    string str1 = "Framerate: ";
                    frameRate = videoTrack.FrameRate;
                    string str2 = frameRate.Value.ToString(CultureInfo.CreateSpecificCulture("en-US"));
                    string str3 = " fps";
                    treeViewItem.Header = str1 + str2 + str3;
                    children.Add(treeViewItem);
                }
                if (!string.IsNullOrWhiteSpace(videoTrack.BitRateFormatted))
                    treeViewItemList[index1].Children.Add(new TreeViewItem()
                    {
                        Header = "BitRate: " + videoTrack.BitRateFormatted
                    });
                if (!string.IsNullOrWhiteSpace(videoTrack.FormatCommercial))
                    treeViewItemList[index1].Children.Add(new TreeViewItem()
                    {
                        Header = "Codec Video: " + (videoTrack.FormatCommercial + " (" + videoTrack.FormatProfile.ToString() + ", " + videoTrack.FormatSettings.ToString()).TrimEnd(',', ' ') + ")"
                    });
                if (!string.IsNullOrWhiteSpace(videoTrack.ColorSpace))
                    treeViewItemList[index1].Children.Add(new TreeViewItem()
                    {
                        Header = "Color Space: " + videoTrack.ColorSpace
                    });
                if (!string.IsNullOrWhiteSpace(videoTrack.ChromaSubsampling))
                    treeViewItemList[index1].Children.Add(new TreeViewItem()
                    {
                        Header = "Chroma Subsampling: " + videoTrack.ChromaSubsampling
                    });
                if (!string.IsNullOrWhiteSpace(videoTrack.ColourPrimaries))
                    treeViewItemList[index1].Children.Add(new TreeViewItem()
                    {
                        Header = "Color Primaries: " + videoTrack.ColourPrimaries
                    });
                nullable = videoTrack.Delay;
                int num4 = 0;
                if ((nullable.GetValueOrDefault() > num4 ? (nullable.HasValue ? 1 : 0) : 0) != 0)
                    treeViewItemList[index1].Children.Add(new TreeViewItem()
                    {
                        Header = "Delay: " + videoTrack.Delay + "ms"
                    });
                bool? forced;
                if (!string.IsNullOrWhiteSpace(videoTrack.DefaultString))
                {
                    IList<TreeViewItem> children = treeViewItemList[index1].Children;
                    TreeViewItem treeViewItem = new TreeViewItem();
                    string str1 = "Default: ";
                    forced = videoTrack.Default;
                    string str2 = forced.GetValueOrDefault() ? "Yes" : "No";
                    treeViewItem.Header = str1 + str2;
                    children.Add(treeViewItem);
                }
                if (!string.IsNullOrWhiteSpace(videoTrack.ForcedString))
                {
                    IList<TreeViewItem> children = treeViewItemList[index1].Children;
                    TreeViewItem treeViewItem = new TreeViewItem();
                    string str1 = "Forced: ";
                    forced = videoTrack.Forced;
                    string str2 = forced.GetValueOrDefault() ? "Yes" : "No";
                    treeViewItem.Header = str1 + str2;
                    children.Add(treeViewItem);
                }
                ++index1;
            }
            int num5 = 0;
            foreach (AudioTrack audioTrack in MediaInfo.Audio)
            {
                if (MediaInfo.Audio.Count == 1)
                    treeViewItemList.Add(new TreeViewItem()
                    {
                        Header = "Audio"
                    });
                else
                    treeViewItemList.Add(new TreeViewItem()
                    {
                        Header = "Audio #" + ++num5
                    });
                if (string.IsNullOrWhiteSpace(audioTrack.LanguageFull))
                    treeViewItemList[index1].Children.Add(new TreeViewItem()
                    {
                        Header = "Undetermined Language"
                    });
                else
                    treeViewItemList[index1].Children.Add(new TreeViewItem()
                    {
                        Header = "Language: " + audioTrack.LanguageFull
                    });
                if (!string.IsNullOrWhiteSpace(audioTrack.Title))
                    treeViewItemList[index1].Children.Add(new TreeViewItem()
                    {
                        Header = "Track Title: " + audioTrack.Title
                    });
                if (!string.IsNullOrWhiteSpace(audioTrack.Format))
                    treeViewItemList[index1].Children.Add(new TreeViewItem()
                    {
                        Header = "Format: " + audioTrack.Format.Trim(' ')
                    });
                int count = 0;
                if (!string.IsNullOrWhiteSpace(audioTrack.FormatProfile))
                    count = audioTrack.FormatProfile.Split(new string[1]
                    {
            " / "
                    }, StringSplitOptions.None).Length;
                else if (!string.IsNullOrWhiteSpace(audioTrack.Format))
                    count = audioTrack.Format.Split(new string[1]
                    {
            " / "
                    }, StringSplitOptions.None).Length;
                if (count > 1)
                {
                    int num2 = treeViewItemList[index1].Children.Count<TreeViewItem>();
                    foreach (int index2 in Enumerable.Range(0, count))
                    {
                        if (!string.IsNullOrWhiteSpace(GetSubTracks(audioTrack.Format, index2)) && !string.IsNullOrWhiteSpace(GetSubTracks(audioTrack.FormatProfile, index2)))
                            treeViewItemList[index1].Children.Add(new TreeViewItem()
                            {
                                Header = GetSubTracks(audioTrack.Format, index2) + "-" + GetSubTracks(audioTrack.FormatProfile, index2)
                            });
                        else if (!string.IsNullOrWhiteSpace(GetSubTracks(audioTrack.Format, index2)))
                            treeViewItemList[index1].Children.Add(new TreeViewItem()
                            {
                                Header = GetSubTracks(audioTrack.Format, index2)
                            });
                        else if (!string.IsNullOrWhiteSpace(GetSubTracks(audioTrack.FormatProfile, index2)))
                            treeViewItemList[index1].Children.Add(new TreeViewItem()
                            {
                                Header = GetSubTracks(audioTrack.FormatProfile, index2)
                            });
                        else
                            treeViewItemList[index1].Children.Add(new TreeViewItem()
                            {
                                Header = string.Format("Subtrack {0}", index2 + 1)
                            });
                        string subTracks = GetSubTracks(audioTrack.BitRateMode, index2);
                        if (!string.IsNullOrWhiteSpace(GetSubTracks(audioTrack.BitRateFormatted, index2)))
                        {
                            if (subTracks.Equals("CBR"))
                                treeViewItemList[index1].Children[num2 + index2].Children.Add(new TreeViewItem()
                                {
                                    Header = "BitRate: " + GetSubTracks(audioTrack.BitRateFormatted, index2)
                                });
                            else if (!string.IsNullOrEmpty(audioTrack.BitRateMaximumFormatted))
                                treeViewItemList[index1].Children[num2 + index2].Children.Add(new TreeViewItem()
                                {
                                    Header = "BitRate: " + audioTrack.BitRateMaximumFormatted
                                });
                        }
                        if (!string.IsNullOrWhiteSpace(GetSubTracks(audioTrack.BitRateMode, index2)))
                            treeViewItemList[index1].Children[num2 + index2].Children.Add(new TreeViewItem()
                            {
                                Header = "BitRate Mode: " + subTracks
                            });
                        if (!string.IsNullOrWhiteSpace(GetSubTracks(audioTrack.ChannelsFormatted, index2)))
                            treeViewItemList[index1].Children[num2 + index2].Children.Add(new TreeViewItem()
                            {
                                Header = "Channel: " + GetSubTracks(audioTrack.ChannelsFormatted, index2)
                            });
                        if (!string.IsNullOrWhiteSpace(GetSubTracks(audioTrack.BitDepthFormatted, index2)))
                            treeViewItemList[index1].Children[num2 + index2].Children.Add(new TreeViewItem()
                            {
                                Header = "Bit Depth: " + GetSubTracks(audioTrack.BitDepthFormatted, index2)
                            });
                        if (!string.IsNullOrWhiteSpace(GetSubTracks(audioTrack.SamplingRateFormatted, index2)))
                            treeViewItemList[index1].Children[num2 + index2].Children.Add(new TreeViewItem()
                            {
                                Header = "Sampling Rate: " + GetSubTracks(audioTrack.SamplingRateFormatted, index2)
                            });
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(audioTrack.FormatProfile))
                        treeViewItemList[index1].Children.Add(new TreeViewItem()
                        {
                            Header = "Profile: " + audioTrack.FormatProfile
                        });
                    if (!string.IsNullOrEmpty(audioTrack.BitRateFormatted))
                        treeViewItemList[index1].Children.Add(new TreeViewItem()
                        {
                            Header = "BitRate: " + audioTrack.BitRateFormatted
                        });
                    if (!string.IsNullOrEmpty(audioTrack.BitRateMode))
                        treeViewItemList[index1].Children.Add(new TreeViewItem()
                        {
                            Header = "BitRate Mode: " + audioTrack.BitRateMode
                        });
                    if (!string.IsNullOrEmpty(audioTrack.ChannelsFormatted))
                        treeViewItemList[index1].Children.Add(new TreeViewItem()
                        {
                            Header = "Channels: " + audioTrack.ChannelsFormatted
                        });
                    if (!string.IsNullOrEmpty(audioTrack.SamplingRateFormatted))
                        treeViewItemList[index1].Children.Add(new TreeViewItem()
                        {
                            Header = "Sampling Rate: " + audioTrack.SamplingRateFormatted
                        });
                    if (!string.IsNullOrEmpty(audioTrack.SamplingRateFormatted))
                        treeViewItemList[index1].Children.Add(new TreeViewItem()
                        {
                            Header = "Bit Depth: " + audioTrack.BitDepth
                        });
                }
                int? delay = audioTrack.Delay;
                int num3 = 0;
                if ((delay.GetValueOrDefault() > num3 ? (delay.HasValue ? 1 : 0) : 0) != 0)
                    treeViewItemList[index1].Children.Add(new TreeViewItem()
                    {
                        Header = "Delay: " + audioTrack.Delay + "ms"
                    });
                if (!string.IsNullOrEmpty(audioTrack.DefaultString))
                    treeViewItemList[index1].Children.Add(new TreeViewItem()
                    {
                        Header = "Default: " + audioTrack.DefaultString
                    });
                if (!string.IsNullOrEmpty(audioTrack.ForcedString))
                    treeViewItemList[index1].Children.Add(new TreeViewItem()
                    {
                        Header = "Forced: " + audioTrack.ForcedString
                    });
                ++index1;
            }
            int num6 = 0;
            foreach (TextTrack textTrack in MediaInfo.Text)
            {
                if (MediaInfo.Text.Count == 1)
                    treeViewItemList.Add(new TreeViewItem()
                    {
                        Header = "Text"
                    });
                else
                    treeViewItemList.Add(new TreeViewItem()
                    {
                        Header = "Text #" + ++num6
                    });
                if (string.IsNullOrEmpty(textTrack.LanguageFull))
                    treeViewItemList[index1].Children.Add(new TreeViewItem()
                    {
                        Header = "Undetermined Language"
                    });
                else
                    treeViewItemList[index1].Children.Add(new TreeViewItem()
                    {
                        Header = "Language: " + textTrack.LanguageFull
                    });
                if (!string.IsNullOrEmpty(textTrack.Title))
                    treeViewItemList[index1].Children.Add(new TreeViewItem()
                    {
                        Header = "Track Title: " + textTrack.Title
                    });
                if (!string.IsNullOrWhiteSpace(textTrack.FormatCommercial))
                    treeViewItemList[index1].Children.Add(new TreeViewItem()
                    {
                        Header = "Codec: " + textTrack.FormatCommercial
                    });
                ++index1;
            }
            int num7 = 0;
            foreach (MenuTrack menuTrack in MediaInfo.Menu)
            {
                if (MediaInfo.Menu.Count == 1)
                    treeViewItemList.Add(new TreeViewItem()
                    {
                        Header = "Menu"
                    });
                else
                    treeViewItemList.Add(new TreeViewItem()
                    {
                        Header = "Menu #" + ++num7
                    });
                treeViewItemList[index1].Children.Add(new TreeViewItem()
                {
                    Header = "Chapters Count: " + menuTrack.ChaptersCount
                });
                if (!string.IsNullOrWhiteSpace(menuTrack.LanguageFull))
                    treeViewItemList[index1].Children.Add(new TreeViewItem()
                    {
                        Header = "Language: " + menuTrack.LanguageFull
                    });
                if (menuTrack.Default.GetValueOrDefault())
                    treeViewItemList[index1].Children.Add(new TreeViewItem()
                    {
                        Header = "Default: Yes"
                    });
                else
                    treeViewItemList[index1].Children.Add(new TreeViewItem()
                    {
                        Header = "Default: No"
                    });
                if (menuTrack.Forced.GetValueOrDefault())
                    treeViewItemList[index1].Children.Add(new TreeViewItem()
                    {
                        Header = "Forced: Yes"
                    });
                else
                    treeViewItemList[index1].Children.Add(new TreeViewItem()
                    {
                        Header = "Forced: No"
                    });
                if (menuTrack.Chapters.Count == 1)
                {
                    int index2 = treeViewItemList[index1].Children.Count<TreeViewItem>();
                    treeViewItemList[index1].Children.Add(new TreeViewItem()
                    {
                        Header = "Chapter"
                    });
                    if (!ValidateChapterTitle(menuTrack.Chapters[0].Timecode, menuTrack.Chapters[0].Title, ':', '.', ' '))
                        treeViewItemList[index1].Children[index2].Children.Add(new TreeViewItem()
                        {
                            Header = "Title: " + menuTrack.Chapters[0].Title.Trim(':')
                        });
                    treeViewItemList[index1].Children[index2].Children.Add(new TreeViewItem()
                    {
                        Header = "Time: " + menuTrack.Chapters[0].Timecode
                    });
                    if (menuTrack.Chapters[0].Language != "0" && !string.IsNullOrEmpty(menuTrack.Chapters[0].Language))
                        treeViewItemList[index1].Children[index2].Children.Add(new TreeViewItem()
                        {
                            Header = "Language: " + menuTrack.Chapters[0].Language
                        });
                    else
                        treeViewItemList[index1].Children[index2].Children.Add(new TreeViewItem()
                        {
                            Header = "Language: Undetermined"
                        });
                }
                else
                {
                    int index2 = treeViewItemList[index1].Children.Count<TreeViewItem>();
                    int index3 = 0;
                    treeViewItemList[index1].Children.Add(new TreeViewItem()
                    {
                        Header = "Chapters"
                    });
                    foreach (ChapterTrack chapter in menuTrack.Chapters)
                    {
                        IList<TreeViewItem> children = treeViewItemList[index1].Children[index2].Children;
                        TreeViewItem treeViewItem = new TreeViewItem();
                        string str = "Chapter #";
                        int? index4 = chapter.Index;
                        var local = (ValueType)(index4.HasValue ? new int?(index4.GetValueOrDefault() + 1) : new int?());
                        treeViewItem.Header = str + local;
                        children.Add(treeViewItem);
                        if (!ValidateChapterTitle(chapter.Timecode, chapter.Title, ':', '.', ' '))
                            treeViewItemList[index1].Children[index2].Children[index3].Children.Add(new TreeViewItem()
                            {
                                Header = "Title: " + chapter.Title.Trim(':')
                            });
                        treeViewItemList[index1].Children[index2].Children[index3].Children.Add(new TreeViewItem()
                        {
                            Header = "Time: " + chapter.Timecode
                        });
                        if (chapter.Language != ":0" && !string.IsNullOrEmpty(chapter.Language))
                            treeViewItemList[index1].Children[index2].Children[index3].Children.Add(new TreeViewItem()
                            {
                                Header = "Language: " + menuTrack.Chapters[0].Language
                            });
                        else
                            treeViewItemList[index1].Children[index2].Children[index3].Children.Add(new TreeViewItem()
                            {
                                Header = "Language: Undetermined"
                            });
                        ++index3;
                    }
                }
                ++index1;
            }
            int num8 = 0;
            foreach (ImageTrack imageTrack in MediaInfo.Image)
            {
                if (MediaInfo.Image.Count == 1)
                    treeViewItemList.Add(new TreeViewItem()
                    {
                        Header = "Image"
                    });
                else
                    treeViewItemList.Add(new TreeViewItem()
                    {
                        Header = "Image #" + ++num8
                    });
                if (!string.IsNullOrWhiteSpace(imageTrack.Format))
                    treeViewItemList[index1].Children.Add(new TreeViewItem()
                    {
                        Header = "Format: " + imageTrack.Format
                    });
                if (!string.IsNullOrWhiteSpace(imageTrack.FormatCompression))
                    treeViewItemList[index1].Children.Add(new TreeViewItem()
                    {
                        Header = "Format compression: " + imageTrack.FormatCompression
                    });
                if (!string.IsNullOrWhiteSpace(imageTrack.FormatProfile))
                    treeViewItemList[index1].Children.Add(new TreeViewItem()
                    {
                        Header = "Format compression: " + imageTrack.FormatProfile
                    });
                int? nullable = imageTrack.Width;
                int num2 = 0;
                if ((nullable.GetValueOrDefault() > num2 ? (nullable.HasValue ? 1 : 0) : 0) != 0)
                {
                    nullable = imageTrack.Height;
                    int num3 = 0;
                    if ((nullable.GetValueOrDefault() > num3 ? (nullable.HasValue ? 1 : 0) : 0) != 0)
                        treeViewItemList[index1].Children.Add(new TreeViewItem()
                        {
                            Header = "Resolution: " + imageTrack.Width + "×" + imageTrack.Height
                        });
                }
                if (!string.IsNullOrWhiteSpace(imageTrack.CompressionMode))
                    treeViewItemList[index1].Children.Add(new TreeViewItem()
                    {
                        Header = "Compression mode: " + imageTrack.CompressionMode
                    });
                if (!string.IsNullOrWhiteSpace(imageTrack.BitDepthFormatted))
                    treeViewItemList[index1].Children.Add(new TreeViewItem()
                    {
                        Header = "Bit depth: " + imageTrack.BitDepthFormatted
                    });
                if (!string.IsNullOrWhiteSpace(imageTrack.ColorSpace))
                    treeViewItemList[index1].Children.Add(new TreeViewItem()
                    {
                        Header = "Color space: " + imageTrack.ColorSpace
                    });
                if (!string.IsNullOrWhiteSpace(imageTrack.ChromaSubsampling))
                    treeViewItemList[index1].Children.Add(new TreeViewItem()
                    {
                        Header = "Chroma subsampling: " + imageTrack.ChromaSubsampling
                    });
                ++index1;
            }
            TreeReport = treeViewItemList;
        }

        private void GetMediaInfoReports()
        {
            if (MediaInfo == null)
                return;
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
            if (string.IsNullOrEmpty(CompleteReport) || !CompleteReport.Contains("Inform"))
                return;
            for (int startIndex = CompleteReport.IndexOf("Inform"); startIndex > 0; startIndex = CompleteReport.IndexOf("Inform", startIndex))
            {
                int num = CompleteReport.IndexOf(Environment.NewLine, startIndex) - startIndex;
                if (num > 0)
                    CompleteReport = CompleteReport.Remove(startIndex, num + Environment.NewLine.Length);
            }
        }

        private void UpdateReportsVisibility()
        {
            SimpleReportVisibility = GlobalSettings.DisableSimpleReport ? Visibility.Collapsed : Visibility.Visible;
            CompleteReportVisibility = GlobalSettings.DisableCompleteReport ? Visibility.Collapsed : Visibility.Visible;
            TreeReportVisibility = GlobalSettings.DisableTreeReport ? Visibility.Collapsed : Visibility.Visible;
            SelectedTabIndex = GlobalSettings.DisableSimpleReport ? 1 : 0;
        }

        private string ShowHideMilliseconds(string time)
        {
            if (GlobalSettings.ShowMilliseconds)
                return time;
            return time.Split('.')[0];
        }

        private string GetSubTracks(string str, int index)
        {
            if (str.Contains(" / "))
            {
                string[] strArray = str.Split(new string[1] { " / " }, StringSplitOptions.None);
                if (strArray.Length > index)
                {
                    if (!string.IsNullOrWhiteSpace(strArray[index]) && !strArray[index].Contains("Core"))
                    {
                        if (strArray[index].Contains("HR") || strArray[index].Contains("MA"))
                            return "HD " + strArray[index];
                        return strArray[index].Trim(' ');
                    }
                }
                else
                    return strArray.Last<string>().Trim(' ');
            }
            else if (!string.IsNullOrEmpty(str))
                return str;
            return string.Empty;
        }

        public bool ValidateChapterTitle(string title, string timecode, params char[] ignorechar)
        {
            if (title.Length < 3 || timecode.Length < 3)
                return false;
            ignorechar.ToList<char>().ForEach(c =>
            {
                title = title.Replace(c.ToString(), "--");
                timecode = timecode.Replace(c.ToString(), "--");
            });
            if (!title.Contains(timecode))
                return timecode.Contains(title);
            return true;
        }

        private bool IsHDR(VideoTrack infos)
        {
            if (infos == null)
                return false;
            int? bitDepth = infos.BitDepth;
            if (bitDepth.HasValue)
            {
                bitDepth = infos.BitDepth;
                if (bitDepth.Value >= 10 && (!string.IsNullOrEmpty(infos.FormatCommercial) && infos.FormatCommercial.Contains("HDR") || !string.IsNullOrEmpty(infos.Format) && infos.Format.Contains("HEVC")) && (!string.IsNullOrEmpty(infos.ColourPrimaries) && infos.ColourPrimaries.Contains("BT.2020")))
                    return true;
            }
            return false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler propertyChanged = PropertyChanged;
            if (propertyChanged == null)
                return;
            propertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
