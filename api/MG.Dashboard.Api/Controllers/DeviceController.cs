using System.ComponentModel.DataAnnotations;

using MG.Dashboard.Api.Extensions;
using MG.Dashboard.Api.Models;
using MG.Dashboard.Api.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MG.Dashboard.Api.Controllers;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}")]
[Produces("application/json")]
public class DeviceController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IDeviceService _deviceService;
    private readonly ITokenService _tokenService;

    public DeviceController(IUserService userService, IDeviceService deviceService, ITokenService tokenService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _deviceService = deviceService ?? throw new ArgumentNullException(nameof(deviceService));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("device/{id}")]
    public async Task<ActionResult<DeviceModels.Device>> GetById([Required] Guid id)
    {
        if (!TryGetUserIdFromRequest(Request, out var userId))
        {
            Unauthorized();
        }

        if (!await _deviceService.HasAccessAsync(id, userId!.Value).ConfigureAwait(false))
        {
            return Forbid();
        }

        var device = await _deviceService.GetByIdAsync(id).ConfigureAwait(false);

        if (device is null)
        {
            return NotFound();
        }

        return Ok(device);
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPost("device/register")]
    public async Task<ActionResult<DeviceModels.Device>> Register(
        [Required] [FromBody] DeviceModels.Registration registration)
    {
        if (!TryGetUserIdFromRequest(Request, out var userId))
        {
            return Unauthorized();
        }

        var device = await _deviceService.RegisterAsync(registration, userId!.Value).ConfigureAwait(false);

        if (device is null)
        {
            return BadRequest();
        }

        return CreatedAtAction(nameof(GetById), new { id = device.Id }, device);
    }

    private bool TryGetUserIdFromRequest(HttpRequest request, out Guid? userId)
    {
        userId = null;
        if (!request.TryGetToken(out var token))
        {
            return false;
        }

        userId = _tokenService.GetUserId(token!);
        return true;
    }
}