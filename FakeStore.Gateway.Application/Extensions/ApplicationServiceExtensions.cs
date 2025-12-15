using FakeStore.Gateway.Application.DTOs;
using FakeStore.Gateway.Application.Interfaces;
using FakeStore.Gateway.Application.Services;
using FakeStore.Gateway.Application.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace FakeStore.Gateway.Application.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Registrar servicios de aplicación
        services.AddScoped<IProductsService, ProductsService>();
        
        // Registrar caché como Singleton para mantener datos en memoria durante toda la sesión
        services.AddSingleton<IProductCacheService, ProductCacheService>();
        
        // Registrar validadores de FluentValidation
        services.AddScoped<IValidator<CreateProductDto>, CreateProductDtoValidator>();
        services.AddScoped<IValidator<UpdateProductDto>, UpdateProductDtoValidator>();
        
        return services;
    }
}
