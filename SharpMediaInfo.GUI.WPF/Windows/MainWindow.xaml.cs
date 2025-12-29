using SharpMediaInfo.GUI.ViewModels;
using SharpMediaInfo.GUI.WPF.Extensions;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
using SharpMediaInfo.GUI.Models;

namespace SharpMediaInfo.GUI.WPF.Windows;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private MainViewModel _viewModel;
    private AppConfiguration? _configuration = null;
    private Point _dragStartPosition;
    private bool _isDragging;
    private FileTabViewModel? _draggedTab;
    private bool _wasDraggedOutside;

    const string customFormat = "SharpMediaInfo.TabTransfer";

    const string tabFormat = "SharpMediaInfo.FileTabViewModel";


    public MainWindow(MainViewModel viewModel)
    {
        // Apply current app theme before UI loads to reduce flash
        this.ThemeMode = Application.Current?.ThemeMode ?? ThemeMode.System;

        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel ?? throw new ArgumentNullException(nameof(viewModel));

        // Removed auto-close logic - keep window open even with no tabs
        // This allows users to drag & drop files or open new files from menu

        AttachConfigurationHandlers(viewModel.Configuration);
        ApplyTheme(viewModel.Configuration.AppTheme);
    }

    private void AttachConfigurationHandlers(AppConfiguration configuration)
    {
        if (_configuration != null)
        {
            _configuration.PropertyChanged -= Configuration_PropertyChanged;
        }

        _configuration = configuration;
        if (_configuration != null)
        {
            _configuration.PropertyChanged += Configuration_PropertyChanged;
        }
    }

    private void Configuration_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AppConfiguration.AppTheme) && _configuration != null)
        {
            ApplyTheme(_configuration.AppTheme);
        }
    }

    private void ApplyTheme(AppTheme appTheme)
    {
        ThemeMode mode = ThemeMode.System;
        if (appTheme == AppTheme.Light) mode = ThemeMode.Light;
        else if (appTheme == AppTheme.Dark) mode = ThemeMode.Dark;

        this.ThemeMode = mode;
        if (Application.Current != null)
        {
            Application.Current.ThemeMode = mode;
        }
    }

    private async void Window_Drop(object sender, DragEventArgs e)
    {

        // Priority 1: Check for SharpMediaInfo custom format
        if (e.Data.GetDataPresent(customFormat))
        {
            var filePath = e.Data.GetData(customFormat) as string;

            if (!string.IsNullOrEmpty(filePath) && System.IO.File.Exists(filePath) && DataContext is MainViewModel viewModel)
            {
                await viewModel.AddFilesFromDropAsync(new[] { filePath });
                e.Handled = true;
                return;
            }
        }

        // Priority 2: Internal tab drag (reordering within same window)
        if (e.Data.GetDataPresent(tabFormat))
        {
            e.Handled = true;
            return;
        }

        // Priority 3: External file drop from Windows Explorer
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            // Filter out files that don't exist
            var validFiles = files.Where(f => !string.IsNullOrEmpty(f) && System.IO.File.Exists(f)).ToArray();

            if (validFiles.Length > 0 && DataContext is MainViewModel viewModel)
            {
                await viewModel.AddFilesFromDropAsync(validFiles);
            }

            e.Handled = true;
        }
    }

    private void Window_DragEnter(object sender, DragEventArgs e)
    {
        // Accept ONLY SharpMediaInfo custom format or regular file drops
        if (e.Data.GetDataPresent(customFormat))
        {
            e.Effects = DragDropEffects.Move;
        }
        else if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            e.Effects = DragDropEffects.Copy;
        }
        else
        {
            e.Effects = DragDropEffects.None;
        }
    }

    private void TabControl_Drop(object sender, DragEventArgs e)
    {
        // Priority 1: SharpMediaInfo tab transfer
        if (e.Data.GetDataPresent(customFormat))
        {
            var filePath = e.Data.GetData(customFormat) as string;

            if (!string.IsNullOrEmpty(filePath) && System.IO.File.Exists(filePath) && DataContext is MainViewModel viewModel)
            {
                _ = viewModel.AddFilesFromDropAsync(new[] { filePath });
                e.Handled = true;
                return;
            }
        }

        // Priority 2: External file drop from Explorer
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            var validFiles = files.Where(f => !string.IsNullOrEmpty(f) && System.IO.File.Exists(f)).ToArray();

            if (validFiles.Length > 0 && DataContext is MainViewModel viewModel)
            {
                _ = viewModel.AddFilesFromDropAsync(validFiles);
            }

            e.Handled = true;
        }
    }

    private void Exit_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    #region Tab Drag & Drop Handlers

    private void TabItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is TabItem tabItem && tabItem.DataContext is FileTabViewModel viewModel)
        {
            _dragStartPosition = e.GetPosition(this);
            _draggedTab = viewModel;
            _wasDraggedOutside = false;
            _isDragging = false;
        }
    }

    private void TabItem_PreviewMouseMove(object sender, MouseEventArgs e)
    {
        if (_draggedTab == null || e.LeftButton != MouseButtonState.Pressed || _isDragging)
            return;

        var currentPosition = e.GetPosition(this);
        var diff = _dragStartPosition - currentPosition;

        // Check if drag threshold exceeded
        if (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
            Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
        {
            StartTabDrag();
        }
    }

    private void TabItem_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        _draggedTab = null;
        _isDragging = false;
    }

    private void TabItem_Drop(object sender, DragEventArgs e)
    {
        if (sender is not TabItem targetTabItem || targetTabItem.DataContext is not FileTabViewModel targetTab)
            return;

        // Check if this is an internal tab drag
        if (e.Data.GetDataPresent(tabFormat))
        {
            var sourceTab = e.Data.GetData(tabFormat) as FileTabViewModel;
            if (sourceTab != null && sourceTab != targetTab && DataContext is MainViewModel viewModel)
            {
                // Get indices
                var sourceIndex = viewModel.Tabs.IndexOf(sourceTab);
                var targetIndex = viewModel.Tabs.IndexOf(targetTab);

                if (sourceIndex >= 0 && targetIndex >= 0 && sourceIndex != targetIndex)
                {
                    // Remove from old position
                    viewModel.Tabs.RemoveAt(sourceIndex);

                    // Insert at new position
                    var insertIndex = sourceIndex < targetIndex ? targetIndex : targetIndex;
                    viewModel.Tabs.Insert(insertIndex, sourceTab);

                    // Keep selection
                    viewModel.SelectedTab = sourceTab;
                }

                e.Handled = true;
            }
        }
    }

    private void StartTabDrag()
    {
        if (_draggedTab == null)
        {
            Debug.WriteLine("StartTabDrag: _draggedTab is null!");
            return;
        }

        Debug.WriteLine($"=== Starting drag for tab: {_draggedTab.Header} ===");
        _isDragging = true;
        _wasDraggedOutside = false;

        try
        {
            // Hook QueryContinueDrag to detect when drag goes outside window
            DragDrop.AddQueryContinueDragHandler(this, OnQueryContinueDrag);
            DragDrop.AddGiveFeedbackHandler(this, OnGiveFeedback);

            // Create drag data with CUSTOM FORMAT that only SharpMediaInfo recognizes
            var data = new DataObject();

            // Custom format for SharpMediaInfo inter-instance communication
            data.SetData(customFormat, _draggedTab.Model.FilePath);

            // Internal reordering marker (process-specific)
            data.SetData(tabFormat, _draggedTab);

            // Start drag operation
            var result = DragDrop.DoDragDrop(MainTabControl, data, DragDropEffects.Move | DragDropEffects.Copy);

            // Handle completion based on result
            if (_wasDraggedOutside)
            {
                if (result == DragDropEffects.None)
                {
                    // Dropped outside but no target accepted it - create new instance
                    HandleOutsideDrop();
                }
                else if (result.HasFlag(DragDropEffects.Move) || result.HasFlag(DragDropEffects.Copy))
                {
                    // Dropped on another SharpMediaInfo window - remove tab
                    var tabToRemove = _draggedTab;
                    _draggedTab = null;

                    if (DataContext is MainViewModel mainViewModel && tabToRemove != null)
                    {
                        Application.Current.Dispatcher.InvokeAsync(() =>
                        {
                            mainViewModel.Tabs.Remove(tabToRemove);
                        });
                    }
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.WriteLine($"ERROR in tab drag: {ex.Message}");
        }
        finally
        {
            DragDrop.RemoveQueryContinueDragHandler(this, OnQueryContinueDrag);
            DragDrop.RemoveGiveFeedbackHandler(this, OnGiveFeedback);
            _isDragging = false;
        }
    }

    private void OnQueryContinueDrag(object sender, QueryContinueDragEventArgs e)
    {
        // Check if Escape pressed
        if (e.EscapePressed)
        {
            e.Action = DragAction.Cancel;
            Debug.WriteLine("Drag cancelled");
            return;
        }

        // Check if still dragging
        if (e.KeyStates.HasFlag(DragDropKeyStates.LeftMouseButton))
        {
            // Check if mouse is outside window
            var mousePos = MouseExtensions.GetScreenPosition();
            var windowBounds = MouseExtensions.GetWindowScreenBounds(this);

            var isInside = windowBounds.Contains(new Point(mousePos.X, mousePos.Y));

            if (!isInside)
            {
                if (!_wasDraggedOutside)
                {
                    // First time outside
                    Debug.WriteLine($"Mouse OUTSIDE window");
                }

                // Mouse is outside window - mark for outside drop
                _wasDraggedOutside = true;

                // Continue but signal we'll handle it (prevents Windows from accepting drop)
                e.Action = DragAction.Continue;
            }
            else if (_wasDraggedOutside)
            {
                // Was outside, now back inside
                Debug.WriteLine($"?? Mouse back INSIDE window");
                _wasDraggedOutside = false;
            }

            e.Action = DragAction.Continue;
        }
        else
        {
            // Mouse released
            Debug.WriteLine($"Mouse RELEASED");

            // Always allow drop - targets decide if they accept via custom format
            // Desktop won't accept because it doesn't recognize custom format
            e.Action = DragAction.Drop;
        }

        e.Handled = true;
    }

    private void OnGiveFeedback(object sender, GiveFeedbackEventArgs e)
    {
        // Provide custom cursor feedback based on drag state
        if (_wasDraggedOutside)
        {
            // Outside window - use custom cursor handling
            e.UseDefaultCursors = false;

            // Check if we're over a valid SharpMediaInfo window
            // For now, show "move" cursor always when outside
            // The system will determine if it's a valid drop target
            Mouse.SetCursor(Cursors.Arrow);

            e.Handled = true;
        }
        else
        {
            // Inside window - use default cursors
            e.UseDefaultCursors = true;
        }
    }

    private void HandleOutsideDrop()
    {
        if (_draggedTab == null)
        {
            Debug.WriteLine("HandleOutsideDrop: _draggedTab is null!");
            return;
        }

        Debug.WriteLine($"Handling outside drop for: {_draggedTab.Model.FilePath}");

        // Save the tab reference BEFORE resetting, because the async operation needs it
        var tabToMove = _draggedTab;
        _draggedTab = null; // Reset now to prevent issues

        if(_viewModel != null)
        {
            Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                if (_viewModel.CompleteTabDragOutsideCommand.CanExecute(tabToMove))
                {
                    await _viewModel.CompleteTabDragOutsideCommand.ExecuteAsync(tabToMove);
                }
            });
        }
    }

    #endregion
}
