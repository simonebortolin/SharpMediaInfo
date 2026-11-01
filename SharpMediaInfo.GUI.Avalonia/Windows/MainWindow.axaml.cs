using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using CascadiaUI.Controls;
using SharpMediaInfo.GUI.ViewModels;
using System;
using System.IO;
using System.Linq;

namespace SharpMediaInfo.GUI.Avalonia.Windows;

public partial class MainWindow : CascadiaWindow
{
    private MainViewModel? _viewModel;
    
    // Drag & drop state: a tab of ours is being dragged out (_isDragging), and it was dropped
    // back on this same window (_isDraggingTag) - then it must not be closed.
    private bool _isDragging;
    private bool _isDraggingTag;

    private static DataFormat<string> customFormat;
    private static DataFormat<string> tabFormat;



    static MainWindow()
    {
        customFormat = DataFormat.CreateStringPlatformFormat("SharpMediaInfo.TabTransfer");
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

    private static bool TryExtractFilePathFromCustomFormat(
    string? rawValue,
    out string filePath)
    {
        filePath = string.Empty;

        if (string.IsNullOrWhiteSpace(rawValue))
            return false;

        const string prefix = "SharpMediaInfo.GUI.ViewModels.";

        if (!rawValue.StartsWith(prefix))
            return false;

        var payload = rawValue.Substring(prefix.Length);

        var typePrefix = nameof(FileTabViewModel) + ".";
        if (!payload.StartsWith(typePrefix))
            return false;

        filePath = payload.Substring(typePrefix.Length);

        return !string.IsNullOrWhiteSpace(filePath)
               && File.Exists(filePath);
    }


    private async void Window_Drop(object? sender, DragEventArgs e)
    {
        // Priority 1: Check for SharpMediaInfo custom format
        var rawValue = e.DataTransfer.TryGetValue(customFormat);

        if (TryExtractFilePathFromCustomFormat(rawValue, out var filePath) &&
            DataContext is MainViewModel viewModel)
        {
            await viewModel.AddFilesFromDropAsync(new[] { filePath });
                    e.Handled = true;
                    return;
                
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
            if (files != null && files.Length > 0 && DataContext is MainViewModel vm)
            {
                await vm.AddFilesFromDropAsync(files);
            }

            e.Handled = true;
            return;
        }
    }

    private void Window_DragOver(object? sender, DragEventArgs e)
    {
        var rawValue = e.DataTransfer.TryGetValue(customFormat); 

        if (e.DataTransfer.Contains(customFormat) && _isDragging == false && TryExtractFilePathFromCustomFormat(rawValue, out _ ))
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
        e.Handled = true;
    }

    #endregion

    #region Tab tear-out (drag a tab to another SharpMediaInfo window)

    /// <summary>
    /// The tab strip reorders tabs itself; once a tab is dragged off the strip it hands the
    /// drag over to us, and we carry on as a cross-window drag-and-drop. Another instance that
    /// accepts it (see <see cref="Window_Drop"/>) opens the file, and we close our copy.
    /// </summary>
    private async void TabStrip_TabDraggedOut(object? sender, TabDraggedOutEventArgs e)
    {
        if (e.Item is not ITabViewModel tab)
            return;

        _isDragging = true;
        _isDraggingTag = false;

        try
        {
            var dragData = new DataTransfer();
            var tabItem = new DataTransferItem();
            tabItem.Set(tabFormat, tab.Id);
            dragData.Add(tabItem);
            var customItem = new DataTransferItem();
            customItem.Set(customFormat, tab.Id);
            dragData.Add(customItem);

            var result = await DragDrop.DoDragDropAsync(e.TriggerEvent, dragData, DragDropEffects.Move);

            if (_isDraggingTag == false && result == DragDropEffects.Move)
            {
                _viewModel?.Tabs.Remove(tab);
            }
            System.Diagnostics.Debug.WriteLine($"Tab drag completed with result: {result}");
        }
        catch (System.Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Tab drag error: {ex.Message}");
        }
        finally
        {
            _isDraggingTag = false;
            _isDragging = false;
        }
    }

    #endregion

    private void Exit_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}