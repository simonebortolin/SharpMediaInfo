using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Linq;
using DataObject = System.Windows.DataObject;

namespace GetMediaInfoGUI
{
    public partial class MainWindow : Window
    {
        private BackgroundWorker BackgroundWorker;
        private bool _killThisInstance;

        public MainWindow()
        {
            try
            {
                using (CrossProcessLockFactory.CreateCrossProcessLock())
                {
                    if (CheckForOtherInstances(Environment.GetCommandLineArgs()))
                        return;
                    InitializeComponent();
                    System.Windows.Forms.Application.EnableVisualStyles();
                    ClearAll();
                    BackgroundWorker = new BackgroundWorker();
                    BackgroundWorker.DoWork += BackgroundWorker_DoWork;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("GetMediaInfo constructor", ex.Message + Environment.NewLine + ex.InnerException + Environment.NewLine + ex.StackTrace);
            }
        }

        private void Window_Loaded(object sender, EventArgs e)
        {
            try
            {
                VerifyMediaInfo();
                VerifyGetMediaInfo();
                ProcessArgs(Environment.GetCommandLineArgs());
                RestoreConfig();
                RestoreHistory();
                StatusLab.Content = "Ready.";
                StatusLab.InvalidateVisual();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("GetMediaInfo loading", ex.Message + Environment.NewLine + ex.InnerException + Environment.NewLine + ex.StackTrace);
            }
        }

        private void Windows_Closing(object sender, EventArgs e)
        {
            if (_killThisInstance)
                return;
            SaveConfig();
            SaveHistory();
        }

        private void MenuItemOpenFiles_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog1 =
                new Microsoft.Win32.OpenFileDialog
                {
                    Filter = "Media Files|*.*",
                    Title = "Select a files to analyze",
                    Multiselect = true
                };
            Microsoft.Win32.OpenFileDialog openFileDialog2 = openFileDialog1;
            if (!openFileDialog2.ShowDialog().GetValueOrDefault())
                return;
            AddTabItems(openFileDialog2.FileNames);
        }

        private void Windows_Drop(object sender, System.Windows.DragEventArgs e)
        {
            if (!((DataObject) e.Data).ContainsFileDropList())
                return;
            AddTabItems((string[])e.Data.GetData(System.Windows.DataFormats.FileDrop, false));
        }

        private void Windows_DragEnter(object sender, System.Windows.DragEventArgs e)
        {
            e.Effects = System.Windows.DragDropEffects.Copy;
        }

        private void MenuItemUpdater_Click(object sender, RoutedEventArgs e)
        {
            StartUpdater();
        }

        private void CopyAll_Click(object sender, RoutedEventArgs e)
        {
            FileTab selectedItem = FileTabControl.SelectedItem as FileTab;
            if (selectedItem.SelectedTab.Header.Equals("Standard"))
            {
                System.Windows.Clipboard.SetText(selectedItem.StandardReport);
            }
            else
            {
                if (!selectedItem.SelectedTab.Header.Equals("Complete"))
                    return;
                System.Windows.Clipboard.SetText(selectedItem.CompleteReport);
            }
        }

        private void MenuItemSaveCover_Click(object sender, RoutedEventArgs e)
        {
            if (FileTabControl.SelectedIndex == -1)
                return;
            FileTab fileTab = FileTabControl.Items[FileTabControl.SelectedIndex] as FileTab;
            bool? nullable = fileTab?.MediaInfo?.General[0]?.Cover;
            if (!nullable.GetValueOrDefault())
                return;
            Microsoft.Win32.SaveFileDialog saveFileDialog1 =
                new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "JPEG Image|*.jpeg",
                    Title = "Select the output path for the cover file",
                    DefaultExt = ".jpeg",
                    FileName = Path.GetFileNameWithoutExtension(fileTab.MediaInfo.General[0].FileName) + ".jpeg",
                    InitialDirectory = fileTab.MediaInfo.General[0].FolderName
                };
            Microsoft.Win32.SaveFileDialog saveFileDialog2 = saveFileDialog1;
            nullable = saveFileDialog2.ShowDialog();
            if (!nullable.GetValueOrDefault())
                return;
            JpegBitmapEncoder jpegBitmapEncoder = new JpegBitmapEncoder();
            jpegBitmapEncoder.Frames.Add(BitmapFrame.Create(fileTab.GetCoverData()));
            using (FileStream fileStream = new FileStream(saveFileDialog2.FileName, FileMode.Create))
                jpegBitmapEncoder.Save(fileStream);
        }

        private void TextBoxReport_PreviewDragOver(object sender, System.Windows.DragEventArgs e)
        {
            e.Handled = true;
        }

        private void TabItem_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            TabItem source = e.Source as TabItem;
            if (source == null || Mouse.PrimaryDevice.LeftButton != MouseButtonState.Pressed)
                return;
            int num = (int)DragDrop.DoDragDrop(source, source, System.Windows.DragDropEffects.All);
        }

        private void TabItem_Drop(object sender, System.Windows.DragEventArgs e)
        {
            TabItem data = e.Data.GetData(typeof(TabItem)) as TabItem;
            TabItem source;
            if ((source = e.Source as TabItem) == null || data == null)
                return;
            System.Windows.Controls.TabControl parent = source.Parent as System.Windows.Controls.TabControl;
            if (source.Equals(data) || parent == null)
                return;
            int insertIndex1 = parent.Items.IndexOf(data);
            int insertIndex2 = parent.Items.IndexOf(source);
            parent.Items.Remove(data);
            parent.Items.Insert(insertIndex2, data);
            parent.Items.Remove(source);
            parent.Items.Insert(insertIndex1, source);
        }

        private void MenuItemSaveStandardReport_Click(object sender, RoutedEventArgs e)
        {
            if (FileTabControl.SelectedIndex == -1)
                return;
            FileTab fileTab = FileTabControl.Items[FileTabControl.SelectedIndex] as FileTab;
            if (!fileTab.IsReady)
                return;
            Microsoft.Win32.SaveFileDialog saveFileDialog1 = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog1.Filter = "Text Document|*.txt";
            saveFileDialog1.Title = "Choose where to save the report";
            saveFileDialog1.DefaultExt = ".txt";
            saveFileDialog1.FileName = Path.GetFileNameWithoutExtension(fileTab.MediaInfo.General[0].FileName) + ".txt";
            saveFileDialog1.InitialDirectory = fileTab.MediaInfo.General[0].FolderName;
            Microsoft.Win32.SaveFileDialog saveFileDialog2 = saveFileDialog1;
            if (!saveFileDialog2.ShowDialog().GetValueOrDefault())
                return;
            new StreamWriter(saveFileDialog2.FileName).WriteLine(fileTab.MediaInfo.InfoStandard ?? "Unable to save report.");
        }

        private void MenuItemSaveCompleteReport_Click(object sender, RoutedEventArgs e)
        {
            if (FileTabControl.SelectedIndex == -1)
                return;
            FileTab fileTab = FileTabControl.Items[FileTabControl.SelectedIndex] as FileTab;
            if (!fileTab.IsReady || fileTab.MediaInfo.InfoComplete.Length <= 0)
                return;
            Microsoft.Win32.SaveFileDialog saveFileDialog1 = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog1.Filter = "Text Document|*.txt";
            saveFileDialog1.Title = "Choose where to save the report";
            saveFileDialog1.DefaultExt = ".txt";
            saveFileDialog1.FileName = Path.GetFileNameWithoutExtension(fileTab.MediaInfo.General[0].FileName) + ".txt";
            saveFileDialog1.InitialDirectory = fileTab.MediaInfo.General[0].FolderName;
            Microsoft.Win32.SaveFileDialog saveFileDialog2 = saveFileDialog1;
            if (!saveFileDialog2.ShowDialog().GetValueOrDefault())
                return;
            new StreamWriter(saveFileDialog2.FileName).WriteLine(fileTab.MediaInfo.InfoComplete);
        }

        private void MenuItemSettings_Click(object sender, RoutedEventArgs e)
        {
            Settings settings = new Settings();
            settings.Owner = GetWindow(this);
            settings.ShowDialog();
            UpdateHistoryMenuItem();
            BackgroundWorker backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += UIRefresher_DoWork;
            backgroundWorker.RunWorkerAsync();
        }

        private void CloseTabButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (FileTab fileTab in FileTabControl.Items)
            {
                string text = ((TextBlock)((System.Windows.Controls.Panel)((FrameworkElement)e.Source).Parent).Children[1]).Text;
                if (fileTab.HeaderText.Equals(text))
                {
                    FileTabControl.Items.Remove(fileTab);
                    break;
                }
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((Selector)sender).SelectedIndex != -1)
                return;
            ((Selector)sender).SelectedIndex = GlobalSettings.DisableSimpleReport ? 1 : 0;
        }

        private void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            SaveConfig();
            Environment.Exit(0);
        }

        private void FileTabControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (FileTabControl.Items.Count != 0)
                return;
            MenuItemOpenFiles_Click(null, null);
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            bool flag1 = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
            bool flag2 = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
            if (e.Key == Key.O & flag1)
            {
                if (flag2)
                    MenuItemOpenFolder_Click(null, null);
                else
                    MenuItemOpenFiles_Click(null, null);
            }
            if (e.Key == Key.S & flag1 & flag2)
            {
                if (flag2)
                    MenuItemSaveCompleteReport_Click(null, null);
                else
                    MenuItemSaveStandardReport_Click(null, null);
            }
            if (e.Key == Key.X & flag1)
                MenuItemCloseFiles_Click(null, null);
            if (e.Key == Key.E & flag1)
                MenuItemExit_Click(null, null);
            if (e.Key != Key.Tab || FileTabControl.Items.Count <= 0)
                return;
            FileTabControl.SelectedIndex += 1 % FileTabControl.Items.Count;
        }

        private void MenuItemOpenFolder_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog()
            {
                ShowNewFolderButton = false,
                Description = "Select a folder to analyze"
            };
            if (folderBrowserDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK || !Directory.Exists(folderBrowserDialog.SelectedPath))
                return;
            AddTabItems(Directory.GetFiles(folderBrowserDialog.SelectedPath));
        }

        private void MenuItemCloseFiles_Click(object sender, RoutedEventArgs e)
        {
            while (FileTabControl.HasItems)
                FileTabControl.Items.RemoveAt(0);
        }

        private void MenuItemOpenRecentFiles_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.MenuItem menuItem = sender as System.Windows.Controls.MenuItem;
            AddTabItems(GlobalSettings.History.First(s => s.Contains((menuItem.Header as string).Substring(3))));
        }

        private void ClearAll()
        {
            StatusLab.Content = "Cleaning all...";
            StatusLab.InvalidateVisual();
            while (FileTabControl.Items.Count > 0)
                FileTabControl.Items.RemoveAt(0);
            StatusLab.Content = "Ready.";
            StatusLab.InvalidateVisual();
        }

        private void ProcessArgs(params string[] args)
        {
            string[] strArray = args;
            if (strArray.Length <= 1)
                return;
            List<string> stringList = new List<string>();
            for (int index = 1; index < strArray.Length; ++index)
            {
                if (File.Exists(strArray[index]))
                    stringList.Add(strArray[index]);
            }
            AddTabItems(stringList.ToArray());
        }

        private void AddTabItems(params string[] inputpaths)
        {
            List<string> source = new List<string>()
      {
        ".lnk",
        ".db",
        ".exe",
        ".dll",
        ".bat",
        ".cmd",
        ".ini",
        ".xml",
        ".cfg",
        ".json",
        ".iso",
        ".bin",
        ".rar",
        ".zip",
        ".7z",
        ".inf",
        ".nfo",
        ".pdf",
        ".pptx",
        ".ppt",
        ".docx",
        ".doc",
        ".xls",
        ".xlsx"
      };
            if (inputpaths.Length > 10 && System.Windows.MessageBox.Show("Are you sure do you want to open " + inputpaths.Length + " files?", "Multiple files opening confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                return;
            int count = FileTabControl.Items.Count;
            foreach (string inputpath in inputpaths)
            {
                string path = inputpath;
                if (File.Exists(path) && !source.Any(f => path.EndsWith(f)) && !FileTabControl.Items.Cast<FileTab>().Any(s => s.FilePath.Equals(path)))
                {
                    FileTabControl.Items.Add(new FileTab(path));
                    AddFileToHistory(path);
                }
            }
            if (inputpaths.Length != 0)
                FileTabControl.SelectedIndex = count;
            if (BackgroundWorker.IsBusy)
                return;
            BackgroundWorker.RunWorkerAsync();
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            label_6:
            if (!FileTabControl.Items.Cast<FileTab>().AsQueryable().Any(i => !i.IsReady))
                return;
            for (int index = 0; index < FileTabControl.Items.Count; ++index)
            {
                FileTab fileTab = (FileTab)FileTabControl.Items[index];
                if (!fileTab.IsReady)
                    fileTab.Initialize();
            }
            goto label_6;
        }

        private void UIRefresher_DoWork(object sender, DoWorkEventArgs e)
        {
            List<Task> taskList = new List<Task>();
            foreach (FileTab fileTab in FileTabControl.Items)
            {
                FileTab tab = fileTab;
                Task task = Task.Factory.StartNew(() => tab.Refresh());
                taskList.Add(task);
            }
            Task.WaitAll(taskList.ToArray());
        }

        private void SaveConfig()
        {
            try
            {
                string str = Path.Combine(GetAppDataFolder(), "userconfig.xml");
                if (File.Exists(str))
                    File.Delete(str);
                XmlWriter xmlWriter = XmlWriter.Create(str, new XmlWriterSettings()
                {
                    Indent = true
                });
                xmlWriter.WriteStartDocument();
                xmlWriter.WriteComment("GetMediaInfo GUI - User Configuration");
                xmlWriter.WriteStartElement("database");
                xmlWriter.WriteStartElement("config");
                xmlWriter.WriteAttributeString("tag", "AutoSaveReport");
                xmlWriter.WriteValue(GlobalSettings.AutoSaveReport);
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("config");
                xmlWriter.WriteAttributeString("tag", "DisableSimple");
                xmlWriter.WriteValue(GlobalSettings.DisableSimpleReport);
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("config");
                xmlWriter.WriteAttributeString("tag", "DisableComplete");
                xmlWriter.WriteValue(GlobalSettings.DisableCompleteReport);
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("config");
                xmlWriter.WriteAttributeString("tag", "DisableTree");
                xmlWriter.WriteValue(GlobalSettings.DisableTreeReport);
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("config");
                xmlWriter.WriteAttributeString("tag", "ShowMilliseconds");
                xmlWriter.WriteValue(GlobalSettings.ShowMilliseconds);
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("config");
                xmlWriter.WriteAttributeString("tag", "RemoveSpace");
                xmlWriter.WriteValue(GlobalSettings.RemoveJustification);
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("config");
                xmlWriter.WriteAttributeString("tag", "ShowCover");
                xmlWriter.WriteValue(GlobalSettings.ShowCover);
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("config");
                xmlWriter.WriteAttributeString("tag", "Bold");
                xmlWriter.WriteValue(GlobalSettings.Bold);
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("config");
                xmlWriter.WriteAttributeString("tag", "Italic");
                xmlWriter.WriteValue(GlobalSettings.Italic);
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("config");
                xmlWriter.WriteAttributeString("tag", "FontSize");
                xmlWriter.WriteValue(GlobalSettings.ReportFontSize.GetValueOrDefault());
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("config");
                xmlWriter.WriteAttributeString("tag", "FontFamily");
                xmlWriter.WriteValue(GlobalSettings.ReportFontFamily.ToString());
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("config");
                xmlWriter.WriteAttributeString("tag", "HistorySize");
                xmlWriter.WriteValue(GlobalSettings.HistorySize);
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndDocument();
                xmlWriter.Close();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Unable to save user configuration. (" + ex.Message + ").");
            }
        }

        private void RestoreConfig()
        {
            string str1 = Path.Combine(GetAppDataFolder(), "userconfig.xml");
            if (File.Exists(str1))
            {
                try
                {
                    foreach (XElement xelement in XDocument.Load(str1).Element("database").Elements("config").ToList())
                    {
                        string str2 = xelement.Value;
                        switch (xelement.Attribute("tag").Value)
                        {
                            case "AutoSaveReport":
                                GlobalSettings.AutoSaveReport = bool.Parse(str2);
                                continue;
                            case "Bold":
                                GlobalSettings.Bold = bool.Parse(str2);
                                continue;
                            case "DisableComplete":
                                GlobalSettings.DisableCompleteReport = bool.Parse(str2);
                                continue;
                            case "DisableSimple":
                                GlobalSettings.DisableSimpleReport = bool.Parse(str2);
                                continue;
                            case "DisableTree":
                                GlobalSettings.DisableTreeReport = bool.Parse(str2);
                                continue;
                            case "FontFamily":
                                GlobalSettings.ReportFontFamily = new FontFamily(str2);
                                continue;
                            case "FontSize":
                                GlobalSettings.ReportFontSize = new double?(double.Parse(str2));
                                continue;
                            case "HistorySize":
                                GlobalSettings.History.MaxSize = int.Parse(str2);
                                continue;
                            case "Italic":
                                GlobalSettings.Italic = bool.Parse(str2);
                                continue;
                            case "RemoveSpace":
                                GlobalSettings.RemoveJustification = bool.Parse(str2);
                                continue;
                            case "ShowCover":
                                GlobalSettings.ShowCover = bool.Parse(str2);
                                continue;
                            case "ShowMilliseconds":
                                GlobalSettings.ShowMilliseconds = bool.Parse(str2);
                                continue;
                            default:
                                continue;
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Unable to restore user settings. Configuration file is corrupted! (" + ex.Message + ").");
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Unable to restore user settings. Configuration file is missing!");
            }
        }

        private void ClearHistory(object sender, RoutedEventArgs e)
        {
            HistoryMenuItem.Items.Clear();
            GlobalSettings.History.Clear();
        }

        private void SaveHistory()
        {
            try
            {
                string str = Path.Combine(GetAppDataFolder(), "history.xml");
                if (File.Exists(str))
                    File.Delete(str);
                XmlWriter xmlWriter = XmlWriter.Create(str, new XmlWriterSettings()
                {
                    Indent = true
                });
                xmlWriter.WriteStartDocument();
                xmlWriter.WriteComment("GetMediaInfo GUI - Recent Files List");
                xmlWriter.WriteStartElement("history");
                while (GlobalSettings.History.Count > 0)
                    xmlWriter.WriteElementString("file", GlobalSettings.History.Dequeue());
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndDocument();
                xmlWriter.Close();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Unable to save user configuration. (" + ex.Message + ").");
            }
        }

        private void RestoreHistory()
        {
            string str = Path.Combine(GetAppDataFolder(), "history.xml");
            if (!File.Exists(str))
                return;
            try
            {
                GlobalSettings.History = new FixedSizedQueue<string>(GlobalSettings.History.MaxSize, XDocument.Load(str).Element("history")?.Elements("file").Select(s => s.Value).ToList());
                UpdateHistoryMenuItem();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Unable to restore user settings. (" + ex.Message + ").");
            }
        }

        private void AddFileToHistory(string path)
        {
            if (GlobalSettings.History.Contains(path))
            {
                List<string> list = GlobalSettings.History.ToList();
                list.Remove(path);
                GlobalSettings.History = new FixedSizedQueue<string>(GlobalSettings.History.MaxSize, list);
            }
            GlobalSettings.History.Enqueue(path);
            UpdateHistoryMenuItem();
        }

        private void UpdateHistoryMenuItem()
        {
            HistoryMenuItem.Items.Clear();
            if (GlobalSettings.History.Count <= 0)
                return;
            int num = 0;
            foreach (string str in GlobalSettings.History.Reverse())
            {
                ++num;
                System.Windows.Controls.MenuItem menuItem = new System.Windows.Controls.MenuItem();
                menuItem.Click += MenuItemOpenRecentFiles_Click;
                menuItem.Header = num.ToString() + " " + str;
                HistoryMenuItem.Items.Add(menuItem);
            }
            HistoryMenuItem.Items.Add(new Separator());
            System.Windows.Controls.MenuItem menuItem1 = new System.Windows.Controls.MenuItem();
            menuItem1.Header = "Clear history";
            System.Windows.Controls.MenuItem menuItem2 = menuItem1;
            menuItem2.Click += ClearHistory;
            HistoryMenuItem.Items.Add(menuItem2);
        }

        private void VerifyMediaInfo(bool ignoremissing = false)
        {
            if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MediaInfo.dll")))
                return;
            GeneralGrid.IsEnabled = false;
            if (ignoremissing)
                return;
            if (StartUpdater())
            {
                GeneralGrid.IsEnabled = true;
            }
            else
            {
                System.Windows.MessageBox.Show("MediaInfo.dll library not found.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Hand);
                System.Windows.Application.Current.Shutdown();
            }
        }

        private void VerifyGetMediaInfo()
        {
            if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GetMediaInfo.dll")))
                return;
            GeneralGrid.IsEnabled = false;
            System.Windows.MessageBox.Show("GetMediaInfo.dll library not found.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Hand);
            System.Windows.Application.Current.Shutdown();
        }

        private bool StartUpdater()
        {
            ClearAll();
            Updater updater1 = new Updater();
            updater1.Owner = GetWindow(this);
            Updater updater2 = updater1;
            updater2.ShowDialog();
            VerifyMediaInfo(true);
            StatusLab.Content = updater2.GetMessageStatus();
            return updater2.GetStatus() == Updater.Status.Success;
        }

        private string GetAppDataFolder()
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GetMediaInfo");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }

        private bool CheckForOtherInstances(params string[] args)
        {
            if (args.Length > 1)
            {
                IntPtr window = InterComm.FindWindow(null, "GetMediaInfo");
                if (!window.Equals(IntPtr.Zero))
                {
                    XElement xelement = new XElement("inputs");
                    for (int index = 1; index < args.Length; ++index)
                        xelement.Add(new XElement("input", args[index]));
                    string s = xelement.ToString(SaveOptions.DisableFormatting);
                    COPYDATASTRUCT lParam = new COPYDATASTRUCT()
                    {
                        dwData = IntPtr.Zero,
                        cbData = Encoding.Unicode.GetByteCount(s) + 2,
                        lpData = Marshal.StringToHGlobalUni(s)
                    };
                    InterComm.SendMessage(window, 74, IntPtr.Zero, ref lParam);
                    System.Windows.Application.Current.Shutdown();
                    _killThisInstance = true;
                    return true;
                }
            }
            return false;
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == 74)
            {
                string stringUni = Marshal.PtrToStringUni(((COPYDATASTRUCT)Marshal.PtrToStructure(lParam, typeof(COPYDATASTRUCT))).lpData);
                if (!string.IsNullOrEmpty(stringUni))
                {
                    try
                    {
                        XDocument xdocument = XDocument.Parse(stringUni);
                        List<string> stringList = new List<string>()
            {
              "dummy"
            };
                        XName name = "input";
                        foreach (XElement descendant in xdocument.Descendants(name))
                            stringList.Add(descendant.Value);
                        if (WindowState == WindowState.Minimized)
                            WindowState = WindowState.Normal;
                        Activate();
                        ProcessArgs(stringList.ToArray());
                        handled = true;
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show(ex.StackTrace);
                    }
                }
            }
            return IntPtr.Zero;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            (PresentationSource.FromVisual(this) as HwndSource)?.AddHook(WndProc);
        }
    }
}
