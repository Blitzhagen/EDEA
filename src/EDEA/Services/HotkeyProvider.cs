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
using EDEA.ViewModels;
using log4net;

namespace EDEA.Services;

public class HotkeyProvider : ViewModelBase
{
    public static readonly ImmutableDictionary<HotkeyId, string> HotkeyDescription = new Dictionary<HotkeyId, string>
    {
        {
            HotkeyId.ToggleHudWindow,
            "Open/Close HUD Window"
        },
        {
            HotkeyId.ToggleHudMousePassThrough,
            "Enable/Disable HUD Window Mouse Pass Through"
        },
        {
            HotkeyId.OpenRouteTab,
            "Open Route Tab"
        },
        {
            HotkeyId.OpenBodiesTab,
            "Open Bodies Tab"
        },
        {
            HotkeyId.OpenBiologicalsTab,
            "Open Biologicals Tab"
        },
        {
            HotkeyId.OpenSurroundingsTab,
            "Open Surroundings Tab"
        },
        {
            HotkeyId.OpenHistoryTab,
            "Open History Tab"
        },
        {
            HotkeyId.TryCopyNextSystemToClipboard,
            "Copy Next System Name In Locked/Plotter Route To Clipboard"
        },
        {
            HotkeyId.QuitSpeechOutput,
            "Cancel Current Speech Output"
        }
    }.ToImmutableDictionary();

    private static readonly ILog log = LogManager.GetLogger(typeof(HotkeyProvider));

    private static HotkeyProvider? instance;

    private MainViewModel? mainViewModel;

    private Window? mainWindow;

    private nint mainWindowHandle;

    private readonly StarSystemProvider _starSystemProvider;

    public Dictionary<string, HotkeyViewModel> Hotkeys { get; private set; }

    private HotkeyProvider(StarSystemProvider starSystemProvider)
    {
        _starSystemProvider = starSystemProvider;
        Hotkeys = new Dictionary<string, HotkeyViewModel>();
        setHotkeysFromPreferences();
    }

    public static HotkeyProvider Instance(StarSystemProvider starSystemProvider)
    {
        if (instance == null)
        {
            instance = new HotkeyProvider(starSystemProvider);
        }
        return instance;
    }

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

    public void AssignHotKey(HotkeyViewModel hotkeyViewModel)
    {
        if (!hotkeyViewModel.IsValid)
        {
            return;
        }
        try
        {
            UnregisterHotKey(mainWindowHandle, (int)hotkeyViewModel.Id);
            RegisterHotKey(mainWindowHandle, (int)hotkeyViewModel.Id, (int)hotkeyViewModel.Modifier, KeyInterop.VirtualKeyFromKey(hotkeyViewModel.Key));
            log.Debug($"Assigned hotkey {hotkeyViewModel.FullKey} for '{hotkeyViewModel.Description}' ({hotkeyViewModel.Id}) ");
        }
        catch (Exception exception)
        {
            log.Error($"Could not set hotkey with id {hotkeyViewModel.Id}, modifier {hotkeyViewModel.Modifier} and key {hotkeyViewModel.Key}", exception);
        }
    }

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

    public void UnassignAllHotkeys()
    {
        foreach (HotkeyViewModel hotkey in Hotkeys.Values)
        {
            UnassignHotKey(hotkey);
        }
    }

    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(nint hWnd, int id, int fsModifiers, int vlc);

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(nint hWnd, int id);

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
                Hotkeys.Add(key, new HotkeyViewModel(hotkey, HotkeyDescription[hotkey.Id]));
            }
        }
        catch (Exception exception)
        {
            log.Error("Could not initialize hotkeys, hotkey will not work correctly!", exception);
        }
        OnPropertyChanged(nameof(Hotkeys));
    }

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
