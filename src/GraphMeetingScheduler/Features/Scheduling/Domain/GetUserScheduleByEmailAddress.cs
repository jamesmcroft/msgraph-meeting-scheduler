namespace GraphMeetingScheduler.Features.Scheduling.Domain;

using Infrastructure.Requests;
using Infrastructure.Responses;
using MediatR;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Users.Item.Calendar.GetSchedule;
using Models;
using Users.Domain;

public class GetUserScheduleByEmailAddressHandler : IRequestHandler<GetUserScheduleByEmailAddressHandler.Request,
    Response<UserSchedule?>>
{
    private readonly GraphServiceClient graphClient;
    private readonly IMediator mediator;

    public GetUserScheduleByEmailAddressHandler(GraphServiceClient graphClient, IMediator mediator)
    {
        this.graphClient = graphClient;
        this.mediator = mediator;
    }

    public async Task<Response<UserSchedule?>> Handle(Request request, CancellationToken cancellationToken)
    {
        Response<User?> user = await this.mediator.Send(new GetUserByEmailAddressHandler.Request(request.EmailAddress),
            cancellationToken);
        if (user is not { Error: null })
        {
            return Response.NotFound(user.Error.Value);
        }

        var scheduleRequest = new GetSchedulePostRequestBody
        {
            StartTime = new DateTimeTimeZone { DateTime = request.StartDateTime.ToString("O"), TimeZone = "UTC" },
            EndTime = new DateTimeTimeZone { DateTime = request.EndDateTime.ToString("O"), TimeZone = "UTC" },
            Schedules = new List<string> { request.EmailAddress }
        };

        GetScheduleResponse? scheduleResponse = await this.graphClient
            .Users[user.Data!.Id]
            .Calendar
            .GetSchedule
            .PostAsync(scheduleRequest, cancellationToken: cancellationToken);

        if (scheduleResponse?.Value == null || scheduleResponse.Value.Count == 0)
        {
            return Response.NotFound(new ResponseErrorMessage("UserScheduleNotFound",
                new { request.EmailAddress, request.StartDateTime, request.EndDateTime }));
        }

        return new UserSchedule(user.Data, scheduleResponse.Value);
    }

    public class Request : MediatorRequest<UserSchedule?>
    {
        public Request(
            string emailAddress,
            DateTime startDateTime,
            DateTime endDateTime,
            Guid? correlationId = default) : base(correlationId)
        {
            this.EmailAddress = emailAddress;
            this.StartDateTime = startDateTime;
            this.EndDateTime = endDateTime;
        }

        public string EmailAddress { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }
    }
}