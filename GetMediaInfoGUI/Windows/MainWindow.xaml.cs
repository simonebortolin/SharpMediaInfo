using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Win32;
using GetMediaInfoGUI.Utils;
using GetMediaInfoGUI.Interop;
using GetMediaInfoGUI.Configuration;
using GetMediaInfoGUI.History;
using GetMediaInfoGUI.Logging;
using GetMediaInfoGUI.Windows;
using GetMediaInfoGUI.Theme;
using GetMediaInfoGUI.Themes.Custom;

namespace GetMediaInfoGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// <seealso cref="Window" />
    public partial class MainWindow : Window
    {
        #region Global Variables
        /// <summary>
        /// Flag to see if the current instance needs to be killed, 
        /// if it's set to true the app should skip all operation 
        /// normally executed at closing.
        /// </summary>
        private bool _killThisInstance = false;
        /// <summary>
        /// The starting drag position.
        /// </summary>
        private Point _startDragPosition = new Point(0,0);
        /// <summary>
        /// The flag that defines whether the user was dragging a tab or not.
        /// </summary>
        private bool _wasDragging = false;
        /// <summary>
        /// Determines whether the mediainfo library has been already loaded (this will prevent the updater to delete the dll).
        /// </summary>
        private bool _libraryAlreadyLoaded = false;
        #endregion

        #region Constructor 
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {

#if DEBUG
            Logger.Instance.SetLogLevel(LogLevel.DEBUG);
#endif
            try
            {
                using (CrossProcessLockFactory.CreateCrossProcessLock())
                {
                    var args = GetArguments();

                    var outsideDropMode = args.Length > 3 && args.Contains(Constants.OutsideTabDropMode);
                    if (outsideDropMode)
                    {
                        CheckDropMode(args);
                    }
                    else if (CheckForOtherInstances(args))
                    {
                        return;
                    }

                    InitializeComponent();
                    this.Title = GetWindowTitle();

                    RestoreConfig();
                    RestoreHistory();

                    if (args.Contains(Constants.ForceUpdateMode))
                    {
                        this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                        StartUpdater(true);
                    }
                    else
                    {
                        StatusLab.Content = "Ready.";
                    }
                }
            } catch (Exception ex)
            {
                MessageDialog.Show(this, $"Unable to create main window: {ex.Message}", "Error detected");
                Logger.Instance.Error(ex);
                Environment.Exit(-1);
            }
        }
        #endregion

        #region Interface Handlers

        #region Window
        /// <summary>
        /// Handles for Loaded event of the Window. Inizializate the window, check dependencies and restore user config.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Window_Loaded(object sender, EventArgs e)
        {
            try
            {
                using (CrossProcessLockFactory.CreateCrossProcessLock())
                {
                    VerifyMediaInfo();
                    VerifyGetMediaInfo();

                    CheckLastUpdateDate();
                }
              
                AnalyzeFiles(GetArguments(true));
            }
            catch (Exception ex)
            {
                MessageDialog.Show(this, $"Unable to correctly load main window: {ex.Message}", "Error detected");
                Logger.Instance.Error(ex);
                Environment.Exit(-1);
            }
        }

        /// <summary>
        /// Handles for Closing event of the Window: saves user configuration.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Window_Closing(object sender, EventArgs e)
        {
            if (!_killThisInstance)
            {
                SaveResources();
            }
        }

        /// <summary>
        /// Handles the Drop event: analyze dropped file\s.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DragEventArgs"/> instance containing the event data.</param>
        private void Window_Drop(object sender, DragEventArgs e)
        {
            DataObject data = e.Data as DataObject;

            if (data.ContainsFileDropList())
            {
                string[] listOfPaths = (string[])e.Data.GetData(DataFormats.FileDrop, false);
                CreateFileTabs(listOfPaths);
            } else if (data.ContainsText())
            {
                var input = e.Data.GetData(DataFormats.Text, false) as string;
                CreateFileTabs(input);
            }
        }

        /// <summary>
        /// Handles the DragEnter event: set cursor with the drag enter icon.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DragEventArgs"/> instance containing the event data.</param>
        private void Window_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
        }

        /// <summary>
        /// Handles the KeyDown event of the Window control. Handles all Hotkeys.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            bool ctrlPressed = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
            bool shiftPressed = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);

            // Open File/Files/Folder
            if (e.Key == Key.O && ctrlPressed)
            {
                if (shiftPressed)
                {
                    //Ctrl+Shift+O : open a folder
                    MenuItemOpenFolder_Click(null, null);
                }
                else
                {
                    //Ctrl+O : open a file
                    MenuItemOpenFiles_Click(null, null);
                }

            }

            // Save 
            if (e.Key == Key.S && ctrlPressed && shiftPressed)
            {
                if (shiftPressed)
                {
                    //Ctrl+Shift+S : save a complete report
                    MenuItemSaveCompleteReport_Click(null, null);
                }
                else
                {
                    //Ctrl+S : save a standard report
                    MenuItemSaveStandardReport_Click(null, null);
                }
            }

            // Ctrl+X : close all tabs
            if (e.Key == Key.X && ctrlPressed)
            {
                MenuItemCloseFiles_Click(null, null);
            }

            // Ctrl+E : exit
            if (e.Key == Key.E && ctrlPressed)
            {
                MenuItemExit_Click(null, null);
            }

            // Tab : move the next file tab
            if (e.Key == Key.Tab && FileTabControl.Items.Count > 0)
            {
                FileTabControl.SelectedIndex = FileTabControl.SelectedIndex + 1 % FileTabControl.Items.Count;
            }
        }
        #endregion

        #region Menu
        /// <summary>
        /// Handles the Click event of the MenuItemExit control: save config and exit.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            SaveResources();
            Environment.Exit(0);
        }
        
        /// <summary>
        /// Handles the Click event of the CopyAll controls in Context Menu: select all and copy text in the standard or complete report.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void MenuItemCopyAll_Click(object sender, RoutedEventArgs e)
        {
            FileTab FileTab = FileTabControl.SelectedItem as FileTab;
            if (FileTab.SelectedTab.Header.Equals("Standard"))
            {
                Clipboard.SetText(FileTab.StandardReport);
            }
            else if (FileTab.SelectedTab.Header.Equals("Complete"))
            {
                Clipboard.SetText(FileTab.CompleteReport);
            }
        }

        /// <summary>
        /// Handles for the Click event of the SaveCover control: saves the cover to disk.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void MenuItemSaveCover_Click(object sender, RoutedEventArgs e)
        {
            if (FileTabControl.SelectedIndex != -1)
            {
                FileTab FileTab = FileTabControl.Items[FileTabControl.SelectedIndex] as FileTab;
                if ((FileTab.MediaInfo?.General[0]?.Cover).GetValueOrDefault())
                {
                    SaveFileDialog dialog = new SaveFileDialog()
                    {
                        Filter = "JPEG Image|*.jpeg",
                        Title = "Select the output path for the cover file",
                        DefaultExt = ".jpeg",
                        FileName = Path.GetFileNameWithoutExtension(FileTab.MediaInfo.General[0].FileName) + ".jpeg",
                        InitialDirectory = FileTab.MediaInfo.General[0].FolderName
                    };
                    if (dialog.ShowDialog().GetValueOrDefault())
                    {
                        JpegBitmapEncoder JpegBitmapEncoder = new JpegBitmapEncoder();
                        JpegBitmapEncoder.Frames.Add(BitmapFrame.Create(FileTab.GetCoverData()));
                        using (FileStream filestream = new FileStream(dialog.FileName, FileMode.Create))
                        {
                            JpegBitmapEncoder.Save(filestream);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the UpdateButton control: check for Updates.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void MenuItemUpdater_Click(object sender, RoutedEventArgs e)
        {
            StartUpdater();
        }

        /// <summary>
        /// Handles the Click event of the MenuItemSaveStandardReport control: saves the standard report to disk.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void MenuItemSaveStandardReport_Click(object sender, RoutedEventArgs e)
        {
            if (FileTabControl.SelectedIndex != -1)
            {
                FileTab FileTab = FileTabControl.Items[FileTabControl.SelectedIndex] as FileTab;
                if (FileTab.IsReady)
                {
                    SaveFileDialog SaveFileDialog = new SaveFileDialog()
                    {
                        Filter = "Text Document|*.txt",
                        Title = "Choose where to save the report",
                        DefaultExt = ".txt",
                        FileName = Path.GetFileNameWithoutExtension(FileTab.MediaInfo.General[0].FileName) + ".txt",
                        InitialDirectory = FileTab.MediaInfo.General[0].FolderName

                    };
                    if (SaveFileDialog.ShowDialog().GetValueOrDefault())
                    {
                        StreamWriter File = new StreamWriter(SaveFileDialog.FileName);
                        File.WriteLine(FileTab.MediaInfo.InfoStandard ?? "Unable to save report.");
                    }

                }
            }
        }

        /// <summary>
        /// Handles the Click event of the MenuItemSaveCompleteReport control: saves the complete report to disk.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void MenuItemSaveCompleteReport_Click(object sender, RoutedEventArgs e)
        {
            if (FileTabControl.SelectedIndex != -1)
            {
                FileTab FileTab = FileTabControl.Items[FileTabControl.SelectedIndex] as FileTab;
                if (FileTab.IsReady)
                {
                    if (FileTab.MediaInfo.InfoComplete.Length > 0)
                    {
                        SaveFileDialog SaveFileDialog = new SaveFileDialog()
                        {
                            Filter = "Text Document|*.txt",
                            Title = "Choose where to save the report",
                            DefaultExt = ".txt",
                            FileName = Path.GetFileNameWithoutExtension(FileTab.MediaInfo.General[0].FileName) + ".txt",
                            InitialDirectory = FileTab.MediaInfo.General[0].FolderName
                        };
                        if (SaveFileDialog.ShowDialog().GetValueOrDefault())
                        {
                            StreamWriter File = new StreamWriter(SaveFileDialog.FileName);
                            File.WriteLine(FileTab.MediaInfo.InfoComplete);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the MenuItemSetting control: opens the setting window.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void MenuItemSettings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow options = new SettingsWindow()
            {
                Owner = Window.GetWindow(this)
            };
            options.ShowDialog();
            UpdateHistoryMenuItem();

            Task.Run(() => RefreshTabs());
        }

        /// <summary>
        /// Handles the Click event of the MenuItemOpenFolder control. Analyze all files inside the target folder.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void MenuItemOpenFolder_Click(object sender, RoutedEventArgs e)
        {
            // Create a "Save As" dialog for selecting a directory (HACK)
            var dialog = new SaveFileDialog
            {
                //InitialDirectory = textbox.Text; // Use current value for initial dir
                Title = "Select a Directory", // instead of default "Save As"
                Filter = "Directory|*.this.directory", // Prevents displaying files
                FileName = "select", // Filename will then be "select.this.directory"
            };
            if (dialog.ShowDialog() == true)
            {
                string path = dialog.FileName;
                // Remove fake filename from resulting path
                path = path.Replace("\\select.this.directory", "");
                path = path.Replace(".this.directory", "");
                // If user has changed the filename, create the new directory
                if (Directory.Exists(path))
                {
                    CreateFileTabs(Directory.GetFiles(path));
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the MenuItem_Open control: opens a new openfiledialog to choose new input file\s.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void MenuItemOpenFiles_Click(object sender, RoutedEventArgs e)
        {
            var file = new OpenFileDialog()
            {
                Filter = "Media Files|*.*",
                Title = "Select a files to analyze",
                Multiselect = true
            };
            if (file.ShowDialog().GetValueOrDefault())
            {

                CreateFileTabs(file.FileNames);
            }
        }

        /// <summary>
        /// Handles the Click event of the MenuItemCloseFiles control. Close all current open File tabs.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void MenuItemCloseFiles_Click(object sender, RoutedEventArgs e)
        {
            while (FileTabControl.HasItems)
            {
                var fileTab = FileTabControl.Items.GetItemAt(0) as FileTab;
                fileTab.Dispose();
                FileTabControl.Items.Remove(fileTab);
            }
        }

        /// <summary>
        /// Handles the Click event of the MenuItemOpenRecentFiles control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void MenuItemOpenRecentFiles_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem)
            {
                CreateFileTabs(menuItem.Tag as string);
            }           
        }

        /// <summary>
        /// Clears the recent files history.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void MenuItemClearRecentFiles_Click(object sender, RoutedEventArgs e)
        {
            HistoryMenuItem.Items.Clear();
            HistoryHelper.GetHistory().Clear();
        }

        /// <summary>
        /// Handles the Click event of the MenuItemAbout control.
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data</param>
        private void MenuItemAbout_Click(object sender, RoutedEventArgs e)
        {
            var aboutWindow = new AboutWindow()
            {
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = Window.GetWindow(this)
            };
            aboutWindow.ShowDialog();
        }
        #endregion

        #region TabControl
        /// <summary>
        /// Handles the SelectionChanged event of the TabControl control: changes the selected index based on the global settings if the current selected index is -1.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is TabControl tabControl)
            {
                if (tabControl.SelectedIndex == -1)
                {
                    tabControl.SelectedIndex = ConfigHelper.GetConfig().DisableSimpleReport ? 1 : 0;
                }
            }           
        }

        #region TabItem
        /// <summary>
        /// Handles the Click event of the TabItem CloseButton control: closes the related TabItem.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void TabItem_CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                if (button.DataContext is FileTab fileTab)
                {
                    fileTab.Dispose();
                    FileTabControl.Items.Remove(fileTab);
                }
            }
        }

        /// <summary>
        /// Handles the Drop event of the TabItem control: enables the user to drop a tab into a new position.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DragEventArgs"/> instance containing the event data.</param>
        private void TabItem_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(typeof(TabItem)) is TabItem source && e.Source is TabItem target)
            {
                TabControl parent = target.Parent as TabControl;
                var targetPresent = FileTabControl.Items.Contains(target);
                var sourcePresent = FileTabControl.Items.Contains(source);

                // outer tabcontrol
                if (parent == null && source.Content is FileTab sourceContent && target.Content is FileTab targetContent)
                {
                    if (!targetContent.Equals(sourceContent))
                    {
                        int sourceIndex = FileTabControl.Items.IndexOf(sourceContent);
                        int targetIndex = FileTabControl.Items.IndexOf(targetContent);

                        FileTabControl.Items.Remove(sourceContent);
                        FileTabControl.Items.Insert(targetIndex, sourceContent);

                        FileTabControl.Items.Remove(targetContent);
                        FileTabControl.Items.Insert(sourceIndex, targetContent);
                    }
                }
                else //inner tabcontrol
                {
                    if (!target.Equals(source))
                    {
                        int sourceIndex = parent.Items.IndexOf(source);
                        int targetIndex = parent.Items.IndexOf(target);

                        parent.Items.Remove(source);
                        parent.Items.Insert(targetIndex, source);

                        parent.Items.Remove(target);
                        parent.Items.Insert(sourceIndex, target);
                    }
                }
            }
        }

        /// <summary>
        /// Handles the MouseDown event of the TabControl control. Open a Openfiledialog if user left click the empty FileTabControl or closes the hovered tab if the middle button is pressed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void TabItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (FileTabControl.Items.Count == 0)
                {
                    MenuItemOpenFiles_Click(null, null);
                }
            }
            else if (e.MiddleButton == MouseButtonState.Pressed)
            {
                if (e.Source is TabItem item && item.Content is FileTab fileTab)
                {
                    fileTab.Dispose();
                    FileTabControl.Items.Remove(fileTab);
                }
            }
        }

        /// <summary>
        /// Handles the PreviewMouseMove event of the TabItem control: generate drag n' drop event if a tab is moved to let user to reorder the TabControl items.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        private void TabItem_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.Source is TabItem item)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    DragDrop.DoDragDrop(item, item, DragDropEffects.Move);
                }
            }
        }

        #endregion

        #region Drag & Drop Management
        /// <summary>
        /// Handles the MouseMove event of the TabItem control: enables the drag outside the main window and the spawn of a new application instance if the drop is
        /// outside the window or the move of a tab if the drop is done inside the window of another instance.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event arguments</param>
        private void TabItem_MouseMove(object sender, MouseEventArgs e)
        {
            Point mpos = e.GetPosition(null);
            Vector diff = this._startDragPosition - mpos;

            if (e.Source is TabItem item && item.Content is FileTab fileTab)
            {
                //create drag event if the cursor goes outside the main window
                if (e.LeftButton == MouseButtonState.Pressed &&
                 Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance &&
                 Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    //mock drag event to let file explorer to change cursor
                    string dataFormat = DataFormats.FileDrop;
                    DataObject dataObject = new DataObject(dataFormat, new string[] { "" });

                    DragDrop.AddQueryContinueDragHandler(this, QueryContinueDragHandler);
                    DragDrop.DoDragDrop(FileTabControl, dataObject, DragDropEffects.Copy);

                    _wasDragging = true;
                }

                //handle ouside drop
                if (_wasDragging && e.LeftButton == MouseButtonState.Released)
                {
                    DragDrop.RemoveQueryContinueDragHandler(this, QueryContinueDragHandler);

                    if (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance &&
                        Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
                    {
                        //Compute drop screen point
                        var mousePos = MouseUtils.GetMousePosition();
                        var dropPos = new System.Drawing.Point(mousePos.X, mousePos.Y);

                        //Detect collision with other instances window
                        var dropInsideAnotherInstance = false;
                        var instances = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName);
                        foreach (var instance in instances)
                        {
                            IntPtr windowHandle = instance.MainWindowHandle;
                            var rect = InterComm.GetWindowRectangle(windowHandle);
                            if (rect.HasValue && rect.Value.Contains(dropPos.X, dropPos.Y))
                            {
                                //if the drop is in the same instance do nothing
                                if (Process.GetCurrentProcess().MainWindowHandle.Equals(windowHandle))
                                {
                                    _wasDragging = false;
                                    return;
                                }

                                dropInsideAnotherInstance = true;
                                break;
                            }
                        }

                        //if no collision is found, create a new instance
                        if (!dropInsideAnotherInstance)
                        {
                            using (var p = new Process()
                            {
                                StartInfo = new ProcessStartInfo
                                {
                                    FileName = System.Reflection.Assembly.GetExecutingAssembly().Location,
                                    Arguments = string.Format("{0} {1} {2} \"{3}\"", Constants.OutsideTabDropMode, dropPos.X, dropPos.Y, fileTab.FilePath)
                                }
                            })
                            {
                                p.Start();
                            }
                        }
                        else
                        {
                            string dataFormat = DataFormats.FileDrop;
                            DataObject dataObject = new DataObject(dataFormat, new string[] { fileTab.FilePath });
                            DragDrop.DoDragDrop(FileTabControl, dataObject, DragDropEffects.Copy);
                        }
                    }

                    //remove the dropped item
                    fileTab.Dispose();
                    FileTabControl.Items.Remove(fileTab);

                    if (FileTabControl.Items.Count == 0)
                    {
                        Application.Current.Shutdown();
                        _killThisInstance = true;
                    }

                    _wasDragging = false;
                }
            }
        }

        /// <summary>
        /// Handles the PreviewMouseLeftButtonDown event of the TabItem control: keeps track of the drag starting position.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event arguments</param>
        private void TabItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _startDragPosition = e.GetPosition(null);
        }

        /// <summary>
        /// Handles the QueryContinueDrag event of the TabItem control: enables the program to cancel or continue the dragging.
        /// </summary>
        /// <param name="source">The source of the event</param>
        /// <param name="e">The event arguments</param>
        private void QueryContinueDragHandler(object source, QueryContinueDragEventArgs e)
        {
            e.Handled = true;

            if (e.EscapePressed)
            {
                e.Action = DragAction.Cancel;
                return;
            }

            if (e.KeyStates.HasFlag(DragDropKeyStates.LeftMouseButton))
            {
                e.Action = DragAction.Continue;
            }
            else
            {
                e.Action = DragAction.Cancel;
                return;
            }

            if (e.KeyStates.HasFlag(DragDropKeyStates.MiddleMouseButton))
            {
                e.Action = DragAction.Continue;
            }
        }

        #endregion

        #endregion

        #endregion

        #region Arguments Management

        /// <summary>
        /// Retrieves the command line arguments properly filtered.
        /// </summary>
        /// <param name="filterFilesOnly">Whether to filter out any non-path argument or not.</param>
        /// <returns>The list of valid arguments.</returns>
        private string[] GetArguments(bool filterFilesOnly = false)
        {
            var args = Environment.GetCommandLineArgs().Where(i => !i.Equals(Assembly.GetExecutingAssembly().Location)).ToArray();
            if (filterFilesOnly)
            {
                args = args.Where(i => !i.Equals(Constants.ForceUpdateMode) && !i.Equals(Constants.OutsideTabDropMode)).ToArray();
            }        
            return args;
        }

        /// <summary>
        /// Checks for outside drop mode.
        /// </summary>
        /// <param name="args">The process arguments</param>
        private void CheckDropMode(params string[] args)
        {
            var outsideDropMode = (args.Length > 3 && args.Contains(Constants.OutsideTabDropMode));
            if (outsideDropMode)
            {
                Application.Current.MainWindow = this;
                this.Left = int.Parse(args[1]);
                this.Top = int.Parse(args[2]);

                this.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Launches the analysis of all files in the program arguments.
        /// </summary>
        private void AnalyzeFiles(params string[] args)
        {
            var paths = args.Where(i => File.Exists(i));
            CreateFileTabs(paths.ToArray());
        }

        /// <summary>
        /// Creates a new FileTab for each path in the input list and adds it to the interface.
        /// </summary>
        /// <param name="paths">The list of input paths.</param>
        /// <param name="skipWarning">Whether to skip the warning about the number of files to open</param>
        private void CreateFileTabs(params string[] paths)
        {
            List<string> formatToIgnore = new List<string>() { ".lnk", ".db" , ".exe", ".dll", ".bat",
                ".cmd", ".ini", ".xml", ".cfg", ".json", ".iso", ".bin", ".rar", ".zip", ".7z", ".inf",
                ".nfo", ".pdf", ".pptx", ".ppt", ".docx", ".doc", ".xls", ".xlsx" };

            if (paths.Length > 10)
            {
                var result = MessageDialog.Show(this, $"Are you sure do you want to open {paths.Length} files?",
                    "Multiple files opening confirmation", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.No)
                {
                    return;
                }
            }

            int currCount = FileTabControl.SelectedIndex >= 0 ? FileTabControl.SelectedIndex : 0;

            foreach (string path in paths)
            {
                if (File.Exists(path) && !formatToIgnore.Any(f => path.EndsWith(f)))
                {
                    if (!FileTabControl.Items.Cast<FileTab>().Any(s => s.FilePath.Equals(path)))
                    {
                        FileTabControl.Items.Add(new FileTab(path));
                        AddFileToHistory(path);
                    }
                }
            }

            if (paths.Length > 0)
            {
                Task.Run(() => {
                    InitializeTabs();
                }).ConfigureAwait(false);

                FileTabControl.SelectedIndex = currCount;
                _libraryAlreadyLoaded = true;
            }
        }

        /// <summary>
        /// Gets the list of opened files.
        /// </summary>
        /// <returns>The list of opened file paths</returns>
        public List<string> GetOpenedFiles()
        {
            return FileTabControl.Items.Cast<FileTab>().Select(i => i.FilePath).ToList();
        }
        #endregion

        #region Backgrounds Activities

        /// <summary>
        /// Initializes all tabs.
        /// </summary>
        private void InitializeTabs()
        {
            var fileTabs = FileTabControl.Items.Cast<FileTab>();
            //blocking method (useless to parallelize I/O bound operations)
            fileTabs.Where(i => !i.IsReady).ToList().ForEach(i => i.Initialize());
        }

        /// <summary>
        /// Refreshes all tabs.
        /// </summary>
        private void RefreshTabs()
        {
            var fileTabs = FileTabControl.Items.Cast<FileTab>();
            //blocking method (useless to parallelize I/O bound operations)
            fileTabs.Where(i => !i.IsReady).ToList().ForEach(i => i.Refresh());
        }
        #endregion

        #region Settings Management
        /// <summary>
        /// Saves user settings into a configuration file.
        /// </summary>
        private void SaveConfig()
        {
            try
            {
                ConfigHelper.SaveConfig();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error("Unable to save user settings", ex);
            }
        }

        /// <summary>
        /// Restores user settings from a configuration file.
        /// </summary>
        private void RestoreConfig()
        {
            try
            {
                ConfigHelper.LoadConfig();            
            }
            catch (Exception ex)
            {
                Logger.Instance.Error("Unable to load user settings", ex);
            }

            ThemeHelper.SetTheme(ConfigHelper.GetConfig().Theme);
        }
        #endregion

        #region History Management
        /// <summary>
        /// Saves recent file list into a file.
        /// </summary>
        private void SaveHistory()
        {
            try
            {
                HistoryHelper.SaveHistory();
            }
            catch (Exception ex)
            {
                MessageDialog.Show(this, $"Unable to save recent file history: {ex.Message}", "Error detected");
                Logger.Instance.Error(ex);
            }
        }

        /// <summary>
        /// Restores recent file list from a file.
        /// </summary>
        private void RestoreHistory()
        {
            try
            {
                HistoryHelper.LoadHistory();
                UpdateHistoryMenuItem();
            } catch (Exception ex) {
                Logger.Instance.Error(ex);
            }         
        }

        /// <summary>
        /// Adds a file to history.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        private void AddFileToHistory(string path)
        {
            if (HistoryHelper.GetHistory().Any(i => i.Path.Equals(path)))
            {
                HistoryHelper.GetHistory().Remove(i => i.Path.Equals(path));
            }
            HistoryHelper.GetHistory().Enqueue(new HistoryItem(path));
            UpdateHistoryMenuItem();
        }

        /// <summary>
        /// Updates the history menu item.
        /// </summary>
        private void UpdateHistoryMenuItem()
        {
            HistoryMenuItem.Items.Clear();

            var clearItem = new MenuItem()
            {
                Header = "Clear all"
            };
            clearItem.Click += MenuItemClearRecentFiles_Click;
            HistoryMenuItem.Items.Add(clearItem);

            HistoryMenuItem.Items.Add(new Separator());

            foreach (var item in HistoryHelper.GetHistory().Reverse())
            {
                var menuItem = new MenuItem();
                menuItem.Click += MenuItemOpenRecentFiles_Click;
                menuItem.Header = item.Path;
                menuItem.Tag = item.Path;
                HistoryMenuItem.Items.Add(menuItem);
            }
        }
        #endregion

        #region Updater Management

        /// <summary>
        /// Verifies the MediaInfo library. If it does not find the dll launches the update, and if the update fails, closes the application.
        /// </summary>
        /// <param name="ignoremissing">if set to <c>true</c> do not force to update if the dll is missing.</param>
        private void VerifyMediaInfo(bool ignoremissing = false)
        {
            if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MediaInfo.dll")))
            {
                GeneralGrid.IsEnabled = false;

                if (!ignoremissing)
                {
                    if (StartUpdater())
                    {
                        GeneralGrid.IsEnabled = true;
                    }
                    else
                    {
                        MessageDialog.Show(this, $"Unable to download the latest version of Mediainfo library", "Error detected");
                        Logger.Instance.Error("Unable to download the latest version of Mediainfo library");
                        Application.Current.Shutdown();
                    }
                }
            }
        }

        /// <summary>
        /// Verifies the GetMediaInfo library. If it does not find the dll closes the application.
        /// </summary>
        private void VerifyGetMediaInfo()
        {
            if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GetMediaInfo.dll")))
            {
                GeneralGrid.IsEnabled = false;
                MessageDialog.Show(this, $"GetMediaInfo.dll library not found", "Error detected");
                Logger.Instance.Error("GetMediaInfo.dll library not found");
                Application.Current.Shutdown();
            }
        }

        /// <summary>
        /// Checks the last update date, if more than a week ago, ask the user if he wats to check again. If the user confirms the updater is launched.
        /// </summary>
        private void CheckLastUpdateDate()
        {
            var lastCheckDate = ConfigHelper.GetConfig().LastUpdateCheck;
            if (lastCheckDate == DateTime.MinValue)
            {
                StartUpdater();
            } else if (DateTime.Now.Date.Subtract(lastCheckDate).Days > 7)
            {
                var result = MessageDialog.Show(this,
                    $"The latest Mediainfo library version was checked at {lastCheckDate:g}. Do you want to check again?",
                    "Mediainfo update check", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    StartUpdater();
                }
            }
        }

        /// <summary>
        /// Starts the Updater.
        /// </summary>
        /// <param name="centerScreen">Force the updater to position at the center of the screen.</param>
        /// <returns>Status of update.</returns>
        private bool StartUpdater(bool centerScreen = false)
        {
            if (_libraryAlreadyLoaded)
            {
                var result = MessageDialog.Show(this, $"To complete the update the program needs to be restarted.{Environment.NewLine}Do you want to restart it now?", 
                    "Update confirmation", MessageBoxButton.YesNo);
                if (result != MessageBoxResult.Yes)
                {
                    return false;
                }

                Process.Start(new ProcessStartInfo
                {
                    FileName = Process.GetCurrentProcess().MainModule.FileName,
                    Arguments = $"{Constants.ForceUpdateMode} {string.Join(" ", GetOpenedFiles())}"
                });

                Application.Current.Shutdown();
                return true;
            } else
            {
                var updater = new UpdaterWindow();
                if (centerScreen)
                {
                    updater.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                } else
                {
                    updater.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    updater.Owner = Window.GetWindow(this);
                }
                updater.ShowDialog();

                StatusLab.Content = updater.GetMessageStatus();
                VerifyMediaInfo(true);
                
                if (updater.GetStatus() == UpdaterWindow.Status.Success || 
                    updater.GetStatus() == UpdaterWindow.Status.NotNeeded)
                {
                    ConfigHelper.GetConfig().LastUpdateCheck = DateTime.Now;
                    return true;
                }

                return false;
            }
        }
        #endregion

        #region Helpers     
        /// <summary>
        /// Restore interface to his default state and flush all main variables.
        /// </summary>
        private void RestoreInterface()
        {
            StatusLab.Content = "Cleaning all...";
            StatusLab.InvalidateVisual();
            FileTabControl.Items.Cast<FileTab>().ToList().ForEach(i => i.Dispose());
            FileTabControl.Items.Clear();
            StatusLab.Content = "Ready.";
            StatusLab.InvalidateVisual();
        }

        /// <summary>
        /// Saves all resources.
        /// </summary>
        private void SaveResources()
        {
            using (CrossProcessLockFactory.CreateCrossProcessLock())
            {
                SaveConfig();
                SaveHistory();
            }
        }

        /// <summary>
        /// Gets the main window title.
        /// </summary>
        /// <returns>The main window title.</returns>
        private string GetWindowTitle()
        {
            return string.Format("GetMediaInfo GUI v{0}", Assembly.GetExecutingAssembly().GetName().Version);
        }
        #endregion

        #region Inter Process Communication
        /// <summary>
        /// Checks for other instances of this application, if another instance 
        /// is found send input arguments then close this application.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        private bool CheckForOtherInstances(params string[] args)
        {
            if (args.Length > 0)
            {
                var hTargetWnd = InterComm.GetWindow(null, GetWindowTitle());

                if (hTargetWnd.HasValue)
                {
                    XElement elem = new XElement("inputs");
                    for (int i = 0; i < args.Length; i++)
                    {
                        elem.Add(new XElement("input", args[i]));
                    }
                    string data = elem.ToString(SaveOptions.DisableFormatting);

                    InterComm.SendMessageToWindow(hTargetWnd.Value, InterComm.WM_COPYDATA, data);

                    Application.Current.Shutdown();
                    _killThisInstance = true;

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Generates the event <see cref="E:System.Windows.Window.SourceInitialized" />.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the evene data.</param>
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
            InterComm.Instance.SubscribeWindowMessageHook(source);
            InterComm.Instance.OnWindowMessage += (eventArgs) => {
                try
                {
                    XDocument doc = XDocument.Parse(eventArgs.Message);
                    List<string> args = new List<string>();
                    foreach (var input in doc.Descendants("input"))
                    {
                        args.Add(input.Value);
                    }

                    // Bring window to foreground
                    if (this.WindowState == WindowState.Minimized)
                    {
                        this.WindowState = WindowState.Normal;
                    }
                    this.Activate();

                    AnalyzeFiles(args.ToArray());
                }
                catch (Exception ex)
                {
                    Logger.Instance.Error(ex);
                }
            };
        }
        #endregion
    }
}
