using MG.Dashboard.Controller;
using MG.Dashboard.Controller.Camera;
using MG.Dashboard.Controller.Device;
using MG.Dashboard.Controller.Mqtt;
using MG.Dashboard.Controller.Options;
using MG.Dashboard.Controller.Serial;
using MG.Dashboard.Env;

using MMALSharp;

using Serilog;

DotEnv.Load(".env");
DotEnv.Load(".env.development");

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
                                     .Configure<SerialConfiguration>(
                                         context.Configuration.GetSection(SerialConfiguration.Key))
                                     .AddSingleton<IDeviceService, DeviceService>()
                                     .AddSingleton<IMqttClientService, MqttClientService>()
                                     .AddSingleton<ISerialService, SerialService>()
                                     .AddSingleton<ICameraService, CameraService>()
                                     .AddSingleton<ICamera, PiCamera>()
                                     .AddHostedService<ControlService>())
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
