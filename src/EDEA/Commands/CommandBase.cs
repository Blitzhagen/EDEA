using System;
using System.Windows.Input;

namespace EDEA.Commands;

public abstract class CommandBase : ICommand
{
    public event EventHandler? CanExecuteChanged = delegate
    {
    };

    public virtual bool CanExecute(object? parameter)
    {
        return true;
    }

    public abstract void Execute(object? parameter);

    protected void OnCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
