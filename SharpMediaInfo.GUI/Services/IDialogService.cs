namespace SharpMediaInfo.GUI.Services;

/// <summary>
/// Interface for dialog services
/// </summary>
public interface IDialogService
{
    /// <summary>
    /// Shows an information message
    /// </summary>
    System.Threading.Tasks.Task ShowInformationAsync(string message, string title = "Information");

    /// <summary>
    /// Shows an error message
    /// </summary>
    System.Threading.Tasks.Task ShowErrorAsync(string message, string title = "Error");

    /// <summary>
    /// Shows a confirmation dialog
    /// </summary>
    System.Threading.Tasks.Task<bool> ShowConfirmationAsync(string message, string title = "Confirmation");
}
