namespace MG.Dashboard.Api.Services;

public interface ITokenService
{
    Guid GetUserId(string token);

    string CreateToken(string name, Guid userId);
}