using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Xml;
using System.Xml.Linq;
using System.Net;
using SevenZip;
using GetMediaInfos;

namespace GetMediaInfoGUI
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    /// <seealso cref="System.Windows.Window" />
    /// <seealso cref="System.Windows.Markup.IComponentConnector" />
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Boolean that define if the report should be automatically generated or not
        /// </summary>
        bool SaveReportActive;

        /// <summary>
        /// Generated mediainfo report
        /// </summary>
        MediaInfo report;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            ClearAll();
        }

        /// <summary>
        /// Handler for application loading event.Contains functions to inizializate program, check for dependencies and restore user config.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Inizialize(object sender, EventArgs e)
        {
            Version.Content = "Version " + System.Windows.Forms.Application.ProductVersion;
            if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\mediainfo.dll"))
            {
                FileVersionInfo tmp = FileVersionInfo.GetVersionInfo(System.Windows.Forms.Application.StartupPath + "\\MediaInfo.dll");
                StatusMediainfo.Text = "OK v" + tmp.FileVersion;
                StatusMediainfo.Foreground = Brushes.Green;
            }
            else
            {
                StatusMediainfo.Text = "ERROR";
                StatusMediainfo.Foreground = Brushes.Red;
                System.Windows.Forms.MessageBox.Show("Library MediaInfo.dll not found.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Windows.Forms.Application.Exit();
            }
            if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\GetMediaInfos.dll"))
            {
                FileVersionInfo tmp = FileVersionInfo.GetVersionInfo(System.Windows.Forms.Application.StartupPath + "\\GetMediaInfos.dll");
                StatusGetMediaInfos.Text = "OK v" + tmp.FileVersion;
                StatusGetMediaInfos.Foreground = Brushes.Green;
            }
            else
            {
                StatusGetMediaInfos.Text = "ERROR";
                StatusGetMediaInfos.Foreground = Brushes.Red;
                System.Windows.Forms.MessageBox.Show("Library GetMediaInfos.dll not found.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Windows.Forms.Application.Exit();
            }
            ProcessArgs();
            RestoreConfig();
        }

        /// <summary>
        /// Handler for application closing event. Saves user configuration.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void SaveAndClose(object sender, EventArgs e)
        {
            SaveConfig();
        }

        /// <summary>
        /// Handles the Click event of the AddContextMenu control. It adds context menu GetMediaInfos entry for specific extensions.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void AddContextMenu_Click(object sender, RoutedEventArgs e)
        {
            List<string> listext = new List<string>() { ".mkv", ".mka", ".mk3d", ".mp4", ".m4v", ".m4a", ".m2ts", ".ts", ".avi",
                ".webm", ".weba", ".ac3", ".dts", ".dtshd", ".thd", ".flac", ".alac", ".aac", ".mp3", ".wav", ".aiff", ".h264",
                ".h265", ".hevc", ".avc", ".y4m", ".yuv", ".ssif" };

            List<string> completelist = new List<string>();

            foreach (string str in listext)
            {
                completelist.Add("VLC" + str);
                completelist.Add("mpc-be64" + str);
            }

            Microsoft.Win32.RegistryKey key;
            foreach (string ext in completelist)
            {
                key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software", true).OpenSubKey("Classes", true);
                key = key.CreateSubKey(ext);
                key = key.CreateSubKey("shell");
                key = key.CreateSubKey("open_with_getmediainfosgui");
                key.SetValue("", "GetMediaInfos");
                key.SetValue("Icon", "\"" + System.Windows.Forms.Application.StartupPath + "\\icon.ico\"");

                key = key.CreateSubKey("command");
                key.SetValue("", "\"" + System.Windows.Forms.Application.ExecutablePath + "\" \"%1\"");

                key.Close();
            }
            AssociationStatus.Content = "Successufully Added.";
        }

        /// <summary>
        /// Handles the Click event of the RemoveContextMenu control. It removes context menu GetMediaInfos entry for specific extensions.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void RemoveContextMenu_Click(object sender, RoutedEventArgs e)
        {
            List<string> listext = new List<string>() { ".mkv", ".mka", ".mk3d", ".mp4", ".m4v", ".m4a", ".m2ts", ".ts", ".avi",
                ".webm", ".weba", ".ac3", ".dts", ".dtshd", ".thd", ".flac", ".alac", ".aac", ".mp3", ".wav", ".aiff", ".h264",
                ".h265", ".hevc", ".avc", ".y4m", ".yuv", ".ssif" };

            List<string> completelist = new List<string>();

            foreach (string str in listext)
            {
                completelist.Add("VLC" + str);
                completelist.Add("mpc-be64" + str);
            }

            Microsoft.Win32.RegistryKey key;
            foreach (string ext in completelist)
            {
                try
                {
                    key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software", true).OpenSubKey("Classes", true).OpenSubKey(ext, true).CreateSubKey("shell");
                    key.DeleteSubKeyTree("open_with_getmediainfosgui");
                    key.Close();
                }
                catch { }
            }
            AssociationStatus.Content = "Successufully Removed.";
        }

        /// <summary>
        /// Handles the Shown event of the GetMediaInfo control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void GetMediaInfo_Shown(object sender, EventArgs e)
        {
            /*this.Update();
            this.Refresh();
            ApriDaArgomentoExe();*/
        }

        /// <summary>
        /// Handles the Click event of the OpenFile control. Is used to open a new OpenFileDialog to let user to select the input file.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            ClearAll();
            OpenFileDialog file = new OpenFileDialog();
            file.Filter = "Mediafile|*.*";
            file.Title = "Select a Media File";
            file.Multiselect = false;

            if (file.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                filepathBox.Text = file.FileName;
                //Analisi(filepathBox.Text);
            }

        }

        /// <summary>
        /// Analyze arguments passed to the application. If the arguments count is greater than 1 analyze the first one.
        /// </summary>
        public void ProcessArgs()
        {
            String[] arguments = Environment.GetCommandLineArgs();
            if (arguments.Length > 1)
            {
                //toolStripStatusLabel1.Text = "Analyzing...";
                filepathBox.Text = arguments[1];
                AnalyzeFile(arguments[1]);
            }
        }

        /// <summary>
        /// Analyze declared path in filepathBox.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void AnalyzeDeclaredPath(object sender, EventArgs e)
        {
            AnalyzeFile(filepathBox.Text);
        }

        /// <summary>
        /// Analyze dropped file.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DragEventArgs"/> instance containing the event data.</param>
        public void OpenDroppedFile(object sender, System.Windows.DragEventArgs e)
        {
            //ClearAll();
            System.Windows.DataObject data = (System.Windows.DataObject)e.Data;
            if (data.ContainsFileDropList())
            {
                string[] FileList = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop, false);
                filepathBox.Text = FileList[0];
                //Analisi(filepathBox.Text);
            }
        }

        /// <summary>
        /// Handle for Drag Enter event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DragEventArgs"/> instance containing the event data.</param>
        public void DragEnter_EventHandler(object sender, System.Windows.DragEventArgs e)
        {
            e.Effects = System.Windows.DragDropEffects.Copy;
        }

        /// <summary>
        /// Analyze the specified path.
        /// </summary>
        /// <param name="path">Path to file to analyze</param>
        public void AnalyzeFile(string path)
        {
            ClearAll();
            if (File.Exists(path))
            {
                toolStripStatusLabel1.Content = "Analysing...";
                report = new MediaInfo(path);

                CreateSimpleReport();
                ShowReportsAndSaveToDisk();
                this.Title += " (" + System.IO.Path.GetFileName(path) + ")";
                toolStripStatusLabel1.Content = "Analysis Completed.";
                if (!AutoSaveReport.IsChecked.Value) SaveReport.IsEnabled = true;
                SaveReportActive = true;
            }
            else
            {
                toolStripStatusLabel1.Content = "Not existing file.";
                SaveReport.IsEnabled = SaveReportActive = false;
            }
        }

        /// <summary>
        /// Clear all variables and the interface.
        /// </summary>
        private void ClearAll()
        {
            TextBoxComplete.Text = TextBoxStandard.Text = "";
            this.Title = "GetMediaInfo";
            SaveReport.IsEnabled = SaveReportActive = false;
            SubInfoText.Text = AudioInfoText.Text = VideoInfoText.Text = GeneralInfoText.Text = "";
            while (TreeReport.HasItems)
            {
                TreeReport.Items.RemoveAt(0);
            }
        }

        /// <summary>
        /// Create the file report and save it to disk as txt file if AutoSaveReport is checked.
        /// </summary>
        private void ShowReportsAndSaveToDisk()
        {
            if (report != null)
            {
                TextBoxStandard.Text = report.InfoStandard;

                TextBoxComplete.Text = report.InfoComplete;

                if (RemoveSpace.IsChecked.Value)
                {
                    TextBoxStandard.Text = TextBoxStandard.Text.Replace("  ", "");
                    TextBoxStandard.Text = TextBoxStandard.Text.Replace(" : ", ": ");
                    TextBoxComplete.Text = TextBoxComplete.Text.Replace("  ", "");
                    TextBoxComplete.Text = TextBoxComplete.Text.Replace(" : ", ": ");
                }

                if (AutoSaveReport.IsChecked.Value)
                {
                    string ReportName = System.IO.Path.ChangeExtension(filepathBox.Text, ".txt");
                    int i = 2;
                    while (File.Exists(ReportName))
                    {
                        ReportName = System.IO.Path.ChangeExtension(filepathBox.Text, i + ".txt");
                        i++;
                    }
                    StreamWriter ReportTxt = new StreamWriter(ReportName);
                    ReportTxt.WriteLine(TextBoxStandard.Text);
                    ReportTxt.Close();
                }
            }
        }

        /// <summary>
        /// Remove milliseconds from the input timecode if MilliSec is checked else return the input string.
        /// </summary>
        /// <param name="time">String with timecode</param>
        /// <returns></returns>
        private string ManageMillisec(string time)
        {
            if (!MilliSec.IsChecked.Value)
            {
                string[] durata = time.Split('.');
                return durata[0];
            }
            else return time;
        }

        /// <summary>
        /// Create the Simple report.
        /// </summary>
        private void CreateSimpleReport()
        {
            if (report != null)
            {
                if (!DisableSimpleInfo.IsChecked.Value)
                {
                    //general infos
                    GeneralInfoText.Text = "File Name: " + report.General[0].FileName + "." + report.General[0].FileExtension + Environment.NewLine;
                    GeneralInfoText.Text += "Format: " + report.General[0].Format + Environment.NewLine;
                    GeneralInfoText.Text += "File Size: " + report.General[0].FileSizePrecise + Environment.NewLine;
                    if (!String.IsNullOrWhiteSpace(report.General[0].PlayTimeFormatted)) GeneralInfoText.Text += "Duration: " + ManageMillisec(report.General[0].PlayTimeFormatted) + Environment.NewLine;
                    if (report.Video.Count > 0) GeneralInfoText.Text += "Frame Count: " + report.Video[0].FrameCount + Environment.NewLine;
                    if (!String.IsNullOrWhiteSpace(report.General[0].OveralBitRateString)) GeneralInfoText.Text += "Overral Bitrate: " + report.General[0].OveralBitRateString + Environment.NewLine;
                    if (report.Video.Count > 0) if (!String.IsNullOrEmpty(report.Video[0].MultiViewLayout)) GeneralInfoText.Text += "3D Mode: " + report.Video[0].MultiViewLayout + Environment.NewLine;
                    if (!String.IsNullOrWhiteSpace(report.General[0].Title)) GeneralInfoText.Text += "Title: " + report.General[0].Title + Environment.NewLine;
                    if (!String.IsNullOrWhiteSpace(report.General[0].EncodedDate)) GeneralInfoText.Text += "Date Encoded: " + report.General[0].EncodedDate + Environment.NewLine;
                    if (!String.IsNullOrWhiteSpace(report.General[0].EncodedLibrary)) GeneralInfoText.Text += "Created with: " + report.General[0].EncodedLibrary + Environment.NewLine;

                    //video tracks
                    VideoInfoText.Text = "";
                    for (int i = 0; i < report.Video.Count; i++)
                    {
                        VideoInfoText.Text += "> ";
                        if (String.IsNullOrWhiteSpace(report.Video[i].LanguageString))
                        {
                            VideoInfoText.Text += "Undetermined";
                        }
                        else
                        {
                            VideoInfoText.Text += report.Video[i].LanguageString;
                        }
                        if (!String.IsNullOrWhiteSpace(report.Video[i].Title)) VideoInfoText.Text += " - [" + report.Video[i].Title + "]";
                        if (!String.IsNullOrWhiteSpace(report.Video[i].DurationString)) VideoInfoText.Text += " - " + ManageMillisec(report.Video[i].DurationString) + " - ";
                        VideoInfoText.Text += report.Video[i].Width + "×" + report.Video[i].Height + ", ";
                        VideoInfoText.Text += report.Video[i].CodecString + " (" + report.Video[i].FormatProfile + "), ";
                        if (!String.IsNullOrWhiteSpace(report.Video[i].BitRateString)) VideoInfoText.Text += report.Video[i].BitRateString + ", ";
                        VideoInfoText.Text += report.Video[i].FrameRate.ToString().Replace(",", ".") + " fps, " + report.Video[i].AspectRatioString + ", ";
                        if (!String.IsNullOrWhiteSpace(report.Video[i].FormatSettings)) VideoInfoText.Text += "(" + report.Video[i].FormatSettings + ")" + Environment.NewLine;
                        if (report.Video[i].Delay > 0)
                        {
                            VideoInfoText.Text += " - Delay: " + report.Audio[i].Delay + "ms";
                        }
                        if (report.Video[i].Default) VideoInfoText.Text += " - Default Track";
                        if (report.Video[i].Forced) VideoInfoText.Text += " - Forced Track";

                        AudioInfoText.Text += Environment.NewLine;
                    }

                    //audio tracks
                    AudioInfoText.Text = "";
                    string[] codecAudio = report.General[0].AudioCodecList.Split('/');

                    for (int i = 0; i < report.Audio.Count; i++)
                    {
                        AudioInfoText.Text += "> ";
                        if (String.IsNullOrEmpty(report.Audio[i].LanguageString))
                        {
                            AudioInfoText.Text += "Undetermined";
                        }
                        else
                        {
                            AudioInfoText.Text += report.Audio[i].LanguageString;
                        }

                        if (!String.IsNullOrWhiteSpace(report.Audio[i].Title)) AudioInfoText.Text += " - [" + report.Audio[i].Title + "]";

                        if (!String.IsNullOrWhiteSpace(report.Audio[i].DurationStringFormatted)) AudioInfoText.Text += " - " + ManageMillisec(report.Audio[i].DurationStringFormatted) + " - ";

                        int numberofsubtracks = report.Audio[i].FormatProfile.Split('/').Length;

                        if (numberofsubtracks > 1)
                        {
                            for (int j = 0; j < numberofsubtracks; j++)
                            {
                                if (j > 0) AudioInfoText.Text += '\t';

                                if (!String.IsNullOrWhiteSpace(report.Audio[i].Format)) AudioInfoText.Text += report.Audio[i].Format.Trim(' ');

                                if (report.Audio[i].FormatProfile.Contains("/"))
                                {
                                    if (!String.IsNullOrWhiteSpace(report.Audio[i].FormatProfile.Split('/')[j])) AudioInfoText.Text += "-" + report.Audio[i].FormatProfile.Split('/')[j].Trim(' ');
                                }
                                else
                                {
                                    if (!String.IsNullOrEmpty(report.Audio[i].FormatProfile)) AudioInfoText.Text += "-" + report.Audio[i].FormatProfile;
                                }
                                if (report.Audio[i].BitRateString.Contains("/"))
                                {
                                    if (!String.IsNullOrWhiteSpace(report.Audio[i].BitRateString.Split('/')[j])) AudioInfoText.Text += ", " + report.Audio[i].BitRateString.Split('/')[j].Trim(' ');
                                }
                                else
                                {
                                    if (!String.IsNullOrEmpty(report.Audio[i].BitRateString)) AudioInfoText.Text += ", " + report.Audio[i].BitRateString;
                                }
                                if (report.Audio[i].BitRateMode.Contains("/"))
                                {
                                    if (!String.IsNullOrWhiteSpace(report.Audio[i].BitRateMode.Split('/')[j])) AudioInfoText.Text += ", " + report.Audio[i].BitRateMode.Split('/')[j].Trim(' ');
                                }
                                else
                                {
                                    if (!String.IsNullOrEmpty(report.Audio[i].BitRateMode)) AudioInfoText.Text += ", " + report.Audio[i].BitRateMode;
                                }
                                if (report.Audio[i].ChannelsString.Contains("/"))
                                {
                                    if (!String.IsNullOrWhiteSpace(report.Audio[i].ChannelsString.Split('/')[j])) AudioInfoText.Text += ", " + report.Audio[i].ChannelsString.Split('/')[j].Trim(' ');
                                }
                                else
                                {
                                    if (!String.IsNullOrEmpty(report.Audio[i].ChannelsString)) AudioInfoText.Text += ", " + report.Audio[i].ChannelsString;
                                }
                                if (report.Audio[i].BitDepthString.Contains("/"))
                                {
                                    if (!String.IsNullOrWhiteSpace(report.Audio[i].BitDepthString.Split('/')[j])) AudioInfoText.Text += ", " + report.Audio[i].BitDepthString.Split('/')[j].Trim(' ');
                                }
                                else
                                {
                                    if (!String.IsNullOrEmpty(report.Audio[i].BitDepthString)) AudioInfoText.Text += ", " + report.Audio[i].BitDepthString;
                                }
                                if (report.Audio[i].SamplingRateString.Contains("/"))
                                {
                                    if (!String.IsNullOrWhiteSpace(report.Audio[i].SamplingRateString.Split('/')[j])) AudioInfoText.Text += ", " + report.Audio[i].SamplingRateString.Split('/')[j].Trim(' ');
                                }
                                else
                                {
                                    if (!String.IsNullOrWhiteSpace(report.Audio[i].SamplingRateString)) AudioInfoText.Text += ", " + report.Audio[i].SamplingRateString;
                                }

                                if (j == 0)
                                {
                                    if (report.Audio[i].Delay > 0)
                                    {
                                        AudioInfoText.Text += " - Delay: " + report.Audio[i].Delay + "ms";
                                    }
                                    if (report.Audio[i].Default) AudioInfoText.Text += " - Default Track";
                                    if (report.Audio[i].Forced) AudioInfoText.Text += " - Forced Track";
                                }

                                AudioInfoText.Text += Environment.NewLine;
                            }
                        }
                        else
                        {
                            if (!String.IsNullOrWhiteSpace(report.Audio[i].Format)) AudioInfoText.Text += report.Audio[i].Format + " - ";
                            if (!String.IsNullOrWhiteSpace(report.Audio[i].FormatProfile)) AudioInfoText.Text += report.Audio[i].FormatProfile + ", ";
                            if (!String.IsNullOrWhiteSpace(report.Audio[i].BitRateString)) AudioInfoText.Text += report.Audio[i].BitRateString + ", ";
                            if (!String.IsNullOrWhiteSpace(report.Audio[i].BitRateMode)) AudioInfoText.Text += report.Audio[i].BitRateMode + ", ";
                            if (!String.IsNullOrWhiteSpace(report.Audio[i].ChannelsString)) AudioInfoText.Text += report.Audio[i].ChannelsString + ", ";
                            if (!String.IsNullOrWhiteSpace(report.Audio[i].BitDepthString)) AudioInfoText.Text += report.Audio[i].BitDepthString + ", ";
                            if (!String.IsNullOrWhiteSpace(report.Audio[i].SamplingRateString)) AudioInfoText.Text += report.Audio[i].SamplingRateString;

                            //delay if present
                            if (report.Audio[i].Delay > 0)
                            {
                                AudioInfoText.Text += " - Delay: " + report.Audio[i].Delay + "ms";
                            }
                            if (report.Audio[i].Default) AudioInfoText.Text += " - Default Track";
                            if (report.Audio[i].Forced) AudioInfoText.Text += " - Forced Track";

                            AudioInfoText.Text += Environment.NewLine;
                        }
                    }

                    //subtitles
                    SubInfoText.Text = "";

                    for (int i = 0; i < report.Text.Count; i++)
                    {
                        SubInfoText.Text += "> ";
                        if (String.IsNullOrEmpty(report.Text[i].LanguageString))
                        {
                            SubInfoText.Text += "Undetermined";
                        }
                        else
                        {
                            SubInfoText.Text += report.Text[i].LanguageString;
                        }
                        if (!String.IsNullOrWhiteSpace(report.Text[i].Title)) SubInfoText.Text += " - [" + report.Text[i].Title + "]";
                        SubInfoText.Text += " - " + report.Text[i].Codec;
                        if (!String.IsNullOrWhiteSpace(report.Text[i].DefaultString)) SubInfoText.Text += ", Default: " + report.Text[i].DefaultString;
                        if (!String.IsNullOrWhiteSpace(report.Text[i].ForcedString)) SubInfoText.Text += ", Forced: " + report.Text[i].ForcedString;

                        //delay if present
                        if (report.Text[i].Delay > 0)
                        {
                            SubInfoText.Text += " - Delay: " + report.Text[i].Delay + "ms" + Environment.NewLine;
                        }
                        else
                        {
                            SubInfoText.Text += Environment.NewLine;
                        }
                    }

                    if (report.Video.Count == 0) VideoInfoText.Text = "No video tracks";
                    if (report.Audio.Count == 0) AudioInfoText.Text = "No audio tracks";
                    if (report.Text.Count == 0) SubInfoText.Text = "No subtitle tracks";
                }

                //resize box
                //ResizeBox();

                //TREE
                if (!DisableTree.IsChecked.Value)
                {
                    TreeViewItem temp = null;

                    temp = new TreeViewItem();

                    temp.Header = "General";
                    temp.Items.Add(new TreeViewItem() { Header = "File Name: " + report.General[0].FileName + "." + report.General[0].FileExtension });
                    temp.Items.Add(new TreeViewItem() { Header = "Format: " + report.General[0].Format });
                    temp.Items.Add(new TreeViewItem() { Header = "File Size: " + report.General[0].FileSizePrecise });

                    if (!String.IsNullOrEmpty(ManageMillisec(report.General[0].PlayTimeFormatted))) temp.Items.Add(new TreeViewItem() { Header = "Play Time: " + ManageMillisec(report.General[0].PlayTimeFormatted) });
                    if (!String.IsNullOrEmpty(report.General[0].OveralBitRateString)) temp.Items.Add(new TreeViewItem() { Header = "Overal BitRate: " + report.General[0].OveralBitRateString });
                    if (report.Video.Count > 0) if (report.Video[0].FrameCount > 0) temp.Items.Add(new TreeViewItem() { Header = "Frame Count: " + report.Video[0].FrameCount });
                    if (!String.IsNullOrEmpty(report.General[0].Title)) temp.Items.Add(new TreeViewItem() { Header = "Title: " + report.General[0].Title });
                    if (report.Video.Count > 0) if (!String.IsNullOrEmpty(report.Video[0].MultiViewLayout)) temp.Items.Add(new TreeViewItem() { Header = "3D Mode: " + report.Video[0].MultiViewLayout });
                    if (!String.IsNullOrEmpty(report.General[0].EncodedDate)) temp.Items.Add(new TreeViewItem() { Header = "Date Encoded: " + report.General[0].EncodedDate });
                    if (!String.IsNullOrEmpty(report.General[0].EncodedLibrary)) temp.Items.Add(new TreeViewItem() { Header = "Created with: " + report.General[0].EncodedLibrary });

                    TreeReport.Items.Add(temp);

                    //video tracks tree
                    for (int i = 0; i < report.Video.Count; i++)
                    {
                        temp = null;
                        temp = new TreeViewItem();
                        temp.Header = "Video " + (i + 1);
                        if (String.IsNullOrEmpty(report.Video[i].LanguageString)) temp.Items.Add(new TreeViewItem() { Header = "Undetermined Language" });
                        else temp.Items.Add(new TreeViewItem() { Header = "Language: " + report.Video[i].LanguageString });
                        if (!String.IsNullOrEmpty(report.Video[i].Title)) temp.Items.Add(new TreeViewItem() { Header = "Track Title: " + report.Video[i].Title });
                        temp.Items.Add(new TreeViewItem() { Header = "Resolution: " + report.Video[i].Width + "×" + report.Video[i].Height });
                        temp.Items.Add(new TreeViewItem() { Header = "Aspect Ratio: " + report.Video[i].AspectRatioString });
                        temp.Items.Add(new TreeViewItem() { Header = "Framerate: " + report.Video[i].FrameRate.ToString().Replace(",", ".") + " fps" });
                        temp.Items.Add(new TreeViewItem() { Header = "Codec Video: " + report.Video[i].CodecString + " (" + report.Video[i].FormatProfile + ", " + report.Video[i].FormatSettings + ")" });
                        if (report.Video[i].Delay > 0)
                        {
                            temp.Items.Add(new TreeViewItem() { Header = "Delay: " + report.Video[i].Delay + "ms" });
                        }
                        if (!String.IsNullOrEmpty(report.Video[i].DefaultString))
                        {
                            temp.Items.Add(new TreeViewItem() { Header = "Default: " + report.Video[i].DefaultString });
                        }
                        if (!String.IsNullOrEmpty(report.Video[i].ForcedString))
                        {
                            temp.Items.Add(new TreeViewItem() { Header = "Forced: " + report.Video[i].ForcedString });
                        }
                        TreeReport.Items.Add(temp);

                    }

                    //audio tracks tree
                    for (int i = 0; i < report.Audio.Count; i++)
                    {
                        temp = null;
                        temp = new TreeViewItem();
                        temp.Header = "Audio " + (i + 1);
                        if (String.IsNullOrEmpty(report.Audio[i].LanguageString)) temp.Items.Add(new TreeViewItem() { Header = "Undetermined Language" });
                        else temp.Items.Add(new TreeViewItem() { Header = "Language: " + report.Audio[i].LanguageString });
                        if (!String.IsNullOrEmpty(report.Audio[i].Title)) temp.Items.Add(new TreeViewItem() { Header = "Track Title: " + report.Audio[i].Title });
                        if (!String.IsNullOrWhiteSpace(report.Audio[i].Format)) temp.Items.Add(new TreeViewItem() { Header = "Format: " + report.Audio[i].Format.Trim(' ') });

                        int numberofsubtracks = report.Audio[i].FormatProfile.Split('/').Length;

                        //composed tracks with subtracks
                        if (numberofsubtracks > 1)
                        {
                            TreeViewItem sub = null;

                            for (int j = 0; j < numberofsubtracks; j++)
                            {
                                sub = null;
                                sub = new TreeViewItem();
                                if (!String.IsNullOrWhiteSpace(report.Audio[i].Format)) sub.Header = report.Audio[i].Format;
                                if (report.Audio[i].FormatProfile.Contains("/"))
                                {
                                    if (!String.IsNullOrWhiteSpace(report.Audio[i].FormatProfile.Split('/')[j]))
                                    {
                                        sub.Items.Add(new TreeViewItem() { Header = "Profile: " + report.Audio[i].FormatProfile.Split('/')[j].Trim(' ') });
                                        if (!String.IsNullOrWhiteSpace(sub.Header.ToString())) sub.Header += " - " + report.Audio[i].FormatProfile.Split('/')[j].Trim(' ');
                                        else sub.Header = report.Audio[i].FormatProfile.Split('/')[j].Trim(' ');
                                    }
                                }
                                else
                                {
                                    if (!String.IsNullOrEmpty(report.Audio[i].FormatProfile))
                                    {
                                        sub.Items.Add(new TreeViewItem() { Header = "Profile: " + report.Audio[i].FormatProfile });
                                        if (!String.IsNullOrWhiteSpace(sub.Header.ToString())) sub.Header += " - " + report.Audio[i].FormatProfile;
                                        else sub.Header = report.Audio[i].FormatProfile;
                                    }
                                }
                                if (report.Audio[i].BitRateString.Contains("/"))
                                {
                                    if (!String.IsNullOrWhiteSpace(report.Audio[i].BitRateString.Split('/')[j])) sub.Items.Add(new TreeViewItem() { Header = "BitRate: " + report.Audio[i].BitRateString.Split('/')[j].Trim(' ') });
                                }
                                else
                                {
                                    if (!String.IsNullOrEmpty(report.Audio[i].BitRateString)) sub.Items.Add(new TreeViewItem() { Header = "BitRate: " + report.Audio[i].BitRateString });
                                }
                                if (report.Audio[i].BitRateMode.Contains("/"))
                                {
                                    if (!String.IsNullOrWhiteSpace(report.Audio[i].BitRateMode.Split('/')[j])) sub.Items.Add(new TreeViewItem() { Header = "BitRate Mode: " + report.Audio[i].BitRateMode.Split('/')[j].Trim(' ') });
                                }
                                else
                                {
                                    if (!String.IsNullOrEmpty(report.Audio[i].BitRateMode)) sub.Items.Add(new TreeViewItem() { Header = "BitRate Mode: " + report.Audio[i].BitRateMode });
                                }
                                if (report.Audio[i].ChannelsString.Contains("/"))
                                {
                                    if (!String.IsNullOrWhiteSpace(report.Audio[i].ChannelsString.Split('/')[j])) sub.Items.Add(new TreeViewItem() { Header = "Channel: " + report.Audio[i].ChannelsString.Split('/')[j].Trim(' ') });
                                }
                                else
                                {
                                    if (!String.IsNullOrEmpty(report.Audio[i].ChannelsString)) sub.Items.Add(new TreeViewItem() { Header = "Channel: " + report.Audio[i].ChannelsString });
                                }
                                if (report.Audio[i].BitDepthString.Contains("/"))
                                {
                                    if (!String.IsNullOrWhiteSpace(report.Audio[i].BitDepthString.Split('/')[j])) sub.Items.Add(new TreeViewItem() { Header = "Bit Depth: " + report.Audio[i].BitDepthString.Split('/')[j].Trim(' ') });
                                }
                                else
                                {
                                    if (!String.IsNullOrEmpty(report.Audio[i].BitDepthString)) sub.Items.Add(new TreeViewItem() { Header = "Bit Depth: " + report.Audio[i].BitDepthString });
                                }
                                if (report.Audio[i].SamplingRateString.Contains("/"))
                                {
                                    if (!String.IsNullOrWhiteSpace(report.Audio[i].SamplingRateString.Split('/')[j])) sub.Items.Add(new TreeViewItem() { Header = "Sampling Rate: " + report.Audio[i].SamplingRateString.Split('/')[j].Trim(' ') });
                                }
                                else
                                {
                                    if (!String.IsNullOrEmpty(report.Audio[i].SamplingRateString)) sub.Items.Add(new TreeViewItem() { Header = "Sampling Rate: " + report.Audio[i].SamplingRateString });
                                }
                                temp.Items.Add(sub);
                            }


                        }
                        else //single tracks without subtracks
                        {
                            if (!String.IsNullOrEmpty(report.Audio[i].FormatProfile)) temp.Items.Add(new TreeViewItem() { Header = "Profile: " + report.Audio[i].FormatProfile });
                            if (!String.IsNullOrEmpty(report.Audio[i].BitRateString)) temp.Items.Add(new TreeViewItem() { Header = "BitRate: " + report.Audio[i].BitRateString });
                            if (!String.IsNullOrEmpty(report.Audio[i].BitRateMode)) temp.Items.Add(new TreeViewItem() { Header = "BitRate Mode: " + report.Audio[i].BitRateMode });
                            if (!String.IsNullOrEmpty(report.Audio[i].ChannelsString)) temp.Items.Add(new TreeViewItem() { Header = "Channels: " + report.Audio[i].ChannelsString });
                            if (!String.IsNullOrEmpty(report.Audio[i].SamplingRateString)) temp.Items.Add(new TreeViewItem() { Header = "Sampling Rate: " + report.Audio[i].SamplingRateString });


                        }
                        if (report.Audio[i].Delay > 0)
                        {
                            temp.Items.Add(new TreeViewItem() { Header = "Delay: " + report.Audio[i].Delay + "ms" });
                        }
                        if (!String.IsNullOrEmpty(report.Audio[i].DefaultString))
                        {
                            temp.Items.Add(new TreeViewItem() { Header = "Default: " + report.Audio[i].DefaultString });
                        }
                        if (!String.IsNullOrEmpty(report.Audio[i].ForcedString))
                        {
                            temp.Items.Add(new TreeViewItem() { Header = "Forced: " + report.Audio[i].ForcedString });
                        }

                        TreeReport.Items.Add(temp);
                    }

                    //subtitle tracks tree 
                    for (int i = 0; i < report.Text.Count; i++)
                    {
                        temp = null;
                        temp = new TreeViewItem();
                        temp.Header = "Subtitle " + (i + 1);
                        if (String.IsNullOrEmpty(report.Text[i].LanguageString)) temp.Items.Add(new TreeViewItem() { Header = "Undetermined Language" });
                        else temp.Items.Add(new TreeViewItem() { Header = "Language: " + report.Text[i].LanguageString });
                        if (!String.IsNullOrEmpty(report.Text[i].Title)) temp.Items.Add(new TreeViewItem() { Header = "Track Title: " + report.Text[i].Title });
                        temp.Items.Add(new TreeViewItem() { Header = "Codec: " + report.Text[i].Codec });

                        TreeReport.Items.Add(temp);
                    }
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the SaveReport control. Save the report of the file in the same pa
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void SaveReport_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog Report = new SaveFileDialog();
            Report.Filter = "Simple Text|*.txt";
            Report.Title = "Select the file you want to use to save the report";
            Report.DefaultExt = ".txt";
            Report.FileName = System.IO.Path.GetFileNameWithoutExtension(filepathBox.Text) + ".txt";
            Report.InitialDirectory = System.IO.Path.GetDirectoryName(filepathBox.Text);
            Report.ShowDialog();
            if (!String.IsNullOrEmpty(Report.FileName))
            {
                StreamWriter File = new StreamWriter(Report.FileName);
                if (tabControl.SelectedIndex == 2) File.WriteLine(TextBoxComplete.Text);
                else File.WriteLine(TextBoxStandard.Text);
                File.Close();
            }
        }

        /// <summary>
        /// Handles the Checked event of the DisableSimpleInfo control. Disable Simple Report generation.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void DisableSimpleInfo_Checked(object sender, RoutedEventArgs e)
        {
            var tab = tabControl.Items[0] as TabItem;
            tab.Visibility = Visibility.Hidden;
            tab.Width = 0;
            CreateSimpleReport();
        }

        /// <summary>
        /// Handles the Unchecked event of the DisableSimpleInfo control. Enable Simple Report generation.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void DisableSimpleInfo_Unchecked(object sender, RoutedEventArgs e)
        {
            var tab = tabControl.Items[0] as TabItem;
            tab.Visibility = Visibility.Visible;
            tab.Visibility = Visibility.Visible;
            tab.Width = 52;
            CreateSimpleReport();
        }

        /// <summary>
        /// Handles the Checked event of the DisableSimpleInfo control. Disable Simple Report generation.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void DisableTreeInfo_Checked(object sender, RoutedEventArgs e)
        {
            var tab = tabControl.Items[3] as TabItem;
            tab.Visibility = Visibility.Hidden;
            tab.Width = 0;
            while (TreeReport.HasItems)
            {
                TreeReport.Items.RemoveAt(0);
            }
            CreateSimpleReport();
        }

        /// <summary>
        /// Handles the Unchecked event of the DisableSimpleInfo control. Enable Simple Report generation.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void DisableTreeInfo_Unchecked(object sender, RoutedEventArgs e)
        {
            var tab = tabControl.Items[3] as TabItem;
            tab.Visibility = Visibility.Visible;
            tab.Width = 38;
            CreateSimpleReport();
        }

        /// <summary>
        /// Handles the Change event of the MS control. Enable or disable Milliseconds in simple report.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void MS_Change(object sender, RoutedEventArgs e)
        {
            SubInfoText.Text = AudioInfoText.Text = VideoInfoText.Text = GeneralInfoText.Text = String.Empty;
            CreateSimpleReport();
        }

        /// <summary>
        /// Handles the Checked event of the AutoSaveReport control. Enable auto file report save.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void AutoSaveReport_Checked(object sender, RoutedEventArgs e)
        {
            SaveReport.IsEnabled = false;
        }

        /// <summary>
        /// Handles the Unchecked event of the AutoSaveReport control. Disable auto file report save.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void AutoSaveReport_Unchecked(object sender, RoutedEventArgs e)
        {
            SaveReport.IsEnabled = SaveReportActive;
        }

        /// <summary>
        /// Saves user configuration.
        /// </summary>
        private void SaveConfig()
        {
            try
            {
                if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\userconfig.xml"))
                {
                    File.Delete(System.Windows.Forms.Application.StartupPath + "\\userconfig.xml");
                }

                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;

                XmlWriter XmlWriter = XmlWriter.Create(System.Windows.Forms.Application.StartupPath + "\\userconfig.xml", settings);

                XmlWriter.WriteStartDocument();
                XmlWriter.WriteComment("GetMediaInfo GUI user configuration");
                XmlWriter.WriteComment("DO NOT MODIFY VALUES");
                XmlWriter.WriteStartElement("database");

                XmlWriter.WriteStartElement("config");
                XmlWriter.WriteAttributeString("tag", "AutoSaveReport");
                if ((bool)AutoSaveReport.IsChecked)
                {
                    XmlWriter.WriteValue("true");
                }
                else
                {
                    XmlWriter.WriteValue("false");
                }
                XmlWriter.WriteEndElement();

                XmlWriter.WriteStartElement("config");
                XmlWriter.WriteAttributeString("tag", "DisableSimpleReport");
                if ((bool)DisableSimpleInfo.IsChecked)
                {
                    XmlWriter.WriteValue("true");
                }
                else
                {
                    XmlWriter.WriteValue("false");
                }
                XmlWriter.WriteEndElement();

                XmlWriter.WriteStartElement("config");
                XmlWriter.WriteAttributeString("tag", "DisableTree");
                if ((bool)DisableTree.IsChecked)
                {
                    XmlWriter.WriteValue("true");
                }
                else
                {
                    XmlWriter.WriteValue("false");
                }
                XmlWriter.WriteEndElement();

                XmlWriter.WriteStartElement("config");
                XmlWriter.WriteAttributeString("tag", "ShowMilliSec");
                if ((bool)MilliSec.IsChecked)
                {
                    XmlWriter.WriteValue("true");
                }
                else
                {
                    XmlWriter.WriteValue("false");
                }
                XmlWriter.WriteEndElement();

                XmlWriter.WriteStartElement("config");
                XmlWriter.WriteAttributeString("tag", "RemoveSpace");
                if ((bool)RemoveSpace.IsChecked)
                {
                    XmlWriter.WriteValue("true");
                }
                else
                {
                    XmlWriter.WriteValue("false");
                }
                XmlWriter.WriteEndElement();

                XmlWriter.WriteEndElement();
                XmlWriter.WriteEndDocument();
                XmlWriter.Close();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Unable to save user configuration. (" + ex.Message + ").");
            }
        }

        /// <summary>
        /// Restores user configuration.
        /// </summary>
        public void RestoreConfig()
        {
            if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\userconfig.xml"))
            {
                try
                {
                    XDocument xml = XDocument.Load(System.Windows.Forms.Application.StartupPath + "\\userconfig.xml");

                    foreach (XElement config in xml.Element("database").Elements("config"))
                    {
                        string configuration = config.Attribute("tag").Value;
                        bool value = false;

                        if (!bool.TryParse(config.Value, out value))
                        {
                            System.Windows.Forms.MessageBox.Show("Unable to restore user configuration. XML file is corrupted! (Unable to parse boolean from xml value).");
                        }

                        switch (configuration)
                        {
                            case "AutoSaveReport":
                                AutoSaveReport.IsChecked = value;
                                break;
                            case "DisableSimpleReport":
                                DisableSimpleInfo.IsChecked = value;
                                break;
                            case "DisableTree":
                                DisableTree.IsChecked = value;
                                break;
                            case "ShowMilliSec":
                                MilliSec.IsChecked = value;
                                break;
                            case "RemoveSpace":
                                RemoveSpace.IsChecked = value;
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Unable to restore user configuration. XML file is corrupted! (" + ex.Message + ").");
                }
            }
            else
            {
                SaveConfig();
            }
        }

        /// <summary>
        /// Reload standard and complete report at checked/unchecked the checkbox
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">instance containing the event data.</param>
        private void RemoveSpace_Reload(object sender, RoutedEventArgs e)
        {
            ShowReportsAndSaveToDisk();
        }

        /// <summary>
        /// Open the FontDialog
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">instance containing the event data.</param>
        private void font_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FontDialog fontDialog = new System.Windows.Forms.FontDialog();
            FontFamilyConverter ffc = new FontFamilyConverter();
            fontDialog.MinSize = 10;
            fontDialog.MaxSize = 40;

            if (fontDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {


                this.TextBoxStandard.FontSize = fontDialog.Font.Size;
                this.TextBoxStandard.FontFamily = (FontFamily)ffc.ConvertFromString(fontDialog.Font.Name);
                this.TextBoxComplete.FontSize = fontDialog.Font.Size;
                this.TextBoxComplete.FontFamily = (FontFamily)ffc.ConvertFromString(fontDialog.Font.Name);

                if (fontDialog.Font.Bold) TextBoxComplete.FontWeight = TextBoxStandard.FontWeight = FontWeights.Bold;
                else
                    TextBoxComplete.FontWeight = TextBoxStandard.FontWeight = FontWeights.Normal;

                if (fontDialog.Font.Italic)
                    TextBoxComplete.FontStyle = TextBoxStandard.FontStyle = FontStyles.Italic;
                else
                    TextBoxComplete.FontStyle = TextBoxStandard.FontStyle = FontStyles.Normal;
            }
        }

        /// <summary>
        /// Check for Updates
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">instance containing the event data.</param>
        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\7z.dll") && File.Exists(System.Windows.Forms.Application.StartupPath + "\\SevenZipSharp.dll"))
            {
                string currmediainfo = String.Empty;

                if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\Mediainfo.dll"))
                {
                    FileVersionInfo tmp = FileVersionInfo.GetVersionInfo(System.Windows.Forms.Application.StartupPath + "\\Mediainfo.dll");

                    if (!String.IsNullOrEmpty(tmp.FileVersion))
                    {
                        currmediainfo = tmp.FileVersion;
                    }
                }

                string newvermediainfo = GetLatestMediainfoVersion();

                if (!currmediainfo.Contains(newvermediainfo))
                {
                    UpdaterProgress.Content = "Found newer Mediainfo version! (" + newvermediainfo + ")";

                    string url = "http://mediaarea.net/download/binary/libmediainfo0/" + newvermediainfo + "/MediaInfo_DLL_" + newvermediainfo + "_Windows_x64_WithoutInstaller.7z";
                    string filename = url.Substring(url.LastIndexOf("/") + 1, url.Length - url.LastIndexOf("/") - 1);
                    string filetoextract = "MediaInfo.dll";
                    string newfilepath = System.Windows.Forms.Application.StartupPath + "\\" + filetoextract;
                    string downloadpath = System.Windows.Forms.Application.StartupPath + "\\tmp\\" + filename;

                    if (!Directory.Exists(System.Windows.Forms.Application.StartupPath + "\\tmp\\"))
                    {
                        Directory.CreateDirectory(System.Windows.Forms.Application.StartupPath + "\\tmp\\");
                    }

                    // Starts the download
                    WebClient client = new WebClient();
                    UpdaterProgress.Content = "Downloading...";
                    client.DownloadFile(new Uri(url), downloadpath);
                    UpdaterProgress.Content = "Download Completed!";

                    //decompress
                    SevenZip.SevenZipCompressor.SetLibraryPath(System.Windows.Forms.Application.StartupPath + "\\7z.dll");
                    using (SevenZipExtractor archive = new SevenZipExtractor(downloadpath))
                    {
                        UpdaterProgress.Content = "Deleting old files...";

                        if (File.Exists(newfilepath))
                        {
                            try
                            {
                                File.Delete(newfilepath);
                            }
                            catch (Exception ex)
                            {
                                System.Windows.Forms.MessageBox.Show("File " + filename + " cannot be extracted. Error message: " + ex.Message);
                                return;
                            }
                        }

                        UpdaterProgress.Content = "Extracting...";

                        var fileStream = File.Create(newfilepath);

                        foreach (string file in archive.ArchiveFileNames)
                        {
                            if (file.Contains(filetoextract))
                            {
                                filetoextract = file;
                                break;
                            }
                        }

                        archive.ExtractFile(filetoextract, fileStream);
                        UpdaterProgress.Content = "Extraction Completed!";

                        StatusMediainfo.Text = "OK v" + newvermediainfo;
                        StatusMediainfo.Foreground = Brushes.Green;

                        client.Dispose();
                        archive.Dispose();

                        try
                        {
                            File.Delete(downloadpath);
                            Directory.Delete(System.Windows.Forms.Application.StartupPath + "\\tmp\\", true);
                        }
                        catch (Exception ex)
                        {
                            System.Windows.Forms.MessageBox.Show(ex.Message);
                        }

                        UpdaterProgress.Content = "Successfully Updated!";
                    }
                }
                else
                {
                    UpdaterProgress.Content = "Mediainfo is already at the latest version.";
                }
            }
            else
            {
                UpdaterProgress.Content = "Can't update, libraries 7z.dll and/or  SevenZipSharp.dll absent.";
                System.Windows.Forms.MessageBox.Show("libraries 7z.dll and/or  SevenZipSharp.dll absent.", "Can't update", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }

        /// <summary>
        /// Gets the latest mediainfo version.
        /// </summary>
        /// <returns>String as the latest available mediainfo version</returns>
        private string GetLatestMediainfoVersion()
        {
            XmlReader reader = XmlTextReader.Create("https://mediaarea.net/rss/mediainfo_updates.xml");
            reader.ReadToFollowing("title");
            reader.ReadToFollowing("title");
            string latest = reader.ReadElementContentAsString();
            latest = latest.Replace("MediaInfo ", "");
            return latest;
        }

    }

    /// <summary>
    /// TextUtils Class
    /// </summary>
    public static class TextUtils
    {
        /// <summary>
        /// Gets the lines.
        /// </summary>
        /// <param name="source">The source of text as TextBlock</param>
        /// <returns>Enumerable collection of all lines in the input control</returns>
        public static IEnumerable<string> GetLines(this TextBlock source)
        {
            var text = source.Text;
            int offset = 0;
            System.Windows.Documents.TextPointer lineStart = source.ContentStart.GetPositionAtOffset(1, System.Windows.Documents.LogicalDirection.Forward);
            do
            {
                System.Windows.Documents.TextPointer lineEnd = lineStart != null ? lineStart.GetLineStartPosition(1) : null;
                int length = lineEnd != null ? lineStart.GetOffsetToPosition(lineEnd) : text.Length - offset;
                yield return text.Substring(offset, length);
                offset += length;
                lineStart = lineEnd;
            }
            while (lineStart != null);
        }
    }
}
