using Microsoft.AspNetCore.Http;
using Sample.Serilog.WebApi.Core.Extensions;
using Serilog.Context;
using System.Threading.Tasks;

namespace Sample.Serilog.WebApi.Core.Middleware
{
    public class RequestSerilLogContextMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestSerilLogContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext context)
        {
            using (LogContext.PushProperty("UserName", context?.User?.Identity?.Name ?? "anônimo"))
            using (LogContext.PushProperty("CorrelationId", context.GetCorrelationId()))
            {
                return _next.Invoke(context);
            }
        }
    }
}
