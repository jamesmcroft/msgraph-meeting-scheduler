namespace GraphMeetingScheduler.Infrastructure.Serialization;

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

/// <summary>
/// Defines a Json.Net converter to convert an <see cref="Enum"/> to and from its name string value.
/// </summary>
public class StringEnumWithDefaultConverter : StringEnumConverter
{
    /// <summary>Reads the JSON representation of the object.</summary>
    /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
    /// <param name="objectType">Type of the object.</param>
    /// <param name="existingValue">The existing value of object being read.</param>
    /// <param name="serializer">The calling serializer.</param>
    /// <returns>The object value.</returns>
    /// <footer><a href="https://www.google.com/search?q=Newtonsoft.Json.Converters.StringEnumConverter.ReadJson">`StringEnumConverter.ReadJson` on google.com</a></footer>
    public override object? ReadJson(JsonReader reader,
        Type objectType,
        object? existingValue,
        JsonSerializer serializer)
    {
        try
        {
            return base.ReadJson(reader, objectType, existingValue, serializer);
        }
        catch (JsonSerializationException)
        {
            return existingValue;
        }
    }
}