using MG.Dashboard.Controller.Domain;

namespace MG.Dashboard.Controller.Device;

public interface IDeviceService
{
    IObservable<DeviceState> State { get; }

    IObservable<DeviceMode> Mode { get; }

    IObservable<Message> DeviceMessages { get; }

    IObservable<Message> ExternalMessages { get; }

    Task SendDeviceMessageAsync(Message message, CancellationToken cancellationToken = default);

    Task StartAsync(CancellationToken cancellationToken = default);

    Task StopAsync(CancellationToken cancellationToken = default);
}
