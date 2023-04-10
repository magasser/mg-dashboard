using MG.Dashboard.Api.Models;
using MG.Dashboard.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MG.Dashboard.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/user")]
[Produces("application/json")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;

    public UserController(ILogger<UserController> logger, IUserService userService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPost("signin")]
    public async Task<ActionResult<string>> SignIn([FromBody] User user)
    {
        var token = await _userService.SignIn(user);

        if (token is null)
        {
            return Unauthorized();
        }

        return Ok(token);
    }

    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost("signup")]
    public async Task<ActionResult> SignUp([FromBody] User user)
    {
        if (!await _userService.SignUp(user))
        {
            return BadRequest();
        }

        return Ok();
    }

    [Authorize]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> Get(Guid id)
    {
        var user = await _userService.Get(id);

        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }
}
