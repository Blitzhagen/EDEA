using System.Windows.Controls.Primitives;

namespace EDEA.Commands;

/// <summary>
/// Represents the parameters used by <see cref="CopyToClipboardCommand"/>.
/// </summary>
public class CopyToClipboardCommandParameter
{
    /// <summary>
    /// Gets the text to copy to the clipboard.
    /// </summary>
    /// <value>The text to copy.</value>
    public string Text { get; }

    /// <summary>
    /// Gets the popup to display after copying the text.
    /// </summary>
    /// <value>The popup to display.</value>
    public Popup Popup { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CopyToClipboardCommandParameter"/> class.
    /// </summary>
    /// <param name="text">The text to copy.</param>
    /// <param name="popup">The popup to display.</param>
    public CopyToClipboardCommandParameter(string text, Popup popup)
    {
        Text = text;
        Popup = popup;
    }
}
