using Avalonia;
using Avalonia.Controls;

namespace EDEA.Avalonia.Helpers;

/// <summary>
/// Attached behavior that ensures a <see cref="DataGrid"/> only sorts by a single column at a time.
/// </summary>
public class DataGridSingleSortBehavior : AvaloniaObject
{
    /// <summary>
    /// Identifies the <see cref="IsEnabledProperty"/> attached property.
    /// </summary>
    public static readonly AttachedProperty<bool> IsEnabledProperty =
        AvaloniaProperty.RegisterAttached<DataGridSingleSortBehavior, DataGrid, bool>(
            "IsEnabled",
            defaultValue: false);

    /// <summary>
    /// Gets a value indicating whether single-sort behavior is enabled for the specified <see cref="DataGrid"/>.
    /// </summary>
    public static bool GetIsEnabled(DataGrid element) => element.GetValue(IsEnabledProperty);

    /// <summary>
    /// Sets a value indicating whether single-sort behavior is enabled for the specified <see cref="DataGrid"/>.
    /// </summary>
    public static void SetIsEnabled(DataGrid element, bool value) => element.SetValue(IsEnabledProperty, value);

    static DataGridSingleSortBehavior()
    {
        IsEnabledProperty.Changed.AddClassHandler<DataGrid>(OnIsEnabledChanged);
    }

    private static void OnIsEnabledChanged(DataGrid element, AvaloniaPropertyChangedEventArgs args)
    {
        if (args.NewValue is true)
        {
            element.Sorting += OnSorting;
        }
        else
        {
            element.Sorting -= OnSorting;
        }
    }

    private static void OnSorting(object? sender, DataGridColumnEventArgs e)
    {
        if (sender is not DataGrid dataGrid || e.Column is null)
        {
            return;
        }

        foreach (var column in dataGrid.Columns)
        {
            if (column != e.Column)
            {
                column.ClearSort();
            }
        }
    }
}
