using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using EDEA.Models;
using log4net;

namespace EDEA.Commands;

public class CopyToClipboardCommand : CommandBase
{
    private static readonly ILog log = LogManager.GetLogger(typeof(CopyToClipboardCommand));

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

    private void Popup_Opened(object? sender, EventArgs e)
    {
        DispatcherTimer timer = new DispatcherTimer();
        timer.Interval = TimeSpan.FromSeconds(1.0);
        timer.Start();
        timer.Tick += delegate
        {
            if (sender is Popup popup)
            {
                popup.Opened -= Popup_Opened;
                popup.IsOpen = false;
            }
            timer.Stop();
        };
    }
}
