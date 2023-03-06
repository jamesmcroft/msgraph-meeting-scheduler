namespace GraphMeetingScheduler.Features.Scheduling.Domain;

using Infrastructure.Requests;
using Infrastructure.Responses;
using MediatR;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Users.Item.Calendar.GetSchedule;
using Models;
using Users.Domain;
using User = Users.Models.User;

public class GetUserScheduleByEmailAddressHandler
    : IRequestHandler<GetUserScheduleByEmailAddressHandler.Request, Response<UserSchedule>>
{
    private readonly GraphServiceClient graphClient;
    private readonly IMediator mediator;

    public GetUserScheduleByEmailAddressHandler(GraphServiceClient graphClient, IMediator mediator)
    {
        this.graphClient = graphClient;
        this.mediator = mediator;
    }

    public async Task<Response<UserSchedule>> Handle(Request request, CancellationToken cancellationToken)
    {
        Response<User> userResponse = await this.mediator.Send(
            new GetUserByEmailAddressHandler.Request(request.EmailAddress),
            cancellationToken);
        if (userResponse is not { Error: null })
        {
            return Response.NotFound(userResponse.Error.Value);
        }

        GetScheduleResponse? scheduleResponse =
            await this.GetUserScheduleAsync(request, userResponse.Data!, cancellationToken);

        if (scheduleResponse?.Value == null || scheduleResponse.Value.Count == 0 ||
            scheduleResponse.Value.FirstOrDefault()?.WorkingHours == null)
        {
            return Response.NotFound(new ResponseErrorMessage("UserScheduleNotFound",
                new { request.EmailAddress, request.StartDateTime, request.EndDateTime }));
        }

        DateTime startDateTime = request.StartDateTime;
        DateTime endDateTime = request.EndDateTime;
        int duration = request.Duration;
        List<ScheduleInformation>? schedule = scheduleResponse.Value;

        IEnumerable<UserScheduleItem> userScheduledMeetings =
            schedule.SelectMany(scheduleInformation => scheduleInformation.ScheduleItems!)
                .Select(UserScheduleItem.FromGraphScheduleItem);

        WorkingHours scheduleWorkingHours = schedule.FirstOrDefault()!.WorkingHours!;

        IEnumerable<DateTime> daysOfWeekInRange = Enumerable.Range(0, 1 + endDateTime.Subtract(startDateTime).Days)
            .Select(offset => startDateTime.AddDays(offset)).ToList();

        IEnumerable<UserWorkingHours> userWorkingDays = daysOfWeekInRange
            .Where(rangeDate =>
                scheduleWorkingHours.DaysOfWeek!.Any(scheduleDay => (int)scheduleDay! == (int)rangeDate.DayOfWeek))
            .Select(workingDay => UserWorkingHours.FromGraphWorkingHours(workingDay, scheduleWorkingHours)).ToList();

        // Gets all the availability time slots for the user based on their working hours, their scheduled meetings, and the duration of the meeting.
        IEnumerable<UserScheduleAvailability> userScheduleAvailability = userWorkingDays
            .SelectMany(workingDay => Enumerable
                .Range(0, (int)((workingDay.EndTimeUtc - workingDay.StartTimeUtc).TotalMinutes / duration))
                .Select(offset => UserScheduleAvailability.FromUserWorkingHours(workingDay, offset, duration)))
            .Where(availability =>
                !userScheduledMeetings.Any(meeting =>
                    availability.StartTimeUtc < meeting.EndTimeUtc && availability.EndTimeUtc > meeting.StartTimeUtc));

        return new UserSchedule(userResponse.Data!, userWorkingDays, userScheduledMeetings, userScheduleAvailability);
    }

    private async Task<GetScheduleResponse?> GetUserScheduleAsync(
        Request request,
        User user,
        CancellationToken cancellationToken = default)
    {
        var scheduleRequest = new GetSchedulePostRequestBody
        {
            StartTime = new DateTimeTimeZone { DateTime = request.StartDateTime.ToString("O"), TimeZone = "UTC" },
            EndTime = new DateTimeTimeZone { DateTime = request.EndDateTime.ToString("O"), TimeZone = "UTC" },
            Schedules = new List<string> { request.EmailAddress }
        };

        return await this.graphClient
            .Users[user.Id]
            .Calendar
            .GetSchedule
            .PostAsync(scheduleRequest, cancellationToken: cancellationToken);
    }

    public class Request : MediatorRequest<UserSchedule>
    {
        public Request(
            string emailAddress,
            DateTime startDateTime,
            DateTime endDateTime,
            int duration,
            Guid? correlationId = default) : base(correlationId)
        {
            this.EmailAddress = emailAddress;
            this.StartDateTime = startDateTime;
            this.EndDateTime = endDateTime;
            this.Duration = duration;
        }

        public string EmailAddress { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public int Duration { get; set; }
    }
}