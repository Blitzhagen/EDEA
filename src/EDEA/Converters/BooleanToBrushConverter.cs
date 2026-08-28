using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace EDEA.Converters;

/// <summary>
/// Converts a boolean value to a brush, optionally inverting the result.
/// </summary>
public class BooleanToBrushConverter : IValueConverter
{
    /// <summary>
    /// Gets or sets the brush used when the value is <c>true</c>.
    /// </summary>
    /// <value>The brush for <c>true</c> values.</value>
    public Brush TrueBrush { get; set; } = Brushes.LimeGreen;

    /// <summary>
    /// Gets or sets the brush used when the value is <c>false</c>.
    /// </summary>
    /// <value>The brush for <c>false</c> values.</value>
    public Brush FalseBrush { get; set; } = Brushes.Gray;

    /// <summary>
    /// Converts a boolean value to a brush.
    /// </summary>
    /// <param name="value">The boolean value to convert.</param>
    /// <param name="targetType">The target type of the conversion.</param>
    /// <param name="parameter">A string value; if <c>"Invert"</c>, the boolean value is inverted before selecting the brush.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns><see cref="TrueBrush"/> if the value is <c>true</c>; otherwise <see cref="FalseBrush"/>.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var b = value is true;
        if (parameter as string == "Invert")
            b = !b;
        return b ? TrueBrush : FalseBrush;
    }

    /// <summary>
    /// Converts a brush back to a boolean value. Not supported.
    /// </summary>
    /// <param name="value">The value to convert back.</param>
    /// <param name="targetType">The target type of the conversion.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>Not supported; throws a <see cref="NotSupportedException"/>.</returns>
    /// <exception cref="NotSupportedException">Always thrown because the conversion is not supported.</exception>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
