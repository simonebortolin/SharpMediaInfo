using SharpMediaInfo.GUI.ViewModels;
using System.ComponentModel;
using System.Windows;
using SharpMediaInfo.GUI.Models;

namespace SharpMediaInfo.GUI.WPF.Windows;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private MainViewModel _viewModel;
    private AppConfiguration _configuration;

    public MainWindow(MainViewModel viewModel)
    {
        // Apply current app theme before UI loads to reduce flash
        this.ThemeMode = Application.Current?.ThemeMode ?? ThemeMode.System;

        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;

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
        if (e.PropertyName == nameof(AppConfiguration.AppTheme))
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
        if (DataContext is MainViewModel viewModel)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                await viewModel.AddFilesFromDropAsync(files);
            }
        }
    }

    private void Window_DragEnter(object sender, DragEventArgs e)
    {
        e.Effects = DragDropEffects.Copy;
    }

    private void Exit_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }
}
