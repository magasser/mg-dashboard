using Microsoft.AspNetCore.Mvc;

namespace MG.Dashboard.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/device")]
[Produces("application/json")]
public class DeviceController
{
}
