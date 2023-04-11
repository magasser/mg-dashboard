using MG.Dashboard.Api.Entities.Types;
using System;
using System.Collections.Generic;

namespace MG.Dashboard.Api.Entities;

public partial class Device
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
