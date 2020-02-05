using App.Metrics;
using App.Metrics.Timer;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WebAPISample.Handlers
{
    public class MetricsHandler : DelegatingHandler
    {
        private const string API_METRICS_RESPONSE_TIME_KEY = "__ApiMetrics.ResponseTime__";
        private const string API_METRICS_ROUTE = "metrics";

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var routeTemplate = GetRouteTemplate(request);

            // 如果访问的是 /metrics ，则不计入统计中
            if (routeTemplate == API_METRICS_ROUTE)
            {
                return await base.SendAsync(request, cancellationToken);
            }

            StartRecordingResponseTime(request);

            var response = await base.SendAsync(request, cancellationToken);

            EndRecordingResponseTime(routeTemplate, request, response);

            return response;
        }

        private string GetRouteTemplate(HttpRequestMessage request)
        {
            // MS_SubRoutes 适用于 Route Attribute 的情况
            request.GetRouteData().Values.TryGetValue("MS_SubRoutes", out var routes);

            return (routes as System.Web.Http.Routing.IHttpRouteData[])?.FirstOrDefault()?.Route?.RouteTemplate ?? "unknown";
        }

        #region Response Time

        /// <summary>
        /// 开始记录响应时间
        /// </summary>
        /// <param name="request"></param>
        /// <param name="routeTemplate"></param>
        private void StartRecordingResponseTime(HttpRequestMessage request)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            request.Properties.Add(API_METRICS_RESPONSE_TIME_KEY, stopwatch);
        }

        /// <summary>
        /// 停止记录响应时间
        /// </summary>
        /// <param name="response"></param>
        private void EndRecordingResponseTime(string routeTemplate, HttpRequestMessage request, HttpResponseMessage response)
        {
            var stopwatch = response.RequestMessage.Properties[API_METRICS_RESPONSE_TIME_KEY] as Stopwatch;

            ApiMetrics.GetMetrics().Provider.Timer.Instance(new TimerOptions
            {
                Name = "Response Time",
                Tags = new MetricTags(
                    new string[] { "method", "route", "status" },
                    new string[] { request.Method.Method, routeTemplate, ((int)response.StatusCode).ToString() }
                    ),
                DurationUnit = TimeUnit.Milliseconds,
                RateUnit = TimeUnit.Milliseconds,
                MeasurementUnit = Unit.Requests
            }).Record(stopwatch.ElapsedMilliseconds, TimeUnit.Milliseconds);

            response.RequestMessage.Properties.Remove(API_METRICS_RESPONSE_TIME_KEY);
        }

        #endregion

    }
}