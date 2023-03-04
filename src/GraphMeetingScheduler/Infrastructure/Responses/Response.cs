namespace GraphMeetingScheduler.Infrastructure.Responses;

using System.Net;

/// <summary>
/// Defines a response for a request handler.
/// </summary>
public class Response
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Response"/> class with a status code and optional error message.
    /// </summary>
    /// <param name="status">The status code for the response.</param>
    /// <param name="error">The error message for the response if applicable.</param>
    protected Response(HttpStatusCode status, ResponseErrorMessage? error = default)
    {
        this.Status = status;
        this.Error = error;
    }

    /// <summary>
    /// Gets the status code for the response.
    /// </summary>
    public HttpStatusCode Status { get; }

    /// <summary>
    /// Gets the error message for the response if applicable; otherwise, null.
    /// </summary>
    public ResponseErrorMessage? Error { get; }

    /// <summary>
    /// Creates a new success response with an expected success status code.
    /// </summary>
    /// <param name="status">The status code for the response.</param>
    public static Response Success(HttpStatusCode status)
    {
        if (status is < HttpStatusCode.OK or >= HttpStatusCode.MultipleChoices)
        {
            throw new ArgumentException("Status code must be a success status code", nameof(status));
        }

        return new Response(status);
    }

    /// <summary>
    /// Creates an OK response.
    /// </summary>
    /// <returns>A <see cref="Response"/> with a 200 OK status code.</returns>
    public static Response OK()
    {
        return Success(HttpStatusCode.OK);
    }

    /// <summary>
    /// Creates an OK response with data.
    /// </summary>
    /// <typeparam name="T">The type of data response.</typeparam>
    /// <param name="data">The data returned from the request.</param>
    /// <returns>A <see cref="Response{T}"/> with a 200 OK status code and the specified data.</returns>
    public static Response<T> OK<T>(T data)
    {
        return Response<T>.From(HttpStatusCode.OK, data);
    }

    /// <summary>
    /// Creates a Created response.
    /// </summary>
    /// <returns>A <see cref="Response"/> with a 201 Created status code.</returns>
    public static Response Created()
    {
        return Success(HttpStatusCode.Created);
    }

    /// <summary>
    /// Creates a Created response with data.
    /// </summary>
    /// <typeparam name="T">The type of data response.</typeparam>
    /// <param name="data">The data returned from the request.</param>
    /// <returns>A <see cref="Response{T}"/> with a 201 Created status code and the specified data.</returns>
    public static Response<T> Created<T>(T data)
    {
        return Response<T>.From(HttpStatusCode.Created, data);
    }

    /// <summary>
    /// Creates an Accepted response.
    /// </summary>
    /// <returns>A <see cref="Response"/> with a 202 Accepted status code.</returns>
    public static Response Accepted()
    {
        return Success(HttpStatusCode.Accepted);
    }

    /// <summary>
    /// Creates an Accepted response with data.
    /// </summary>
    /// <typeparam name="T">The type of data response.</typeparam>
    /// <param name="data">The data returned from the request.</param>
    /// <returns>A <see cref="Response{T}"/> with a 202 Accepted status code and the specified data.</returns>
    public static Response<T> Accepted<T>(T data)
    {
        return Response<T>.From(HttpStatusCode.Accepted, data);
    }

    /// <summary>
    /// Creates a Bad Request response with an error message.
    /// </summary>
    /// <param name="message">The detail of the error.</param>
    /// <returns>A <see cref="Response"/> with a 400 Bad Request status code and the specified error message.</returns>
    public static ErrorResponse BadRequest(ResponseErrorMessage message)
    {
        return new ErrorResponse(HttpStatusCode.BadRequest, message);
    }

    /// <summary>
    /// Creates a Not Found response with an error message.
    /// </summary>
    /// <param name="message">The detail of the error.</param>
    /// <returns>A <see cref="Response"/> with a 404 Not Found status code and the specified error message.</returns>
    public static ErrorResponse NotFound(ResponseErrorMessage message)
    {
        return new ErrorResponse(HttpStatusCode.NotFound, message);
    }

    /// <summary>
    /// Creates a Conflict response with an error message.
    /// </summary>
    /// <param name="message">The detail of the error.</param>
    /// <returns>A <see cref="Response"/> with a 409 Conflict status code and the specified error message.</returns>
    public static ErrorResponse Conflict(ResponseErrorMessage message)
    {
        return new ErrorResponse(HttpStatusCode.Conflict, message);
    }
}