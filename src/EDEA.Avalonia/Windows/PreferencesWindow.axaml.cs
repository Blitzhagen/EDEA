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
using EDEA.Enums;
using EDEA.Models;
using EDEA.Services;

namespace EDEA.Avalonia.Windows;

/// <summary>
/// Represents a color setting item in the preferences window.
/// </summary>
public class ColorItem
{
    /// <summary>
    /// Gets or sets the color property name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the current color as a brush for display.
    /// </summary>
    public global::Avalonia.Media.IBrush Color { get; set; } = new global::Avalonia.Media.SolidColorBrush(global::Avalonia.Media.Colors.Transparent);

    /// <summary>
    /// Gets or sets the raw color value.
    /// </summary>
    public global::Avalonia.Media.Color RawColor { get; set; }

    /// <summary>
    /// Gets or sets the reflection property info.
    /// </summary>
    public PropertyInfo? PropertyInfo { get; set; }
}

/// <summary>
/// Avalonia window for editing application preferences.
/// </summary>

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
    /// Defines the <see cref="SelectedColor"/> property.
    /// </summary>
    public static readonly global::Avalonia.StyledProperty<global::Avalonia.Media.Color> SelectedColorProperty =
        global::Avalonia.AvaloniaProperty.Register<PreferencesWindow, global::Avalonia.Media.Color>(
            nameof(SelectedColor),
            global::Avalonia.Media.Colors.White);

    /// <summary>
    /// A flag that prevents re-entrant updates when changing speech output detail controls.
    /// </summary>
    private bool _updatingSpeechOutput;

    /// <summary>
    /// A flag that prevents re-entrant updates when changing planet classification detail controls.
    /// </summary>
    private bool _updatingPlanet;

    /// <summary>
    /// Gets or sets the currently selected color in the color picker.
    /// </summary>
    public global::Avalonia.Media.Color SelectedColor
    {
        get => GetValue(SelectedColorProperty);
        set => SetValue(SelectedColorProperty, value);
    }

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
        var displaySize = Preferences.Other.DisplaySize;
        DisplaySizeSmall.IsChecked = displaySize == 0;
        DisplaySizeStandard.IsChecked = displaySize == 1;
        DisplaySizeLarge.IsChecked = displaySize == 2;
        DisplaySizeHuge.IsChecked = displaySize == 3;

        AutomaticTabSwitchingCheckBox.IsChecked = Preferences.Other.AutomaticTabSwitching;

        RebuildColorList();
        if (ColorListBox.Items.Count > 0)
        {
            ColorListBox.SelectedIndex = 0;
        }

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

        var hud = Preferences.HudWindow;
        HudOpacitySlider.Value = hud.Opacity;

        HudColumnComboBox.SelectedIndex = 0;
        HudBodiesPanel.IsVisible = true;
        HudRoutePanel.IsVisible = false;
        HudGenusPanel.IsVisible = false;

        HudViewAutoRadioButton.IsChecked = hud.HudWindowTabViewModel == 0;
        HudViewBodiesRadioButton.IsChecked = hud.HudWindowTabViewModel == 1;
        HudViewRouteRadioButton.IsChecked = hud.HudWindowTabViewModel == 2;

        InitializeHudCheckBoxes(HudBodiesPanel);
        InitializeHudCheckBoxes(HudRoutePanel);
        InitializeHudCheckBoxes(HudGenusPanel);
        InitializeHudCheckBoxes(HideOnStackPanel);

        PlanetClassificationListBox.ItemsSource = Preferences.PlanetsOfInterest.PlanetClassifications;
        if (Preferences.PlanetsOfInterest.PlanetClassifications.Count > 0)
        {
            PlanetClassificationListBox.SelectedIndex = 0;
        }

        var hotkeyItems = new Dictionary<string, HotkeyItem>();
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

            var description = GetHotkeyDescription(hotkey.Id);
            hotkeyItems.Add(property.Name, new HotkeyItem(hotkeySettings, description, property.Name));
        }

        HotkeysListBox.ItemsSource = hotkeyItems;
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
    /// Opens a placeholder dialog to assign a new hotkey.
    /// </summary>
    private void HotkeyTextBlock_Click(object? sender, AvaloniaInput.PointerPressedEventArgs e)
    {
        // TODO: Show an input dialog for assigning a new hotkey combination.
    }

    /// <summary>
    /// Returns the localized description for the given hotkey.
    /// </summary>
    /// <param name="id">The hotkey identifier.</param>
    /// <returns>The localized description.</returns>
    private static string GetHotkeyDescription(HotkeyId id)
    {
        if (EDEA.Properties.Resources.Culture.TwoLetterISOLanguageName.Equals("de", StringComparison.OrdinalIgnoreCase))
        {
            return id switch
            {
                HotkeyId.ToggleHudWindow => "HUD-Fenster öffnen/schließen",
                HotkeyId.ToggleHudMousePassThrough => "Maus-Durchgriff im HUD-Fenster ein-/ausschalten",
                HotkeyId.OpenRouteTab => "Route-Tab öffnen",
                HotkeyId.OpenBodiesTab => "Himmelskörper-Tab öffnen",
                HotkeyId.OpenBiologicalsTab => "Biologie-Tab öffnen",
                HotkeyId.OpenSurroundingsTab => "Umgebung-Tab öffnen",
                HotkeyId.OpenHistoryTab => "Historie-Tab öffnen",
                HotkeyId.TryCopyNextSystemToClipboard => "Nächsten Systemnamen der gesperrten/Plotter-Route kopieren",
                HotkeyId.QuitSpeechOutput => "Aktuelle Sprachausgabe abbrechen",
                _ => id.ToString(),
            };
        }

        return id switch
        {
            HotkeyId.ToggleHudWindow => "Open/Close HUD Window",
            HotkeyId.ToggleHudMousePassThrough => "Enable/Disable HUD Window Mouse Pass Through",
            HotkeyId.OpenRouteTab => "Open Route Tab",
            HotkeyId.OpenBodiesTab => "Open Bodies Tab",
            HotkeyId.OpenBiologicalsTab => "Open Biologicals Tab",
            HotkeyId.OpenSurroundingsTab => "Open Surroundings Tab",
            HotkeyId.OpenHistoryTab => "Open History Tab",
            HotkeyId.TryCopyNextSystemToClipboard => "Copy Next System Name In Locked/Plotter Route To Clipboard",
            HotkeyId.QuitSpeechOutput => "Cancel Current Speech Output",
            _ => id.ToString(),
        };
    }

    /// <summary>
    /// Rebuilds the color list items from the current preferences.
    /// </summary>
    private void RebuildColorList()
    {
        ColorListBox.Items.Clear();
        var colors = Preferences.Colors;
        foreach (var property in colors.GetType().GetProperties())
        {
            if (property.PropertyType != typeof(EDEA.Core.Drawing.Color))
            {
                continue;
            }

            var coreColor = (EDEA.Core.Drawing.Color?)property.GetValue(colors) ?? default;
            var avaloniaColor = global::Avalonia.Media.Color.FromArgb(coreColor.A, coreColor.R, coreColor.G, coreColor.B);

            var panel = new StackPanel
            {
                Orientation = global::Avalonia.Layout.Orientation.Horizontal,
                Tag = new ColorItem
                {
                    Name = property.Name,
                    RawColor = avaloniaColor,
                    Color = new SolidColorBrush(avaloniaColor),
                    PropertyInfo = property,
                },
            };
            panel.Children.Add(new global::Avalonia.Controls.Shapes.Rectangle
            {
                Width = 16,
                Height = 16,
                Fill = new SolidColorBrush(avaloniaColor),
                Margin = new global::Avalonia.Thickness(0, 0, 6, 0),
            });
            panel.Children.Add(new TextBlock
            {
                Text = property.Name,
            });
            ColorListBox.Items.Add(panel);
        }
    }

    /// <summary>
    /// Updates the color picker when a color is selected.
    /// </summary>
    private void ColorListBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (ColorListBox?.SelectedItem is StackPanel { Tag: ColorItem item })
        {
            SelectedColor = item.RawColor;
        }
    }

    /// <summary>
    /// Updates the selected color from a manually entered hex value.
    /// </summary>
    private void HexColorTextBox_LostFocus(object? sender, global::Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (HexColorTextBox is null)
        {
            return;
        }

        var text = HexColorTextBox.Text?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        if (!text.StartsWith('#'))
        {
            text = "#" + text;
        }

        if (global::Avalonia.Media.Color.TryParse(text, out var color))
        {
            SelectedColor = color;
        }
        else
        {
            HexColorTextBox.Text = string.Format(CultureInfo.InvariantCulture, "#{0:X2}{1:X2}{2:X2}", SelectedColor.R, SelectedColor.G, SelectedColor.B);
        }
    }

    /// <summary>
    /// Applies the selected color to the color setting.
    /// </summary>
    protected override void OnPropertyChanged(global::Avalonia.AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == SelectedColorProperty)
        {
            ApplySelectedColor();
        }
    }

    /// <summary>
    /// Applies the currently selected color to the active color setting.
    /// </summary>
    private void ApplySelectedColor()
    {
        if (ColorListBox?.SelectedItem is not StackPanel { Tag: ColorItem item } selectedItem || item.PropertyInfo is null)
        {
            return;
        }

        var color = SelectedColor;
        item.RawColor = color;
        item.Color = new SolidColorBrush(color);
        item.PropertyInfo.SetValue(Preferences.Colors, new EDEA.Core.Drawing.Color(color.A, color.R, color.G, color.B));

        // Apply the new color to the running application resources.
        EDEA.Services.PlatformServices.ColorTheme?.ApplyCurrentColors();

        // Update the color swatch in the list item.
        if (selectedItem.Children.Count > 0 && selectedItem.Children[0] is global::Avalonia.Controls.Shapes.Rectangle swatch)
        {
            swatch.Fill = new SolidColorBrush(color);
        }

        if (HexColorTextBox is not null)
        {
            HexColorTextBox.Text = string.Format(CultureInfo.InvariantCulture, "#{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B);
        }
    }

    /// <summary>
    /// Updates the planet classification detail controls.
    /// </summary>
    private void PlanetClassificationListBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        _updatingPlanet = true;
        if (PlanetClassificationListBox?.SelectedItem is PlanetClassification item)
        {
            PlanetDetailPanel.IsEnabled = true;
            PlanetDetailName.Text = item.Name;
            PlanetDetailActiveCheckBox.IsChecked = item.IsActive;
            PlanetDetailNameTextBox.Text = item.Name;
            PlanetDetailPlanetClassesTextBox.Text = item.PlanetClassesAsString;
            PlanetDetailStarClassesTextBox.Text = item.StarClassesAsString;
            PlanetDetailDistanceMinTextBox.Text = item.DistanceMin?.ToString(CultureInfo.CurrentCulture) ?? string.Empty;
            PlanetDetailDistanceMaxTextBox.Text = item.DistanceMax?.ToString(CultureInfo.CurrentCulture) ?? string.Empty;
            PlanetDetailRadiusMinTextBox.Text = item.RadiusMin?.ToString(CultureInfo.CurrentCulture) ?? string.Empty;
            PlanetDetailRadiusMaxTextBox.Text = item.RadiusMax?.ToString(CultureInfo.CurrentCulture) ?? string.Empty;
            PlanetDetailLandableComboBox.SelectedIndex = item.Landable == null ? 0 : item.Landable == false ? 1 : 2;
        }
        else
        {
            PlanetDetailPanel.IsEnabled = false;
        }
        _updatingPlanet = false;
    }

    /// <summary>
    /// Adds a new planet classification.
    /// </summary>
    private void AddPlanetFilterButton_Click(object? sender, RoutedEventArgs e)
    {
        var item = new PlanetClassification("New filter");
        Preferences.PlanetsOfInterest.PlanetClassifications.Add(item);
        PlanetClassificationListBox.ItemsSource = null;
        PlanetClassificationListBox.ItemsSource = Preferences.PlanetsOfInterest.PlanetClassifications;
        PlanetClassificationListBox.SelectedItem = item;
    }

    /// <summary>
    /// Renames the selected planet classification.
    /// </summary>
    private void RenamePlanetFilterButton_Click(object? sender, RoutedEventArgs e)
    {
        if (PlanetClassificationListBox?.SelectedItem is PlanetClassification item)
        {
            PlanetDetailNameTextBox.Focus();
            PlanetDetailNameTextBox.SelectAll();
        }
    }

    /// <summary>
    /// Deletes the selected planet classification.
    /// </summary>
    private void DeletePlanetFilterButton_Click(object? sender, RoutedEventArgs e)
    {
        if (PlanetClassificationListBox?.SelectedItem is PlanetClassification item)
        {
            Preferences.PlanetsOfInterest.PlanetClassifications.Remove(item);
            PlanetClassificationListBox.ItemsSource = null;
            PlanetClassificationListBox.ItemsSource = Preferences.PlanetsOfInterest.PlanetClassifications;
            PlanetClassificationListBox.SelectedIndex = Preferences.PlanetsOfInterest.PlanetClassifications.Count > 0 ? 0 : -1;
        }
    }

    /// <summary>
    /// Applies the active state of the selected planet classification.
    /// </summary>
    private void PlanetDetailActiveCheckBox_IsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        if (_updatingPlanet || PlanetClassificationListBox?.SelectedItem is not PlanetClassification item)
        {
            return;
        }
        item.IsActive = PlanetDetailActiveCheckBox.IsChecked == true;
        PlanetClassificationListBox.ItemsSource = null;
        PlanetClassificationListBox.ItemsSource = Preferences.PlanetsOfInterest.PlanetClassifications;
    }

    /// <summary>
    /// Applies the name of the selected planet classification.
    /// </summary>
    private void PlanetDetailNameTextBox_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_updatingPlanet || PlanetClassificationListBox?.SelectedItem is not PlanetClassification item)
        {
            return;
        }
        item.Name = PlanetDetailNameTextBox.Text ?? string.Empty;
    }

    /// <summary>
    /// Applies the planet classes of the selected planet classification.
    /// </summary>
    private void PlanetDetailPlanetClassesTextBox_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_updatingPlanet || PlanetClassificationListBox?.SelectedItem is not PlanetClassification item)
        {
            return;
        }
        item.PlanetClasses = (PlanetDetailPlanetClassesTextBox.Text ?? string.Empty)
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();
    }

    /// <summary>
    /// Applies the star classes of the selected planet classification.
    /// </summary>
    private void PlanetDetailStarClassesTextBox_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_updatingPlanet || PlanetClassificationListBox?.SelectedItem is not PlanetClassification item)
        {
            return;
        }
        item.StarClasses = (PlanetDetailStarClassesTextBox.Text ?? string.Empty)
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();
    }

    /// <summary>
    /// Applies the distance min of the selected planet classification.
    /// </summary>
    private void PlanetDetailDistanceMinTextBox_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_updatingPlanet || PlanetClassificationListBox?.SelectedItem is not PlanetClassification item)
        {
            return;
        }
        item.DistanceMin = double.TryParse(PlanetDetailDistanceMinTextBox.Text, out var v) ? v : null;
    }

    /// <summary>
    /// Applies the distance max of the selected planet classification.
    /// </summary>
    private void PlanetDetailDistanceMaxTextBox_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_updatingPlanet || PlanetClassificationListBox?.SelectedItem is not PlanetClassification item)
        {
            return;
        }
        item.DistanceMax = double.TryParse(PlanetDetailDistanceMaxTextBox.Text, out var v) ? v : null;
    }

    /// <summary>
    /// Applies the radius min of the selected planet classification.
    /// </summary>
    private void PlanetDetailRadiusMinTextBox_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_updatingPlanet || PlanetClassificationListBox?.SelectedItem is not PlanetClassification item)
        {
            return;
        }
        item.RadiusMin = double.TryParse(PlanetDetailRadiusMinTextBox.Text, out var v) ? v : null;
    }

    /// <summary>
    /// Applies the radius max of the selected planet classification.
    /// </summary>
    private void PlanetDetailRadiusMaxTextBox_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_updatingPlanet || PlanetClassificationListBox?.SelectedItem is not PlanetClassification item)
        {
            return;
        }
        item.RadiusMax = double.TryParse(PlanetDetailRadiusMaxTextBox.Text, out var v) ? v : null;
    }

    /// <summary>
    /// Applies the landable setting of the selected planet classification.
    /// </summary>
    private void PlanetDetailLandableComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_updatingPlanet || PlanetClassificationListBox?.SelectedItem is not PlanetClassification item)
        {
            return;
        }
        item.Landable = PlanetDetailLandableComboBox.SelectedIndex switch
        {
            1 => false,
            2 => true,
            _ => null,
        };
    }

    /// <summary>
    /// Applies the HUD opacity.
    /// </summary>
    private void HudOpacitySlider_ValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        if (HudOpacitySlider != null)
        {
            Preferences.HudWindow.Opacity = HudOpacitySlider.Value;
        }
    }

    /// <summary>
    /// Initializes all HUD check boxes in the specified panel from the current preferences.
    /// </summary>
    private void InitializeHudCheckBoxes(StackPanel? panel)
    {
        if (panel is null)
        {
            return;
        }

        var hud = Preferences.HudWindow;
        foreach (var child in panel.Children)
        {
            if (child is not CheckBox checkBox || checkBox.Tag is not string propertyName)
            {
                continue;
            }

            var property = hud.GetType().GetProperty(propertyName);
            if (property is not null && property.PropertyType == typeof(bool))
            {
                checkBox.IsChecked = (bool?)property.GetValue(hud) ?? false;
            }
        }
    }

    /// <summary>
    /// Applies a HUD boolean setting.
    /// </summary>
    private void HudWindowCheckBox_Changed(object? sender, RoutedEventArgs e)
    {
        if (sender is not CheckBox { Tag: string propertyName } checkBox)
        {
            return;
        }

        var property = Preferences.HudWindow.GetType().GetProperty(propertyName);
        if (property is not null && property.PropertyType == typeof(bool))
        {
            property.SetValue(Preferences.HudWindow, checkBox.IsChecked == true);
        }
    }

    /// <summary>
    /// Shows the column check box panel that matches the selected combo box item.
    /// </summary>
    private void HudColumnComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (HudBodiesPanel is null || HudRoutePanel is null || HudGenusPanel is null)
        {
            return;
        }

        HudBodiesPanel.IsVisible = HudColumnComboBox.SelectedIndex == 0;
        HudRoutePanel.IsVisible = HudColumnComboBox.SelectedIndex == 1;
        HudGenusPanel.IsVisible = HudColumnComboBox.SelectedIndex == 2;
    }

    /// <summary>
    /// Applies the selected HUD tab view model.
    /// </summary>
    private void HudViewRadioButton_Changed(object? sender, RoutedEventArgs e)
    {
        if (sender is not RadioButton { IsChecked: true, Tag: string view } radio)
        {
            return;
        }

        Preferences.HudWindow.HudWindowTabViewModel = view switch
        {
            "Auto" => 0,
            "BodyTableViewModel" => 1,
            "NavRouteTableViewModel" => 2,
            _ => Preferences.HudWindow.HudWindowTabViewModel,
        };
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
