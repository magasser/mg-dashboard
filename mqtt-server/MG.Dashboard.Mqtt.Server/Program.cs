using MG.Dashboard.Env;
using MG.Dashboard.Mqtt.Server.Options;
using MG.Dashboard.Mqtt.Server.Services;

using MQTTnet;
using MQTTnet.AspNetCore;

DotEnv.Load(".env");
DotEnv.Load(".env.development");

var mqttServerConfiguration = new MqttServerConfiguration();

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.GetSection(MqttServerConfiguration.Key).Bind(mqttServerConfiguration);

builder.WebHost.ConfigureKestrel(
    options =>
    {
        options.ListenAnyIP(mqttServerConfiguration.Port, l => l.UseMqtt());
        options.ListenAnyIP(mqttServerConfiguration.WebSocketPort);
    });

builder.Services.Configure<MqttServerConfiguration>(builder.Configuration.GetSection(MqttServerConfiguration.Key))
       .AddSingleton<MqttFactory>()
       .AddHostedService<MqttServerHostedService>()
       .AddHostedMqttServer(options => options.WithDefaultEndpoint())
       .AddMqttConnectionHandler()
       .AddConnections();

var app = builder.Build();

app.UseRouting();

app.UseEndpoints(
    endpoints =>
    {
        endpoints.MapConnectionHandler<MqttConnectionHandler>(
            "/mqtt",
            options => options.WebSockets.SubProtocolSelector =
                           protocolList => protocolList.FirstOrDefault() ?? string.Empty);
    });

await app.RunAsync();