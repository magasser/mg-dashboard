namespace MG.Dashboard.Controller;

public interface IControlService
{
    Task StartAsync(CancellationToken cancellationToken = default);

    Task StopAsync(CancellationToken cancellationToken = default);
}
