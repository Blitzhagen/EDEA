using System.Windows;

namespace EDEA;

/// <summary>
/// Freezable proxy object that provides a bindable <see cref="Data"/> property for XAML resources.
/// </summary>
public class BindingProxy : Freezable
{
    /// <summary>
    /// Identifies the <see cref="Data"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(object), typeof(BindingProxy), new UIPropertyMetadata(null));

    /// <summary>
    /// Gets or sets the data object exposed through the proxy.
    /// </summary>
    /// <value>The data object.</value>
    public object Data
    {
        get => GetValue(DataProperty);
        set => SetValue(DataProperty, value);
    }

    /// <summary>
    /// Creates a new instance of the <see cref="BindingProxy"/> class.
    /// </summary>
    /// <returns>A new <see cref="BindingProxy"/> instance.</returns>
    protected override Freezable CreateInstanceCore()
    {
        return new BindingProxy();
    }
}
