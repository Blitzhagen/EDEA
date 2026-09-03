using System;
using Avalonia.Controls;
using Avalonia.Interactivity;

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
    }

    /// <summary>
    /// Applies the selected language.
    /// </summary>
    private void LanguageComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
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
        Preferences.Other.AutomaticTabSwitching = AutomaticTabSwitchingCheckBox.IsChecked == true;
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
