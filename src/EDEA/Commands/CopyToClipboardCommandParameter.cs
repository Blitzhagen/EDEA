using System.Windows.Controls.Primitives;

namespace EDEA.Commands;

public class CopyToClipboardCommandParameter
{
    public string Text { get; }

    public Popup Popup { get; }

    public CopyToClipboardCommandParameter(string text, Popup popup)
    {
        Text = text;
        Popup = popup;
    }
}
