using MG.Dashboard.Api.Entities.Types;
using MG.Dashboard.Api.Models;

namespace MG.Dashboard.Api.Services;

public interface IUserService
{
    Task<UserRole?> GetRoleAsync(Guid id);

    Task<UserModels.User?> GetByIdAsync(Guid id);

    Task<UserModels.Identification?> LoginAsync(UserModels.Credentials credentials);

    Task<UserModels.Identification?> RegisterAsync(UserModels.Registration registration);
}