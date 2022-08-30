using System;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Filters;
using Serilog.Sinks.Elasticsearch;

namespace Sample.Serilog.WebApi.Core.Extensions;

public static class SerilogExtensions
{
    public static WebApplicationBuilder AddSerilog(this WebApplicationBuilder builder, IConfiguration configuration, string applicationName)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.WithProperty("ApplicationName", $"{applicationName} - {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}")
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentUserName()
            .Enrich.WithDemystifiedStackTraces()
            .Filter.ByExcluding(Matching.FromSource("Microsoft.AspNetCore.StaticFiles"))
            .Filter.ByExcluding(z => z.MessageTemplate.Text.Contains("erro de negócio"))
            .WriteTo.Async(writeTo => writeTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(configuration["ElasticsearchSettings:uri"]))
            {
                AutoRegisterTemplate = true,
                IndexFormat = "logs",
                ModifyConnectionSettings = x => x.BasicAuthentication(configuration["ElasticsearchSettings:username"],
                    configuration["ElasticsearchSettings:password"])
            }))
            .WriteTo.Async(writeTo => writeTo.Seq(configuration["Seq:uri"]))
            .WriteTo.Async(writeTo => writeTo.ApplicationInsights(TelemetryConfiguration.Active, TelemetryConverter.Traces))
            .WriteTo.Async(writeTo => writeTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"))
            .CreateLogger();

        builder.Logging.ClearProviders();
        builder.Host.UseSerilog(Log.Logger, true);

        return builder;
    }
}