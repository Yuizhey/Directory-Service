using System;
using DirectoryService.Application.Abstractions.Locations;
using DirectoryService.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services)
    {
        services.AddScoped<ILocationsRepository, LocationsRepository>();

        return services;
    }
}
