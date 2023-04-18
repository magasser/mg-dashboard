using System.ComponentModel;
using System.Text.Json.Serialization;

using MG.Dashboard.Controller.Domain.Twin;

namespace MG.Dashboard.Controller.Domain;

public abstract record DeviceTwin
{
    protected DeviceTwin(string id, string name, DeviceType type)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
        }

        if (!Enum.IsDefined(type))
        {
            throw new InvalidEnumArgumentException(nameof(type), (int)type, typeof(DeviceType));
        }

        Id = id;
        Name = name;
        Type = type;
    }

    [JsonPropertyName("id")]
    public string Id { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; }

    [JsonPropertyName("type")]
    public DeviceType Type { get; init; }

    public static DeviceTwin Create(string id, string name, DeviceType type)
    {
        return type switch
        {
            DeviceType.Car => new CarTwin(id, name),
            DeviceType.Drone => new DroneTwin(id, name),
            DeviceType.Boat => new BoatTwin(id, name),
            _ => throw new NotSupportedException($"The device type '{type}' is not supported.")
        };
    }

    public abstract DeviceTwin Update(Message message);
}
