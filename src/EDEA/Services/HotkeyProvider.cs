using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Interop;
using EDEA.Enums;
using EDEA.Models;
using EDEA.Properties;
using EDEA.Services.Platform;
using EDEA.ViewModels;
using log4net;

namespace EDEA.Services;

/// <summary>Represents the HotkeyProvider class.</summary>
public class HotkeyProvider : ViewModelBase
{
    /// <summary>
    /// Returns the localized description for the given hotkey.
    /// </summary>
    /// <param name="id">The hotkey identifier.</param>
    /// <returns>The localized description.</returns>
    public static string GetHotkeyDescription(HotkeyId id)
    {
        if (Resources.Culture.TwoLetterISOLanguageName.Equals("de", StringComparison.OrdinalIgnoreCase))
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
                _ => id.ToString()
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
            _ => id.ToString()
        };
    }

    /// <summary>The log field.</summary>
    private static readonly ILog log = LogManager.GetLogger(typeof(HotkeyProvider));

    /// <summary>The instance field.</summary>
    private static HotkeyProvider? instance;

    /// <summary>The mainViewModel field.</summary>
    private MainViewModel? mainViewModel;

    /// <summary>The mainWindow field.</summary>
    private Window? mainWindow;

    /// <summary>The global hotkey service.</summary>
    private readonly IGlobalHotkeyService _globalHotkeyService;

    /// <summary>The _starSystemProvider field.</summary>
    private readonly StarSystemProvider _starSystemProvider;

    /// <summary>Gets or sets the Hotkeys.</summary>
    /// <value>A Dictionary<string, HotkeyViewModel> value.</value>
    public Dictionary<string, HotkeyViewModel> Hotkeys { get; private set; }

    /// <summary>Initializes a new instance of the HotkeyProvider class.</summary>
    /// <param name="starSystemProvider">The StarSystemProvider value of the starSystemProvider parameter.</param>
    /// <param name="globalHotkeyService">The global hotkey service.</param>
    private HotkeyProvider(StarSystemProvider starSystemProvider, IGlobalHotkeyService globalHotkeyService)
    {
        _starSystemProvider = starSystemProvider;
        _globalHotkeyService = globalHotkeyService;
        _globalHotkeyService.HotkeyPressed += OnHotkeyPressed;
        Hotkeys = new Dictionary<string, HotkeyViewModel>();
        setHotkeysFromPreferences();
    }

    /// <summary>Performs the Instance operation.</summary>
    /// <param name="starSystemProvider">The StarSystemProvider value of the starSystemProvider parameter.</param>
    /// <param name="globalHotkeyService">The global hotkey service.</param>
    /// <returns>A HotkeyProvider result.</returns>
    public static HotkeyProvider Instance(StarSystemProvider starSystemProvider, IGlobalHotkeyService globalHotkeyService)
    {
        if (instance == null)
        {
            instance = new HotkeyProvider(starSystemProvider, globalHotkeyService);
        }
        return instance;
    }

    /// <summary>Performs the AttachHotkeyListener operation.</summary>
    /// <param name="mainViewModel">The MainViewModel value of the mainViewModel parameter.</param>
    /// <param name="mainWindow">The Window value of the mainWindow parameter.</param>
    public void AttachHotkeyListener(MainViewModel mainViewModel, Window mainWindow)
    {
        this.mainViewModel = mainViewModel;
        this.mainWindow = mainWindow;
        mainWindow.Loaded += MainWindow_Loaded;
        mainWindow.Closed += MainWindow_Closed;

        if (mainWindow.IsLoaded)
        {
            MainWindow_Loaded(mainWindow, new RoutedEventArgs());
        }
    }

    /// <summary>Sets HotKey.</summary>
    /// <param name="hotkeyViewModel">The HotkeyViewModel value of the hotkeyViewModel parameter.</param>
    public void AssignHotKey(HotkeyViewModel hotkeyViewModel)
    {
        if (!hotkeyViewModel.IsValid)
        {
            return;
        }
        try
        {
            _globalHotkeyService.Register(hotkeyViewModel.Id, hotkeyViewModel.Modifier, hotkeyViewModel.Key);
            log.Debug($"Assigned hotkey {hotkeyViewModel.FullKey} for '{hotkeyViewModel.Description}' ({hotkeyViewModel.Id}) ");
        }
        catch (Exception exception)
        {
            log.Error($"Could not set hotkey with id {hotkeyViewModel.Id}, modifier {hotkeyViewModel.Modifier} and key {hotkeyViewModel.Key}", exception);
        }
    }

    /// <summary>Performs the UnassignHotKey operation.</summary>
    /// <param name="hotkeyViewModel">The HotkeyViewModel value of the hotkeyViewModel parameter.</param>
    public void UnassignHotKey(HotkeyViewModel hotkeyViewModel)
    {
        try
        {
            _globalHotkeyService.Unregister(hotkeyViewModel.Id);
        }
        catch (Exception exception)
        {
            log.Error($"Could unnot set hotkey with id {hotkeyViewModel.Id}, modifier {hotkeyViewModel.Modifier} and key {hotkeyViewModel.Key}", exception);
        }
    }

    /// <summary>Sets AllHotkeys.</summary>
    /// <param name="reloadFromPreferences">The bool value of the reloadFromPreferences parameter.</param>
    public void AssignAllHotkeys(bool reloadFromPreferences = false)
    {
        if (reloadFromPreferences)
        {
            setHotkeysFromPreferences();
        }
        foreach (HotkeyViewModel hotkey in Hotkeys.Values)
        {
            AssignHotKey(hotkey);
        }
    }

    /// <summary>Performs the UnassignAllHotkeys operation.</summary>
    public void UnassignAllHotkeys()
    {
        _globalHotkeyService.UnregisterAll();
    }

    /// <summary>Performs the setHotkeysFromPreferences operation.</summary>
    private void setHotkeysFromPreferences()
    {
        Hotkeys = new Dictionary<string, HotkeyViewModel>();
        try
        {
            PropertyInfo[] properties = Preferences.Hotkeys.GetType().GetProperties();
            foreach (PropertyInfo propertyInfo in properties)
            {
                if (propertyInfo.PropertyType != typeof(Hotkey))
                {
                    continue;
                }
                Hotkey? hotkey = (Hotkey?)propertyInfo.GetValue(Preferences.Hotkeys);
                if (hotkey == null)
                {
                    continue;
                }
                string? key = Enum.GetName(typeof(HotkeyId), hotkey.Id);
                if (string.IsNullOrEmpty(key))
                {
                    continue;
                }
                Hotkeys.Add(key, new HotkeyViewModel(hotkey, GetHotkeyDescription(hotkey.Id)));
            }
        }
        catch (Exception exception)
        {
            log.Error("Could not initialize hotkeys, hotkey will not work correctly!", exception);
        }
        OnPropertyChanged(nameof(Hotkeys));
    }

    /// <summary>Performs the MainWindow_Loaded operation.</summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void MainWindow_Loaded(object? sender, RoutedEventArgs e)
    {
        try
        {
            nint mainWindowHandle = new WindowInteropHelper(mainWindow).Handle;
            if (mainWindowHandle == 0)
            {
                log.Error("Error on setting hotkeys from preferences, can not get handle for main window. Hotkeys will not work!");
                return;
            }
            _globalHotkeyService.Attach(mainWindowHandle);
            AssignAllHotkeys();
        }
        catch (Exception exception)
        {
            log.Error("Generic error on setting hotkeys from preferences. Hotkeys will not work!", exception);
        }
    }

    /// <summary>Performs the MainWindow_Closed operation.</summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void MainWindow_Closed(object? sender, EventArgs e)
    {
        try
        {
            _globalHotkeyService.Detach();
        }
        catch (Exception exception)
        {
            log.Error("Generic error on unsetting hotkeys", exception);
        }
    }

    /// <summary>
    /// Called when a registered global hotkey is pressed.
    /// </summary>
    /// <param name="id">The hotkey identifier.</param>
    private void OnHotkeyPressed(HotkeyId id)
    {
        if (mainViewModel == null)
        {
            return;
        }

        try
        {
            switch (id)
            {
                case HotkeyId.ToggleHudWindow:
                    mainViewModel.OpenCloseHudWindowCommand.Execute(null);
                    break;
                case HotkeyId.ToggleHudMousePassThrough:
                    mainViewModel.EnableDisableHudWindowMousePassThroughCommand.Execute(null);
                    break;
                case HotkeyId.OpenRouteTab:
                    mainViewModel.OpenTabOfType(typeof(NavRouteTableViewModel), forceOpen: true);
                    break;
                case HotkeyId.OpenBodiesTab:
                    mainViewModel.OpenTabOfType(typeof(BodyTableViewModel), forceOpen: true);
                    break;
                case HotkeyId.OpenBiologicalsTab:
                    mainViewModel.OpenTabOfType(typeof(GenusTableViewModel), forceOpen: true);
                    break;
                case HotkeyId.OpenSurroundingsTab:
                    mainViewModel.OpenTabOfType(typeof(SurroundingsTableViewModel), forceOpen: true);
                    break;
                case HotkeyId.OpenHistoryTab:
                    mainViewModel.OpenTabOfType(typeof(HistoryViewModel), forceOpen: true);
                    break;
                case HotkeyId.TryCopyNextSystemToClipboard:
                    _starSystemProvider.CopyNextSystemNametoClipboard();
                    break;
                case HotkeyId.QuitSpeechOutput:
                    SpeechProvider.ShutUp();
                    break;
            }
        }
        catch (Exception exception)
        {
            log.Error($"Error while handling hotkey {id}", exception);
        }
    }
}
