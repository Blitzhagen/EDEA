using System;

namespace EDEA.Services;

/// <summary>
/// Represents a timer that invokes a callback at regular intervals.
/// </summary>
public interface IUiTimer : IDisposable
{
    /// <summary>
    /// Gets or sets the interval between ticks.
    /// </summary>
    TimeSpan Interval { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the timer is running.
    /// </summary>
    bool IsEnabled { get; set; }

    /// <summary>
    /// Occurs when the timer interval has elapsed.
    /// </summary>
    event Action? Tick;

    /// <summary>
    /// Starts the timer.
    /// </summary>
    void Start();

    /// <summary>
    /// Stops the timer.
    /// </summary>
    void Stop();
}
