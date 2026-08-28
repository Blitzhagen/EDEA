using System.Windows;

namespace EDEA.Windows;

public partial class InputStringDialogWindow : Window
{
    public string Prompt
    {
        get => PromptTextBlock.Text;
        set => PromptTextBlock.Text = value;
    }

    public string Value
    {
        get => ValueTextBox.Text;
        set => ValueTextBox.Text = value;
    }

    public InputStringDialogWindow()
    {
        InitializeComponent();
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
