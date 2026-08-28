using System;
using System.Windows.Input;

namespace EDEA.Commands;

/// <summary>
/// Base class for commands providing a default implementation of <see cref="ICommand"/>.
/// </summary>
public abstract class CommandBase : ICommand
{
    /// <summary>
    /// Occurs when changes that affect whether the command can execute are detected.
    /// </summary>
    public event EventHandler? CanExecuteChanged = delegate
    {
    };

    /// <summary>
    /// Determines whether the command can execute in its current state.
    /// </summary>
    /// <param name="parameter">Data used by the command. If not required, the parameter can be set to <c>null</c>.</param>
    /// <returns><c>true</c> if the command can execute; otherwise, <c>false</c>.</returns>
    public virtual bool CanExecute(object? parameter)
    {
        return true;
    }

    /// <summary>
    /// Executes the command logic.
    /// </summary>
    /// <param name="parameter">Data used by the command. If not required, the parameter can be set to <c>null</c>.</param>
    public abstract void Execute(object? parameter);

    /// <summary>
    /// Raises the <see cref="CanExecuteChanged"/> event.
    /// </summary>
    protected void OnCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
