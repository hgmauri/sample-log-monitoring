using System;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Filters;
using Serilog.Sinks.Elasticsearch;

namespace Sample.Serilog.WebApi.Core.Extensions;

public static class SerilogExtensions
{
    public static IHostBuilder AddSerilog(this IHostBuilder builder, IConfiguration configuration, string applicationName)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.WithProperty("ApplicationName", $"API Exemplo - {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}")
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentUserName()
            .Enrich.WithDemystifiedStackTraces()
            .Filter.ByExcluding(Matching.FromSource("Microsoft.AspNetCore.StaticFiles"))
            .Filter.ByExcluding(z => z.MessageTemplate.Text.Contains("erro de negócio"))
            .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(configuration["ElasticsearchSettings:uri"]))
            {
                AutoRegisterTemplate = true,
                IndexFormat = "logs",
                ModifyConnectionSettings = x => x.BasicAuthentication(configuration["ElasticsearchSettings:username"], 
                    configuration["ElasticsearchSettings:password"])
            })
            .WriteTo.Seq(configuration["Seq:uri"])
            .WriteTo.ApplicationInsights(TelemetryConfiguration.Active, TelemetryConverter.Traces)
            .WriteTo.Console()
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
            .CreateLogger();

        builder.UseSerilog();
        return builder;
    }
}