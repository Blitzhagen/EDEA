using EDEA.Services;
using EDEA.ViewModels;

namespace EDEA.Commands;

/// <summary>
/// Command that plays the currently selected speech output preview.
/// </summary>
internal class PlaySpeechCommand : CommandBase
{
    private readonly PreferencesViewModel _preferencesViewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlaySpeechCommand"/> class.
    /// </summary>
    /// <param name="preferencesViewModel">The preferences view model that provides the selected speech output.</param>
    public PlaySpeechCommand(PreferencesViewModel preferencesViewModel)
    {
        _preferencesViewModel = preferencesViewModel;
    }

    /// <summary>
    /// Plays the selected speech output using the speech provider.
    /// </summary>
    /// <param name="parameter">Data used by the command. Not used.</param>
    public override void Execute(object? parameter)
    {
        SpeechProvider.SpeechOutputType? selectedSpeechOutput = _preferencesViewModel.GetSelectedSpeechOutput();
        if (selectedSpeechOutput.HasValue)
        {
            SpeechProvider.SpeakPreferencesSelection(selectedSpeechOutput.Value);
        }
    }
}
