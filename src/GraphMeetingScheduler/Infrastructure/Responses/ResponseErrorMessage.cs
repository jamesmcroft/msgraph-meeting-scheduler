namespace GraphMeetingScheduler.Infrastructure.Responses;

using System.Text.Json;

/// <summary>
/// Defines an error message for a response.
/// </summary>
public readonly record struct ResponseErrorMessage(string Code, object? Data)
{
    /// <summary>
    /// Creates a new error message from a code.
    /// </summary>
    public static implicit operator ResponseErrorMessage(string code)
    {
        return new ResponseErrorMessage(code, null);
    }

    /// <summary>Returns the detail of the response error message as serialized JSON.</summary>
    /// <returns>The fully qualified type name.</returns>
    public override string ToString()
    {
        try
        {
            return JsonSerializer.Serialize(new { code = this.Code, data = this.Data });
        }
        catch
        {
            return this.Code;
        }
    }
}