using System.Reactive.Disposables;

using MG.Dashboard.Controller.Device;

namespace MG.Dashboard.Controller;

public sealed class ControlService : IControlService, IHostedService, IDisposable
{
    private readonly ILogger<ControlService> _logger;
    private readonly IDeviceService _deviceService;
    private readonly CompositeDisposable _subscriptions;

    public ControlService(ILogger<ControlService> logger, IDeviceService deviceService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _deviceService = deviceService ?? throw new ArgumentNullException(nameof(deviceService));

        _subscriptions = new CompositeDisposable();
    }

    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _deviceService.StartAsync(cancellationToken).ConfigureAwait(false);

        _subscriptions.Add(
            _deviceService.ExternalMessages.Subscribe(
                async message => await _deviceService.SendDeviceMessageAsync(message)));

        _logger.LogInformation("Started controller.");
    }

    /// <inheritdoc />
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _deviceService.StopAsync(cancellationToken).ConfigureAwait(false);

        _subscriptions.Clear();

        _logger.LogInformation("Stopped controller.");
    }

    /// <inheritdoc />
    public void Dispose() => _subscriptions.Dispose();
}
