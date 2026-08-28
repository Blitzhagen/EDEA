using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using EDEA.Services;
using EDEA.ViewModels;
using log4net;

namespace EDEA.Views;

/// <summary>
/// HUD view that displays a table of planetary bodies.
/// </summary>
public partial class BodyHudTableView : UserControl
{
    /// <summary>
    /// Provides access to the current star system data.
    /// </summary>
    private StarSystemProvider? starSystemProvider;

    /// <summary>
    /// View model that backs the body table.
    /// </summary>
    private BodyTableViewModel? bodyTableViewModel;

    /// <summary>
    /// Logger for this view.
    /// </summary>
    private static readonly ILog log = LogManager.GetLogger(typeof(BodyHudTableView));

    /// <summary>
    /// Initializes a new instance of the <see cref="BodyHudTableView"/> class.
    /// </summary>
    public BodyHudTableView()
    {
        InitializeComponent();
        Loaded += bodyHudTableView_Loaded;
    }

    /// <summary>
    /// Handles the <see cref="Loaded"/> event to attach the view model and scroll to the current planet.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The routed event data.</param>
    private void bodyHudTableView_Loaded(object? sender, RoutedEventArgs e)
    {
        bodyTableViewModel = (BodyTableViewModel)DataContext;
        starSystemProvider = bodyTableViewModel.StarSystemProvider;
        starSystemProvider.CurrentPlanetChanged += delegate
        {
            Application.Current?.Dispatcher.Invoke(delegate
            {
                scrollToCurrentPlanet();
            });
        };
        scrollToCurrentPlanet();
    }

    /// <summary>
    /// Scrolls the body data grid to the currently selected planet.
    /// </summary>
    private void scrollToCurrentPlanet()
    {
        try
        {
            if (bodyTableViewModel?.CurrentPlanet != null && BodyDataGrid?.Items != null)
            {
                int currentPlanetIndex = BodyDataGrid.Items.IndexOf(bodyTableViewModel.CurrentPlanet);
                if (currentPlanetIndex > -1)
                {
                    BodyDataGrid.ScrollIntoView(BodyDataGrid.Items[BodyDataGrid.Items.Count - 1]);
                    BodyDataGrid.UpdateLayout();
                    int index = (currentPlanetIndex > 0) ? (currentPlanetIndex - 1) : 0;
                    BodyDataGrid.ScrollIntoView(BodyDataGrid.Items[index]);
                    BodyDataGrid.UpdateLayout();
                }
            }
        }
        catch (Exception exception)
        {
            log.Error("Error on scrolling to current planet", exception);
        }
    }
}
