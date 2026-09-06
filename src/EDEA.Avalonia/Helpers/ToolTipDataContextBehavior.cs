using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;

namespace EDEA.Avalonia.Helpers;

/// <summary>
/// Forwards the <see cref="Control.DataContext"/> of the element that owns a
/// <see cref="ToolTip"/> to the tooltip control and its content. This is required because
/// tooltip content lives in a popup and does not inherit the logical parent's data context.
/// </summary>
public static class ToolTipDataContextBehavior
{
    private static readonly AttachedProperty<ToolTip?>? ToolTipProperty;
    private static readonly ConditionalWeakTable<Control, IDisposable> _dataContextSubscriptions = new();

    static ToolTipDataContextBehavior()
    {
        var toolTipPropertyField = typeof(ToolTip).GetField(
            "ToolTipProperty",
            BindingFlags.NonPublic | BindingFlags.Static);

        ToolTipProperty = toolTipPropertyField?.GetValue(null) as AttachedProperty<ToolTip?>;

        ToolTip.TipProperty.Changed.Subscribe(new PropertyObserver<AvaloniaPropertyChangedEventArgs<object?>>(OnTipChanged));
        ToolTip.IsOpenProperty.Changed.Subscribe(new PropertyObserver<AvaloniaPropertyChangedEventArgs<bool>>(OnIsOpenChanged));
    }

    /// <summary>
    /// Ensures the static constructor has run and the behavior is active.
    /// </summary>
    public static void Initialize()
    {
        // Static constructor performs all wiring; this call just triggers it.
    }

    private static void OnTipChanged(AvaloniaPropertyChangedEventArgs<object?> e)
    {
        if (e.Property != ToolTip.TipProperty || e.Sender is not Control control)
        {
            return;
        }

        UpdateTipDataContext(control);
    }

    private static void OnIsOpenChanged(AvaloniaPropertyChangedEventArgs<bool> e)
    {
        if (e.Property != ToolTip.IsOpenProperty || e.Sender is not Control control)
        {
            return;
        }

        if (e.NewValue.GetValueOrDefault())
        {
            UpdateTipDataContext(control);

            if (_dataContextSubscriptions.TryGetValue(control, out var oldSubscription))
            {
                _dataContextSubscriptions.Remove(control);
                oldSubscription?.Dispose();
            }

            var weakControl = new WeakReference<Control>(control);
            var subscription = control.GetObservable(Control.DataContextProperty)
                .Subscribe(new PropertyObserver<object?>(_ =>
                {
                    if (weakControl.TryGetTarget(out var targetControl))
                    {
                        UpdateTipDataContext(targetControl);
                    }
                }));

            _dataContextSubscriptions.Add(control, subscription);
        }
        else
        {
            if (_dataContextSubscriptions.TryGetValue(control, out var subscription))
            {
                _dataContextSubscriptions.Remove(control);
                subscription?.Dispose();
            }
        }
    }

    private static void UpdateTipDataContext(Control control)
    {
        if (control.DataContext is null)
        {
            return;
        }

        if (ToolTipProperty is not null && control.GetValue(ToolTipProperty) is { } toolTip)
        {
            toolTip.DataContext = control.DataContext;

            if (toolTip.Content is ContentControl contentToolTip && contentToolTip.Content is null)
            {
                contentToolTip.Content = control.DataContext;
            }
        }

        if (ToolTip.GetTip(control) is Control tipControl)
        {
            tipControl.DataContext = control.DataContext;

            if (tipControl is ContentControl contentControl && contentControl.Content is null)
            {
                contentControl.Content = control.DataContext;
            }
        }
    }

    private sealed class PropertyObserver<T> : IObserver<T>
    {
        private readonly Action<T> _onNext;

        public PropertyObserver(Action<T> onNext)
        {
            _onNext = onNext;
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(T value)
        {
            _onNext(value);
        }
    }
}
