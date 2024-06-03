using JadehRo.Api.Infrastructure.Api;
using JadehRo.Common.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JadehRo.Api.Controllers;

[ApiController]
[ApiResultFilter]
[Authorize]
[Route("api/[controller]")]
public class BaseController : ControllerBase
{
    public bool UserIsAuthenticated => HttpContext.User.Identity!.IsAuthenticated;
    public long? UserId => UserIsAuthenticated ? long.Parse(HttpContext.User.Identity!.GetUserId()) : null;
}