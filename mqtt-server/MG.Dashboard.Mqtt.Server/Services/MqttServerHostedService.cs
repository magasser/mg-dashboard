﻿using Microsoft.Extensions.Hosting;
using MQTTnet;
using MQTTnet.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;
using MG.Dashboard.Mqtt.Server.Options;
using MQTTnet.Protocol;

namespace MG.Dashboard.Mqtt.Server.Services;
internal sealed class MqttServerHostedService : IHostedService
{
    private readonly MqttFactory _factory;
    private readonly MqttServer _server;
    private readonly string _userName;
    private readonly string _password;

    public MqttServerHostedService(IOptions<MqttServerConfiguration> options, MqttFactory factory)
    {
        if (options?.Value is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        _factory= factory ?? throw new ArgumentNullException(nameof(factory));

        _userName = Environment.GetEnvironmentVariable("MQTT_SERVER_USER")!;
        _password = Environment.GetEnvironmentVariable("MQTT_SERVER_PASS")!;

        var serverOptions = _factory.CreateServerOptionsBuilder()
                              .WithDefaultEndpoint()
                              .WithDefaultEndpointPort(options.Value.Port)
                              .Build();

        _server = _factory.CreateMqttServer(serverOptions);

        _server.ValidatingConnectionAsync += ValidateConnection;
    }

    private Task ValidateConnection(ValidatingConnectionEventArgs arg)
    {
        if (arg.UserName != _userName || arg.Password != _password)
        {
            arg.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _server.StartAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _server.StopAsync().ConfigureAwait(false);
    }
}
