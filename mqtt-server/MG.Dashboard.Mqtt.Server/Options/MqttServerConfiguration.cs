namespace MG.Dashboard.Mqtt.Server.Options;

internal sealed class MqttServerConfiguration
{
    public const string Key = "MqttServerConfiguration";

    public int Port { get; init; } = 1883;

    public int WebSocketPort { get; init; } = 8083;
}