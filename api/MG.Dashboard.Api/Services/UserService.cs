using System.Net;
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
    public async Task<ServiceResult<UserRole>> GetRoleAsync(Guid id)
    {
        var user = await _context.Users
                                 .FindAsync(id)
                                 .ConfigureAwait(false);

        return user is not null
                   ? ServiceResult.Success(user.Role)
                   : ServiceResult.Failure<UserRole>(HttpStatusCode.NotFound, "User for id not found.");
    }

    /// <inheritdoc />
    public async Task<ServiceResult<UserModels.User>> GetByIdAsync(Guid id)
    {
        var user = await _context.Users
                                 .FindAsync(id)
                                 .ConfigureAwait(false);

        if (user is null)
        {
            return null;
        }

        return user is not null
                   ? ServiceResult.Success(
                       new UserModels.User
                       {
                           Id = user.Id,
                           Name = user.Name
                       })
                   : ServiceResult.Failure<UserModels.User>(HttpStatusCode.NotFound, "User for id not found.");
    }

    /// <inheritdoc />
    public async Task<ServiceResult<UserModels.Identification>> LoginAsync(UserModels.Credentials credentials)
    {
        var passwordHash = CreateHash(credentials.Password);

        var data = await _context.Users
                                 .Where(u => u.Name == credentials.Name && u.Password == passwordHash)
                                 .Select(u => new { u.Id })
                                 .FirstOrDefaultAsync()
                                 .ConfigureAwait(false);

        return data is not null
                   ? ServiceResult.Success(
                       new UserModels.Identification
                       {
                           Id = data!.Id,
                           Token = _tokenService.CreateToken(credentials.Name, data.Id)
                       })
                   : ServiceResult.Failure<UserModels.Identification>(
                       HttpStatusCode.Unauthorized,
                       "Invalid user credentials.");
    }

    /// <inheritdoc />
    public async Task<ServiceResult<UserModels.Identification>> RegisterAsync(UserModels.Registration registration)
    {
        var key = await _context.AccessKeys
                                .FirstOrDefaultAsync(key => key.Key == registration.AccessKey)
                                .ConfigureAwait(false);

        if (key is null)
        {
            return ServiceResult.Failure<UserModels.Identification>(HttpStatusCode.NotFound, "Access key not found.");
        }

        if (key.Type is not KeyType.User and not KeyType.Admin)
        {
            return ServiceResult.Failure<UserModels.Identification>(
                HttpStatusCode.Unauthorized,
                "Invalid access key for user registration.");
        }

        var passwordHash = CreateHash(registration.Password);

        var role = key.Type switch
        {
            KeyType.Admin => UserRole.Admin,
            KeyType.User => UserRole.User,
            _ => throw new InvalidOperationException($"The given access '{key.Type}' is not valid.")
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

        try
        {
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }
        catch (DbUpdateException ex)
        {
            return ServiceResult.Failure<UserModels.Identification>(HttpStatusCode.Conflict, ex.Message);
        }

        return ServiceResult.Success(
            HttpStatusCode.Created,
            new UserModels.Identification
            {
                Id = user.Id,
                Token = _tokenService.CreateToken(registration.Name, user.Id)
            });
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