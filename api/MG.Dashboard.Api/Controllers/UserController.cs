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
public class UserController : ControllerBase
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
        var identification = await _userService.LoginAsync(credentials).ConfigureAwait(false);

        if (identification is null)
        {
            return Unauthorized();
        }

        return Ok(identification);
    }

    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost("user/register")]
    public async Task<ActionResult<UserModels.Identification>> Register([FromBody] UserModels.Registration registration)
    {
        var identification = await _userService.RegisterAsync(registration).ConfigureAwait(false);
        if (identification is null)
        {
            return BadRequest();
        }

        return CreatedAtAction(nameof(GetById), new { id = identification.Id }, identification);
    }

    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("user/{id}")]
    public async Task<ActionResult<UserModels.User>> GetById([Required] Guid id)
    {
        var user = await _userService.GetByIdAsync(id).ConfigureAwait(false);

        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }
}