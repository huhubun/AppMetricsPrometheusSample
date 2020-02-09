using App.Metrics;
using App.Metrics.Gauge;
using App.Metrics.Timer;
using System;
using System.Diagnostics;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MvcSample
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var metrics = MvcMetrics.GetMetrics();

            var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            metrics.Measure.Gauge.SetValue(new GaugeOptions
            {
                Name = "Boot Time Seconds"
            }, unixTimestamp);
        }

        private const string MVC_METRICS_RESPONSE_TIME_KEY = "__MVC.ResponseTime__";

        protected void Application_BeginRequest()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            HttpContext.Current.Items.Add(MVC_METRICS_RESPONSE_TIME_KEY, stopwatch);
        }

        protected void Application_EndRequest()
        {
            var routeValues = Request.RequestContext.RouteData.Values;

            var items = HttpContext.Current.Items;
            if (items.Contains(MVC_METRICS_RESPONSE_TIME_KEY))
            {
                var controller = GetCurrentController(routeValues);
                var action = GetCurrentAction(routeValues);

                if (controller == "metrics" && action == "getmetrics")
                {
                    return;
                }

                var stopwatch = items[MVC_METRICS_RESPONSE_TIME_KEY] as Stopwatch;

                var requestTimer = MvcMetrics.GetMetrics().Provider.Timer.Instance(new TimerOptions
                {
                    Name = "Response Time",
                    Tags = new MetricTags(
                        new string[] { "method", "controller", "action", "status" },
                        new string[] { Request.HttpMethod, controller, action, Response.StatusCode.ToString() }
                    ),
                    DurationUnit = TimeUnit.Milliseconds,
                    RateUnit = TimeUnit.Milliseconds,
                    MeasurementUnit = Unit.Requests
                });

                requestTimer.Record(stopwatch.ElapsedMilliseconds, TimeUnit.Milliseconds);
            }
        }

        protected void Application_Error()
        {
            var routeValues = Request.RequestContext.RouteData.Values;

            var controller = GetCurrentController(routeValues);
            var action = GetCurrentAction(routeValues);

            if (controller == "metrics" && action == "getmetrics")
            {
                return;
            }

            var exception = Server.GetLastError();

            MvcMetrics.GetMetrics().Measure.Counter.Increment(new App.Metrics.Counter.CounterOptions
            {
                Name = "Exception Count",
                Tags = new MetricTags(
                    new string[] { "method", "controller", "action", "exception" },
                    new string[] { Request.HttpMethod, controller, action, exception.GetType().FullName }
                )
            });
        }

        private string GetCurrentController(RouteValueDictionary routeValues)
        {
            object controller;
            string controllerStr = null;

            if (routeValues.TryGetValue("controller", out controller))
            {
                controllerStr = controller.ToString().ToLowerInvariant();
            }

            return controllerStr;
        }

        private string GetCurrentAction(RouteValueDictionary routeValues)
        {
            object action;
            string actionStr = null;

            if (routeValues.TryGetValue("action", out action))
            {
                actionStr = action.ToString().ToLowerInvariant();
            }

            return actionStr;
        }
    }
}
