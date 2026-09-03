using EDEA.Core.Input;
using EDEA.Models;
using EDEA.Properties;
using EDEA.Enums;

namespace EDEA.ViewModels;

/// <summary>
/// View model that wraps a <see cref="Hotkey"/> for display and editing.
/// </summary>
public class HotkeyViewModel : ViewModelBase
{
    /// <summary>
    /// The underlying hotkey model.
    /// </summary>
    private readonly Hotkey _hotkey;


    /// <summary>
    /// Gets the hotkey identifier.
    /// </summary>
    /// <value>The hotkey identifier.</value>
    public HotkeyId Id => _hotkey.Id;

    /// <summary>
    /// Gets or sets the modifier keys of the hotkey.
    /// </summary>
    /// <value>The modifier keys.</value>
    public ModifierKeys Modifier
    {
        get
        {
            return _hotkey.Modifier;
        }
        set
        {
            if (_hotkey.Modifier != value)
            {
                _hotkey.Modifier = value;
                OnPropertyChanged("IsValid");
                OnPropertyChanged("FullKey");
            }
        }
    }

    /// <summary>
    /// Gets or sets the key of the hotkey.
    /// </summary>
    /// <value>The key.</value>
    public Key Key
    {
        get
        {
            return _hotkey.Key;
        }
        set
        {
            if (_hotkey.Key != value)
            {
                _hotkey.Key = value;
                OnPropertyChanged("IsValid");
                OnPropertyChanged("FullKey");
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether the hotkey is valid.
    /// </summary>
    /// <value><c>true</c> if the hotkey is valid; otherwise, <c>false</c>.</value>
    public bool IsValid => _hotkey.IsValid;

    /// <summary>
    /// Gets the description of the hotkey.
    /// </summary>
    /// <value>The hotkey description.</value>
    public string Description { get; }

    /// <summary>
    /// Gets the full key combination as a string.
    /// </summary>
    /// <value>The full key combination string.</value>
    public string FullKey
    {
        get
        {
            if (!IsValid)
            {
                return Resources.Hotkey_Unassigned;
            }
            return Modifier.ToString().Replace(", ", "+") + "+" + Key;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HotkeyViewModel"/> class.
    /// </summary>
    /// <param name="hotkey">The hotkey model to wrap.</param>
    public HotkeyViewModel(Hotkey hotkey) : this(hotkey, string.Empty) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="HotkeyViewModel"/> class.
    /// </summary>
    /// <param name="hotkey">The hotkey model to wrap.</param>
    /// <param name="description">The hotkey description.</param>
    public HotkeyViewModel(Hotkey hotkey, string description)
    {
        _hotkey = hotkey;
        Description = description;
    }
}
