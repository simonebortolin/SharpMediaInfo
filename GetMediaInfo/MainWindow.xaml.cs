using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using GetMediaInfos;

namespace GetMediaInfoGUI
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //bool removed1 = false, removed2 = false, removed3 = false;
        bool SaveReportActive;
        MediaInfo report;
        /// //////////////////////////////////////////////////////////////////////////////////////////////// ///

        public MainWindow()
        {
            InitializeComponent();
            Reset();
        }

        private void PrimoAvvio(object sender, EventArgs e)
        {
            //this.DoubleBuffered = true;
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
        }

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

        private void GetMediaInfo_Shown(object sender, EventArgs e)
        {
            /*this.Update();
            this.Refresh();
            ApriDaArgomentoExe();*/
        }

        /// //////////////////////////////////////////////////////////////////////////////////////////////// ///
        ///                                                                                                  ///
        ///                                          FILE OPEN                                               ///
        ///                                                                                                  ///
        /// //////////////////////////////////////////////////////////////////////////////////////////////// ///

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            Reset();
            OpenFileDialog file = new OpenFileDialog();
            file.Filter = "Mediafile|*.*";
            //file.Filter = "Matorska|*.mkv;*.mka;*.mkv;*.mk3d|MPEG-4|*.mp4;*.m4a;*.m4v;*m4b;|AVI|*.avi|FLAC|*.flac|MP3|*.mp3";
            //file.Filter += "|OGM|*.ogg;*.ogm;*.oga;*.ogv;*.ogx|WEB-M|*.weba;*.webv|Windows Media|*.wmv;*.wma|";
            file.Title = "Select a Media File";
            file.Multiselect = false;

            if (file.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                filepathBox.Text = file.FileName;
                //Analisi(filepathBox.Text);
            }
<<<<<<< HEAD:GetMediaInfo/Main.cs
=======
        }

        public void ApriDaArgomentoExe()
        {
            String[] arguments = Environment.GetCommandLineArgs();
            if (arguments.Length > 1)
            {
                //toolStripStatusLabel1.Text = "Analyzing...";
                filepathBox.Text = arguments[1];
                //this.Update();
                //this.Refresh();
                Analisi(arguments[1]);
            }
        }

        private void ApriFileTextBox(object sender, EventArgs e)
        {
            Analisi(filepathBox.Text);
>>>>>>> 4bd9927c3a942e958602956aa9a1355fc9135b06:GetMediaInfo/MainWindow.xaml.cs
        }

        //drag e drop
        public void ApriFileDragDrop(object sender, System.Windows.DragEventArgs e)
        {
            Reset();
            System.Windows.DataObject data = (System.Windows.DataObject)e.Data;
            if (data.ContainsFileDropList())
            {
                string[] FileList = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop, false);
                filepathBox.Text = FileList[0];
                //Analisi(filepathBox.Text);
            }
        }

        public void ApriFileDragEnter(object sender, System.Windows.DragEventArgs e)
        {
            e.Effects = System.Windows.DragDropEffects.Copy;
        }

        /// //////////////////////////////////////////////////////////////////////////////////////////////// ///
        ///                                                                                                  ///
        ///                                              ANALISI                                             ///
        ///                                                                                                  ///
        /// //////////////////////////////////////////////////////////////////////////////////////////////// ///

        public void Analisi(string path)
        {
            Reset();
            if (File.Exists(path))
            {
                toolStripStatusLabel1.Content = "Analysing...";
                report = new MediaInfo(path);

                RealizzaReportSemplice();
                RealizzaReportTestuale();
                this.Title += " (" + System.IO.Path.GetFileName(path) + ")";
                toolStripStatusLabel1.Content = "Analysis Completed.";
                if(!AutoSaveReport.IsChecked.Value) SaveReport.IsEnabled = true;
                SaveReportActive = true;
            }
            else
            {
                toolStripStatusLabel1.Content = "Not existing file.";
                SaveReport.IsEnabled = SaveReportActive = false;
            }
        }

        
        /// //////////////////////////////////////////////////////////////////////////////////////////////// ///
        ///                                                                                                  ///
        ///                                  CREAZIONE E RESET REPORT                                        ///
        ///                                                                                                  ///
        /// //////////////////////////////////////////////////////////////////////////////////////////////// ///

        private void Reset()
        {
            TextBoxComplete.Text = TextBoxStandard.Text = "";
            this.Title = "GetMediaInfo";
            SaveReport.IsEnabled = SaveReportActive = false;
            sub_info.Text = audio_info.Text = video_info.Text = GeneralInfoText.Text = "";
        }

        private void RealizzaReportTestuale() // realizza il report testale
        {
            TextBoxStandard.Text = report.InfoStandard;
            TextBoxComplete.Text = report.InfoComplete;
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
        private string MSYesOrNo(string time)
        {
            if (!MilliSec.IsChecked.Value)
            {
                string[] durata = time.Split('.');
                return durata[0];
            }
            else return time;
        }
        private string AggiungiQuadreSeNonNullo(string title)
        {
            if (String.IsNullOrEmpty(title)) return " - ";
            else return " [" + title + "] ";
        }
        private void RealizzaReportSemplice() // realizza il report testale
        {
            GeneralInfoText.Text = "File Name: " + report.General[0].FileName + "." + report.General[0].FileExtension + Environment.NewLine;
            GeneralInfoText.Text += report.General[0].Format + ", " + report.General[0].FileSizePrecise + ", " + MSYesOrNo(report.General[0].PlayTimeFormatted) + Environment.NewLine;
            if (report.General[0].OveralBitRateString != "") GeneralInfoText.Text += "Overal BitRate: " + report.General[0].OveralBitRateString + Environment.NewLine;
            if (report.General[0].Title != "") GeneralInfoText.Text += "Title: " + report.General[0].Title + Environment.NewLine;

            // tracce video gen
            if (report.General[0].VideoCount == 1) GeneralInfoText.Text += "1 video track " + report.General[0].VideoCodecList + Environment.NewLine;
            else GeneralInfoText.Text += report.General[0].VideoCount + " track video " + report.General[0].VideoCodecList + Environment.NewLine;
            // tracce audio gen
            if (report.General[0].AudioCount == 1) GeneralInfoText.Text += "1 audio track " + report.General[0].AudioCodecList + Environment.NewLine;
            else GeneralInfoText.Text += report.General[0].AudioCount + " track audio " + report.General[0].AudioCodecList + Environment.NewLine;
            // tracce sub gen
            if (report.General[0].TextCount == 1) GeneralInfoText.Text += "1 sub track " + report.General[0].AudioCodecList + Environment.NewLine;
            else GeneralInfoText.Text += report.General[0].TextCount + " sub track " + report.General[0].TextCodecList + Environment.NewLine;
            //  capitoli
            if (report.General[0].Chapter != "") GeneralInfoText.Text += report.General[0].Chapter + Environment.NewLine;

            if (report.General[0].EncodedDate != "") GeneralInfoText.Text += "Date Encoded: " + report.General[0].EncodedDate + Environment.NewLine;
            if (report.General[0].EncodedLibrary != "") GeneralInfoText.Text += "Created with: " + report.General[0].EncodedLibrary + Environment.NewLine;


            // tracce video
            for (int i = 0; i < report.Video.Count; i++)
            {
                string lang = report.Video[i].LanguageString;
                if (String.IsNullOrEmpty(lang))
                {
                    lang = "Undetermined";
                }
                video_info.Text += lang + AggiungiQuadreSeNonNullo(report.Video[i].Title) + report.Video[i].DurationString + " - ";
                video_info.Text += report.Video[i].BitRateString + ", " + report.Video[i].Width + "×" + report.Video[i].Height + ", ";

                video_info.Text += report.Video[i].AspectRatioString + ", " + report.Video[i].FrameRate + " fps, ";
                video_info.Text += report.Video[i].CodecString + " (" + report.Video[i].FormatProfile + ") (";
                video_info.Text += report.Video[i].FormatSettings + ")" + Environment.NewLine;
                //video_info.Text += "Bitrate: "+report.Video[i].MinimumBitRateString + " - " + report.Video[i].NominalBitRateString + " - " + report.Video[i].MaximumBitRateString + Environment.NewLine;
            }

            // tracce audio
            string[] codecAudio = report.General[0].AudioCodecList.Split('/');
            for (int i = 0; i < report.Audio.Count; i++)
            {
                string lang = report.Audio[i].LanguageString;
                int delay = report.Audio[i].Delay;
                if (String.IsNullOrEmpty(lang))
                {
                    lang = "Undetermined";
                }
                audio_info.Text += lang + AggiungiQuadreSeNonNullo(report.Audio[i].Title) + report.Audio[i].DurationStringFormatted + " - ";
                audio_info.Text += report.Audio[i].BitRateString;
                if (report.Audio[i].BitRateMode !="")  audio_info.Text += " - " + report.Audio[i].BitRateMode; 
                audio_info.Text += ", " + report.Audio[i].SamplingRateString + ", ";
                audio_info.Text += report.Audio[i].ChannelsString + ", " + report.Audio[i].Format;

                if (!String.IsNullOrEmpty(report.Audio[i].FormatProfile))
                {
                    audio_info.Text += " - " + report.Audio[i].FormatProfile;
                }

                if (delay > 0)
                {
                    audio_info.Text += ", Delay: " + delay + "ms" + Environment.NewLine;
                }
                else
                {
                    audio_info.Text += Environment.NewLine;
                }
                //audio_info.Text += "Bitrate: " + report.Audio[i].BitRateMinimumString + " - " + report.Audio[i].BitRateNominalString + " - " + report.Audio[i].BitRateMaximumString + Environment.NewLine;
                //report.Audio[i].CodecIDUrl + report.Audio[i].CodecID + report.Audio[i].CodecIDDescription + report.Audio[i].CodecIDHint non vanno
                // come cavolo si reperiscono i codec audio -.-
            }

            // sub
            for (int i = 0; i < report.Text.Count; i++)
            {
                sub_info.Text += report.Text[i].LanguageString + AggiungiQuadreSeNonNullo(report.Text[i].Title);
                sub_info.Text += report.Text[i].Codec + Environment.NewLine;
            }
            if (report.Text.Count == 0) sub_info.Text = "No sub track";
            if (report.Audio.Count == 0) audio_info.Text = "No audio track";
            if (report.Video.Count == 0) video_info.Text = "No video track";

        }

        private void SaveReport_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog Report = new SaveFileDialog();
            Report.Filter = "Simple Text|*.txt";
            Report.Title = "Select the file you want to use to save the report";
            Report.DefaultExt = ".txt";
            Report.FileName = System.IO.Path.GetFileNameWithoutExtension(filepathBox.Text) + ".txt";
            Report.InitialDirectory = System.IO.Path.GetDirectoryName(filepathBox.Text);
            Report.ShowDialog();
            if (Report.FileName != "")
            {
                StreamWriter File = new StreamWriter(Report.FileName);
                if (tabControl.SelectedIndex == 2) File.WriteLine(TextBoxComplete.Text);
                else File.WriteLine(TextBoxStandard.Text);
                File.Close();
            }
        }

        private void DisableSimpleInfo_Checked(object sender, RoutedEventArgs e)
        {
            var tab = tabControl.Items[0] as TabItem;
            tab.Visibility = Visibility.Hidden;
            tab.Width = 0;
        }
        private void DisableSimpleInfo_Unchecked(object sender, RoutedEventArgs e)
        {
            var tab = tabControl.Items[0] as TabItem;
            tab.Visibility = Visibility.Visible;
            tab.Width = 52;
        }
        private void MS_Change(object sender, RoutedEventArgs e)
        {
            sub_info.Text = audio_info.Text = video_info.Text = GeneralInfoText.Text = "";
            RealizzaReportSemplice();
        }

        private void AutoSaveReport_Checked(object sender, RoutedEventArgs e)
        {
            SaveReport.IsEnabled = false;
        }
        private void AutoSaveReport_Unchecked(object sender, RoutedEventArgs e)
        {
            SaveReport.IsEnabled = SaveReportActive;
        }
    }
}
