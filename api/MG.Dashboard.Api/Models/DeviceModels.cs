using System.ComponentModel.DataAnnotations;

using MG.Dashboard.Api.Entities.Types;

namespace MG.Dashboard.Api.Models;

public sealed class DeviceModels
{
    public sealed record Device
    {
        [Required]
        public Guid Id { get; set; }

        [Required(AllowEmptyStrings = true)]
        public string Name { get; set; }

        [Required]
        public DeviceType Type { get; set; }
    }

    public sealed record Registration
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public DeviceType Type { get; set; }
    }
}