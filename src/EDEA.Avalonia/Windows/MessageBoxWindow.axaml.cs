using Avalonia.Controls;
using Avalonia.Interactivity;

namespace EDEA.Avalonia.Windows;

/// <summary>
/// Simple message box window for Avalonia.
/// </summary>
public partial class MessageBoxWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MessageBoxWindow"/> class.
    /// </summary>
    public MessageBoxWindow()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageBoxWindow"/> class with the specified title and message.
    /// </summary>
    /// <param name="title">The window title.</param>
    /// <param name="message">The message to display.</param>
    public MessageBoxWindow(string title, string message)
        : this()
    {
        Title = title;
        MessageTextBlock.Text = message;
    }

    /// <summary>
    /// Closes the message box.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The routed event data.</param>
    private void OkButton_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
