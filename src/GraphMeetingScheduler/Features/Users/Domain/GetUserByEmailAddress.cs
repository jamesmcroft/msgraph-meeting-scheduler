namespace GraphMeetingScheduler.Features.Users.Domain;

using GraphMeetingScheduler.Infrastructure.Requests;
using GraphMeetingScheduler.Infrastructure.Responses;
using MediatR;
using Microsoft.Graph;
using Microsoft.Graph.Models;

public class GetUserByEmailAddressHandler : IRequestHandler<GetUserByEmailAddressHandler.Request, Response<User?>>
{
    private readonly GraphServiceClient graphClient;

    public GetUserByEmailAddressHandler(GraphServiceClient graphClient)
    {
        this.graphClient = graphClient;
    }

    public async Task<Response<User?>> Handle(Request request, CancellationToken cancellationToken)
    {
        UserCollectionResponse? usersResponse = await this.graphClient.Users
            .GetAsync(configuration =>
                {
                    configuration.QueryParameters.Filter = $"mail eq '{request.EmailAddress}'";
                },
                cancellationToken);

        if (usersResponse?.Value == null || usersResponse.Value.Count == 0)
        {
            return Response.NotFound(new ResponseErrorMessage("UserNotFound", new { request.EmailAddress }));
        }

        return usersResponse.Value.FirstOrDefault();
    }

    public class Request : MediatorRequest<User?>
    {
        public Request(string emailAddress, Guid? correlationId = default) : base(correlationId)
        {
            this.EmailAddress = emailAddress;
        }

        public string EmailAddress { get; set; }
    }
}