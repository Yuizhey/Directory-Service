using DirectoryService.Application.Abstractions.DatabaseHelpers;
using DirectoryService.Application.Abstractions.Departments;
using DirectoryService.Application.Abstractions.Locations;
using DirectoryService.Application.Abstractions.Positions;
using DirectoryService.Infrastructure.DatabaseHelpers;
using DirectoryService.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services)
    {
        services.AddScoped<ILocationsRepository, LocationsRepository>();
        services.AddScoped<IDepartmentsRepository, DepartmentsRepository>();
        services.AddScoped<IPositionsRepository, PositionsRepository>();
        services.AddScoped<ITransactionManager, TransactionManager>();

        return services;
    }
}
