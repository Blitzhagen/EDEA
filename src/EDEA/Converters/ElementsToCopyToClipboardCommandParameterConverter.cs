using System;
using System.Globalization;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using EDEA.Commands;
using log4net;

namespace EDEA.Converters;

/// <summary>
/// Multi-value converter that combines a text string and a popup into a <see cref="CopyToClipboardCommandParameter"/>.
/// </summary>
public class ElementsToCopyToClipboardCommandParameterConverter : IMultiValueConverter
{
    private static readonly ILog log = LogManager.GetLogger(typeof(ElementsToCopyToClipboardCommandParameterConverter));

    /// <summary>
    /// Converts the provided values into a <see cref="CopyToClipboardCommandParameter"/>.
    /// </summary>
    /// <param name="values">The values to convert. The first value is the text, the second is the popup.</param>
    /// <param name="targetType">The target type of the conversion.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>A <see cref="CopyToClipboardCommandParameter"/> instance, or <c>null</c> when conversion fails.</returns>
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        try
        {
            return new CopyToClipboardCommandParameter((string)values[0], (Popup)values[1]);
        }
        catch (Exception exception)
        {
            log.Error("Could not convert element to CopyToClipboardCommandParameter", exception);
        }

        return null!;
    }

    /// <summary>
    /// Converts a <see cref="CopyToClipboardCommandParameter"/> back to its original values. Not supported.
    /// </summary>
    /// <param name="value">The value to convert back.</param>
    /// <param name="targetTypes">The target types of the conversion.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>Not supported; throws a <see cref="NotSupportedException"/>.</returns>
    /// <exception cref="NotSupportedException">Always thrown because the conversion is not supported.</exception>
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
