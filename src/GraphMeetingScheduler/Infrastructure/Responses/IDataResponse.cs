namespace GraphMeetingScheduler.Infrastructure.Responses;

public interface IResponseWithData
{
    object? Data { get; }
}