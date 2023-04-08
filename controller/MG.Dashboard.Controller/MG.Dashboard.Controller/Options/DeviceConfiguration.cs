namespace MG.Dashboard.Controller.Options;

public sealed record DeviceConfiguration
{
    public const string Key = "DeviceConfiguration";

    public string Id { get; init; }

    public string Name { get; init; }
}