using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Sample.Serilog.WebApi.Core.HealthCheck;

namespace Sample.Serilog.WebApi.Core.Extensions
{
    public static class HeathCheckExtensions
    {
        public static void AddHeathCheckApi(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHealthChecks()
                .AddCheck("Situação", () => HealthCheckResult.Unhealthy())
                .AddCheck<MyHealthCheck>("Dependências")
                .AddSqlServer(
                    configuration.GetConnectionString("DefaultConnection"), "SELECT 1;", "Sql Server", HealthStatus.Degraded, timeout: TimeSpan.FromSeconds(30), tags: new[] { "db", "sql", "sqlServer", })
                .AddRedis(
                    configuration.GetConnectionString("RedisConnection"), "Redis", HealthStatus.Degraded, new[] { "redis", "cache" })
                //.AddRabbitMQ(
                //    configuration.GetConnectionString("RabbitMQ"), null, "RabbitMQ", HealthStatus.Degraded, new[] { "rabbitmq", "queue", "message", "broker" })
                .AddElasticsearch(
                    configuration.GetConnectionString("Elasticsearch"), "ElasticSearch", HealthStatus.Degraded, new[] { "elastic", "search" });

            services.AddHealthChecksUI(config =>
            {
                config.AddHealthCheckEndpoint("Host Externo", ObterHostNameApiHealthCheck());
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
