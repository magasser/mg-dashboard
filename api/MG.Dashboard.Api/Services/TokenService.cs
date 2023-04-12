using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.IdentityModel.Tokens;

namespace MG.Dashboard.Api.Services;

internal sealed class TokenService : ITokenService
{
    public const string IdType = "Id";

    private readonly JwtSecurityTokenHandler _tokenHandler;

    public TokenService()
    {
        _tokenHandler = new JwtSecurityTokenHandler();
    }

    /// <inheritdoc />
    public Guid GetUserId(string token)
    {
        var jwtToken = _tokenHandler.ReadJwtToken(token);

        var id = jwtToken.Claims.First(claim => claim.Type == IdType);

        return Guid.Parse(id.Value);
    }

    /// <inheritdoc />
    public string CreateToken(string name, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
        }

        var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
        var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");
        var key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY")!);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
                new[]
                {
                    new Claim(IdType, userId.ToString()),
                    new Claim(JwtRegisteredClaimNames.Sub, name),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha512Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}