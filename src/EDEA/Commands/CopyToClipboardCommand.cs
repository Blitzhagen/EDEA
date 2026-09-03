using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using EDEA.Models;
using EDEA.Services;
using log4net;

namespace EDEA.Commands;

/// <summary>
/// Command that copies text or numeric content to the system clipboard and optionally shows a popup.
/// </summary>
public class CopyToClipboardCommand : CommandBase
{
    private static readonly ILog log = LogManager.GetLogger(typeof(CopyToClipboardCommand));

    /// <summary>
    /// Copies the supplied value or text to the clipboard. If the control key is held, numeric values are extracted.
    /// </summary>
    /// <param name="parameter">The value to copy. Can be a <see cref="Body"/>, <see cref="StarSystem"/>, <see cref="CopyToClipboardCommandParameter"/>, or a <see cref="FrameworkElement"/> with named popup and text block.</param>
    public override void Execute(object? parameter)
    {
        if (parameter == null)
        {
            return;
        }

        bool ctrlPressed = Keyboard.Modifiers == ModifierKeys.Control;

        if (parameter is Body body)
        {
            copyTextToClipboard(body.Name, ctrlPressed: false);
        }
        else if (parameter is StarSystem starSystem)
        {
            copyTextToClipboard(starSystem.Name, ctrlPressed: false);
        }
        else if (parameter is CopyToClipboardCommandParameter copyToClipboardCommandParameter)
        {
            copyTextToClipboard(copyToClipboardCommandParameter.Text, ctrlPressed, copyToClipboardCommandParameter.Popup);
        }
        else
        {
            if (!parameter.GetType().IsSubclassOf(typeof(FrameworkElement)))
            {
                return;
            }

            try
            {
                Popup? popup = (Popup?)(parameter as FrameworkElement)?.FindName("CopyToClipboardPopup");
                TextBlock? textBlock = (TextBlock?)(parameter as FrameworkElement)?.FindName("CopyToClipboardTextBlock");
                if (popup != null && textBlock != null)
                {
                    string text = textBlock.Text;
                    copyTextToClipboard(text, ctrlPressed, popup);
                }
            }
            catch (Exception exception)
            {
                log.Error("Could not copy text to clipboard", exception);
            }
        }
    }

    /// <summary>
    /// Copies the specified text to the clipboard and shows a popup if one is provided.
    /// </summary>
    /// <param name="text">The text to copy.</param>
    /// <param name="ctrlPressed">Whether the control key is pressed, triggering numeric-only extraction.</param>
    /// <param name="popup">An optional popup shown after copying.</param>
    private void copyTextToClipboard(string text, bool ctrlPressed, Popup? popup = null)
    {
        string clipboardText = text;
        if (ctrlPressed)
        {
            string numericText = Regex.Replace(text, "[^0-9,.]", "");
            if (numericText.Length > 0)
            {
                try
                {
                    clipboardText = Convert.ToDouble(numericText, CultureInfo.CurrentCulture).ToString();
                }
                catch (Exception exception)
                {
                    log.Warn($"Requested number, but unable to convert '{(numericText)}' to a number string", exception);
                }
            }
            else
            {
                log.Warn($"Requested number, but found no numeric digits in '{(text)}'");
            }
        }

        Clipboard.SetText(clipboardText);
        if (popup != null && popup.Child is Border border && border.Child is TextBlock textBlock)
        {
            textBlock.Text = "'" + clipboardText + "' copied to Clipboard";
            popup.Opened += Popup_Opened;
            popup.IsOpen = true;
        }
    }

    /// <summary>
    /// Closes the popup one second after it was opened.
    /// </summary>
    /// <param name="sender">The popup that was opened.</param>
    /// <param name="e">Event data for the opened event.</param>
    private void Popup_Opened(object? sender, EventArgs e)
    {
        var timer = PlatformServices.UiTimer?.CreateTimer(TimeSpan.FromSeconds(1.0));
        if (timer is null)
        {
            return;
        }
        timer.Tick += () =>
        {
            if (sender is Popup popup)
            {
                popup.Opened -= Popup_Opened;
                popup.IsOpen = false;
            }
            timer.Dispose();
        };
        timer.Start();
    }
}
