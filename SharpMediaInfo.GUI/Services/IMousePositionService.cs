namespace SharpMediaInfo.GUI.Services;

/// <summary>
/// Platform-agnostic service for getting mouse position
/// </summary>
public interface IMousePositionService
{
    /// <summary>
    /// Gets the current mouse position in screen coordinates
    /// </summary>
    /// <returns>Tuple with X and Y coordinates in physical pixels</returns>
    (int X, int Y) GetScreenPosition();
}
