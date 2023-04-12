using MG.Dashboard.Api.Models;

namespace MG.Dashboard.Api.Services;

public interface IDeviceService
{
    Task<bool> HasAccessAsync(Guid deviceId, Guid userId);

    Task<DeviceModels.Device?> GetByIdAsync(Guid id);

    Task<IReadOnlyList<DeviceModels.Device>> GetByUserIdAsync(Guid userId);

    Task<IReadOnlyList<DeviceModels.Device>> GetAllAsync();

    Task<DeviceModels.Device> RegisterAsync(DeviceModels.Registration registration, Guid userId);
}