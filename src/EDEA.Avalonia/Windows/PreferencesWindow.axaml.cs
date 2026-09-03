using System;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using EDEA.Services;

namespace EDEA.Avalonia.Windows;

/// <summary>
/// Avalonia window for editing application preferences.
/// </summary>
public partial class PreferencesWindow : Window
{
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
        SpeechWelcomeCheckBox.IsChecked = speech.Welcome;
        SpeechGoodbyeCheckBox.IsChecked = speech.Goodbye;
        SpeechGeoCheckBox.IsChecked = speech.GeologicalSignals;
        SpeechBioCheckBox.IsChecked = speech.BiologicalSignals;
        SpeechFirstDiscoverySystemCheckBox.IsChecked = speech.FirstDiscoverySystem;
        SpeechFirstDiscoveryBodyCheckBox.IsChecked = speech.FirstDiscoveryBody;
        SpeechTerraformableCheckBox.IsChecked = speech.Terraformable;
        SpeechLandableCheckBox.IsChecked = speech.Landable;
        SpeechValuableBodyCheckBox.IsChecked = speech.ValuableBody;
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

    private void SpeechWelcomeCheckBox_IsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        if (SpeechWelcomeCheckBox != null) Preferences.Speech.Welcome = SpeechWelcomeCheckBox.IsChecked == true;
    }

    private void SpeechGoodbyeCheckBox_IsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        if (SpeechGoodbyeCheckBox != null) Preferences.Speech.Goodbye = SpeechGoodbyeCheckBox.IsChecked == true;
    }

    private void SpeechGeoCheckBox_IsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        if (SpeechGeoCheckBox != null) Preferences.Speech.GeologicalSignals = SpeechGeoCheckBox.IsChecked == true;
    }

    private void SpeechBioCheckBox_IsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        if (SpeechBioCheckBox != null) Preferences.Speech.BiologicalSignals = SpeechBioCheckBox.IsChecked == true;
    }

    private void SpeechFirstDiscoverySystemCheckBox_IsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        if (SpeechFirstDiscoverySystemCheckBox != null) Preferences.Speech.FirstDiscoverySystem = SpeechFirstDiscoverySystemCheckBox.IsChecked == true;
    }

    private void SpeechFirstDiscoveryBodyCheckBox_IsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        if (SpeechFirstDiscoveryBodyCheckBox != null) Preferences.Speech.FirstDiscoveryBody = SpeechFirstDiscoveryBodyCheckBox.IsChecked == true;
    }

    private void SpeechTerraformableCheckBox_IsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        if (SpeechTerraformableCheckBox != null) Preferences.Speech.Terraformable = SpeechTerraformableCheckBox.IsChecked == true;
    }

    private void SpeechLandableCheckBox_IsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        if (SpeechLandableCheckBox != null) Preferences.Speech.Landable = SpeechLandableCheckBox.IsChecked == true;
    }

    private void SpeechValuableBodyCheckBox_IsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        if (SpeechValuableBodyCheckBox != null) Preferences.Speech.ValuableBody = SpeechValuableBodyCheckBox.IsChecked == true;
    }

    /// <summary>
    /// Tests the speech output.
    /// </summary>
    private void SpeechTestButton_Click(object? sender, RoutedEventArgs e)
    {
        PlatformServices.Speech?.SpeakPreferencesSelection("Test", System.Array.Empty<EDEA.Models.SpeechOutput>());
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
