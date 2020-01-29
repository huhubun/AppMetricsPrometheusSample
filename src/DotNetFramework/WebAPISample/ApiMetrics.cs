using App.Metrics;
using System;
using System.Reflection;

namespace WebAPISample
{
    public class ApiMetrics
    {
        private static IMetricsRoot _metrics;

        public static IMetricsRoot GetMetrics()
        {
            if (_metrics == null)
            {
                _metrics = InitAppMetrics();
            }

            return _metrics;
        }

        private static IMetricsRoot InitAppMetrics()
        {
            var metrics = new MetricsBuilder()
                            .Configuration.Configure(options =>
                            {
                                options.DefaultContextLabel = "API";
                                options.AddAppTag(Assembly.GetExecutingAssembly().GetName().Name);
                                options.AddServerTag(Environment.MachineName);

#if DEBUG
                                options.AddEnvTag("Dev");
#else
                                options.AddEnvTag("Release");
#endif

                                options.GlobalTags.Add("MyCustomTag", "MyCustonValue");
                            })
                            .Build();

            return metrics;
        }
    }
}