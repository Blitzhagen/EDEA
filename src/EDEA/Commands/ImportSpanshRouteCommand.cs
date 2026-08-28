using System;
using System.Windows;
using EDEA.Services;
using EDEA.ViewModels;
using log4net;
using Microsoft.Win32;

namespace EDEA.Commands;

public class ImportSpanshRouteCommand : CommandBase
{
    private static readonly ILog log = LogManager.GetLogger(typeof(ImportSpanshRouteCommand));

    private readonly MainViewModel _mainViewModel;
    private readonly RouteProvider _routeProvider;

    public ImportSpanshRouteCommand(MainViewModel mainViewModel, RouteProvider routeProvider)
    {
        _mainViewModel = mainViewModel;
        _routeProvider = routeProvider;
    }

    public override void Execute(object? parameter)
    {
        _mainViewModel.OpenTabOfType(typeof(NavRouteTableViewModel), forceOpen: true);
        OpenFileDialog openFileDialog = new OpenFileDialog
        {
            Filter = "Spansh route files (*.json;*.csv)|*.json;*.csv",
            Title = "Import Spansh route"
        };
        if (openFileDialog.ShowDialog() == true)
        {
            if (!_routeProvider.ImportSpanshRouteFile(openFileDialog.FileName))
            {
                MessageBox.Show("This does not seem to be a valid Spansh route file.", "Error: Invalid file", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            _mainViewModel.RefreshMenuItems();
        }
    }
}
