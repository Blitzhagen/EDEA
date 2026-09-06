using System;
using System.ComponentModel;
using System.Linq;
using Avalonia.Controls;
using EDEA.ViewModels;

namespace EDEA.Avalonia.Views;

/// <summary>
/// Avalonia view for the genus HUD table.
/// </summary>
public partial class GenusHudTableView : UserControl
{
    private GenusTableViewModel? _viewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenusHudTableView"/> class.
    /// </summary>
    public GenusHudTableView()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (_viewModel is not null)
        {
            _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }

        _viewModel = DataContext as GenusTableViewModel;

        if (_viewModel is not null)
        {
            _viewModel.PropertyChanged += OnViewModelPropertyChanged;
            UpdateScanDistanceColumnVisibility();
        }
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(GenusTableViewModel.GeneraInAnalysisAvailabe))
        {
            UpdateScanDistanceColumnVisibility();
        }
    }

    private void UpdateScanDistanceColumnVisibility()
    {
        if (_viewModel is null)
        {
            return;
        }

        bool visible = _viewModel.GeneraInAnalysisAvailabe;
        var scanDistanceColumns = GenusDataGrid.Columns
            .Where(c => c.SortMemberPath is "CurrentDistanceToLocationAt1stScanSort" or "CurrentDistanceToLocationAt2ndScanSort")
            .ToList();

        foreach (var column in scanDistanceColumns)
        {
            column.IsVisible = visible;
        }
    }
}
