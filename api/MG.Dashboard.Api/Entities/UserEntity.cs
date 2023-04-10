namespace MG.Dashboard.Api.Entities;

public class UserEntity
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int AccessKeyId { get; set; }

    public virtual AccessKeyEntity AccessKey { get; set; } = null!;

    public virtual ICollection<UserDeviceEntity> UserDevices { get; } = new List<UserDeviceEntity>();
}