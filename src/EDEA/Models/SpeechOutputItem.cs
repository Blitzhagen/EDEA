using System;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;

namespace EDEA.Models;

public partial class SpeechOutputItem : ObservableObject
{
    private readonly UserSettingsSpeech _speech;
    private readonly string _boolProperty;
    private readonly string _textProperty;

    public string Name { get; }
    public string PlaceholderText { get; }
    public string ExampleText { get; }

    [ObservableProperty]
    private bool _isEnabled;

    [ObservableProperty]
    private string _text = string.Empty;

    partial void OnIsEnabledChanged(bool value)
        => _speech.GetType().GetProperty(_boolProperty)?.SetValue(_speech, value);

    partial void OnTextChanged(string value)
        => _speech.GetType().GetProperty(_textProperty)?.SetValue(_speech, value);

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
