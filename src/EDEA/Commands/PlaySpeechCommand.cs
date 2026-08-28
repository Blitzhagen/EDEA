using EDEA.Services;
using EDEA.ViewModels;

namespace EDEA.Commands;

internal class PlaySpeechCommand : CommandBase
{
    private readonly PreferencesViewModel _preferencesViewModel;

    public PlaySpeechCommand(PreferencesViewModel preferencesViewModel)
    {
        _preferencesViewModel = preferencesViewModel;
    }

    public override void Execute(object? parameter)
    {
        SpeechProvider.SpeechOutputType? selectedSpeechOutput = _preferencesViewModel.GetSelectedSpeechOutput();
        if (selectedSpeechOutput.HasValue)
        {
            SpeechProvider.SpeakPreferencesSelection(selectedSpeechOutput.Value);
        }
    }
}
