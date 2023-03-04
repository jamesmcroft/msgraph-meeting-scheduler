using GraphMeetingScheduler;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]

namespace GraphMeetingScheduler;

using Azure.Identity;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddHttpClient();
        builder.Services.AddMemoryCache();

        AddGraphServiceClient(builder.Services);
    }

    private static void AddGraphServiceClient(IServiceCollection services)
    {
        string? clientId = Environment.GetEnvironmentVariable("AzureAd__ClientId");
        string? clientSecret = Environment.GetEnvironmentVariable("AzureAd__ClientSecret");
        string? tenantId = Environment.GetEnvironmentVariable("AzureAd__TenantId");

        var clientSecretCredential = new ClientSecretCredential(tenantId, clientId, clientSecret);

        services.AddSingleton(_ => new GraphServiceClient(clientSecretCredential));
    }
}