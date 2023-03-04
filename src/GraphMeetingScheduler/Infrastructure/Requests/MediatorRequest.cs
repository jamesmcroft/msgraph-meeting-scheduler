namespace GraphMeetingScheduler.Infrastructure.Requests;

using MediatR;
using Responses;

public class MediatorRequest : IRequest<Response>
{
    public MediatorRequest(Guid? correlationId = default)
    {
        this.CorrelationId = correlationId ?? Guid.NewGuid();
    }

    public Guid CorrelationId { get; }
}