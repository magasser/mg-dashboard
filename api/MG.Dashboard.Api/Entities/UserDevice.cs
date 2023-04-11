using MG.Dashboard.Api.Entities.Types;
using System;
using System.Collections.Generic;

namespace MG.Dashboard.Api.Entities;

public partial class UserDevice
{
    public int Id { get; set; }

    public Guid UserId { get; set; }

    public Guid DeviceId { get; set; }

    public DeviceRole Role { get; set; }

    public virtual Device Device { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
