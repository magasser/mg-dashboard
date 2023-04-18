using System.Text.Json.Serialization;

namespace MG.Dashboard.Controller.Domain.Twin;

public sealed record CarTwin : DeviceTwin
{
    public CarTwin(string id, string name)
        : base(id, name, DeviceType.Car) { }

    [JsonPropertyName("steer")]
    public int Steer { get; init; }

    [JsonPropertyName("throttle")]
    public int Throttle { get; init; }

    public override CarTwin Update(Message message)
    {
        switch (message.Type)
        {
            case Message.Types.Acknowledge when message.Parameters[0] is Message.Types.Command:
                if (message.Parameters.Length != 3)
                {
                    return this with { };
                }

                switch (message.Parameters[1])
                {
                    case Message.Commands.TurnLeft when int.TryParse(message.Parameters[2].ToString(), out var steer):
                        return this with { Steer = -steer };
                    case Message.Commands.TurnRight when int.TryParse(message.Parameters[2].ToString(), out var steer):
                        return this with { Steer = steer };
                    case Message.Commands.Forward when int.TryParse(message.Parameters[2].ToString(), out var throttle):
                        return this with { Throttle = throttle };
                    case Message.Commands.Backward
                        when int.TryParse(message.Parameters[2].ToString(), out var throttle):
                        return this with { Throttle = -throttle };

                    default:
                        return this with { };
                }

            default:
                return this with { };
        }
    }
}
