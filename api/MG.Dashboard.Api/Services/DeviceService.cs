using MG.Dashboard.Api.Context;
using MG.Dashboard.Api.Entities;
using MG.Dashboard.Api.Entities.Types;
using MG.Dashboard.Api.Models;

using Microsoft.EntityFrameworkCore;

namespace MG.Dashboard.Api.Services;

public sealed class DeviceService : IDeviceService
{
    private readonly MgDashboardContext _context;

    public DeviceService(MgDashboardContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public async Task<bool> HasAccessAsync(Guid deviceId, Guid userId)
    {
        return await _context.UserDevices.Where(ud => ud.DeviceId == deviceId)
                             .AnyAsync(ud => ud.UserId == userId)
                             .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<DeviceModels.Device?> GetByIdAsync(Guid id)
    {
        var device = await _context.Devices
                                   .FindAsync(id)
                                   .ConfigureAwait(false);

        return device is not null
                   ? new DeviceModels.Device
                   {
                       Id = device.Id,
                       Name = device.Name
                   }
                   : null;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<DeviceModels.Device>> GetByUserIdAsync(Guid userId)
    {
        return await _context.UserDevices
                             .Where(ud => ud.UserId == userId)
                             .Include(ud => ud.Device)
                             .Select(
                                 ud => new DeviceModels.Device
                                 {
                                     Id = ud.DeviceId,
                                     Name = ud.Device.Name
                                 })
                             .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<DeviceModels.Device>> GetAllAsync()
    {
        return await _context.Devices
                             .Select(
                                 d => new DeviceModels.Device
                                 {
                                     Id = d.Id,
                                     Name = d.Name
                                 })
                             .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<DeviceModels.Device> RegisterAsync(DeviceModels.Registration registration, Guid userId)
    {
        var accessKey = new AccessKey
        {
            Key = Guid.NewGuid(),
            Type = KeyType.Device
        };

        await _context.AccessKeys.AddAsync(accessKey).ConfigureAwait(false);

        await _context.SaveChangesAsync().ConfigureAwait(false);

        var device = new Device
        {
            Id = Guid.NewGuid(),
            Name = registration.Name,
            OwnerId = userId,
            Type = registration.Type,
            AccessKeyId = accessKey.Id
        };

        var userDevice = new UserDevice
        {
            DeviceId = device.Id,
            UserId = userId,
            Role = DeviceRole.Owner
        };

        await _context.Devices.AddAsync(device).ConfigureAwait(false);
        await _context.UserDevices.AddAsync(userDevice).ConfigureAwait(false);

        await _context.SaveChangesAsync().ConfigureAwait(false);

        return new DeviceModels.Device
        {
            Id = device.Id,
            Name = device.Name,
            Type = device.Type
        };
    }
}