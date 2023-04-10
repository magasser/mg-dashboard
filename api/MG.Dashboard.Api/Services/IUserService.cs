using MG.Dashboard.Api.Models;
using System.IdentityModel.Tokens.Jwt;

namespace MG.Dashboard.Api.Services;

public interface IUserService
{
    Task<User?> Get(Guid id);

    Task<string?> SignIn(User user);

    Task<bool> SignUp(User user);
}
