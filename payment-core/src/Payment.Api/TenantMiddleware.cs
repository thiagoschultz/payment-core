using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace payment_core.src.Payment.Api
{
    public class TenantMiddleware
    {

        private readonly RequestDelegate _next;

        public TenantMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {

            var tenant = context.Request.Headers["tenant-id"];

            context.Items["tenant"] = tenant;

            await _next(context);

        }

    }
}