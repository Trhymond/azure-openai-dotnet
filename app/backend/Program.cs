using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(s=> {
        s.AddApplicationInsightsTelemetryWorkerService();
        s.ConfigureFunctionsApplicationInsights();
        s.AddSingleton<IHttpResponderService, DefaultHttpResponderService>();
        s.Configure<LoggerFilterOptions>(options => {
            LoggerFilterRule? toRemove = options.Rules.FirstOrDefault(rule => rule?.ProviderName
                == "Microsoft.Extensions.Logging.ApplicationInsigthsLoggerProvider");
            
            if(toRemove is not null) {
                options.Rules.Remove(toRemove);
            }
        });
        s.AddLogging();
        s.AddDistributedMemoryCache();
        s.AddAzureServices();
        s.AddMemoryStore();
    }).Build();


await host.RunAsync();
