using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
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
    public async Task<User?> SignIn(User user)
    {
        var context = new MgDashboardContext();

        var passwordHash = CreateHash(user.Password);

        var userEntity = await context.Users.FirstOrDefaultAsync(u => u.Name == user.Name && u.Password == passwordHash).ConfigureAwait(false);

        var token = userEntity is not null ? CreateToken(user) : null;

        return new User
        {
            Id = userEntity!.Id,
            Name = userEntity.Name,
            Token = token
        };
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

        var passwordHash = CreateHash(user.Password);

        await context.Users.AddAsync(
                         new Entities.UserEntity
                         {
                             Id = Guid.NewGuid(),
                             Name = user.Name,
                             Password = passwordHash,
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
        return tokenHandler.WriteToken(token);
    }

    private string CreateHash(string password)
    {
        const int keySize = 64;
        const int iterations = 333333;
        var salt = Environment.GetEnvironmentVariable("HASH_SALT");

        var key = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            Encoding.UTF8.GetBytes(salt!),
            iterations,
            HashAlgorithmName.SHA512,
            keySize);

        return Convert.ToHexString(key);
    }
}