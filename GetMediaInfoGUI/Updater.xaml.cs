using SevenZip;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows;
using System.Xml;

namespace GetMediaInfoGUI
{
    public partial class Updater : Window
    {
        private static string mediainfoBaseUrl = "https://mediaarea.net/download/binary/libmediainfo0/";
        private BackgroundWorker BackgroundWorker = new BackgroundWorker();
        private Status? status;
        private Semaphore semaphore;
        private bool ;

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

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            BackgroundWorker.CancelAsync();
             = true;
            semaphore.Release();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            try
            {
                Directory.Delete(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tmp"), true);
            }
            catch
            {
            }
            BackgroundWorker.CancelAsync();
        }

        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            ResizeMode = ResizeMode.CanResize;
            Expander.Height -= 150.0;
            ExpanderGrid.Height = new GridLength(ExpanderGrid.Height.Value - 150.0);
            Height -= 150.0;
            ResizeMode = ResizeMode.NoResize;
            Expander.Header = "Further information";
        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            ResizeMode = ResizeMode.CanResize;
            Expander.Height += 150.0;
            ExpanderGrid.Height = new GridLength(ExpanderGrid.Height.Value + 150.0);
            Height += 150.0;
            ResizeMode = ResizeMode.NoResize;
            Expander.Header = "Less details";
        }

        public string GetMessageStatus()
        {
            Status? status = this.status;
            if (status.HasValue)
            {
                switch (status.GetValueOrDefault())
                {
                    case Status.NotNeeded:
                        return "MediaInfo library is already at the latest version.";
                    case Status.Success:
                        return "MediaInfo library have been successfully updated.";
                    case Status.ed:
                        return "Mediainfo library update have been ed by the user.";
                    case Status.MissingFiles:
                        return "Unable to update ( missing 7z.dll / SevenZipSharp.dll).";
                    case Status.Error:
                        return "Mediainfo library update failed.";
                }
            }
            return "Generic error.";
        }

        public Status GetStatus()
        {
            return status.GetValueOrDefault(Status.Error);
        }

        private void PerformUpdate()
        {
            if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "7z.dll")) && File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SevenZipSharp.dll")))
            {
                CancelButton.IsEnabled = true;
                BackgroundWorker.RunWorkerAsync();
                status = new Status?();
            }
            else
            {
                Log.AppendText("Unable to update, some libraries are missing (7z.dll and/or SevenZipSharp.dll).");
                status = new Status?(Status.MissingFiles);
            }
        }

        private string GetLatestMediaInfoVersion()
        {
            XmlReader xmlReader = XmlReader.Create("https://mediaarea.net/rss/mediainfo_updates.xml");
            xmlReader.ReadToFollowing("title");
            xmlReader.ReadToFollowing("title");
            return xmlReader.ReadElementContentAsString().Replace("MediaInfo ", "");
        }

        private string GetLocalMediaInfoVersion()
        {
            if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MediaInfo.dll")))
            {
                FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MediaInfo.dll"));
                if (!string.IsNullOrEmpty(versionInfo.FileVersion))
                    return versionInfo.FileVersion;
            }
            return string.Empty;
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Dispatcher.Invoke((Action)(() => simpleStatus.Content = (object)"Step 1 of 3"));
            if (BackgroundWorker.CancellationPending)
            {
                e.Cancel = true;
            }
            else
            {
                BackgroundWorker.ReportProgress(5, "Verifying local library version...");
                string mediaInfoVersion1 = GetLocalMediaInfoVersion();
                if (!string.IsNullOrEmpty(mediaInfoVersion1))
                    BackgroundWorker.ReportProgress(45, "Found version: " + mediaInfoVersion1);
                else
                    BackgroundWorker.ReportProgress(45, "Failed");
                if (BackgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                }
                else
                {
                    BackgroundWorker.ReportProgress(50, "Verifying latest remote library version...");
                    string mediaInfoVersion2 = GetLatestMediaInfoVersion();
                    if (!string.IsNullOrEmpty(mediaInfoVersion2))
                        BackgroundWorker.ReportProgress(95, "Found version: " + mediaInfoVersion2);
                    else
                        BackgroundWorker.ReportProgress(95, "Failed");
                    BackgroundWorker.ReportProgress(100);
                    if (!mediaInfoVersion1.Contains(mediaInfoVersion2))
                    {
                        SevenZipBase.SetLibraryPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "7z.dll"));
                        string str1 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tmp");
                        string uriString = mediainfoBaseUrl + (Environment.Is64BitProcess ? mediaInfoVersion2 + "/MediaInfo_DLL_" + mediaInfoVersion2 + "_Windows_x64_WithoutInstaller.7z" : mediaInfoVersion2 + "/MediaInfo_DLL_" + mediaInfoVersion2 + "_Windows_i386_WithoutInstaller.7z");
                        string path2 = uriString.Substring(uriString.LastIndexOf("/") + 1, uriString.Length - uriString.LastIndexOf("/") - 1);
                        string str2 = "MediaInfo.dll";
                        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, str2);
                        string str3 = Path.Combine(str1, path2);
                        if (!Directory.Exists(str1))
                            Directory.CreateDirectory(str1);
                        if (BackgroundWorker.CancellationPending)
                        {
                            e.Cancel = true;
                        }
                        else
                        {
                            Dispatcher.Invoke((Action)(() => simpleStatus.Content = (object)"Step 2 of 3"));
                            BackgroundWorker.ReportProgress(1, "Downloading latest version of the library...");
                            using (WebClient webClient = new WebClient())
                            {
                                webClient.DownloadProgressChanged += Client_DownloadProgressChanged;
                                webClient.DownloadFileCompleted += Client_DownloadFileCompleted;
                                try
                                {
                                    webClient.DownloadFileAsync(new Uri(uriString), str3);
                                    if (!semaphore.WaitOne(TimeSpan.FromSeconds(10.0)))
                                    {
                                        e.Result = "Download failed due to timeout.";
                                        throw new TimeoutException();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    e.Result = "Download failed. Error message: " + ex.Message;
                                    throw ex;
                                }
                            }
                            if (BackgroundWorker.CancellationPending)
                            {
                                e.Cancel = true;
                            }
                            else
                            {
                                Dispatcher.Invoke((Action)(() => simpleStatus.Content = (object)"Step 3 of 3"));
                                using (SevenZipExtractor sevenZipExtractor = new SevenZipExtractor(str3))
                                {
                                    sevenZipExtractor.Extracting += Archive_Extracting;
                                    sevenZipExtractor.ExtractionFinished += Archive_ExtractionFinished;
                                    BackgroundWorker.ReportProgress(5, "Deleting old files...");
                                    if (File.Exists(path))
                                    {
                                        try
                                        {
                                            File.Delete(path);
                                            BackgroundWorker.ReportProgress(100, "Old file has been successfully removed");
                                        }
                                        catch (Exception ex)
                                        {
                                            e.Result = "Cannot remove old files. Error message: " + ex.Message;
                                            throw ex;
                                        }
                                    }
                                    FileStream fileStream = File.Create(path);
                                    BackgroundWorker.ReportProgress(5, "Start extraction of the downloaded archive...");
                                    foreach (string archiveFileName in sevenZipExtractor.ArchiveFileNames)
                                    {
                                        if (archiveFileName.Contains(str2))
                                        {
                                            str2 = archiveFileName;
                                            break;
                                        }
                                    }
                                    try
                                    {
                                        sevenZipExtractor.ExtractFile(str2, fileStream);
                                        if (!semaphore.WaitOne(TimeSpan.FromSeconds(10.0)))
                                        {
                                            e.Result = "Extraction failed due to timeout.";
                                            throw new TimeoutException();
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        e.Result = "Extraction failed. Error message: " + ex.Message;
                                        throw ex;
                                    }
                                    finally
                                    {
                                        fileStream.Dispose();
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Dispatcher.Invoke((Action)(() => simpleStatus.Content = (object)"Step 3 of 3"));
                        BackgroundWorker.ReportProgress(100, "MediaInfo is already at the latest version. No need to update.");
                        e.Result = Status.NotNeeded;
                    }
                }
            }
        }

        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            int num = e.ProgressPercentage;
            string userState = (string)e.UserState;
            if (num > 0)
            {
                if (num > 100)
                    num = 100;
                ProgressUpdate.Value = num;
                ProgressLabel.Content = num.ToString() + "%";
            }
            if (!string.IsNullOrEmpty(userState))
            {
                Log.AppendText(userState + Environment.NewLine);
                Log.ScrollToEnd();
            }
            InvalidateVisual();
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            CancelButton.IsEnabled = false;
            ProgressUpdate.Value = 100.0;
            if (e.Cancelled || )
            {
                Log.AppendText("Operation ed by the user!" + Environment.NewLine);
                ProgressLabel.Content = "ed";
                status = new Status?(Status.ed);
            }
            else if (e.Error != null)
            {
                Log.AppendText("Exception generated! " + e.Result + Environment.NewLine);
                ProgressLabel.Content = "Error";
                status = new Status?(Status.Error);
            }
            else if (e.Result != null && e.Result.Equals(Status.NotNeeded))
            {
                Log.AppendText("Analysis completed: no need to update." + Environment.NewLine);
                status = new Status?(Status.NotNeeded);
                Close();
            }
            else
            {
                Log.AppendText("All operations successfully completed!" + Environment.NewLine);
                status = new Status?(Status.Success);
                Close();
            }
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if ()
            {
                ((WebClient)sender).CancelAsync();
            }
            else
            {
                int progress = e.ProgressPercentage;
                ((float)(e.TotalBytesToReceive / 1024.0 / 1024.0)).ToString("0.00");
                if (progress <= 0)
                    return;
                if (progress > 100)
                    progress = 100;
                Dispatcher.Invoke((Action)(() =>
                {
                    ProgressUpdate.Value = progress;
                    ProgressLabel.Content = progress.ToString() + "%";
                }));
            }
        }

        private void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (!e.Cancelled && e.Error == null)
                Dispatcher.Invoke((Action)(() =>
                {
                    Log.AppendText("Download completed!" + Environment.NewLine);
                    Log.ScrollToEnd();
                    ProgressUpdate.Value = 100.0;
                    ProgressLabel.Content = "100%";
                }));
            semaphore.Release();
        }

        private void Archive_Extracting(object sender, ProgressEventArgs e)
        {
            if ()
                e.Cancel = true;
            int progress = e.PercentDone;
            if (progress <= 0)
                return;
            if (progress > 100)
                progress = 100;
            Dispatcher.Invoke((Action)(() => ProgressUpdate.Value = (double)progress));
        }

        private void Archive_ExtractionFinished(object sender, EventArgs e)
        {
            Dispatcher.Invoke((Action)(() =>
            {
                Log.AppendText("Extraction completed!" + Environment.NewLine);
                Log.ScrollToEnd();
                ProgressUpdate.Value = 100.0;
                ProgressLabel.Content = "100%";
            }));
            semaphore.Release();
        }



        public enum Status
        {
            NotNeeded,
            Success,
            ed,
            MissingFiles,
            Error,
        }
    }
}
