using System.Text.Json.Serialization;

namespace MG.Dashboard.Api.Models;

public sealed record User
{
    [JsonPropertyName("id")]
    public Guid? Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("password")]
    public string Password { get; set; }

    [JsonPropertyName("token")]
    public string? Token { get; set; }

    [JsonPropertyName("accessKey")]
    public string? AccessKey { get; set; }
}
