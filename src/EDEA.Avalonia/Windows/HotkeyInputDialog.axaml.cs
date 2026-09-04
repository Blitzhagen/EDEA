using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaInput = Avalonia.Input;
using RoutingStrategies = Avalonia.Interactivity.RoutingStrategies;
using CoreInput = EDEA.Core.Input;
using EDEA.Models;

namespace EDEA.Avalonia.Windows;

/// <summary>
/// Dialog window for capturing a global hotkey combination.
/// </summary>
public partial class HotkeyInputDialog : Window
{
    /// <summary>
    /// The hotkey combination currently captured in the dialog.
    /// </summary>
    private Hotkey _current;

    /// <summary>
    /// Initializes a new instance of the <see cref="HotkeyInputDialog"/> class.
    /// </summary>
    /// <param name="initial">The initial hotkey combination, if any.</param>
    public HotkeyInputDialog()
        : this(null)
    {
    }

    public HotkeyInputDialog(Hotkey? initial)
    {
        InitializeComponent();
        _current = initial ?? new Hotkey();
        UpdateDisplay();
        AddHandler(KeyDownEvent, OnPreviewKeyDown, RoutingStrategies.Tunnel);
    }

    /// <summary>
    /// Captures a key combination while the dialog is open.
    /// </summary>
    private void OnPreviewKeyDown(object? sender, AvaloniaInput.KeyEventArgs e)
    {
        if (e.Key == AvaloniaInput.Key.Escape)
        {
            Close(null);
            e.Handled = true;
            return;
        }

        if (e.Key == AvaloniaInput.Key.Enter)
        {
            if (_current.IsValid)
            {
                Close(_current);
            }

            e.Handled = true;
            return;
        }

        var modifiers = MapModifiers(e.KeyModifiers | ModifierForKey(e.Key));
        var coreKey = MapKey(e.Key);

        if (coreKey == CoreInput.Key.None || IsModifierKey(e.Key))
        {
            _current = new Hotkey { Modifier = modifiers, Key = CoreInput.Key.None };
            UpdateDisplay();
            e.Handled = true;
            return;
        }

        _current = new Hotkey { Modifier = modifiers, Key = coreKey };
        Close(_current);
        e.Handled = true;
    }

    /// <summary>
    /// Confirms the captured hotkey.
    /// </summary>
    private void OkButton_Click(object? sender, RoutedEventArgs e)
    {
        if (_current.IsValid)
        {
            Close(_current);
        }
    }

    /// <summary>
    /// Cancels the dialog.
    /// </summary>
    private void CancelButton_Click(object? sender, RoutedEventArgs e)
    {
        Close(null);
    }

    /// <summary>
    /// Updates the display text box with the current hotkey combination.
    /// </summary>
    private void UpdateDisplay()
    {
        HotkeyTextBox.Text = FormatHotkey(_current);
    }

    /// <summary>
    /// Determines whether the given Avalonia key is a modifier key.
    /// </summary>
    private static bool IsModifierKey(AvaloniaInput.Key key)
    {
        return key is AvaloniaInput.Key.LeftShift or AvaloniaInput.Key.RightShift
            or AvaloniaInput.Key.LeftCtrl or AvaloniaInput.Key.RightCtrl
            or AvaloniaInput.Key.LeftAlt or AvaloniaInput.Key.RightAlt
            or AvaloniaInput.Key.LWin or AvaloniaInput.Key.RWin;
    }

    /// <summary>
    /// Returns the modifier flag corresponding to a modifier key.
    /// </summary>
    private static AvaloniaInput.KeyModifiers ModifierForKey(AvaloniaInput.Key key)
    {
        return key switch
        {
            AvaloniaInput.Key.LeftShift or AvaloniaInput.Key.RightShift => AvaloniaInput.KeyModifiers.Shift,
            AvaloniaInput.Key.LeftCtrl or AvaloniaInput.Key.RightCtrl => AvaloniaInput.KeyModifiers.Control,
            AvaloniaInput.Key.LeftAlt or AvaloniaInput.Key.RightAlt => AvaloniaInput.KeyModifiers.Alt,
            AvaloniaInput.Key.LWin or AvaloniaInput.Key.RWin => AvaloniaInput.KeyModifiers.Meta,
            _ => AvaloniaInput.KeyModifiers.None,
        };
    }

    /// <summary>
    /// Maps an Avalonia key to the platform-independent key enum.
    /// </summary>
    private static CoreInput.Key MapKey(AvaloniaInput.Key key)
    {
        return Enum.TryParse<CoreInput.Key>(key.ToString(), out var result) ? result : CoreInput.Key.None;
    }

    /// <summary>
    /// Maps Avalonia key modifiers to the platform-independent modifier enum.
    /// </summary>
    private static CoreInput.ModifierKeys MapModifiers(AvaloniaInput.KeyModifiers modifiers)
    {
        var result = CoreInput.ModifierKeys.None;
        if (modifiers.HasFlag(AvaloniaInput.KeyModifiers.Alt)) result |= CoreInput.ModifierKeys.Alt;
        if (modifiers.HasFlag(AvaloniaInput.KeyModifiers.Control)) result |= CoreInput.ModifierKeys.Control;
        if (modifiers.HasFlag(AvaloniaInput.KeyModifiers.Shift)) result |= CoreInput.ModifierKeys.Shift;
        if (modifiers.HasFlag(AvaloniaInput.KeyModifiers.Meta)) result |= CoreInput.ModifierKeys.Windows;
        return result;
    }

    /// <summary>
    /// Formats a hotkey combination for display.
    /// </summary>
    private static string FormatHotkey(Hotkey hotkey)
    {
        if (hotkey.Modifier == CoreInput.ModifierKeys.None && hotkey.Key == CoreInput.Key.None)
        {
            return "-";
        }

        var parts = new List<string>();
        if (hotkey.Modifier.HasFlag(CoreInput.ModifierKeys.Alt)) parts.Add("Alt");
        if (hotkey.Modifier.HasFlag(CoreInput.ModifierKeys.Control)) parts.Add("Ctrl");
        if (hotkey.Modifier.HasFlag(CoreInput.ModifierKeys.Shift)) parts.Add("Shift");
        if (hotkey.Modifier.HasFlag(CoreInput.ModifierKeys.Windows)) parts.Add("Win");
        if (hotkey.Key != CoreInput.Key.None)
        {
            parts.Add(hotkey.Key.ToString());
        }

        return string.Join(" + ", parts);
    }
}
