using System;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;

namespace EDEA.Models;

/// <summary>
/// Represents an editable speech output item bound to user settings.
/// </summary>
public partial class SpeechOutputItem : ObservableObject
{
    /// <summary>
    /// The speech settings object this item reads from.
    /// </summary>
    private readonly UserSettingsSpeech _speech;

    /// <summary>
    /// The name of the boolean settings property.
    /// </summary>
    private readonly string _boolProperty;

    /// <summary>
    /// The name of the text settings property.
    /// </summary>
    private readonly string _textProperty;

    /// <summary>
    /// Gets the display name.
    /// </summary>
    /// <value>The display name.</value>
    public string Name { get; }

    /// <summary>
    /// Gets the placeholder text.
    /// </summary>
    /// <value>The placeholder text.</value>
    public string PlaceholderText { get; }

    /// <summary>
    /// Gets the example text.
    /// </summary>
    /// <value>The example text.</value>
    public string ExampleText { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the item is enabled.
    /// </summary>
    /// <value><see langword="true"/> if enabled; otherwise, <see langword="false"/>.</value>
    [ObservableProperty]
    private bool _isEnabled;

    /// <summary>
    /// Gets or sets the speech text.
    /// </summary>
    /// <value>The speech text.</value>
    [ObservableProperty]
    private string _text = string.Empty;

    /// <summary>
    /// Called when the enabled state changes.
    /// </summary>
    /// <param name="value">The new enabled state.</param>
    partial void OnIsEnabledChanged(bool value)
        => _speech.GetType().GetProperty(_boolProperty)?.SetValue(_speech, value);

    /// <summary>
    /// Called when the text changes.
    /// </summary>
    /// <param name="value">The new text.</param>
    partial void OnTextChanged(string value)
        => _speech.GetType().GetProperty(_textProperty)?.SetValue(_speech, value);

    /// <summary>
    /// Initializes a new instance of the <see cref="SpeechOutputItem"/> class.
    /// </summary>
    /// <param name="speech">The speech settings.</param>
    /// <param name="name">The display name.</param>
    /// <param name="boolProperty">The boolean settings property name.</param>
    /// <param name="textProperty">The text settings property name.</param>
    /// <param name="placeholderText">The placeholder text.</param>
    /// <param name="exampleText">The example text.</param>
    public SpeechOutputItem(UserSettingsSpeech speech, string name, string boolProperty, string textProperty, string placeholderText, string exampleText)
    {
        _speech = speech;
        Name = name;
        _boolProperty = boolProperty;
        _textProperty = textProperty;
        PlaceholderText = placeholderText;
        ExampleText = exampleText;

        var boolProp = _speech.GetType().GetProperty(_boolProperty);
        IsEnabled = (bool)(boolProp?.GetValue(_speech) ?? false);

        var textProp = _speech.GetType().GetProperty(_textProperty);
        Text = textProp?.GetValue(_speech)?.ToString() ?? string.Empty;
    }
}
