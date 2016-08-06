using GetMediaInfos;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Linq;

namespace GetMediaInfoGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// <seealso cref="Window" />
    public partial class MainWindow : Window
    {
        #region GlobalVariables
        /// <summary>
        /// Boolean that define if standard / complete report are bold
        /// </summary>
        bool Bold;

        /// <summary>
        /// Boolean that define if standard / complete report are italic
        /// </summary>
        bool Italic;

        /// <summary>
        /// Generated mediainfo report
        /// </summary>
        MediaInfo report;
        #endregion

        #region Constructor 
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            System.Windows.Forms.Application.EnableVisualStyles();
            ClearAll();

            if (!File.Exists(Path.Combine(System.Windows.Forms.Application.StartupPath, "icon.ico")))
            {
                Uri uri = new Uri("/GetMediaInfoGUI;component/icon.ico", UriKind.Relative);
                System.Drawing.Image img = System.Drawing.Image.FromStream(System.Windows.Application.GetResourceStream(uri).Stream);
                img.Save("icon.ico", System.Drawing.Imaging.ImageFormat.Icon);
                img.Dispose();
            }
        }
        #endregion

        #region InterfaceHandlers

        /// <summary>
        /// Handler for Loaded event of the Window. Contains functions to inizializate program, check for dependencies and restore user config.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Window_Loaded(object sender, EventArgs e)
        {
            Version.Content = "Version " + System.Windows.Forms.Application.ProductVersion;

            VerifyMediaInfoVersion();
            VerifyGetMediaInfosVersion();

            ProcessArgs();
            ComboBoxFont.SelectedValue = TextBoxComplete.FontFamily;
            ComboBoxSize.SelectedValue = TextBoxComplete.FontSize;
            RestoreConfig();
            StatusLab.Content = "Ready.";
            StatusLab.InvalidateVisual();
        }

        /// <summary>
        /// Handler for Closing event of the Window: saves user configuration.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Windows_Closing(object sender, EventArgs e)
        {
            SaveConfig();
        }

        /// <summary>
        /// Handles the Click event of the AddContextMenu control: it adds context menu GetMediaInfos entry for specific extensions.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void AddContextMenu_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.RegistryKey key;

            try
            {
                // for all files
                key = Microsoft.Win32.Registry.CurrentUser
                    .OpenSubKey("Software", true)
                    .OpenSubKey("Classes", true);
                key = key.CreateSubKey("*");
                key = key.CreateSubKey("shell");
                key = key.CreateSubKey("open_with_getmediainfosgui_all");
                key.SetValue("", "GetMediaInfo");
                key.SetValue("Icon", "\"" + System.Windows.Forms.Application.StartupPath + "\\icon.ico\"");
                key = key.CreateSubKey("command");
                key.SetValue("", "\"" + System.Windows.Forms.Application.ExecutablePath + "\" \"%1\"");
                key.Close();
                // end 
                AssociationStatus.Content = "Association has been completed.";
            }
            catch (Exception ex)
            {
                AssociationStatus.Content = "Association failed: " + ex.Message + ".";
            }

        }

        /// <summary>
        /// Handles the Click event of the RemoveContextMenu control: it removes context menu GetMediaInfos entry for specific extensions.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void RemoveContextMenu_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.RegistryKey key;
            try
            {
                key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software", true)
                    .OpenSubKey("Classes", true)
                    .OpenSubKey("*", true)
                    .CreateSubKey("shell");
                key.DeleteSubKeyTree("open_with_getmediainfosgui_all");
                key.Close();

                AssociationStatus.Content = "Association has been successfully removed.";
            }
            catch (Exception ex)
            {
                AssociationStatus.Content = "Association not removed: " + ex.Message + ".";
            }

        }

        /// <summary>
        /// Handles the Click event of the OpenFile control: is used to open a new OpenFileDialog to let user to select the input file.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog file = new System.Windows.Forms.OpenFileDialog();
            file.Filter = "Mediafile|*.*";
            file.Title = "Select a Media File";
            file.Multiselect = false;

            if (file.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                FilePathBox.Text = file.FileName;
            }
        }

        /// <summary>
        /// Handles the Click event of the SaveReport control: save the report of the file in the same directory.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void SaveReport_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog Report = new System.Windows.Forms.SaveFileDialog();
            Report.Filter = "Simple Text|*.txt";
            Report.Title = "Select the file you want to use to save the report";
            Report.DefaultExt = ".txt";
            Report.FileName = Path.GetFileNameWithoutExtension(FilePathBox.Text) + ".txt";
            Report.InitialDirectory = Path.GetDirectoryName(FilePathBox.Text);
            if (Report.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (!string.IsNullOrEmpty(Report.FileName))
                {
                    StreamWriter File = new StreamWriter(Report.FileName);
                    if (Pages.SelectedIndex == 2) File.WriteLine(TextBoxComplete.Text);
                    else File.WriteLine(TextBoxStandard.Text);
                    File.Close();
                }
            }
        }

        /// <summary>
        /// Handles the TextChanged event of FilePathBox control: analyze declared path in filepathBox.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void FilePathBox_TextChanged(object sender, EventArgs e)
        {
            AnalyzeFile(FilePathBox.Text);
        }

        /// <summary>
        /// Handles the Drop event: analyze dropped file.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DragEventArgs"/> instance containing the event data.</param>
        private void Windows_Drop(object sender, DragEventArgs e)
        {
            System.Windows.DataObject data = (System.Windows.DataObject)e.Data;
            if (data.ContainsFileDropList())
            {
                string[] FileList = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop, false);
                FilePathBox.Text = FileList[0];
            }
        }

        /// <summary>
        /// Handles the DragEnter event: copy the effects.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DragEventArgs"/> instance containing the event data.</param>
        private void Windows_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
        }

        /// <summary>
        /// Handles the Checked event of the DisableSimpleInfo control: disable Simple Report generation.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void DisableSimpleInfo_Checked(object sender, RoutedEventArgs e)
        {
            TabItem tab = Pages.Items[0] as TabItem;
            tab.Visibility = Visibility.Collapsed;
            TextInfoText.Text = AudioInfoText.Text = VideoInfoText.Text = GeneralInfoText.Text = string.Empty;
        }

        /// <summary>
        /// Handles the Unchecked event of the DisableSimpleInfo control: enable Simple Report generation.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void DisableSimpleInfo_Unchecked(object sender, RoutedEventArgs e)
        {
            TabItem tab = Pages.Items[0] as TabItem;
            tab.Visibility = Visibility.Visible;
            StatusLab.Content = "Creating Simple Report...";
            StatusLab.InvalidateVisual();
            CreateSimpleReport();
            StatusLab.Content = "Ready.";
            StatusLab.InvalidateVisual();
        }

        /// <summary>
        /// Handles the Checked event of the DisableSimpleInfo control: disable Simple Report generation.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void DisableTreeInfo_Checked(object sender, RoutedEventArgs e)
        {
            TabItem tab = Pages.Items[3] as TabItem;
            tab.Visibility = Visibility.Hidden;
            tab.Width = 0;
        }

        /// <summary>
        /// Handles the Unchecked event of the DisableSimpleInfo control: enable Simple Report generation.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void DisableTreeInfo_Unchecked(object sender, RoutedEventArgs e)
        {
            TabItem tab = Pages.Items[3] as TabItem;
            tab.Visibility = Visibility.Visible;
            tab.Width = 38;
            StatusLab.Content = "Creating Tree Report...";
            StatusLab.InvalidateVisual();
            CreateTreeReport();
            StatusLab.Content = "Ready.";
            StatusLab.InvalidateVisual();
        }

        /// <summary>
        /// Handles the Change event of the EnableMilliSecond control: enable or disable Milliseconds in simple report.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void EnableMilliSecond_CheckedUnchecked(object sender, RoutedEventArgs e)
        {
            TextInfoText.Text = AudioInfoText.Text = VideoInfoText.Text = GeneralInfoText.Text = string.Empty;

            StatusLab.Content = "Creating Simple Report...";
            StatusLab.InvalidateVisual();
            CreateSimpleReport();
            StatusLab.Content = "Creating Tree Report...";
            StatusLab.InvalidateVisual();
            CreateTreeReport();
            StatusLab.Content = "Ready.";
            StatusLab.InvalidateVisual();


        }

        /// <summary>
        /// Handles the Checked and Unchecked event of the AutoSaveReport control: enable auto file report save.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void AutoSaveReport_CheckedUnchecked(object sender, RoutedEventArgs e)
        {
            ReloadEnabledSaveButton();
        }

        /// <summary>
        /// Handles the Click of the Version control: show the changelog.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Version_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.MessageBox.Show(
                "- Chapter count" + Environment.NewLine +
                "- Bug Fixed", "New in " + System.Windows.Forms.Application.ProductVersion);
        }

        /// <summary>
        /// Handles the Checked and Unchecked event of the RemoveSpace control: reload standard and complete report.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void RemoveSpace_CheckedUnchecked(object sender, RoutedEventArgs e)
        {
            ShowReportsAndSaveToDisk();
        }

        /// <summary>
        /// Handles the Click event of the UpdateButton control: check for Updates
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateDLL();
        }

        /// <summary>
        /// Handles the Click event of the CopyAll controls in Context Menu: select all and copy text in the standard and complete report.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void CopyAll_Click(object sender, RoutedEventArgs e)
        {
            if (Pages.SelectedIndex == 2)
            {
                TextBoxComplete.SelectAll();
                TextBoxComplete.Copy();

            }
            else if (Pages.SelectedIndex == 1)
            {
                TextBoxStandard.SelectAll();
                TextBoxStandard.Copy();
            }
        }

        /// <summary>
        /// Handles the Click event of the BoldControl control: change the bold Setting in the standard and complete report.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void BoldControl_Click(object sender, RoutedEventArgs e)
        {
            Bold = !Bold;
            BoldChange();
        }

        /// <summary>
        /// Handles the Click event of the ItalicControl control: change the bold Setting in the standard and complete report.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void ItalicControl_Click(object sender, RoutedEventArgs e)
        {
            Italic = !Italic;
            ItalicChange();
        }

        /// <summary>
        /// Handles the SelectionChanged event of the ComboBoxFont control: change the font Setting in the standard and complete report.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void ComboBoxFont_SelectionChanged(object sender = null, SelectionChangedEventArgs e = null)
        {
            if (ComboBoxFont.SelectedValue != null)
                TextBoxComplete.FontFamily = TextBoxStandard.FontFamily = (FontFamily)ComboBoxFont.SelectedValue;
        }

        /// <summary>
        /// Handles the SelectionChanged event of the ComboBoxSize control: change the size Setting in the standard and complete report.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void ComboBoxSize_SelectionChanged(object sender = null, SelectionChangedEventArgs e = null)
        {
            if (ComboBoxSize.SelectedValue != null)
                TextBoxComplete.FontSize = TextBoxStandard.FontSize = (double)ComboBoxSize.SelectedValue;
        }

        /// <summary>
        /// Handles for the Click event of the SaveCover control: save the cover.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void SaveCover_Click(object sender, RoutedEventArgs e)
        {
            if (report.General[0].Cover)
            {
                System.Windows.Forms.SaveFileDialog dialog = new System.Windows.Forms.SaveFileDialog();
                dialog.Filter = "JPEG Image|*.jpeg";
                dialog.Title = "Select the file you want to use to save the cover";
                dialog.DefaultExt = ".jpeg";
                dialog.FileName = "cover";
                dialog.InitialDirectory = report.General[0].FolderName;
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {

                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(GetCover));
                    using (FileStream filestream = new FileStream(dialog.FileName, FileMode.Create))
                    {
                        encoder.Save(filestream);
                    }
                }
            }
        }

        /// <summary>
        /// Handles for enable Drag/Drop on report TextBox
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DragEventArgs"/> instance containing the event data.</param>
        private void TextBoxReport_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        #endregion

        #region HelperMethods

        /// <summary>
        /// Method for analyze arguments passed to the application. If the arguments count is greater than 1 analyze the first one file.
        /// </summary>
        private void ProcessArgs()
        {
            string[] arguments = Environment.GetCommandLineArgs();
            if (arguments.Length > 1)
            {
                FilePathBox.Text = arguments[1];
                AnalyzeFile(arguments[1]);
            }
        }

        /// <summary>
        /// Method for analyze the specified path.
        /// </summary>
        /// <param name="path">Path to file to analyze.</param>
        private void AnalyzeFile(string path)
        {
            ClearAll();
            if (File.Exists(path))
            {
                StatusLab.Content = "Analysing...";
                StatusLab.InvalidateVisual();
                Stopwatch sw = new Stopwatch();
                sw.Start();
                report = new MediaInfo(path);
                sw.Stop();
                StatusLab.Content = "Creating Simple Report...";
                StatusLab.InvalidateVisual();
                CreateSimpleReport();
                StatusLab.Content = "Creating Tree Report...";
                StatusLab.InvalidateVisual();
                CreateTreeReport();
                ShowReportsAndSaveToDisk();
                this.Title += " (" + Path.GetFileName(path) + ")";
                StatusLab.Content = "Analysis Completed. (" + sw.ElapsedMilliseconds + "ms)";
                StatusLab.InvalidateVisual();
                ReloadEnabledSaveButton();
                ReloadEnabledCopyAllButton();
            }
            else if (string.IsNullOrWhiteSpace(path))
            {
                StatusLab.Content = "Ready.";
                StatusLab.InvalidateVisual();
            }
            else
            {
                StatusLab.Content = "Not existing file.";
                StatusLab.InvalidateVisual();
                ReloadEnabledSaveButton();
                ReloadEnabledCopyAllButton();
            }
        }

        /// <summary>
        /// Method for clear all variables and the interface.
        /// </summary>
        private void ClearAll()
        {
            StatusLab.Content = "Cleaning all...";
            StatusLab.InvalidateVisual();
            TextBoxComplete.Text = TextBoxStandard.Text = string.Empty;
            this.Title = "GetMediaInfo";
            cover.Source = null;
            report = null;
            ReloadEnabledSaveButton();
            ReloadEnabledCopyAllButton();
            TVideo.Visibility = TAudio.Visibility = TText.Visibility = Visibility.Visible;
            TextInfoText.Text = AudioInfoText.Text = VideoInfoText.Text = GeneralInfoText.Text = GeneralInfoText.Text = string.Empty;
            TreeReport.RemoveItems();

        }

        /// <summary>
        /// Method for create the file report and save it to disk as txt file if AutoSaveReport is checked.
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

                if (AutoSaveReport.IsChecked.GetValueOrDefault(false))
                {
                    string ReportName = Path.ChangeExtension(FilePathBox.Text, ".txt");
                    int i = 2;
                    while (File.Exists(ReportName))
                    {
                        ReportName = Path.ChangeExtension(FilePathBox.Text, i + ".txt");
                        i++;
                    }
                    StreamWriter ReportTxt = new StreamWriter(ReportName);
                    ReportTxt.WriteLine(TextBoxStandard.Text);
                    ReportTxt.Close();
                }
            }
        }

        /// <summary>
        /// Method for management time strings. If the checkbox to show the milliseconds is inactive he removes them.
        /// </summary>
        /// <param name="time">string with timecode.</param>
        /// <returns>If the checkbox is not active splitted returns a string, otherwise the original passed as input.</returns>
        private string ManageMilliSec(string time)
        {
            return EnableMilliSecond.IsChecked.GetValueOrDefault(false) ? time : time.Split('.')[0];
        }

        /// <summary>
        /// Return Bitmap Image if present in the input files report.
        /// </summary>
        /// <value>
        /// Image Cover for WPF.
        /// </value>
        private BitmapImage GetCover
        {
            get
            {
                if (report.General[0].Cover)
                {
                    BitmapImage image = new BitmapImage();
                    using (MemoryStream mem = new MemoryStream(report.General[0].CoverData))
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
                else return null;
            }
        }

        /// <summary>
        /// Create the Simple report from a Mediainfo report.
        /// </summary>
        private void CreateSimpleReport()
        {
            if (report != null)
            {
                if (!DisableSimpleInfo.IsChecked.Value)
                {
                    // generals track
                    #region GeneralTrack
                    GeneralInfoText.Text = string.Empty;

                    foreach (GeneralTrack General in report.General)
                    {
                        GeneralInfoText.Text += "File Name: " + General.FileName + "." + General.FileExtension + Environment.NewLine;
                        GeneralInfoText.Text += "Format: " + General.Format + Environment.NewLine;
                        GeneralInfoText.Text += "File Size: " + General.FileSizePrecise + Environment.NewLine;
                        if (!string.IsNullOrWhiteSpace(General.PlayTimeFormatted)) GeneralInfoText.Text += "Duration: " + ManageMilliSec(General.PlayTimeFormatted) + Environment.NewLine;
                        if (report.Video.Count > 0) GeneralInfoText.Text += "Frame Count: " + report.Video[0].FrameCount + Environment.NewLine;
                        if (!string.IsNullOrWhiteSpace(General.OveralBitRateString)) GeneralInfoText.Text += "Overral Bitrate: " + General.OveralBitRateString + Environment.NewLine;
                        if (report.Video.Count > 0) if (!string.IsNullOrEmpty(report.Video[0].MultiViewLayout)) GeneralInfoText.Text += "3D Mode: " + report.Video[0].MultiViewLayout + Environment.NewLine;
                        if (!string.IsNullOrWhiteSpace(General.Title)) GeneralInfoText.Text += "Title: " + General.Title + Environment.NewLine;
                        if ((!string.IsNullOrWhiteSpace(General.Album)) && (General.Title != General.Album)) GeneralInfoText.Text += "Album: " + General.Album + Environment.NewLine;
                        if (!string.IsNullOrWhiteSpace(General.Artist)) GeneralInfoText.Text += "Artist: " + General.Artist + Environment.NewLine;
                        if (!string.IsNullOrWhiteSpace(General.Genre)) GeneralInfoText.Text += "Genre: " + General.Genre + Environment.NewLine;
                        GeneralInfoText.Text += "Chapter: " + ((General.ChaptersCount > 0) ? "Yes (" + General.ChaptersCount + ")" : "No") + Environment.NewLine;
                        if (!string.IsNullOrWhiteSpace(General.EncodedDate)) GeneralInfoText.Text += "Date Encoded: " + General.EncodedDate + Environment.NewLine;
                        if (!string.IsNullOrWhiteSpace(General.EncodedLibrary)) GeneralInfoText.Text += "Created with: " + General.EncodedLibrary + Environment.NewLine;

                        if (General.Cover)
                        {
                            cover.Source = GetCover;
                            CoverGrid.Width = new GridLength(pixels: 150);
                        }
                        else
                        {
                            cover.Source = null;
                            CoverGrid.Width = new GridLength(pixels: 0);
                        }
                    }
                    #endregion

                    // video track
                    #region VideoTrack
                    VideoInfoText.Text = string.Empty;

                    foreach (VideoTrack Video in report.Video)
                    {
                        VideoInfoText.Text += "> ";

                        if (string.IsNullOrWhiteSpace(Video.LanguageString))
                        {
                            VideoInfoText.Text += "Undetermined";
                        }
                        else
                        {
                            VideoInfoText.Text += Video.LanguageString;
                        }

                        if (!string.IsNullOrWhiteSpace(Video.Title)) VideoInfoText.Text += " - [" + Video.Title + "]";

                        if (!string.IsNullOrWhiteSpace(Video.DurationString))
                        {
                            VideoInfoText.Text += " - " + ManageMilliSec(Video.DurationString) + " -";
                        }
                        else
                        {
                            VideoInfoText.Text += " -";
                        }

                        VideoInfoText.Text += " " + Video.Width + "×" + Video.Height + ", ";

                        VideoInfoText.Text += Video.CodecString + " (" + Video.FormatProfile + "), ";

                        if (!string.IsNullOrWhiteSpace(Video.BitRateString)) VideoInfoText.Text += Video.BitRateString + ", ";

                        VideoInfoText.Text += Video.FrameRate.ToString().Replace(",", ".") + " fps, " + Video.AspectRatioString + ", ";

                        if (!string.IsNullOrWhiteSpace(Video.FormatSettings)) VideoInfoText.Text += "(" + Video.FormatSettings + ")" + Environment.NewLine;

                        if (Video.Delay > 0)
                        {
                            VideoInfoText.Text += " - Delay: " + Video.Delay + "ms";
                        }
                        if (Video.Default) VideoInfoText.Text += " - Default Track: Yes";

                        if (Video.Forced) VideoInfoText.Text += " - Forced Track: Yes";
                        VideoInfoText.Text = VideoInfoText.Text.TrimEnd(',', ' ', '-');
                    }
                    #endregion

                    // audio track
                    #region AudioTrack
                    AudioInfoText.Text = string.Empty;

                    foreach (AudioTrack Audio in report.Audio)
                    {
                        AudioInfoText.Text += "> ";

                        if (string.IsNullOrEmpty(Audio.LanguageString))
                        {
                            AudioInfoText.Text += "Undetermined";
                        }
                        else
                        {
                            AudioInfoText.Text += Audio.LanguageString;
                        }

                        if (!string.IsNullOrWhiteSpace(Audio.Title)) AudioInfoText.Text += " - [" + Audio.Title + "]";

                        if (!string.IsNullOrWhiteSpace(Audio.DurationStringFormatted))
                        {
                            AudioInfoText.Text += " - " + ManageMilliSec(Audio.DurationStringFormatted) + " - ";
                        }
                        else
                        {
                            AudioInfoText.Text += " - ";
                        }

                        int numberofsubtracks = 0;

                        if (!string.IsNullOrWhiteSpace(Audio.FormatProfile))
                        {
                            numberofsubtracks = Audio.FormatProfile.Split('/').Length;

                        }
                        else if (!string.IsNullOrWhiteSpace(Audio.Format))
                        {
                            numberofsubtracks = Audio.Format.Split('/').Length;

                        }

                        if (numberofsubtracks > 1)
                        {


                            foreach (int j in Enumerable.Range(0, numberofsubtracks))
                            {
                                // add tab if not are first subtrack
                                if (j > 0) AudioInfoText.Text += '\t';
                                // format

                                if (!string.IsNullOrWhiteSpace(ManagerSubTrackAudio(Audio.Format, j))) AudioInfoText.Text += ManagerSubTrackAudio(Audio.Format, j);

                                // format profile
                                if (!string.IsNullOrWhiteSpace(ManagerSubTrackAudio(Audio.FormatProfile, j)))
                                {
                                    if (string.IsNullOrWhiteSpace(AudioInfoText.Text.Split('\r').Last()))
                                    {
                                        AudioInfoText.Text += ManagerSubTrackAudio(Audio.FormatProfile, j);
                                    }
                                    else
                                    {
                                        AudioInfoText.Text += "-" + ManagerSubTrackAudio(Audio.FormatProfile, j);
                                    }
                                }
                                // bitrate

                                if (!string.IsNullOrWhiteSpace(ManagerSubTrackAudio(Audio.BitRateString, j)))
                                {
                                    AudioInfoText.Text += ", " + ManagerSubTrackAudio(Audio.BitRateString, j);
                                }


                                // bitrate mode  
                                if (!string.IsNullOrEmpty(ManagerSubTrackAudio(Audio.BitRateMode, j))) AudioInfoText.Text += ", " + ManagerSubTrackAudio(Audio.BitRateMode, j);

                                // channels

                                if (!string.IsNullOrEmpty(ManagerSubTrackAudio(Audio.ChannelsString, j))) AudioInfoText.Text += ", " + ManagerSubTrackAudio(Audio.ChannelsString, j);


                                // bit depth

                                if (!string.IsNullOrEmpty(ManagerSubTrackAudio(Audio.BitDepthString, j))) AudioInfoText.Text += ", " + ManagerSubTrackAudio(Audio.BitDepthString, j);

                                // sampling rate

                                if (Audio.SamplingRateString.Contains("/"))
                                {
                                    if (!string.IsNullOrWhiteSpace(Audio.SamplingRateString.Split('/')[j])) AudioInfoText.Text += ", " + Audio.SamplingRateString.Split('/')[j].Trim(' ');

                                }
                                else
                                {
                                    if (!string.IsNullOrWhiteSpace(Audio.SamplingRateString)) AudioInfoText.Text += ", " + Audio.SamplingRateString;
                                }

                                AudioInfoText.Text = AudioInfoText.Text.Trim(',').Trim(' ');

                                // delay if present for the radix track
                                if (j == 0)
                                {
                                    if (Audio.Delay > 0)
                                    {
                                        AudioInfoText.Text += " - Delay: " + Audio.Delay + "ms";
                                    }
                                    if (Audio.Default) AudioInfoText.Text += " - Default Track";
                                    if (Audio.Forced) AudioInfoText.Text += " - Forced Track";
                                }

                                AudioInfoText.Text += Environment.NewLine;

                            }
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(Audio.Format)) AudioInfoText.Text += Audio.Format + " - ";
                            if (!string.IsNullOrWhiteSpace(Audio.FormatProfile)) AudioInfoText.Text += Audio.FormatProfile + ", ";
                            if (!string.IsNullOrWhiteSpace(Audio.BitRateString)) AudioInfoText.Text += Audio.BitRateString + ", ";
                            if (!string.IsNullOrWhiteSpace(Audio.BitRateMode)) AudioInfoText.Text += Audio.BitRateMode + ", ";
                            if (!string.IsNullOrWhiteSpace(Audio.ChannelsString)) AudioInfoText.Text += Audio.ChannelsString + ", ";
                            if (!string.IsNullOrWhiteSpace(Audio.BitDepthString)) AudioInfoText.Text += Audio.BitDepthString + ", ";
                            if (!string.IsNullOrWhiteSpace(Audio.SamplingRateString)) AudioInfoText.Text += Audio.SamplingRateString;

                            AudioInfoText.Text = AudioInfoText.Text.Trim(',').Trim(' ');

                            //delay if present
                            if (Audio.Delay > 0)
                            {
                                AudioInfoText.Text += " - Delay: " + Audio.Delay + "ms";
                            }
                            if (Audio.Default) AudioInfoText.Text += " - Default Track";
                            if (Audio.Forced) AudioInfoText.Text += " - Forced Track";

                            AudioInfoText.Text += Environment.NewLine;
                        }
                    }
                    #endregion

                    // text track
                    #region TextTrack
                    TextInfoText.Text = string.Empty;

                    foreach (TextTrack Text in report.Text)
                    {
                        TextInfoText.Text += "> ";
                        if (string.IsNullOrWhiteSpace(Text.LanguageString))
                        {
                            TextInfoText.Text += "Undetermined";
                        }
                        else
                        {
                            TextInfoText.Text += Text.LanguageString;
                        }
                        if (!string.IsNullOrWhiteSpace(Text.Title)) TextInfoText.Text += " - [" + Text.Title + "]";
                        TextInfoText.Text += " - " + Text.Codec;
                        if (!string.IsNullOrWhiteSpace(Text.DefaultString)) TextInfoText.Text += ", Default: " + Text.DefaultString;
                        if (!string.IsNullOrWhiteSpace(Text.ForcedString)) TextInfoText.Text += ", Forced: " + Text.ForcedString;

                        AudioInfoText.Text = AudioInfoText.Text.Trim(' ');

                        //delay if present
                        if (Text.Delay > 0)
                        {
                            TextInfoText.Text += " - Delay: " + Text.Delay + "ms" + Environment.NewLine;
                        }
                        else
                        {
                            TextInfoText.Text += Environment.NewLine;
                        }
                    }
                    #endregion

                    if (report.Video.Count == 0) TVideo.Visibility = Visibility.Collapsed;

                    if (report.Audio.Count == 0) TAudio.Visibility = Visibility.Collapsed;

                    if (report.Text.Count == 0) TText.Visibility = Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// Create the Tree report from a Mediainfo report.
        /// </summary>
        private void CreateTreeReport()
        {
            TreeReport.RemoveItems();
            if (report != null)
            {

                //TREE
                if (!DisableTree.IsChecked.Value)
                {
                    if (!DisableTree.IsChecked.Value)
                    {
                        TreeViewItem temp = null;
                        // general track
                        #region GeneralTrack
                        foreach (GeneralTrack General in report.General)
                        {
                            temp = new TreeViewItem();

                            temp.Header = "General";
                            temp.Items.Add(new TreeViewItem() { Header = "File Name: " + General.FileName + "." + General.FileExtension });
                            temp.Items.Add(new TreeViewItem() { Header = "Format: " + General.Format });
                            temp.Items.Add(new TreeViewItem() { Header = "File Size: " + General.FileSizePrecise });

                            if (!string.IsNullOrWhiteSpace(ManageMilliSec(General.PlayTimeFormatted))) temp.Items.Add(new TreeViewItem() { Header = "Duration: " + ManageMilliSec(General.PlayTimeFormatted) });
                            if (!string.IsNullOrWhiteSpace(General.OveralBitRateString)) temp.Items.Add(new TreeViewItem() { Header = "Overal BitRate: " + General.OveralBitRateString });
                            if (report.Video.Count > 0) if (report.Video[0].FrameCount > 0) temp.Items.Add(new TreeViewItem() { Header = "Frame Count: " + report.Video[0].FrameCount });
                            if (!string.IsNullOrWhiteSpace(General.Title)) temp.Items.Add(new TreeViewItem() { Header = "Title: " + General.Title });
                            if ((!string.IsNullOrWhiteSpace(General.Album)) && (General.Title != General.Album)) temp.Items.Add(new TreeViewItem() { Header = "Album: " + General.Album });
                            if (!string.IsNullOrWhiteSpace(General.Artist)) temp.Items.Add(new TreeViewItem() { Header = "Artist: " + General.Artist });
                            if (!string.IsNullOrWhiteSpace(General.Genre)) temp.Items.Add(new TreeViewItem() { Header = "Genre: " + General.Genre });
                            temp.Items.Add(new TreeViewItem() { Header = "Chapter: " + ((General.ChaptersCount > 0) ? "Yes (" + General.ChaptersCount + ")" : "No") });
                            if (report.Video.Count > 0) if (!string.IsNullOrEmpty(report.Video[0].MultiViewLayout)) temp.Items.Add(new TreeViewItem() { Header = "3D Mode: " + report.Video[0].MultiViewLayout });
                            if (!string.IsNullOrWhiteSpace(General.EncodedDate)) temp.Items.Add(new TreeViewItem() { Header = "Date Encoded: " + General.EncodedDate });
                            if (!string.IsNullOrWhiteSpace(General.EncodedLibrary)) temp.Items.Add(new TreeViewItem() { Header = "Created with: " + General.EncodedLibrary });

                            TreeReport.Items.Add(temp);
                        }
                        #endregion

                        // video track
                        #region VideoTrack
                        int i = 0;
                        foreach (VideoTrack Video in report.Video)
                        {
                            temp = new TreeViewItem();
                            temp.Header = "Video " + ++i;
                            if (string.IsNullOrWhiteSpace(Video.LanguageString)) temp.Items.Add(new TreeViewItem() { Header = "Undetermined Language" });
                            else temp.Items.Add(new TreeViewItem() { Header = "Language: " + Video.LanguageString });
                            if (!string.IsNullOrWhiteSpace(Video.Title)) temp.Items.Add(new TreeViewItem() { Header = "Track Title: " + Video.Title });
                            temp.Items.Add(new TreeViewItem() { Header = "Resolution: " + Video.Width + "×" + Video.Height });
                            temp.Items.Add(new TreeViewItem() { Header = "Aspect Ratio: " + Video.AspectRatioString });
                            temp.Items.Add(new TreeViewItem() { Header = "Framerate: " + Video.FrameRate.ToString().Replace(",", ".") + " fps" });
                            temp.Items.Add(new TreeViewItem() { Header = "Codec Video: " + (Video.CodecString + " (" + Video.FormatProfile + ", " + Video.FormatSettings).TrimEnd(',', ' ') + ")" });
                            if (Video.Delay > 0)
                            {
                                temp.Items.Add(new TreeViewItem() { Header = "Delay: " + Video.Delay + "ms" });
                            }
                            if (!string.IsNullOrWhiteSpace(Video.DefaultString))
                            {
                                temp.Items.Add(new TreeViewItem() { Header = "Default: " + Video.DefaultString });
                            }
                            if (!string.IsNullOrWhiteSpace(Video.ForcedString))
                            {
                                temp.Items.Add(new TreeViewItem() { Header = "Forced: " + Video.ForcedString });
                            }
                            TreeReport.Items.Add(temp);
                        }
                        #endregion

                        // audio tracks
                        #region AudioTrack
                        i = 0;
                        foreach (AudioTrack Audio in report.Audio)
                        {
                            temp = new TreeViewItem();
                            temp.Header = "Audio " + ++i;
                            if (string.IsNullOrWhiteSpace(Audio.LanguageString)) temp.Items.Add(new TreeViewItem() { Header = "Undetermined Language" });
                            else temp.Items.Add(new TreeViewItem() { Header = "Language: " + Audio.LanguageString });
                            if (!string.IsNullOrWhiteSpace(Audio.Title)) temp.Items.Add(new TreeViewItem() { Header = "Track Title: " + Audio.Title });
                            if (!string.IsNullOrWhiteSpace(Audio.Format)) temp.Items.Add(new TreeViewItem() { Header = "Format: " + Audio.Format.Trim(' ') });

                            int numberofsubtracks = 0;
                            if (!string.IsNullOrWhiteSpace(Audio.FormatProfile))
                            {
                                numberofsubtracks = Audio.FormatProfile.Split('/').Length;

                            }
                            else if (!string.IsNullOrWhiteSpace(Audio.Format))
                            {
                                numberofsubtracks = Audio.Format.Split('/').Length;

                            }
                            //composed tracks with subtracks
                            if (numberofsubtracks > 1)
                            {
                                TreeViewItem sub = null;

                                foreach (int j in Enumerable.Range(0, numberofsubtracks))
                                {
                                    sub = null;
                                    sub = new TreeViewItem();
                                    if ((!string.IsNullOrWhiteSpace(ManagerSubTrackAudio(Audio.Format, j))) && (!string.IsNullOrWhiteSpace(ManagerSubTrackAudio(Audio.FormatProfile, j))))
                                        sub.Header = ManagerSubTrackAudio(Audio.Format, j) + "-" + ManagerSubTrackAudio(Audio.FormatProfile, j);
                                    else if (!string.IsNullOrWhiteSpace(ManagerSubTrackAudio(Audio.Format, j)))
                                        sub.Header = ManagerSubTrackAudio(Audio.Format, j);
                                    else if (!string.IsNullOrWhiteSpace(ManagerSubTrackAudio(Audio.FormatProfile, j)))
                                        sub.Header = ManagerSubTrackAudio(Audio.FormatProfile, j);
                                    else sub.Header = $"Subtrack {j + 1}";

                                    if (!string.IsNullOrWhiteSpace(ManagerSubTrackAudio(Audio.BitRateString, j)))
                                        sub.Items.Add(new TreeViewItem() { Header = "BitRate: " + ManagerSubTrackAudio(Audio.BitRateString, j) });

                                    if (!string.IsNullOrWhiteSpace(ManagerSubTrackAudio(Audio.BitRateMode, j)))
                                        sub.Items.Add(new TreeViewItem() { Header = "BitRate Mode: " + ManagerSubTrackAudio(Audio.BitRateMode, j) });

                                    if (!string.IsNullOrWhiteSpace(ManagerSubTrackAudio(Audio.ChannelsString, j)))
                                        sub.Items.Add(new TreeViewItem() { Header = "Channel: " + ManagerSubTrackAudio(Audio.ChannelsString, j) });

                                    if (!string.IsNullOrWhiteSpace(ManagerSubTrackAudio(Audio.BitDepthString, j)))
                                        sub.Items.Add(new TreeViewItem() { Header = "Bit Depth: " + ManagerSubTrackAudio(Audio.BitDepthString, j) });

                                    if (!string.IsNullOrWhiteSpace(ManagerSubTrackAudio(Audio.SamplingRateString, j)))
                                        sub.Items.Add(new TreeViewItem() { Header = "Sampling Rate: " + ManagerSubTrackAudio(Audio.SamplingRateString, j) });
                                    temp.Items.Add(sub);
                                }


                            }
                            else //single tracks without subtracks
                            {
                                if (!string.IsNullOrEmpty(Audio.FormatProfile)) temp.Items.Add(new TreeViewItem() { Header = "Profile: " + Audio.FormatProfile });
                                if (!string.IsNullOrEmpty(Audio.BitRateString)) temp.Items.Add(new TreeViewItem() { Header = "BitRate: " + Audio.BitRateString });
                                if (!string.IsNullOrEmpty(Audio.BitRateMode)) temp.Items.Add(new TreeViewItem() { Header = "BitRate Mode: " + Audio.BitRateMode });
                                if (!string.IsNullOrEmpty(Audio.ChannelsString)) temp.Items.Add(new TreeViewItem() { Header = "Channels: " + Audio.ChannelsString });
                                if (!string.IsNullOrEmpty(Audio.SamplingRateString)) temp.Items.Add(new TreeViewItem() { Header = "Sampling Rate: " + Audio.SamplingRateString });


                            }
                            if (Audio.Delay > 0)
                            {
                                temp.Items.Add(new TreeViewItem() { Header = "Delay: " + Audio.Delay + "ms" });
                            }
                            if (!string.IsNullOrEmpty(Audio.DefaultString))
                            {
                                temp.Items.Add(new TreeViewItem() { Header = "Default: " + Audio.DefaultString });
                            }
                            if (!string.IsNullOrEmpty(Audio.ForcedString))
                            {
                                temp.Items.Add(new TreeViewItem() { Header = "Forced: " + Audio.ForcedString });
                            }

                            TreeReport.Items.Add(temp);
                        }
                        #endregion

                        // text track
                        #region TextTrack 
                        i = 0;
                        foreach (TextTrack Text in report.Text)
                        {
                            temp = new TreeViewItem();
                            temp.Header = "Text " + ++i;
                            if (string.IsNullOrEmpty(Text.LanguageString)) temp.Items.Add(new TreeViewItem() { Header = "Undetermined Language" });
                            else temp.Items.Add(new TreeViewItem() { Header = "Language: " + Text.LanguageString });
                            if (!string.IsNullOrEmpty(Text.Title)) temp.Items.Add(new TreeViewItem() { Header = "Track Title: " + Text.Title });
                            temp.Items.Add(new TreeViewItem() { Header = "Codec: " + Text.Codec });

                            TreeReport.Items.Add(temp);
                        }
                        #endregion
                    }
                }
            }
        }

        /// <summary>
        /// Generate infos of current Sub Track Audio from a string.
        /// </summary>
        /// <param name="str">String containing the output of a sound properties of mediainfo report.</param>
        /// <param name="index">Index of current Sub Track.</param>
        /// <returns></returns>
        private string ManagerSubTrackAudio(string str, int index)
        {
            if (str.Contains(" / "))
            {
                if (!string.IsNullOrWhiteSpace(str.Split(new string[] { " / " }, StringSplitOptions.None)[index])) return str.Split(new string[] { " / " }, StringSplitOptions.None)[index].Trim(' ');
            }
            else
            {
                if (!string.IsNullOrEmpty(str)) return str;
            }
            return string.Empty;
        }

        /// <summary>
        /// Saves user configuration to a XML config files.
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
                if ((bool)EnableMilliSecond.IsChecked)
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

                XmlWriter.WriteStartElement("config");
                XmlWriter.WriteAttributeString("tag", "Bold");
                if ((bool)Bold)
                {
                    XmlWriter.WriteValue("true");
                }
                else
                {
                    XmlWriter.WriteValue("false");
                }
                XmlWriter.WriteEndElement();

                XmlWriter.WriteStartElement("config");
                XmlWriter.WriteAttributeString("tag", "Italic");
                if ((bool)Italic)
                {
                    XmlWriter.WriteValue("true");
                }
                else
                {
                    XmlWriter.WriteValue("false");
                }
                XmlWriter.WriteEndElement();

                XmlWriter.WriteStartElement("config");
                XmlWriter.WriteAttributeString("font", "Size");

                XmlWriter.WriteValue((int)TextBoxComplete.FontSize);

                XmlWriter.WriteEndElement();

                XmlWriter.WriteStartElement("config");
                XmlWriter.WriteAttributeString("font", "Name");

                XmlWriter.WriteValue(TextBoxComplete.FontFamily.ToString());

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
        /// Restores user configuration from a XML config files.
        /// </summary>
        private void RestoreConfig()
        {
            if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\userconfig.xml"))
            {
                try
                {
                    XDocument xml = XDocument.Load(System.Windows.Forms.Application.StartupPath + "\\userconfig.xml");
                    List<XElement> settings = xml.Element("database").Elements("config").ToList();
                    IEnumerable<XElement> configs = from setting in settings
                                                    let attrs = setting.Attributes()
                                                    from attr in attrs
                                                    where attr.Name == "tag"
                                                    select setting;
                    IEnumerable<XElement> fonts = from setting in settings
                                                  let attrs = setting.Attributes()
                                                  from attr in attrs
                                                  where attr.Name == "font"
                                                  select setting;
                    foreach (XElement config in configs)
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
                                EnableMilliSecond.IsChecked = value;
                                break;
                            case "RemoveSpace":
                                RemoveSpace.IsChecked = value;
                                break;
                            case "Italic":
                                Italic = value;
                                ItalicChange();
                                break;
                            case "Bold":
                                Bold = value;
                                BoldChange();
                                break;
                        }
                    }
                    foreach (XElement font in fonts)
                    {
                        string configuration = font.Attribute("font").Value;
                        switch (configuration)
                        {
                            case "Size":
                                int size;
                                if (!int.TryParse(font.Value, out size))
                                {
                                    System.Windows.Forms.MessageBox.Show("Unable to restore user configuration. XML file is corrupted! (Unable to parse int from xml).");
                                }
                                else
                                {
                                    ComboBoxSize.SelectedValue = TextBoxComplete.FontSize = TextBoxStandard.FontSize = (double)size;
                                    //ComboBoxSize.SetValue
                                }
                                break;
                            case "Name":
                                try
                                {
                                    ComboBoxFont.SelectedValue = TextBoxComplete.FontFamily = TextBoxStandard.FontFamily = new FontFamily(font.Value);

                                }
                                catch
                                {
                                    System.Windows.Forms.MessageBox.Show("Unable to restore user configuration. XML file is corrupted! (Unable to parse font from xml).");
                                }
                                break;
                        }

                    }

                    ComboBoxFont_SelectionChanged();
                    ComboBoxSize_SelectionChanged();
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Unable to restore user configuration. XML file is corrupted! (" + ex.Message + ").");
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Unable to restore user configuration. XML file is missing!");
            }
        }

        /// <summary>
        /// Bold/un-Bold apply in the standard/complete report.
        /// </summary>
        private void BoldChange()
        {
            if (Bold)
            {
                TextBoxComplete.FontWeight = TextBoxStandard.FontWeight = FontWeights.Bold;
                BoldControl.CheckedButton();
            }
            else
            {
                TextBoxComplete.FontWeight = TextBoxStandard.FontWeight = FontWeights.Normal;
                BoldControl.UncheckedButton();
            }
        }

        /// <summary>
        /// Italic/un-Italic apply in the standard/complete report.
        /// </summary>
        private void ItalicChange()
        {
            if (Italic)
            {
                TextBoxComplete.FontStyle = TextBoxStandard.FontStyle = FontStyles.Italic;
                ItalicControl.CheckedButton();
            }
            else
            {
                TextBoxComplete.FontStyle = TextBoxStandard.FontStyle = FontStyles.Normal;
                ItalicControl.UncheckedButton();
            }
        }

        /// <summary>
        /// Reload enable/disable Save Report buttons.
        /// </summary>
        private void ReloadEnabledSaveButton()
        {
            if ((!AutoSaveReport.IsChecked.GetValueOrDefault(false)) && (report != null))
            {
                SaveReport_Complete.IsEnabled = SaveReport_Standard.IsEnabled = SaveReport.IsEnabled = true;
            }
            else
            {
                SaveReport_Complete.IsEnabled = SaveReport_Standard.IsEnabled = SaveReport.IsEnabled = false;
            }
        }

        /// <summary>
        /// Reload enable/disable Copy All buttons.
        /// </summary>
        private void ReloadEnabledCopyAllButton()
        {
            if (report != null)
            {
                CopyAll_Complete.IsEnabled = CopyAll_Standard.IsEnabled = true;
            }
            else
            {
                CopyAll_Complete.IsEnabled = CopyAll_Standard.IsEnabled = false;
            }
        }

        /// <summary>
        /// Verifies the mediainfo version. If it does not find the dll launches the update, and if the update fails, the application closes.
        /// </summary>
        private void VerifyMediaInfoVersion()
        {
            if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\mediainfo.dll"))
            {
                FileVersionInfo tmp = FileVersionInfo.GetVersionInfo(System.Windows.Forms.Application.StartupPath + "\\MediaInfo.dll");
                StatusMediainfo.Text = "v" + tmp.FileVersion;
                StatusMediainfo.Foreground = Brushes.Green;
            }
            else
            {
                GeneralGrid.IsEnabled = false;
                if (UpdateDLL() == "Mediainfo.dll was successfully updated.")
                {
                    GeneralGrid.IsEnabled = true;
                }
                else
                {
                    StatusGetMediaInfos.Text = "ERROR";
                    StatusGetMediaInfos.Foreground = Brushes.Red;
                    System.Windows.Forms.MessageBox.Show("Library Mediainfo.dll not found.", "ERROR", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    System.Windows.Forms.Application.Exit();
                    System.Windows.Application.Current.Shutdown();
                    Environment.Exit(0);
                }

            }
        }

        /// <summary>
        /// Verifies the getmediainfos version. If it does not find the DLL closes the application.
        /// </summary>
        private void VerifyGetMediaInfosVersion()
        {
            if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\GetMediaInfos.dll"))
            {
                FileVersionInfo tmp = FileVersionInfo.GetVersionInfo(System.Windows.Forms.Application.StartupPath + "\\GetMediaInfos.dll");
                StatusGetMediaInfos.Text = "v" + tmp.FileVersion;
                StatusGetMediaInfos.Foreground = Brushes.Green;
            }
            else
            {
                StatusGetMediaInfos.Text = "ERROR";
                StatusGetMediaInfos.Foreground = Brushes.Red;
                System.Windows.Forms.MessageBox.Show("Library GetMediaInfos.dll not found.", "ERROR", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                System.Windows.Forms.Application.Exit();
                System.Windows.Application.Current.Shutdown();
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Update Media Info DLL. Show the UpdateDLL windows.
        /// </summary>
        /// <returns>Status of update</returns>
        private string UpdateDLL()
        {
            ClearAll();
            UpdateDLL updateDLL = new UpdateDLL();
            updateDLL.ShowDialog();
            UpdaterProgress.Content = updateDLL.status;
            VerifyMediaInfoVersion();
            return updateDLL.status;
        }

        #endregion

    }
    /// <summary>
    /// Static class whit a Extension method for a better code.
    /// </summary>
    static public class InterfaceUtils
    {
        #region Extension method

        /// <summary>
        /// Extension method: It removes all items in a TreeView.
        /// </summary>
        /// <param name="treeView">TreeView to clean up.</param>
        static public void RemoveItems(this TreeView treeView)
        {
            while (treeView.HasItems)
            {
                treeView.Items.RemoveAt(0);
            }
        }
        /// <summary>
        /// Extension method: Change the button style so that partner wild with pleasure checked.
        /// </summary>
        /// <param name="treeView">Button to make checked.</param>
        static public void CheckedButton(this Button button)
        {
            button.Background = new SolidColorBrush(Color.FromArgb(255, 221, 221, 221));
            button.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 112, 112, 112));
        }
        /// <summary>
        /// Extension method: Change the button style so that partner wild with pleasure unchecked.
        /// </summary>
        /// <param name="button">Button to make unchecked.</param>
        static public void UncheckedButton(this Button button)
        {
            button.Background = Brushes.Transparent;
            button.BorderBrush = Brushes.Transparent;
        }

        #endregion
    }
}
