namespace MG.Dashboard.Controller.Camera;

public interface ICameraService
{
    Task StartAsync(CancellationToken cancellationToken = default);

    Task StopAsync(CancellationToken cancellationToken = default);
}
