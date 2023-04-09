using System.IO.Ports;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;

using MG.Dashboard.Controller.Domain;
using MG.Dashboard.Controller.Options;

using Microsoft.Extensions.Options;

namespace MG.Dashboard.Controller.Serial;

public sealed class SerialService : ISerialService, IDisposable
{
    private readonly ILogger<SerialService> _logger;
    private readonly SerialConfiguration _configuration;
    private readonly StringBuilder _messageBuilder;
    private readonly ReplaySubject<string> _messagesSubject;
    private readonly BehaviorSubject<bool> _isConnectedSubject;
    private readonly string _separator;

    private SerialPort? _serialPort;
    private CancellationTokenSource? _watchtogCancellationSource;
    private DateTime _lastConnectionAcknowledgeReceived;

    public SerialService(ILogger<SerialService> logger, IOptions<SerialConfiguration> options)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = options?.Value ?? throw new ArgumentNullException(nameof(options));

        _separator = _configuration.MessageSeparator;

        _messageBuilder = new StringBuilder();
        _messagesSubject = new ReplaySubject<string>();
        _isConnectedSubject = new BehaviorSubject<bool>(false);
        _lastConnectionAcknowledgeReceived = DateTime.MinValue;

        IsConnected = _isConnectedSubject.AsObservable().DistinctUntilChanged();
        Messages = _messagesSubject.AsObservable();
    }

    /// <inheritdoc />
    public IObservable<bool> IsConnected { get; }

    /// <inheritdoc />
    public IObservable<string> Messages { get; }

    /// <inheritdoc />
    public Task SendMessageAsync(string message, CancellationToken cancellationToken = default)
    {
        if (_serialPort?.IsOpen is not true)
        {
            return Task.CompletedTask;
        }

        _serialPort.Write(message);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        var ports = SerialPort.GetPortNames();

        if (ports.Length > 1)
        {
            _logger.LogWarning(
                "Mutliple serial ports available for communication: {Ports}. First port will be used.",
                string.Join(',', ports));
        }
        else if (ports.Length == 0)
        {
            _logger.LogError("No serial ports available for communication.");
            return Task.CompletedTask;
        }

        _serialPort = new SerialPort(ports[0], _configuration.BaudRate)
        {
            Handshake = Handshake.None,
            ReadTimeout = _configuration.ReadWriteTimeout,
            WriteTimeout = _configuration.ReadWriteTimeout,
        };

        _serialPort.DataReceived += OnDataReceived;

        try
        {
            _serialPort.Open();
        }
        catch (Exception ex)
        {
            _logger.LogError("Error when trying to open serial port: {Message}.", ex.Message);
            return Task.CompletedTask;
        }

        _lastConnectionAcknowledgeReceived = DateTime.UtcNow;
        _watchtogCancellationSource = new CancellationTokenSource();
        _ = RunConnectionWatchdogAsync(_watchtogCancellationSource.Token);

        _logger.LogDebug("Started Serial communication with port '{PortName}'.", _serialPort.PortName);

        return Task.CompletedTask;
    }

    private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        var data = _serialPort!.ReadExisting();

        HandleExistingData(data);
    }

    private void HandleExistingData(string data)
    {
        const string connectionAchknowledge = $"{Message.Types.Acknowledge}.{Message.Types.Connection}";
        if (!data.Contains(_separator))
        {
            _messageBuilder.Append(data);
            return;
        }

        var parts = data.Split(_separator);

        _messageBuilder.Append(parts[0]);

        var message = _messageBuilder.ToString();

        if (message == connectionAchknowledge)
        {
            _lastConnectionAcknowledgeReceived = DateTime.UtcNow;
        }
        else
        {
            _messagesSubject.OnNext(message);
        }

        _messageBuilder.Clear();

        _messageBuilder.Append(parts[1]);
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        if (_serialPort is not null)
        {
            _serialPort.DataReceived -= OnDataReceived;
        }

        _watchtogCancellationSource?.Cancel();

        _serialPort?.Close();

        _logger.LogDebug("Stopped Serial communication with port '{PortName}'.", _serialPort?.PortName);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _serialPort?.Dispose();
        _messagesSubject.Dispose();
        _watchtogCancellationSource?.Dispose();
    }

    private async Task RunConnectionWatchdogAsync(CancellationToken cancellationToken)
    {
        var connectionTimeout = TimeSpan.FromMilliseconds(_configuration.ConnectionTimeout);
        var connectionCheckDelay = TimeSpan.FromMilliseconds(_configuration.ConnectionTimeout / 10);

        while (!cancellationToken.IsCancellationRequested)
        {
            await SendMessageAsync(Message.Types.Connection, cancellationToken).ConfigureAwait(false);

            await Task.Delay(connectionCheckDelay).ConfigureAwait(false);

            var isConnected = _lastConnectionAcknowledgeReceived + connectionTimeout > DateTime.UtcNow;

            _isConnectedSubject.OnNext(isConnected);
        }
    }
}
