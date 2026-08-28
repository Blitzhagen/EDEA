using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using EDEA.Models;
using EDEA.ViewModels;
using log4net;

namespace EDEA.Commands;

/// <summary>
/// Command that removes a custom planet classification filter after user confirmation.
/// </summary>
public class RemoveCustomPlanetFilterCommand : CommandBase
{
    private static readonly ILog log = LogManager.GetLogger(typeof(RemoveCustomPlanetFilterCommand));

    private readonly PreferencesViewModel _preferencesViewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="RemoveCustomPlanetFilterCommand"/> class.
    /// </summary>
    /// <param name="preferencesViewModel">The preferences view model that holds the planet filter collection.</param>
    public RemoveCustomPlanetFilterCommand(PreferencesViewModel preferencesViewModel)
    {
        _preferencesViewModel = preferencesViewModel;
    }

    /// <summary>
    /// Prompts the user for confirmation and removes the selected planet classification filter.
    /// </summary>
    /// <param name="parameter">The <see cref="PlanetClassificationViewModel"/> to remove.</param>
    public override void Execute(object? parameter)
    {
        try
        {
            if (parameter is not PlanetClassificationViewModel viewModel)
                return;

            PlanetClassification? planetClassification = viewModel.PlanetClassification;
            if (planetClassification == null)
                return;

            List<PlanetClassificationViewModel> dependentCriteriaSets = _preferencesViewModel.PlanetClassificationClones.Where(child => child.ParentPlanetClassificationId == planetClassification.Id).ToList();
            string additionalInfo = string.Empty;
            if (dependentCriteriaSets.Count > 0)
            {
                additionalInfo = $"\nIt is used in {dependentCriteriaSets.Count} other set{((dependentCriteriaSets.Count > 1) ? "s" : string.Empty)} as a Parent Criteria Set and is also removed there.";
            }
            string messageBoxText = "The criteria set '" + planetClassification.Name + "' will be permanently deleted." + additionalInfo;
            string caption = "Deleting Criteria Set";
            MessageBoxButton button = MessageBoxButton.YesNoCancel;
            MessageBoxImage icon = MessageBoxImage.Exclamation;
            if (MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.No) != MessageBoxResult.Yes)
            {
                return;
            }
            _preferencesViewModel.removePlanetClassification(planetClassification);
            if (dependentCriteriaSets.Count <= 0)
            {
                return;
            }
            foreach (PlanetClassificationViewModel dependentCriteriaSet in dependentCriteriaSets)
            {
                dependentCriteriaSet.ParentPlanetClassificationId = null!;
            }
        }
        catch (Exception exception)
        {
            log.Error("Error on removing criteria set!", exception);
        }
    }
}
