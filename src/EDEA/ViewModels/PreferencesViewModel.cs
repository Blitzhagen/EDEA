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
using EDEA.Models;
using EDEA.Properties;
using EDEA.Services;
using EDEA.Windows;
using Microsoft.Xaml.Behaviors;
using log4net;

namespace EDEA.ViewModels;

public class PreferencesViewModel : ViewModelBase
{
    private static readonly ILog log = LogManager.GetLogger(typeof(PreferencesViewModel));

    private static PreferencesWindow? preferencesWindow;

    private ListBox colorElementListBox = null!;

    private SquarePicker squarePicker = null!;

    private ColorSliders colorSliders = null!;

    private HexColorTextBox hexColorTextBox = null!;

    private StackPanel hideOnStackPanel = null!;

    private DispatcherTimer? _colorApplyTimer;
    private string? _pendingColorPropertyName;
    private Color _pendingColor;

    private ListBox speechOutputListBox = null!;

    private TextBox speechOutputTextBox = null!;

    private StackPanel speechOutputPlaceholder = null!;

    private StackPanel speechOutputExamples = null!;

    private Button speechOutputPlayButton = null!;

    private ComboBox speechOutputVoicesComboBox = null!;

    private Slider speechOutputRateSlider = null!;

    private Slider speechOutputVolumeSlider = null!;

    private TextBox valuableGenusThresholdTextBox = null!;

    private TextBox valuableBodyThresholdTextBox = null!;

    private TextBox biologicalsViewAltitudeThresholdTextBox = null!;

    private ComboBox parentPlanetClassificationsComboBox = null!;

    private ListBox planetsOfInterestListBox = null!;

    private TabControl planetsOfInterestTabControl = null!;

    private Popup copyToClipboardPopup = null!;

    private readonly HudViewModel _hudViewModel;

    private readonly PlanetsOfInterestProvider _planetsOfInterestProvider;

    private readonly StarSystemProvider _starSystemProvider;

    private readonly HotkeyProvider _hotkeyProvider;

    private List<PlanetClassification> _planetClassificationClones = null!;

    private string edSavedGamePathOnPreferencesOpen = string.Empty;

    public ObservableCollection<PlanetClassificationViewModel> PlanetClassificationClones => new ObservableCollection<PlanetClassificationViewModel>(_planetClassificationClones.Select((PlanetClassification classification) => new PlanetClassificationViewModel(classification)).ToList());

    public PlanetClassificationViewModel SelectedPlanetClassificationClone => (planetsOfInterestListBox.SelectedItem as PlanetClassificationViewModel)!;

    public Dictionary<string, HotkeyViewModel> Hotkeys => _hotkeyProvider.Hotkeys;

    public bool PreferencesWindowOpen => preferencesWindow != null;

    public bool SpeechOutputReady => !SpeechProvider.IsSpeaking;

    public bool restartRequired { get; set; }

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

    public List<string> SpeechOutputVoices => SpeechProvider.GetVoiceNames();

    public ICommand? CloseWindowCommand { get; private set; }

    public ICommand? SaveAndClosePreferencesCommand { get; private set; }

    public ICommand? RestoreDefaultPreferencesCommand { get; private set; }

    public ICommand? PlaySpeechCommand { get; private set; }

    public ICommand? AddCustomPlanetFilterCommand { get; private set; }

    public ICommand? RenameCustomPlanetFilterCommand { get; private set; }

    public ICommand? RemoveCustomPlanetFilterCommand { get; private set; }

    public ICommand? SelectStringsFromStringListCommand { get; private set; }

    public ICommand? SetJournalFolderCommand { get; private set; }

    public ICommand CopySpeechPlaceholderToClipboardCommand { get; }

    public ICommand? AssignHotkeyCommand { get; private set; }

    public PreferencesViewModel(HudViewModel hudViewModel, PlanetsOfInterestProvider planetsOfInterestProvider, StarSystemProvider starSystemProvider, HotkeyProvider hotkeyProvider)
    {
        _hudViewModel = hudViewModel;
        _planetsOfInterestProvider = planetsOfInterestProvider;
        _starSystemProvider = starSystemProvider;
        _hotkeyProvider = hotkeyProvider;
        CopySpeechPlaceholderToClipboardCommand = new CopyToClipboardCommand();
        setDisplaySize();
    }

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

    public void ShowPreferencesWindow()
    {
        if (preferencesWindow == null)
        {
            preferencesWindow = new PreferencesWindow();
            JotSettingsProvider.Tracker.Track(preferencesWindow);
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

    private void parentPlanetClassificationsComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs? e)
    {
        if (parentPlanetClassificationsComboBox.SelectedItem is TextBlock textBlock && SelectedPlanetClassificationClone != null && (string)textBlock.Tag != SelectedPlanetClassificationClone.Id && SelectedPlanetClassificationClone.ParentPlanetClassificationId != (string)textBlock.Tag)
        {
            SelectedPlanetClassificationClone.ParentPlanetClassificationId = (string)textBlock.Tag;
            log.Debug($"Selected '{textBlock.Text}' ({(string)textBlock.Tag}) as parent criteria set for '{SelectedPlanetClassificationClone.Name}' ({SelectedPlanetClassificationClone.Id})");
        }
    }

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

    private void planetsOfInterestTabControl_SelectionChanged(object? sender, SelectionChangedEventArgs? e)
    {
        Application.Current?.Dispatcher.BeginInvoke((ThreadStart)delegate
        {
            Keyboard.ClearFocus();
        });
    }

    private void thresholdValueTextBox_PreviewTextInput(object? sender, TextCompositionEventArgs e)
    {
        Regex regex = new Regex("[^0-9]+");
        e.Handled = regex.IsMatch(e.Text);
    }

    private void speechProvider_SpeechSynthesizerStateChanged(object? sender, EventArgs e)
    {
        OnPropertyChanged("SpeechOutputReady");
    }

    private void speechProvider_VoicesLoaded(object? sender, EventArgs e)
    {
        OnPropertyChanged("SpeechOutputVoices");
    }

    private void SpeechOutputVoiceParameterChanged(object? sender, EventArgs e)
    {
        SpeechProvider.UpdateSpeechSynthesizerParameter();
    }

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

    private void hideOnCheckboxChanged(object? sender, RoutedEventArgs e)
    {
        _hudViewModel.UpdateVisibilityBasedOnSettings();
    }

    public void SaveAllPreferences()
    {
        _planetsOfInterestProvider.SetPlanetClassifications(_planetClassificationClones);
        Preferences.SaveUserSettings();
        checkRestart();
    }

    public void Cancel()
    {
        Preferences.ReloadUserSettings();
        Helpers.ColorThemeHelper.ApplyCurrentColors();
        generateColorElementListBoxItems();
    }

    public void ResetAllPreferences()
    {
        Preferences.ResetUserSettings();
        Preferences.SaveUserSettings();
        checkRestart();
    }

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

    public void renamePlanetClassification(PlanetClassification planetClassification, string newName)
    {
        planetClassification.Name = newName;
        OnPropertyChanged("PlanetClassificationClones");
        planetsOfInterestListBox.SelectedIndex = _planetClassificationClones.IndexOf(planetClassification);
        planetsOfInterestListBox.Focus();
    }

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

    private void colorPickerControl_ColorChanged(object? sender, RoutedEventArgs e)
    {
        Color color = ((dynamic)sender!).SelectedColor;
        squarePicker.SelectedColor = color;
        colorSliders.SelectedColor = color;
        hexColorTextBox.SelectedColor = color;
        StackPanel? stackPanel = (StackPanel?)colorElementListBox.SelectedItem;
        if (stackPanel != null)
        {
            ((Rectangle)stackPanel.Children[0]).Fill = new SolidColorBrush(color);
            TextBlock textBlock = (TextBlock)stackPanel.Children[1];
            (from x in Preferences.Colors.GetType().GetProperties()
             where x.Name == textBlock.Text
             select x).FirstOrDefault()!.SetValue(Preferences.Colors, color);

            _pendingColorPropertyName = textBlock.Text;
            _pendingColor = color;
            if (_colorApplyTimer == null)
            {
                _colorApplyTimer = new DispatcherTimer(DispatcherPriority.Background, Application.Current.Dispatcher)
                {
                    Interval = TimeSpan.FromMilliseconds(150)
                };
                _colorApplyTimer.Tick += (s, args) =>
                {
                    _colorApplyTimer?.Stop();
                    if (!string.IsNullOrEmpty(_pendingColorPropertyName))
                    {
                        Helpers.ColorThemeHelper.ApplyColor(_pendingColorPropertyName, _pendingColor);
                        _pendingColorPropertyName = null;
                    }
                };
            }
            _colorApplyTimer.Stop();
            _colorApplyTimer.Start();
        }
    }

    private void colorElementListBox_SelectionChanged(object? sender, SelectionChangedEventArgs? e)
    {
        StackPanel stackPanel = (StackPanel)colorElementListBox.SelectedItem;
        if (stackPanel != null)
        {
            Color color = ((SolidColorBrush)((Rectangle)stackPanel.Children[0]).Fill).Color;
            squarePicker.SelectedColor = color;
            colorSliders.SelectedColor = color;
            hexColorTextBox.SelectedColor = color;
        }
    }

    private void generateColorElementListBoxItems()
    {
        colorElementListBox.Items.Clear();
        foreach (PropertyInfo colorProperty in from colorPropertyCandidate in Preferences.Colors.GetType().GetProperties()
                                               where colorPropertyCandidate.PropertyType == typeof(Color)
                                               select colorPropertyCandidate)
        {
            Rectangle rectangle = new Rectangle();
            rectangle.Fill = new SolidColorBrush((Color)colorProperty.GetValue(Preferences.Colors)!);
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

    private void preferencesWindow_Closed(object? sender, EventArgs e)
    {
        Preferences.ReloadUserSettings();
        Helpers.ColorThemeHelper.ApplyCurrentColors();
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

