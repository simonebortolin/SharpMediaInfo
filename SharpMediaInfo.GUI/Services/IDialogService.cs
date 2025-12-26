using System.Threading.Tasks;

namespace SharpMediaInfo.GUI.Services;

/// <summary>
/// Interface for dialog services
/// </summary>
public interface IDialogService
{
    /// <summary>
    /// Shows an information message
    /// </summary>
    Task ShowInformationAsync(string message, string title = "Information");

    /// <summary>
    /// Shows an error message
    /// </summary>
    Task ShowErrorAsync(string message, string title = "Error");

    /// <summary>
    /// Shows a confirmation dialog
    /// </summary>
    Task<bool> ShowConfirmationAsync(string message, string title = "Confirmation");
}
