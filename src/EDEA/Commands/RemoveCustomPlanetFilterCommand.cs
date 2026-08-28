using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using EDEA.Models;
using EDEA.ViewModels;
using log4net;

namespace EDEA.Commands;

public class RemoveCustomPlanetFilterCommand : CommandBase
{
    private static readonly ILog log = LogManager.GetLogger(typeof(RemoveCustomPlanetFilterCommand));

    private readonly PreferencesViewModel _preferencesViewModel;

    public RemoveCustomPlanetFilterCommand(PreferencesViewModel preferencesViewModel)
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
