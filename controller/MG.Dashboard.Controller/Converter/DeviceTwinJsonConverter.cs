using System.Text.Json;
using System.Text.Json.Serialization;

using MG.Dashboard.Controller.Domain;
using MG.Dashboard.Controller.Domain.Twin;

namespace MG.Dashboard.Controller.Converter;

public class DeviceTwinJsonConverter : JsonConverter<DeviceTwin>
{
    /// <inheritdoc />
    public override DeviceTwin? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, DeviceTwin value, JsonSerializerOptions options)
    {
        switch (value)
        {
            case CarTwin carTwin:
                JsonSerializer.Serialize(writer, carTwin, carTwin.GetType(), options);
                break;
            case BoatTwin boatTwin:
                JsonSerializer.Serialize(writer, boatTwin, boatTwin.GetType(), options);
                break;
            case DroneTwin droneTwin:
                JsonSerializer.Serialize(writer, droneTwin, droneTwin.GetType(), options);
                break;

            default:
                throw new NotSupportedException($"The device twin type '{value.GetType().Name}' is not supported.");
        }
    }
}
