using GetMediaInfoGUI.Configuration;
using GetMediaInfoGUI.Extensions;
using GetMediaInfoGUI.History;
using GetMediaInfoGUI.Utils;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;

namespace GetMediaInfoGUI
{
    /// <summary>
    /// Iteration logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        #region Global Variables
        /// <summary>
        /// Determine if the windows is initializating components.
        /// </summary>
        private bool initialization;
        #endregion

        #region Constructor 

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigHelper.GetConfig()"/> class.
        /// </summary>
        public SettingsWindow()
        {
            initialization = true;
            InitializeComponent();         
        }

        #endregion

        #region Interface Handlers

        /// <summary>
        /// Handles for Loaded event of the Window. Inizializate the window, check dependencies and their versions and restore user config.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Window_Loaded(object sender, EventArgs e)
        {
            StatusFrontend.Text = System.Windows.Forms.Application.ProductVersion;
            StatusFrontend.Foreground = Brushes.Green;
            
            //Check versions
            VerifyMediaInfoVersion();
            VerifyGetMediaInfoVersion();

            //Global
            AutoSaveReport.IsChecked = ConfigHelper.GetConfig().AutoSaveReport;
            DisableSimple.IsChecked = ConfigHelper.GetConfig().DisableSimpleReport;
            DisableComplete.IsChecked = ConfigHelper.GetConfig().DisableCompleteReport;
            DisableTree.IsChecked = ConfigHelper.GetConfig().DisableTreeReport;
            EnableMilliseconds.IsChecked = ConfigHelper.GetConfig().ShowMilliseconds;
            RemoveSpace.IsChecked = ConfigHelper.GetConfig().RemoveJustification;
            ShowCover.IsChecked = ConfigHelper.GetConfig().ShowCover;

            //Appearence
            ComboBoxFontSize.SelectedValue = ConfigHelper.GetConfig().ReportFontSize;
            ComboBoxFontFamily.SelectedIndex = ComboBoxFontFamily.Items.Cast<FontFamily>().FindIndex(i => i.Source.Equals(ConfigHelper.GetConfig().ReportFontFamily));
            BoldControl.SetChecked(ConfigHelper.GetConfig().Bold);    
            ItalicControl.SetChecked(ConfigHelper.GetConfig().Italic);

            //History
            HistorySize.Value = HistoryHelper.GetHistory().MaxSize;

            initialization = false;
        }

        /// <summary>
        /// Handles the Click event of the AddContextMenu control: adds context menu GetMediaInfo entries.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void AddContextMenu_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.RegistryKey RegistryKey;

            try
            {
                // for all files
                RegistryKey = Microsoft.Win32.Registry.CurrentUser
                    .OpenSubKey("Software", true)
                    .OpenSubKey("Classes", true);
                RegistryKey = RegistryKey.CreateSubKey("*");
                RegistryKey = RegistryKey.CreateSubKey("shell");
                RegistryKey = RegistryKey.CreateSubKey("open_with_getmediainfogui");
                RegistryKey.SetValue("", "GetMediaInfo");
                RegistryKey.SetValue("Icon", "\"" + Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "icon.ico") + "\"");
                RegistryKey = RegistryKey.CreateSubKey("command");
                RegistryKey.SetValue("", "\"" + Process.GetCurrentProcess().MainModule.FileName + "\" \"%1\"");
                RegistryKey.Close();
                // end 
                AssociationStatus.Text = "Context menu entry creation succeeded.";
            }
            catch (Exception ex)
            {
                AssociationStatus.Text = "Context menu entry creation failed: " + ex.Message;
            }
        }

        /// <summary>
        /// Handles the Click event of the RemoveContextMenu control: removes context menu GetMediaInfo entries.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void RemoveContextMenu_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.RegistryKey RegistryKey;
            try
            {
                RegistryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software", true)
                    .OpenSubKey("Classes", true)
                    .OpenSubKey("*", true)
                    .CreateSubKey("shell");
                RegistryKey.DeleteSubKeyTree("open_with_getmediainfogui");
                RegistryKey.Close();

                AssociationStatus.Text = "Context menu entry successfully removed.";
            }
            catch (ArgumentException)
            {
                AssociationStatus.Text = "Context menu not present.";
            }
            catch (Exception ex)
            {
                AssociationStatus.Text = "Context menu not removed: " + ex.Message;
            }
        }

        /// <summary>
        /// Handles the Checked event of the DisableSimple control: determine whether simple report must be shown or not.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void DisableSimple_CheckedUnchecked(object sender, RoutedEventArgs e)
        {
            if (!initialization)
                ConfigHelper.GetConfig().DisableSimpleReport = DisableSimple.IsChecked.GetValueOrDefault();
        }

        /// <summary>
        /// Handles the Checked event of the DisableComplete control: determines whether complete report must be shown or not.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void DisableComplete_CheckedUnchecked(object sender, RoutedEventArgs e)
        {
            if (!initialization)
                ConfigHelper.GetConfig().DisableCompleteReport = DisableComplete.IsChecked.GetValueOrDefault();
        }

        /// <summary>
        /// Handles the Unchecked event of the DisableSimpleInfo control: determines whether tree report must be shown or not.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void DisableTree_CheckedUnchecked(object sender, RoutedEventArgs e)
        {
            if (!initialization)
                ConfigHelper.GetConfig().DisableTreeReport = DisableTree.IsChecked.GetValueOrDefault();
        }

        /// <summary>
        /// Handles the Change event of the EnableMilliseconds control: determines whether simple report's durations are shown with milliseconds or not.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void EnableMilliseconds_CheckedUnchecked(object sender, RoutedEventArgs e)
        {
            if (!initialization)
                ConfigHelper.GetConfig().ShowMilliseconds = EnableMilliseconds.IsChecked.GetValueOrDefault();
        }

        /// <summary>
        /// Handles the Checked and Unchecked event of the AutoSaveReport control: determines whether report must be automatically saved to disk or not.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void AutoSaveReport_CheckedUnchecked(object sender, RoutedEventArgs e)
        {
            if (!initialization)
                ConfigHelper.GetConfig().AutoSaveReport = AutoSaveReport.IsChecked.GetValueOrDefault();
        }

        /// <summary>
        /// Handles the Checked and Unchecked event of the RemoveSpace control: determines whether reports must have tabulation or not.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void RemoveSpace_CheckedUnchecked(object sender, RoutedEventArgs e)
        {
            if (!initialization)
                ConfigHelper.GetConfig().RemoveJustification = RemoveSpace.IsChecked.GetValueOrDefault();
        }

        /// <summary>
        /// Handles the Checked and Unchecked event of the ShowCover control: determines whether to show cover or not.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void ShowCover_CheckedUnchecked(object sender, RoutedEventArgs e)
        {
            if (!initialization)
                ConfigHelper.GetConfig().ShowCover = ShowCover.IsChecked.GetValueOrDefault();
        }

        /// <summary>
        /// Handles the Click event of the BoldControl control: change the font bolding.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void BoldControl_Click(object sender, RoutedEventArgs e)
        {
            if (!initialization)
            {
                ConfigHelper.GetConfig().Bold = !ConfigHelper.GetConfig().Bold;
                BoldControl.SetChecked(ConfigHelper.GetConfig().Bold);
            }
        }

        /// <summary>
        /// Handles the Click event of the ItalicControl control: change the font style.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void ItalicControl_Click(object sender, RoutedEventArgs e)
        {
            if (!initialization)
            {
                ConfigHelper.GetConfig().Italic = !ConfigHelper.GetConfig().Italic;
                ItalicControl.SetChecked(ConfigHelper.GetConfig().Italic);
            }
        }

        /// <summary>
        /// Handles the SelectionChanged event of the ComboBoxFont control: change the font family.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void ComboBoxFont_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!initialization)
            {
                ConfigHelper.GetConfig().ReportFontFamily = ComboBoxFontFamily?.SelectedValue as string ?? ConfigHelper.GetConfig().ReportFontFamily;
            }               
        }

        /// <summary>
        /// Handles the SelectionChanged event of the ComboBoxSize control: changes the font size.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void ComboBoxSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!initialization)
            {
                ConfigHelper.GetConfig().ReportFontSize = (double)(ComboBoxFontSize?.SelectedValue ?? ConfigHelper.GetConfig().ReportFontSize);
            }            
        }

        /// <summary>
        /// Handles the Click event of the UpdateButton control: starts components Updater.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            StartUpdater();
        }

        /// <summary>
        /// Handles the ValueChanged event of the RecentFileCount control: changes the history max size.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void HistorySize_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (!initialization)
            {
                var historySize = (int)HistorySize.Value;
                ConfigHelper.GetConfig().HistorySize = historySize;
                HistoryHelper.GetHistory().ChangeMaximumSize(historySize);
            }          
        }

        /// <summary>
        /// Handles the Click event of the ClearRecentFileList control: clears the history.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void ClearRecentFileList_Click(object sender, RoutedEventArgs e)
        {
            HistoryHelper.GetHistory().Clear();
        }
        #endregion

        #region Interface Helpers

        /// <summary>
        /// Verifies the MediaInfo library version. 
        /// </summary>
        private void VerifyMediaInfoVersion()
        {
            if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MediaInfo.dll")))
            {
                FileVersionInfo FileVersionInfo = FileVersionInfo.GetVersionInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MediaInfo.dll"));
                if (!string.IsNullOrWhiteSpace(FileVersionInfo.FileVersion))
                {
                    StatusMediaInfo.Text = FileVersionInfo.FileVersion;
                    StatusMediaInfo.Foreground = Brushes.Green;
                }
                else
                {
                    StatusMediaInfo.Text = "Unable to find version";
                    StatusMediaInfo.Foreground = Brushes.DarkOrange;
                }
            }
            else
            {
                StatusMediaInfo.Text = "ERROR";
                StatusMediaInfo.Foreground = Brushes.Red;
            }
        }

        /// <summary>
        /// Verifies the GetMediaInfo library version. 
        /// </summary>
        private void VerifyGetMediaInfoVersion()
        {
            if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GetMediaInfo.dll")))
            {
                FileVersionInfo FileVersionInfo = FileVersionInfo.GetVersionInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GetMediaInfo.dll"));
                if (!string.IsNullOrWhiteSpace(FileVersionInfo.FileVersion))
                {
                    StatusGetMediaInfo.Text = FileVersionInfo.FileVersion;
                    StatusGetMediaInfo.Foreground = Brushes.Green;
                }
                else
                {
                    StatusGetMediaInfo.Text = "Unable to find version";
                    StatusGetMediaInfo.Foreground = Brushes.YellowGreen;
                }
            }
            else
            {
                StatusGetMediaInfo.Text = "ERROR";
                StatusGetMediaInfo.Foreground = Brushes.Red;
            }
        }

        /// <summary>
        /// Starts the Components Updater.
        /// </summary>
        private void StartUpdater()
        {
            try
            {
                WebRequest request = WebRequest.Create(Constants.MediainfoRssUrl);
                request.Timeout = 5000;

                using (var response = request.GetResponse())
                using (var reader = XmlReader.Create(response.GetResponseStream()))
                {
                    reader.ReadToFollowing("title");
                    reader.ReadToFollowing("title");
                    string latest = reader.ReadElementContentAsString();
                    latest = latest.Replace("MediaInfo ", "");

                    if (!StatusMediaInfo.Text.StartsWith(latest))
                    {
                        UpdaterProgress.Text = "A new Mediainfo library version is available (" + latest + ").";
                    } else {
                        UpdaterProgress.Text = "The Mediainfo library is already at its latest version.";
                    }  
                }
            } catch
            {
                MessageBox.Show("An error occurred while trying to retrieve the latest version of Mediainfo.", "Error");
            }
                  
        }
        #endregion
    }
}
