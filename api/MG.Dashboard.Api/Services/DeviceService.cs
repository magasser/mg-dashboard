using System.Net;

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
    public async Task<ServiceResult<DeviceModels.Device>> GetByIdAsync(Guid id)
    {
        var device = await _context.Devices
                                   .FindAsync(id)
                                   .ConfigureAwait(false);

        return device is not null
                   ? ServiceResult.Success(
                       new DeviceModels.Device
                       {
                           Id = device.Id,
                           Name = device.Name,
                           Type = device.Type
                       })
                   : ServiceResult.Failure<DeviceModels.Device>(HttpStatusCode.NotFound, "User for id not found.");
    }

    /// <inheritdoc />
    public async Task<ServiceResult<IReadOnlyList<DeviceModels.Device>>> GetByUserIdAsync(Guid userId)
    {
        return ServiceResult.Success<IReadOnlyList<DeviceModels.Device>>(
            await _context.UserDevices
                          .Where(ud => ud.UserId == userId)
                          .Include(ud => ud.Device)
                          .Select(
                              ud => new DeviceModels.Device
                              {
                                  Id = ud.DeviceId,
                                  Name = ud.Device.Name,
                                  Type = ud.Device.Type
                              })
                          .ToListAsync());
    }

    /// <inheritdoc />
    public async Task<ServiceResult<IReadOnlyList<DeviceModels.Device>>> GetAllAsync()
    {
        return ServiceResult.Success<IReadOnlyList<DeviceModels.Device>>(
            await _context.Devices
                          .Select(
                              d => new DeviceModels.Device
                              {
                                  Id = d.Id,
                                  Name = d.Name,
                                  Type = d.Type
                              })
                          .ToListAsync());
    }

    /// <inheritdoc />
    public async Task<ServiceResult<DeviceModels.Device>> RegisterAsync(
        DeviceModels.Registration registration,
        Guid userId)
    {
        var accessKey = new AccessKey
        {
            Key = Guid.NewGuid(),
            Type = KeyType.Device
        };

        await _context.AccessKeys.AddAsync(accessKey).ConfigureAwait(false);

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

        try
        {
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }
        catch (DbUpdateException ex)
        {
            return ServiceResult.Failure<DeviceModels.Device>(HttpStatusCode.Conflict, ex.Message);
        }

        return ServiceResult.Success(
            HttpStatusCode.Created,
            new DeviceModels.Device
            {
                Id = device.Id,
                Name = device.Name,
                Type = device.Type
            });
    }
}