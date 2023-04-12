using System.ComponentModel.DataAnnotations;

namespace MG.Dashboard.Api.Models;

public sealed class UserModels
{
    public sealed record Credentials
    {
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Password { get; set; }
    }

    public sealed record Registration
    {
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Password { get; set; }

        [Required]
        public Guid AccessKey { get; set; }
    }

    public sealed record Identification
    {
        [Required]
        public Guid Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Token { get; set; }
    }

    public sealed record User
    {
        [Required]
        public Guid Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }
    }
}