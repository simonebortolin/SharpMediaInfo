using GetMediaInfos;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace GetMediaInfo
{
    public partial class GetMediaInfo : Form
    {
        bool removed1 = false, removed2 = false, removed3 = false;

        /// //////////////////////////////////////////////////////////////////////////////////////////////// ///

        public GetMediaInfo()
        {
            InitializeComponent();
        }

        private void PrimoAvvio(object sender, EventArgs e)
        {
            this.DoubleBuffered = true;
            Version.Text = "Version " + Application.ProductVersion;
            if (File.Exists(Application.StartupPath + "\\mediainfo.dll"))
            {
                FileVersionInfo tmp = FileVersionInfo.GetVersionInfo(Application.StartupPath + "\\MediaInfo.dll");
                StatusMediainfo.Text = "OK v" + tmp.FileVersion;
                StatusMediainfo.ForeColor = Color.FromName("Green");
            }
            else
            {
                StatusMediainfo.Text = "ERROR";
                StatusMediainfo.ForeColor = Color.FromName("Red");
                MessageBox.Show("Library MediaInfo.dll not found.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            if (File.Exists(Application.StartupPath + "\\GetMediaInfos.dll"))
            {
                FileVersionInfo tmp = FileVersionInfo.GetVersionInfo(Application.StartupPath + "\\GetMediaInfos.dll");
                StatusGetMediaInfos.Text = "OK v" + tmp.FileVersion;
                StatusGetMediaInfos.ForeColor = Color.FromName("Green");
            }
            else
            {
                StatusGetMediaInfos.Text = "ERROR";
                StatusGetMediaInfos.ForeColor = Color.FromName("Red");
                MessageBox.Show("Library GetMediaInfos.dll not found.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        private void Associate_Click(object sender, EventArgs e)
        {
            List<string> listext = new List<string>() { ".mkv", ".ac3", ".dts", ".avi", ".m2ts", ".dtshd", ".m4a", ".mp4" };

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
                key.SetValue("Icon", "\"" + Application.StartupPath + "\\icon.ico\"");
                key = key.CreateSubKey("command");
                key.SetValue("", "\"" + Application.ExecutablePath + "\" \"%1\"");
                key.Close();
            }
            Associationstatus.Text = "Successufully Added.";
        }

        private void RemoveAssociation_Click(object sender, EventArgs e)
        {
            List<string> listext = new List<string>() { ".mkv", ".ac3", ".dts", ".avi", ".m2ts", ".dtshd", ".m4a", ".mp4" };

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
            Associationstatus.Text = "Successufully Removed.";
        }

        private void GetMediaInfo_Shown(object sender, EventArgs e)
        {
            this.Update();
            this.Refresh();
            ApriDaArgomentoExe();
        }

        /// //////////////////////////////////////////////////////////////////////////////////////////////// ///
        ///                                                                                                  ///
        ///                                          FILE OPEN                                               ///
        ///                                                                                                  ///
        /// //////////////////////////////////////////////////////////////////////////////////////////////// ///

        public void ApriDaArgomentoExe()
        {
            String[] arguments = Environment.GetCommandLineArgs();
            if (arguments.Length > 1)
            {
                toolStripStatusLabel1.Text = "Analyzing...";
                filepathBox.Text = arguments[1];
                this.Update();
                this.Refresh();
                Analisi(arguments[1]);
            }
        }

        private void ApriFileDialog(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            file.Filter = "MEDIA FILE|*.*";
            //file.Filter = "Matorska|*.mkv;*.mka;*.mkv;*.mk3d|MPEG-4|*.mp4;*.m4a;*.m4v;*m4b;|AVI|*.avi|FLAC|*.flac|MP3|*.mp3";
            //file.Filter += "|OGM|*.ogg;*.ogm;*.oga;*.ogv;*.ogx|WEB-M|*.weba;*.webv|Windows Media|*.wmv;*.wma|";
            file.Title = "Select a Media File";
            file.Multiselect = false;

            if (file.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                filepathBox.Text = file.FileName;
                Analisi(filepathBox.Text);
            }



        }

        public void ApriFileDragDrop(object sender, DragEventArgs e)
        {
            DataObject data = (DataObject)e.Data;
            if (data.ContainsFileDropList())
            {
                string[] FileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);
                filepathBox.Text = FileList[0];
                Analisi(filepathBox.Text);
            }
        }

        public void ApriFileDragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void ApriFileTextBox(object sender, EventArgs e)
        {
            Analisi(filepathBox.Text);
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
                toolStripStatusLabel1.Text = "Analysing...";
                MediaInfo tmp = new MediaInfo(path);
                
                RealizzaReportSemplice(tmp);
                RealizzaReportTestuale(tmp);
                this.Text += " (" + path + ")";
                toolStripStatusLabel1.Text = "Analysis Completed.";
                btnSalvaReport.Enabled = true;
            }
            else
            {
                toolStripStatusLabel1.Text = "Not existing file.";
                btnSalvaReport.Enabled = false;
            }

        }

        /// //////////////////////////////////////////////////////////////////////////////////////////////// ///
        ///                                                                                                  ///
        ///                                  CREAZIONE E RESET REPORT                                        ///
        ///                                                                                                  ///
        /// //////////////////////////////////////////////////////////////////////////////////////////////// ///
        
        private void Reset() // azzera i report
        {
            btnSalvaReport.Enabled = false;
            toolStripStatusLabel1.Text = "Ready";
            textBox3.Text = textBox2.Text = "";
            this.Text = "Get Media Infos";
            label3.Text = "";
            //filepath = "";
            //filename = "";
            general_info1.Text = "";
            general_info2.Text = "";
            video_info.Text = "";
            audio_info.Text = "";
            sub_info.Text = "";
            if(removed1)
            {
                tableLayoutPanel1.RowStyles.Insert(1, new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 22.22222F)); tableLayoutPanel1.RowCount++;
                groupBox2.Visible = true; removed1 = false;
            }
            if (removed2)
            {
                tableLayoutPanel1.RowStyles.Insert(2, new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 22.22222F)); tableLayoutPanel1.RowCount++;
                groupBox3.Visible = true; removed2 = false;
            }
            if (removed3)
            {
                tableLayoutPanel1.RowStyles.Insert(3, new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 22.22222F)); tableLayoutPanel1.RowCount++;
                groupBox4.Visible = true; removed3 = false;
            }

        }

        private string AggiungiQuadreSeNonNullo(string temp)
        {
            if (temp == "") return "";
            if (temp == " ") return "";
            else return " [" + temp + "] - ";
        }

        private string SistemaBitrete(MediaInfo report)
        {
            ulong bitrateSum = 0;
            for (int i = 0; i < report.Video.Count; i++)
            {
                bitrateSum += report.Video[i].BitRate;
            }
            for (int i = 0; i < report.Audio.Count; i++)
            {
                bitrateSum += report.Audio[i].BitRate;
            }
            if (bitrateSum < 100000) return (float)(bitrateSum / 100)/(float)10 + "kbps";
            else if (bitrateSum < 1000000000) return (float)(bitrateSum / 100000) / (float)10 + "Mbps";
            else return (float)(bitrateSum / 100000000) / (float)10 + "Gbps";

        }

        private void RealizzaReportSemplice(MediaInfo report) // realizza il report grafico
        {
            // info generali
            if (report.General[0].VideoCount == 0) { tableLayoutPanel1.RowStyles.RemoveAt(1); tableLayoutPanel1.RowCount--; groupBox2.Visible = false; removed1 = true; }
            if (report.General[0].AudioCount == 0) { tableLayoutPanel1.RowStyles.RemoveAt(removed1?1:2); tableLayoutPanel1.RowCount--; groupBox3.Visible = false; removed2 = true; }
            if (report.General[0].TextCount == 0) { tableLayoutPanel1.RowStyles.RemoveAt(--tableLayoutPanel1.RowCount); groupBox4.Visible = false; removed3 = true; }

            label3.Text = "File Name: " + report.General[0].FileName + "." + report.General[0].FileExtension;
            //general_info1.Text += report.General[0].UniqueID + Environment.NewLine;

            string[] durata = report.General[0].PlayTimeFormatted.Split('.');
            /*if (report.General[0].VideoCount != 0) durata = report.Video[0].DurationString;
            else if (report.General[0].AudioCount != 0) durata = report.Audio[0].DurationStringFormatted;
            else durata = "Indefinite";*/

            general_info1.Text += report.General[0].Format + ", " + report.General[0].FileSizePrecise + ", " + durata[0] + Environment.NewLine;
            

            // tracce video gen
            if (report.General[0].VideoCount == 1) general_info1.Text += "1 video track " + report.General[0].VideoCodecList + Environment.NewLine;
            else general_info1.Text += report.General[0].VideoCount + " track video " + report.General[0].VideoCodecList + Environment.NewLine;
            // tracce audio gen
            if (report.General[0].AudioCount == 1) general_info1.Text += "1 audio track " + report.General[0].AudioCodecList + Environment.NewLine;
            else general_info1.Text += report.General[0].AudioCount + " track audio " + report.General[0].AudioCodecList + Environment.NewLine;
            if(report.General[0].OveralBitRateString!="") general_info2.Text += "Overal BitRate: " + report.General[0].OveralBitRateString + Environment.NewLine;
            else general_info2.Text += "Overal BitRate: " + SistemaBitrete(report) + Environment.NewLine;
            general_info2.Text += "Title: " + report.General[0].Title + Environment.NewLine;
            general_info2.Text += "Date Encoded: " + report.General[0].EncodedDate + Environment.NewLine;
            general_info2.Text += "Created with: " + report.General[0].EncodedLibrary + Environment.NewLine;


            // tracce video
            for (int i = 0; i < report.Video.Count; i++)
            {
                video_info.Text += report.Video[i].LanguageString + " - " + report.Video[i].DurationMillisec + "s - " + AggiungiQuadreSeNonNullo(report.Video[i].Title);
                video_info.Text += report.Video[i].BitRateString +", " + report.Video[i].Width + "×" + report.Video[i].Height + ", ";

                video_info.Text += report.Video[i].AspectRatioString + ", " + report.Video[i].FrameRate + " fps, ";
                video_info.Text += report.Video[i].CodecString + " (" + report.Video[i].FormatProfile + ") (";
                video_info.Text += report.Video[i].FormatSettings + ")" + Environment.NewLine;
                //video_info.Text += "Bitrate: "+report.Video[i].MinimumBitRateString + " - " + report.Video[i].NominalBitRateString + " - " + report.Video[i].MaximumBitRateString + Environment.NewLine;
            }

            // tracce audio
            string[] codecAudio = report.General[0].AudioCodecList.Split('/');
            for (int i = 0; i < report.Audio.Count; i++)
            {
                audio_info.Text += report.Audio[i].LanguageString + " - " + report.Audio[i].DurationMillisec + "s - " + AggiungiQuadreSeNonNullo(report.Audio[i].Title);
                audio_info.Text += report.Audio[i].BitRateString +" " + report.Audio[i].BitRateMode +", " +report.Audio[i].SamplingRateString +  ", " ;
                audio_info.Text += report.Audio[i].ChannelsString + ", " + codecAudio[i] + Environment.NewLine;
                //audio_info.Text += "Bitrate: " + report.Audio[i].BitRateMinimumString + " - " + report.Audio[i].BitRateNominalString + " - " + report.Audio[i].BitRateMaximumString + Environment.NewLine;
                //report.Audio[i].CodecIDUrl + report.Audio[i].CodecID + report.Audio[i].CodecIDDescription + report.Audio[i].CodecIDHint non vanno
                // come cavolo si reperiscono i codec audio -.-
            }

            // sub
            for (int i=0; i< report.Text.Count; i++)
            {
                sub_info.Text+= report.Text[i].LanguageString + " - " + AggiungiQuadreSeNonNullo(report.Text[i].Title);
                sub_info.Text += report.Text[i].Codec + Environment.NewLine;
            }

        }



        private void RealizzaReportTestuale(MediaInfo tmp) // realizza il report testale
        {
            textBox2.Text = tmp.InfoStandard;
            textBox3.Text = tmp.InfoComplete;
        }

        /// //////////////////////////////////////////////////////////////////////////////////////////////// ///
        ///                                                                                                  ///
        ///                             SALVATAGGIO REPORT SU FILE TXT                                       ///
        ///                                                                                                  ///
        /// //////////////////////////////////////////////////////////////////////////////////////////////// ///
       
        private void SalvaReportSuFile(object sender, EventArgs e) // salva
        {
            SaveFileDialog Report = new SaveFileDialog();
            Report.Filter = "Simple Text|*.txt";
            Report.Title = "Select the file you want to use to save the report";
            Report.DefaultExt = ".txt";
            Report.FileName = Path.GetFileNameWithoutExtension(filepathBox.Text) + ".txt";
            Report.InitialDirectory = Path.GetDirectoryName(filepathBox.Text);
            Report.ShowDialog();
            if (Report.FileName != "")
            {
                StreamWriter File = new StreamWriter(Report.FileName);
                if(tabControl1.SelectedIndex==2) File.WriteLine(textBox3.Text);
                else File.WriteLine(textBox2.Text);
                File.Close();
            }
        }
    }
}
