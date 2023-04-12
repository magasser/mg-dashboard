using MG.Dashboard.Api.Entities.Types;

namespace MG.Dashboard.Api.Entities;

public class Device
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public Guid OwnerId { get; set; }

    public int AccessKeyId { get; set; }

    public DeviceType Type { get; set; }

    public virtual AccessKey AccessKey { get; set; } = null!;

    public virtual User Owner { get; set; } = null!;

    public virtual ICollection<UserDevice> UserDevices { get; } = new List<UserDevice>();
}