namespace GraphMeetingScheduler.Infrastructure.Serialization;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

/// <summary>
/// Defines the API-wide defaults for Json.Net serialization settings.
/// </summary>
public static class JsonConstants
{
    static JsonConstants()
    {
        SerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
            Formatting = Formatting.Indented
        };
        SerializerSettings.Converters.Add(new StringEnumWithDefaultConverter());
    }

    /// <summary>
    /// Gets the default Json.Net serialization settings.
    /// </summary>
    public static JsonSerializerSettings SerializerSettings { get; }
}