using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using EDEA.ViewModels;
using HorizontalAlignment = global::Avalonia.Layout.HorizontalAlignment;
using VerticalAlignment = global::Avalonia.Layout.VerticalAlignment;

namespace EDEA.Avalonia.Views;

/// <summary>
/// Tooltip content for a body's rings.
/// </summary>
public partial class BodyRingsTooltip : UserControl
{
    private readonly Grid? _ringsGrid;
    private readonly Border? _ringsListBorder;

    /// <summary>
    /// Initializes a new instance of the <see cref="BodyRingsTooltip"/> class.
    /// </summary>
    public BodyRingsTooltip()
    {
        InitializeComponent();

        _ringsGrid = this.FindControl<Grid>("RingsGrid");
        _ringsListBorder = this.FindControl<Border>("RingsListBorder");

        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        UpdateRings();
    }

    private void UpdateRings()
    {
        if (_ringsGrid is null || _ringsListBorder is null)
        {
            return;
        }

        _ringsGrid.RowDefinitions.Clear();
        _ringsGrid.Children.Clear();

        if (DataContext is not BodyViewModel vm || !vm.HasRings)
        {
            _ringsListBorder.IsVisible = false;
            return;
        }

        _ringsListBorder.IsVisible = true;

        var rings = vm.Rings.ToList();
        var fontSize = this.TryFindResource("MainFontSize", out var fontSizeObj)
            ? fontSizeObj as double? ?? 12.0
            : 12.0;
        var foreground = this.TryFindResource("MainColor", out var foregroundObj)
            ? foregroundObj as IBrush
            : null;
        var separatorBrush = this.TryFindResource("CellHighlightColor", out var separatorObj)
            ? separatorObj as IBrush
            : null;

        for (int i = 0; i < rings.Count; i++)
        {
            var ring = rings[i];
            _ringsGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

            AddCell(_ringsGrid, i * 2, 0, ring.Name, fontSize, foreground);
            AddCell(_ringsGrid, i * 2, 1, ring.Type, fontSize, foreground);
            AddCell(_ringsGrid, i * 2, 2, ring.Width, fontSize, foreground);
            AddCell(_ringsGrid, i * 2, 3, ring.Density, fontSize, foreground);

            if (i < rings.Count - 1)
            {
                _ringsGrid.RowDefinitions.Add(new RowDefinition(new GridLength(1, GridUnitType.Pixel)));

                var separator = new Rectangle
                {
                    Height = 1,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                };

                if (separatorBrush is not null)
                {
                    separator.Fill = separatorBrush;
                }

                Grid.SetRow(separator, (i * 2) + 1);
                Grid.SetColumnSpan(separator, 4);
                _ringsGrid.Children.Add(separator);
            }
        }
    }

    private static void AddCell(Grid grid, int row, int column, string text, double fontSize, IBrush? foreground)
    {
        var textBlock = new TextBlock
        {
            Text = text,
            Padding = new Thickness(2, 4, 2, 5),
            FontSize = fontSize,
            VerticalAlignment = VerticalAlignment.Stretch,
        };

        if (foreground is not null)
        {
            textBlock.Foreground = foreground;
        }

        Grid.SetRow(textBlock, row);
        Grid.SetColumn(textBlock, column);
        grid.Children.Add(textBlock);
    }
}
