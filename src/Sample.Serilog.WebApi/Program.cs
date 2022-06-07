using System;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sample.Serilog.WebApi.Core.Extensions;
using Sample.Serilog.WebApi.Core.HealthCheck;
using Sample.Serilog.WebApi.Core.Middleware;
using Serilog;

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.AddSerilog(builder.Configuration, "API Monitoring");
    Log.Information("Getting the motors running...");

    builder.Services.AddRouting(options => options.LowercaseUrls = true);
    builder.Services.AddSwaggerApi(builder.Configuration);
    builder.Services.AddHeathCheckApi(builder.Configuration);
    builder.Services.AddControllers();
    builder.Services.AddScoped<IMyCustomService, MyCustomService>();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    app.UseSerilogRequestLogging(opts => opts.EnrichDiagnosticContext = LogEnricherExtensions.EnrichFromRequest);

    app.UseMiddleware<RequestSerilLogMiddleware>();
    app.UseMiddleware<ErrorHandlingMiddleware>();

    app.UseSwaggerDocApi();
    app.UseHttpsRedirection();
    app.UseRouting();
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
