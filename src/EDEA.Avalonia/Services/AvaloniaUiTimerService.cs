using System;
using Avalonia.Threading;
using EDEA.Services;

namespace EDEA.Avalonia.Services;

/// <summary>
/// Avalonia implementation of <see cref="IUiTimerService"/>.
/// </summary>
public sealed class AvaloniaUiTimerService : IUiTimerService
{
    /// <summary>
    /// Creates a new timer with the specified interval and optional tick callback.
    /// </summary>
    /// <param name="interval">The interval between ticks.</param>
    /// <param name="onTick">Optional callback invoked on each tick.</param>
    /// <returns>The created timer.</returns>
    public IUiTimer CreateTimer(TimeSpan interval, Action? onTick = null)
    {
        var timer = new AvaloniaUiTimer(interval);
        if (onTick != null)
        {
            timer.Tick += onTick;
        }

        return timer;
    }

    /// <summary>
    /// Avalonia <see cref="DispatcherTimer"/> wrapper implementing <see cref="IUiTimer"/>.
    /// </summary>
    private sealed class AvaloniaUiTimer : IUiTimer
    {
        private readonly DispatcherTimer _timer;

        /// <summary>
        /// Initializes a new instance of the <see cref="AvaloniaUiTimer"/> class.
        /// </summary>
        /// <param name="interval">The interval between ticks.</param>
        public AvaloniaUiTimer(TimeSpan interval)
        {
            _timer = new DispatcherTimer { Interval = interval };
            _timer.Tick += (s, e) => Tick?.Invoke();
        }

        /// <inheritdoc />
        public TimeSpan Interval
        {
            get => _timer.Interval;
            set => _timer.Interval = value;
        }

        /// <inheritdoc />
        public bool IsEnabled
        {
            get => _timer.IsEnabled;
            set => _timer.IsEnabled = value;
        }

        /// <inheritdoc />
        public event Action? Tick;

        /// <inheritdoc />
        public void Start() => _timer.Start();

        /// <inheritdoc />
        public void Stop() => _timer.Stop();

        /// <inheritdoc />
        public void Dispose()
        {
            _timer.Stop();
        }
    }
}
