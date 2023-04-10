namespace MG.Dashboard.Api.Entities;

public class DeviceEntity
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public int Type { get; set; }

    public virtual ICollection<UserDeviceEntity> UserDevices { get; } = new List<UserDeviceEntity>();
}