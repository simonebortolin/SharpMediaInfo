using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Win32;
using System.Threading.Tasks;
using System.Diagnostics;
using GetMediaInfoGUI.Utils;
using GetMediaInfoGUI.Interop;

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
        #endregion

        #region Constructor 
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            try
            {
                using (CrossProcessLockFactory.CreateCrossProcessLock())
                {
                    var args = Environment.GetCommandLineArgs().Where(i => i != System.Reflection.Assembly.GetExecutingAssembly().Location).ToArray();
                    var outsideDropMode = args.Length > 3 && args.Contains(Constants.OutsideTabDropMode);

                    if (outsideDropMode)
                    {
                        CheckDropMode(args);
                    } else if (CheckForOtherInstances(args)) {
                        return;
                    }

                    InitializeComponent();
                    ClearAll();      
                }
            } catch (Exception ex)
            {
                MessageBox.Show("GetMediaInfo constructor", ex.Message + Environment.NewLine + ex.InnerException + Environment.NewLine + ex.StackTrace );
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
                VerifyMediaInfo();
                VerifyGetMediaInfo();

                var args = Environment.GetCommandLineArgs().Where(i => i != System.Reflection.Assembly.GetExecutingAssembly().Location).ToArray();
                CheckDropMode(args);
                ProcessArgs(args);

                RestoreConfig();
                RestoreHistory();
                StatusLab.Content = "Ready.";
                StatusLab.InvalidateVisual();              
            }
            catch (Exception ex)
            {
                MessageBox.Show("GetMediaInfo loading", ex.Message + Environment.NewLine + ex.InnerException + Environment.NewLine + ex.StackTrace);
            }
        }

        /// <summary>
        /// Handles for Closing event of the Window: saves user configuration.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Windows_Closing(object sender, EventArgs e)
        {
            if (!_killThisInstance)
            {
                SaveConfig();
                SaveHistory();
            }
        }

        /// <summary>
        /// Handles the Drop event: analyze dropped file\s.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DragEventArgs"/> instance containing the event data.</param>
        private void Windows_Drop(object sender, DragEventArgs e)
        {
            Console.WriteLine("Windows_Drop");

            DataObject data = e.Data as DataObject;

            if (data.ContainsFileDropList())
            {
                string[] listOfPaths = (string[])e.Data.GetData(DataFormats.FileDrop, false);
                AddTabItems(listOfPaths);
            } else if (data.ContainsText())
            {
                var input = e.Data.GetData(DataFormats.Text, false) as string;
                AddTabItems(input);
            }
        }

        /// <summary>
        /// Handles the DragEnter event: set cursor with the drag enter icon.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DragEventArgs"/> instance containing the event data.</param>
        private void Windows_DragEnter(object sender, DragEventArgs e)
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
            SaveConfig();
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

                AddTabItems(file.FileNames);
            }
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
            Settings options = new Settings()
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
            System.Windows.Forms.FolderBrowserDialog folder = new System.Windows.Forms.FolderBrowserDialog()
            {
                ShowNewFolderButton = false,
                Description = "Select a folder to analyze"
            };
            if (folder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (Directory.Exists(folder.SelectedPath))
                {
                    AddTabItems(Directory.GetFiles(folder.SelectedPath));
                }
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
            MenuItem menuItem = sender as MenuItem;
            string path = GlobalSettings.History.First(s => s.Contains((menuItem.Header as string).Substring(3)));
            AddTabItems(path);
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
            if (((TabControl)sender).SelectedIndex == -1)
            {
                ((TabControl)sender).SelectedIndex = GlobalSettings.DisableSimpleReport ? 1 : 0;
            }
        }

        /// <summary>
        /// Handles the MouseDown event of the TabControl control. Open a Openfiledialog if user click the emmpty FileTabControl.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void TabControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (FileTabControl.Items.Count == 0)
                {
                    MenuItemOpenFiles_Click(null, null);
                }
            } else if (e.MiddleButton == MouseButtonState.Pressed)
            {
                var mousePos = e.GetPosition(FileTabControl);


                if (FileTabControl.SelectedItem is FileTab fileTab)
                {
                    fileTab.Dispose();
                    FileTabControl.Items.Remove(fileTab);
                }               
            }
        }

        /// <summary>
        /// Handles the PreviewMouseMove event of the TabItem control: generate drag n' drop event of the selected tab if the left mouse button is pressed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        private void TabItem_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.Source is TabItem item)
            {
                if (Mouse.PrimaryDevice.LeftButton == MouseButtonState.Pressed)
                {
                    DragDrop.DoDragDrop(item, item, DragDropEffects.All);
                }
            }           
        }

        /// <summary>
        /// Handles the Drop event of the TabItem control: moves a tab into a new position.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DragEventArgs"/> instance containing the event data.</param>
        private void TabItem_Drop(object sender, DragEventArgs e)
        {
            TabItem Source = e.Data.GetData(typeof(TabItem)) as TabItem;

            if (e.Source is TabItem Target && Source != null)
            {
                TabControl Parent = Target.Parent as TabControl;

                if (!Target.Equals(Source) && Parent != null)
                {
                    int sourceIndex = Parent.Items.IndexOf(Source);
                    int targetIndex = Parent.Items.IndexOf(Target);

                    Parent.Items.Remove(Source);
                    Parent.Items.Insert(targetIndex, Source);

                    Parent.Items.Remove(Target);
                    Parent.Items.Insert(sourceIndex, Target);
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the CloseTabButton control: close the parent tab of the sender button.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void CloseTabButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (FileTab tab in FileTabControl.Items)
            {
                string text = ((TextBlock)((StackPanel)((Button)e.Source).Parent).Children[1]).Text;
                if (tab.HeaderText.Equals(text))
                {
                    FileTabControl.Items.Remove(tab);
                    return;
                }
            }
        }

        /// <summary>
        /// Handles for enable Drag/Drop on report TextBox
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DragEventArgs"/> instance containing the event data.</param>
        private void TextBoxReport_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }
        #endregion

        #endregion

        #region Process Inputs

        /// <summary>
        /// Restore interface to his default state and flush all main variables.
        /// </summary>
        private void ClearAll()
        {
            StatusLab.Content = "Cleaning all...";
            StatusLab.InvalidateVisual();
            FileTabControl.Items.Cast<FileTab>().ToList().ForEach(i => i.Dispose());
            FileTabControl.Items.Clear();
            StatusLab.Content = "Ready.";
            StatusLab.InvalidateVisual();
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
        /// Process arguments passed to the application.
        /// </summary>
        private void ProcessArgs(params string[] args)
        {
            string[] arguments = args;

            var paths = arguments.Where(i => File.Exists(i));
            AddTabItems(paths.ToArray());
        }

        /// <summary>
        /// For each valid path in the input list creates a new FileTab and add it to the interface.
        /// </summary>
        /// <param name="inputpaths">The list of input paths.</param>
        private void AddTabItems(params string[] inputpaths)
        {
            List<string> formatToIgnore = new List<string>() { ".lnk", ".db" , ".exe", ".dll", ".bat",
                ".cmd", ".ini", ".xml", ".cfg", ".json", ".iso", ".bin", ".rar", ".zip", ".7z", ".inf",
                ".nfo", ".pdf", ".pptx", ".ppt", ".docx", ".doc", ".xls", ".xlsx" };

            if (inputpaths.Length > 10)
            {
                if (MessageBox.Show("Are you sure do you want to open " + inputpaths.Length + " files?", 
                    "Multiple files opening confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                {
                    return;
                }
            }

            int currCount = FileTabControl.SelectedIndex >= 0 ? FileTabControl.SelectedIndex : 0;

            foreach (string path in inputpaths)
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

            Task.Run(() => {
                InitializeTabs();
            }).ConfigureAwait(false);
      
            if (inputpaths.Length > 0)
            {
                FileTabControl.SelectedIndex = currCount;
            }
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

        #region Drag & Drop Management
        /// <summary>
        /// Handles the preview mouse left button down event of the TabControl.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event arguments</param>
        private void TabControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _startDragPosition = e.GetPosition(null);
        }

        /// <summary>
        /// Handles the mouse move event in the TabControl.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event arguments</param>
        private void TabControl_MouseMove(object sender, MouseEventArgs e)
        {
            TabControl tabControl = sender as TabControl;
            Point mpos = e.GetPosition(null);
            Vector diff = this._startDragPosition - mpos;

            if (tabControl.SelectedItem is FileTab fileTab)
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
                    DragDrop.DoDragDrop(tabControl, dataObject, DragDropEffects.Copy);
                                  
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
                        var mousePos = System.Windows.Forms.Cursor.Position;
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
                        } else
                        {
                            Console.WriteLine("TabControl_MouseMove: generating valid dragdrop");

                            string dataFormat = DataFormats.FileDrop;
                            DataObject dataObject = new DataObject(dataFormat, new string[] { fileTab.FilePath });
                            DragDrop.DoDragDrop(tabControl, dataObject, DragDropEffects.Copy);
                        }                 
                    }

                    //remove the dropped item
                    fileTab.Dispose();
                    tabControl.Items.Remove(fileTab);

                    if (tabControl.Items.Count == 0)
                    {
                        Application.Current.Shutdown();
                        _killThisInstance = true;
                    }

                    _wasDragging = false;
                }
            }   
        }

        /// <summary>
        /// Handles the drag event's queries for continuation.
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

        #region Settings Management
        /// <summary>
        /// Saves user settings into a configuration file.
        /// </summary>
        private void SaveConfig()
            {
                try
                {
                    string path = Path.Combine(GetAppDataFolder(), "userconfig.xml");

                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }

                    XmlWriter XmlWriter = XmlWriter.Create(path, new XmlWriterSettings() { Indent = true });

                    XmlWriter.WriteStartDocument();
                    XmlWriter.WriteComment("GetMediaInfo GUI - User Configuration");
                    XmlWriter.WriteStartElement("database");

                    #region configs
                    XmlWriter.WriteStartElement("config");
                    XmlWriter.WriteAttributeString("tag", "AutoSaveReport");
                    XmlWriter.WriteValue(GlobalSettings.AutoSaveReport);
                    XmlWriter.WriteEndElement();

                    XmlWriter.WriteStartElement("config");
                    XmlWriter.WriteAttributeString("tag", "DisableSimple");
                    XmlWriter.WriteValue(GlobalSettings.DisableSimpleReport);
                    XmlWriter.WriteEndElement();

                    XmlWriter.WriteStartElement("config");
                    XmlWriter.WriteAttributeString("tag", "DisableComplete");
                    XmlWriter.WriteValue(GlobalSettings.DisableCompleteReport);
                    XmlWriter.WriteEndElement();

                    XmlWriter.WriteStartElement("config");
                    XmlWriter.WriteAttributeString("tag", "DisableTree");
                    XmlWriter.WriteValue(GlobalSettings.DisableTreeReport);
                    XmlWriter.WriteEndElement();

                    XmlWriter.WriteStartElement("config");
                    XmlWriter.WriteAttributeString("tag", "ShowMilliseconds");
                    XmlWriter.WriteValue(GlobalSettings.ShowMilliseconds);
                    XmlWriter.WriteEndElement();

                    XmlWriter.WriteStartElement("config");
                    XmlWriter.WriteAttributeString("tag", "RemoveSpace");
                    XmlWriter.WriteValue(GlobalSettings.RemoveJustification);
                    XmlWriter.WriteEndElement();

                    XmlWriter.WriteStartElement("config");
                    XmlWriter.WriteAttributeString("tag", "ShowCover");
                    XmlWriter.WriteValue(GlobalSettings.ShowCover);
                    XmlWriter.WriteEndElement();

                    XmlWriter.WriteStartElement("config");
                    XmlWriter.WriteAttributeString("tag", "Bold");
                    XmlWriter.WriteValue(GlobalSettings.Bold);
                    XmlWriter.WriteEndElement();

                    XmlWriter.WriteStartElement("config");
                    XmlWriter.WriteAttributeString("tag", "Italic");
                    XmlWriter.WriteValue(GlobalSettings.Italic);
                    XmlWriter.WriteEndElement();
                    #endregion

                    #region fonts
                    XmlWriter.WriteStartElement("config");
                    XmlWriter.WriteAttributeString("tag", "FontSize");
                    XmlWriter.WriteValue(GlobalSettings.ReportFontSize.GetValueOrDefault());
                    XmlWriter.WriteEndElement();

                    XmlWriter.WriteStartElement("config");
                    XmlWriter.WriteAttributeString("tag", "FontFamily");
                    XmlWriter.WriteValue(GlobalSettings.ReportFontFamily.ToString());
                    XmlWriter.WriteEndElement();
                    #endregion

                    #region other

                    XmlWriter.WriteStartElement("config");
                    XmlWriter.WriteAttributeString("tag", "HistorySize");
                    XmlWriter.WriteValue(GlobalSettings.HistorySize);
                    XmlWriter.WriteEndElement();
                    #endregion

                    XmlWriter.WriteEndElement();
                    XmlWriter.WriteEndDocument();
                    XmlWriter.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to save user configuration. (" + ex.Message + ").");
                }
            }

        /// <summary>
        /// Restores user settings from a configuration file.
        /// </summary>
        private void RestoreConfig()
        {
            string path = Path.Combine(GetAppDataFolder(), "userconfig.xml");

            if (File.Exists(path))
            {
                try
                {
                    XDocument XDocument = XDocument.Load(path);
                    List<XElement> settings = XDocument.Element("database").Elements("config").ToList();

                    #region configs
                    foreach (XElement config in settings)
                    {
                        string value = config.Value;

                        switch (config.Attribute("tag").Value)
                        {
                            case "AutoSaveReport":
                                GlobalSettings.AutoSaveReport = bool.Parse(value);
                                break;
                            case "DisableSimple":
                                GlobalSettings.DisableSimpleReport = bool.Parse(value);
                                break;
                            case "DisableComplete":
                                GlobalSettings.DisableCompleteReport = bool.Parse(value);
                                break;
                            case "DisableTree":
                                GlobalSettings.DisableTreeReport = bool.Parse(value);
                                break;
                            case "ShowMilliseconds":
                                GlobalSettings.ShowMilliseconds = bool.Parse(value);
                                break;
                            case "RemoveSpace":
                                GlobalSettings.RemoveJustification = bool.Parse(value);
                                break;
                            case "ShowCover":
                                GlobalSettings.ShowCover = bool.Parse(value);
                                break;
                            case "Italic":
                                GlobalSettings.Italic = bool.Parse(value);
                                break;
                            case "Bold":
                                GlobalSettings.Bold = bool.Parse(value);
                                break;
                            case "FontSize":
                                GlobalSettings.ReportFontSize = double.Parse(value);
                                break;
                            case "FontFamily":
                                GlobalSettings.ReportFontFamily = new System.Windows.Media.FontFamily(value);
                                break;
                            case "HistorySize":
                                GlobalSettings.History.MaxSize = int.Parse(value);
                                break;
                        }
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to restore user settings. Configuration file is corrupted! (" + ex.Message + ").");
                }
            }
            else
            {
                MessageBox.Show("Unable to restore user settings. Configuration file is missing!");
            }
        }
        #endregion

        #region History Management
        /// <summary>
        /// Clears the recent files history.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void ClearHistory(object sender, RoutedEventArgs e)
        {
            HistoryMenuItem.Items.Clear();
            GlobalSettings.History.Clear();
        }

        /// <summary>
        /// Saves recent file list into a file.
        /// </summary>
        private void SaveHistory()
        {
            try
            {
                string path = Path.Combine(GetAppDataFolder(), "history.xml");
                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                XmlWriter xmlWriter = XmlWriter.Create(path, new XmlWriterSettings() { Indent = true });

                xmlWriter.WriteStartDocument();
                xmlWriter.WriteComment("GetMediaInfo GUI - Recent Files List");
                xmlWriter.WriteStartElement("history");
                while (GlobalSettings.History.Count > 0)
                {
                    xmlWriter.WriteElementString("file", GlobalSettings.History.Dequeue());
                }
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndDocument();
                xmlWriter.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to save user configuration. (" + ex.Message + ").");
            }
        }

        /// <summary>
        /// Restores recent file list from a file.
        /// </summary>
        private void RestoreHistory()
        {
            string path = Path.Combine(GetAppDataFolder(), "history.xml");
            if (File.Exists(path))
            {
                try
                {
                    XDocument xDocument = XDocument.Load(path);
                    GlobalSettings.History = new FixedSizedQueue<string>(GlobalSettings.History.MaxSize, 
                        xDocument.Element("history").Elements("file").Select(s => s.Value).ToList());

                    UpdateHistoryMenuItem();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to restore user settings. (" + ex.Message + ").");
                }
            }
        }

        /// <summary>
        /// Adds a file to history.
        /// </summary>
        /// <param name="path">The path of the file.</param>
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

        /// <summary>
        /// Updates the history menu item.
        /// </summary>
        private void UpdateHistoryMenuItem()
        {
            HistoryMenuItem.Items.Clear();
            if (GlobalSettings.History.Count > 0)
            {
                int i = 0;
                foreach (string recentFile in GlobalSettings.History.Reverse())
                {
                    i++;
                    MenuItem menuItem = new MenuItem();
                    menuItem.Click += MenuItemOpenRecentFiles_Click;
                    menuItem.Header = i + " " + recentFile;
                    HistoryMenuItem.Items.Add(menuItem);
                }
                HistoryMenuItem.Items.Add(new Separator());

                MenuItem clearItem = new MenuItem()
                {
                    Header = "Clear history"
                };
                clearItem.Click += ClearHistory;

                HistoryMenuItem.Items.Add(clearItem);
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
                        MessageBox.Show("MediaInfo.dll library not found.","ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show("GetMediaInfo.dll library not found.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }

        /// <summary>
        /// Starts the Updater.
        /// </summary>
        /// <returns>Status of update.</returns>
        private bool StartUpdater()
        {
            if (FileTabControl.Items.Count > 0)
            {
                MessageBox.Show("Detected open file tabs, in order to update the library it's necessary to close and reopen the program.", "WARNING", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            ClearAll();
            Updater updater = new Updater()
            {
                Owner = Window.GetWindow(this)
            };
            updater.ShowDialog();
            VerifyMediaInfo(true);
            StatusLab.Content = updater.GetMessageStatus();
            return (updater.GetStatus() == Updater.Status.Success);
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Gets the app data folder where to save the config files, creates it if it does not exists.
        /// </summary>
        /// <returns>The path to the appdata folder.</returns>
        private string GetAppDataFolder()
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            folder = Path.Combine(folder, "GetMediaInfo");
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            return folder;
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
                var hTargetWnd = InterComm.GetWindow(null, "GetMediaInfo");

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

                    ProcessArgs(args.ToArray());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.StackTrace);
                }
            };
        }
        #endregion
    }
}
