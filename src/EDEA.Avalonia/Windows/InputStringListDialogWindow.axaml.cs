using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace EDEA.Avalonia.Windows;

/// <summary>
/// Avalonia dialog window for selecting one or more string values from a list.
/// </summary>
public partial class InputStringListDialogWindow : Window
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
    /// Sets the collection of values the user can choose from.
    /// </summary>
    /// <value>The list of selectable values.</value>
    public IReadOnlyList<string> Items
    {
        set => ValuesListBox.ItemsSource = value;
    }

    /// <summary>
    /// Gets the currently selected values.
    /// </summary>
    /// <value>The selected values.</value>
    public List<string> SelectedInputStringList => ValuesListBox.SelectedItems?.OfType<string>().ToList() ?? new List<string>();

    /// <summary>
    /// Initializes a new instance of the <see cref="InputStringListDialogWindow"/> class.
    /// </summary>
    public InputStringListDialogWindow()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InputStringListDialogWindow"/> class with the specified title and items.
    /// </summary>
    /// <param name="title">The dialog title.</param>
    /// <param name="allItems">The complete list of available values.</param>
    /// <param name="selectedItems">The values that should be pre-selected.</param>
    public InputStringListDialogWindow(string title, IReadOnlyList<string> allItems, IReadOnlyList<string> selectedItems)
        : this()
    {
        Title = title;
        Prompt = $"Select one or more values for {title}:";
        Items = allItems;
        var allItemsList = allItems.ToList();
        foreach (string selectedItem in selectedItems)
        {
            int index = allItemsList.IndexOf(selectedItem);
            if (index >= 0)
            {
                ValuesListBox.SelectedItems?.Add(allItemsList[index]);
            }
        }
    }

    /// <summary>
    /// Confirms the dialog and returns the selected values.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The routed event data.</param>
    private void OkButton_Click(object? sender, RoutedEventArgs e)
    {
        Close(SelectedInputStringList);
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
    /// Handles the enter key to confirm and escape to cancel the dialog.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The key event data.</param>
    private void ValuesListBox_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            Close(SelectedInputStringList);
        }
        else if (e.Key == Key.Escape)
        {
            Close(null);
        }
    }
}
