using System.Diagnostics;

namespace UniDesc.Web.Filters
{
    public class RequestTimingFilter : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(
            EndpointFilterInvocationContext context,
            EndpointFilterDelegate next)
        {
            var request = context.HttpContext.Request;
            var stopwatch = Stopwatch.StartNew();

            Console.WriteLine($"[Tickets API] START {request.Method} {request.Path}");

            try
            {
                return await next(context);
            }
            finally
            {
                stopwatch.Stop();
                Console.WriteLine($"[Tickets API] END {request.Method} {request.Path} - {stopwatch.ElapsedMilliseconds} ms");
            }
        }
    }
}