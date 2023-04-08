using MG.Dashboard.Controller.Domain;

namespace MG.Dashboard.Controller.Device;

public interface IDeviceService
{
    IObservable<DeviceState> State { get; }

    IObservable<DeviceMode> Mode { get; }

    IObservable<string> Messages { get; }

    Task SendMessageAsync(string message, CancellationToken cancellationToken = default);

    Task StartAsync(CancellationToken cancellationToken = default);

    Task StopAsync(CancellationToken cancellationToken = default);
}