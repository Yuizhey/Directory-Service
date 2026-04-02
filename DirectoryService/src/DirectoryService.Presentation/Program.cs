using System.Globalization;
using DirectoryService.Application;
using DirectoryService.Infrastructure;
using Serilog;
using Serilog.Core;

namespace DirectoryService.Presentation;

public class Program
{
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
            .CreateBootstrapLogger();
        
        try
        {
            Log.Information("Starting web application");

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddControllers();
            builder.Services.AddScoped<DirectoryServiceDbContext>(_ =>
                new DirectoryServiceDbContext(builder.Configuration.GetConnectionString("DirectoryServiceDB")!));

            builder.Services.AddApplicationLayer(builder.Configuration);
            builder.Services.AddInfrastructureLayer();

            var app = builder.Build();
            app.Configure();
            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}