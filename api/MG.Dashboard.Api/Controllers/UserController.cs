using System.ComponentModel.DataAnnotations;

using MG.Dashboard.Api.Models;
using MG.Dashboard.Api.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MG.Dashboard.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}")]
[Produces("application/json")]
public class UserController : MgController
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPost("user/login")]
    public async Task<ActionResult<UserModels.Identification>> Login(
        [Required] [FromBody] UserModels.Credentials credentials)
    {
        return FromResult(await _userService.LoginAsync(credentials).ConfigureAwait(false));
    }

    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPost("user/register")]
    public async Task<ActionResult<UserModels.Identification>> Register([FromBody] UserModels.Registration registration)
    {
        return FromResult(await _userService.RegisterAsync(registration).ConfigureAwait(false));
    }

    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("user/{id}")]
    public async Task<ActionResult<UserModels.User>> GetById([Required] Guid id)
    {
        return FromResult(await _userService.GetByIdAsync(id).ConfigureAwait(false));
    }
}