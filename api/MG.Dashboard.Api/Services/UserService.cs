using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

using MG.Dashboard.Api.Context;
using MG.Dashboard.Api.Entities;
using MG.Dashboard.Api.Entities.Types;
using MG.Dashboard.Api.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace MG.Dashboard.Api.Services;

public sealed class UserService : IUserService
{
    private readonly MgDashboardContext _context;

    public UserService(MgDashboardContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public async Task<UserModels.UserResponse?> GetByIdAsync(Guid id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(user => user.Id == id).ConfigureAwait(false);

        if (user is null)
        {
            return null;
        }

        return new UserModels.UserResponse
        {
            Id = user.Id,
            Name = user.Name
        };
    }

    /// <inheritdoc />
    public async Task<UserModels.SignInResponse?> SignInAsync(UserModels.SignInRequest req)
    {
        var passwordHash = CreateHash(req.Password);

        var userEntity = await _context.Users.FirstOrDefaultAsync(u => u.Name == req.Name && u.Password == passwordHash)
                                       .ConfigureAwait(false);

        return userEntity is not null
                   ? new UserModels.SignInResponse
                   {
                       Id = userEntity!.Id,
                       Token = CreateToken(req.Name)
                   }
                   : null;
    }

    /// <inheritdoc />
    public async Task<UserModels.SignInResponse?> SignUpAsync(UserModels.SignUpRequest req)
    {
        var accessKey = await _context.AccessKeys.FirstOrDefaultAsync(key => key.Key == req.AccessKey)
                                      .ConfigureAwait(false);

        if (accessKey is null || accessKey.Type is not KeyType.User and not KeyType.Admin)
        {
            return null;
        }

        var passwordHash = CreateHash(req.Password);

        var role = accessKey.Type switch
        {
            KeyType.Admin => UserRole.Admin,
            KeyType.User => UserRole.User,
            _ => throw new InvalidOperationException("The type of given access key is not valid.")
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = req.Name,
            Password = passwordHash,
            AccessKeyId = accessKey.Id,
            Role = role
        };

        await _context.Users.AddAsync(user)
                      .ConfigureAwait(false);

        await _context.SaveChangesAsync().ConfigureAwait(false);

        return new UserModels.SignInResponse
        {
            Id = user.Id,
            Token = CreateToken(user.Name)
        };
    }

    private string CreateToken(string name)
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
                    new Claim(JwtRegisteredClaimNames.Sub, name),
                    new Claim(JwtRegisteredClaimNames.Email, name),
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