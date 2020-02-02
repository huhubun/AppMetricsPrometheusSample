using App.Metrics;
using App.Metrics.Timer;
using System;
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

            StartRecordingResponseTime(request, routeTemplate);

            var response = await base.SendAsync(request, cancellationToken);

            RecordStatusCode(request, routeTemplate, response);
            EndRecordingResponseTime(response);

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
        private void StartRecordingResponseTime(HttpRequestMessage request, string routeTemplate)
        {
            var requestTimer = ApiMetrics.GetMetrics().Measure.Timer.Time(new TimerOptions
            {
                Name = "Response Time",
                Tags = new MetricTags(
                    new string[] { "method", "route" },
                    new string[] { request.Method.Method, routeTemplate }
                    ),
                DurationUnit = TimeUnit.Milliseconds,
                RateUnit = TimeUnit.Milliseconds,
                MeasurementUnit = Unit.Requests
            });

            request.Properties.Add(API_METRICS_RESPONSE_TIME_KEY, requestTimer);
        }

        /// <summary>
        /// 停止记录响应时间
        /// </summary>
        /// <param name="response"></param>
        private void EndRecordingResponseTime(HttpResponseMessage response)
        {
            var responseTimer = response.RequestMessage.Properties[API_METRICS_RESPONSE_TIME_KEY];

            using (responseTimer as IDisposable) { }

            response.RequestMessage.Properties.Remove(API_METRICS_RESPONSE_TIME_KEY);
        }

        #endregion

        #region Status Code

        /// <summary>
        /// 记录 HTTP 响应的状态码
        /// </summary>
        /// <param name="request"></param>
        /// <param name="routeTemplate"></param>
        /// <param name="response"></param>
        private void RecordStatusCode(HttpRequestMessage request, string routeTemplate, HttpResponseMessage response)
        {
            ApiMetrics.GetMetrics().Measure.Counter.Increment(new App.Metrics.Counter.CounterOptions
            {
                Name = "Response Status Code",
                Tags = new MetricTags(
                new string[] { "method", "route", "status_code" },
                new string[] { request.Method.Method, routeTemplate, response.StatusCode.ToString() }
                )
            });
        }

        #endregion

    }
}