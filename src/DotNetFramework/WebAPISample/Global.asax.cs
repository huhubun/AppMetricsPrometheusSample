using App.Metrics.Gauge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace WebAPISample
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var metrics = ApiMetrics.GetMetrics();

            var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            metrics.Measure.Gauge.SetValue(new GaugeOptions
            {
                Name = "Boot Time Seconds"
            }, unixTimestamp);
        }
    }
}
