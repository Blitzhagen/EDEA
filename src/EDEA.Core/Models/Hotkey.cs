using EDEA.Core.Input;
using EDEA.Enums;

namespace EDEA.Models;

/// <summary>
/// Represents a configurable keyboard hotkey.
/// </summary>
public class Hotkey
{
    /// <summary>
    /// Gets or sets the hotkey identifier.
    /// </summary>
    /// <value>The hotkey identifier.</value>
    public HotkeyId Id { get; set; }

    /// <summary>
    /// Gets or sets the modifier keys.
    /// </summary>
    /// <value>The modifier keys.</value>
    public ModifierKeys Modifier { get; set; }

    /// <summary>
    /// Gets or sets the primary key.
    /// </summary>
    /// <value>The primary key.</value>
    public Key Key { get; set; }

    /// <summary>
    /// Gets a value indicating whether this hotkey is valid.
    /// </summary>
    /// <value><see langword="true"/> if both modifier and key are set; otherwise, <see langword="false"/>.</value>
    public bool IsValid => Modifier != ModifierKeys.None && Key != Key.None;

    /// <summary>
    /// Initializes a new instance of the <see cref="Hotkey"/> class.
    /// </summary>
    /// <param name="id">The hotkey identifier.</param>
    /// <param name="modifier">The modifier keys.</param>
    /// <param name="key">The primary key.</param>
    public Hotkey(HotkeyId id, ModifierKeys modifier, Key key)
    {
        Id = id;
        Modifier = modifier;
        Key = key;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Hotkey"/> class.
    /// </summary>
    public Hotkey()
    {
    }
}
