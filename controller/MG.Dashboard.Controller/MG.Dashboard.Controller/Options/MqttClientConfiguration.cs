namespace MG.Dashboard.Controller.Options;

public sealed record MqttClientConfiguration
{
    public const string Key = "MqttClientConfiguration";

    public string Host { get; set; }

    public int Port { get; set; }
}
