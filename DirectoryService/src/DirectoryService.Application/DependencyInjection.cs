using System;
using DirectoryService.Application.Abstractions.Locations;
using DirectoryService.Application.Implementations.Locations;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        services.AddScoped<ILocationsService, LocationService>();

        return services;
    }
}
