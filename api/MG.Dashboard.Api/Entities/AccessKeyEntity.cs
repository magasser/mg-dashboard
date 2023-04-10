namespace MG.Dashboard.Api.Entities;

public class AccessKeyEntity
{
    public int Id { get; set; }

    public string? Key { get; set; }

    public virtual ICollection<UserEntity> Users { get; } = new List<UserEntity>();
}