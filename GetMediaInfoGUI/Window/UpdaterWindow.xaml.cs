using GetMediaInfoGUI.Util;
using System;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace GetMediaInfoGUI
{
    /// <summary>
    /// Interaction logic for UpdateComponent.xaml
    /// </summary>
    public partial class UpdaterWindow : System.Windows.Window
    {
        #region Global Variables
        /// <summary>
        /// Enumeration of the Current status of the operation.
        /// </summary>
        public enum Status
        {
            NotNeeded,
            Success,
            ed,
            Error
        };

        /// <summary>
        /// The _status Enum Status variable.
        /// </summary>
        private Status? status;

        /// <summary>
        /// The cancellation token source.
        /// </summary>
        private CancellationTokenSource tokenSource = new CancellationTokenSource();
        /// <summary>
        /// The task scheduler for UI tasks.
        /// </summary>
        private TaskScheduler uiScheduler;
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdaterWindow"/> class.
        /// </summary>
        public UpdaterWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region Interface Handlers
        /// <summary>
        /// Handles the Loaded event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            PerformUpdate();
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

            try
            {
                tokenSource.Dispose();
            }
            catch { }
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
            Height -= 150;
            ResizeMode = ResizeMode.NoResize;
            Expander.Header = "More details";
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
            Height += 150;
            ResizeMode = ResizeMode.NoResize;
            Expander.Header = "Less details";
        }

        /// <summary>
        /// Handles the Click event of the CancelButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            tokenSource.Cancel();
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
        /// Gets the latest MediaInfo version.
        /// </summary>
        /// <returns>String as the latest available MediaInfo version</returns>
        private string GetLatestMediaInfoVersion()
        {
            XmlReader XmlReader = XmlReader.Create(Constants.MediainfoRssUrl);
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
            return FileUtils.GetAssemblyVersion(FolderUtils.GetAbsolutePathFromBaseDir("MediaInfo.dll"));
        }
        #endregion

        #region Operations
        /// <summary>
        /// Performs the components update.
        /// </summary>
        private void PerformUpdate()
        {
            Action checkCancellation = () =>
            {
                tokenSource.Token.ThrowIfCancellationRequested();
            };

            Task.Run(async () =>
            {
                status = null;

                UpdateGlobalStatus("Step 1 of 3");

                UpdateProgress(5, "Verifying local library version...");
                string currMediaInfo = GetLocalMediaInfoVersion();
                if (!string.IsNullOrEmpty(currMediaInfo))
                {
                    UpdateProgress(45, $"Found version: {currMediaInfo}");
                }
                else
                {
                    UpdateProgress(45, "The local library is missing.");
                }

                UpdateProgress(50, "Verifying latest remote library version...");
                string newVerMediaInfo = GetLatestMediaInfoVersion();
                if (!string.IsNullOrEmpty(newVerMediaInfo))
                {
                    UpdateProgress(95, $"Found version: {newVerMediaInfo}");
                }
                else
                {
                    UpdateProgress(95, "Unable to get latest version.");
                }

                UpdateProgress(100);

                checkCancellation();

                if (!currMediaInfo.Contains(newVerMediaInfo))
                {
                    var tempDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tmp");

                    var sourceFileName = $"MediaInfo_DLL_{newVerMediaInfo}_Windows_{(Environment.Is64BitProcess ? "x64" : "i386")}_WithoutInstaller.zip";
                    var downloadUrl = $"{Constants.MediainfoBaseDownloadUrl}{newVerMediaInfo}/{sourceFileName}";
                    var downloadPath = Path.Combine(tempDir, sourceFileName);

                    var fileToExtract = "MediaInfo.dll";
                    var extractPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileToExtract);

                    if (!Directory.Exists(tempDir))
                    {
                        Directory.CreateDirectory(tempDir);
                    }

                    //removing previous files
                    if (File.Exists(extractPath))
                    {
                        UpdateProgress(-1, "Deleting previous file...");

                        try
                        {
                            File.Delete(extractPath);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"Unable to remove previous file: { ex.Message}", ex);
                        }
                    }

                    checkCancellation();

                    //downloading
                    UpdateGlobalStatus("Step 2 of 3");
                    UpdateProgress(-1, "Downloading...");
                    await DownloadFile(downloadUrl, downloadPath);
                    UpdateProgress(100, "Download completed.");

                    checkCancellation();

                    //extracting
                    UpdateGlobalStatus("Step 3 of 3");
                    UpdateProgress(-1, "Extracting...");
                    await ExtractArchiveFiles(downloadPath, AppDomain.CurrentDomain.BaseDirectory, fileToExtract);
                    UpdateProgress(100, "Extraction completed.");

                    checkCancellation();

                    UpdateCompleted(Status.Success);
                }
                else
                {
                    UpdateGlobalStatus("Step 3 of 3");

                    UpdateProgress(100, "MediaInfo is already at the latest version. No need to update.");
                    UpdateCompleted(Status.NotNeeded);
                }
            }, tokenSource.Token)
            .ContinueWith((task) =>
            {
                if (task.IsFaulted)
                {
                    UpdateCompleted(Status.Error, task.Exception);
                }
                else if (task.IsCanceled)
                {
                    UpdateCompleted(Status.ed);
                }
            }, TaskContinuationOptions.NotOnRanToCompletion);
        }

        /// <summary>
        /// Downloads a file.
        /// </summary>
        /// <param name="url">The url</param>
        /// <param name="outputPath">The output path</param>
        /// <returns>The task that represents the asynchronous operations</returns>
        private async Task DownloadFile(string url, string outputPath)
        {
            using (var outputStream = new FileStream(outputPath, FileMode.Create))
            using (var httpClient = new HttpClient())
            using (var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, url), tokenSource.Token))
            {
                 var inputStream = await response.Content.ReadAsStreamAsync();
                 await inputStream.CopyToAsync(outputStream);
            }
        }

        /// <summary>
        /// Extracts specified files from a target zip archive.
        /// </summary>
        /// <param name="archivePath">The archive path</param>
        /// <param name="destionationFolder">The destination folder</param>
        /// <param name="filesToExtract">The files to be extracted</param>
        /// <returns>The task that represents the asynchronous operations</returns>
        private Task ExtractArchiveFiles(string archivePath, string destionationFolder, params string[] filesToExtract)
        {
            return Task.Run(() => {
                using (var srcArchive = ZipFile.OpenRead(archivePath))
                {
                    srcArchive.Entries
                        .Where(entry => filesToExtract.Contains(entry.FullName))
                        .ToList()
                        .ForEach((entry) => entry.ExtractToFile(Path.Combine(destionationFolder, entry.FullName), true));
                }
            }, tokenSource.Token);  
        }

        /// <summary>
        /// Updates the global status.
        /// </summary>
        /// <param name="status">The new status</param>
        private void UpdateGlobalStatus(string status)
        {
            Task.Factory.StartNew(() => {
                simpleStatus.Content = status;
            }, tokenSource.Token, TaskCreationOptions.None, uiScheduler);
        }

        /// <summary>
        /// Updates the progress.
        /// </summary>
        /// <param name="progress">The new progress</param>
        /// <param name="status">The new status</param>
        private void UpdateProgress(int progress, string status = "")
        {
            Task.Factory.StartNew(() =>
            {
                if (progress > 0)
                {
                    ProgressUpdate.IsIndeterminate = false;

                    if (progress > 100)
                    {
                        progress = 100;
                    }

                    ProgressUpdate.Value = progress;
                    ProgressLabel.Content = $"{progress}%";
                }
                else
                {
                    ProgressUpdate.IsIndeterminate = true;
                    ProgressLabel.Content = "";
                }

                if (!string.IsNullOrEmpty(status))
                {
                    Log.AppendText($"{status}{Environment.NewLine}");
                    Log.ScrollToEnd();
                }
            }, tokenSource.Token, TaskCreationOptions.None, uiScheduler);
        }

        /// <summary>
        /// Handles the completion of the operations
        /// </summary>
        /// <param name="status">The status</param>
        /// <param name="ex">The exception</param>
        private void UpdateCompleted(Status status, Exception ex = null)
        {
            this.status = status;

            Task.Factory.StartNew(() =>
            {
                CancelButton.IsEnabled = false;
                UpdateProgress(100);

                switch (status)
                {
                    case Status.NotNeeded:
                        Log.AppendText($"Analysis completed: no need to update." + Environment.NewLine);
                        Close();
                        break;
                    case Status.Success:
                        Log.AppendText($"All operations successfully completed!" + Environment.NewLine);
                        Close();
                        break;
                    case Status.ed:
                        Log.AppendText($"Operation ed by the user!{Environment.NewLine}");
                        ProgressLabel.Content = "ed";
                        break;
                    case Status.Error:
                        Log.AppendText($"Error occurred: {ex.Message}{Environment.NewLine}");
                        ProgressLabel.Content = "Error";
                        break;
                }
            }, tokenSource.Token, TaskCreationOptions.None, uiScheduler);
        }
        #endregion
    }
}