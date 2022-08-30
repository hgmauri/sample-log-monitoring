using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sample.Serilog.WebApi.Core.HealthCheck;
using Sample.Serilog.WebApi.Core.Middleware;
using Serilog;

namespace Sample.Serilog.WebApi.Core.Extensions;
public static class ApiConfigurationExtensions
{
    public static void AddApiConfiguration(this IServiceCollection services)
    {
        services.AddRouting(options => options.LowercaseUrls = true);

        services.AddScoped<IMyCustomService, MyCustomService>();

        services.AddHttpClient();
        services.AddControllers();
    }

    public static void UseApiConfiguration(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseSerilogRequestLogging(opts => opts.EnrichDiagnosticContext = LogEnricherExtensions.EnrichFromRequest);

        app.UseMiddleware<RequestSerilLogMiddleware>();
        app.UseMiddleware<ErrorHandlingMiddleware>();

        app.UseHttpsRedirection();
        app.UseRouting();
    }
}