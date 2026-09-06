using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using EDEA.Avalonia.ViewModels;
using EDEA.Services;

namespace EDEA.Avalonia.Windows;

/// <summary>
/// Avalonia window for importing journal history data.
/// </summary>
public partial class JournalHistoryImportWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="JournalHistoryImportWindow"/> class.
    /// </summary>
    public JournalHistoryImportWindow()
    {
        InitializeComponent();
        DataContext = new JournalHistoryImportViewModel(JournalHistoryImporter.Current);
        Opened += JournalHistoryImportWindow_Opened;
    }

    /// <summary>
    /// Initializes the view model when the window is opened.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private async void JournalHistoryImportWindow_Opened(object? sender, EventArgs e)
    {
        if (DataContext is JournalHistoryImportViewModel viewModel)
        {
            await viewModel.InitializeAsync();
        }
    }

    /// <summary>
    /// Starts the journal history import.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The routed event data.</param>
    private void StartImportButton_Click(object? sender, RoutedEventArgs e)
    {
        (DataContext as JournalHistoryImportViewModel)?.StartImport();
    }

    /// <summary>
    /// Cancels the import and closes the window.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The routed event data.</param>
    private void StopImportCloseButton_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is JournalHistoryImportViewModel viewModel && !viewModel.IsImportCompleted)
        {
            viewModel.CancelImport();
        }
        Close();
    }
}
