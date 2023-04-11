using MG.Dashboard.Api.Entities.Types;
using System;
using System.Collections.Generic;

namespace MG.Dashboard.Api.Entities;

public partial class User
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int AccessKeyId { get; set; }

    public UserRole Role { get; set; }

    public virtual AccessKey AccessKey { get; set; } = null!;

    public virtual ICollection<Device> Devices { get; } = new List<Device>();

    public virtual ICollection<UserDevice> UserDevices { get; } = new List<UserDevice>();
}
