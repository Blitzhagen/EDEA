using System.Windows;

namespace EDEA.Commands;

public class CloseWindowCommand : CommandBase
{
    private readonly Window _window;

    public CloseWindowCommand(Window window)
    {
        _window = window;
    }

    public override void Execute(object? parameter)
    {
        _window.Close();
    }
}
