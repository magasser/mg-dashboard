using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using MG.Dashboard.Api.Entities.Types;

namespace MG.Dashboard.Api.Entities;

public class AccessKey
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public Guid Key { get; set; }

    public KeyType Type { get; set; }

    public virtual ICollection<Device> Devices { get; } = new List<Device>();

    public virtual ICollection<User> Users { get; } = new List<User>();
}