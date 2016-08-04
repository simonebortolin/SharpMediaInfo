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
    /// Logica di interazione per MainWindow.xaml
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
        /// Boolean that define if the report should be automatically generated or not
        /// </summary>
        bool SaveReportActive;

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
        /// Handler for application loading event.Contains functions to inizializate program, check for dependencies and restore user config.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Inizialize_Loaded(object sender, EventArgs e)
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
        /// Handler for application closing event. Saves user configuration.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void SaveAndClose_Event(object sender, EventArgs e)
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
        /// Handles the Click event of the RemoveContextMenu control. It removes context menu GetMediaInfos entry for specific extensions.
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
        /// Handles the Click event of the OpenFile control. Is used to open a new OpenFileDialog to let user to select the input file.
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
                filepathBox.Text = file.FileName;
            }
        }

        /// <summary>
        /// Handles the Click event of the SaveReport control. Save the report of the file in the same directory.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void SaveReport_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog Report = new System.Windows.Forms.SaveFileDialog();
            Report.Filter = "Simple Text|*.txt";
            Report.Title = "Select the file you want to use to save the report";
            Report.DefaultExt = ".txt";
            Report.FileName = Path.GetFileNameWithoutExtension(filepathBox.Text) + ".txt";
            Report.InitialDirectory = Path.GetDirectoryName(filepathBox.Text);
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
        /// Analyze declared path in filepathBox.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void AnalyzeDeclaredPath_Event(object sender, EventArgs e)
        {
            AnalyzeFile(filepathBox.Text);
        }

        /// <summary>
        /// Analyze dropped file.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DragEventArgs"/> instance containing the event data.</param>
        public void OpenDroppedFile_Event(object sender, DragEventArgs e)
        {
            //ClearAll();
            System.Windows.DataObject data = (System.Windows.DataObject)e.Data;
            if (data.ContainsFileDropList())
            {
                string[] FileList = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop, false);
                filepathBox.Text = FileList[0];
            }
        }

        /// <summary>
        /// Handle for Drag Enter event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DragEventArgs"/> instance containing the event data.</param>
        public void DragEnter_EventHandler(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
        }

        /// <summary>
        /// Handles the Checked event of the DisableSimpleInfo control. Disable Simple Report generation.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void DisableSimpleInfo_Checked(object sender, RoutedEventArgs e)
        {
            TabItem tab = Pages.Items[0] as TabItem;
            tab.Visibility = Visibility.Hidden;
            tab.Width = 0;
            TextInfoText.Text = AudioInfoText.Text = VideoInfoText.Text = GeneralInfoTextWithoutCover.Text = string.Empty;
        }

        /// <summary>
        /// Handles the Unchecked event of the DisableSimpleInfo control. Enable Simple Report generation.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void DisableSimpleInfo_Unchecked(object sender, RoutedEventArgs e)
        {
            TabItem tab = Pages.Items[0] as TabItem;
            tab.Visibility = Visibility.Visible;
            tab.Width = 52;
            StatusLab.Content = "Creating Simple Report...";
            StatusLab.InvalidateVisual();
            CreateSimpleReport();
            StatusLab.Content = "Ready.";
            StatusLab.InvalidateVisual();
        }

        /// <summary>
        /// Handles the Checked event of the DisableSimpleInfo control. Disable Simple Report generation.
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
        /// Handles the Unchecked event of the DisableSimpleInfo control. Enable Simple Report generation.
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
        /// Handles the Change event of the MS control. Enable or disable Milliseconds in simple report.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void MilliSec_Reload(object sender, RoutedEventArgs e)
        {
            TextInfoText.Text = AudioInfoText.Text = VideoInfoText.Text = GeneralInfoTextWithoutCover.Text = string.Empty;

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
        /// Handles the Checked event of the AutoSaveReport control. Enable auto file report save.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void AutoSaveReport_Checked(object sender, RoutedEventArgs e)
        {
            SaveMenu_Complete.IsEnabled = SaveMenu_Standard.IsEnabled = SaveReport.IsEnabled = false;
        }

        /// <summary>
        /// Handles the Unchecked event of the AutoSaveReport control. Disable auto file report save.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void AutoSaveReport_Unchecked(object sender, RoutedEventArgs e)
        {
            SaveMenu_Complete.IsEnabled = SaveMenu_Standard.IsEnabled = SaveReport.IsEnabled = SaveReportActive;
        }

        /// <summary>
        /// Show the changelog
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private void Version_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.MessageBox.Show(
                "- Added chapter info in Simple and Tree Report" + Environment.NewLine +
                "- If there are no traces removes the box in Simple report", "New in " + System.Windows.Forms.Application.ProductVersion);
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
        /// Check for Updates
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">instance containing the event data.</param>
        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateDLL();
        }

        /// <summary>
        /// Select all and copy text from contextual menu in the standard/complete report.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private void CopyAllText_Click(object sender, RoutedEventArgs e)
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
        /// Bold setting Click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private void Bold_Click(object sender, RoutedEventArgs e)
        {
            Bold = !Bold;
            BoldChange();
        }

        /// <summary>
        /// Italic setting Click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private void Italic_Click(object sender, RoutedEventArgs e)
        {
            Italic = !Italic;
            ItalicChange();
        }

        /// <summary>
        /// Set the text size in the standard/complete report.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private void Size_Change(object sender, EventArgs e)
        {
            if (ComboBoxSize.SelectedValue != null)
                TextBoxComplete.FontSize = TextBoxStandard.FontSize = (double)ComboBoxSize.SelectedValue;
        }

        /// <summary>
        /// Set the font size in the standard/complete report
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private void Font_Change(object sender, EventArgs e)
        {
            if (ComboBoxFont.SelectedValue != null)
                TextBoxComplete.FontFamily = TextBoxStandard.FontFamily = (FontFamily)ComboBoxFont.SelectedValue;
        }

        /// <summary>
        /// Handles the Click event of the SaveCover control.
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxReport_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        #endregion

        #region HelperMethods

        /// <summary>
        /// Analyze arguments passed to the application. If the arguments count is greater than 1 analyze the first one.
        /// </summary>
        public void ProcessArgs()
        {
            string[] arguments = Environment.GetCommandLineArgs();
            if (arguments.Length > 1)
            {
                filepathBox.Text = arguments[1];
                AnalyzeFile(arguments[1]);
            }
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
                if (!AutoSaveReport.IsChecked.Value) SaveMenu_Complete.IsEnabled = SaveMenu_Standard.IsEnabled = SaveReport.IsEnabled = true;
                SaveMenu_Complete.IsEnabled = SaveMenu_Standard.IsEnabled = SaveReportActive = true;
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
                SaveMenu_Complete.IsEnabled = SaveMenu_Standard.IsEnabled = SaveReport.IsEnabled = SaveReportActive = false;
            }
        }

        /// <summary>
        /// Clear all variables and the interface.
        /// </summary>
        private void ClearAll()
        {
            StatusLab.Content = "Cleaning all...";
            StatusLab.InvalidateVisual();
            TextBoxComplete.Text = TextBoxStandard.Text = string.Empty;
            this.Title = "GetMediaInfo";
            cover.Source = null;
            report = null;
            SaveReport.IsEnabled = SaveReportActive = false;
            TVideo.Visibility = TAudio.Visibility = TText.Visibility = Visibility.Visible;
            TextInfoText.Text = AudioInfoText.Text = VideoInfoText.Text = GeneralInfoTextWithoutCover.Text = GeneralInfoTextWithCover.Text = string.Empty;
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
                    string ReportName = Path.ChangeExtension(filepathBox.Text, ".txt");
                    int i = 2;
                    while (File.Exists(ReportName))
                    {
                        ReportName = Path.ChangeExtension(filepathBox.Text, i + ".txt");
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
        /// <param name="time">string with timecode.</param>
        /// <returns></returns>
        private string ManageMilliSec(string time)
        {
            return MilliSec.IsChecked.Value ? time : time.Split('.')[0];
        }

        /// <summary>
        /// Return Bitmap Image if present in the input files report
        /// </summary>
        /// <returns>Image Cover.</returns>
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
        /// Create the Simple report.
        /// </summary>
        private void CreateSimpleReport()
        {
            if (report != null)
            {
                if (!DisableSimpleInfo.IsChecked.Value)
                {
                    // generals track
                    #region GeneralTrack
                    GeneralInfoTextWithCover.Text = GeneralInfoTextWithoutCover.Text = string.Empty;
                    
                    foreach (GeneralTrack General in report.General)
                    {
                        if (General.Cover)
                        {
                            GeneralInfoTextWithCover.Text = "File Name: " + General.FileName + "." + General.FileExtension + Environment.NewLine;
                            GeneralInfoTextWithCover.Text += "Format: " + General.Format + Environment.NewLine;
                            GeneralInfoTextWithCover.Text += "File Size: " + General.FileSizePrecise + Environment.NewLine;
                            if (!string.IsNullOrWhiteSpace(General.PlayTimeFormatted)) GeneralInfoTextWithCover.Text += "Duration: " + ManageMilliSec(General.PlayTimeFormatted) + Environment.NewLine;
                            if (report.Video.Count > 0) GeneralInfoTextWithCover.Text += "Frame Count: " + report.Video[0].FrameCount + Environment.NewLine;
                            if (!string.IsNullOrWhiteSpace(General.OveralBitRateString)) GeneralInfoTextWithCover.Text += "Overral Bitrate: " + General.OveralBitRateString + Environment.NewLine;
                            if (report.Video.Count > 0) if (!string.IsNullOrEmpty(report.Video[0].MultiViewLayout)) GeneralInfoTextWithCover.Text += "3D Mode: " + report.Video[0].MultiViewLayout + Environment.NewLine;
                            if (!string.IsNullOrWhiteSpace(General.Title)) GeneralInfoTextWithCover.Text += "Title: " + General.Title + Environment.NewLine;
                            if ((!string.IsNullOrWhiteSpace(General.Album)) && (General.Title != General.Album)) GeneralInfoTextWithCover.Text += "Album: " + General.Album + Environment.NewLine;
                            if (!string.IsNullOrWhiteSpace(General.Artist)) GeneralInfoTextWithCover.Text += "Artist: " + General.Artist + Environment.NewLine;
                            if (!string.IsNullOrWhiteSpace(General.Genre)) GeneralInfoTextWithCover.Text += "Genre: " + General.Genre + Environment.NewLine;
                            if (!string.IsNullOrWhiteSpace(General.Chapter)) GeneralInfoTextWithCover.Text += "Chapter: " + General.Genre + Environment.NewLine;
                            if (!string.IsNullOrWhiteSpace(General.EncodedDate)) GeneralInfoTextWithCover.Text += "Date Encoded: " + General.EncodedDate + Environment.NewLine;
                            if (!string.IsNullOrWhiteSpace(General.EncodedLibrary)) GeneralInfoTextWithCover.Text += "Created with: " + General.EncodedLibrary + Environment.NewLine;
                            cover.Source = GetCover;
                        }
                        else
                        {
                            GeneralInfoTextWithoutCover.Text = "File Name: " + General.FileName + "." + General.FileExtension + Environment.NewLine;
                            GeneralInfoTextWithoutCover.Text += "Format: " + General.Format + Environment.NewLine;
                            GeneralInfoTextWithoutCover.Text += "File Size: " + General.FileSizePrecise + Environment.NewLine;
                            if (!string.IsNullOrWhiteSpace(General.PlayTimeFormatted)) GeneralInfoTextWithoutCover.Text += "Duration: " + ManageMilliSec(General.PlayTimeFormatted) + Environment.NewLine;
                            if (report.Video.Count > 0) GeneralInfoTextWithoutCover.Text += "Frame Count: " + report.Video[0].FrameCount + Environment.NewLine;
                            if (!string.IsNullOrWhiteSpace(General.OveralBitRateString)) GeneralInfoTextWithoutCover.Text += "Overral Bitrate: " + General.OveralBitRateString + Environment.NewLine;
                            if (report.Video.Count > 0) if (!string.IsNullOrEmpty(report.Video[0].MultiViewLayout)) GeneralInfoTextWithoutCover.Text += "3D Mode: " + report.Video[0].MultiViewLayout + Environment.NewLine;
                            if (!string.IsNullOrWhiteSpace(General.Title)) GeneralInfoTextWithoutCover.Text += "Title: " + General.Title + Environment.NewLine;
                            if ((!string.IsNullOrWhiteSpace(General.Album)) && (General.Title != General.Album)) GeneralInfoTextWithoutCover.Text += "Album: " + General.Album + Environment.NewLine;
                            if (!string.IsNullOrWhiteSpace(General.Artist)) GeneralInfoTextWithoutCover.Text += "Artist: " + General.Artist + Environment.NewLine;
                            if (!string.IsNullOrWhiteSpace(General.Genre)) GeneralInfoTextWithoutCover.Text += "Genre: " + General.Genre + Environment.NewLine;
                            if (!string.IsNullOrWhiteSpace(General.Chapter)) GeneralInfoTextWithoutCover.Text += "Chapter: " + General.Genre + Environment.NewLine;
                            if (!string.IsNullOrWhiteSpace(General.EncodedDate)) GeneralInfoTextWithoutCover.Text += "Date Encoded: " + General.EncodedDate + Environment.NewLine;
                            if (!string.IsNullOrWhiteSpace(General.EncodedLibrary)) GeneralInfoTextWithoutCover.Text += "Created with: " + General.EncodedLibrary + Environment.NewLine;
                        }
                    }
                    #endregion
                    
                    // video track
                    #region VideoTrack
                    VideoInfoText.Text = string.Empty;

                    foreach(VideoTrack Video in report.Video)
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
					
                    foreach(AudioTrack Audio in report.Audio)
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

                        if(!string.IsNullOrWhiteSpace(Audio.FormatProfile))
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

                    foreach(TextTrack Text in report.Text)
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
        /// Create the Tree report.
        /// </summary>
        private void CreateTreeReport()
        {

            while (TreeReport.HasItems)
            {
                TreeReport.Items.RemoveAt(0);
            }
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
                        foreach(GeneralTrack General in report.General)
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
                            if (!string.IsNullOrWhiteSpace(General.Chapter)) temp.Items.Add(new TreeViewItem() { Header = "Chapter: " + General.Chapter }); 
                            if (report.Video.Count > 0) if (!string.IsNullOrEmpty(report.Video[0].MultiViewLayout)) temp.Items.Add(new TreeViewItem() { Header = "3D Mode: " + report.Video[0].MultiViewLayout });
                            if (!string.IsNullOrWhiteSpace(General.EncodedDate)) temp.Items.Add(new TreeViewItem() { Header = "Date Encoded: " + General.EncodedDate });
                            if (!string.IsNullOrWhiteSpace(General.EncodedLibrary)) temp.Items.Add(new TreeViewItem() { Header = "Created with: " + General.EncodedLibrary });

                            TreeReport.Items.Add(temp);
                        }
                        #endregion

                        // video track
                        #region VideoTrack
                        int i = 0;
                        foreach(VideoTrack Video in report.Video)
                        {
                            temp = new TreeViewItem();
                            temp.Header = "Video " + ++i;
                            if (string.IsNullOrWhiteSpace(Video.LanguageString)) temp.Items.Add(new TreeViewItem() { Header = "Undetermined Language" });
                            else temp.Items.Add(new TreeViewItem() { Header = "Language: " + Video.LanguageString });
                            if (!string.IsNullOrWhiteSpace(Video.Title)) temp.Items.Add(new TreeViewItem() { Header = "Track Title: " + Video.Title });
                            temp.Items.Add(new TreeViewItem() { Header = "Resolution: " + Video.Width + "×" + Video.Height });
                            temp.Items.Add(new TreeViewItem() { Header = "Aspect Ratio: " + Video.AspectRatioString });
                            temp.Items.Add(new TreeViewItem() { Header = "Framerate: " + Video.FrameRate.ToString().Replace(",", ".") + " fps" });
                            temp.Items.Add(new TreeViewItem() { Header = "Codec Video: " + (Video.CodecString + " (" + Video.FormatProfile + ", " + Video.FormatSettings).TrimEnd(',',' ') + ")" });
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
                        foreach(AudioTrack Audio in report.Audio)
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

                                foreach(int j in Enumerable.Range(0, numberofsubtracks))
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
                        foreach(TextTrack Text in report.Text)
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
        /// Generate infos of current Sub Track Audio from a string
        /// </summary>
        /// <param name="str">String of input</param>
        /// <param name="j">Index of current Sub Track</param>
        /// <returns></returns>
        private string ManagerSubTrackAudio(string str, int j)
        {
            if (str.Contains(" / "))
            {
                if (!string.IsNullOrWhiteSpace(str.Split(new string[] { " / " }, StringSplitOptions.None)[j])) return str.Split(new string[] { " / " }, StringSplitOptions.None)[j].Trim(' ');
            }
            else
            {
                if (!string.IsNullOrEmpty(str)) return str;
            }
            return string.Empty;
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
        /// Restores user configuration.
        /// </summary>
        public void RestoreConfig()
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
                                MilliSec.IsChecked = value;
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
        /// Bold/un-Bold apply in the standard/complete report.
        /// </summary>
        private void BoldChange()
        {
            if (Bold)
            {
                TextBoxComplete.FontWeight = TextBoxStandard.FontWeight = FontWeights.Bold;
                B.Background = new SolidColorBrush(Color.FromArgb((byte)255, (byte)221, (byte)221, (byte)221));
                B.BorderBrush = new SolidColorBrush(Color.FromArgb((byte)255, (byte)112, (byte)112, (byte)112));
            }
            else
            {
                TextBoxComplete.FontWeight = TextBoxStandard.FontWeight = FontWeights.Normal;
                B.Background = Brushes.Transparent;
                B.BorderBrush = Brushes.Transparent;
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
                I.Background = new SolidColorBrush(Color.FromArgb((byte)255, (byte)221, (byte)221, (byte)221));
                I.BorderBrush = new SolidColorBrush(Color.FromArgb((byte)255, (byte)112, (byte)112, (byte)112));
            }
            else
            {
                TextBoxComplete.FontStyle = TextBoxStandard.FontStyle = FontStyles.Normal;
                I.Background = Brushes.Transparent;
                I.BorderBrush = Brushes.Transparent;
            }
        }

        /// <summary>
        /// Verifies the mediainfo version.
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
        /// Verifies the getmediainfos version.
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
        /// Update Media Info DLL.
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
}
