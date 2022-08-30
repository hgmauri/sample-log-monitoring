using System;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Sample.Serilog.WebApi.Core.Extensions;
using Serilog;

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.AddSerilog(builder.Configuration, "API Exemplo");
    Log.Information("Getting the motors running...");

    builder.Services.AddApiConfiguration();
    builder.Services.AddSwaggerApi(builder.Configuration);
    builder.Services.AddHeathCheckApi(builder.Configuration);

    var app = builder.Build();

    app.UseApiConfiguration(app.Environment);

    app.UseSwaggerDocApi();
    app.UseHealthCheckApi();
    
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
        endpoints.MapHealthChecks("/hc",
            new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
            });
    });

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.Information("Server Shutting down...");
    Log.CloseAndFlush();
}
