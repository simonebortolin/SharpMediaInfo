using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GetMediaInfoGUI
{
    public partial class Settings
    {
        private bool initialization;

        public Settings()
        {
            initialization = true;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, EventArgs e)
        {
            StatusGUI.Text = System.Windows.Forms.Application.ProductVersion;
            StatusGUI.Foreground = Brushes.Green;
            VerifyMediaInfoVersion();
            VerifyGetMediaInfoVersion();
            AutoSaveReport.IsChecked = new bool?(GlobalSettings.AutoSaveReport);
            DisableSimple.IsChecked = new bool?(GlobalSettings.DisableSimpleReport);
            DisableComplete.IsChecked = new bool?(GlobalSettings.DisableCompleteReport);
            DisableTree.IsChecked = new bool?(GlobalSettings.DisableTreeReport);
            EnableMilliseconds.IsChecked = new bool?(GlobalSettings.ShowMilliseconds);
            RemoveSpace.IsChecked = new bool?(GlobalSettings.RemoveJustification);
            ShowCover.IsChecked = new bool?(GlobalSettings.ShowCover);
            ComboBoxFontSize.SelectedValue = GlobalSettings.ReportFontSize;
            ComboBoxFontFamily.SelectedIndex = ComboBoxFontFamily.Items.Cast<FontFamily>().FindIndex<FontFamily>(s => s.Equals(GlobalSettings.ReportFontFamily));
            BoldControl.SetChecked(GlobalSettings.Bold);
            ItalicControl.SetChecked(GlobalSettings.Italic);
            HistorySize.Value = GlobalSettings.History.MaxSize;
            initialization = false;
        }

        private void AddContextMenu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RegistryKey subKey1 = Registry.CurrentUser.OpenSubKey("Software", true).OpenSubKey("Classes", true).CreateSubKey("*").CreateSubKey("shell").CreateSubKey("open_with_getmediainfogui");
                subKey1.SetValue("", "GetMediaInfo");
                subKey1.SetValue("Icon", "\"" + Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "icon.ico") + "\"");
                RegistryKey subKey2 = subKey1.CreateSubKey("command");
                subKey2.SetValue("", "\"" + Process.GetCurrentProcess().MainModule.FileName + "\" \"%1\"");
                subKey2.Close();
                AssociationStatus.Content = "Context menu entry creation succeeded.";
            }
            catch (Exception ex)
            {
                AssociationStatus.Content = "Context menu entry creation failed: " + ex.Message;
            }
        }

        private void RemoveContextMenu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RegistryKey subKey = Registry.CurrentUser.OpenSubKey("Software", true).OpenSubKey("Classes", true).OpenSubKey("*", true).CreateSubKey("shell");
                subKey.DeleteSubKeyTree("open_with_getmediainfogui");
                subKey.Close();
                AssociationStatus.Content = "Context menu entry successfully removed.";
            }
            catch (ArgumentException ex)
            {
                AssociationStatus.Content = "Context menu not present.";
            }
            catch (Exception ex)
            {
                AssociationStatus.Content = "Context menu not removed: " + ex.Message;
            }
        }

        private void DisableSimple_CheckedUnchecked(object sender, RoutedEventArgs e)
        {
            if (initialization)
                return;
            GlobalSettings.DisableSimpleReport = DisableSimple.IsChecked.GetValueOrDefault();
        }

        private void DisableComplete_CheckedUnchecked(object sender, RoutedEventArgs e)
        {
            if (initialization)
                return;
            GlobalSettings.DisableCompleteReport = DisableComplete.IsChecked.GetValueOrDefault();
        }

        private void DisableTree_CheckedUnchecked(object sender, RoutedEventArgs e)
        {
            if (initialization)
                return;
            GlobalSettings.DisableTreeReport = DisableTree.IsChecked.GetValueOrDefault();
        }

        private void EnableMilliseconds_CheckedUnchecked(object sender, RoutedEventArgs e)
        {
            if (initialization)
                return;
            GlobalSettings.ShowMilliseconds = EnableMilliseconds.IsChecked.GetValueOrDefault();
        }

        private void AutoSaveReport_CheckedUnchecked(object sender, RoutedEventArgs e)
        {
            if (initialization)
                return;
            GlobalSettings.AutoSaveReport = AutoSaveReport.IsChecked.GetValueOrDefault();
        }

        private void RemoveSpace_CheckedUnchecked(object sender, RoutedEventArgs e)
        {
            if (initialization)
                return;
            GlobalSettings.RemoveJustification = RemoveSpace.IsChecked.GetValueOrDefault();
        }

        private void ShowCover_CheckedUnchecked(object sender, RoutedEventArgs e)
        {
            if (initialization)
                return;
            GlobalSettings.ShowCover = ShowCover.IsChecked.GetValueOrDefault();
        }

        private void BoldControl_Click(object sender, RoutedEventArgs e)
        {
            if (initialization)
                return;
            GlobalSettings.Bold = !GlobalSettings.Bold;
            BoldControl.SetChecked(GlobalSettings.Bold);
        }

        private void ItalicControl_Click(object sender, RoutedEventArgs e)
        {
            if (initialization)
                return;
            GlobalSettings.Italic = !GlobalSettings.Italic;
            ItalicControl.SetChecked(GlobalSettings.Italic);
        }

        private void ComboBoxFont_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (initialization)
                return;
            GlobalSettings.ReportFontFamily = (FontFamily)(ComboBoxFontFamily?.SelectedValue ?? GlobalSettings.ReportFontFamily);
        }

        private void ComboBoxSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (initialization)
                return;
            GlobalSettings.ReportFontSize = new double?((double)(ComboBoxFontSize?.SelectedValue ?? GlobalSettings.ReportFontSize));
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            StartUpdater();
        }

        private void HistorySize_ValueChanged(object sender, EventArgs e)
        {
            if (initialization)
                return;
            GlobalSettings.HistorySize = (int)HistorySize.Value;
            FixedSizedQueue<string> history = GlobalSettings.History;
            GlobalSettings.History.Clear();
            GlobalSettings.History.MaxSize = GlobalSettings.HistorySize;
            for (int index = 0; index < GlobalSettings.HistorySize && history.Count > 0; ++index)
                GlobalSettings.History.Enqueue(history.Dequeue());
        }

        private void ClearRecentFileList_Click(object sender, RoutedEventArgs e)
        {
            GlobalSettings.History.Clear();
        }

        private void VerifyMediaInfoVersion()
        {
            if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MediaInfo.dll")))
            {
                FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MediaInfo.dll"));
                if (!string.IsNullOrWhiteSpace(versionInfo.FileVersion))
                {
                    StatusMediaInfo.Text = versionInfo.FileVersion;
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

        private void VerifyGetMediaInfoVersion()
        {
            if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GetMediaInfo.dll")))
            {
                FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GetMediaInfo.dll"));
                if (!string.IsNullOrWhiteSpace(versionInfo.FileVersion))
                {
                    StatusGetMediaInfo.Text = versionInfo.FileVersion;
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

        private void StartUpdater()
        {
            Updater updater = new Updater();
            updater.ShowDialog();
            UpdaterProgress.Content = updater.GetMessageStatus();
            VerifyMediaInfoVersion();
        }
    }
}
