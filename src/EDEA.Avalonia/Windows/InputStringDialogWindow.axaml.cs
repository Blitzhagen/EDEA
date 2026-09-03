using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace EDEA.Avalonia.Windows;

/// <summary>
/// Avalonia dialog window for entering a single string value.
/// </summary>
public partial class InputStringDialogWindow : Window
{
    /// <summary>
    /// Gets or sets the prompt text displayed to the user.
    /// </summary>
    /// <value>The prompt text.</value>
    public string Prompt
    {
        get => PromptTextBlock.Text ?? string.Empty;
        set => PromptTextBlock.Text = value;
    }

    /// <summary>
    /// Gets or sets the value entered by the user.
    /// </summary>
    /// <value>The entered value.</value>
    public string Value
    {
        get => ValueTextBox.Text ?? string.Empty;
        set => ValueTextBox.Text = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InputStringDialogWindow"/> class.
    /// </summary>
    public InputStringDialogWindow()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Confirms the dialog and returns the entered value.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The routed event data.</param>
    private void OkButton_Click(object? sender, RoutedEventArgs e)
    {
        Close(Value);
    }

    /// <summary>
    /// Cancels the dialog and returns <c>null</c>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The routed event data.</param>
    private void CancelButton_Click(object? sender, RoutedEventArgs e)
    {
        Close(null);
    }

    /// <summary>
    /// Handles the enter key to confirm the dialog.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The key event data.</param>
    private void ValueTextBox_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            Close(Value);
        }
        else if (e.Key == Key.Escape)
        {
            Close(null);
        }
    }
}
