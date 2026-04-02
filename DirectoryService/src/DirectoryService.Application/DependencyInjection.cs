using System;
using System.Windows.Input;
using DirectoryService.Application.Abstractions.Command;
using DirectoryService.Application.Abstractions.Locations;
using DirectoryService.Application.Implementations.Locations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Exceptions;

namespace DirectoryService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSerilogLogger(configuration);

        var assembly = typeof(DependencyInjection).Assembly;
        services.Scan(scan => scan.FromAssemblies(assembly)
            .AddClasses(c => c.AssignableToAny(typeof(ICommandHandler<,>), typeof(ICommandHandler<>)))
            .AsSelfWithInterfaces()
            .WithScopedLifetime());
        return services;
    }

    private static IServiceCollection AddSerilogLogger(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSerilog((sp, lc) => lc
            .ReadFrom.Configuration(configuration)
            .ReadFrom.Services(sp)
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .Enrich.WithProperty("Service Name", "Directory Service"));

        return services;
    }
}
