using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using EDEA.Services;
using EDEA.ViewModels;
using EDEA.Windows;
using log4net;

namespace EDEA.Commands;

public class AssignHotkeyCommand : CommandBase
{
    private static readonly ILog log = LogManager.GetLogger(typeof(AssignHotkeyCommand));

    private readonly HotkeyProvider _hotkeyProvider;

    private readonly PreferencesWindow _preferencesWindow;

    private readonly TabItem _globalHotkeysTabItem;

    private HotkeyViewModel? _currentHotkey;

    private TextBlock? _hotkeyTextBlock;

    private bool _isActive;

    public AssignHotkeyCommand(HotkeyProvider hotkeyProvider, PreferencesWindow preferencesWindow)
    {
        _hotkeyProvider = hotkeyProvider;
        _preferencesWindow = preferencesWindow;
        _globalHotkeysTabItem = (TabItem)preferencesWindow.FindName("GlobalHotkeysTabItem");
        _isActive = false;
    }

    public override void Execute(object? parameter)
    {
        if (_isActive)
        {
            return;
        }
        try
        {
            _isActive = true;
            _hotkeyTextBlock = parameter as TextBlock;
            string? tag = _hotkeyTextBlock?.Tag?.ToString();
            if (_hotkeyTextBlock != null &&
                !string.IsNullOrEmpty(tag) &&
                _hotkeyProvider.Hotkeys.TryGetValue(tag, out _currentHotkey) &&
                _currentHotkey != null)
            {
                _hotkeyProvider.UnassignHotKey(_currentHotkey);
                log.Debug($"Unassigned hotkey {_currentHotkey.FullKey} for '{_currentHotkey.Description}' ({_currentHotkey.Id}) ");
                _hotkeyTextBlock.LostFocus += hotkeyTextBlock_LostFocus;
                _hotkeyTextBlock.KeyUp += hotkeyTextBlock_KeyUp;
                _hotkeyTextBlock.Focusable = true;
                _hotkeyTextBlock.Text = "Press New Hotkey";
                _hotkeyTextBlock.Focus();
                return;
            }
            throw new Exception("Selected hotkey does not exist!?");
        }
        catch (Exception exception)
        {
            log.Error($"Cannot initialize setter mode for hotkey {_currentHotkey}, parameter was: {parameter}", exception);
            _isActive = false;
        }
    }

    private void hotkeyTextBlock_KeyUp(object? sender, KeyEventArgs e)
    {
        if (_currentHotkey == null)
            return;

        try
        {
            Key key = ((e.Key != Key.System) ? e.Key : e.SystemKey);
            ModifierKeys modifiers = Keyboard.Modifiers;
            log.Debug($"Pressed hotkey modifiers {modifiers} and key {key}");
            if (modifiers != ModifierKeys.None && key != Key.None && key != Key.LeftAlt && key != Key.LeftCtrl && key != Key.LeftShift && key != Key.RightAlt && key != Key.RightCtrl && key != Key.RightShift)
            {
                _currentHotkey.Key = key;
                _currentHotkey.Modifier = modifiers;
            }
            else
            {
                _currentHotkey.Key = Key.None;
                _currentHotkey.Modifier = ModifierKeys.None;
            }
            _globalHotkeysTabItem?.Focus();
        }
        catch (Exception exception)
        {
            log.Error($"Error while getting user input for hotkey {_currentHotkey}", exception);
        }
    }

    private void hotkeyTextBlock_LostFocus(object? sender, RoutedEventArgs e)
    {
        if (_currentHotkey == null || _hotkeyTextBlock == null)
            return;

        try
        {
            _hotkeyProvider.AssignHotKey(_currentHotkey);
            Binding binding = new Binding
            {
                Source = _currentHotkey.FullKey,
                Mode = BindingMode.OneWay
            };
            _hotkeyTextBlock.SetBinding(TextBlock.TextProperty, binding);
            _hotkeyTextBlock.Focusable = false;
            _hotkeyTextBlock.LostFocus -= hotkeyTextBlock_LostFocus;
            _hotkeyTextBlock.KeyUp -= hotkeyTextBlock_KeyUp;
            _currentHotkey = null;
            _hotkeyTextBlock = null;
            _isActive = false;
        }
        catch (Exception exception)
        {
            log.Error($"Error while assigning new hotkey for {_currentHotkey} ", exception);
        }
    }
}
