using MG.Dashboard.Api.Entities.Types;
using MG.Dashboard.Api.Models;

namespace MG.Dashboard.Api.Services;

public interface IUserService
{
    Task<ServiceResult<UserRole>> GetRoleAsync(Guid id);

    Task<ServiceResult<UserModels.User>> GetByIdAsync(Guid id);

    Task<ServiceResult<UserModels.Identification>> LoginAsync(UserModels.Credentials credentials);

    Task<ServiceResult<UserModels.Identification>> RegisterAsync(UserModels.Registration registration);
}