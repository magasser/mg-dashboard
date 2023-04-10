namespace MG.Dashboard.Api.Entities;

public class UserDeviceEntity
{
    public int Id { get; set; }

    public Guid UserId { get; set; }

    public Guid DeviceId { get; set; }

    public virtual DeviceEntity Device { get; set; } = null!;

    public virtual UserEntity User { get; set; } = null!;
}