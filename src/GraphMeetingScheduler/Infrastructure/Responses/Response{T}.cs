namespace GraphMeetingScheduler.Infrastructure.Responses;

using System.Net;

public class Response<T> : Response, IResponseWithData
{
    private Response(HttpStatusCode status, T? data, ResponseErrorMessage? error = default) : base(status, error)
    {
        this.Data = data;
    }

    public T? Data { get; }

    object? IResponseWithData.Data => this.Data;

    public static implicit operator Response<T>(T result)
    {
        return From(HttpStatusCode.OK, result);
    }

    public static implicit operator Response<T>(ErrorResponse response)
    {
        return new Response<T>(response.Status, default, response.Error);
    }

    public static Response<T> From(HttpStatusCode status, T data)
    {
        return new Response<T>(status, data);
    }
}