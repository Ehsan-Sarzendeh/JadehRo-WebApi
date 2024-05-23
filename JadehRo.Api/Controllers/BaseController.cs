using Microsoft.AspNetCore.Mvc;

namespace JadehRo.Api.Controllers;

[ApiController]
[ApiResultFilter]
[Route("api/[controller]")]
public class BaseController : ControllerBase
{
    public bool UserIsAuthenticated => HttpContext.User.Identity!.IsAuthenticated;
    public long? UserId => UserIsAuthenticated ? long.Parse(HttpContext.User.Identity.GetUserId()) : null;
}