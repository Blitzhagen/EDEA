using System.Windows.Input;
using EDEA.Models;

using EDEA.Enums;

namespace EDEA.ViewModels;

public class HotkeyViewModel : ViewModelBase
{
    private readonly Hotkey _hotkey;

    private readonly KeyConverter _keyConverter;

    public HotkeyId Id => _hotkey.Id;

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

    public bool IsValid => _hotkey.IsValid;

    public string Description { get; }

    public string FullKey
    {
        get
        {
            if (!IsValid)
            {
                return "<Unassigned>";
            }
            return Modifier.ToString().Replace(", ", "+") + "+" + _keyConverter.ConvertToString(Key);
        }
    }
    public HotkeyViewModel(Hotkey hotkey) : this(hotkey, string.Empty) { }


    public HotkeyViewModel(Hotkey hotkey, string description)
    {
        _hotkey = hotkey;
        _keyConverter = new KeyConverter();
        Description = description;
    }
}
