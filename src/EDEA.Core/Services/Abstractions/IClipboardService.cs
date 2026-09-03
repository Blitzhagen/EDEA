namespace EDEA.Services;

/// <summary>
/// Abstraction for clipboard operations.
/// </summary>
public interface IClipboardService
{
    /// <summary>
    /// Sets the specified text on the system clipboard.
    /// </summary>
    /// <param name="text">The text to set.</param>
    void SetText(string text);
}
