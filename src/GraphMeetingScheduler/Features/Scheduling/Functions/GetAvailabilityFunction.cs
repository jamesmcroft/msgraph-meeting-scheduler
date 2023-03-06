namespace GraphMeetingScheduler.Features.Scheduling.Functions;

using System.Collections.Specialized;
using System.Net;
using System.Web;
using GraphMeetingScheduler.Features.Scheduling.Domain;
using Infrastructure.Functions;
using Infrastructure.Responses;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;
using Models;

public class GetAvailabilityFunction : BaseFunction
{
    public GetAvailabilityFunction(IMediator mediator) : base(mediator)
    {
    }

    [Function(nameof(GetAvailabilityFunction))]
    [OpenApiOperation("GetAvailability", "Scheduling")]
    [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
    [OpenApiParameter("email", In = ParameterLocation.Query, Required = true, Type = typeof(string[]),
        Description = "The email addresses of the users to get availability for.", Explode = true)]
    [OpenApiParameter("start", In = ParameterLocation.Query, Required = true, Type = typeof(DateTime),
        Description = "The UTC start date and time of the availability window in ISO 8601 format.")]
    [OpenApiParameter("end", In = ParameterLocation.Query, Required = true, Type = typeof(DateTime),
        Description = "The UTC end date and time of the availability window in ISO 8601 format.")]
    [OpenApiParameter("duration", In = ParameterLocation.Query, Required = false, Type = typeof(int),
        Description = "The duration of the meeting in minutes.")]
    [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(List<UserScheduleAvailability>),
        Description = "The availability of all users.")]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, "application/json", typeof(ResponseErrorMessage),
        Description = "Thrown when the request query does not contain the expected values.")]
    [OpenApiResponseWithBody(HttpStatusCode.NotFound, "application/json", typeof(ResponseErrorMessage),
        Description = "Thrown when a user or their schedule could not be found.")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "scheduling/availability")]
        HttpRequestData req)
    {
        NameValueCollection query = HttpUtility.ParseQueryString(req.Url.Query);

        var emailAddresses = query["email"]?.Split(",").ToList();
        bool hasScheduleStart = DateTime.TryParse(query["start"], out DateTime scheduleStart);
        bool hasScheduleEnd = DateTime.TryParse(query["end"], out DateTime scheduleEnd);
        bool hasDuration = int.TryParse(query["duration"], out int duration);

        if (emailAddresses is not { Count: > 1 })
        {
            return await ExecuteErrorResultAsync(req, HttpStatusCode.BadRequest,
                new ResponseErrorMessage("AvailabilityUserEmailRequired",
                    "At least two email addresses are required to get availability"));
        }

        if (!hasScheduleStart)
        {
            return await ExecuteErrorResultAsync(req, HttpStatusCode.BadRequest,
                new ResponseErrorMessage("AvailabilityStartRequired",
                    "The start date and time of the availability window is required"));
        }

        if (!hasScheduleEnd)
        {
            return await ExecuteErrorResultAsync(req, HttpStatusCode.BadRequest,
                new ResponseErrorMessage("AvailabilityEndRequired",
                    "The end date and time of the availability window is required"));
        }

        if (!hasDuration)
        {
            duration = 60;
        }

        var userSchedules = new List<UserSchedule>();

        foreach (string? emailAddress in emailAddresses)
        {
            Response<UserSchedule> scheduleResponse =
                await this.Mediator.Send(
                    new GetUserScheduleByEmailAddressHandler.Request(emailAddress, scheduleStart, scheduleEnd,
                        duration));

            (bool success, HttpResponseData? errorResponse) = await ValidateResponseAsync(req, scheduleResponse);
            if (!success)
            {
                return errorResponse!;
            }

            userSchedules.Add(scheduleResponse.Data!);
        }

        // Compares all users schedules in UTC time to find matching availability slots.
        IEnumerable<UserScheduleAvailability> allParticipantAvailability = userSchedules
            .SelectMany(schedule => schedule.Availability)
            .GroupBy(availability => new { availability.StartTimeUtc, availability.EndTimeUtc })
            .Where(group => group.Count() == emailAddresses.Count)
            .Select(availabilityMatchGroup => new UserScheduleAvailability
            {
                StartTimeUtc = availabilityMatchGroup.Key.StartTimeUtc,
                EndTimeUtc = availabilityMatchGroup.Key.EndTimeUtc
            });

        return await ExecuteResultAsync(req, HttpStatusCode.OK, allParticipantAvailability);
    }
}