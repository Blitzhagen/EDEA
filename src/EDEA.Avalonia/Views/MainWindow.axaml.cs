using System;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using EDEA;
using EDEA.Avalonia.ViewModels;
using EDEA.Enums;
using EDEA.Services;

namespace EDEA.Avalonia.Views;

/// <summary>
/// Avalonia main window.
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
        Opened += OnOpened;
    }

    /// <summary>
    /// Attaches the global hotkey service to this window and registers all configured hotkeys.
    /// </summary>
    private void OnOpened(object? sender, EventArgs e)
    {
        UpdateWindowChrome();

        var handle = TryGetPlatformHandle()?.Handle ?? 0;
        if (handle == 0 || PlatformServices.GlobalHotkey is null)
        {
            return;
        }

        PlatformServices.GlobalHotkey.HotkeyPressed -= OnGlobalHotkeyPressed;
        PlatformServices.GlobalHotkey.HotkeyPressed += OnGlobalHotkeyPressed;
        PlatformServices.GlobalHotkey.Attach(handle);
        RegisterHotkeys();
    }

    /// <summary>
    /// Updates the maximize/restore and resize grip visuals based on the current window state.
    /// </summary>
    private void UpdateWindowChrome()
    {
        var button = this.FindControl<Button>("MaximizeRestoreButton");
        if (button is not null)
        {
            button.Content = WindowState == WindowState.Maximized ? "▣" : "▢";
        }

        var resizeThumb = this.FindControl<Thumb>("ResizeThumb");
        if (resizeThumb is not null)
        {
            resizeThumb.IsVisible = WindowState != WindowState.Maximized && WindowState != WindowState.FullScreen;
        }
    }

    /// <summary>
    /// Registers all valid hotkeys from the current preferences with the global hotkey service.
    /// </summary>
    private static void RegisterHotkeys()
    {
        var service = PlatformServices.GlobalHotkey;
        if (service is null)
        {
            return;
        }

        service.UnregisterAll();
        foreach (var property in Preferences.Hotkeys.GetType().GetProperties())
        {
            if (property.PropertyType != typeof(EDEA.Models.Hotkey))
            {
                continue;
            }

            var hotkey = (EDEA.Models.Hotkey?)property.GetValue(Preferences.Hotkeys);
            if (hotkey is { IsValid: true })
            {
                service.Register(hotkey.Id, hotkey.Modifier, hotkey.Key);
            }
        }
    }

    /// <summary>
    /// Handles a global hotkey press.
    /// </summary>
    private void OnGlobalHotkeyPressed(HotkeyId id)
    {
        if (DataContext is not MainViewModel viewModel)
        {
            return;
        }

        switch (id)
        {
            case HotkeyId.ToggleHudWindow:
                viewModel.OpenCloseHudWindowCommand.Execute(null);
                break;
            case HotkeyId.OpenRouteTab:
                viewModel.SelectedTabIndex = 0;
                Activate();
                break;
            case HotkeyId.OpenBodiesTab:
                viewModel.SelectedTabIndex = 1;
                Activate();
                break;
            case HotkeyId.OpenBiologicalsTab:
                viewModel.SelectedTabIndex = 2;
                Activate();
                break;
            case HotkeyId.OpenSurroundingsTab:
                viewModel.SelectedTabIndex = 3;
                Activate();
                break;
            case HotkeyId.OpenHistoryTab:
                viewModel.SelectedTabIndex = 4;
                Activate();
                break;
            case HotkeyId.QuitSpeechOutput:
                PlatformServices.Speech?.ShutUp();
                break;
            case HotkeyId.ToggleHudMousePassThrough:
                viewModel.ToggleHudMousePassThrough();
                break;
            case HotkeyId.TryCopyNextSystemToClipboard:
                viewModel.CopyNextSystemToClipboard();
                break;
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == WindowStateProperty)
        {
            UpdateWindowChrome();
        }
    }

    /// <summary>
    /// Starts dragging the main window from the custom title bar.
    /// </summary>
    private void TitleBar_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        BeginMoveDrag(e);
    }

    /// <summary>
    /// Minimizes the main window.
    /// </summary>
    private void MinimizeButton_Click(object? sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    /// <summary>
    /// Toggles between maximized and normal window state.
    /// </summary>
    private void MaximizeRestoreButton_Click(object? sender, RoutedEventArgs e)
    {
        WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
    }

    /// <summary>
    /// Closes the main window.
    /// </summary>
    private void CloseButton_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    /// <summary>
    /// Resizes the main window from the bottom-right resize thumb.
    /// </summary>
    private void ResizeThumb_DragDelta(object? sender, VectorEventArgs e)
    {
        Width = Math.Max(MinWidth, Width + e.Vector.X);
        Height = Math.Max(MinHeight, Height + e.Vector.Y);
    }
}
