using System;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sample.Serilog.WebApi.Core.Extensions;
using Sample.Serilog.WebApi.Core.HealthCheck;
using Sample.Serilog.WebApi.Core.Middleware;
using Serilog;

namespace Sample.Serilog.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddSwaggerApi(Configuration);

            services.AddHeathCheckApi();

            services.AddControllers();

            services.AddScoped<IMyCustomService, MyCustomService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSerilogRequestLogging(opts => opts.EnrichDiagnosticContext = LogEnricherExtensions.EnrichFromRequest);

            app.UseMiddleware<RequestSerilLogContextMiddleware>();

            app.UseSwaggerDocApi();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseHealthCheckApi();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health",
                    new HealthCheckOptions
                    {
                        Predicate = _ => true,
                        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                    });
            });
        }
    }
}
