using MG.Dashboard.Api.Models;

namespace MG.Dashboard.Api.Services;

public interface IDeviceService
{
    Task<bool> HasAccessAsync(Guid deviceId, Guid userId);

    Task<ServiceResult<DeviceModels.Device>> GetByIdAsync(Guid id);

    Task<ServiceResult<IReadOnlyList<DeviceModels.Device>>> GetByUserIdAsync(Guid userId);

    Task<ServiceResult<IReadOnlyList<DeviceModels.Device>>> GetAllAsync();

    Task<ServiceResult<DeviceModels.Device>> RegisterAsync(DeviceModels.Registration registration, Guid userId);
}