using MG.Dashboard.Controller.Device;
using MG.Dashboard.Controller.Mqtt;
using MG.Dashboard.Controller.Options;

using Microsoft.Extensions.Options;

using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;

using Serilog;

await Host.CreateDefaultBuilder()
          .ConfigureAppConfiguration(
              config => config.AddJsonFile("appsettings.json", optional: false)
                              .AddJsonFile("appsettings.development.json", optional: true))
          .ConfigureServices(
              (context, services) => services
                                     .Configure<DeviceConfiguration>(
                                         context.Configuration.GetSection(DeviceConfiguration.Key))
                                     .Configure<MqttClientConfiguration>(
                                         context.Configuration.GetSection(MqttClientConfiguration.Key))
                                     .AddSingleton<IDeviceService>(
                                         provider =>
                                         {
                                             var deviceConfiguration = provider
                                                                       .GetRequiredService<
                                                                           IOptions<DeviceConfiguration>>()
                                                                       .Value;

                                             return new DeviceService(
                                                 provider.GetRequiredService<ILogger<DeviceService>>(),
                                                 provider.GetRequiredService<IMqttClientService>(),
                                                 deviceConfiguration.Id);
                                         })
                                     .AddSingleton<IMqttClientService>(
                                         provider =>
                                         {
                                             var deviceConfiguration = provider
                                                                       .GetRequiredService<
                                                                           IOptions<DeviceConfiguration>>()
                                                                       .Value;
                                             var mqttConfiguration = provider
                                                                     .GetRequiredService<
                                                                         IOptions<MqttClientConfiguration>>()
                                                                     .Value;

                                             var clientOptions = new ManagedMqttClientOptionsBuilder()
                                                                 .WithAutoReconnectDelay(
                                                                     TimeSpan.FromSeconds(
                                                                         mqttConfiguration.ReconnectDelay))
                                                                 .WithClientOptions(
                                                                     new MqttClientOptionsBuilder()
                                                                         .WithClientId(deviceConfiguration.Id)
                                                                         .WithTcpServer(
                                                                             $"{mqttConfiguration.Host}:{mqttConfiguration.Port}")
                                                                         .Build())
                                                                 .Build();

                                             var client = new MqttFactory().CreateManagedMqttClient();

                                             return new MqttClientService(
                                                 provider.GetRequiredService<ILogger<MqttClientService>>(),
                                                 client,
                                                 clientOptions);
                                         }))
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
