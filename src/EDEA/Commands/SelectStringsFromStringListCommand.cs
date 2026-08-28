using System;
using EDEA.Enums;
using EDEA.ViewModels;
using EDEA.Windows;
using log4net;

namespace EDEA.Commands;

public class SelectStringsFromStringListCommand : CommandBase
{
    private static readonly ILog log = LogManager.GetLogger(typeof(SelectStringsFromStringListCommand));

    private readonly PreferencesViewModel _preferencesViewModel;

    public SelectStringsFromStringListCommand(PreferencesViewModel preferencesViewModel)
    {
        _preferencesViewModel = preferencesViewModel;
    }

    public override void Execute(object? parameter)
    {
        try
        {
            if (parameter is not UserSelectableInputStringListsKey userSelectableInputStringListsKey)
                return;

            PlanetClassificationViewModel selectedPlanetClassificationClone = _preferencesViewModel.SelectedPlanetClassificationClone;
            if (selectedPlanetClassificationClone == null)
                return;

            var allItems = Globals.UserSelectableInputStringLists[userSelectableInputStringListsKey];
            var selectedItems = selectedPlanetClassificationClone.GetInputStringListByKey(userSelectableInputStringListsKey);

            InputStringListDialogWindow inputStringListDialogWindow = new InputStringListDialogWindow("Select", allItems, selectedItems);
            if (inputStringListDialogWindow.ShowDialog() == true)
            {
                selectedPlanetClassificationClone.SetInputStringListByKey(userSelectableInputStringListsKey, inputStringListDialogWindow.SelectedInputStringList);
            }
        }
        catch (Exception exception)
        {
            log.Error("Error while using dialog for selecting string from string list", exception);
        }
    }
}
