using MG.Dashboard.Controller.Device;

namespace MG.Dashboard.Controller;

public sealed class ControlService : IControlService, IHostedService
{
    private readonly ILogger<ControlService> _logger;
    private readonly IDeviceService _deviceService;

    public ControlService(ILogger<ControlService> logger, IDeviceService deviceService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _deviceService = deviceService ?? throw new ArgumentNullException(nameof(deviceService));
    }

    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _deviceService.StartAsync(cancellationToken).ConfigureAwait(false);

        _logger.LogInformation("Started controller.");
    }

    /// <inheritdoc />
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _deviceService.StopAsync(cancellationToken).ConfigureAwait(false);

        _logger.LogInformation("Stopped controller.");
    }
}
