namespace EDEA.Core.Windowing;

/// <summary>
/// Represents the position and size of a screen rectangle.
/// </summary>
public readonly struct ScreenRect
{
    /// <summary>
    /// The X coordinate.
    /// </summary>
    public double X { get; }

    /// <summary>
    /// The Y coordinate.
    /// </summary>
    public double Y { get; }

    /// <summary>
    /// The rectangle width.
    /// </summary>
    public double Width { get; }

    /// <summary>
    /// The rectangle height.
    /// </summary>
    public double Height { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScreenRect"/> struct.
    /// </summary>
    /// <param name="x">The X coordinate.</param>
    /// <param name="y">The Y coordinate.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    public ScreenRect(double x, double y, double width, double height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }
}
