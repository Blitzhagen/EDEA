using System.Windows.Input;
using EDEA.Enums;

namespace EDEA.Models;

public class Hotkey
{
    public HotkeyId Id { get; set; }
    public ModifierKeys Modifier { get; set; }
    public Key Key { get; set; }

    public bool IsValid => Modifier != ModifierKeys.None && Key != Key.None;

    public Hotkey(HotkeyId id, ModifierKeys modifier, Key key)
    {
        Id = id;
        Modifier = modifier;
        Key = key;
    }

    public Hotkey()
    {
    }
}
