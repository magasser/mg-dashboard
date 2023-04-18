using MG.Dashboard.Controller.Domain;

namespace MG.Dashboard.Controller.Options;

public sealed record DeviceConfiguration
{
    public const string Key = "DeviceConfiguration";

    public string Id { get; init; }

    public string Name { get; init; }

    public DeviceType Type { get; init; }
}
