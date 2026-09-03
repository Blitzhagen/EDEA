using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using EDEA.Enums;
using EDEA.Models;
using EDEA.Properties;
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

    /// <summary>The mainWindowHandle field.</summary>
    private nint mainWindowHandle;

    /// <summary>The _starSystemProvider field.</summary>
    private readonly StarSystemProvider _starSystemProvider;

    /// <summary>Gets or sets the Hotkeys.</summary>
    /// <value>A Dictionary<string, HotkeyViewModel> value.</value>
    public Dictionary<string, HotkeyViewModel> Hotkeys { get; private set; }

    /// <summary>Initializes a new instance of the HotkeyProvider class.</summary>
    /// <param name="starSystemProvider">The StarSystemProvider value of the starSystemProvider parameter.</param>
    private HotkeyProvider(StarSystemProvider starSystemProvider)
    {
        _starSystemProvider = starSystemProvider;
        Hotkeys = new Dictionary<string, HotkeyViewModel>();
        setHotkeysFromPreferences();
    }

    /// <summary>Performs the Instance operation.</summary>
    /// <param name="starSystemProvider">The StarSystemProvider value of the starSystemProvider parameter.</param>
    /// <returns>A HotkeyProvider result.</returns>
    public static HotkeyProvider Instance(StarSystemProvider starSystemProvider)
    {
        if (instance == null)
        {
            instance = new HotkeyProvider(starSystemProvider);
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
            UnregisterHotKey(mainWindowHandle, (int)hotkeyViewModel.Id);
            RegisterHotKey(mainWindowHandle, (int)hotkeyViewModel.Id, (int)hotkeyViewModel.Modifier, KeyInterop.VirtualKeyFromKey((System.Windows.Input.Key)hotkeyViewModel.Key));
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
            UnregisterHotKey(mainWindowHandle, (int)hotkeyViewModel.Id);
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
        foreach (HotkeyViewModel hotkey in Hotkeys.Values)
        {
            UnassignHotKey(hotkey);
        }
    }

    /// <summary>Performs the RegisterHotKey operation.</summary>
    /// <param name="hWnd">The nint value of the hWnd parameter.</param>
    /// <param name="id">The int value of the id parameter.</param>
    /// <param name="fsModifiers">The int value of the fsModifiers parameter.</param>
    /// <param name="vlc">The int value of the vlc parameter.</param>
    /// <returns>A bool result.</returns>
    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(nint hWnd, int id, int fsModifiers, int vlc);

    /// <summary>Performs the UnregisterHotKey operation.</summary>
    /// <param name="hWnd">The nint value of the hWnd parameter.</param>
    /// <param name="id">The int value of the id parameter.</param>
    /// <returns>A bool result.</returns>
    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(nint hWnd, int id);

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
            mainWindowHandle = new WindowInteropHelper(mainWindow).Handle;
            HwndSource? hwndSource = PresentationSource.FromVisual(mainWindow) as HwndSource;
            if (mainWindowHandle == 0 || hwndSource == null)
            {
                log.Error("Error on setting hotkeys from preferences, can not get handle for main window. Hotkeys will not work!");
                return;
            }
            hwndSource.AddHook(WndProc);
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
            UnassignAllHotkeys();
        }
        catch (Exception exception)
        {
            log.Error("Generic error on unsetting hotkeys", exception);
        }
    }

    /// <summary>Performs the WndProc operation.</summary>
    /// <param name="hwnd">The nint value of the hwnd parameter.</param>
    /// <param name="msg">The int value of the msg parameter.</param>
    /// <param name="wParam">The nint value of the wParam parameter.</param>
    /// <param name="lParam">The nint value of the lParam parameter.</param>
    /// <param name="handled">The bool value of the handled parameter.</param>
    /// <returns>A nint result.</returns>
    private nint WndProc(nint hwnd, int msg, nint wParam, nint lParam, ref bool handled)
    {
        if (msg == 0x0312)
        {
            log.Debug($"Hotkey press event with id: {wParam}");
            long hotkeyId = wParam;
            long hotkeyOffset = hotkeyId - 171701;
            if ((ulong)hotkeyOffset <= 8uL)
            {
                switch ((int)hotkeyOffset)
                {
                    case 0:
                        mainViewModel!.OpenCloseHudWindowCommand.Execute(null);
                        handled = true;
                        break;
                    case 1:
                        mainViewModel!.EnableDisableHudWindowMousePassThroughCommand.Execute(null);
                        handled = true;
                        break;
                    case 2:
                        mainViewModel!.OpenTabOfType(typeof(NavRouteTableViewModel), forceOpen: true);
                        handled = true;
                        break;
                    case 3:
                        mainViewModel!.OpenTabOfType(typeof(BodyTableViewModel), forceOpen: true);
                        handled = true;
                        break;
                    case 4:
                        mainViewModel!.OpenTabOfType(typeof(GenusTableViewModel), forceOpen: true);
                        handled = true;
                        break;
                    case 5:
                        mainViewModel!.OpenTabOfType(typeof(SurroundingsTableViewModel), forceOpen: true);
                        handled = true;
                        break;
                    case 6:
                        mainViewModel!.OpenTabOfType(typeof(HistoryViewModel), forceOpen: true);
                        handled = true;
                        break;
                    case 7:
                        _starSystemProvider.CopyNextSystemNametoClipboard();
                        handled = true;
                        break;
                    case 8:
                        SpeechProvider.ShutUp();
                        handled = true;
                        break;
                }
            }
        }
        return IntPtr.Zero;
    }
}
