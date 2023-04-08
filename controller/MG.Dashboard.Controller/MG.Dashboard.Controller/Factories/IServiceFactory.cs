using MG.Dashboard.Controller.Device;
using MG.Dashboard.Controller.Mqtt;

namespace MG.Dashboard.Controller.Factories;

public interface IServiceFactory
{
    IDeviceService CreateDeviceService();

    IMqttClientService CreateMqttClientService();
}
