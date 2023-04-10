﻿using MG.Dashboard.Env;
using MG.Dashboard.Mqtt.Server.Options;
using MG.Dashboard.Mqtt.Server.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using MQTTnet;

DotEnv.Load(".env");
DotEnv.Load(".env.development");

await Host.CreateDefaultBuilder(args)
          .ConfigureServices(
              (context, services) => services
                                     .Configure<MqttServerConfiguration>(
                                         context.Configuration.GetSection(MqttServerConfiguration.Key))
                                     .AddSingleton<MqttFactory>()
                                     .AddHostedService<MqttServerHostedService>())
          .Build()
          .RunAsync();