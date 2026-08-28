using EDEA.Models;
using EDEA.ViewModels;
using EDEA.Windows;

namespace EDEA.Commands;

public class AddCustomPlanetFilterCommand : CommandBase
{
    private readonly PreferencesViewModel _preferencesViewModel;

    public AddCustomPlanetFilterCommand(PreferencesViewModel preferencesViewModel)
    {
        _preferencesViewModel = preferencesViewModel;
    }

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
