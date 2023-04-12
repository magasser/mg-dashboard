using System.Security.Cryptography;
using System.Text;

using MG.Dashboard.Api.Context;
using MG.Dashboard.Api.Entities;
using MG.Dashboard.Api.Entities.Types;
using MG.Dashboard.Api.Models;

using Microsoft.EntityFrameworkCore;

namespace MG.Dashboard.Api.Services;

public sealed class UserService : IUserService
{
    private readonly MgDashboardContext _context;
    private readonly ITokenService _tokenService;

    public UserService(MgDashboardContext context, ITokenService tokenService)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
    }

    /// <inheritdoc />
    public async Task<UserRole?> GetRoleAsync(Guid id)
    {
        var user = await _context.Users
                                 .FindAsync(id)
                                 .ConfigureAwait(false);

        if (user is null)
        {
            return null;
        }

        return user.Role;
    }

    /// <inheritdoc />
    public async Task<UserModels.User?> GetByIdAsync(Guid id)
    {
        var user = await _context.Users
                                 .FindAsync(id)
                                 .ConfigureAwait(false);

        if (user is null)
        {
            return null;
        }

        return new UserModels.User
        {
            Id = user.Id,
            Name = user.Name
        };
    }

    /// <inheritdoc />
    public async Task<UserModels.Identification?> LoginAsync(UserModels.Credentials credentials)
    {
        var passwordHash = CreateHash(credentials.Password);

        var data = await _context.Users
                                 .Where(u => u.Name == credentials.Name && u.Password == passwordHash)
                                 .Select(u => new { u.Id })
                                 .FirstOrDefaultAsync()
                                 .ConfigureAwait(false);

        return data is not null
                   ? new UserModels.Identification
                   {
                       Id = data!.Id,
                       Token = _tokenService.CreateToken(credentials.Name, data.Id)
                   }
                   : null;
    }

    /// <inheritdoc />
    public async Task<UserModels.Identification?> RegisterAsync(UserModels.Registration registration)
    {
        var key = await _context.AccessKeys
                                .FindAsync(registration.AccessKey)
                                .ConfigureAwait(false);

        if (key is null || key.Type is not KeyType.User and not KeyType.Admin)
        {
            return null;
        }

        var passwordHash = CreateHash(registration.Password);

        var role = key.Type switch
        {
            KeyType.Admin => UserRole.Admin,
            KeyType.User => UserRole.User,
            _ => throw new InvalidOperationException("The type of given access accessKey is not valid.")
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = registration.Name,
            Password = passwordHash,
            AccessKeyId = key.Id,
            Role = role
        };

        await _context.Users.AddAsync(user)
                      .ConfigureAwait(false);

        await _context.SaveChangesAsync().ConfigureAwait(false);

        return new UserModels.Identification
        {
            Id = user.Id,
            Token = _tokenService.CreateToken(registration.Name, user.Id)
        };
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