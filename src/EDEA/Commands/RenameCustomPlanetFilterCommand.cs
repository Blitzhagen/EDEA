using System;
using System.Reflection;
using EDEA.Models;
using EDEA.ViewModels;
using EDEA.Windows;
using log4net;

namespace EDEA.Commands;

public class RenameCustomPlanetFilterCommand : CommandBase
{
    private static readonly ILog log = LogManager.GetLogger(typeof(RenameCustomPlanetFilterCommand));

    private readonly PreferencesViewModel _preferencesViewModel;

    public RenameCustomPlanetFilterCommand(PreferencesViewModel preferencesViewModel)
    {
        _preferencesViewModel = preferencesViewModel;
    }

    public override void Execute(object? parameter)
    {
        try
        {
            if (parameter is not PlanetClassificationViewModel viewModel)
                return;

            PlanetClassification? planetClassification = viewModel.PlanetClassification;
            if (planetClassification == null)
                return;

            InputStringDialogWindow inputStringDialogWindow = new InputStringDialogWindow
            {
                Prompt = "Criteria Set Name",
                Value = planetClassification.Name
            };
            if (inputStringDialogWindow.ShowDialog() == true && !string.IsNullOrWhiteSpace(inputStringDialogWindow.Value))
            {
                _preferencesViewModel.renamePlanetClassification(planetClassification, inputStringDialogWindow.Value);
            }
        }
        catch (Exception exception)
        {
            log.Error("Error on renaming criteria set!", exception);
        }
    }
}
