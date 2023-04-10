namespace MG.Dashboard.Controller.Serial;

public interface ISerialService
{
    IObservable<bool> IsConnected { get; }

    IObservable<string> Messages { get; }

    Task SendMessageAsync(string message, CancellationToken cancellationToken = default);

    Task StartAsync(CancellationToken cancellationToken = default);

    Task StopAsync(CancellationToken cancellationToken = default);
}
