using System;
using System.Reflection;
using EDEA.Models;
using EDEA.ViewModels;
using EDEA.Windows;
using log4net;

namespace EDEA.Commands;

/// <summary>
/// Command that renames a custom planet classification filter.
/// </summary>
public class RenameCustomPlanetFilterCommand : CommandBase
{
    private static readonly ILog log = LogManager.GetLogger(typeof(RenameCustomPlanetFilterCommand));

    private readonly PreferencesViewModel _preferencesViewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="RenameCustomPlanetFilterCommand"/> class.
    /// </summary>
    /// <param name="preferencesViewModel">The preferences view model that holds the planet filter collection.</param>
    public RenameCustomPlanetFilterCommand(PreferencesViewModel preferencesViewModel)
    {
        _preferencesViewModel = preferencesViewModel;
    }

    /// <summary>
    /// Opens a dialog to prompt for a new name and renames the selected planet classification filter.
    /// </summary>
    /// <param name="parameter">The <see cref="PlanetClassificationViewModel"/> to rename.</param>
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
