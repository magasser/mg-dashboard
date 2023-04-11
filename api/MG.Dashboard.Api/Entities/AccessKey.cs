using MG.Dashboard.Api.Entities.Types;
using System;
using System.Collections.Generic;

namespace MG.Dashboard.Api.Entities;

public partial class AccessKey
{
    public int Id { get; set; }

    public Guid Key { get; set; }

    public KeyType Type { get; set; }

    public virtual ICollection<Device> Devices { get; } = new List<Device>();

    public virtual ICollection<User> Users { get; } = new List<User>();
}
