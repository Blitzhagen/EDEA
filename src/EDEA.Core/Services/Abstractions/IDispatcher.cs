using System;
using System.Threading.Tasks;

namespace EDEA.Services;

/// <summary>
/// Abstraction for marshalling calls to the UI thread.
/// </summary>
public interface IDispatcher
{
    /// <summary>
    /// Invokes the specified action synchronously on the UI thread.
    /// </summary>
    /// <param name="action">The action to invoke.</param>
    void Invoke(Action action);

    /// <summary>
    /// Invokes the specified action asynchronously on the UI thread.
    /// </summary>
    /// <param name="action">The action to invoke.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task InvokeAsync(Func<Task> action);
}
