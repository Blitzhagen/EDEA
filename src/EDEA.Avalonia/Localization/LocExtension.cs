using System;
using Avalonia.Data;
using Avalonia.Markup.Xaml;

namespace EDEA.Avalonia.Localization;

/// <summary>
/// Markup extension binding to a localized string: <c>{l:SomeResourceKey}</c>.
/// Updates automatically when <see cref="EDEA.Properties.Resources.Culture"/> changes.
/// </summary>
public sealed class LocExtension : MarkupExtension
{
    /// <summary>Initializes a new instance of the <see cref="LocExtension"/> class.</summary>
    /// <param name="key">The resource key.</param>
    public LocExtension(string key)
    {
        Key = key;
    }

    /// <summary>Gets the resource key.</summary>
    /// <value>The resource key.</value>
    public string Key { get; }

    /// <inheritdoc />
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return new Binding
        {
            Source = LocalizedStrings.Instance,
            Path = $"[{Key}]",
            Mode = BindingMode.OneWay,
        };
    }
}
