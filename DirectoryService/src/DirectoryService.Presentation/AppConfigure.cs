using DirectoryService.Presentation.Middlewares;
using Serilog;

namespace DirectoryService.Application;

public static class AppConfigure
{
    public static IApplicationBuilder Configure(this WebApplication app)
    {
        app.UseMiddleware<ExceptionMiddleware>();
        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.MapControllers();
        app.UseHttpsRedirection();
        
        return app;
    }
}