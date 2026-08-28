using System.Windows;

namespace EDEA.Windows;

/// <summary>
/// Dialog window for entering a single string value.
/// </summary>
public partial class InputStringDialogWindow : Window
{
    /// <summary>
    /// Gets or sets the prompt text displayed to the user.
    /// </summary>
    /// <value>The prompt text.</value>
    public string Prompt
    {
        get => PromptTextBlock.Text;
        set => PromptTextBlock.Text = value;
    }

    /// <summary>
    /// Gets or sets the value entered by the user.
    /// </summary>
    /// <value>The entered value.</value>
    public string Value
    {
        get => ValueTextBox.Text;
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
    /// Confirms the dialog and sets the result to <c>true</c>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The routed event data.</param>
    private void OkButton_Click(object? sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }

    /// <summary>
    /// Cancels the dialog and sets the result to <c>false</c>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The routed event data.</param>
    private void CancelButton_Click(object? sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
