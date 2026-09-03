using System;
using System.Threading.Tasks;
using Avalonia.Threading;
using EDEA.Services;

namespace EDEA.Avalonia.Services;

/// <summary>
/// Avalonia implementation of <see cref="IDispatcher"/>.
/// </summary>
public sealed class AvaloniaDispatcher : EDEA.Services.IDispatcher
{
    /// <summary>
    /// Invokes the specified action synchronously on the UI thread.
    /// </summary>
    /// <param name="action">The action to invoke.</param>
    public void Invoke(Action action)
    {
        if (Dispatcher.UIThread.CheckAccess())
        {
            action();
        }
        else
        {
            Dispatcher.UIThread.Invoke(action);
        }
    }

    /// <summary>
    /// Invokes the specified action asynchronously on the UI thread.
    /// </summary>
    /// <param name="action">The action to invoke.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task InvokeAsync(Func<Task> action)
    {
        await Dispatcher.UIThread.InvokeAsync(action);
    }
}
