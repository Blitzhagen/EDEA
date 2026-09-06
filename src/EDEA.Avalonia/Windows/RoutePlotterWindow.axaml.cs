using Avalonia.Controls;
using EDEA.Avalonia.ViewModels;

namespace EDEA.Avalonia.Windows;

/// <summary>
/// Avalonia window for the neutron route plotter.
/// </summary>
public partial class RoutePlotterWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RoutePlotterWindow"/> class for the designer.
    /// </summary>
    public RoutePlotterWindow()
        : this(null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RoutePlotterWindow"/> class.
    /// </summary>
    /// <param name="viewModel">The route plotter view model.</param>
    public RoutePlotterWindow(RoutePlotterViewModel? viewModel)
    {
        InitializeComponent();
        if (viewModel == null)
        {
            return;
        }

        DataContext = viewModel;
        Opened += (_, _) => viewModel.OnWindowOpened();
        Closed += (_, _) => viewModel.OnWindowClosed();
        viewModel.CloseRequested += (_, _) => Close();
    }
}
