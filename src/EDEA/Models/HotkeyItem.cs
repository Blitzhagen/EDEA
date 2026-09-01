using System;
using System.Reflection;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using EDEA.Enums;
using EDEA.Properties;

namespace EDEA.Models;

/// <summary>
/// Represents a hotkey entry displayed in the user interface.
/// </summary>
public class HotkeyItem : ObservableObject
{
    /// <summary>
    /// The hotkey settings object this item reads from.
    /// </summary>
    private readonly UserSettingsHotkeys _hotkeys;

    /// <summary>
    /// The name of the settings property this item is bound to.
    /// </summary>
    private readonly string _propertyName;

    /// <summary>
    /// The converter used to format the primary key.
    /// </summary>
    private readonly KeyConverter _keyConverter = new();

    /// <summary>
    /// Gets the underlying hotkey from the settings.
    /// </summary>
    /// <value>The bound hotkey, or <see langword="null"/> if not found.</value>
    private Hotkey? Hotkey => _hotkeys.GetType().GetProperty(_propertyName)?.GetValue(_hotkeys) as Hotkey;

    /// <summary>
    /// Gets the hotkey identifier.
    /// </summary>
    /// <value>The hotkey identifier.</value>
    public HotkeyId Id => Hotkey?.Id ?? default;

    /// <summary>
    /// Gets the description.
    /// </summary>
    /// <value>The hotkey description.</value>
    public string Description { get; }

    /// <summary>
    /// Gets the full key combination as a string.
    /// </summary>
    /// <value>The formatted key combination, or the unassigned label if invalid.</value>
    public string FullKey
    {
        get
        {
            var h = Hotkey;
            if (h is null || !h.IsValid)
                return Resources.Hotkey_Unassigned;
            return h.Modifier.ToString().Replace(", ", "+") + "+" + _keyConverter.ConvertToString(h.Key);
        }
    }

    /// <summary>
    /// Gets a value indicating whether the hotkey is valid.
    /// </summary>
    /// <value><see langword="true"/> if the hotkey is valid; otherwise, <see langword="false"/>.</value>
    public bool IsValid => Hotkey?.IsValid ?? false;

    /// <summary>
    /// Initializes a new instance of the <see cref="HotkeyItem"/> class.
    /// </summary>
    /// <param name="hotkeys">The hotkey settings.</param>
    /// <param name="description">The hotkey description.</param>
    /// <param name="propertyName">The bound settings property name.</param>
    public HotkeyItem(UserSettingsHotkeys hotkeys, string description, string propertyName)
    {
        _hotkeys = hotkeys;
        Description = description;
        _propertyName = propertyName;
    }

    /// <summary>
    /// Refreshes the bound properties.
    /// </summary>
    public void Refresh()
    {
        OnPropertyChanged(nameof(FullKey));
        OnPropertyChanged(nameof(IsValid));
    }
}
