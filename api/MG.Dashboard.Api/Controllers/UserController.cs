using System.ComponentModel.DataAnnotations;

using MG.Dashboard.Api.Models;
using MG.Dashboard.Api.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace MG.Dashboard.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/user")]
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
    [HttpPost("signin")]
    public async Task<ActionResult<UserModels.SignInResponse>> SignIn(
        [Required] [FromBody] UserModels.SignInRequest req)
    {
        var res = await _userService.SignInAsync(req);

        if (res is null)
        {
            return Unauthorized();
        }

        return Ok(res);
    }

    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost("signup")]
    public async Task<ActionResult<UserModels.SignInResponse>> SignUp([FromBody] UserModels.SignUpRequest req)
    {
        var res = await _userService.SignUpAsync(req);
        if (res is null)
        {
            return BadRequest();
        }

        return CreatedAtAction(nameof(GetById), new { id = res.Id }, res);
    }

    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{id}")]
    public async Task<ActionResult<UserModels.UserResponse>> GetById([Required] Guid id)
    {
        var user = await _userService.GetByIdAsync(id);

        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }
}