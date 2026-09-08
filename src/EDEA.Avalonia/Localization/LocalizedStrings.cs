using System.ComponentModel;
using EDEA.Properties;

namespace EDEA.Avalonia.Localization;

/// <summary>
/// Binding source for localized strings that invalidates all indexer bindings
/// when <see cref="Resources.Culture"/> changes.
/// </summary>
public sealed class LocalizedStrings : INotifyPropertyChanged
{
    /// <summary>The shared binding source.</summary>
    public static readonly LocalizedStrings Instance = new();

    private LocalizedStrings()
    {
        Resources.CultureChanged += () =>
        {
            // Avalonia invalidates indexer bindings on "Item" (WPF convention "Item[]" is
            // kept as well so the source also works for WPF-style binding engines).
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
        };
    }

    /// <summary>Gets the localized string for the given resource key.</summary>
    /// <param name="key">The resource key.</param>
    /// <returns>The localized string.</returns>
    public string this[string key] => Resources.Lookup(key);

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;
}
