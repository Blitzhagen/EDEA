using System;

namespace EDEA.Core.Input;

/// <summary>
/// Specifies the set of modifier keys.
/// This is a platform-independent replacement for <see cref="System.Windows.Input.ModifierKeys"/>.
/// </summary>
[Flags]
public enum ModifierKeys
{
    /// <summary>No modifier key.</summary>
    None = 0,

    /// <summary>The ALT key.</summary>
    Alt = 1,

    /// <summary>The CTRL key.</summary>
    Control = 2,

    /// <summary>The SHIFT key.</summary>
    Shift = 4,

    /// <summary>The Windows logo key.</summary>
    Windows = 8,
}
