using MG.Dashboard.Mqtt.Server.Options;

using Microsoft.Extensions.Options;

using MQTTnet;
using MQTTnet.Protocol;
using MQTTnet.Server;

namespace MG.Dashboard.Mqtt.Server.Services;

internal sealed class MqttServerHostedService : IHostedService
{
    private readonly MqttFactory _factory;
    private readonly MqttServer _server;
    private readonly string _userName;
    private readonly string _password;

    public MqttServerHostedService(IOptions<MqttServerConfiguration> options, MqttFactory factory, MqttServer server)
    {
        if (options?.Value is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        _factory = factory ?? throw new ArgumentNullException(nameof(factory));

        _userName = Environment.GetEnvironmentVariable("MQTT_SERVER_USER")!;
        _password = Environment.GetEnvironmentVariable("MQTT_SERVER_PASS")!;

        var serverOptions = _factory;

        _server = server;

        _server.ValidatingConnectionAsync += ValidateConnection;
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

    private Task ValidateConnection(ValidatingConnectionEventArgs arg)
    {
        if (arg.UserName != _userName || arg.Password != _password)
        {
            arg.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
        }

        return Task.CompletedTask;
    }
}