using Microsoft.Xaml.Behaviors;
using SharpMediaInfo.GUI.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SharpMediaInfo.GUI.WPF.Behaviors;

/// <summary>
/// Attached behavior for advanced tab drag & drop operations
/// Enables dragging tabs outside the window to create new instances
/// </summary>
public class TabDragDropBehavior : Behavior<TabControl>
{
    private Point _startPosition;
    private bool _isDragging;
    private TabItem? _draggedTab;

    public static readonly DependencyProperty AllowOutsideDragProperty =
        DependencyProperty.Register(
            nameof(AllowOutsideDrag),
            typeof(bool),
            typeof(TabDragDropBehavior),
            new PropertyMetadata(true));

    /// <summary>
    /// Gets or sets whether tabs can be dragged outside the window
    /// </summary>
    public bool AllowOutsideDrag
    {
        get => (bool)GetValue(AllowOutsideDragProperty);
        set => SetValue(AllowOutsideDragProperty, value);
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.PreviewMouseLeftButtonDown += OnMouseDown;
        AssociatedObject.PreviewMouseMove += OnMouseMove;
        AssociatedObject.PreviewMouseLeftButtonUp += OnMouseUp;
        AssociatedObject.Drop += OnDrop;
        AssociatedObject.AllowDrop = true;
    }

    protected override void OnDetaching()
    {
        AssociatedObject.PreviewMouseLeftButtonDown -= OnMouseDown;
        AssociatedObject.PreviewMouseMove -= OnMouseMove;
        AssociatedObject.PreviewMouseLeftButtonUp -= OnMouseUp;
        AssociatedObject.Drop -= OnDrop;
        base.OnDetaching();
    }

    private void OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.Source is TabItem tabItem && tabItem.DataContext is FileTabViewModel)
        {
            _startPosition = e.GetPosition(null);
            _draggedTab = tabItem;
        }
    }

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        if (_draggedTab == null || e.LeftButton != MouseButtonState.Pressed)
            return;

        var currentPosition = e.GetPosition(null);
        var diff = _startPosition - currentPosition;

        // Check if drag threshold exceeded
        if (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
            Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
        {
            _isDragging = true;

            if (_draggedTab.DataContext is FileTabViewModel viewModel)
            {
                // Create drag data
                var data = new DataObject();
                data.SetData(DataFormats.FileDrop, new[] { viewModel.Model.FilePath });
                data.SetData("FileTabViewModel", viewModel);

                // Start drag operation
                var result = DragDrop.DoDragDrop(_draggedTab, data, DragDropEffects.Move | DragDropEffects.Copy);

                // Handle post-drag cleanup
                if (result == DragDropEffects.Move && AllowOutsideDrag)
                {
                    HandleOutsideDrop(viewModel);
                }
            }

            _isDragging = false;
            _draggedTab = null;
        }
    }

    private void OnMouseUp(object sender, MouseButtonEventArgs e)
    {
        _isDragging = false;
        _draggedTab = null;
    }

    private void OnDrop(object sender, DragEventArgs e)
    {
        // Handle drop from another instance or reordering
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            
            // Get MainViewModel from window DataContext
            if (Window.GetWindow(AssociatedObject)?.DataContext is MainViewModel mainViewModel)
            {
                // Open dropped files
                Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    await mainViewModel.AddFilesFromDropAsync(files);
                });
            }
        }
    }

    private void HandleOutsideDrop(FileTabViewModel viewModel)
    {
        // Check if dropped outside window
        var mousePos = Extensions.MouseExtensions.GetScreenPosition();
        var window = Window.GetWindow(AssociatedObject);
        
        if (window == null) return;

        var windowBounds = Extensions.MouseExtensions.GetWindowScreenBounds(window);

        // If dropped outside current window
        if (!windowBounds.Contains(mousePos))
        {
            if (window.DataContext is MainViewModel mainViewModel)
            {
                // Execute drag outside command
                Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    if (mainViewModel.CompleteTabDragOutsideCommand.CanExecute(viewModel))
                    {
                        await mainViewModel.CompleteTabDragOutsideCommand.ExecuteAsync(viewModel);
                    }
                });
            }
        }
    }
}
