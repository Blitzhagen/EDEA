using System;
using System.Globalization;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using EDEA.Commands;
using log4net;

namespace EDEA.Converters;

public class ElementsToCopyToClipboardCommandParameterConverter : IMultiValueConverter
{
    private static readonly ILog log = LogManager.GetLogger(typeof(ElementsToCopyToClipboardCommandParameterConverter));

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

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
