namespace GraphMeetingScheduler.Infrastructure.Responses;

using System.Net;
using System.Text;
using System.Threading.Tasks;
using GraphMeetingScheduler.Infrastructure.Serialization;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;

/// <summary>
/// Defines a collection of extensions for a <see cref="HttpResponseData" /> object.
/// </summary>
public static class HttpResponseDataExtensions
{
    /// <summary>
    /// Writes an object value as JSON to the specified <paramref name="response" />.
    /// </summary>
    /// <param name="response">The HTTP response to write to.</param>
    /// <param name="statusCode">The status code of the response.</param>
    /// <param name="value">The object to serialize as JSON.</param>
    /// <param name="serializerSettings">The JSON serializer settings.</param>
    /// <returns>An asynchronous operation.</returns>
    public static async Task WriteJsonAsync(
        this HttpResponseData response,
        HttpStatusCode statusCode,
        object? value,
        JsonSerializerSettings? serializerSettings = null)
    {
        response.Headers.Add(HeaderNames.ContentType, new MediaTypeHeaderValue("application/json") { Encoding = Encoding.UTF8 }.ToString());
        response.StatusCode = statusCode;

        if (value is null)
        {
            return;
        }

        string json = JsonConvert.SerializeObject(value, Formatting.Indented,
            serializerSettings ?? JsonConstants.SerializerSettings);

        await response.WriteStringAsync(json, Encoding.UTF8);
    }
}