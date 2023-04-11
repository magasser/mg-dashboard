using MG.Dashboard.Api.Models;

namespace MG.Dashboard.Api.Services;

public interface IUserService
{
    Task<UserModels.UserResponse?> GetByIdAsync(Guid id);

    Task<UserModels.SignInResponse?> SignInAsync(UserModels.SignInRequest req);

    Task<UserModels.SignInResponse?> SignUpAsync(UserModels.SignUpRequest req);
}
