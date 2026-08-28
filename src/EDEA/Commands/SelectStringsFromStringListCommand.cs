using System;
using EDEA.Enums;
using EDEA.ViewModels;
using EDEA.Windows;
using log4net;

namespace EDEA.Commands;

/// <summary>
/// Command that opens a dialog to select strings from a predefined list for the selected planet classification.
/// </summary>
public class SelectStringsFromStringListCommand : CommandBase
{
    private static readonly ILog log = LogManager.GetLogger(typeof(SelectStringsFromStringListCommand));

    private readonly PreferencesViewModel _preferencesViewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectStringsFromStringListCommand"/> class.
    /// </summary>
    /// <param name="preferencesViewModel">The preferences view model that contains the selected planet classification.</param>
    public SelectStringsFromStringListCommand(PreferencesViewModel preferencesViewModel)
    {
        _preferencesViewModel = preferencesViewModel;
    }

    /// <summary>
    /// Opens the string selection dialog for the specified input list key.
    /// </summary>
    /// <param name="parameter">The <see cref="UserSelectableInputStringListsKey"/> that identifies the list to select from.</param>
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
