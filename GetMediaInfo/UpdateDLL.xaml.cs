using SevenZip;
using System.Net;
using System.Windows;
using System.IO;
using System;
using System.ComponentModel;

namespace GetMediaInfoGUI
{
    /// <summary>
    /// Logica di interazione per UpdateDLL.xaml
    /// </summary>
    public partial class UpdateDLL : Window
    {
        
        #region GlobalVariables
        /// <summary>
        /// The worker
        /// </summary>
        private BackgroundWorker worker = new BackgroundWorker();

        /// <summary>
        /// Enumeration of the Current status of the operation
        /// </summary>
        enum Status { NotNeeded, Success, ed, MissingFiles, Error };
        
        /// <summary>
        /// The _status Enum Status variable
        /// </summary>
        private Status? _status;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateDLL"/> class.
        /// </summary>
        public UpdateDLL()
        {
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.ProgressChanged += worker_ProgressChanged;

            InitializeComponent();
            CancelButton.IsEnabled = false;

            PerformUpdate();
        }
        #endregion

        #region InterfaceHandlers
        /// <summary>
        /// Handles the Click event of the CancelButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            worker.CancelAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="CancelEventArgs"/> instance containing the event data.</param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            worker.CancelAsync();
        }
        #endregion

        #region HelperMethods
        /// <summary>
        /// Gets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public string status
        {
            get
            {
                switch (_status)
                {
                    case Status.NotNeeded: return "Mediainfo.dll is up-to-date.";
                    case Status.ed: return "Updated ed by user.";
                    case Status.Error: return "Failed to update.";
                    case Status.Success: return "Mediainfo.dll was successfully updated.";
                    case Status.MissingFiles: return "Unable to update ( missing 7z.dll / SevenZipSharp.dll).";
                    default: return "Generic error.";
                }
            }
        }

        /// <summary>
        /// Performs the update.
        /// </summary>
        private void PerformUpdate()
        {
            if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\7z.dll") &&
                File.Exists(System.Windows.Forms.Application.StartupPath + "\\SevenZipSharp.dll"))
            {
                CancelButton.IsEnabled = true;
                worker.RunWorkerAsync();
                _status = null;
            }
            else
            {
                Log.AppendText("Unable to update, some libraries are missing (7z.dll and/or SevenZipSharp.dll).");
                _status = Status.MissingFiles;
            }
        }

        /// <summary>
        /// Gets the latest mediainfo version.
        /// </summary>
        /// <returns>String as the latest available mediainfo version</returns>
        private string GetLatestMediainfoVersion()
        {
            var reader = System.Xml.XmlTextReader.Create("https://mediaarea.net/rss/mediainfo_updates.xml");
            reader.ReadToFollowing("title");
            reader.ReadToFollowing("title");
            string latest = reader.ReadElementContentAsString();
            latest = latest.Replace("MediaInfo ", "");
            return latest;
        }

        /// <summary>
        /// Gets the local mediainfo version.
        /// </summary>
        /// <returns></returns>
        private string GetLocalMediainfoVersion()
        {
            if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\Mediainfo.dll"))
            {
                var tmp = System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Windows.Forms.Application.StartupPath + "\\Mediainfo.dll");

                if (!String.IsNullOrEmpty(tmp.FileVersion))
                {
                    return tmp.FileVersion;
                }
            }

            return String.Empty;
        }
        #endregion

        #region Threads
        /// <summary>
        /// Handles the DoWork event of the worker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DoWorkEventArgs"/> instance containing the event data.</param>
        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (worker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            worker.ReportProgress(5, "Verifying local library version...");
            string currmediainfo = GetLocalMediainfoVersion();
            if (!string.IsNullOrEmpty(currmediainfo))
            {
                worker.ReportProgress(45, "Found version: " + currmediainfo);
            } else
            {
                worker.ReportProgress(45, "Failed");
            }

            if (worker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            worker.ReportProgress(50, "Verifying latest remote library version...");
            string newvermediainfo = GetLatestMediainfoVersion();
            if (!string.IsNullOrEmpty(newvermediainfo))
            {
                worker.ReportProgress(95, "Found version: " + newvermediainfo);
            }
            else
            {
                worker.ReportProgress(95, "Failed");
            }

            worker.ReportProgress(100);

            if (!currmediainfo.Contains(newvermediainfo))
            {
                SevenZipExtractor.SetLibraryPath(System.Windows.Forms.Application.StartupPath + "\\7z.dll");
                string url = Environment.Is64BitProcess ? "http://mediaarea.net/download/binary/libmediainfo0/" + newvermediainfo +
                                            "/MediaInfo_DLL_" + newvermediainfo + "_Windows_x64_WithoutInstaller.7z" :
                                            "http://mediaarea.net/download/binary/libmediainfo0/" + newvermediainfo +
                                            "/MediaInfo_DLL_" + newvermediainfo + "_Windows_i386_WithoutInstaller.7z";
                string filename = url.Substring(url.LastIndexOf("/") + 1, url.Length - url.LastIndexOf("/") - 1);
                string filetoextract = "MediaInfo.dll";
                string newfilepath = System.Windows.Forms.Application.StartupPath + "\\" + filetoextract;
                string downloadpath = System.Windows.Forms.Application.StartupPath + "\\tmp\\" + filename;
                
                if (!Directory.Exists(System.Windows.Forms.Application.StartupPath + "\\tmp\\"))
                {
                    Directory.CreateDirectory(System.Windows.Forms.Application.StartupPath + "\\tmp\\");
                }

                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                //download
                worker.ReportProgress(5, "Downloading latest version of the library...");

                using (WebClient client = new WebClient())
                {
                    client.DownloadProgressChanged += Client_DownloadProgressChanged;
                    client.DownloadFileCompleted += Client_DownloadFileCompleted;
                    try
                    {
                        client.DownloadFile(new Uri(url), downloadpath);
                    }
                    catch (Exception ex)
                    {
                        worker.ReportProgress(100, "Download failed. Error message: " + ex.Message);
                        _status = Status.Error;
                        return;
                    }
                }

                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                //extract
                using (SevenZipExtractor archive = new SevenZipExtractor(downloadpath))
                {
                    archive.Extracting += Archive_Extracting;
                    archive.ExtractionFinished += Archive_ExtractionFinished;

                    worker.ReportProgress(5, "Deleting old files...");

                    if (File.Exists(newfilepath))
                    {
                        try
                        {
                            File.Delete(newfilepath);
                            worker.ReportProgress(100, "Old file has been successfully removed");
                        }
                        catch (Exception ex)
                        {
                            worker.ReportProgress(100, "Cannot remove old files. Error message: " + ex.Message);
                            _status = Status.Error;
                            return;
                        }
                    }

                    worker.ReportProgress(5, "Start extraction of the downloaded archive...");

                    FileStream fileStream = File.Create(newfilepath);

                    foreach (string file in archive.ArchiveFileNames)
                    {
                        if (file.Contains(filetoextract))
                        {
                            filetoextract = file;
                            break;
                        }
                    }

                    try
                    {
                        archive.ExtractFile(filetoextract, fileStream);
                    }
                    catch (Exception ex)
                    {
                        worker.ReportProgress(100, "Extraction failed. Error message: " + ex.Message);
                        _status = Status.Error;
                        return;
                    }
                }
                
                //delete temp files
                try
                {
                    File.Delete(downloadpath);
                    Directory.Delete(System.Windows.Forms.Application.StartupPath + "\\tmp\\", true);
                }
                catch { }
            }
            else
            {
                worker.ReportProgress(100, "Mediainfo is already at the latest version. No need to update.");
                _status = Status.NotNeeded;
            }
        }

        /// <summary>
        /// Handles the Extracting event of the Archive control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ProgressEventArgs"/> instance containing the event data.</param>
        private void Archive_Extracting(object sender, ProgressEventArgs e)
        {
            int progress = e.PercentDone;

            if (progress > 0)
            {
                if (progress > 100)
                {
                    progress = 100;
                }

                Dispatcher.Invoke(new Action(delegate { ProgressUpdate.Value = progress; }));           
            }
        }

        /// <summary>
        /// Handles the ExtractionFinished event of the Archive control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Archive_ExtractionFinished(object sender, EventArgs e)
        {
            Dispatcher.Invoke(new Action(delegate { Log.AppendText("Extraction completed!" + Environment.NewLine); }));
        }

        /// <summary>
        /// Handles the DownloadProgressChanged event of the Client control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DownloadProgressChangedEventArgs"/> instance containing the event data.</param>
        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            int progress = e.ProgressPercentage;
            string bytesloaded = ((e.TotalBytesToReceive / 1024f) / 1024f).ToString("0.00");

            if (progress > 0)
            {
                if (progress > 100)
                {
                    progress = 100;
                }

                ProgressUpdate.Value = progress;
            }

            if (!string.IsNullOrEmpty(bytesloaded))
            {
                Log.AppendText("Received: " + bytesloaded + "Mb" + Environment.NewLine);
            }
        }

        /// <summary>
        /// Handles the DownloadFileCompleted event of the Client control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="AsyncCompletedEventArgs"/> instance containing the event data.</param>
        private void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            Log.AppendText("Download completed!" + Environment.NewLine);
        }

        /// <summary>
        /// Handles the ProgressChanged event of the worker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ProgressChangedEventArgs"/> instance containing the event data.</param>
        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            int progress = e.ProgressPercentage;
            string status = (string)e.UserState;

            if (progress > 0)
            {
                if (progress > 100)
                {
                    progress = 100;
                }

                ProgressUpdate.Value = progress;
            }

            if (!string.IsNullOrEmpty(status))
            {
                Log.AppendText(status + Environment.NewLine);
            }
        }

        /// <summary>
        /// Handles the RunWorkerCompleted event of the worker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RunWorkerCompletedEventArgs"/> instance containing the event data.</param>
        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled && e.Error == null)
            {
                Log.AppendText("All operations successfully completed!" + Environment.NewLine);
                ProgressUpdate.Value = 100;
                if(_status == null)
                {
                    _status = Status.Success;
                }
            }
            if (e.Cancelled)
            {
                Log.AppendText("Operation ed by the user!" + Environment.NewLine);
                ProgressUpdate.Value = 100;
                _status = Status.ed;
            }
            if (e.Error != null)
            {
                Log.AppendText("Exception generated! Error message: " + e.Error.Message + Environment.NewLine);
                ProgressUpdate.Value = 100;
                _status = Status.Error;
            }

            CancelButton.IsEnabled = false;
            System.Threading.Thread.Sleep(2000);
            this.Close();
        }
        #endregion
    }
}
