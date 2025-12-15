using FakeStore.Gateway.Infrastructure.Clients;
using FakeStore.Gateway.Infrastructure.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;

namespace FakeStore.Gateway.Infrastructure.Extensions;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configurar settings de FakeStore API
        services.Configure<FakeStoreApiSettings>(configuration.GetSection(FakeStoreApiSettings.SectionName));

        // Registrar HttpClient tipado con políticas de resiliencia
        services.AddHttpClient<IFakeStoreApiClient, FakeStoreApiClient>((serviceProvider, client) =>
        {
            var settings = serviceProvider.GetRequiredService<IOptions<FakeStoreApiSettings>>().Value;
            
            client.BaseAddress = new Uri(settings.BaseUrl);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.Timeout = TimeSpan.FromSeconds(settings.TimeoutSeconds);
        })
        .AddPolicyHandler((serviceProvider, _) => GetRetryPolicy(serviceProvider))
        .AddPolicyHandler((serviceProvider, _) => GetCircuitBreakerPolicy(serviceProvider));

        return services;
    }

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(IServiceProvider serviceProvider)
    {
        var settings = serviceProvider.GetRequiredService<IOptions<FakeStoreApiSettings>>().Value;

        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(
                retryCount: settings.RetryCount,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }

    private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(IServiceProvider serviceProvider)
    {
        var settings = serviceProvider.GetRequiredService<IOptions<FakeStoreApiSettings>>().Value;

        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: settings.CircuitBreakerThreshold,
                durationOfBreak: TimeSpan.FromSeconds(settings.CircuitBreakerDurationSeconds));
    }
}
