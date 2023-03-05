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
    [OpenApiParameter("email", In = ParameterLocation.Query, Required = true, Type = typeof(string[]), Description = "The email addresses of the users to get availability for.", Explode = true)]
    [OpenApiParameter("start", In = ParameterLocation.Query, Required = true, Type = typeof(DateTime), Description = "The start date and time of the availability window in ISO 8601 format.")]
    [OpenApiParameter("end", In = ParameterLocation.Query, Required = true, Type = typeof(DateTime), Description = "The end date and time of the availability window in ISO 8601 format.")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "scheduling/availability")]
        HttpRequestData req)
    {
        NameValueCollection query = HttpUtility.ParseQueryString(req.Url.Query);

        var emailAddresses = query["email"]?.Split(",").ToList();
        bool hasScheduleStart = DateTime.TryParse(query["start"], out DateTime scheduleStart);
        bool hasScheduleEnd = DateTime.TryParse(query["end"], out DateTime scheduleEnd);
        
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

        var calendars = new List<UserSchedule>();

        foreach (string? emailAddress in emailAddresses)
        {
            Response<UserSchedule?> scheduleResponse = await this.Mediator.Send(new GetUserScheduleByEmailAddressHandler.Request(emailAddress, scheduleStart, scheduleEnd));

            (bool success, HttpResponseData? errorResponse) = await ValidateResponseAsync(req, scheduleResponse);
            if (!success)
            {
                return errorResponse!;
            }

            calendars.Add(scheduleResponse.Data!);
        }

        return await ExecuteResultAsync(req, HttpStatusCode.OK, calendars);
    }
}