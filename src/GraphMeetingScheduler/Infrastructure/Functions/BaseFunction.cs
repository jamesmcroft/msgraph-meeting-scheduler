namespace GraphMeetingScheduler.Infrastructure.Functions;

using System.Net;
using MediatR;
using Microsoft.Azure.Functions.Worker.Http;
using Responses;
using Serialization;

public abstract class BaseFunction
{
    protected BaseFunction(IMediator mediator)
    {
        this.Mediator = mediator;
    }

    protected IMediator Mediator { get; }

    protected static async Task<(bool success, HttpResponseData? errorResponse)> ValidateResponseAsync<T>(
        HttpRequestData req,
        T response)
        where T : Response
    {
        return response.Error is not { } error
            ? (true, default)
            : (false, await ExecuteErrorResultAsync(req, response.Status, error));
    }

    protected static Task<HttpResponseData> ExecuteSuccessResultAsync(HttpRequestData req, Response response)
    {
        if (response is IResponseWithData responseWithData)
        {
            return ExecuteResultAsync(req, response.Status, responseWithData.Data);
        }

        return ExecuteResultAsync(req, response.Status, null);
    }

    protected static Task<HttpResponseData> ExecuteErrorResultAsync(HttpRequestData req, HttpStatusCode status,
        ResponseErrorMessage error)
    {
        var body = new Dictionary<string, object> { { "code", error.Code } };

        if (error.Data is { } data)
        {
            body.Add("data", data);
        }

        return ExecuteResultAsync(req, status, body);
    }

    protected static async Task<HttpResponseData> ExecuteResultAsync(
        HttpRequestData req,
        HttpStatusCode status,
        object? data)
    {
        HttpResponseData response = req.CreateResponse();
        await response.WriteJsonAsync(status, data, JsonConstants.SerializerSettings);
        return response;
    }
}