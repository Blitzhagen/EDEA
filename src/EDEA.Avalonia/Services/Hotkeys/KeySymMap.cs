using EDEA.Core.Input;

namespace EDEA.Avalonia.Services.Hotkeys;

/// <summary>
/// Maps the platform-independent <see cref="Key"/> enum to XKB keysym names
/// (as defined by xkbcommon-keysyms.h without the XKB_KEY_ prefix).
/// The same names are used by X11 (<c>XStringToKeysym</c>) and by the
/// XDG GlobalShortcuts portal (<c>preferred_trigger</c>).
/// </summary>
internal static class KeySymMap
{
    /// <summary>
    /// Returns the XKB keysym name for the given key, or null if it cannot be mapped.
    /// </summary>
    public static string? GetKeySymName(Key key)
    {
        if (key >= Key.D0 && key <= Key.D9)
        {
            return ((char)('0' + (key - Key.D0))).ToString();
        }

        if (key >= Key.A && key <= Key.Z)
        {
            return ((char)('a' + (key - Key.A))).ToString();
        }

        if (key >= Key.NumPad0 && key <= Key.NumPad9)
        {
            return $"KP_{key - Key.NumPad0}";
        }

        if (key >= Key.F1 && key <= Key.F12)
        {
            return $"F{key - Key.F1 + 1}";
        }

        return key switch
        {
            Key.Back => "BackSpace",
            Key.Tab => "Tab",
            Key.Enter => "Return",
            Key.Pause => "Pause",
            Key.Escape => "Escape",
            Key.Space => "space",
            Key.PageUp => "Prior",
            Key.PageDown => "Next",
            Key.End => "End",
            Key.Home => "Home",
            Key.Left => "Left",
            Key.Up => "Up",
            Key.Right => "Right",
            Key.Down => "Down",
            Key.Insert => "Insert",
            Key.Delete => "Delete",
            Key.LWin => "Super_L",
            Key.RWin => "Super_R",
            Key.Apps => "Menu",
            Key.Multiply => "KP_Multiply",
            Key.Add => "KP_Add",
            Key.Subtract => "KP_Subtract",
            Key.Decimal => "KP_Decimal",
            Key.Divide => "KP_Divide",
            Key.NumLock => "Num_Lock",
            Key.Scroll => "Scroll_Lock",
            Key.LeftShift => "Shift_L",
            Key.RightShift => "Shift_R",
            Key.LeftCtrl => "Control_L",
            Key.RightCtrl => "Control_R",
            Key.LeftAlt => "Alt_L",
            Key.RightAlt => "Alt_R",
            Key.OemSemicolon => "semicolon",
            Key.OemPlus => "plus",
            Key.OemComma => "comma",
            Key.OemMinus => "minus",
            Key.OemPeriod => "period",
            Key.OemQuestion => "slash",
            Key.OemTilde => "grave",
            Key.OemOpenBrackets => "bracketleft",
            Key.OemPipe => "backslash",
            Key.OemCloseBrackets => "bracketright",
            Key.OemQuotes => "apostrophe",
            Key.OemBackslash => "backslash",
            Key.OemClear => "Clear",
            _ => null,
        };
    }

    /// <summary>
    /// Builds a shortcut trigger string in the XDG shortcuts specification format
    /// (e.g. "CTRL+ALT+h"), or null if the key cannot be mapped.
    /// </summary>
    public static string? GetPortalTrigger(ModifierKeys modifierKeys, Key key)
    {
        var keyName = GetKeySymName(key);
        if (keyName == null)
        {
            return null;
        }

        var parts = new System.Text.StringBuilder();
        if ((modifierKeys & ModifierKeys.Control) != 0)
        {
            parts.Append("CTRL+");
        }

        if ((modifierKeys & ModifierKeys.Alt) != 0)
        {
            parts.Append("ALT+");
        }

        if ((modifierKeys & ModifierKeys.Shift) != 0)
        {
            parts.Append("SHIFT+");
        }

        if ((modifierKeys & ModifierKeys.Windows) != 0)
        {
            parts.Append("LOGO+");
        }

        parts.Append(keyName);
        return parts.ToString();
    }
}
