using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shapes;
using EDEA;
using EDEA.Commands;
using EDEA.Models;
using EDEA.Properties;
using EDEA.Services;
using EDEA.Windows;
using log4net;

namespace EDEA.ViewModels;

public class HudViewModel : ViewModelBase
{
    private static readonly ILog log = LogManager.GetLogger(typeof(HudViewModel));

    private static HudWindow? hudWindow = null;

    private const int WS_EX_TRANSPARENT = 32;

    private const int GWL_EXSTYLE = -20;

    private readonly MainViewModel _mainViewModel;

    private EdGuiFocus currentGuiFocus;

    private bool currentIsOnFoot;

    private bool hudWindowMousePassThroughEnabled;

    private int hudWindowOriginalExtendedStyle;

    private Button? closeHudWindowButton => (Button?)(hudWindow?.FindName("CloseHudWindowButton"));

    private Button? resizeHudWindowButton => (Button?)(hudWindow?.FindName("ResizeHudWindowButton"));

    private Rectangle? hudWindowMoveGrabber => (Rectangle?)(hudWindow?.FindName("HudWindowMoveGrabber"));

    public ICommand? CloseHudWindowCommand { get; set; } = null;

    public bool HudWindowOpen => hudWindow != null;

    public bool HudWindowMousePassThroughEnabled
    {
        get
        {
            return hudWindowMousePassThroughEnabled;
        }
        private set
        {
            if (hudWindowMousePassThroughEnabled != value)
            {
                hudWindowMousePassThroughEnabled = value;
                OnPropertyChanged("HudWindowMousePassThroughEnabled");
            }
        }
    }

    public ViewModelBase? CurrentViewModel { get; private set; } = null;

    public string TableHeadline { get; private set; } = string.Empty;

    public StarSystemViewModel CurrentSystemViewModel => _mainViewModel.CurrentSystemViewModel;

    public string SystemName => CurrentSystemViewModel.Name;

    public bool IsExplorationStatusUnexplored => CurrentSystemViewModel.IsExplorationStatusUnexplored;

    public bool IsExplorationStatusComplete => CurrentSystemViewModel.IsExplorationStatusComplete;

    public bool IsExplorationStatusUnscanned => CurrentSystemViewModel.IsExplorationStatusUnscanned;

    public bool IsExplorationStatusIncomplete => CurrentSystemViewModel.IsExplorationStatusIncomplete;

    public string ExplorationStatus => CurrentSystemViewModel.ExplorationStatus;

    public string BodyExplorationStatus => Resources.Hud_BodyExplorationStatus + _mainViewModel.BodyExplorationStatus;

    public string NonBodyExplorationStatus => Resources.Hud_NonBodyExplorationStatus + _mainViewModel.NonBodyExplorationStatus;

    public HudViewModel(MainViewModel mainViewModel, StatusProvider statusProvider)
    {
        _mainViewModel = mainViewModel;
        _mainViewModel.SelectedTabIndexChanged += _mainViewModel_SelectedTabIndexChanged;
        _mainViewModel.GuiHudDataUpdated += _mainViewModel_GuiHudDataUpdated;
        currentGuiFocus = EdGuiFocus.NoFocus;
        currentIsOnFoot = false;
        hudWindowMousePassThroughEnabled = Preferences.Application.HudWindowMousePassThroughEnabled;
        statusProvider.GuiFocusUpdated += delegate (object? sender, EdGuiFocus guiFocus, bool isOnFoot)
        {
            Application.Current?.Dispatcher.Invoke(delegate
            {
                setVisibilityBasedOnSettings(guiFocus, isOnFoot);
            });
        };
        if (Preferences.HudWindow.HudWindowTabViewModel != 0)
        {
            SetView();
        }
    }

    private void setVisibilityBasedOnSettings(EdGuiFocus guiFocus, bool isOnFoot)
    {
        log.Debug($"GUI focus set to {guiFocus}, player is {(isOnFoot ? "" : "not ")}on foot");
        currentGuiFocus = guiFocus;
        currentIsOnFoot = isOnFoot;
        if (!HudWindowOpen)
        {
            return;
        }
        try
        {
            if (isOnFoot && Preferences.HudWindow.HideOnOnFoot)
            {
                log.Debug("Hide HUD according to preferences: HideOnOnFoot");
                hudWindow!.Hide();
            }
            else if ((bool)Preferences.HudWindow.GetType().GetProperty("HideOn" + Enum.GetName(typeof(EdGuiFocus), guiFocus)!)!.GetValue(Preferences.HudWindow)!)
            {
                log.Debug("Hide HUD according to preferences: HideOn" + Enum.GetName(typeof(EdGuiFocus), guiFocus));
                hudWindow!.Hide();
            }
            else
            {
                hudWindow!.Show();
            }
        }
        catch (Exception exception)
        {
            log.Error("Error on hiding/showing the HUD window!", exception);
        }
    }

    private void _mainViewModel_GuiHudDataUpdated(object? sender, EventArgs e)
    {
        setTableHeadline();
        OnPropertyChanged("CurrentSystemViewModel");
        OnPropertyChanged("SystemName");
        OnPropertyChanged("IsExplorationStatusUnexplored");
        OnPropertyChanged("IsExplorationStatusComplete");
        OnPropertyChanged("IsExplorationStatusUnscanned");
        OnPropertyChanged("IsExplorationStatusIncomplete");
        OnPropertyChanged("BodyExplorationStatus");
        OnPropertyChanged("NonBodyExplorationStatus");
        OnPropertyChanged("ExplorationStatus");
    }

    private void _mainViewModel_SelectedTabIndexChanged(object? sender, EventArgs e)
    {
        if (Preferences.HudWindow.HudWindowTabViewModel == 0 && !(_mainViewModel.TabViewModels[_mainViewModel.SelectedTabIndex].GetType() == typeof(HistoryViewModel)) && !(_mainViewModel.TabViewModels[_mainViewModel.SelectedTabIndex].GetType() == typeof(SurroundingsTableViewModel)))
        {
            CurrentViewModel = _mainViewModel.TabViewModels[_mainViewModel.SelectedTabIndex];
            setTableHeadline();
            OnPropertyChanged("CurrentViewModel");
        }
    }

    private void setTableHeadline()
    {
        TableHeadline = ((TabViewModel?)CurrentViewModel)?.TabHeader ?? string.Empty;
        OnPropertyChanged("TableHeadline");
    }

    public void UpdateVisibilityBasedOnSettings()
    {
        setVisibilityBasedOnSettings(currentGuiFocus, currentIsOnFoot);
    }

    public void SetView()
    {
        string tabViewModel = ((HudWindowTabViewModel)Preferences.HudWindow.HudWindowTabViewModel/*cast due to constrained. prefix*/).ToString();
        if (typeof(NavRouteTableViewModel).ToString().Contains(tabViewModel) || typeof(BodyTableViewModel).ToString().Contains(tabViewModel))
        {
            CurrentViewModel = _mainViewModel.TabViewModels.FirstOrDefault(vm => vm.GetType().ToString().Contains(tabViewModel));
            setTableHeadline();
            OnPropertyChanged("CurrentViewModel");
        }
        else
        {
            _mainViewModel_SelectedTabIndexChanged(this, EventArgs.Empty);
        }
    }

    public void SetMousePassThrough(bool state)
    {
        HudWindowMousePassThroughEnabled = state;
        if (HudWindowOpen)
        {
            nint handle = new WindowInteropHelper(hudWindow!).Handle;
            if (state)
            {
                hudWindowOriginalExtendedStyle = GetWindowLong(handle, -20);
                SetWindowLong(handle, -20, hudWindowOriginalExtendedStyle | 0x20);
            }
            else if (hudWindowOriginalExtendedStyle != 0)
            {
                SetWindowLong(handle, -20, hudWindowOriginalExtendedStyle);
            }
        }
    }

    public void ShowHudWindow()
    {
        if (hudWindow == null)
        {
            hudWindow = new HudWindow();
            OnPropertyChanged("HudWindowOpen");
            JotSettingsProvider.Tracker.Track(hudWindow);
            hudWindow!.DataContext = this;
            hudWindow!.Loaded += HudWindow_Loaded;
            hudWindow!.Closed += HudWindow_Closed;
            hudWindow!.MouseEnter += HudWindow_MouseEnter;
            hudWindow!.MouseLeave += HudWindow_MouseLeave;
            hudWindowMoveGrabber!.MouseDown += HudWindowMoveGrabber_MouseDown;
            hudWindowMoveGrabber!.MouseEnter += HudWindowMoveGrabber_MouseEnter;
            hudWindowMoveGrabber!.MouseLeave += HudWindowMoveGrabber_MouseLeave;
            closeHudWindowButton!.MouseEnter += CloseHudWindowButton_MouseEnter;
            closeHudWindowButton!.MouseLeave += CloseHudWindowButton_MouseLeave;
            CloseHudWindowCommand = new CloseWindowCommand(hudWindow!);
            hudWindow!.Show();
        }
        else
        {
            if (!hudWindow!.IsActive)
            {
                hudWindow!.Activate();
            }
            if (!hudWindow!.IsFocused)
            {
                hudWindow!.Focus();
            }
        }
    }

    private void HudWindow_Loaded(object? sender, RoutedEventArgs e)
    {
        SetMousePassThrough(HudWindowMousePassThroughEnabled);
    }

    private void CloseHudWindowButton_MouseLeave(object? sender, MouseEventArgs e)
    {
        if (HudWindowOpen)
        {
            closeHudWindowButton!.Opacity = 1.0;
            closeHudWindowButton!.Background = new SolidColorBrush(Colors.Transparent);
            closeHudWindowButton!.Foreground = (SolidColorBrush)Application.Current.Resources["MainColor"];
        }
    }

    private void CloseHudWindowButton_MouseEnter(object? sender, MouseEventArgs e)
    {
        closeHudWindowButton!.Opacity = 0.4;
        closeHudWindowButton!.Foreground = (SolidColorBrush)Application.Current.Resources["MainBackgroundColor"];
        closeHudWindowButton!.Background = (SolidColorBrush)Application.Current.Resources["MainColor"];
    }

    private void HudWindowMoveGrabber_MouseLeave(object? sender, MouseEventArgs e)
    {
        Mouse.OverrideCursor = null;
    }

    private void HudWindowMoveGrabber_MouseEnter(object? sender, MouseEventArgs e)
    {
        Mouse.OverrideCursor = Cursors.SizeAll;
    }

    private void HudWindowMoveGrabber_MouseDown(object? sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
        {
            if (hudWindow!.WindowState == WindowState.Normal)
            {
                hudWindow!.WindowState = WindowState.Maximized;
            }
            else if (hudWindow!.WindowState == WindowState.Maximized)
            {
                hudWindow!.WindowState = WindowState.Normal;
            }
        }
        if (Mouse.LeftButton == MouseButtonState.Pressed)
        {
            hudWindow!.DragMove();
        }
    }

    private void HudWindow_MouseLeave(object? sender, MouseEventArgs e)
    {
        if (HudWindowOpen)
        {
            _ = (HudWindow)sender!;
            closeHudWindowButton!.Opacity = 0.0;
            resizeHudWindowButton!.Opacity = 0.0;
        }
    }

    private void HudWindow_MouseEnter(object? sender, MouseEventArgs e)
    {
        _ = (HudWindow)sender!;
        closeHudWindowButton!.Opacity = 1.0;
        resizeHudWindowButton!.Opacity = 1.0;
    }

    private void HudWindow_Closed(object? sender, EventArgs e)
    {
        CloseHudWindowCommand = null;
        hudWindow = null;
        OnPropertyChanged("HudWindowOpen");
    }

    [DllImport("user32.dll")]
    public static extern int GetWindowLong(nint hwnd, int index);

    [DllImport("user32.dll")]
    public static extern int SetWindowLong(nint hwnd, int index, int newStyle);
}

