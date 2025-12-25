using SharpMediaInfo.GUI.Models;
using System.Threading.Tasks;

namespace SharpMediaInfo.GUI.Services;

/// <summary>
/// Service for managing application themes
/// </summary>
public interface IThemeService
{
    /// <summary>
    /// Gets the current theme
    /// </summary>
    AppTheme CurrentTheme { get; }
    
    /// <summary>
    /// Sets the application theme
    /// </summary>
    /// <param name="theme">The theme to apply</param>
    void SetTheme(AppTheme theme);
    
    /// <summary>
    /// Gets the system theme preference
    /// </summary>
    /// <returns>The detected system theme</returns>
    AppTheme GetSystemTheme();
}