using System;
using System.Globalization;
using System.Text.Json.Serialization;

namespace EDEA.Core.Drawing;

/// <summary>
/// Represents a color with alpha, red, green and blue components.
/// This is a platform-independent replacement for <see cref="System.Windows.Media.Color"/>.
/// </summary>
[JsonConverter(typeof(ColorJsonConverter))]
public readonly struct Color : IEquatable<Color>
{
    /// <summary>
    /// The alpha component.
    /// </summary>
    public byte A { get; }

    /// <summary>
    /// The red component.
    /// </summary>
    public byte R { get; }

    /// <summary>
    /// The green component.
    /// </summary>
    public byte G { get; }

    /// <summary>
    /// The blue component.
    /// </summary>
    public byte B { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Color"/> struct.
    /// </summary>
    /// <param name="a">The alpha component.</param>
    /// <param name="r">The red component.</param>
    /// <param name="g">The green component.</param>
    /// <param name="b">The blue component.</param>
    public Color(byte a, byte r, byte g, byte b)
    {
        A = a;
        R = r;
        G = g;
        B = b;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Color"/> struct.
    /// </summary>
    /// <param name="r">The red component.</param>
    /// <param name="g">The green component.</param>
    /// <param name="b">The blue component.</param>
    public Color(byte r, byte g, byte b)
        : this(255, r, g, b)
    {
    }

    /// <summary>
    /// Creates a color from ARGB components.
    /// </summary>
    /// <param name="a">The alpha component.</param>
    /// <param name="r">The red component.</param>
    /// <param name="g">The green component.</param>
    /// <param name="b">The blue component.</param>
    /// <returns>The created color.</returns>
    public static Color FromArgb(byte a, byte r, byte g, byte b) => new(a, r, g, b);

    /// <summary>
    /// Creates a color from RGB components.
    /// </summary>
    /// <param name="r">The red component.</param>
    /// <param name="g">The green component.</param>
    /// <param name="b">The blue component.</param>
    /// <returns>The created color.</returns>
    public static Color FromRgb(byte r, byte g, byte b) => new(r, g, b);

    /// <summary>
    /// Creates a color from a 32-bit ARGB value.
    /// </summary>
    /// <param name="argb">The 32-bit ARGB value.</param>
    /// <returns>The created color.</returns>
    public static Color FromArgb(uint argb)
    {
        byte a = (byte)((argb >> 24) & 0xFF);
        byte r = (byte)((argb >> 16) & 0xFF);
        byte g = (byte)((argb >> 8) & 0xFF);
        byte b = (byte)(argb & 0xFF);
        return new Color(a, r, g, b);
    }

    /// <summary>
    /// Parses a color from a hexadecimal string (e.g. "#FF112233" or "112233").
    /// </summary>
    /// <param name="hex">The hexadecimal color string.</param>
    /// <returns>The parsed color.</returns>
    /// <exception cref="FormatException">Thrown when the string is not a valid color.</exception>
    public static Color Parse(string hex)
    {
        if (string.IsNullOrWhiteSpace(hex))
        {
            throw new FormatException("Color string is empty.");
        }

        ReadOnlySpan<char> span = hex.AsSpan();
        if (span[0] == '#')
        {
            span = span.Slice(1);
        }

        if (span.Length == 6)
        {
            byte r = byte.Parse(span.Slice(0, 2), NumberStyles.HexNumber);
            byte g = byte.Parse(span.Slice(2, 2), NumberStyles.HexNumber);
            byte b = byte.Parse(span.Slice(4, 2), NumberStyles.HexNumber);
            return new Color(255, r, g, b);
        }

        if (span.Length == 8)
        {
            byte a = byte.Parse(span.Slice(0, 2), NumberStyles.HexNumber);
            byte r = byte.Parse(span.Slice(2, 2), NumberStyles.HexNumber);
            byte g = byte.Parse(span.Slice(4, 2), NumberStyles.HexNumber);
            byte b = byte.Parse(span.Slice(6, 2), NumberStyles.HexNumber);
            return new Color(a, r, g, b);
        }

        throw new FormatException($"Color string '{hex}' is not valid. Expected #AARRGGBB, #RRGGBB, AARRGGBB or RRGGBB.");
    }

    /// <summary>
    /// Converts the color to a 32-bit ARGB value.
    /// </summary>
    /// <returns>The 32-bit ARGB value.</returns>
    public uint ToArgb() => ((uint)A << 24) | ((uint)R << 16) | ((uint)G << 8) | B;

    /// <summary>
    /// Converts the color to a hexadecimal string with alpha (e.g. "#FF112233").
    /// </summary>
    /// <returns>The hexadecimal color string.</returns>
    public override string ToString() => $"#{ToArgb():X8}";

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns><c>true</c> if the current object is equal to the other parameter; otherwise, <c>false</c>.</returns>
    public bool Equals(Color other) => A == other.A && R == other.R && G == other.G && B == other.B;

    /// <summary>
    /// Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns><c>true</c> if the object and this instance are of the same type and represent the same value; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj) => obj is Color other && Equals(other);

    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
    public override int GetHashCode() => HashCode.Combine(A, R, G, B);

    /// <summary>
    /// Compares two colors for equality.
    /// </summary>
    /// <param name="left">The first color.</param>
    /// <param name="right">The second color.</param>
    /// <returns><c>true</c> if the colors are equal; otherwise, <c>false</c>.</returns>
    public static bool operator ==(Color left, Color right) => left.Equals(right);

    /// <summary>
    /// Compares two colors for inequality.
    /// </summary>
    /// <param name="left">The first color.</param>
    /// <param name="right">The second color.</param>
    /// <returns><c>true</c> if the colors are not equal; otherwise, <c>false</c>.</returns>
    public static bool operator !=(Color left, Color right) => !left.Equals(right);
}
