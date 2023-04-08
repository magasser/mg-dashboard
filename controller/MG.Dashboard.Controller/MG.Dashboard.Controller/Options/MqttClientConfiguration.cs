namespace MG.Dashboard.Controller.Options;

public sealed record MqttClientConfiguration
{
    public const string Key = "MqttClientConfiguration";

    public string Host { get; init; }

    public int Port { get; init; }

    public int ReconnectDelay { get; init; } = 5;
}