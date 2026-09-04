using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EDEA.Core.Drawing;

/// <summary>
/// Converts <see cref="Color"/> values to and from JSON.
/// </summary>
public class ColorJsonConverter : JsonConverter<Color>
{
    /// <summary>
    /// Reads and converts a <see cref="Color"/> from JSON.
    /// </summary>
    public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var hex = reader.GetString() ?? string.Empty;
            if (string.IsNullOrEmpty(hex))
            {
                return default;
            }

            return Color.Parse(hex);
        }

        if (reader.TokenType != JsonTokenType.StartObject)
        {
            return default;
        }

        byte a = 255;
        byte r = 0;
        byte g = 0;
        byte b = 0;
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                break;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                continue;
            }

            var propertyName = reader.GetString();
            reader.Read();
            switch (propertyName?.ToUpperInvariant())
            {
                case "A" when reader.TokenType == JsonTokenType.Number:
                    a = (byte)reader.GetInt32();
                    break;
                case "R" when reader.TokenType == JsonTokenType.Number:
                    r = (byte)reader.GetInt32();
                    break;
                case "G" when reader.TokenType == JsonTokenType.Number:
                    g = (byte)reader.GetInt32();
                    break;
                case "B" when reader.TokenType == JsonTokenType.Number:
                    b = (byte)reader.GetInt32();
                    break;
                default:
                    reader.Skip();
                    break;
            }
        }

        return new Color(a, r, g, b);
    }

    /// <summary>
    /// Writes a <see cref="Color"/> as a JSON string.
    /// </summary>
    public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
