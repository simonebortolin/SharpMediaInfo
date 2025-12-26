using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using SharpMediaInfo.GUI.ViewModels;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace SharpMediaInfo.GUI.Avalonia.Windows;

public partial class MainWindow : Window
{
    private MainViewModel? _viewModel;

    public MainWindow()
    {
        InitializeComponent();
    }

    public MainWindow(MainViewModel viewModel) : this()
    {
        _viewModel = viewModel;
        DataContext = viewModel;
    }

    private void Exit_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}