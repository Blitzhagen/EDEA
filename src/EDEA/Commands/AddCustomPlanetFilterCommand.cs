using EDEA.Models;
using EDEA.ViewModels;
using EDEA.Windows;

namespace EDEA.Commands;

/// <summary>
/// Command that adds a new custom planet classification filter to the preferences view model.
/// </summary>
public class AddCustomPlanetFilterCommand : CommandBase
{
    private readonly PreferencesViewModel _preferencesViewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="AddCustomPlanetFilterCommand"/> class.
    /// </summary>
    /// <param name="preferencesViewModel">The preferences view model that holds the planet filter collection.</param>
    public AddCustomPlanetFilterCommand(PreferencesViewModel preferencesViewModel)
    {
        _preferencesViewModel = preferencesViewModel;
    }

    /// <summary>
    /// Opens an input dialog to prompt for the criteria set name and adds a new active planet classification filter.
    /// </summary>
    /// <param name="parameter">Data used by the command. Not used.</param>
    public override void Execute(object? parameter)
    {
        InputStringDialogWindow inputStringDialogWindow = new InputStringDialogWindow
        {
            Prompt = "Criteria Set Name",
            Value = $"Criteria Set {_preferencesViewModel.PlanetClassificationClones.Count + 1}"
        };
        if (inputStringDialogWindow.ShowDialog() == true && !string.IsNullOrWhiteSpace(inputStringDialogWindow.Value))
        {
            _preferencesViewModel.addPlanetClassification(new PlanetClassification(inputStringDialogWindow.Value, isActive: true));
        }
    }
}
