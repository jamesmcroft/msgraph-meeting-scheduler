namespace GraphMeetingScheduler.Features.Scheduling.Functions;

using System.Collections.Specialized;
using System.Net;
using System.Web;
using Features.Scheduling.Domain;
using Infrastructure.Functions;
using Infrastructure.Responses;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Graph.Models;
using Microsoft.OpenApi.Models;

public class GetAvailabilityFunction : BaseFunction
{
    public GetAvailabilityFunction(IMediator mediator) : base(mediator)
    {
    }

    [Function(nameof(GetAvailabilityFunction))]
    [OpenApiOperation("GetAvailability", "Scheduling")]
    [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
    [OpenApiParameter("email", In = ParameterLocation.Query, Required = true, Type = typeof(string[]), Description = "The email addresses of the users to get availability for.", Explode = true)]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "scheduling/availability")]
        HttpRequestData req)
    {
        NameValueCollection query = HttpUtility.ParseQueryString(req.Url.Query);

        var emailAddresses = query["email"]?.Split(",").ToList();

        if (emailAddresses is not { Count: > 1 })
        {
            return await ExecuteErrorResultAsync(req, HttpStatusCode.BadRequest,
                new ResponseErrorMessage("AvailabilityUserEmailRequired",
                    "At least two email addresses are required to get availability"));
        }

        var users = new List<User>();

        foreach (string? emailAddress in emailAddresses)
        {
            Response<User?> userResponse = await this.Mediator.Send(new GetUserByEmailAddressHandler.Request(emailAddress));

            (bool success, HttpResponseData? errorResponse) = await ValidateResponseAsync(req, userResponse);
            if (!success)
            {
                return errorResponse!;
            }

            users.Add(userResponse.Data!);
        }

        return await ExecuteResultAsync(req, HttpStatusCode.OK, users);
    }
}