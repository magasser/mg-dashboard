using MG.Dashboard.Controller.Device;
using MG.Dashboard.Controller.Mqtt;
using MG.Dashboard.Controller.Options;
using Microsoft.Extensions.Options;

using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;

namespace MG.Dashboard.Controller.Factories;

public sealed class ServiceFactory : IServiceFactory
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly MqttClientConfiguration _mqttConfiguration;
    private readonly DeviceConfiguration _deviceConfiguration;
    private readonly MqttFactory _mqttFactory;

    public ServiceFactory(
        ILoggerFactory loggerFactory,
        IOptions<MqttClientConfiguration> mqttOptions,
        IOptions<DeviceConfiguration> deviceOptions)
    {
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        _mqttConfiguration = mqttOptions?.Value ?? throw new ArgumentNullException(nameof(mqttOptions));
        _deviceConfiguration = deviceOptions?.Value ?? throw new ArgumentNullException(nameof(deviceOptions));

        _mqttFactory = new MqttFactory();
    }

    /// <inheritdoc />
    public IDeviceService CreateDeviceService()
    {
        var clientService = CreateMqttClientService();

        return new DeviceService(_loggerFactory.CreateLogger<DeviceService>(), clientService, _deviceConfiguration.Id);
    }

    /// <inheritdoc />
    public IMqttClientService CreateMqttClientService()
    {
        var clientOptions = new ManagedMqttClientOptionsBuilder()
                            .WithAutoReconnectDelay(TimeSpan.FromSeconds(_mqttConfiguration.ReconnectDelay))
                            .WithClientOptions(
                                new MqttClientOptionsBuilder()
                                    .WithClientId(_deviceConfiguration.Id)
                                    .WithTcpServer($"{_mqttConfiguration.Host}:{_mqttConfiguration.Port}")
                                    .Build())
                            .Build();

        var client = _mqttFactory.CreateManagedMqttClient();

        return new MqttClientService(_loggerFactory.CreateLogger<MqttClientService>(), client, clientOptions);
    }
}
