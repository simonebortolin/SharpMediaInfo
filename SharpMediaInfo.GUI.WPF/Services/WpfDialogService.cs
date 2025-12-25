using SharpMediaInfo.GUI.Services;
using System.Threading.Tasks;
using System.Windows;

namespace SharpMediaInfo.GUI.WPF.Services;

/// <summary>
/// WPF implementation of dialog service
/// </summary>
public class WpfDialogService : IDialogService
{
    public Task ShowInformationAsync(string message, string title = "Information")
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        return Task.CompletedTask;
    }

    public Task ShowErrorAsync(string message, string title = "Error")
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        return Task.CompletedTask;
    }

    public Task<bool> ShowConfirmationAsync(string message, string title = "Confirmation")
    {
        var result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
        return Task.FromResult(result == MessageBoxResult.Yes);
    }
}
