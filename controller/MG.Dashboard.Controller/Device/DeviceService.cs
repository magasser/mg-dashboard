using System.Reactive.Linq;

using MG.Dashboard.Controller.Domain;
using MG.Dashboard.Controller.Mqtt;
using MG.Dashboard.Controller.Options;
using MG.Dashboard.Controller.Serial;

using Microsoft.Extensions.Options;

namespace MG.Dashboard.Controller.Device;

public sealed class DeviceService : IDeviceService
{
    private readonly ILogger<DeviceService> _logger;
    private readonly DeviceConfiguration _configuration;
    private readonly ISerialService _serialService;
    private readonly IMqttClientService _mqttClientService;

    private bool _isSerialConnected;
    private bool _isMqttConnected;

    public DeviceService(
        ILogger<DeviceService> logger,
        IOptions<DeviceConfiguration> options,
        ISerialService serialService,
        IMqttClientService mqttClientService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _serialService = serialService ?? throw new ArgumentNullException(nameof(serialService));
        _mqttClientService = mqttClientService ?? throw new ArgumentNullException(nameof(mqttClientService));

        _serialService.IsConnected.Subscribe(isConnected => _isSerialConnected = isConnected);
        _mqttClientService.IsConnected.Subscribe(isConnected => _isMqttConnected = isConnected);

        DeviceMessages = _serialService.Messages
                                       .Select(Message.FromString);
        ExternalMessages = _mqttClientService.SubscribeTopic($"{_configuration.Id}{DeviceTopics.Message}")
                                             .Select(Message.FromString);
    }

    /// <inheritdoc />
    public IObservable<DeviceState> State { get; }

    /// <inheritdoc />
    public IObservable<DeviceMode> Mode { get; }

    /// <inheritdoc />
    public IObservable<Message> DeviceMessages { get; }

    /// <inheritdoc />
    public IObservable<Message> ExternalMessages { get; }

    /// <inheritdoc />
    public async Task SendDeviceMessageAsync(Message message, CancellationToken cancellationToken = default)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        if (!_isSerialConnected)
        {
            _logger.LogWarning("Failed to send message '{Message}' because serial is not connected.", message);
            return;
        }

        await _serialService.SendMessageAsync(message.ToString(), cancellationToken).ConfigureAwait(false);

        _logger.LogDebug("Send device message '{Message}' to serial.", message);
    }

    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        await _serialService.StartAsync(cancellationToken).ConfigureAwait(false);
        await _mqttClientService.StartAsync(cancellationToken).ConfigureAwait(false);

        _logger.LogDebug("Started device service.");
    }

    /// <inheritdoc />
    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        await _serialService.StopAsync(cancellationToken).ConfigureAwait(false);
        await _mqttClientService.StopAsync(cancellationToken).ConfigureAwait(false);

        _logger.LogDebug("Stopped device service.");
    }
}
