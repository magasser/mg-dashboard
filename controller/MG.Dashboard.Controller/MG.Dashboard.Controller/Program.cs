using MG.Dashboard.Controller.Options;

using Serilog;

await Host.CreateDefaultBuilder()
          .ConfigureHostConfiguration(configurationBuilder => { configurationBuilder.AddUserSecrets<Program>(); })
          .ConfigureServices(
              (context, services) =>
              {
                  services.Configure<MqttClientConfiguration>(
                      option =>
                      {
                          option.Host = context.Configuration["MqttClient:Host"]!;
                          option.Port = int.Parse(context.Configuration["MqttClient:Port"]!);
                      });
              })
          .ConfigureLogging(
              (context, loggingBuilder) =>
              {
                  var logger = new LoggerConfiguration()
                               .ReadFrom.Configuration(context.Configuration)
                               .Enrich.FromLogContext()
                               .CreateLogger();

                  loggingBuilder.ClearProviders()
                                .AddSerilog(logger);
              })
          .Build()
          .RunAsync();