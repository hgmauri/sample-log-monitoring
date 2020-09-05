using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Primitives;
using Sample.Serilog.WebApi.Core.HealthCheck;
using Serilog;
using Serilog.Exceptions;
using Serilog.Filters;
using Serilog.Sinks.Elasticsearch;

namespace Sample.Serilog.WebApi.Core.Extensions
{
    public static class HeathCheckExtensions
    {
        public static void AddHeathCheckApi(this IServiceCollection services)
        {
            services.AddHealthChecks()
                .AddCheck("Situação", () => HealthCheckResult.Healthy())
                .AddCheck<MyHealthCheck>("Dependências");

            services.AddHealthChecksUI(config =>
            {
                config.AddHealthCheckEndpoint("SQL Server", ObterHostNameApiHealthCheck());
                config.AddHealthCheckEndpoint("Meu GitHub", $"http://github.com/hgmauri");
                config.AddHealthCheckEndpoint("Aplicação", $"http://localhost:5001/health");
            }).AddInMemoryStorage();
        }

        public static void UseHealthCheckApi(this IApplicationBuilder app)
        {
            app.UseHealthChecksUI(config =>
            {
                config.UIPath = "/hc";
            });

            app.UseHealthChecksUI();
        }

        public static string ObterHostNameApiHealthCheck()
        {
            return Environment.GetEnvironmentVariable("HostNameHealthCheck") == null ? "/api/health" : $"{Environment.GetEnvironmentVariable("HostNameHealthCheck")}/api/health";
        }
    }
}
