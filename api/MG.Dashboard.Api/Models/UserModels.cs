using System.ComponentModel.DataAnnotations;

namespace MG.Dashboard.Api.Models;

public sealed class UserModels
{
    public sealed record SignInRequest
    {
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Password { get; set; }
    }

    public sealed record SignInResponse
    {
        [Required]
        public Guid Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Token { get; set; }
    }

    public sealed record SignUpRequest
    {
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Password { get; set; }

        [Required]
        public Guid AccessKey { get; set; }

    }

    public sealed record UserResponse
    {
        [Required]
        public Guid Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }
    }
}
