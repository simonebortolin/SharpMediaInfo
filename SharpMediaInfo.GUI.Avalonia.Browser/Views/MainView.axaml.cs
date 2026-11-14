using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using SharpMediaInfo.GUI.Avalonia.Browser.Services;
using SharpMediaInfo.GUI.ViewModels;
using System.Linq;

namespace SharpMediaInfo.GUI.Avalonia.Browser.Views;

public partial class MainView : UserControl
{
    private readonly BrowserFileRegistry? _registry;

    public MainView()
    {
        InitializeComponent();

        AddHandler(DragDrop.DropEvent, OnDrop);
        AddHandler(DragDrop.DragOverEvent, OnDragOver);
    }

    public MainView(MainViewModel viewModel, BrowserFileRegistry registry) : this()
    {
        DataContext = viewModel;
        _registry = registry;
    }

    private async void OnDrop(object? sender, DragEventArgs e)
    {
        var files = e.DataTransfer.TryGetFiles()?.OfType<IStorageFile>().ToArray();
        if (_registry == null || files is not { Length: > 0 } || DataContext is not MainViewModel vm)
            return;

        e.Handled = true;
        await vm.AddFilesFromDropAsync(files.Select(_registry.Register).ToArray());
    }

    private void OnDragOver(object? sender, DragEventArgs e)
    {
        e.DragEffects = e.DataTransfer.Contains(DataFormat.File) ? DragDropEffects.Copy : DragDropEffects.None;
        e.Handled = true;
    }
}
