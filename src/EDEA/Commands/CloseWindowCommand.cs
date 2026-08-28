using System.Windows;

namespace EDEA.Commands;

/// <summary>
/// Command that closes an associated <see cref="Window"/>.
/// </summary>
public class CloseWindowCommand : CommandBase
{
    private readonly Window _window;

    /// <summary>
    /// Initializes a new instance of the <see cref="CloseWindowCommand"/> class.
    /// </summary>
    /// <param name="window">The window to close.</param>
    public CloseWindowCommand(Window window)
    {
        _window = window;
    }

    /// <summary>
    /// Closes the associated window.
    /// </summary>
    /// <param name="parameter">Data used by the command. Not used.</param>
    public override void Execute(object? parameter)
    {
        _window.Close();
    }
}
