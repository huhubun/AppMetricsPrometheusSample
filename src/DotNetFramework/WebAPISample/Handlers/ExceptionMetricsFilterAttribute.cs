using App.Metrics;
using System.Net.Http;
using System.Web.Http.Filters;

namespace WebAPISample.Handlers
{
    public class ExceptionMetricsFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            var request = context.Request;
            var routeTemplate = GetRouteTemplate(request);

            ApiMetrics.GetMetrics().Measure.Counter.Increment(new App.Metrics.Counter.CounterOptions
            {
                Name = "Exception Count",
                Tags = new MetricTags(
                    new string[] { "method", "route", "exception" },
                    new string[] { request.Method.Method, routeTemplate, context.Exception.GetType().FullName }
                )
            });
        }

        private string GetRouteTemplate(HttpRequestMessage request)
        {
            return request.GetRouteData().Route.RouteTemplate ?? "unknown";
        }
    }
}