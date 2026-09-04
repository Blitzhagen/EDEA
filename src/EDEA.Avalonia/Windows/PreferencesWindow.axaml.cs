using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using AvaloniaInput = global::Avalonia.Input;
using EDEA.Models;
using EDEA.Services;

namespace EDEA.Avalonia.Windows;

/// <summary>
/// Represents a configurable speech output item in the preferences window.
/// </summary>
public class SpeechOutputItem
{
    /// <summary>
    /// Gets or sets the output identifier.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display label.
    /// </summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the output is enabled.
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// Gets or sets the speech phrase.
    /// </summary>
    public string Phrase { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the available placeholder text.
    /// </summary>
    public string Placeholders { get; set; } = string.Empty;
}

/// <summary>
/// Avalonia window for editing application preferences.
/// </summary>
public partial class PreferencesWindow : Window
{
    /// <summary>
    /// A flag that prevents re-entrant updates when changing speech output detail controls.
    /// </summary>
    private bool _updatingSpeechOutput;

    /// <summary>
    /// Initializes a new instance of the <see cref="PreferencesWindow"/> class.
    /// </summary>
    public PreferencesWindow()
    {
        InitializeComponent();
        LoadPreferences();
    }

    /// <summary>
    /// Loads the current preferences into the UI controls.
    /// </summary>
    private void LoadPreferences()
    {
        var languages = new[] { "Auto", "en", "de", "es", "fr", "ru", "pt-BR" };
        var language = Preferences.Application.Language;
        var index = Array.IndexOf(languages, language);
        LanguageComboBox.SelectedIndex = index >= 0 ? index : 0;

        var displaySize = Preferences.Other.DisplaySize;
        DisplaySizeSmall.IsChecked = displaySize == 0;
        DisplaySizeStandard.IsChecked = displaySize == 1;
        DisplaySizeLarge.IsChecked = displaySize == 2;
        DisplaySizeHuge.IsChecked = displaySize == 3;

        AutomaticTabSwitchingCheckBox.IsChecked = Preferences.Other.AutomaticTabSwitching;

        EdSavedGamePathTextBox.Text = Preferences.Other.EdSavedGamePath;
        ValuableBodyThresholdTextBox.Text = Preferences.Other.ValuableBodyThreshold.ToString(CultureInfo.CurrentCulture);
        ValuableGenusThresholdTextBox.Text = Preferences.Other.ValuableGenusThreshold.ToString(CultureInfo.CurrentCulture);
        BiologicalsViewAltitudeThresholdTextBox.Text = Preferences.Other.BiologicalsViewAltitudeThreshold.ToString(CultureInfo.CurrentCulture);

        var speech = Preferences.Speech;
        SpeechVoiceComboBox.ItemsSource = new[] { speech.SpeechSynthesizerVoice };
        SpeechVoiceComboBox.SelectedIndex = 0;
        SpeechRateSlider.Value = speech.SpeechSynthesizerRate;
        SpeechVolumeSlider.Value = speech.SpeechSynthesizerVolume;

        var speechType = speech.GetType();
        var items = new List<SpeechOutputItem>();
        foreach (var property in speechType.GetProperties())
        {
            if (property.PropertyType != typeof(bool))
            {
                continue;
            }

            var speechTextProperty = speechType.GetProperty(property.Name + "Speech");
            if (speechTextProperty is null || speechTextProperty.PropertyType != typeof(string))
            {
                continue;
            }

            var value = (bool?)property.GetValue(speech) ?? false;
            var text = (string?)speechTextProperty.GetValue(speech) ?? string.Empty;
            var label = PlatformServices.Speech?.GetLabelForSpeechOutput(property.Name) ?? property.Name;
            var placeholders = System.Text.RegularExpressions.Regex.Matches(text, @"\{(.*?)\}")
                .Select(m => m.Groups[1].Value)
                .Distinct()
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();

            items.Add(new SpeechOutputItem
            {
                Name = property.Name,
                Label = label,
                IsEnabled = value,
                Phrase = text,
                Placeholders = placeholders.Count > 0 ? $"Placeholders: {string.Join(", ", placeholders)}" : string.Empty,
            });
        }

        SpeechOutputListBox.ItemsSource = items;
        if (items.Count > 0)
        {
            SpeechOutputListBox.SelectedIndex = 0;
        }

        HotkeysPanel.Children.Clear();
        var hotkeySettings = Preferences.Hotkeys;
        var hotkeyType = hotkeySettings.GetType();
        foreach (var property in hotkeyType.GetProperties())
        {
            if (property.PropertyType != typeof(EDEA.Models.Hotkey))
            {
                continue;
            }

            var hotkey = (EDEA.Models.Hotkey?)property.GetValue(hotkeySettings);
            if (hotkey == null)
            {
                continue;
            }

            var row = new StackPanel { Orientation = global::Avalonia.Layout.Orientation.Horizontal, Spacing = 8, Margin = new global::Avalonia.Thickness(0, 0, 0, 8) };
            var label = new TextBlock { Text = property.Name, Width = 250, VerticalAlignment = global::Avalonia.Layout.VerticalAlignment.Center, Foreground = new SolidColorBrush(Color.Parse("#FFFFFF")) };
            var textBox = new TextBox { Text = FormatHotkey(hotkey), Width = 200, IsReadOnly = true, Background = new SolidColorBrush(Color.Parse("#222222")), Foreground = new SolidColorBrush(Color.Parse("#FFFFFF")), VerticalAlignment = global::Avalonia.Layout.VerticalAlignment.Center };
            textBox.KeyDown += (s, e) => HotkeyTextBox_KeyDown(s, e, property, textBox);
            row.Children.Add(label);
            row.Children.Add(textBox);
            HotkeysPanel.Children.Add(row);
        }
    }

    /// <summary>
    /// Applies the selected language.
    /// </summary>
    private void LanguageComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (LanguageComboBox == null)
        {
            return;
        }

        var languages = new[] { "Auto", "en", "de", "es", "fr", "ru", "pt-BR" };
        if (LanguageComboBox.SelectedIndex >= 0 && LanguageComboBox.SelectedIndex < languages.Length)
        {
            Preferences.Application.Language = languages[LanguageComboBox.SelectedIndex];
        }
    }

    /// <summary>
    /// Applies the selected display size.
    /// </summary>
    private void DisplaySize_Changed(object? sender, RoutedEventArgs e)
    {
        if (sender is RadioButton { IsChecked: true } radio)
        {
            var displaySize = radio.Name switch
            {
                "DisplaySizeSmall" => 0,
                "DisplaySizeStandard" => 1,
                "DisplaySizeLarge" => 2,
                "DisplaySizeHuge" => 3,
                _ => Preferences.Other.DisplaySize,
            };
            Preferences.Other.DisplaySize = displaySize;
        }
    }

    /// <summary>
    /// Applies the automatic tab switching setting.
    /// </summary>
    private void AutomaticTabSwitching_Changed(object? sender, RoutedEventArgs e)
    {
        if (AutomaticTabSwitchingCheckBox == null)
        {
            return;
        }

        Preferences.Other.AutomaticTabSwitching = AutomaticTabSwitchingCheckBox.IsChecked == true;
    }

    /// <summary>
    /// Applies the selected speech voice.
    /// </summary>
    private void SpeechVoiceComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (SpeechVoiceComboBox?.SelectedItem is string voice)
        {
            Preferences.Speech.SpeechSynthesizerVoice = voice;
        }
    }

    /// <summary>
    /// Applies the speech rate.
    /// </summary>
    private void SpeechRateSlider_ValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        if (SpeechRateSlider != null)
        {
            Preferences.Speech.SpeechSynthesizerRate = SpeechRateSlider.Value;
        }
    }

    /// <summary>
    /// Applies the speech volume.
    /// </summary>
    private void SpeechVolumeSlider_ValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        if (SpeechVolumeSlider != null)
        {
            Preferences.Speech.SpeechSynthesizerVolume = (int)SpeechVolumeSlider.Value;
        }
    }

    /// <summary>
    /// Updates the selected speech output detail controls.
    /// </summary>
    private void SpeechOutputListBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        _updatingSpeechOutput = true;
        if (SpeechOutputListBox?.SelectedItem is SpeechOutputItem item)
        {
            SpeechOutputLabel.Text = item.Label;
            SpeechOutputEnabledCheckBox.IsChecked = item.IsEnabled;
            SpeechOutputPhraseTextBox.Text = item.Phrase;
            SpeechOutputPlaceholdersTextBlock.Text = item.Placeholders;
        }
        _updatingSpeechOutput = false;
    }

    /// <summary>
    /// Applies the enabled state of the selected speech output.
    /// </summary>
    private void SpeechOutputEnabledCheckBox_IsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        if (_updatingSpeechOutput || SpeechOutputListBox?.SelectedItem is not SpeechOutputItem item)
        {
            return;
        }

        item.IsEnabled = SpeechOutputEnabledCheckBox.IsChecked == true;
        var speechType = Preferences.Speech.GetType();
        var property = speechType.GetProperty(item.Name);
        if (property is not null && property.PropertyType == typeof(bool))
        {
            property.SetValue(Preferences.Speech, item.IsEnabled);
        }
    }

    /// <summary>
    /// Applies the phrase of the selected speech output.
    /// </summary>
    private void SpeechOutputPhraseTextBox_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_updatingSpeechOutput || SpeechOutputListBox?.SelectedItem is not SpeechOutputItem item)
        {
            return;
        }

        item.Phrase = SpeechOutputPhraseTextBox.Text ?? string.Empty;
        var speechType = Preferences.Speech.GetType();
        var property = speechType.GetProperty(item.Name + "Speech");
        if (property is not null && property.PropertyType == typeof(string))
        {
            property.SetValue(Preferences.Speech, item.Phrase);
        }
    }

    /// <summary>
    /// Tests the selected speech output.
    /// </summary>
    private void SpeechTestButton_Click(object? sender, RoutedEventArgs e)
    {
        if (SpeechOutputListBox?.SelectedItem is SpeechOutputItem item)
        {
            PlatformServices.Speech?.SpeakPreferencesSelection(item.Phrase, System.Array.Empty<EDEA.Models.SpeechOutput>());
        }
    }

    /// <summary>
    /// Formats a hotkey for display.
    /// </summary>
    private static string FormatHotkey(EDEA.Models.Hotkey hotkey)
    {
        if (!hotkey.IsValid)
        {
            return "-";
        }

        var parts = new List<string>();
        if (hotkey.Modifier.HasFlag(EDEA.Core.Input.ModifierKeys.Alt)) parts.Add("Alt");
        if (hotkey.Modifier.HasFlag(EDEA.Core.Input.ModifierKeys.Control)) parts.Add("Ctrl");
        if (hotkey.Modifier.HasFlag(EDEA.Core.Input.ModifierKeys.Shift)) parts.Add("Shift");
        if (hotkey.Modifier.HasFlag(EDEA.Core.Input.ModifierKeys.Windows)) parts.Add("Win");
        parts.Add(hotkey.Key.ToString());
        return string.Join(" + ", parts);
    }

    /// <summary>
    /// Captures a new hotkey when a key is pressed in a hotkey text box.
    /// </summary>
    private void HotkeyTextBox_KeyDown(object? sender, AvaloniaInput.KeyEventArgs e, PropertyInfo property, TextBox textBox)
    {
        var coreKey = MapKey(e.Key);
        var coreModifier = MapModifiers(e.KeyModifiers);
        if (coreKey == EDEA.Core.Input.Key.None)
        {
            return;
        }

        var hotkey = new EDEA.Models.Hotkey { Modifier = coreModifier, Key = coreKey };
        property.SetValue(Preferences.Hotkeys, hotkey);
        textBox.Text = FormatHotkey(hotkey);
        e.Handled = true;
    }

    /// <summary>
    /// Maps an Avalonia key to the platform-independent key enum.
    /// </summary>
    private static EDEA.Core.Input.Key MapKey(AvaloniaInput.Key key)
    {
        return Enum.TryParse<EDEA.Core.Input.Key>(key.ToString(), out var result) ? result : EDEA.Core.Input.Key.None;
    }

    /// <summary>
    /// Maps Avalonia key modifiers to the platform-independent modifier enum.
    /// </summary>
    private static EDEA.Core.Input.ModifierKeys MapModifiers(AvaloniaInput.KeyModifiers modifiers)
    {
        var result = EDEA.Core.Input.ModifierKeys.None;
        if (modifiers.HasFlag(AvaloniaInput.KeyModifiers.Alt)) result |= EDEA.Core.Input.ModifierKeys.Alt;
        if (modifiers.HasFlag(AvaloniaInput.KeyModifiers.Control)) result |= EDEA.Core.Input.ModifierKeys.Control;
        if (modifiers.HasFlag(AvaloniaInput.KeyModifiers.Shift)) result |= EDEA.Core.Input.ModifierKeys.Shift;
        if (modifiers.HasFlag(AvaloniaInput.KeyModifiers.Meta)) result |= EDEA.Core.Input.ModifierKeys.Windows;
        return result;
    }

    /// <summary>
    /// Opens a folder picker to set the ED saved game path.
    /// </summary>
    private async void SetJournalFolderButton_Click(object? sender, RoutedEventArgs e)
    {
        var options = new FolderPickerOpenOptions
        {
            Title = "Select Elite Dangerous saved games folder",
        };

        var result = await StorageProvider.OpenFolderPickerAsync(options);
        if (result.Count > 0)
        {
            var path = result[0].TryGetLocalPath();
            if (!string.IsNullOrWhiteSpace(path))
            {
                Preferences.Other.EdSavedGamePath = path;
                EdSavedGamePathTextBox.Text = path;
            }
        }
    }

    /// <summary>
    /// Applies the valuable body threshold.
    /// </summary>
    private void ValuableBodyThresholdTextBox_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (int.TryParse(ValuableBodyThresholdTextBox.Text, out var value))
        {
            Preferences.Other.ValuableBodyThreshold = value;
        }
    }

    /// <summary>
    /// Applies the valuable genus threshold.
    /// </summary>
    private void ValuableGenusThresholdTextBox_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (int.TryParse(ValuableGenusThresholdTextBox.Text, out var value))
        {
            Preferences.Other.ValuableGenusThreshold = value;
        }
    }

    /// <summary>
    /// Applies the biologicals view altitude threshold.
    /// </summary>
    private void BiologicalsViewAltitudeThresholdTextBox_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (int.TryParse(BiologicalsViewAltitudeThresholdTextBox.Text, out var value))
        {
            Preferences.Other.BiologicalsViewAltitudeThreshold = value;
        }
    }

    /// <summary>
    /// Restores default preferences.
    /// </summary>
    private void RestoreDefaultButton_Click(object? sender, RoutedEventArgs e)
    {
        Preferences.User.SetDefaultValues();
        LoadPreferences();
    }

    /// <summary>
    /// Saves the preferences and closes the window.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The routed event data.</param>
    private void OkButton_Click(object? sender, RoutedEventArgs e)
    {
        Preferences.SaveUserSettings();
        Close();
    }

    /// <summary>
    /// Closes the window without saving.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The routed event data.</param>
    private void CancelButton_Click(object? sender, RoutedEventArgs e)
    {
        Preferences.ReloadUserSettings();
        Close();
    }
}
