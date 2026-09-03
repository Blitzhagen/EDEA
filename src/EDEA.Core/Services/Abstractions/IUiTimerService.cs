using System;

namespace EDEA.Services;

/// <summary>
/// Abstraction for creating platform-independent UI timers.
/// </summary>
public interface IUiTimerService
{
    /// <summary>
    /// Creates a new timer with the specified interval and optional tick callback.
    /// </summary>
    /// <param name="interval">The interval between ticks.</param>
    /// <param name="onTick">Optional callback invoked on each tick.</param>
    /// <returns>The created timer.</returns>
    IUiTimer CreateTimer(TimeSpan interval, Action? onTick = null);
}
