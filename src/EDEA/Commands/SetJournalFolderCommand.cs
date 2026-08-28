using System;
using System.IO;
using System.Windows;
using EDEA;
using EDEA.ViewModels;
using Microsoft.Win32;

namespace EDEA.Commands;

public class SetJournalFolderCommand : CommandBase
{
    private readonly PreferencesViewModel _preferencesViewModel;

    public SetJournalFolderCommand(PreferencesViewModel preferencesViewModel)
    {
        _preferencesViewModel = preferencesViewModel;
    }

    public override void Execute(object? parameter)
    {
        OpenFolderDialog openFolderDialog = new OpenFolderDialog();
        openFolderDialog.Title = "Please select the Elite Dangerous Save Game folder where your Journal*.log files are stored.";
        openFolderDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        if (Directory.Exists(Preferences.Other.EdSavedGamePath))
        {
            openFolderDialog.InitialDirectory = Preferences.Other.EdSavedGamePath;
        }
        if (openFolderDialog.ShowDialog() != true)
        {
            return;
        }
        if (Helpsters.GetLatestJournalFile(openFolderDialog.FolderName) != null)
        {
            if (!Preferences.Other.EdSavedGamePath.Equals(openFolderDialog.FolderName))
            {
                Preferences.Other.EdSavedGamePath = openFolderDialog.FolderName;
            }
        }
        else
        {
            string caption = "Missing Journal Files!";
            MessageBoxButton button = MessageBoxButton.OK;
            MessageBoxImage icon = MessageBoxImage.Hand;
            MessageBox.Show("The selected folder does not contain any Journal*.log files and will not be used.\n\nPlease set the appropriate folder so that EDEA works correctly.", caption, button, icon, MessageBoxResult.OK);
        }
    }
}
