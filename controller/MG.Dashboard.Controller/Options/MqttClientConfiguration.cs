namespace MG.Dashboard.Controller.Options;

public sealed record MqttClientConfiguration
{
    public const string Key = "MqttClientConfiguration";

    public string ClientId { get; init; }

    public int ReconnectDelay { get; init; } = 5;
}
