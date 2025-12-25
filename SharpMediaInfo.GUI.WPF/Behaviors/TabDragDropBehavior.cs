using Microsoft.Xaml.Behaviors;
using SharpMediaInfo.GUI.ViewModels;
using SharpMediaInfo.GUI.WPF.Extensions;
using System;
using System.Diagnostics;
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
    private FileTabViewModel? _draggedViewModel;
    private bool _wasDraggedOutside;

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
        AssociatedObject.Drop += OnDrop;
        AssociatedObject.AllowDrop = true;
    }

    protected override void OnDetaching()
    {
        AssociatedObject.PreviewMouseLeftButtonDown -= OnMouseDown;
        AssociatedObject.PreviewMouseMove -= OnMouseMove;
        AssociatedObject.Drop -= OnDrop;
        
        // Cleanup drag handlers
        var window = Window.GetWindow(AssociatedObject);
        if (window != null)
        {
            DragDrop.RemoveQueryContinueDragHandler(window, OnQueryContinueDrag);
        }
        
        base.OnDetaching();
    }

    private void OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.Source is TabItem tabItem && tabItem.DataContext is FileTabViewModel viewModel)
        {
            _startPosition = e.GetPosition(null);
            _draggedViewModel = viewModel;
            _wasDraggedOutside = false;
        }
        else
        {
            _draggedViewModel = null;
        }
    }

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        if (_draggedViewModel == null || e.LeftButton != MouseButtonState.Pressed || _isDragging)
            return;

        var currentPosition = e.GetPosition(null);
        var diff = _startPosition - currentPosition;

        // Check if drag threshold exceeded
        if (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
            Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
        {
            StartDrag();
        }
    }

    private void StartDrag()
    {
        if (_draggedViewModel == null) return;

        _isDragging = true;
        _wasDraggedOutside = false;

        try
        {
            var window = Window.GetWindow(AssociatedObject);
            if (window != null && AllowOutsideDrag)
            {
                // Hook QueryContinueDrag to detect when drag goes outside window
                DragDrop.AddQueryContinueDragHandler(window, OnQueryContinueDrag);
            }

            // Create drag data
            var data = new DataObject();
            data.SetData(DataFormats.FileDrop, new[] { _draggedViewModel.Model.FilePath });
            data.SetData("FileTabViewModel", _draggedViewModel);

            // Start drag operation
            var result = DragDrop.DoDragDrop(AssociatedObject, data, DragDropEffects.Move | DragDropEffects.Copy);

            // Handle completion
            if (_wasDraggedOutside && result == DragDropEffects.None)
            {
                // Dragged outside - create new instance
                HandleOutsideDrop();
            }
        }
        finally
        {
            var window = Window.GetWindow(AssociatedObject);
            if (window != null)
            {
                DragDrop.RemoveQueryContinueDragHandler(window, OnQueryContinueDrag);
            }

            _isDragging = false;
            _draggedViewModel = null;
        }
    }

    private void OnQueryContinueDrag(object sender, QueryContinueDragEventArgs e)
    {
        // Check if Escape pressed
        if (e.EscapePressed)
        {
            e.Action = DragAction.Cancel;
            return;
        }

        // Check if still dragging
        if (e.KeyStates.HasFlag(DragDropKeyStates.LeftMouseButton))
        {
            // Check if mouse is outside window
            var window = Window.GetWindow(AssociatedObject);
            if (window != null)
            {
                var mousePos = MouseExtensions.GetScreenPosition();
                var windowBounds = MouseExtensions.GetWindowScreenBounds(window);

                if (!windowBounds.Contains(new Point(mousePos.X, mousePos.Y)))
                {
                    // Mouse is outside window - mark for outside drop
                    _wasDraggedOutside = true;
                    
                    // Continue drag but signal that we'll handle it ourselves
                    e.Action = DragAction.Continue;
                }
                else
                {
                    e.Action = DragAction.Continue;
                }
            }
        }
        else
        {
            // Mouse released - drop
            e.Action = DragAction.Drop;
        }

        e.Handled = true;
    }

    private void HandleOutsideDrop()
    {
        if (_draggedViewModel == null) return;

        var window = Window.GetWindow(AssociatedObject);
        if (window?.DataContext is MainViewModel mainViewModel)
        {
            Debug.WriteLine($"Tab dragged outside window: {_draggedViewModel.Model.FilePath}");

            // Execute command to handle outside drop
            Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                if (mainViewModel.CompleteTabDragOutsideCommand.CanExecute(_draggedViewModel))
                {
                    await mainViewModel.CompleteTabDragOutsideCommand.ExecuteAsync(_draggedViewModel);
                }
            });
        }
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
                Debug.WriteLine($"Files dropped: {string.Join(", ", files)}");

                // Open dropped files
                Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    await mainViewModel.AddFilesFromDropAsync(files);
                });
            }

            e.Handled = true;
        }
    }
}
