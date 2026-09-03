using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using ColorPicker;
using EDEA.Commands;
using EDEA.Core.Drawing;
using EDEA.Models;
using EDEA.Properties;
using EDEA.Services;
using EDEA.Windows;
using Microsoft.Xaml.Behaviors;
using log4net;

namespace EDEA.ViewModels;

/// <summary>
/// View model that manages the preferences window and its settings.
/// </summary>
public class PreferencesViewModel : ViewModelBase
{
    /// <summary>
    /// Logger instance for this class.
    /// </summary>
    private static readonly ILog log = LogManager.GetLogger(typeof(PreferencesViewModel));

    /// <summary>
    /// Holds the singleton instance of the preferences window.
    /// </summary>
    private static PreferencesWindow? preferencesWindow;

    /// <summary>
    /// The color element list box.
    /// </summary>
    private ListBox colorElementListBox = null!;

    /// <summary>
    /// The color picker.
    /// </summary>
    private SquarePicker squarePicker = null!;

    /// <summary>
    /// The color sliders.
    /// </summary>
    private ColorSliders colorSliders = null!;

    /// <summary>
    /// The hex color text box.
    /// </summary>
    private HexColorTextBox hexColorTextBox = null!;

    /// <summary>
    /// The stack panel with hide-on check boxes.
    /// </summary>
    private StackPanel hideOnStackPanel = null!;

    /// <summary>
    /// The timer used to delay color application.
    /// </summary>
    private IUiTimer? _colorApplyTimer;

    /// <summary>
    /// The name of the pending color property.
    /// </summary>
    private string? _pendingColorPropertyName;

    /// <summary>
    /// The pending color value.
    /// </summary>
    private EDEA.Core.Drawing.Color _pendingColor;

    /// <summary>
    /// The speech output list box.
    /// </summary>
    private ListBox speechOutputListBox = null!;

    /// <summary>
    /// The speech output text box.
    /// </summary>
    private TextBox speechOutputTextBox = null!;

    /// <summary>
    /// The placeholder panel for speech output placeholders.
    /// </summary>
    private StackPanel speechOutputPlaceholder = null!;

    /// <summary>
    /// The examples panel for speech outputs.
    /// </summary>
    private StackPanel speechOutputExamples = null!;

    /// <summary>
    /// The play button for speech output previews.
    /// </summary>
    private Button speechOutputPlayButton = null!;

    /// <summary>
    /// The combo box for selecting a speech output voice.
    /// </summary>
    private ComboBox speechOutputVoicesComboBox = null!;

    /// <summary>
    /// The slider for the speech output rate.
    /// </summary>
    private Slider speechOutputRateSlider = null!;

    /// <summary>
    /// The slider for the speech output volume.
    /// </summary>
    private Slider speechOutputVolumeSlider = null!;

    /// <summary>
    /// The text box for the valuable genus threshold.
    /// </summary>
    private TextBox valuableGenusThresholdTextBox = null!;

    /// <summary>
    /// The text box for the valuable body threshold.
    /// </summary>
    private TextBox valuableBodyThresholdTextBox = null!;

    /// <summary>
    /// The text box for the biologicals view altitude threshold.
    /// </summary>
    private TextBox biologicalsViewAltitudeThresholdTextBox = null!;

    /// <summary>
    /// The combo box for selecting a parent planet classification.
    /// </summary>
    private ComboBox parentPlanetClassificationsComboBox = null!;

    /// <summary>
    /// The list box for planets of interest.
    /// </summary>
    private ListBox planetsOfInterestListBox = null!;

    /// <summary>
    /// The tab control for planets of interest.
    /// </summary>
    private TabControl planetsOfInterestTabControl = null!;

    /// <summary>
    /// The popup shown after copying to clipboard.
    /// </summary>
    private Popup copyToClipboardPopup = null!;

    /// <summary>
    /// The HUD view model.
    /// </summary>
    private readonly HudViewModel _hudViewModel;

    /// <summary>
    /// The provider for planets of interest data.
    /// </summary>
    private readonly PlanetsOfInterestProvider _planetsOfInterestProvider;

    /// <summary>
    /// The provider for star system data.
    /// </summary>
    private readonly StarSystemProvider _starSystemProvider;

    /// <summary>
    /// The provider for hotkey data.
    /// </summary>
    private readonly HotkeyProvider _hotkeyProvider;

    /// <summary>
    /// The cloned planet classifications being edited.
    /// </summary>
    private List<PlanetClassification> _planetClassificationClones = null!;

    /// <summary>
    /// The Elite Dangerous saved game path at the time the preferences were opened.
    /// </summary>
    private string edSavedGamePathOnPreferencesOpen = string.Empty;

    /// <summary>
    /// Gets the collection of cloned planet classification view models.
    /// </summary>
    /// <value>The planet classification clones.</value>
    public ObservableCollection<PlanetClassificationViewModel> PlanetClassificationClones => new ObservableCollection<PlanetClassificationViewModel>(_planetClassificationClones.Select((PlanetClassification classification) => new PlanetClassificationViewModel(classification)).ToList());

    /// <summary>
    /// Gets the currently selected planet classification clone.
    /// </summary>
    /// <value>The selected clone.</value>
    public PlanetClassificationViewModel SelectedPlanetClassificationClone => (planetsOfInterestListBox.SelectedItem as PlanetClassificationViewModel)!;

    /// <summary>
    /// Gets the available hotkeys.
    /// </summary>
    /// <value>The hotkey view models.</value>
    public Dictionary<string, HotkeyViewModel> Hotkeys => _hotkeyProvider.Hotkeys;

    /// <summary>
    /// Gets a value indicating whether the preferences window is open.
    /// </summary>
    /// <value><c>true</c> if the preferences window is open; otherwise, <c>false</c>.</value>
    public bool PreferencesWindowOpen => preferencesWindow != null;

    /// <summary>
    /// Gets a value indicating whether the speech output is ready to play.
    /// </summary>
    /// <value><c>true</c> if speech output is ready; otherwise, <c>false</c>.</value>
    public bool SpeechOutputReady => !SpeechProvider.IsSpeaking;

    /// <summary>
    /// Gets or sets a value indicating whether a restart is required.
    /// </summary>
    /// <value><c>true</c> if a restart is required; otherwise, <c>false</c>.</value>
    public bool restartRequired { get; set; }

    /// <summary>
    /// Gets or sets the display size.
    /// </summary>
    /// <value>The display size.</value>
    public DisplaySize DisplaySize
    {
        get
        {
            return (DisplaySize)Preferences.Other.DisplaySize;
        }
        set
        {
            Preferences.Other.DisplaySize = (int)value;
            setDisplaySize();
        }
    }

    /// <summary>
    /// Gets or sets the view model displayed in the HUD window.
    /// </summary>
    /// <value>The HUD window tab view model.</value>
    public HudWindowTabViewModel HudWindowTabViewModel
    {
        get
        {
            return (HudWindowTabViewModel)Preferences.HudWindow.HudWindowTabViewModel;
        }
        set
        {
            Preferences.HudWindow.HudWindowTabViewModel = (int)value;
            _hudViewModel.SetView();
        }
    }

    /// <summary>
    /// Gets the list of available speech output voices.
    /// </summary>
    /// <value>The voice names.</value>
    public List<string> SpeechOutputVoices => SpeechProvider.GetVoiceNames();

    /// <summary>
    /// Gets the command that closes the preferences window.
    /// </summary>
    /// <value>The close command, or <c>null</c>.</value>
    public ICommand? CloseWindowCommand { get; private set; }

    /// <summary>
    /// Gets the command that saves the preferences and closes the window.
    /// </summary>
    /// <value>The save and close command, or <c>null</c>.</value>
    public ICommand? SaveAndClosePreferencesCommand { get; private set; }

    /// <summary>
    /// Gets the command that restores default preferences.
    /// </summary>
    /// <value>The restore defaults command, or <c>null</c>.</value>
    public ICommand? RestoreDefaultPreferencesCommand { get; private set; }

    /// <summary>
    /// Gets the command that plays a speech output preview.
    /// </summary>
    /// <value>The play speech command, or <c>null</c>.</value>
    public ICommand? PlaySpeechCommand { get; private set; }

    /// <summary>
    /// Gets the command that adds a custom planet filter.
    /// </summary>
    /// <value>The add custom planet filter command, or <c>null</c>.</value>
    public ICommand? AddCustomPlanetFilterCommand { get; private set; }

    /// <summary>
    /// Gets the command that renames a custom planet filter.
    /// </summary>
    /// <value>The rename custom planet filter command, or <c>null</c>.</value>
    public ICommand? RenameCustomPlanetFilterCommand { get; private set; }

    /// <summary>
    /// Gets the command that removes a custom planet filter.
    /// </summary>
    /// <value>The remove custom planet filter command, or <c>null</c>.</value>
    public ICommand? RemoveCustomPlanetFilterCommand { get; private set; }

    /// <summary>
    /// Gets the command that opens the string list selection dialog.
    /// </summary>
    /// <value>The select strings command, or <c>null</c>.</value>
    public ICommand? SelectStringsFromStringListCommand { get; private set; }

    /// <summary>
    /// Gets the command that sets the journal folder.
    /// </summary>
    /// <value>The set journal folder command, or <c>null</c>.</value>
    public ICommand? SetJournalFolderCommand { get; private set; }

    /// <summary>
    /// Gets the command that copies a speech placeholder to the clipboard.
    /// </summary>
    /// <value>The copy placeholder command.</value>
    public ICommand CopySpeechPlaceholderToClipboardCommand { get; }

    /// <summary>
    /// Gets the command that assigns a hotkey.
    /// </summary>
    /// <value>The assign hotkey command, or <c>null</c>.</value>
    public ICommand? AssignHotkeyCommand { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PreferencesViewModel"/> class.
    /// </summary>
    /// <param name="hudViewModel">The HUD view model.</param>
    /// <param name="planetsOfInterestProvider">The provider for planets of interest.</param>
    /// <param name="starSystemProvider">The provider for star system data.</param>
    /// <param name="hotkeyProvider">The provider for hotkey data.</param>
    public PreferencesViewModel(HudViewModel hudViewModel, PlanetsOfInterestProvider planetsOfInterestProvider, StarSystemProvider starSystemProvider, HotkeyProvider hotkeyProvider)
    {
        _hudViewModel = hudViewModel;
        _planetsOfInterestProvider = planetsOfInterestProvider;
        _starSystemProvider = starSystemProvider;
        _hotkeyProvider = hotkeyProvider;
        CopySpeechPlaceholderToClipboardCommand = new CopyToClipboardCommand();
        setDisplaySize();
    }

    /// <summary>
    /// Applies the current display size to application resources.
    /// </summary>
    private void setDisplaySize()
    {
        int displaySizeOffset = Preferences.Other.DisplaySize - 2;
        Thickness borderThickness = (Thickness)Application.Current.Resources["TabViewBorderSizes"];
        Thickness inverseBorderThickness = (Thickness)Application.Current.Resources["TabViewInverseBorderSizes"];
        Application.Current.Resources["MainFontSize"] = Convert.ToDouble(12 + displaySizeOffset);
        Application.Current.Resources["HeaderFontSize"] = Convert.ToDouble(14 + displaySizeOffset);
        Application.Current.Resources["SmallFontSize"] = Convert.ToDouble(10 + displaySizeOffset);
        Application.Current.Resources["HistoryFontSize"] = Convert.ToDouble(20 + displaySizeOffset);
        Application.Current.Resources["HistoryTotalValueFontSize"] = Convert.ToDouble(30 + displaySizeOffset);
        Application.Current.Resources["MainIconSize"] = Convert.ToDouble(15 + displaySizeOffset);
        Application.Current.Resources["MediumIconSize"] = Convert.ToDouble(12 + displaySizeOffset);
        Application.Current.Resources["SmallIconSize"] = Convert.ToDouble(7 + displaySizeOffset);
        borderThickness.Top = Convert.ToDouble(21 + displaySizeOffset);
        inverseBorderThickness.Top = Convert.ToDouble(21 + displaySizeOffset);
        Application.Current.Resources["TabViewBorderSizes"] = borderThickness;
        Application.Current.Resources["TabViewInverseBorderSizes"] = inverseBorderThickness;
    }

    /// <summary>
    /// Shows the preferences window or activates it if already open.
    /// </summary>
    public void ShowPreferencesWindow()
    {
        if (preferencesWindow == null)
        {
            preferencesWindow = new PreferencesWindow();
            PlatformServices.WindowState?.Track(preferencesWindow, "PreferencesWindow");
            CloseWindowCommand = new CancelPreferencesCommand(this, preferencesWindow);
            SaveAndClosePreferencesCommand = new SaveAndClosePreferencesCommand(this, preferencesWindow);
            RestoreDefaultPreferencesCommand = new RestoreDefaultPreferencesCommand(this, preferencesWindow);
            PlaySpeechCommand = new PlaySpeechCommand(this);
            AddCustomPlanetFilterCommand = new AddCustomPlanetFilterCommand(this);
            RenameCustomPlanetFilterCommand = new RenameCustomPlanetFilterCommand(this);
            RemoveCustomPlanetFilterCommand = new RemoveCustomPlanetFilterCommand(this);
            SelectStringsFromStringListCommand = new SelectStringsFromStringListCommand(this);
            SetJournalFolderCommand = new SetJournalFolderCommand(this);
            AssignHotkeyCommand = new AssignHotkeyCommand(_hotkeyProvider, preferencesWindow);
            preferencesWindow.DataContext = this;
            copyToClipboardPopup = (Popup)preferencesWindow.FindName("CopyToClipboardPopup");
            colorElementListBox = (ListBox)preferencesWindow.FindName("ColorElementListBox");
            generateColorElementListBoxItems();
            colorElementListBox.SelectionChanged += colorElementListBox_SelectionChanged;
            squarePicker = (SquarePicker)preferencesWindow.FindName("SquarePicker");
            colorSliders = (ColorSliders)preferencesWindow.FindName("ColorSliders");
            hexColorTextBox = (HexColorTextBox)preferencesWindow.FindName("HexColorTextBox");
            squarePicker.ColorChanged += colorPickerControl_ColorChanged;
            colorSliders.ColorChanged += colorPickerControl_ColorChanged;
            hexColorTextBox.ColorChanged += colorPickerControl_ColorChanged;
            hideOnStackPanel = (StackPanel)preferencesWindow.FindName("HideOnStackPanel");
            foreach (object child in hideOnStackPanel.Children)
            {
                if (child.GetType() == typeof(CheckBox))
                {
                    ((CheckBox)child).Checked += hideOnCheckboxChanged;
                    ((CheckBox)child).Unchecked += hideOnCheckboxChanged;
                }
            }
            speechOutputListBox = (ListBox)preferencesWindow.FindName("SpeechOutputListBox");
            generateSpeechOutputListBoxItems();
            speechOutputListBox.SelectionChanged += speechOutputListBox_SelectionChanged;
            speechOutputTextBox = (TextBox)preferencesWindow.FindName("SpeechOutputTextBox");
            speechOutputPlaceholder = (StackPanel)preferencesWindow.FindName("SpeechOutputPlaceholder");
            speechOutputExamples = (StackPanel)preferencesWindow.FindName("SpeechOutputExamples");
            speechOutputPlayButton = (Button)preferencesWindow.FindName("SpeechOutputPlayButton");
            speechOutputVoicesComboBox = (ComboBox)preferencesWindow.FindName("SpeechOutputVoicesComboBox");
            speechOutputRateSlider = (Slider)preferencesWindow.FindName("SpeechOutputRateSlider");
            speechOutputVolumeSlider = (Slider)preferencesWindow.FindName("SpeechOutputVolumeSlider");
            speechOutputVoicesComboBox.SelectionChanged += SpeechOutputVoiceParameterChanged;
            speechOutputRateSlider.ValueChanged += SpeechOutputVoiceParameterChanged;
            speechOutputVolumeSlider.ValueChanged += SpeechOutputVoiceParameterChanged;
            SpeechProvider.SpeechSynthesizerStateChanged += speechProvider_SpeechSynthesizerStateChanged;
            SpeechProvider.VoicesLoaded += speechProvider_VoicesLoaded;
            valuableGenusThresholdTextBox = (TextBox)preferencesWindow.FindName("ValuableGenusThreshold");
            valuableGenusThresholdTextBox.PreviewTextInput += thresholdValueTextBox_PreviewTextInput;
            valuableBodyThresholdTextBox = (TextBox)preferencesWindow.FindName("ValuableBodyThreshold");
            valuableBodyThresholdTextBox.PreviewTextInput += thresholdValueTextBox_PreviewTextInput;
            biologicalsViewAltitudeThresholdTextBox = (TextBox)preferencesWindow.FindName("BiologicalsViewAltitudeThreshold");
            biologicalsViewAltitudeThresholdTextBox.PreviewTextInput += thresholdValueTextBox_PreviewTextInput;
            planetsOfInterestListBox = (ListBox)preferencesWindow.FindName("PlanetsOfInterestListBox");
            planetsOfInterestListBox.SelectionChanged += planetsOfInterestListBox_SelectionChanged;
            _planetClassificationClones = _planetsOfInterestProvider.GetClonedPlanetClassifications();
            planetsOfInterestTabControl = (TabControl)preferencesWindow.FindName("PlanetsOfInterestTabControl");
            planetsOfInterestTabControl.SelectionChanged += planetsOfInterestTabControl_SelectionChanged;
            parentPlanetClassificationsComboBox = (ComboBox)preferencesWindow.FindName("ParentPlanetClassificationsComboBox");
            parentPlanetClassificationsComboBox.SelectedValuePath = "Tag";
            parentPlanetClassificationsComboBox.SelectionChanged += parentPlanetClassificationsComboBox_SelectionChanged;
            preferencesWindow.Closed += preferencesWindow_Closed;
            preferencesWindow.Loaded += preferencesWindow_Loaded;
            edSavedGamePathOnPreferencesOpen = Preferences.Other.EdSavedGamePath;
            preferencesWindow.Show();
        }
        else
        {
            if (!preferencesWindow.IsActive)
            {
                preferencesWindow.Activate();
            }
            if (!preferencesWindow.IsFocused)
            {
                preferencesWindow.Focus();
            }
        }
    }

    /// <summary>
    /// Handles the parent planet classification combo box selection change.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void parentPlanetClassificationsComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs? e)
    {
        if (parentPlanetClassificationsComboBox.SelectedItem is TextBlock textBlock && SelectedPlanetClassificationClone != null && (string)textBlock.Tag != SelectedPlanetClassificationClone.Id && SelectedPlanetClassificationClone.ParentPlanetClassificationId != (string)textBlock.Tag)
        {
            SelectedPlanetClassificationClone.ParentPlanetClassificationId = (string)textBlock.Tag;
            log.Debug($"Selected '{textBlock.Text}' ({(string)textBlock.Tag}) as parent criteria set for '{SelectedPlanetClassificationClone.Name}' ({SelectedPlanetClassificationClone.Id})");
        }
    }

    /// <summary>
    /// Handles the planets of interest list box selection change.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void planetsOfInterestListBox_SelectionChanged(object? sender, SelectionChangedEventArgs? e)
    {
        parentPlanetClassificationsComboBox.Items.Clear();
        parentPlanetClassificationsComboBox.Items.Add(new TextBlock
        {
            Text = Resources.Preferences_ParentPlanetNone,
            Tag = null,
            Style = (Application.Current.Resources["PreferencesComboBoxItemTextBlockNeutral"] as Style)
        });
        foreach (PlanetClassificationViewModel planetClassificationClone in PlanetClassificationClones)
        {
            if (SelectedPlanetClassificationClone == null)
            {
                break;
            }
            if (!(SelectedPlanetClassificationClone.Id == planetClassificationClone.Id))
            {
                TextBlock newItem = new TextBlock
                {
                    Text = planetClassificationClone.Name,
                    Tag = planetClassificationClone.Id
                };
                parentPlanetClassificationsComboBox.Items.Add(newItem);
            }
        }
        if (SelectedPlanetClassificationClone == null || SelectedPlanetClassificationClone.ParentPlanetClassificationId == null)
        {
            parentPlanetClassificationsComboBox.SelectedIndex = 0;
        }
        else
        {
            parentPlanetClassificationsComboBox.SelectedValue = SelectedPlanetClassificationClone.ParentPlanetClassificationId;
        }
    }

    /// <summary>
    /// Handles the planets of interest tab control selection change.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void planetsOfInterestTabControl_SelectionChanged(object? sender, SelectionChangedEventArgs? e)
    {
        Application.Current?.Dispatcher.BeginInvoke((ThreadStart)delegate
        {
            Keyboard.ClearFocus();
        });
    }

    /// <summary>
    /// Handles text input in threshold text boxes by allowing only numeric input.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void thresholdValueTextBox_PreviewTextInput(object? sender, TextCompositionEventArgs e)
    {
        Regex regex = new Regex("[^0-9]+");
        e.Handled = regex.IsMatch(e.Text);
    }

    /// <summary>
    /// Handles speech synthesizer state changes.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void speechProvider_SpeechSynthesizerStateChanged(object? sender, EventArgs e)
    {
        OnPropertyChanged("SpeechOutputReady");
    }

    /// <summary>
    /// Handles the voices loaded event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void speechProvider_VoicesLoaded(object? sender, EventArgs e)
    {
        OnPropertyChanged("SpeechOutputVoices");
    }

    /// <summary>
    /// Handles changes to speech output voice parameters.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void SpeechOutputVoiceParameterChanged(object? sender, EventArgs e)
    {
        SpeechProvider.UpdateSpeechSynthesizerParameter();
    }

    /// <summary>
    /// Handles the <see cref="PreferencesWindow.Loaded"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void preferencesWindow_Loaded(object? sender, RoutedEventArgs e)
    {
        colorElementListBox_SelectionChanged(null, null);
        colorElementListBox.Focus();
        speechOutputListBox_SelectionChanged(null, null);
        speechOutputListBox.Focus();
        if (planetsOfInterestListBox.Items.Count > 0)
        {
            planetsOfInterestListBox.SelectedIndex = 0;
            planetsOfInterestListBox.Focus();
        }
    }

    /// <summary>
    /// Handles changes to the hide-on check boxes.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void hideOnCheckboxChanged(object? sender, RoutedEventArgs e)
    {
        _hudViewModel.UpdateVisibilityBasedOnSettings();
    }

    /// <summary>
    /// Saves all user preferences and applies the planet classifications.
    /// </summary>
    public void SaveAllPreferences()
    {
        _planetsOfInterestProvider.SetPlanetClassifications(_planetClassificationClones);
        Preferences.SaveUserSettings();
        checkRestart();
    }

    /// <summary>
    /// Cancels the preference changes and reloads the saved settings.
    /// </summary>
    public void Cancel()
    {
        Preferences.ReloadUserSettings();
        PlatformServices.ColorTheme?.ApplyCurrentColors();
        generateColorElementListBoxItems();
    }

    /// <summary>
    /// Resets all preferences to their default values.
    /// </summary>
    public void ResetAllPreferences()
    {
        Preferences.ResetUserSettings();
        Preferences.SaveUserSettings();
        checkRestart();
    }

    /// <summary>
    /// Gets the currently selected speech output type.
    /// </summary>
    /// <returns>The selected speech output type, or <c>null</c> if none is selected.</returns>
    public SpeechProvider.SpeechOutputType? GetSelectedSpeechOutput()
    {
        StackPanel stackPanel = (StackPanel)speechOutputListBox.SelectedItem;
        if (stackPanel == null)
        {
            return null;
        }
        string text = ((TextBlock)stackPanel.Children[0]).Text;
        return (SpeechProvider.SpeechOutputType)Enum.Parse(typeof(SpeechProvider.SpeechOutputType), text);
    }

    /// <summary>
    /// Adds a planet classification clone to the list.
    /// </summary>
    /// <param name="planetClassification">The planet classification to add.</param>
    public void addPlanetClassification(PlanetClassification planetClassification)
    {
        _planetClassificationClones.Add(planetClassification);
        int indexOfNewClassification = _planetClassificationClones.IndexOf(planetClassification);
        OnPropertyChanged("PlanetClassificationClones");
        if (planetsOfInterestListBox.Items.Count > indexOfNewClassification)
        {
            planetsOfInterestListBox.SelectedIndex = indexOfNewClassification;
            planetsOfInterestListBox.Focus();
        }
    }

    /// <summary>
    /// Renames a planet classification clone.
    /// </summary>
    /// <param name="planetClassification">The planet classification to rename.</param>
    /// <param name="newName">The new name.</param>
    public void renamePlanetClassification(PlanetClassification planetClassification, string newName)
    {
        planetClassification.Name = newName;
        OnPropertyChanged("PlanetClassificationClones");
        planetsOfInterestListBox.SelectedIndex = _planetClassificationClones.IndexOf(planetClassification);
        planetsOfInterestListBox.Focus();
    }

    /// <summary>
    /// Removes a planet classification clone.
    /// </summary>
    /// <param name="planetClassification">The planet classification to remove.</param>
    public void removePlanetClassification(PlanetClassification planetClassification)
    {
        _planetClassificationClones.Remove(planetClassification);
        OnPropertyChanged("PlanetClassificationClones");
        if (planetsOfInterestListBox.Items.Count > 0)
        {
            planetsOfInterestListBox.SelectedIndex = 0;
            planetsOfInterestListBox.Focus();
        }
    }

    /// <summary>
    /// Handles color changes from the color picker controls.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void colorPickerControl_ColorChanged(object? sender, RoutedEventArgs e)
    {
        System.Windows.Media.Color wpfColor = ((dynamic)sender!).SelectedColor;
        squarePicker.SelectedColor = wpfColor;
        colorSliders.SelectedColor = wpfColor;
        hexColorTextBox.SelectedColor = wpfColor;
        var coreColor = EDEA.Core.Drawing.Color.FromArgb(wpfColor.A, wpfColor.R, wpfColor.G, wpfColor.B);
        StackPanel? stackPanel = (StackPanel?)colorElementListBox.SelectedItem;
        if (stackPanel != null)
        {
            ((Rectangle)stackPanel.Children[0]).Fill = new SolidColorBrush(wpfColor);
            TextBlock textBlock = (TextBlock)stackPanel.Children[1];
            (from x in Preferences.Colors.GetType().GetProperties()
             where x.Name == textBlock.Text
             select x).FirstOrDefault()!.SetValue(Preferences.Colors, coreColor);

            _pendingColorPropertyName = textBlock.Text;
            _pendingColor = coreColor;
            if (_colorApplyTimer == null)
            {
                _colorApplyTimer = PlatformServices.UiTimer?.CreateTimer(TimeSpan.FromMilliseconds(150));
                if (_colorApplyTimer is not null)
                {
                    _colorApplyTimer.Tick += () =>
                    {
                        _colorApplyTimer?.Stop();
                        if (!string.IsNullOrEmpty(_pendingColorPropertyName))
                        {
                            PlatformServices.ColorTheme?.ApplyColor(_pendingColorPropertyName, _pendingColor);
                            _pendingColorPropertyName = null;
                        }
                    };
                }
            }
            _colorApplyTimer?.Stop();
            _colorApplyTimer?.Start();
        }
    }

    /// <summary>
    /// Handles the color element list box selection change.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void colorElementListBox_SelectionChanged(object? sender, SelectionChangedEventArgs? e)
    {
        StackPanel stackPanel = (StackPanel)colorElementListBox.SelectedItem;
        if (stackPanel != null)
        {
            System.Windows.Media.Color color = ((SolidColorBrush)((Rectangle)stackPanel.Children[0]).Fill).Color;
            squarePicker.SelectedColor = color;
            colorSliders.SelectedColor = color;
            hexColorTextBox.SelectedColor = color;
        }
    }

    /// <summary>
    /// Generates the items of the color element list box.
    /// </summary>
    private void generateColorElementListBoxItems()
    {
        colorElementListBox.Items.Clear();
        foreach (PropertyInfo colorProperty in from colorPropertyCandidate in Preferences.Colors.GetType().GetProperties()
                                               where colorPropertyCandidate.PropertyType == typeof(EDEA.Core.Drawing.Color)
                                               select colorPropertyCandidate)
        {
            Rectangle rectangle = new Rectangle();
            EDEA.Core.Drawing.Color coreColor = (EDEA.Core.Drawing.Color)colorProperty.GetValue(Preferences.Colors)!;
            System.Windows.Media.Color wpfColor = System.Windows.Media.Color.FromArgb(coreColor.A, coreColor.R, coreColor.G, coreColor.B);
            rectangle.Fill = new SolidColorBrush(wpfColor);
            rectangle.Width = Application.Current.MainWindow.FontSize;
            rectangle.Height = Application.Current.MainWindow.FontSize;
            rectangle.Margin = new Thickness(0.0, 0.0, 3.0, 0.0);
            TextBlock textBlock = new TextBlock();
            textBlock.Text = colorProperty.Name;
            StackPanel stackPanel = new StackPanel();
            stackPanel.Children.Add(rectangle);
            stackPanel.Children.Add(textBlock);
            stackPanel.Orientation = Orientation.Horizontal;
            colorElementListBox.Items.Add(stackPanel);
        }
        colorElementListBox.SelectedIndex = 0;
    }

    /// <summary>
    /// Handles the speech output list box selection change.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void speechOutputListBox_SelectionChanged(object? sender, SelectionChangedEventArgs? e)
    {
        StackPanel stackPanel = (StackPanel)speechOutputListBox.SelectedItem;
        speechOutputPlaceholder.Children.Clear();
        speechOutputExamples.Children.Clear();
        speechOutputPlayButton.Visibility = Visibility.Visible;
        if (stackPanel == null)
        {
            speechOutputTextBox.Text = string.Empty;
            speechOutputPlayButton.Visibility = Visibility.Hidden;
            return;
        }
        string text = ((TextBlock)stackPanel.Children[0]).Text;
        Binding binding = new Binding();
        binding.Source = Preferences.Speech;
        binding.Path = new PropertyPath(text + "Speech");
        binding.Mode = BindingMode.TwoWay;
        speechOutputTextBox.SetBinding(TextBox.TextProperty, binding);
        foreach (KeyValuePair<string, string> placeholdersFromSpeechOutput in SpeechProvider.GetPlaceholdersFromSpeechOutputs(SpeechProvider.GetExampleSpeechOutputs((SpeechProvider.SpeechOutputType)Enum.Parse(typeof(SpeechProvider.SpeechOutputType), text))))
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = placeholdersFromSpeechOutput.Key;
            ContentControl contentControl = new ContentControl();
            contentControl.Content = textBlock;
            CopyToClipboardCommandParameter commandParameter = new CopyToClipboardCommandParameter(textBlock.Text, copyToClipboardPopup);
            InvokeCommandAction value = new InvokeCommandAction
            {
                Command = CopySpeechPlaceholderToClipboardCommand,
                CommandParameter = commandParameter
            };
            Microsoft.Xaml.Behaviors.EventTrigger eventTrigger = new Microsoft.Xaml.Behaviors.EventTrigger();
            eventTrigger.Actions.Add(value);
            eventTrigger.EventName = "MouseDoubleClick";
            Interaction.GetTriggers(contentControl).Add(eventTrigger);
            speechOutputPlaceholder.Children.Add(contentControl);
            speechOutputExamples.Children.Add(new TextBlock
            {
                Text = placeholdersFromSpeechOutput.Value
            });
        }
    }

    /// <summary>
    /// Generates the items of the speech output list box.
    /// </summary>
    private void generateSpeechOutputListBoxItems()
    {
        speechOutputListBox.Items.Clear();
        try
        {
            string[] speechOutputNames = Enum.GetNames(typeof(SpeechProvider.SpeechOutputType));
            foreach (string speechOutputName in speechOutputNames)
            {
                PropertyInfo property = Preferences.Speech.GetType().GetProperty(speechOutputName)!;
                TextBlock element = new TextBlock
                {
                    Text = property.Name,
                    Visibility = Visibility.Collapsed
                };
                CheckBox checkBox = new CheckBox();
                Binding binding = new Binding
                {
                    Source = Preferences.Speech,
                    Path = new PropertyPath(property.Name),
                    Mode = BindingMode.TwoWay
                };
                checkBox.SetBinding(ToggleButton.IsCheckedProperty, binding);
                checkBox.Margin = new Thickness(0.0, 0.0, 3.0, 0.0);
                TextBlock labelTextBlock = new TextBlock
                {
                    Text = SpeechProvider.GetLabelForSpeechOutput((SpeechProvider.SpeechOutputType)Enum.Parse(typeof(SpeechProvider.SpeechOutputType), speechOutputName))
                };
                StackPanel stackPanel = new StackPanel();
                stackPanel.Children.Add(element);
                stackPanel.Children.Add(checkBox);
                stackPanel.Children.Add(labelTextBlock);
                stackPanel.Orientation = Orientation.Horizontal;
                speechOutputListBox.Items.Add(stackPanel);
            }
            speechOutputListBox.SelectedIndex = 0;
        }
        catch (Exception exception)
        {
            log.Error("Cannot generate speech output ListBoxItems!", exception);
        }
    }

    /// <summary>
    /// Checks whether a restart is required after saving path-related settings.
    /// </summary>
    private void checkRestart()
    {
        if (Preferences.Other.EdSavedGamePath != edSavedGamePathOnPreferencesOpen)
        {
            string caption = "Restart required";
            MessageBoxButton button = MessageBoxButton.OK;
            MessageBoxImage icon = MessageBoxImage.Asterisk;
            if (MessageBox.Show("You have changed settings that require the application to be restarted.\n\nAfter clicking OK, the application is closed. Please restart it afterwards.", caption, button, icon, MessageBoxResult.OK) == MessageBoxResult.OK)
            {
                Application.Current.Shutdown();
            }
        }
    }

    /// <summary>
    /// Handles the <see cref="PreferencesWindow.Closed"/> event and cleans up resources.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void preferencesWindow_Closed(object? sender, EventArgs e)
    {
        Preferences.ReloadUserSettings();
        PlatformServices.ColorTheme?.ApplyCurrentColors();
        _hotkeyProvider.AssignAllHotkeys(reloadFromPreferences: true);
        setDisplaySize();
        _starSystemProvider.triggerGuiDataUpdateEvent();
        preferencesWindow!.Closed -= preferencesWindow_Closed;
        preferencesWindow = null;
        CloseWindowCommand = null;
        SaveAndClosePreferencesCommand = null;
        RestoreDefaultPreferencesCommand = null;
        _planetClassificationClones = null!;
        colorElementListBox.SelectionChanged -= colorElementListBox_SelectionChanged;
        colorElementListBox = null!;
        squarePicker.ColorChanged -= colorPickerControl_ColorChanged;
        squarePicker = null!;
        colorSliders.ColorChanged -= colorPickerControl_ColorChanged;
        colorSliders = null!;
        hexColorTextBox.ColorChanged -= colorPickerControl_ColorChanged;
        hexColorTextBox = null!;
    }
}
