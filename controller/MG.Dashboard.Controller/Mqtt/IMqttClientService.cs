namespace MG.Dashboard.Controller.Mqtt;

public interface IMqttClientService
{
    IObservable<bool> IsConnected { get; }

    IObservable<string> SubscribeTopic(string topic);

    Task PublishAsync(string topic, string payload, CancellationToken cancellationToken = default);

    Task StartAsync(CancellationToken cancellationToken = default);

    Task StopAsync(CancellationToken cancellationToken = default);
}
