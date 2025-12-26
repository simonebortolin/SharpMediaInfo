using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using SharpMediaInfo.GUI.Services;
using System.Threading.Tasks;

namespace SharpMediaInfo.GUI.Avalonia.Services;

public class AvaloniaDialogService : IDialogService
{
    public async Task ShowInformationAsync(string message, string title = "Information")
    {
        var msgBox = MessageBoxManager.GetMessageBoxStandard(
            title,
            message,
            ButtonEnum.Ok,
            Icon.Info);
        
        await msgBox.ShowAsync();
    }

    public async Task ShowErrorAsync(string message, string title = "Error")
    {
        var msgBox = MessageBoxManager.GetMessageBoxStandard(
            title,
            message,
            ButtonEnum.Ok,
            Icon.Error);
        
        await msgBox.ShowAsync();
    }

    public async Task<bool> ShowConfirmationAsync(string message, string title = "Confirmation")
    {
        var msgBox = MessageBoxManager.GetMessageBoxStandard(
            title,
            message,
            ButtonEnum.YesNo,
            Icon.Question);
        
        var result = await msgBox.ShowAsync();
        return result == ButtonResult.Yes;
    }
}
