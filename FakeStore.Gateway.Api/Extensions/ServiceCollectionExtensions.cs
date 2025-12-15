using FakeStore.Gateway.Application.Extensions;
using FakeStore.Gateway.Infrastructure.Extensions;

namespace FakeStore.Gateway.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFakeStoreGateway(this IServiceCollection services, IConfiguration configuration)
    {
        // Registrar servicios de la capa de Aplicación
        services.AddApplicationServices();
        
        // Registrar servicios de la capa de Infraestructura
        services.AddInfrastructureServices(configuration);

        return services;
    }
}
