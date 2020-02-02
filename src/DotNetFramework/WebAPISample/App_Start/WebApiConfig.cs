using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using WebAPISample.Handlers;

namespace WebAPISample
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API 配置和服务

            // Web API 路由
            config.MapHttpAttributeRoutes();

            // 本 demo 只适用于 Route Attribute 的情况
            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);

            // Metrics Handler
            config.MessageHandlers.Add(new MetricsHandler());

        }
    }
}
