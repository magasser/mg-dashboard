using MG.Dashboard.Controller.Domain;
using MG.Dashboard.Controller.Mqtt;

namespace MG.Dashboard.Controller.Device;

public sealed class DeviceService : IDeviceService
{
    public DeviceService(ILogger<DeviceService> logger, IMqttClientService mqttClientService, string deviceId)
    {

    }

    /// <inheritdoc />
    public IObservable<DeviceState> State { get; }

    /// <inheritdoc />
    public IObservable<DeviceMode> Mode { get; }

    /// <inheritdoc />
    public IObservable<string> Messages { get; }

    /// <inheritdoc />
    public Task SendMessageAsync(string message, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}