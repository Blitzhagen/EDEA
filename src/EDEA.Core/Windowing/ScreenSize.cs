namespace EDEA.Core.Windowing;

/// <summary>
/// Represents the size of a screen.
/// </summary>
public readonly struct ScreenSize
{
    /// <summary>
    /// The screen width.
    /// </summary>
    public double Width { get; }

    /// <summary>
    /// The screen height.
    /// </summary>
    public double Height { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScreenSize"/> struct.
    /// </summary>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    public ScreenSize(double width, double height)
    {
        Width = width;
        Height = height;
    }
}
