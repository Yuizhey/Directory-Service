using DirectoryService.Application;
using DirectoryService.Infrastructure;

namespace DirectoryService.Presentation;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddControllers();
        builder.Services.AddScoped<DirectoryServiceDbContext>(_ =>
            new DirectoryServiceDbContext(builder.Configuration.GetConnectionString("DirectoryServiceDB")!));

        builder.Services.AddApplicationLayer();
        builder.Services.AddInfrastructureLayer();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.MapControllers();
        app.UseHttpsRedirection();
        app.Run();
    }
}