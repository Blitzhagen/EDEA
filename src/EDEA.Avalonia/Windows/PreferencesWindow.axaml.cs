using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using AvaloniaInput = global::Avalonia.Input;
using EDEA.Enums;
using EDEA.Models;
using EDEA.Properties;
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
    /// A flag that prevents re-entrant updates when syncing color picker controls.
    /// </summary>
    private bool _syncingColor;

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
        Loaded += OnLoaded;
    }

    /// <summary>
    /// Hooks color picker control value changes and initializes the sync state.
    /// </summary>
    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        SyncColorControls(SelectedColor);

        if (SquarePicker is not null)
        {
            SquarePicker.PropertyChanged += OnSquarePickerPropertyChanged;
        }

        if (ColorSliders is not null)
        {
            ColorSliders.PropertyChanged += OnColorSlidersPropertyChanged;
        }

        if (HexColorTextBox is not null)
        {
            HexColorTextBox.PropertyChanged += OnHexColorTextBoxPropertyChanged;
        }

        if (PlatformServices.Speech is not null)
        {
            PlatformServices.Speech.VoicesLoaded += OnVoicesLoaded;
            Closed += (_, _) => PlatformServices.Speech.VoicesLoaded -= OnVoicesLoaded;
            EDEA.Properties.Resources.CultureChanged += OnCultureChanged;
            Closed += (_, _) => EDEA.Properties.Resources.CultureChanged -= OnCultureChanged;
        }
    }

    /// <summary>
    /// Updates the voice combo box when the available TTS voices have been loaded.
    /// </summary>
    private void OnVoicesLoaded(object? sender, EventArgs e)
    {
        Dispatcher.UIThread.Post(() =>
        {
            var service = PlatformServices.Speech;
            if (service == null || SpeechVoiceComboBox == null)
            {
                return;
            }

            SpeechVoiceComboBox.ItemsSource = service.InstalledVoices;

            var voice = Preferences.Speech.SpeechSynthesizerVoice;
            if (!string.IsNullOrWhiteSpace(voice) && service.InstalledVoices.Contains(voice))
            {
                SpeechVoiceComboBox.SelectedItem = voice;
            }
            else if (service.InstalledVoices.Count > 0)
            {
                SpeechVoiceComboBox.SelectedIndex = 0;
            }
        });
    }

    /// <summary>
    /// Pushes the given color into all color picker controls.
    /// </summary>
    private void SyncColorControls(global::Avalonia.Media.Color color)
    {
        _syncingColor = true;

        if (SquarePicker is not null)
        {
            SquarePicker.SelectedColor = color;
        }

        if (ColorSliders is not null)
        {
            ColorSliders.SelectedColor = color;
        }

        if (HexColorTextBox is not null)
        {
            HexColorTextBox.SelectedColor = color;
        }

        _syncingColor = false;
    }

    /// <summary>
    /// Updates <see cref="SelectedColor"/> when the square picker color changes.
    /// </summary>
    private void OnSquarePickerPropertyChanged(object? sender, global::Avalonia.AvaloniaPropertyChangedEventArgs e)
    {
        if (!_syncingColor && e.Property.Name == "SelectedColor" && e.NewValue is global::Avalonia.Media.Color color)
        {
            SelectedColor = color;
        }
    }

    /// <summary>
    /// Updates <see cref="SelectedColor"/> when the color sliders change.
    /// </summary>
    private void OnColorSlidersPropertyChanged(object? sender, global::Avalonia.AvaloniaPropertyChangedEventArgs e)
    {
        if (!_syncingColor && e.Property.Name == "SelectedColor" && e.NewValue is global::Avalonia.Media.Color color)
        {
            SelectedColor = color;
        }
    }

    /// <summary>
    /// Updates <see cref="SelectedColor"/> when the hex text box color changes.
    /// </summary>
    private void OnHexColorTextBoxPropertyChanged(object? sender, global::Avalonia.AvaloniaPropertyChangedEventArgs e)
    {
        if (!_syncingColor && e.Property.Name == "SelectedColor" && e.NewValue is global::Avalonia.Media.Color color)
        {
            SelectedColor = color;
        }
    }

    /// <summary>
    /// Loads the current preferences into the UI controls.
    /// </summary>
    private void LoadPreferences()
    {
        var displaySize = Preferences.Other.DisplaySize;
        DisplaySizeSmall.IsChecked = displaySize == 1;
        DisplaySizeStandard.IsChecked = displaySize == 2;
        DisplaySizeLarge.IsChecked = displaySize == 3;
        DisplaySizeHuge.IsChecked = displaySize == 4;

        AutomaticTabSwitchingCheckBox.IsChecked = Preferences.Other.AutomaticTabSwitching;

        LanguageComboBox.SelectedIndex = Preferences.Application.Language?.ToLowerInvariant() switch
        {
            "en" => 1,
            "de" => 2,
            "ru" => 3,
            _ => 0,
        };

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
        if (PlatformServices.Speech != null)
        {
            SpeechVoiceComboBox.ItemsSource = PlatformServices.Speech.InstalledVoices;
            SpeechVoiceComboBox.SelectedItem = speech.SpeechSynthesizerVoice;
        }

        SpeechRateSlider.Value = speech.SpeechSynthesizerRate;
        SpeechVolumeSlider.Value = speech.SpeechSynthesizerVolume;

        PopulateSpeechOutputs();

        var hud = Preferences.HudWindow;
        HudOpacitySlider.Value = hud.Opacity;

        HudColumnComboBox.SelectedIndex = 0;
        HudBodiesPanel.IsVisible = true;
        HudRoutePanel.IsVisible = false;
        HudGenusPanel.IsVisible = false;

        HudViewAutoRadioButton.IsChecked = hud.HudWindowTabViewModel == 0;
        HudViewBodiesRadioButton.IsChecked = hud.HudWindowTabViewModel == 1;
        HudViewRouteRadioButton.IsChecked = hud.HudWindowTabViewModel == 2;
        HudViewGenusRadioButton.IsChecked = hud.HudWindowTabViewModel == 3;

        InitializeHudCheckBoxes(HudBodiesPanel);
        InitializeHudCheckBoxes(HudRoutePanel);
        InitializeHudCheckBoxes(HudGenusPanel);
        InitializeHudCheckBoxes(HideOnStackPanel);

        PlanetsOfInterestTabControl.SelectedIndex = 0;
        PlanetClassificationListBox.ItemsSource = new List<PlanetClassification>(Preferences.PlanetsOfInterest.PlanetClassifications);
        if (Preferences.PlanetsOfInterest.PlanetClassifications.Count > 0)
        {
            PlanetClassificationListBox.SelectedIndex = 0;
        }
        else
        {
            PlanetClassificationListBox.SelectedItem = null;
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
                "DisplaySizeSmall" => 1,
                "DisplaySizeStandard" => 2,
                "DisplaySizeLarge" => 3,
                "DisplaySizeHuge" => 4,
                _ => Preferences.Other.DisplaySize,
            };
            Preferences.Other.DisplaySize = displaySize;
            EDEA.Services.PlatformServices.ColorTheme?.ApplyCurrentDisplaySize();
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
    /// Rebuilds the speech output list when the UI language changes so labels,
    /// migrated phrases and examples reflect the new culture.
    /// </summary>
    private void OnCultureChanged()
    {
        if (SpeechOutputListBox == null)
        {
            return;
        }

        var selectedName = (SpeechOutputListBox.SelectedItem as SpeechOutputItem)?.Name;
        PopulateSpeechOutputs();

        var items = SpeechOutputListBox.ItemsSource as List<SpeechOutputItem>;
        if (items != null && items.Count > 0)
        {
            var index = selectedName == null ? 0 : items.FindIndex(i => i.Name == selectedName);
            SpeechOutputListBox.SelectedIndex = index >= 0 ? index : 0;
        }
    }

    /// <summary>
    /// Builds the list of configurable speech outputs from the current settings.
    /// </summary>
    private void PopulateSpeechOutputs()
    {
        var speech = Preferences.Speech;
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

            items.Add(new SpeechOutputItem
            {
                Name = property.Name,
                Label = label,
                IsEnabled = value,
                Phrase = text,
            });
        }

        SpeechOutputListBox.ItemsSource = items;
        if (items.Count > 0)
        {
            SpeechOutputListBox.SelectedIndex = 0;
        }
    }

    /// <summary>
    /// Stores the selected application language; it is applied on the next start.
    /// </summary>
    private void LanguageComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (LanguageComboBox == null)
        {
            return;
        }

        Preferences.Application.Language = LanguageComboBox.SelectedIndex switch
        {
            1 => "en",
            2 => "de",
            3 => "ru",
            _ => "Auto",
        };
        App.ApplyLanguage();
    }

    /// <summary>
    /// Applies the selected speech voice.
    /// </summary>
    private void SpeechVoiceComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (SpeechVoiceComboBox?.SelectedItem is string voice)
        {
            Preferences.Speech.SpeechSynthesizerVoice = voice;
            PlatformServices.Speech?.UpdateSpeechSynthesizerParameter();
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
            PlatformServices.Speech?.UpdateSpeechSynthesizerParameter();
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
            PlatformServices.Speech?.UpdateSpeechSynthesizerParameter();
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
            SpeechOutputEnabledCheckBox.IsChecked = item.IsEnabled;
            SpeechOutputPhraseTextBox.Text = item.Phrase;
            BuildSpeechOutputDetails(item);
        }
        else
        {
            SpeechOutputEnabledCheckBox.IsChecked = false;
            SpeechOutputPhraseTextBox.Text = string.Empty;
            SpeechOutputPlaceholder?.Children.Clear();
            SpeechOutputExamples?.Children.Clear();
        }
        _updatingSpeechOutput = false;
    }

    /// <summary>
    /// Fills the placeholder and example lists for the selected speech output.
    /// </summary>
    private void BuildSpeechOutputDetails(SpeechOutputItem item)
    {
        if (SpeechOutputPlaceholder is null || SpeechOutputExamples is null)
        {
            return;
        }

        SpeechOutputPlaceholder.Children.Clear();
        SpeechOutputExamples.Children.Clear();

        if (PlatformServices.Speech is null)
        {
            return;
        }

        var exampleOutputs = PlatformServices.Speech.GetExampleSpeechOutputs(item.Name);
        foreach (var placeholder in PlatformServices.Speech.GetPlaceholdersFromSpeechOutputs(exampleOutputs))
        {
            SpeechOutputPlaceholder.Children.Add(new TextBlock { Text = placeholder.Key });
            SpeechOutputExamples.Children.Add(new TextBlock { Text = placeholder.Value });
        }
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
        if (SpeechOutputListBox?.SelectedItem is SpeechOutputItem item && PlatformServices.Speech is not null)
        {
            var examples = PlatformServices.Speech.GetExampleSpeechOutputs(item.Name);
            PlatformServices.Speech.SpeakPreferencesSelection(item.Phrase, examples);
        }
    }



    /// <summary>
    /// Opens a dialog to assign a new hotkey combination.
    /// </summary>
    private async void HotkeyTextBlock_Click(object? sender, AvaloniaInput.PointerPressedEventArgs e)
    {
        if (sender is not TextBlock { Tag: string propertyName })
        {
            return;
        }

        var property = Preferences.Hotkeys.GetType().GetProperty(propertyName);
        if (property == null || property.PropertyType != typeof(Hotkey))
        {
            return;
        }

        var currentHotkey = (Hotkey?)property.GetValue(Preferences.Hotkeys);
        var dialog = new HotkeyInputDialog(currentHotkey);
        var result = await dialog.ShowDialog<Hotkey?>(this);
        if (result == null)
        {
            return;
        }

        result.Id = currentHotkey?.Id ?? (HotkeyId)Enum.Parse(typeof(HotkeyId), propertyName);
        property.SetValue(Preferences.Hotkeys, result);

        if (HotkeysListBox.ItemsSource is Dictionary<string, HotkeyItem> items && items.TryGetValue(propertyName, out var item))
        {
            item.Refresh();
        }

        RegisterHotkeys();
    }

    /// <summary>
    /// Registers all valid hotkeys with the global hotkey service.
    /// </summary>
    private static void RegisterHotkeys()
    {
        var service = PlatformServices.GlobalHotkey;
        if (service == null)
        {
            return;
        }

        service.UnregisterAll();
        foreach (var property in Preferences.Hotkeys.GetType().GetProperties())
        {
            if (property.PropertyType != typeof(Hotkey))
            {
                continue;
            }

            var hotkey = (Hotkey?)property.GetValue(Preferences.Hotkeys);
            if (hotkey is { IsValid: true })
            {
                service.Register(hotkey.Id, hotkey.Modifier, hotkey.Key);
            }
        }
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
    /// Applies the selected color to the color setting.
    /// </summary>
    protected override void OnPropertyChanged(global::Avalonia.AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == SelectedColorProperty)
        {
            ApplySelectedColor();

            if (!_syncingColor && change.NewValue is global::Avalonia.Media.Color color)
            {
                SyncColorControls(color);
            }
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

    }

    /// <summary>
    /// Updates the planet classification detail controls.
    /// </summary>
    private void PlanetClassificationListBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        _updatingPlanet = true;
        var item = PlanetClassificationListBox?.SelectedItem as PlanetClassification;
        var hasSelection = item is not null;
        PlanetBasicAttributesPanel.IsEnabled = hasSelection;
        PlanetSurfaceConditionsPanel.IsEnabled = hasSelection;
        PlanetRingRelatedPanel.IsEnabled = hasSelection;
        PlanetParentPlanetPanel.IsEnabled = hasSelection;
        RenamePlanetFilterButton.IsEnabled = hasSelection;
        DeletePlanetFilterButton.IsEnabled = hasSelection;

        UpdatePlanetDetailControls(item);
        PlanetLandableComboBox.SelectedIndex = item?.Landable switch
        {
            false => 1,
            true => 2,
            _ => 0,
        };
        UpdateParentPlanetClassifications(item);
        _updatingPlanet = false;
    }

    /// <summary>
    /// Fills the planet classification detail controls from the selected classification.
    /// </summary>
    /// <param name="item">The selected classification, or <see langword="null"/> to clear the controls.</param>
    private void UpdatePlanetDetailControls(PlanetClassification? item)
    {
        PlanetDistanceMinTextBox.Text = FormatNullableValue(item?.DistanceMin);
        PlanetDistanceMaxTextBox.Text = FormatNullableValue(item?.DistanceMax);
        PlanetRadiusMinTextBox.Text = FormatNullableValue(item?.RadiusMin);
        PlanetRadiusMaxTextBox.Text = FormatNullableValue(item?.RadiusMax);
        PlanetOrbitalInclinationMinTextBox.Text = FormatNullableValue(item?.OrbitalInclinationMin);
        PlanetOrbitalInclinationMaxTextBox.Text = FormatNullableValue(item?.OrbitalInclinationMax);
        PlanetGravityMinTextBox.Text = FormatNullableValue(item?.GravityMin);
        PlanetGravityMaxTextBox.Text = FormatNullableValue(item?.GravityMax);
        PlanetTemperatureMinTextBox.Text = FormatNullableValue(item?.TemperatureMin);
        PlanetTemperatureMaxTextBox.Text = FormatNullableValue(item?.TemperatureMax);
        PlanetRingsTotalWidthMinTextBox.Text = FormatNullableValue(item?.RingsTotalWidthMin);
        PlanetRingsTotalWidthMaxTextBox.Text = FormatNullableValue(item?.RingsTotalWidthMax);
        PlanetRingWidthMinTextBox.Text = FormatNullableValue(item?.RingWidthMin);
        PlanetRingWidthMaxTextBox.Text = FormatNullableValue(item?.RingWidthMax);
        PlanetRingDensityMinTextBox.Text = FormatNullableValue(item?.RingDensityMin);
        PlanetRingDensityMaxTextBox.Text = FormatNullableValue(item?.RingDensityMax);
        PlanetPlanetClassesTextBox.Text = item?.PlanetClassesAsString ?? string.Empty;
        PlanetStarClassesTextBox.Text = item?.StarClassesAsString ?? string.Empty;
        PlanetAtmospheresTextBox.Text = item?.AtmospheresAsString ?? string.Empty;
        PlanetVolcanismsTextBox.Text = item?.VolcanismsAsString ?? string.Empty;
        PlanetRingTypesTextBox.Text = item?.RingTypesAsString ?? string.Empty;
        PlanetRingReserveLevelsTextBox.Text = item?.RingReserveLevelsAsString ?? string.Empty;
    }

    /// <summary>
    /// Formats a nullable value for display in a text box.
    /// </summary>
    /// <param name="value">The value to format.</param>
    /// <returns>The formatted value or an empty string.</returns>
    private static string FormatNullableValue(object? value)
    {
        return value switch
        {
            null => string.Empty,
            IFormattable formattable => formattable.ToString(null, CultureInfo.CurrentCulture),
            _ => value.ToString() ?? string.Empty,
        };
    }

    /// <summary>
    /// Applies a numeric planet classification criterion from the text box identified by its tag.
    /// </summary>
    private void PlanetNumberTextBox_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_updatingPlanet ||
            sender is not TextBox { Tag: string propertyName } textBox ||
            PlanetClassificationListBox?.SelectedItem is not PlanetClassification item)
        {
            return;
        }

        var property = typeof(PlanetClassification).GetProperty(propertyName);
        if (property is null || !property.CanWrite)
        {
            return;
        }

        if (property.PropertyType == typeof(long?))
        {
            property.SetValue(item, long.TryParse(textBox.Text, NumberStyles.Integer, CultureInfo.CurrentCulture, out var longValue) ? longValue : (long?)null);
        }
        else if (property.PropertyType == typeof(double?))
        {
            property.SetValue(item, double.TryParse(textBox.Text, NumberStyles.Float, CultureInfo.CurrentCulture, out var doubleValue) ? doubleValue : (double?)null);
        }
    }

    /// <summary>
    /// Applies the landable setting of the selected planet classification.
    /// </summary>
    private void PlanetLandableComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_updatingPlanet || PlanetClassificationListBox?.SelectedItem is not PlanetClassification item)
        {
            return;
        }
        item.Landable = PlanetLandableComboBox.SelectedIndex switch
        {
            1 => false,
            2 => true,
            _ => null,
        };
    }

    /// <summary>
    /// Opens a dialog to select values for a string list criterion of the selected planet classification.
    /// </summary>
    private async void SelectPlanetStringsButton_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: string key } ||
            PlanetClassificationListBox?.SelectedItem is not PlanetClassification item ||
            !Enum.TryParse<UserSelectableInputStringListsKey>(key, out var listKey) ||
            !Globals.UserSelectableInputStringLists.TryGetValue(listKey, out var allItems))
        {
            return;
        }

        var property = typeof(PlanetClassification).GetProperty(key);
        if (property is null || property.PropertyType != typeof(List<string>) || !property.CanWrite)
        {
            return;
        }

        var selectedItems = property.GetValue(item) as List<string> ?? new List<string>();
        var dialog = new InputStringListDialogWindow("Select", allItems, selectedItems);
        var result = await dialog.ShowDialog<List<string>?>(this);
        if (result is null)
        {
            return;
        }

        property.SetValue(item, result);
        _updatingPlanet = true;
        UpdatePlanetDetailControls(item);
        _updatingPlanet = false;
    }

    /// <summary>
    /// Rebuilds the parent planet classification combo box for the selected classification.
    /// </summary>
    /// <param name="selected">The currently selected classification.</param>
    private void UpdateParentPlanetClassifications(PlanetClassification? selected)
    {
        ParentPlanetClassificationsComboBox.Items.Clear();
        ParentPlanetClassificationsComboBox.Items.Add(new ComboBoxItem
        {
            Content = EDEA.Properties.Resources.Preferences_ParentPlanetNone,
        });
        if (selected is not null)
        {
            foreach (var classification in Preferences.PlanetsOfInterest.PlanetClassifications)
            {
                if (classification.Id != selected.Id)
                {
                    ParentPlanetClassificationsComboBox.Items.Add(new ComboBoxItem
                    {
                        Content = classification.Name,
                        Tag = classification.Id,
                    });
                }
            }
        }

        var selectedIndex = 0;
        if (selected?.ParentPlanetClassificationId is string parentId)
        {
            for (var i = 1; i < ParentPlanetClassificationsComboBox.Items.Count; i++)
            {
                if (ParentPlanetClassificationsComboBox.Items[i] is ComboBoxItem { Tag: string id } && id == parentId)
                {
                    selectedIndex = i;
                    break;
                }
            }
        }
        ParentPlanetClassificationsComboBox.SelectedIndex = selectedIndex;
    }

    /// <summary>
    /// Applies the selected parent planet classification.
    /// </summary>
    private void ParentPlanetClassificationsComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_updatingPlanet || PlanetClassificationListBox?.SelectedItem is not PlanetClassification item)
        {
            return;
        }

        var id = (ParentPlanetClassificationsComboBox.SelectedItem as ComboBoxItem)?.Tag as string;
        if (id != item.Id && item.ParentPlanetClassificationId != id)
        {
            item.ParentPlanetClassificationId = id!;
        }
    }

    /// <summary>
    /// Clears the keyboard focus when switching planet detail tabs.
    /// </summary>
    private void PlanetsOfInterestTabControl_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        TopLevel.GetTopLevel(this)?.FocusManager?.ClearFocus();
    }

    /// <summary>
    /// Adds a new planet classification.
    /// </summary>
    private async void AddPlanetFilterButton_Click(object? sender, RoutedEventArgs e)
    {
        var dialog = new InputStringDialogWindow
        {
            Prompt = "Criteria Set Name",
            Value = $"Criteria Set {Preferences.PlanetsOfInterest.PlanetClassifications.Count + 1}",
        };
        var result = await dialog.ShowDialog<string?>(this);
        if (!string.IsNullOrWhiteSpace(result))
        {
            var item = new PlanetClassification(result, isActive: true);
            Preferences.PlanetsOfInterest.PlanetClassifications.Add(item);
            RefreshPlanetClassificationItems(item);
        }
    }

    /// <summary>
    /// Renames the selected planet classification.
    /// </summary>
    private async void RenamePlanetFilterButton_Click(object? sender, RoutedEventArgs e)
    {
        if (PlanetClassificationListBox?.SelectedItem is not PlanetClassification item)
        {
            return;
        }

        var dialog = new InputStringDialogWindow
        {
            Prompt = "Criteria Set Name",
            Value = item.Name,
        };
        var result = await dialog.ShowDialog<string?>(this);
        if (!string.IsNullOrWhiteSpace(result))
        {
            item.Name = result;
            RefreshPlanetClassificationItems(item);
        }
    }

    /// <summary>
    /// Deletes the selected planet classification.
    /// </summary>
    private void DeletePlanetFilterButton_Click(object? sender, RoutedEventArgs e)
    {
        if (PlanetClassificationListBox?.SelectedItem is not PlanetClassification item)
        {
            return;
        }

        Preferences.PlanetsOfInterest.PlanetClassifications.Remove(item);
        foreach (var dependent in Preferences.PlanetsOfInterest.PlanetClassifications.Where(c => c.ParentPlanetClassificationId == item.Id))
        {
            dependent.ParentPlanetClassificationId = null!;
        }
        RefreshPlanetClassificationItems(Preferences.PlanetsOfInterest.PlanetClassifications.FirstOrDefault());
    }

    /// <summary>
    /// Rebuilds the planet classification list and restores the selection.
    /// </summary>
    /// <param name="selected">The classification to select after rebuilding.</param>
    private void RefreshPlanetClassificationItems(PlanetClassification? selected)
    {
        PlanetClassificationListBox.ItemsSource = new List<PlanetClassification>(Preferences.PlanetsOfInterest.PlanetClassifications);
        PlanetClassificationListBox.SelectedItem = selected;
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
            "GenusTableViewModel" => 3,
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
        EDEA.Services.PlatformServices.ColorTheme?.ApplyCurrentColors();
        EDEA.Services.PlatformServices.ColorTheme?.ApplyCurrentDisplaySize();
        Close();
    }
}
