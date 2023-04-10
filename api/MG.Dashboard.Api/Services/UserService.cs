using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using MG.Dashboard.Api.Context;
using MG.Dashboard.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace MG.Dashboard.Api.Services;

public sealed class UserService : IUserService
{
    /// <inheritdoc />
    public async Task<User?> Get(Guid id)
    {
        var context = new MgDashboardContext();

        var user = await context.Users.FirstOrDefaultAsync(user => user.Id == id).ConfigureAwait(false);

        if (user is null)
        {
            return null;
        }

        return new User
        {
            Id = user.Id,
            Name = user.Name
        };
    }

    /// <inheritdoc />
    public async Task<string?> SignIn(User user)
    {
        var context = new MgDashboardContext();

        var isValidUser = await context.Users.AnyAsync(u => u.Name == user.Name && u.Password == user.Password).ConfigureAwait(false);

        return isValidUser ? CreateToken(user) : null;
    }

    /// <inheritdoc />
    public async Task<bool> SignUp(User user)
    {
        var context = new MgDashboardContext();

        var accessKey = await context.AccessKeys.FirstOrDefaultAsync(key => key.Key == user.AccessKey).ConfigureAwait(false);

        if (accessKey is null)
        {
            return false;
        }

        await context.Users.AddAsync(
                         new Entities.UserEntity
                         {
                             Id = Guid.NewGuid(),
                             Name = user.Name,
                             Password = user.Password,
                             AccessKeyId = accessKey.Id
                         })
                     .ConfigureAwait(false);

        await context.SaveChangesAsync().ConfigureAwait(false);

        return true;
    }

    private string CreateToken(User user)
    {
        var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
        var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");
        var key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY")!);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
                new[]
                {
                    new Claim("Id", Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Name),
                    new Claim(JwtRegisteredClaimNames.Email, user.Name),
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
        var jwtToken = tokenHandler.WriteToken(token);

        return jwtToken;
    }
}