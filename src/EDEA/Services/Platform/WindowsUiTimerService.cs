using System;
using System.Windows.Threading;
using EDEA.Services;

namespace EDEA.Services.Platform;

/// <summary>
/// WPF implementation of <see cref="IUiTimerService"/> using <see cref="DispatcherTimer"/>.
/// </summary>
public sealed class WindowsUiTimerService : IUiTimerService
{
    /// <summary>
    /// Creates a new timer with the specified interval and optional tick callback.
    /// </summary>
    /// <param name="interval">The interval between ticks.</param>
    /// <param name="onTick">Optional callback invoked on each tick.</param>
    /// <returns>The created timer.</returns>
    public IUiTimer CreateTimer(TimeSpan interval, Action? onTick = null)
    {
        var timer = new WindowsUiTimer(interval);
        if (onTick is not null)
        {
            timer.Tick += onTick;
        }
        return timer;
    }

    private sealed class WindowsUiTimer : IUiTimer
    {
        private readonly DispatcherTimer _timer;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsUiTimer"/> class.
        /// </summary>
        /// <param name="interval">The timer interval.</param>
        public WindowsUiTimer(TimeSpan interval)
        {
            _timer = new DispatcherTimer(interval, DispatcherPriority.Background, OnTick, System.Windows.Application.Current.Dispatcher);
        }

        /// <summary>
        /// Occurs when the timer interval has elapsed.
        /// </summary>
        public event Action? Tick;

        /// <summary>
        /// Gets or sets the interval between ticks.
        /// </summary>
        public TimeSpan Interval
        {
            get => _timer.Interval;
            set => _timer.Interval = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the timer is running.
        /// </summary>
        public bool IsEnabled
        {
            get => _timer.IsEnabled;
            set
            {
                if (value)
                {
                    _timer.Start();
                }
                else
                {
                    _timer.Stop();
                }
            }
        }

        /// <summary>
        /// Starts the timer.
        /// </summary>
        public void Start() => _timer.Start();

        /// <summary>
        /// Stops the timer.
        /// </summary>
        public void Stop() => _timer.Stop();

        /// <summary>
        /// Releases resources used by the timer.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;
            _timer.Stop();
            Tick = null;
        }

        private void OnTick(object? sender, EventArgs e)
        {
            Tick?.Invoke();
        }
    }
}
