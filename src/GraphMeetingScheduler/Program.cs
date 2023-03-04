using Azure.Identity;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Graph;

void AddGraphServiceClient(IConfiguration configurationRoot, IServiceCollection serviceCollection)
{
    IConfigurationSection azureSettings = configurationRoot.GetSection("Values").GetSection("AzureAd");

    string? clientId = azureSettings["ClientId"] ?? Environment.GetEnvironmentVariable("AZUREAD_CLIENT_ID");
    string? clientSecret = azureSettings["ClientSecret"] ?? Environment.GetEnvironmentVariable("AZUREAD_CLIENT_SECRET");
    string? tenantId = azureSettings["TenantId"] ?? Environment.GetEnvironmentVariable("AZUREAD_TENANT_ID");

    var clientSecretCredential = new ClientSecretCredential(tenantId, clientId, clientSecret);

    serviceCollection.AddSingleton(_ => new GraphServiceClient(clientSecretCredential));
}

IConfigurationRoot configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

IHost host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults(worker => worker.UseNewtonsoftJson())
    .ConfigureOpenApi()
    .ConfigureServices(services =>
    {
        services.AddHttpClient();
        services.AddMemoryCache();

        AddGraphServiceClient(configuration, services);

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());
    })
    .Build();

await host.RunAsync();