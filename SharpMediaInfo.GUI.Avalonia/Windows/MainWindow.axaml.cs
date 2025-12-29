using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using SharpMediaInfo.GUI.ViewModels;
using System;
using System.IO;
using System.Linq;

namespace SharpMediaInfo.GUI.Avalonia.Windows;

public partial class MainWindow : Window
{
    private MainViewModel? _viewModel;
    
    // Drag & drop state
    private Point? _dragStartPosition;
    private bool _isDragging;
    private ITabViewModel? _draggedTab;
    private Control? _draggedTabControl;

    private static DataFormat<string> customFormat;
    private static DataFormat<string> tabFormat;



    static MainWindow()
    {
        customFormat = DataFormat.CreateStringApplicationFormat("SharpMediaInfo.TabTransfer");
        tabFormat = DataFormat.CreateStringApplicationFormat("SharpMediaInfo.FileTabViewModel");
    }

    public MainWindow()
    {
        InitializeComponent();

        AddHandler(DragDrop.DropEvent, Window_Drop);
        AddHandler(DragDrop.DragOverEvent, Window_DragOver);
    }

    public MainWindow(MainViewModel viewModel) : this()
    {
        _viewModel = viewModel;
        DataContext = viewModel;
    }

    #region File Drag & Drop from Explorer

    private async void Window_Drop(object? sender, DragEventArgs e)
    {
        // Priority 1: Check for SharpMediaInfo custom format
        var filePath = e.DataTransfer.TryGetValue(customFormat);
        if (filePath != null)
        {
            if (!string.IsNullOrEmpty(filePath) && System.IO.File.Exists(filePath) && DataContext is MainViewModel viewModel)
            {
                await viewModel.AddFilesFromDropAsync(new[] { filePath });
                e.Handled = true;
                return;
            }
        }

        // Priority 2: Internal tab drag (reordering within same window)
        var tab = e.DataTransfer.TryGetValue(tabFormat);
        if (tab != null)
        {
            e.Handled = true;
            return;
        }

        // Priority 3: External file drop from Windows Explorer
        var files = e.DataTransfer.TryGetFiles()?.Select(f => f.Path.LocalPath)
                .Where(File.Exists)
                .ToArray();
        if (files?.Any() ?? false)
        {
            if (files != null && files.Length > 0 && DataContext is MainViewModel viewModel)
            {
                await viewModel.AddFilesFromDropAsync(files);
            }

            e.Handled = true;
        }
    }

    private void Window_DragOver(object? sender, DragEventArgs e)
    {
        if (e.DataTransfer.Contains(customFormat))
        {
            e.DragEffects = DragDropEffects.Move;
        }
        else if (e.DataTransfer.Contains(DataFormat.File))
        {
            e.DragEffects = DragDropEffects.Copy;
        }
        else
        {
            e.DragEffects = DragDropEffects.None;
        }
    }

    #endregion

    #region Tab Reordering (Internal)

    /// <summary>
    /// Called when pointer is pressed on a tab header
    /// </summary>
    private void TabHeader_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Control control && 
            control.DataContext is ITabViewModel tabViewModel &&
            e.GetCurrentPoint(control).Properties.IsLeftButtonPressed)
        {
            _dragStartPosition = e.GetPosition(this);
            _draggedTab = tabViewModel;
            _draggedTabControl = control;
            _isDragging = false;
            
            e.Handled = true;
        }
    }

    /// <summary>
    /// Called when pointer moves over a tab header
    /// </summary>
    private void TabHeader_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (_draggedTab == null || 
            _dragStartPosition == null || 
            _isDragging ||
            !e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            return;
        }

        var currentPosition = e.GetPosition(this);
        var diff = currentPosition - _dragStartPosition.Value;

        // Check if drag threshold exceeded (5 pixels minimum)
        if (Math.Abs(diff.X) > 5 || Math.Abs(diff.Y) > 5)
        {
            StartTabDrag(e);
        }
    }

    /// <summary>
    /// Called when pointer is released on a tab header
    /// </summary>
    private void TabHeader_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        // Reset drag state
        _dragStartPosition = null;
        _draggedTab = null;
        _draggedTabControl = null;
        _isDragging = false;
    }

    /// <summary>
    /// Start the tab drag operation
    /// </summary>
    private async void StartTabDrag(PointerEventArgs e)
    {
        if (_draggedTab == null || _draggedTabControl == null)
            return;

        _isDragging = true;

        try
        {
            // Create drag data with custom format
            var dragData = new DataTransfer();
            var tabItem = new DataTransferItem();
            tabItem.Set(tabFormat, _draggedTab.Id);
            dragData.Add(tabItem);
            var customItem = new DataTransferItem();
            customItem.Set(customFormat, _draggedTab.Id);
            dragData.Add(customItem);

            // Start drag operation
            var result = await DragDrop.DoDragDropAsync(e, dragData, DragDropEffects.Move);
            
            // Drag completed
            System.Diagnostics.Debug.WriteLine($"Tab drag completed with result: {result}");
        }
        catch (System.Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Tab drag error: {ex.Message}");
        }
        finally
        {
            _isDragging = false;
            _dragStartPosition = null;
            _draggedTab = null;
            _draggedTabControl = null;
        }
    }

    /// <summary>
    /// Called when something is dragged over a tab header (for drop feedback)
    /// </summary>
    private void TabHeader_DragOver(object? sender, DragEventArgs e)
    {
        // Check if this is a tab reorder operation
        if (e.DataTransfer.Contains(tabFormat))
        {
            e.DragEffects = DragDropEffects.Move;
            e.Handled = true;
        }
        else
        {
            e.DragEffects = DragDropEffects.None;
        }
    }

    /// <summary>
    /// Called when something is dropped on a tab header (perform reorder)
    /// </summary>
    private void TabHeader_Drop(object? sender, DragEventArgs e)
    {
        if(e.DataTransfer.Contains(tabFormat))
        {

        }
        if (sender is not Control targetControl || 
            targetControl.DataContext is not ITabViewModel targetTab ||
            DataContext is not MainViewModel viewModel)
        {
            return;
        }

        var targetTabName = targetTab.Id;

        // Check if this is an internal tab reorder
        if (e.DataTransfer.Contains(tabFormat))
        {
            var sourceTab = e.DataTransfer.TryGetValue(tabFormat);
            if (sourceTab != null && sourceTab != targetTabName)
            {
                // Get indices
                var sourceTabValue = viewModel.Tabs.FirstOrDefault(item => item.Id == sourceTab);
                var sourceIndex = sourceTabValue != null ? viewModel.Tabs.IndexOf(sourceTabValue) : -2;
                var targetIndex = viewModel.Tabs.IndexOf(targetTab);

                if (sourceIndex >= 0 && targetIndex >= 0 && sourceIndex != targetIndex)
                {
                    // Remove from old position
                    viewModel.Tabs.RemoveAt(sourceIndex);

                    // Calculate insert index (adjust if moving forward)
                    var insertIndex = sourceIndex < targetIndex ? targetIndex : targetIndex;
                    
                    // Insert at new position
                    viewModel.Tabs.Insert(insertIndex, sourceTabValue!);

                    // Keep selection
                    viewModel.SelectedTab = sourceTabValue;

                    System.Diagnostics.Debug.WriteLine($"Tab reordered from {sourceIndex} to {insertIndex}");
                }

                e.Handled = true;
            }
        }
    }

    #endregion

    private void Exit_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}