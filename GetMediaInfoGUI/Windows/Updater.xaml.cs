using SevenZip;
using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Xml;

namespace GetMediaInfoGUI
{
    /// <summary>
    /// Interaction logic for UpdateComponent.xaml
    /// </summary>
    public partial class Updater : Window
    {
        #region Global Variables

        private static string mediainfoBaseUrl = "https://mediaarea.net/download/binary/libmediainfo0/";

        /// <summary>
        /// Enumeration of the Current status of the operation.
        /// </summary>
        public enum Status
        {
            NotNeeded,
            Success,
            ed,
            MissingFiles,
            Error
        };

        /// <summary>
        /// The _status Enum Status variable.
        /// </summary>
        private Status? status;

        /// <summary>
        /// The worker.
        /// </summary>
        private BackgroundWorker BackgroundWorker = new BackgroundWorker();

        /// <summary>
        /// The semaphore to sync background worker and extraction.
        /// </summary>
        private Semaphore semaphore;

        /// <summary>
        /// The flag used to  activities.
        /// </summary>
        private bool  = false;
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Updater"/> class.
        /// </summary>
        public Updater()
        {
            BackgroundWorker.WorkerReportsProgress = true;
            BackgroundWorker.WorkerSupportsCancellation = true;
            BackgroundWorker.DoWork += BackgroundWorker_DoWork;
            BackgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
            BackgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;

            InitializeComponent();
            CancelButton.IsEnabled = false;
            semaphore = new Semaphore(0, 1);
            PerformUpdate();
        }

        #endregion

        #region Interface Handlers

        /// <summary>
        /// Handles the Click event of the CancelButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            BackgroundWorker.CancelAsync();
             = true;
            try
            {
                semaphore.Release();
            }
            catch { }      
        }

        /// <summary>
        /// Handles the Closing event of the Windows.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="CancelEventArgs"/> instance containing the event data.</param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            //delete temp files
            try
            {
                Directory.Delete(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tmp"), true);
            }
            catch { }

            BackgroundWorker.CancelAsync();
        }

        /// <summary>
        /// Handles the Collapsed event of the expander control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            ResizeMode = ResizeMode.CanResize;
            Expander.Height -= 150;
            ExpanderGrid.Height = new GridLength(ExpanderGrid.Height.Value - 150.0);
            Height -= 150;
            ResizeMode = ResizeMode.NoResize;
            Expander.Header = "Further information";

        }

        /// <summary>
        /// Handles the Expanded event of the expander control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            ResizeMode = ResizeMode.CanResize;
            Expander.Height += 150;
            ExpanderGrid.Height = new GridLength(ExpanderGrid.Height.Value + 150.0);
            Height += 150;
            ResizeMode = ResizeMode.NoResize;
            Expander.Header = "Less details";
        }

        #endregion

        #region Helper Methods        
        /// <summary>
        /// Gets the message status.
        /// </summary>
        /// <returns>The message status.</returns>
        public string GetMessageStatus()
        {
            switch (status)
            {
                case Status.NotNeeded: return "MediaInfo library is already at the latest version.";
                case Status.ed: return "Mediainfo library update have been ed by the user.";
                case Status.Error: return "Mediainfo library update failed.";
                case Status.Success: return "MediaInfo library have been successfully updated.";
                case Status.MissingFiles: return "Unable to update ( missing 7z.dll / SevenZipSharp.dll).";
                default: return "Generic error.";
            }
        }

        /// <summary>
        /// Gets the status of the updater.
        /// </summary>
        /// <returns>The status of the updater.</returns>
        public Status GetStatus()
        {
            return status.GetValueOrDefault(Status.Error);
        }

        /// <summary>
        /// Performs the update.
        /// </summary>
        private void PerformUpdate()
        {
            if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "7z.dll")) &&
                File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SevenZipSharp.dll")))
            {
                CancelButton.IsEnabled = true;
                BackgroundWorker.RunWorkerAsync();
                status = null;
            }
            else
            {
                Log.AppendText("Unable to update, some libraries are missing (7z.dll and/or SevenZipSharp.dll).");
                status = Status.MissingFiles;
            }
        }

        /// <summary>
        /// Gets the latest MediaInfo version.
        /// </summary>
        /// <returns>String as the latest available MediaInfo version</returns>
        private string GetLatestMediaInfoVersion()
        {
            XmlReader XmlReader = XmlReader.Create("https://mediaarea.net/rss/mediainfo_updates.xml");
            XmlReader.ReadToFollowing("title");
            XmlReader.ReadToFollowing("title");
            string latest = XmlReader.ReadElementContentAsString();
            latest = latest.Replace("MediaInfo ", "");
            return latest;
        }

        /// <summary>
        /// Gets the local MediaInfo version.
        /// </summary>
        /// <returns></returns>
        private string GetLocalMediaInfoVersion()
        {
            if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MediaInfo.dll")))
            {
                System.Diagnostics.FileVersionInfo FileVersionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MediaInfo.dll"));

                if (!string.IsNullOrEmpty(FileVersionInfo.FileVersion))
                {
                    return FileVersionInfo.FileVersion;
                }
            }

            return string.Empty;
        }

        #endregion

        #region Threads

        /// <summary>
        /// Handles the DoWork event of the worker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DoWorkEventArgs"/> instance containing the event data.</param>
        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Dispatcher.Invoke(new Action(delegate { simpleStatus.Content = "Step 1 of 3"; }));

            if (BackgroundWorker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            BackgroundWorker.ReportProgress(5, "Verifying local library version...");
            string currMediaInfo = GetLocalMediaInfoVersion();

            if (!string.IsNullOrEmpty(currMediaInfo))
            {
                BackgroundWorker.ReportProgress(45, "Found version: " + currMediaInfo);
            }
            else
            {
                BackgroundWorker.ReportProgress(45, "Failed");
            }

            if (BackgroundWorker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            BackgroundWorker.ReportProgress(50, "Verifying latest remote library version...");
            string newVerMediaInfo = GetLatestMediaInfoVersion();

            if (!string.IsNullOrEmpty(newVerMediaInfo))
            {
                BackgroundWorker.ReportProgress(95, "Found version: " + newVerMediaInfo);
            }
            else
            {
                BackgroundWorker.ReportProgress(95, "Failed");
            }

            BackgroundWorker.ReportProgress(100);

            if (!currMediaInfo.Contains(newVerMediaInfo))
            {
                SevenZipExtractor.SetLibraryPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "7z.dll"));

                string tempDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tmp");

                string url = mediainfoBaseUrl + (Environment.Is64BitProcess ? 
                    newVerMediaInfo + "/MediaInfo_DLL_" + newVerMediaInfo + "_Windows_x64_WithoutInstaller.7z" :
                    newVerMediaInfo + "/MediaInfo_DLL_" + newVerMediaInfo + "_Windows_i386_WithoutInstaller.7z");

                string fileName = url.Substring(url.LastIndexOf("/") + 1, url.Length - url.LastIndexOf("/") - 1);
                string fileToExtract = "MediaInfo.dll";

                string newfilepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileToExtract);
                string downloadpath = Path.Combine(tempDir, fileName);

                if (!Directory.Exists(tempDir))
                {
                    Directory.CreateDirectory(tempDir);
                }

                if (BackgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                Dispatcher.Invoke(new Action(delegate { simpleStatus.Content = "Step 2 of 3"; }));

                //download
                BackgroundWorker.ReportProgress(1, "Downloading latest version of the library...");

                using (WebClient webClient = new WebClient())
                {
                    webClient.DownloadProgressChanged += Client_DownloadProgressChanged;
                    webClient.DownloadFileCompleted += Client_DownloadFileCompleted;

                    try
                    {
                        webClient.DownloadFileAsync(new Uri(url), downloadpath);
                        if (!semaphore.WaitOne(TimeSpan.FromSeconds(10)))
                        {
                            throw new TimeoutException("Download failed due to timeout.");
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Download failed. Error message: " + ex.Message);
                    }
                }

                if (BackgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                Dispatcher.Invoke(new Action(delegate { simpleStatus.Content = "Step 3 of 3"; }));

                //extract
                using (SevenZipExtractor extractor = new SevenZipExtractor(downloadpath))
                {
                    extractor.Extracting += Archive_Extracting;
                    extractor.ExtractionFinished += Archive_ExtractionFinished;

                    BackgroundWorker.ReportProgress(5, "Deleting old files...");

                    if (File.Exists(newfilepath))
                    {
                        try
                        {
                            File.Delete(newfilepath);
                            BackgroundWorker.ReportProgress(100, "Old file has been successfully removed");
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Cannot remove old files. Error message: " + ex.Message);
                        }
                    }

                    FileStream fileStream = File.Create(newfilepath);

                    BackgroundWorker.ReportProgress(5, "Start extraction of the downloaded archive...");
                  
                    foreach (string file in extractor.ArchiveFileNames)
                    {
                        if (file.Contains(fileToExtract))
                        {
                            fileToExtract = file;
                            break;
                        }
                    }
                   
                    try
                    {
                        extractor.ExtractFile(fileToExtract, fileStream);
                        if (!semaphore.WaitOne(TimeSpan.FromSeconds(10)))
                        {
                            throw new TimeoutException("Extraction failed due to timeout.");
                        }
                    }
                    catch (Exception ex)
                    {              
                        throw new Exception("Extraction failed. Error message: " + ex.Message);
                    }
                    finally
                    {
                        fileStream.Dispose();
                    }
                }
            }
            else
            {
                Dispatcher.Invoke(new Action(delegate { simpleStatus.Content = "Step 3 of 3"; }));

                BackgroundWorker.ReportProgress(100, "MediaInfo is already at the latest version. No need to update.");
                e.Result = Status.NotNeeded;
            }
        }

        /// <summary>
        /// Handles the ProgressChanged event of the worker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ProgressChangedEventArgs"/> instance containing the event data.</param>
        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
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
                ProgressLabel.Content = progress + "%";
            }

            if (!string.IsNullOrEmpty(status))
            {
                Log.AppendText(status + Environment.NewLine);
                Log.ScrollToEnd();
            }

            this.InvalidateVisual();
        }

        /// <summary>
        /// Handles the RunWorkerCompleted event of the worker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RunWorkerCompletedEventArgs"/> instance containing the event data.</param>
        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            CancelButton.IsEnabled = false;
            ProgressUpdate.Value = 100;

            if (e.Cancelled || )
            {
                Log.AppendText("Operation ed by the user!" + Environment.NewLine);
                ProgressLabel.Content = "ed";
                status = Status.ed;
            }
            else if (e.Error != null)
            {
                Log.AppendText("Exception generated! " + e.Error.Message + Environment.NewLine);
                ProgressLabel.Content = "Error";
                status = Status.Error;
            }
            else if (e.Result != null && e.Result.Equals(Status.NotNeeded))
            {
                Log.AppendText("Analysis completed: no need to update." + Environment.NewLine);
                status = Status.NotNeeded;
                Close();
            } else {
                Log.AppendText("All operations successfully completed!" + Environment.NewLine);
                status = Status.Success;
                Close();
            }
        }

        /// <summary>
        /// Handles the DownloadProgressChanged event of the Client control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DownloadProgressChangedEventArgs"/> instance containing the event data.</param>
        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if ()
            {
                ((WebClient)sender).CancelAsync();
                return;
            }

            int progress = e.ProgressPercentage;
            string bytesloaded = ((e.TotalBytesToReceive / 1024f) / 1024f).ToString("0.00");

            if (progress > 0)
            {
                if (progress > 100)
                {
                    progress = 100;
                }

                Dispatcher.Invoke(new MethodInvoker(delegate
                {
                    ProgressUpdate.Value = progress;
                    ProgressLabel.Content = progress + "%";
                }));
            }
        }

        /// <summary>
        /// Handles the DownloadFileCompleted event of the Client control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="AsyncCompletedEventArgs"/> instance containing the event data.</param>
        private void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (!e.Cancelled && e.Error == null) {
                Dispatcher.Invoke(new MethodInvoker(delegate
                {
                    Log.AppendText("Download completed!" + Environment.NewLine);
                    Log.ScrollToEnd();
                    ProgressUpdate.Value = 100;
                    ProgressLabel.Content = "100%";
                }));
            }
            semaphore.Release();
        }

        /// <summary>
        /// Handles the Extracting event of the Archive control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ProgressEventArgs"/> instance containing the event data.</param>
        private void Archive_Extracting(object sender, ProgressEventArgs e)
        {
            if ()
            {
                e.Cancel = true;
            }

            int progress = e.PercentDone;

            if (progress > 0)
            {
                if (progress > 100)
                {
                    progress = 100;
                }

                Dispatcher.Invoke(new MethodInvoker(delegate { ProgressUpdate.Value = progress; }));
            }
        }

        /// <summary>
        /// Handles the ExtractionFinished event of the Archive control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Archive_ExtractionFinished(object sender, EventArgs e)
        {
            Dispatcher.Invoke(new MethodInvoker(delegate
            {
                Log.AppendText("Extraction completed!" + Environment.NewLine);
                Log.ScrollToEnd();
                ProgressUpdate.Value = 100;
                ProgressLabel.Content = "100%";
            }));

            semaphore.Release();
        }
        #endregion
    }
}