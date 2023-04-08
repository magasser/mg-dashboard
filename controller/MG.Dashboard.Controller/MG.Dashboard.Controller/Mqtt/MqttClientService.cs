using System.Reactive.Linq;
using System.Reactive.Subjects;

using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Packets;

namespace MG.Dashboard.Controller.Mqtt;

public sealed class MqttClientService : IMqttClientService, IDisposable
{
    private readonly ILogger<MqttClientService> _logger;
    private readonly IManagedMqttClient _client;
    private readonly ManagedMqttClientOptions _clientOptions;

    private readonly BehaviorSubject<bool> _isConnectedSubject;
    private readonly Dictionary<string, List<ReplaySubject<string>>> _topicSubscriptions;

    public MqttClientService(
        ILogger<MqttClientService> logger,
        IManagedMqttClient client,
        ManagedMqttClientOptions clientOptions)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _clientOptions = clientOptions ?? throw new ArgumentNullException(nameof(clientOptions));

        _isConnectedSubject = new BehaviorSubject<bool>(false);
        _topicSubscriptions = new Dictionary<string, List<ReplaySubject<string>>>();

        IsConnected = _isConnectedSubject.AsObservable();

        _client.ConnectionStateChangedAsync += OnConnectionStateChangedAsync;
        _client.ApplicationMessageReceivedAsync += OnApplicationMessageReceivedAsync;
    }

    /// <inheritdoc />
    public IObservable<bool> IsConnected { get; }

    /// <inheritdoc />
    public void Dispose()
    {
        _client.ConnectionStateChangedAsync -= OnConnectionStateChangedAsync;
        _client.Dispose();
        _isConnectedSubject.Dispose();

        foreach (var subscription in _topicSubscriptions.Values.SelectMany(l => l))
        {
            subscription.Dispose();
        }
    }

    /// <inheritdoc />
    public IObservable<string> SubscribeTopic(string topic)
    {
        if (string.IsNullOrWhiteSpace(topic))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(topic));
        }

        if (!_topicSubscriptions.ContainsKey(topic))
        {
            _topicSubscriptions[topic] = new List<ReplaySubject<string>>();
        }

        var subject = new ReplaySubject<string>(bufferSize: 1);

        _topicSubscriptions[topic].Add(subject);

        _ = _client.SubscribeAsync(CreateTopicFilters(topic));

        return subject.AsObservable();
    }

    /// <inheritdoc />
    public Task PublishAsync(string topic, string payload, CancellationToken cancellationToken = default)
    {
        if (payload is null)
        {
            throw new ArgumentNullException(nameof(payload));
        }

        if (string.IsNullOrWhiteSpace(topic))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(topic));
        }

        return EnqueueAsync(topic, payload);
    }

    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        await _client.StartAsync(_clientOptions).ConfigureAwait(false);

        _logger.LogDebug("Started MQTT managed client.");
    }

    /// <inheritdoc />
    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        await _client.UnsubscribeAsync(new[] { DeviceTopics.Message }).ConfigureAwait(false);

        await _client.StopAsync().ConfigureAwait(false);

        _logger.LogDebug("Stopped MQTT managed client.");
    }

    private Task OnApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
    {
        var topic = arg.ApplicationMessage.Topic;

        if (_topicSubscriptions.ContainsKey(topic))
        {
            var payload = arg.ApplicationMessage.ConvertPayloadToString();

            foreach (var subject in _topicSubscriptions[topic])
            {
                subject.OnNext(payload);
            }
        }

        return Task.CompletedTask;
    }

    private Task OnConnectionStateChangedAsync(EventArgs arg)
    {
        _isConnectedSubject.OnNext(_client.IsConnected);

        return Task.CompletedTask;
    }

    private async Task EnqueueAsync(string topic, string payload)
    {
        await _client.EnqueueAsync(topic, payload).ConfigureAwait(false);

        _logger.LogDebug("Enqueued message for topic '{Topic}' with payload '{Payload}.'", topic, payload);
    }

    private ICollection<MqttTopicFilter> CreateTopicFilters(params string[] topics)
    {
        var builder = new MqttTopicFilterBuilder();
        return topics.Select(topic => builder.WithTopic(topic).WithNoLocal().Build()).ToList();
    }
}
