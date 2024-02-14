using MMALSharp;

namespace MG.Dashboard.Controller.Camera;

internal sealed class CameraService : ICameraService
{
    private readonly ILogger<CameraService> _logger;
    private readonly ICamera _camera;

    public CameraService(ILogger<CameraService> logger, ICamera camera)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _camera = camera ?? throw new ArgumentNullException(nameof(camera));
    }

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();
}
