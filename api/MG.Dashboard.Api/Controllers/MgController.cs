using MG.Dashboard.Api.Services;

using Microsoft.AspNetCore.Mvc;

namespace MG.Dashboard.Api.Controllers;

[Controller]
public class MgController : ControllerBase
{
    [NonAction]
    public virtual ActionResult FromResult(ServiceResult result)
    {
        return result.Message is not null
                   ? StatusCode((int)result.StatusCode, result.Message)
                   : StatusCode((int)result.StatusCode);
    }

    [NonAction]
    public virtual ActionResult FromResult<T>(ServiceResult<T> result)
    {
        return StatusCode((int)result.StatusCode, result.IsSuccess ? result.Value : result.Message);
    }
}