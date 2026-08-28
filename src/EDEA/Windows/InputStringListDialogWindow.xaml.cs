using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace EDEA.Windows;

public partial class InputStringListDialogWindow : Window
{
    public string Prompt
    {
        get => PromptTextBlock.Text;
        set => PromptTextBlock.Text = value;
    }

    public IReadOnlyList<string> Items
    {
        set
        {
            ValuesListBox.ItemsSource = value;
        }
    }

    public List<string> SelectedInputStringList => ValuesListBox.SelectedItems.OfType<string>().ToList();

    public InputStringListDialogWindow()
    {
        InitializeComponent();
    }

    public InputStringListDialogWindow(string title, IReadOnlyList<string> allItems, IReadOnlyList<string> selectedItems)
        : this()
    {
        Title = title;
        Prompt = $"Select one or more values for {title}:";
        Items = allItems;
        ValuesListBox.SelectionChanged += (s, e) => { };
        var allItemsList = allItems.ToList();
        foreach (string selectedItem in selectedItems)
        {
            int index = allItemsList.IndexOf(selectedItem);
            if (index >= 0)
            {
                ValuesListBox.SelectedItems.Add(ValuesListBox.Items[index]);
            }
        }
    }

    private void OkButton_Click(object? sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }

    private void CancelButton_Click(object? sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
