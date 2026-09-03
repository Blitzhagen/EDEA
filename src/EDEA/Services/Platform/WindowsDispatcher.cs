using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using EDEA.Services;

namespace EDEA.Services.Platform;

/// <summary>
/// WPF implementation of <see cref="IDispatcher"/>.
/// </summary>
public sealed class WindowsDispatcher : IDispatcher
{
    /// <summary>
    /// The WPF dispatcher to use.
    /// </summary>
    private readonly Dispatcher _dispatcher;

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowsDispatcher"/> class.
    /// </summary>
    public WindowsDispatcher()
        : this(Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowsDispatcher"/> class.
    /// </summary>
    /// <param name="dispatcher">The WPF dispatcher to use.</param>
    public WindowsDispatcher(Dispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    /// <summary>
    /// Invokes the specified action synchronously on the UI thread.
    /// </summary>
    /// <param name="action">The action to invoke.</param>
    public void Invoke(Action action)
    {
        _dispatcher.Invoke(action);
    }

    /// <summary>
    /// Invokes the specified action asynchronously on the UI thread.
    /// </summary>
    /// <param name="action">The action to invoke.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task InvokeAsync(Func<Task> action)
    {
        await _dispatcher.InvokeAsync(action).Task.ConfigureAwait(false);
    }
}
