using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using MG.Dashboard.Api.Entities.Types;

namespace MG.Dashboard.Api.Entities;

public class UserDevice
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public Guid UserId { get; set; }

    public Guid DeviceId { get; set; }

    public DeviceRole Role { get; set; }

    public virtual Device Device { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}