# ASP.NET Web API Sample

- .NET Framework 4.6.1
- [ASP.NET MVC 5](https://docs.microsoft.com/zh-cn/aspnet/mvc/overview/getting-started/introduction/getting-started)

## Install packages

```bash
Install-Package App.Metrics
Install-Package App.Metrics.Formatters.Prometheus
```

## Config App Metrics

如果项目中引入了依赖注入容器（例如 AutoFac），直接注册 `IMetricsRoot` ，并通过 `InitAppMetrics()` 的代码来配置 App Metrics 即可。如果没有依赖注入框架的话，可以新增一个类 `MvcMetrics` 用于初始化 App Metrics 的配置。

```csharp
    public class MvcMetrics
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
                                options.DefaultContextLabel = "MVC";
                                options.AddAppTag(Assembly.GetExecutingAssembly().GetName().Name);
                                options.AddServerTag(Environment.MachineName);

#if DEBUG
                                options.AddEnvTag("Dev");
#else
                                options.AddEnvTag("Release");
#endif

                                options.GlobalTags.Add("my_custom_tag", "MyCustomValue");
                            })
                            .Build();

            return metrics;
        }

    }

```

1. `DefaultContextLabel` 的值会成为指标的前缀，这里设置成 `MVC`，则默认所有指标都为 `mvc_` 开头
1. `AddAppTag()` 会为所有指标添加一个名为 `app` 的 tag，内容为当前程序的名称
1. `AddServerTag()` 会为所有指标添加一个名为 `server` 的 tag，内容是运行程序的机器名称
1. `AddEnvTag()` 会为所有指标添加一个名为 `env` 的 tag，用于区分运行程序的环境
1. 也可以通过 `GlobalTags` 属性，来添加自定义的 tag

如果没有依赖注入容器，还需要在 `Global.asax` 的 `Application_Start()` 中手动调用一下 `GetMetrics()` 方法以完成初始化。

```csharp
protected void Application_Start()
{
    // 省略其他内容

    ApiMetrics.GetMetrics();
}
```

