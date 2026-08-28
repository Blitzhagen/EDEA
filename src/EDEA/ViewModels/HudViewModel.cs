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

/// <summary>
/// View model that manages the head-up display (HUD) window and its content.
/// </summary>
public class HudViewModel : ViewModelBase
{
    /// <summary>
    /// Logger instance for this class.
    /// </summary>
    private static readonly ILog log = LogManager.GetLogger(typeof(HudViewModel));

    /// <summary>
    /// The singleton instance of the HUD window.
    /// </summary>
    private static HudWindow? hudWindow = null;

    /// <summary>
    /// Win32 extended window style for transparency.
    /// </summary>
    private const int WS_EX_TRANSPARENT = 32;

    /// <summary>
    /// Win32 index for the extended window style.
    /// </summary>
    private const int GWL_EXSTYLE = -20;

    /// <summary>
    /// The main view model used to access tab and system data.
    /// </summary>
    private readonly MainViewModel _mainViewModel;

    /// <summary>
    /// The current Elite Dangerous GUI focus.
    /// </summary>
    private EdGuiFocus currentGuiFocus;

    /// <summary>
    /// A value indicating whether the player is currently on foot.
    /// </summary>
    private bool currentIsOnFoot;

    /// <summary>
    /// A value indicating whether mouse input passes through the HUD window.
    /// </summary>
    private bool hudWindowMousePassThroughEnabled;

    /// <summary>
    /// The original extended window style of the HUD window.
    /// </summary>
    private int hudWindowOriginalExtendedStyle;

    /// <summary>
    /// The close button on the HUD window.
    /// </summary>
    private Button? closeHudWindowButton => (Button?)(hudWindow?.FindName("CloseHudWindowButton"));

    /// <summary>
    /// The resize button on the HUD window.
    /// </summary>
    private Button? resizeHudWindowButton => (Button?)(hudWindow?.FindName("ResizeHudWindowButton"));

    /// <summary>
    /// The move grabber on the HUD window.
    /// </summary>
    private Rectangle? hudWindowMoveGrabber => (Rectangle?)(hudWindow?.FindName("HudWindowMoveGrabber"));

    /// <summary>
    /// Gets or sets the command that closes the HUD window.
    /// </summary>
    /// <value>The close command, or <c>null</c>.</value>
    public ICommand? CloseHudWindowCommand { get; set; } = null;

    /// <summary>
    /// Gets a value indicating whether the HUD window is open.
    /// </summary>
    /// <value><c>true</c> if the HUD window is open; otherwise, <c>false</c>.</value>
    public bool HudWindowOpen => hudWindow != null;

    /// <summary>
    /// Gets or sets a value indicating whether the HUD window passes mouse input through.
    /// </summary>
    /// <value><c>true</c> if mouse pass-through is enabled; otherwise, <c>false</c>.</value>
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

    /// <summary>
    /// Gets the current view model displayed in the HUD.
    /// </summary>
    /// <value>The current view model, or <c>null</c>.</value>
    public ViewModelBase? CurrentViewModel { get; private set; } = null;

    /// <summary>
    /// Gets the headline of the currently displayed table.
    /// </summary>
    /// <value>The table headline.</value>
    public string TableHeadline { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the view model of the current star system.
    /// </summary>
    /// <value>The current system view model.</value>
    public StarSystemViewModel CurrentSystemViewModel => _mainViewModel.CurrentSystemViewModel;

    /// <summary>
    /// Gets the name of the current star system.
    /// </summary>
    /// <value>The system name.</value>
    public string SystemName => CurrentSystemViewModel.Name;

    /// <summary>
    /// Gets a value indicating whether the exploration status is unexplored.
    /// </summary>
    /// <value><c>true</c> if the status is unexplored; otherwise, <c>false</c>.</value>
    public bool IsExplorationStatusUnexplored => CurrentSystemViewModel.IsExplorationStatusUnexplored;

    /// <summary>
    /// Gets a value indicating whether the exploration status is complete.
    /// </summary>
    /// <value><c>true</c> if the status is complete; otherwise, <c>false</c>.</value>
    public bool IsExplorationStatusComplete => CurrentSystemViewModel.IsExplorationStatusComplete;

    /// <summary>
    /// Gets a value indicating whether the exploration status is unscanned.
    /// </summary>
    /// <value><c>true</c> if the status is unscanned; otherwise, <c>false</c>.</value>
    public bool IsExplorationStatusUnscanned => CurrentSystemViewModel.IsExplorationStatusUnscanned;

    /// <summary>
    /// Gets a value indicating whether the exploration status is incomplete.
    /// </summary>
    /// <value><c>true</c> if the status is incomplete; otherwise, <c>false</c>.</value>
    public bool IsExplorationStatusIncomplete => CurrentSystemViewModel.IsExplorationStatusIncomplete;

    /// <summary>
    /// Gets the exploration status of the current system.
    /// </summary>
    /// <value>The exploration status string.</value>
    public string ExplorationStatus => CurrentSystemViewModel.ExplorationStatus;

    /// <summary>
    /// Gets the body exploration status text.
    /// </summary>
    /// <value>The body exploration status string.</value>
    public string BodyExplorationStatus => Resources.Hud_BodyExplorationStatus + _mainViewModel.BodyExplorationStatus;

    /// <summary>
    /// Gets the non-body exploration status text.
    /// </summary>
    /// <value>The non-body exploration status string.</value>
    public string NonBodyExplorationStatus => Resources.Hud_NonBodyExplorationStatus + _mainViewModel.NonBodyExplorationStatus;

    /// <summary>
    /// Initializes a new instance of the <see cref="HudViewModel"/> class.
    /// </summary>
    /// <param name="mainViewModel">The main view model.</param>
    /// <param name="statusProvider">The provider for status data.</param>
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

    /// <summary>
    /// Shows or hides the HUD window based on the current settings and GUI focus.
    /// </summary>
    /// <param name="guiFocus">The current Elite Dangerous GUI focus.</param>
    /// <param name="isOnFoot">A value indicating whether the player is on foot.</param>
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

    /// <summary>
    /// Handles GUI data updates for the HUD.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
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

    /// <summary>
    /// Handles changes of the selected tab index and updates the HUD view model.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void _mainViewModel_SelectedTabIndexChanged(object? sender, EventArgs e)
    {
        if (Preferences.HudWindow.HudWindowTabViewModel == 0 && !(_mainViewModel.TabViewModels[_mainViewModel.SelectedTabIndex].GetType() == typeof(HistoryViewModel)) && !(_mainViewModel.TabViewModels[_mainViewModel.SelectedTabIndex].GetType() == typeof(SurroundingsTableViewModel)))
        {
            CurrentViewModel = _mainViewModel.TabViewModels[_mainViewModel.SelectedTabIndex];
            setTableHeadline();
            OnPropertyChanged("CurrentViewModel");
        }
    }

    /// <summary>
    /// Sets the table headline based on the current view model.
    /// </summary>
    private void setTableHeadline()
    {
        TableHeadline = ((TabViewModel?)CurrentViewModel)?.TabHeader ?? string.Empty;
        OnPropertyChanged("TableHeadline");
    }

    /// <summary>
    /// Updates the HUD visibility based on the current settings.
    /// </summary>
    public void UpdateVisibilityBasedOnSettings()
    {
        setVisibilityBasedOnSettings(currentGuiFocus, currentIsOnFoot);
    }

    /// <summary>
    /// Sets the view model to be displayed in the HUD based on user preferences.
    /// </summary>
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

    /// <summary>
    /// Enables or disables mouse pass-through for the HUD window.
    /// </summary>
    /// <param name="state"><c>true</c> to enable mouse pass-through; <c>false</c> to disable it.</param>
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

    /// <summary>
    /// Shows the HUD window or activates it if already open.
    /// </summary>
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

    /// <summary>
    /// Handles the <see cref="HudWindow.Loaded"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void HudWindow_Loaded(object? sender, RoutedEventArgs e)
    {
        SetMousePassThrough(HudWindowMousePassThroughEnabled);
    }

    /// <summary>
    /// Handles the <see cref="Button.MouseLeave"/> event for the close button.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void CloseHudWindowButton_MouseLeave(object? sender, MouseEventArgs e)
    {
        if (HudWindowOpen)
        {
            closeHudWindowButton!.Opacity = 1.0;
            closeHudWindowButton!.Background = new SolidColorBrush(Colors.Transparent);
            closeHudWindowButton!.Foreground = (SolidColorBrush)Application.Current.Resources["MainColor"];
        }
    }

    /// <summary>
    /// Handles the <see cref="Button.MouseEnter"/> event for the close button.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void CloseHudWindowButton_MouseEnter(object? sender, MouseEventArgs e)
    {
        closeHudWindowButton!.Opacity = 0.4;
        closeHudWindowButton!.Foreground = (SolidColorBrush)Application.Current.Resources["MainBackgroundColor"];
        closeHudWindowButton!.Background = (SolidColorBrush)Application.Current.Resources["MainColor"];
    }

    /// <summary>
    /// Handles the <see cref="Rectangle.MouseLeave"/> event for the move grabber.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void HudWindowMoveGrabber_MouseLeave(object? sender, MouseEventArgs e)
    {
        Mouse.OverrideCursor = null;
    }

    /// <summary>
    /// Handles the <see cref="Rectangle.MouseEnter"/> event for the move grabber.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void HudWindowMoveGrabber_MouseEnter(object? sender, MouseEventArgs e)
    {
        Mouse.OverrideCursor = Cursors.SizeAll;
    }

    /// <summary>
    /// Handles the <see cref="Rectangle.MouseDown"/> event for the move grabber.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
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

    /// <summary>
    /// Handles the <see cref="HudWindow.MouseLeave"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void HudWindow_MouseLeave(object? sender, MouseEventArgs e)
    {
        if (HudWindowOpen)
        {
            _ = (HudWindow)sender!;
            closeHudWindowButton!.Opacity = 0.0;
            resizeHudWindowButton!.Opacity = 0.0;
        }
    }

    /// <summary>
    /// Handles the <see cref="HudWindow.MouseEnter"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void HudWindow_MouseEnter(object? sender, MouseEventArgs e)
    {
        _ = (HudWindow)sender!;
        closeHudWindowButton!.Opacity = 1.0;
        resizeHudWindowButton!.Opacity = 1.0;
    }

    /// <summary>
    /// Handles the <see cref="HudWindow.Closed"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void HudWindow_Closed(object? sender, EventArgs e)
    {
        CloseHudWindowCommand = null;
        hudWindow = null;
        OnPropertyChanged("HudWindowOpen");
    }

    /// <summary>
    /// Retrieves the specified window information.
    /// </summary>
    /// <param name="hwnd">The window handle.</param>
    /// <param name="index">The index of the value to retrieve.</param>
    /// <returns>The requested window information.</returns>
    [DllImport("user32.dll")]
    public static extern int GetWindowLong(nint hwnd, int index);

    /// <summary>
    /// Sets the specified window information.
    /// </summary>
    /// <param name="hwnd">The window handle.</param>
    /// <param name="index">The index of the value to set.</param>
    /// <param name="newStyle">The new value.</param>
    /// <returns>The previous value, or zero if an error occurred.</returns>
    [DllImport("user32.dll")]
    public static extern int SetWindowLong(nint hwnd, int index, int newStyle);
}
