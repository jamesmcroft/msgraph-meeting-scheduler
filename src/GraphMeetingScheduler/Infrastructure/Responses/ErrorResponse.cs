namespace GraphMeetingScheduler.Infrastructure.Responses;

using System.Net;

public sealed class ErrorResponse : Response
{
    public ErrorResponse(HttpStatusCode status, ResponseErrorMessage? error = default) : base(status, error)
    {
    }
}