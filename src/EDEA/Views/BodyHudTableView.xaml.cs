using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using EDEA.Services;
using EDEA.ViewModels;
using log4net;

namespace EDEA.Views;

public partial class BodyHudTableView : UserControl
{
    private StarSystemProvider? starSystemProvider;

    private BodyTableViewModel? bodyTableViewModel;

    private static readonly ILog log = LogManager.GetLogger(typeof(BodyHudTableView));

    public BodyHudTableView()
    {
        InitializeComponent();
        Loaded += bodyHudTableView_Loaded;
    }

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
